using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace DrDocx.WordDocEditing
{
    // Credit: http://www.ericwhite.com/blog/search-and-replace-text-in-an-open-xml-wordprocessingml-document/
    // for the algorithm and much of the code.
    internal class WordFindAndReplace
    {
        public WordFindAndReplace(WordprocessingDocument wordDoc, bool matchCase)
        {
            WordDoc = wordDoc;
            MatchCase = matchCase;
            if (HasTrackedRevisions())
                throw new SearchAndReplaceException(
                    "Search and replace will not work with documents that contain revision tracking.");
            var xmlDoc = GetXmlDocument(wordDoc.MainDocumentPart.DocumentSettingsPart);
            Nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            Nsmgr.AddNamespace("w",
                "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
        }

        private WordprocessingDocument WordDoc { get; set; }
        private XmlNamespaceManager Nsmgr { get; set; }
        private string WordNamespace { get; set; }
        private Dictionary<string, string> CurrentFindAndReplacePairs { get; set; }
        private bool MatchCase { get; set; }

        private IEnumerable<OpenXmlPart> DocParts
        {
            get
            {
                yield return WordDoc.MainDocumentPart;

                foreach (var part in WordDoc.MainDocumentPart.HeaderParts)
                    yield return part;

                foreach (var part in WordDoc.MainDocumentPart.FooterParts)
                    yield return part;

                if (WordDoc.MainDocumentPart.EndnotesPart != null)
                    yield return WordDoc.MainDocumentPart.EndnotesPart;

                if (WordDoc.MainDocumentPart.FootnotesPart != null)
                    yield return WordDoc.MainDocumentPart.FootnotesPart;
            }
        }

        public bool ContainsText(string text, bool matchCase)
        {
            if (matchCase)
                return DocParts.Select(part => GetXmlDocument(part).InnerText).Any(partXmlDocText => partXmlDocText.Contains(text));
            return DocParts.Select(part => GetXmlDocument(part).InnerText.ToLower()).Any(partXmlDocText => partXmlDocText.Contains(text.ToLower()));
        } 

        public void SearchAndReplace(Dictionary<string, string> matchAndReplacePairs)
        {
            CurrentFindAndReplacePairs = matchAndReplacePairs;
            foreach (var part in DocParts)
            {
                var xmlDoc = GetXmlDocument(part);
                SearchAndReplaceInXmlDocument(xmlDoc);
                PutXmlDocument(part, xmlDoc);
            }
        }

        private void SearchAndReplaceInXmlDocument(XmlDocument xmlDocument)
        {
            Nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            Nsmgr.AddNamespace("w",
                "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            var paragraphs = xmlDocument.SelectNodes("descendant::w:p", Nsmgr);
            foreach (var paragraph in paragraphs)
                SearchAndReplaceInParagraph((XmlElement) paragraph);
        }

        private void SearchAndReplaceInParagraph(XmlElement paragraph)
        {
            var xmlDoc = GetDocInfoForParagraph(paragraph);

            // Get all the text nodes of the paragraph
            XmlNodeList paragraphText = paragraph.SelectNodes("descendant::w:t", Nsmgr);

            var anyMatches = false;
            SearchAndReplaceInTextBoxes(paragraph);
            foreach (var (matchText, replaceText) in CurrentFindAndReplacePairs)
            {
                anyMatches = IsMatchInParagraph(matchText, paragraphText);
                if (anyMatches) break;
            }

            if (!anyMatches) return;
            SplitTextRunsIntoCharacterRuns(paragraph, xmlDoc);
            foreach (var (matchText, replaceText) in CurrentFindAndReplacePairs)
                SearchAndReplaceCharRunsParagraph(paragraph, matchText, replaceText, xmlDoc);
            ConsolidateRunsWithSameProperties(paragraph, xmlDoc);
        }

        private XmlDocument GetDocInfoForParagraph(XmlElement paragraph)
        {
            var xmlDoc = paragraph.OwnerDocument;
            // TODO: Figure out if these namespaces are necessary. Reeks of cargo cult programming.
            WordNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            Nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            Nsmgr.AddNamespace("w", WordNamespace);
            return xmlDoc;
        }

        private bool IsMatchInParagraph(string search, XmlNodeList paragraphText)
        {
            // First we just mash all the text together and check for a match before we go any further.
            // If we know there's no match, then we don't have to go through the effort of screwing with
            // the runs to replace text while maintaining the document's structural integrity.
            StringBuilder sb = new StringBuilder();
            foreach (XmlNode text in paragraphText)
                sb.Append(((XmlElement) text).InnerText);
            bool isMatchInParagraph = sb.ToString().Contains(search) ||
                                      !MatchCase && sb.ToString().ToUpper().Contains(search.ToUpper());
            return isMatchInParagraph;
        }

        /// <summary>
        /// Takes in a paragraph that has been chopped into character runs and replaces all string matches
        /// using the main search and replace algorithm
        /// </summary>
        private void SearchAndReplaceCharRunsParagraph(XmlElement paragraph, string search, string replace,
            XmlDocument xmlDoc)
        {
            XmlNodeList runs;
            while (true)
            {
                bool cont = false;
                runs = paragraph.SelectNodes("child::w:r", Nsmgr);
                for (int i = 0; i <= runs.Count - search.Length; i++)
                {
                    var match = IsMatchHere(search, runs, i);
                    if (!match)
                        continue;
                    ReplaceMatchedCharRunsWithText(paragraph, search, replace, xmlDoc, runs, i);
                    cont = true;
                    break;
                }

                if (!cont)
                    break;
            }
        }

        private void ReplaceMatchedCharRunsWithText(XmlElement paragraph, string search, string replace,
            XmlDocument xmlDoc, XmlNodeList runs, int i)
        {
            var runProps =
                (XmlElement) runs[i].SelectSingleNode("descendant::w:rPr", Nsmgr);
            var newRun = xmlDoc.CreateElement("w:r", WordNamespace);
            if (runProps != null)
            {
                XmlElement newRunProps = (XmlElement) runProps.CloneNode(true);
                newRun.AppendChild(newRunProps);
            }

            var newTextElement =
                xmlDoc.CreateElement("w:t", WordNamespace);
            var newText = xmlDoc.CreateTextNode(replace);
            newTextElement.AppendChild(newText);
            if (replace[0] == ' ' || replace[^1] == ' ')
            {
                var xmlSpace = xmlDoc.CreateAttribute("xml", "space",
                    "http://www.w3.org/XML/1998/namespace");
                // Let's not accidentally delete any whitespace.
                xmlSpace.Value = "preserve";
                newTextElement.Attributes.Append(xmlSpace);
            }

            newRun.AppendChild(newTextElement);
            paragraph.InsertAfter(newRun, runs[i - 1]);
            for (int c = 0; c < search.Length; ++c)
                paragraph.RemoveChild(runs[i + c]);
        }

        private bool IsMatchHere(string search, XmlNodeList runs, int i)
        {
            var match = true;
            for (int charIndex = 0; charIndex < search.Length; charIndex++)
            {
                // Look through text nodes. If we find something that doesn't match with the string
                // we're looking for, then there is no match and we break. Otherwise, keep going.
                XmlElement textElement =
                    (XmlElement) runs[i + charIndex].SelectSingleNode("child::w:t", Nsmgr);
                if (textElement == null)
                {
                    match = false;
                    break;
                }

                if (textElement.InnerText == search[charIndex].ToString())
                    continue;
                if (!MatchCase &&
                    string.Equals(textElement.InnerText, search[charIndex].ToString(),
                        StringComparison.CurrentCultureIgnoreCase))
                    continue;
                match = false;
                break;
            }

            return match;
        }

        private void SplitTextRunsIntoCharacterRuns(XmlElement paragraph,
            XmlDocument xmlDoc)
        {
            XmlNodeList runs = paragraph.SelectNodes("child::w:r", Nsmgr);
            foreach (XmlElement run in runs)
            {
                XmlNodeList childElements = run.SelectNodes("child::*", Nsmgr);
                if (childElements.Count > 0)
                {
                    XmlElement last = (XmlElement) childElements[childElements.Count - 1];
                    for (int ch = childElements.Count - 1; ch >= 0; ch--)
                    {
                        if (childElements[ch].Name == "w:rPr")
                            continue;
                        // If this run has text in it, we're going to split it up into a run for every
                        // character. Performance! Efficiency! We shall have none of it!
                        if (childElements[ch].Name == "w:t")
                            SplitSingleRunIntoCharRuns(paragraph, xmlDoc, childElements, ch, run);
                        else
                        {
                            var newRun = xmlDoc.CreateElement("w:r", WordNamespace);
                            var runProps =
                                (XmlElement) run.SelectSingleNode("child::w:rPr", Nsmgr);
                            if (runProps != null)
                            {
                                var newRunProps =
                                    (XmlElement) runProps.CloneNode(true);
                                newRun.AppendChild(newRunProps);
                            }

                            var newChildElement =
                                (XmlElement) childElements[ch].CloneNode(true);
                            newRun.AppendChild(newChildElement);
                            paragraph.InsertAfter(newRun, run);
                        }
                    }

                    paragraph.RemoveChild(run);
                }
            }
        }

        private void SplitSingleRunIntoCharRuns(XmlElement paragraph, XmlDocument xmlDoc, XmlNodeList childElements,
            int c, XmlElement run)
        {
            string textElementString = childElements[c].InnerText;
            for (int i = textElementString.Length - 1; i >= 0; --i)
            {
                XmlElement newRun =
                    xmlDoc.CreateElement("w:r", WordNamespace);
                XmlElement runProps =
                    (XmlElement) run.SelectSingleNode("child::w:rPr", Nsmgr);
                if (runProps != null)
                {
                    XmlElement newRunProps =
                        (XmlElement) runProps.CloneNode(true);
                    newRun.AppendChild(newRunProps);
                }

                XmlElement newTextElement =
                    xmlDoc.CreateElement("w:t", WordNamespace);
                XmlText newText =
                    xmlDoc.CreateTextNode(textElementString[i].ToString());
                newTextElement.AppendChild(newText);
                if (textElementString[i] == ' ')
                {
                    XmlAttribute xmlSpace = xmlDoc.CreateAttribute(
                        "xml", "space",
                        "http://www.w3.org/XML/1998/namespace");
                    xmlSpace.Value = "preserve";
                    newTextElement.Attributes.Append(xmlSpace);
                }

                newRun.AppendChild(newTextElement);
                paragraph.InsertAfter(newRun, run);
            }
        }

        private void ConsolidateRunsWithSameProperties(XmlElement paragraph, XmlDocument xmlDoc)
        {
            // Now that we've found and replaced everything we needed to, let's glue this document back together by
            // consolidating runs with identical properties into one.
            XmlNodeList children = paragraph.SelectNodes("child::*", Nsmgr);
            List<int> matchId = new List<int>();
            int consolidatedRunsToCreate = ConsolidatedRunsToCreate(children, matchId);
            for (int i = 0; i <= consolidatedRunsToCreate; ++i)
            {
                var x1 = matchId.IndexOf(i);
                var x2 = matchId.LastIndexOf(i);
                if (x1 == x2)
                    continue;
                var sb2 = new StringBuilder();
                for (int z = x1; z <= x2; ++z)
                {
                    var singleNode = ((XmlElement) children[z]).SelectSingleNode("w:t", Nsmgr);
                    if (singleNode != null)
                        sb2.Append(singleNode.InnerText);
                }

                XmlElement newRun = xmlDoc.CreateElement("w:r", WordNamespace);
                XmlElement runProps =
                    (XmlElement) children[x1].SelectSingleNode("child::w:rPr", Nsmgr);
                if (runProps != null)
                {
                    XmlElement newRunProps = (XmlElement) runProps.CloneNode(true);
                    newRun.AppendChild(newRunProps);
                }

                XmlElement newTextElement = xmlDoc.CreateElement("w:t", WordNamespace);
                XmlText newText = xmlDoc.CreateTextNode(sb2.ToString());
                newTextElement.AppendChild(newText);
                if (sb2[0] == ' ' || sb2[sb2.Length - 1] == ' ')
                {
                    XmlAttribute xmlSpace = xmlDoc.CreateAttribute(
                        "xml", "space", "http://www.w3.org/XML/1998/namespace");
                    xmlSpace.Value = "preserve";
                    newTextElement.Attributes.Append(xmlSpace);
                }

                newRun.AppendChild(newTextElement);
                paragraph.InsertAfter(newRun, children[x2]);
                for (int z = x1; z <= x2; ++z)
                    paragraph.RemoveChild(children[z]);
            }
        }

        private int ConsolidatedRunsToCreate(XmlNodeList children, List<int> matchId)
        {
            int consolidatedRunsToCreate = 0;
            for (int ch = 0; ch < children.Count; ch++)
            {
                if (ch == 0)
                {
                    matchId.Add(consolidatedRunsToCreate);
                    continue;
                }

                // If these are both text nodes and actually contain text
                if (children[ch].Name == "w:r" &&
                    children[ch - 1].Name == "w:r" &&
                    children[ch].SelectSingleNode("w:t", Nsmgr) != null &&
                    children[ch - 1].SelectSingleNode("w:t", Nsmgr) != null)
                {
                    XmlElement runProps =
                        (XmlElement) children[ch].SelectSingleNode("w:rPr", Nsmgr);
                    XmlElement lastRunProps =
                        (XmlElement) children[ch - 1].SelectSingleNode("w:rPr", Nsmgr);
                    // If only one of these runs has properties, then we can just mash them together
                    // using the properties of the one that does
                    if ((runProps == null && lastRunProps != null) ||
                        (runProps != null && lastRunProps == null))
                    {
                        matchId.Add(consolidatedRunsToCreate++);
                        continue;
                    }

                    // If both have properties, we can only consolidate them if they have the same properties.
                    if (runProps != null && runProps.InnerXml != lastRunProps.InnerXml)
                    {
                        matchId.Add(consolidatedRunsToCreate++);
                        continue;
                    }

                    matchId.Add(consolidatedRunsToCreate);
                    continue;
                }

                matchId.Add(consolidatedRunsToCreate++);
            }

            return consolidatedRunsToCreate;
        }

        private void SearchAndReplaceInTextBoxes(XmlElement paragraph)
        {
            // Text inside text boxes have to be handled separately and yet can exist inside a paragraph???
            var txbxParagraphs = paragraph.SelectNodes("descendant::w:p", Nsmgr);
            foreach (XmlElement p in txbxParagraphs)
                SearchAndReplaceInParagraph((XmlElement) p);
        }


        private static XmlDocument GetXmlDocument(OpenXmlPart part)
        {
            XmlDocument xmlDoc = new XmlDocument();
            using (var partStream = part.GetStream())
            {
                using var partXmlReader = XmlReader.Create(partStream);
                xmlDoc.Load(partXmlReader);
            }

            return xmlDoc;
        }

        private static void PutXmlDocument(OpenXmlPart part, XmlDocument xmlDoc)
        {
            using var partStream = part.GetStream(FileMode.Create, FileAccess.Write);
            using var partXmlWriter = XmlWriter.Create(partStream);
            xmlDoc.Save(partXmlWriter);
        }

        private static bool PartHasTrackedRevisions(OpenXmlPart part)
        {
            // Credit to Eric White, from whom we pinched all of this tracked revision detection code
            XmlDocument doc = GetXmlDocument(part);
            string wordNamespace =
                "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            XmlNamespaceManager nsmgr =
                new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("w", wordNamespace);
            string xpathExpression =
                "descendant::w:cellDel|" +
                "descendant::w:cellIns|" +
                "descendant::w:cellMerge|" +
                "descendant::w:customXmlDelRangeEnd|" +
                "descendant::w:customXmlDelRangeStart|" +
                "descendant::w:customXmlInsRangeEnd|" +
                "descendant::w:customXmlInsRangeStart|" +
                "descendant::w:del|" +
                "descendant::w:delInstrText|" +
                "descendant::w:delText|" +
                "descendant::w:ins|" +
                "descendant::w:moveFrom|" +
                "descendant::w:moveFromRangeEnd|" +
                "descendant::w:moveFromRangeStart|" +
                "descendant::w:moveTo|" +
                "descendant::w:moveToRangeEnd|" +
                "descendant::w:moveToRangeStart|" +
                "descendant::w:moveTo|" +
                "descendant::w:numberingChange|" +
                "descendant::w:rPrChange|" +
                "descendant::w:pPrChange|" +
                "descendant::w:rPrChange|" +
                "descendant::w:sectPrChange|" +
                "descendant::w:tcPrChange|" +
                "descendant::w:tblGridChange|" +
                "descendant::w:tblPrChange|" +
                "descendant::w:tblPrExChange|" +
                "descendant::w:trPrChange";
            XmlNodeList descendants = doc.SelectNodes(xpathExpression, nsmgr);
            return descendants.Count > 0;
        }

        private bool HasTrackedRevisions()
        {
            return DocParts.Any(part => PartHasTrackedRevisions(part));
        }
    }

    public class SearchAndReplaceException : Exception
    {
        public SearchAndReplaceException(string message) : base(message)
        {
        }
    }
}