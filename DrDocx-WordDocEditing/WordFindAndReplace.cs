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

    internal static class WordFindAndReplace
    {
        public static void SearchAndReplace(WordprocessingDocument wordDoc, string search,
            string replace, bool matchCase)
        {
            if (HasTrackedRevisions(wordDoc))
                throw new SearchAndReplaceException("Search and replace will not work with documents that contain revision tracking.");
            
            var xmlDoc = GetXmlDocument(wordDoc.MainDocumentPart.DocumentSettingsPart);
            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("w",
                "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            XmlNodeList trackedRevisions =
                xmlDoc.SelectNodes("descendant::w:trackRevisions", nsmgr);
            if (trackedRevisions.Count > 0)
                throw new SearchAndReplaceException(
                    "Revision tracking is turned on for document.");

            void SearchAndReplaceInDocPart(OpenXmlPart docPart)
            {
                xmlDoc = GetXmlDocument(docPart);
                SearchAndReplaceInXmlDocument(xmlDoc, search, replace, matchCase);
                PutXmlDocument(docPart, xmlDoc);
            }

            SearchAndReplaceInDocPart(wordDoc.MainDocumentPart);

            foreach (var part in wordDoc.MainDocumentPart.HeaderParts)
                SearchAndReplaceInDocPart(part);

            foreach (var part in wordDoc.MainDocumentPart.FooterParts)
                SearchAndReplaceInDocPart(part);

            if (wordDoc.MainDocumentPart.EndnotesPart != null)
                SearchAndReplaceInDocPart(wordDoc.MainDocumentPart.EndnotesPart);

            if (wordDoc.MainDocumentPart.FootnotesPart != null)
                SearchAndReplaceInDocPart(wordDoc.MainDocumentPart.FootnotesPart);
        }

        private static void SearchAndReplaceInXmlDocument(XmlDocument xmlDocument, string search,
            string replace, bool matchCase)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("w",
                "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            var paragraphs = xmlDocument.SelectNodes("descendant::w:p", nsmgr);
            foreach (var paragraph in paragraphs)
                SearchAndReplaceInParagraph((XmlElement) paragraph, search, replace, matchCase);
        }

        static void SearchAndReplaceInParagraph(XmlElement paragraph, string search,
            string replace, bool matchCase)
        {
            
            var xmlDoc = GetDocInfoForParagraph(paragraph, out var nsmgr, out var wordNamespace);

            // Get all the text nodes of the paragraph
            XmlNodeList paragraphText = paragraph.SelectNodes("descendant::w:t", nsmgr);
            
            var isMatchInParagraph = IsMatchInParagraph(search, matchCase, paragraphText);
            if (!isMatchInParagraph) 
                return;
            SplitTextRunsIntoCharacterRuns(paragraph, nsmgr, xmlDoc, wordNamespace);
            SearchAndReplaceCharRunsParagraph(paragraph, search, replace, matchCase, nsmgr, xmlDoc, wordNamespace);
            ConsolidateRunsWithSameProperties(paragraph, nsmgr, xmlDoc, wordNamespace);
            SearchAndReplaceInTextBoxes(paragraph, search, replace, matchCase, nsmgr);
        }

        private static XmlDocument GetDocInfoForParagraph(XmlElement paragraph, out XmlNamespaceManager nsmgr,
            out string wordNamespace)
        {
            XmlDocument xmlDoc = paragraph.OwnerDocument;
            // TODO: Figure out if these namespaces are necessary. Reeks of cargo cult programming.
            wordNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            nsmgr =
                new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("w", wordNamespace);
            return xmlDoc;
        }

        private static bool IsMatchInParagraph(string search, bool matchCase, XmlNodeList paragraphText)
        {
            // First we just mash all the text together and check for a match before we go any further.
            // If we know there's no match, then we don't have to go through the effort of screwing with
            // the runs to replace text while maintaining the document's structural integrity.
            StringBuilder sb = new StringBuilder();
            foreach (XmlNode text in paragraphText)
                sb.Append(((XmlElement) text).InnerText);
            bool isMatchInParagraph = sb.ToString().Contains(search) ||
                                      !matchCase && sb.ToString().ToUpper().Contains(search.ToUpper());
            return isMatchInParagraph;
        }

        // <summary>
        // This method takes in the paragraph after the text has been chopped into char runs and runs the
        // main search and replace algorithm
        // </summary>
        private static void SearchAndReplaceCharRunsParagraph(XmlElement paragraph, string search, string replace,
            bool matchCase, XmlNamespaceManager nsmgr, XmlDocument xmlDoc, string wordNamespace)
        {
            XmlNodeList runs;
            while (true)
            {
                bool cont = false;
                runs = paragraph.SelectNodes("child::w:r", nsmgr);
                for (int i = 0; i <= runs.Count - search.Length; ++i)
                {
                    var match = IsMatchHere(search, matchCase, nsmgr, runs, i);
                    if (!match) 
                        continue;
                    ReplaceMatchedCharRunsWithText(paragraph, search, replace, nsmgr, xmlDoc, wordNamespace, runs, i);
                    cont = true;
                    break;
                }

                if (!cont)
                    break;
            }
        }

        private static void ReplaceMatchedCharRunsWithText(XmlElement paragraph, string search, string replace,
            XmlNamespaceManager nsmgr, XmlDocument xmlDoc, string wordNamespace, XmlNodeList runs, int i)
        {
            var runProps =
                (XmlElement) runs[i].SelectSingleNode("descendant::w:rPr", nsmgr);
            var newRun = xmlDoc.CreateElement("w:r", wordNamespace);
            if (runProps != null)
            {
                XmlElement newRunProps = (XmlElement) runProps.CloneNode(true);
                newRun.AppendChild(newRunProps);
            }

            var newTextElement =
                xmlDoc.CreateElement("w:t", wordNamespace);
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
            paragraph.InsertAfter(newRun, (XmlNode) runs[i]);
            for (int c = 0; c < search.Length; ++c)
                paragraph.RemoveChild(runs[i + c]);
        }

        private static bool IsMatchHere(string search, bool matchCase, XmlNamespaceManager nsmgr, XmlNodeList runs, int i)
        {
            bool match = true;
            for (int charIndex = 0; charIndex < search.Length; charIndex++)
            {
                // Look through text nodes. If we find something that doesn't match with the string
                // we're looking for, then there is no match and we break. Otherwise, keep going.
                XmlElement textElement =
                    (XmlElement) runs[i + charIndex].SelectSingleNode("child::w:t", nsmgr);
                if (textElement == null)
                {
                    match = false;
                    break;
                }

                if (textElement.InnerText == search[charIndex].ToString())
                    continue;
                if (!matchCase &&
                    string.Equals(textElement.InnerText, search[charIndex].ToString(),
                        StringComparison.CurrentCultureIgnoreCase))
                    continue;
                match = false;
                break;
            }

            return match;
        }

        private static void SplitTextRunsIntoCharacterRuns(XmlElement paragraph, XmlNamespaceManager nsmgr,
            XmlDocument xmlDoc, string wordNamespace)
        {
            XmlNodeList runs = paragraph.SelectNodes("child::w:r", nsmgr);
            foreach (XmlElement run in runs)
            {
                XmlNodeList childElements = run.SelectNodes("child::*", nsmgr);
                if (childElements.Count > 0)
                {
                    XmlElement last = (XmlElement) childElements[childElements.Count - 1];
                    for (int c = childElements.Count - 1; c >= 0; --c)
                    {
                        if (childElements[c].Name == "w:rPr")
                            continue;
                        // If this run has text in it, we're going to split it up into a run for every
                        // character. Performance! Efficiency! We shall have none of it!
                        if (childElements[c].Name == "w:t")
                            SplitSingleRunIntoCharRuns(paragraph, nsmgr, xmlDoc, wordNamespace, childElements, c, run);
                        else
                        {
                            var newRun = xmlDoc.CreateElement("w:r", wordNamespace);
                            var runProps =
                                (XmlElement) run.SelectSingleNode("child::w:rPr", nsmgr);
                            if (runProps != null)
                            {
                                var newRunProps =
                                    (XmlElement) runProps.CloneNode(true);
                                newRun.AppendChild(newRunProps);
                            }

                            var newChildElement =
                                (XmlElement) childElements[c].CloneNode(true);
                            newRun.AppendChild(newChildElement);
                            paragraph.InsertAfter(newRun, run);
                        }
                    }

                    paragraph.RemoveChild(run);
                }
            }
        }

        private static void SplitSingleRunIntoCharRuns(XmlElement paragraph, XmlNamespaceManager nsmgr, XmlDocument xmlDoc,
            string wordNamespace, XmlNodeList childElements, int c, XmlElement run)
        {
            string textElementString = childElements[c].InnerText;
            for (int i = textElementString.Length - 1; i >= 0; --i)
            {
                XmlElement newRun =
                    xmlDoc.CreateElement("w:r", wordNamespace);
                XmlElement runProps =
                    (XmlElement) run.SelectSingleNode("child::w:rPr", nsmgr);
                if (runProps != null)
                {
                    XmlElement newRunProps =
                        (XmlElement) runProps.CloneNode(true);
                    newRun.AppendChild(newRunProps);
                }

                XmlElement newTextElement =
                    xmlDoc.CreateElement("w:t", wordNamespace);
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

        private static void ConsolidateRunsWithSameProperties(XmlElement paragraph, XmlNamespaceManager nsmgr, XmlDocument xmlDoc, string wordNamespace)
        {
            // Now that we've found and replaced everything we needed to, let's glue this document back together by
            // consolidating runs with identical properties into one.
            XmlNodeList children = paragraph.SelectNodes("child::*", nsmgr);
            List<int> matchId = new List<int>();
            int consolidatedRunsToCreate = ConsolidatedRunsToCreate(children, matchId, nsmgr);
            for (int i = 0; i <= consolidatedRunsToCreate; ++i)
            {
                var x1 = matchId.IndexOf(i);
                var x2 = matchId.LastIndexOf(i);
                if (x1 == x2)
                    continue;
                StringBuilder sb2 = new StringBuilder();
                for (int z = x1; z <= x2; ++z)
                    sb2.Append(((XmlElement) children[z]
                        .SelectSingleNode("w:t", nsmgr)).InnerText);
                XmlElement newRun = xmlDoc.CreateElement("w:r", wordNamespace);
                XmlElement runProps =
                    (XmlElement) children[x1].SelectSingleNode("child::w:rPr", nsmgr);
                if (runProps != null)
                {
                    XmlElement newRunProps = (XmlElement) runProps.CloneNode(true);
                    newRun.AppendChild(newRunProps);
                }

                XmlElement newTextElement = xmlDoc.CreateElement("w:t", wordNamespace);
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

        private static int ConsolidatedRunsToCreate(XmlNodeList children, List<int> matchId, XmlNamespaceManager nsmgr)
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
                    children[ch].SelectSingleNode("w:t", nsmgr) != null &&
                    children[ch - 1].SelectSingleNode("w:t", nsmgr) != null)
                {
                    XmlElement runProps =
                        (XmlElement) children[ch].SelectSingleNode("w:rPr", nsmgr);
                    XmlElement lastRunProps =
                        (XmlElement) children[ch - 1].SelectSingleNode("w:rPr", nsmgr);
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

        private static void SearchAndReplaceInTextBoxes(XmlElement paragraph, string search, string replace, bool matchCase,
            XmlNamespaceManager nsmgr)
        {
            // Text inside text boxes have to be handled separately and yet can exist inside a paragraph???
            var txbxParagraphs = paragraph.SelectNodes("descendant::w:p", nsmgr);
            foreach (XmlElement p in txbxParagraphs)
                SearchAndReplaceInParagraph((XmlElement) p, search, replace, matchCase);
        }


        public static XmlDocument GetXmlDocument(OpenXmlPart part)
        {
            XmlDocument xmlDoc = new XmlDocument();
            using (Stream partStream = part.GetStream())
            using (XmlReader partXmlReader = XmlReader.Create(partStream))
                xmlDoc.Load(partXmlReader);
            return xmlDoc;
        }

        public static void PutXmlDocument(OpenXmlPart part, XmlDocument xmlDoc)
        {
            using (Stream partStream = part.GetStream(FileMode.Create, FileAccess.Write))
            using (XmlWriter partXmlWriter = XmlWriter.Create(partStream))
                xmlDoc.Save(partXmlWriter);
        }
        
        public static bool PartHasTrackedRevisions(OpenXmlPart part)
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

        public static bool HasTrackedRevisions(WordprocessingDocument doc)
        {
            if (PartHasTrackedRevisions(doc.MainDocumentPart))
                return true;
            foreach (var part in doc.MainDocumentPart.HeaderParts)
                if (PartHasTrackedRevisions(part))
                    return true;
            foreach (var part in doc.MainDocumentPart.FooterParts)
                if (PartHasTrackedRevisions(part))
                    return true;
            if (doc.MainDocumentPart.EndnotesPart != null)
                if (PartHasTrackedRevisions(doc.MainDocumentPart.EndnotesPart))
                    return true;
            if (doc.MainDocumentPart.FootnotesPart != null)
                if (PartHasTrackedRevisions(doc.MainDocumentPart.FootnotesPart))
                    return true;
            return false;
        }
    }

    public class SearchAndReplaceException : Exception
    {
        public SearchAndReplaceException(string message) : base(message)
        {
        }
    }

}