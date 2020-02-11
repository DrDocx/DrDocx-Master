using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DrDocxModels;
using System.Threading.Tasks;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace ReportGen
{
    static class ReportGen
    {
        public static async Task GenerateReport(Patient patient, string templatePath, string newFilePath, Dictionary<string, string> templateReplacements, List<TestResultGroup> testGroups)
        {

            if (File.Exists(newFilePath))
            {
                File.Delete(newFilePath);
            }

            File.Copy(templatePath, newFilePath);

            InsertPatientData(templateReplacements, templatePath, newFilePath);

            Paragraph p = new Paragraph(new Run(new Text("\n")));
            foreach (var testResultGroup in testGroups)
            {
                CreateTitleTable(newFilePath, testResultGroup.TestGroupInfo.Name);
                p = new Paragraph(new Run(new Text("\n")));
                using (WordprocessingDocument doc = WordprocessingDocument.Open(newFilePath, true))
                {
                    doc.MainDocumentPart.Document.Body.AppendChild(p);
                }
                CreateSubTable(newFilePath, testResultGroup.Tests);
                using (WordprocessingDocument doc = WordprocessingDocument.Open(newFilePath, true))
                {
                    Paragraph lineBreak = new Paragraph(new Run(new Text("\n")));
                    doc.MainDocumentPart.Document.Body.AppendChild(lineBreak);
                }

            }
        }

        public static async Task CombineReportAndVisualizations(string reportPath, string[] visualizationImagePaths)
        {
            foreach (var imagePath in visualizationImagePaths)
            {
                BreakPage(reportPath);
                InsertAPicture(reportPath, imagePath);
            }
        }

        public static void BreakPage(string newfilePath){
            using (WordprocessingDocument myDoc = WordprocessingDocument.Open(newfilePath, true))
            {
                myDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(new Break(){ Type = BreakValues.Page })));
            }
        }

        public static void JoinFiles(string newfilePath, string vizPath)
        {
            using (WordprocessingDocument myDoc = WordprocessingDocument.Open(newfilePath, true))
            {
                MainDocumentPart mainPart = myDoc.MainDocumentPart;
                string altChunkId = "AltChunkId1";
                AlternativeFormatImportPart chunk = mainPart.AddAlternativeFormatImportPart(
                    AlternativeFormatImportPartType.WordprocessingML, altChunkId);
                using (FileStream fileStream = File.Open(vizPath, FileMode.Open))
                {
                    chunk.FeedData(fileStream);
                }
                AltChunk altChunk = new AltChunk();
                altChunk.Id = altChunkId;
                mainPart.Document.Body.InsertAfter(altChunk, mainPart.Document.Body.Elements<Paragraph>().Last());
                mainPart.Document.Save();
                myDoc.Close();
            }
        }

        public static void InsertPatientData(Dictionary<string, string> infoReplacements, string templatePath, string newfilePath)
        {
            byte[] byteArray = File.ReadAllBytes(templatePath);

            using (var stream = new MemoryStream())
            {
                stream.Write(byteArray, 0, byteArray.Length);

                using (WordprocessingDocument doc = WordprocessingDocument.Open(stream, true))
                {
                    //Needed because I'm working with template dotx file, 
                    //remove this if the template is a normal docx. 
                    doc.ChangeDocumentType(DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
                    foreach (var substitution in infoReplacements)
                    {
                        doc.InsertText(substitution.Key, substitution.Value);
                    }
                }
                using (FileStream fs = new FileStream(newfilePath, FileMode.Create))
                {
                    stream.WriteTo(fs);
                }
            }
        }

        public static void InsertAPicture(string document, string fileName)
        {
            using (WordprocessingDocument wordprocessingDocument =
                WordprocessingDocument.Open(document, true))
            {
                MainDocumentPart mainPart = wordprocessingDocument.MainDocumentPart;

                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Png);

                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    imagePart.FeedData(stream);
                }

                AddImageToBody(wordprocessingDocument, mainPart.GetIdOfPart(imagePart));
            }
        }

        private static void AddImageToBody(WordprocessingDocument wordDoc, string relationshipId)
        {
            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = 990000L, Cy = 792000L },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.png"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            double sdtWidth = element.Inline.Extent.Cx;
            double sdtHeight = element.Inline.Extent.Cy;
            double sdtRatio = sdtWidth / sdtHeight;

            int finalWidth = (int)(sdtWidth * 7);
            int finalHeight = (int)(finalWidth * 1.2);

            //Resize picture placeholder
            element.Inline.Extent.Cx = finalWidth;
            element.Inline.Extent.Cy = finalHeight;

            //Change width/height of picture shapeproperties Transform
            //This will override above height/width until you manually drag image for example
            element.Inline.Graphic.GraphicData
                .GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>()
                .ShapeProperties.Transform2D.Extents.Cx = finalWidth;
            element.Inline.Graphic.GraphicData
                .GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>()
                .ShapeProperties.Transform2D.Extents.Cy = finalHeight;


            // Append the reference to body, the element should be in a Run.
            wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
        }

        public static void CreateSubTable(string fileName, List<TestResult> testResults)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Open(fileName, true))
            {
                int numResults = testResults.Count();

                Table table = new Table();

                // Append the TableProperties object to the empty table.
                table.AppendChild<TableProperties>(SubTableFormat());


                // Create a row.
                int cellHeight = 375;
                TableRow tr = new TableRow(new TableRowProperties(new TableRowHeight() { Val = Convert.ToUInt32(cellHeight) }));
                TableCell testName = new TableCell(LabelCellFormat(),
                    new Paragraph(new Run(new RunProperties(new RunFonts() { Ascii = "Times New Roman" }, new Bold(), new FontSize() { Val = "24" }),
                    new Text("Tests"))));
                TableCell zScore = new TableCell(LabelCellFormat(),
                    new Paragraph(new Run(new RunProperties(new RunFonts() { Ascii = "Times New Roman" }, new Bold(), new FontSize() { Val = "24" }),
                    new Text("Z-Score"))));
                TableCell percentile = new TableCell(LabelCellFormat(),
                    new Paragraph(new Run(new RunProperties(new RunFonts() { Ascii = "Times New Roman" }, new Bold(), new FontSize() { Val = "24" }),
                    new Text("Percentile"))));
                tr.Append(testName); tr.Append(zScore); tr.Append(percentile);
                table.Append(tr);


                for (int i = 0; i < numResults; i++)
                {
                    tr = new TableRow(new TableRowProperties(new TableRowHeight() { Val = Convert.ToUInt32(cellHeight) }));
                    testName = new TableCell(DataCellFormat(),
                        new Paragraph(new Run(new Text(testResults[i].RelatedTest.Name))));
                    zScore = new TableCell(DataCellFormat(),
                        new Paragraph(new Run(new Text(testResults[i].ZScore.ToString()))));
                    percentile = new TableCell(DataCellFormat(),
                    new Paragraph(new Run(new Text(testResults[i].Percentile.ToString()))));
                    tr.Append(testName); tr.Append(zScore); tr.Append(percentile);
                    table.Append(tr);
                }

                // Append the table to the document.
                //doc.MainDocumentPart.Document.Body.Append(new Paragraph(new Run(new RunProperties(new FontSize(){Val = "50"}),
                //new Text("fevf"))));
                doc.MainDocumentPart.Document.Body.Append(table);
            }
        }

        public static TableCellProperties LabelCellFormat()
        {
            TableCellProperties tcp = new TableCellProperties(
                new TableCellBorders(
                    new TopBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 1
                    },
                    new BottomBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 1
                    },
                    new LeftBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.None),
                        Size = 0
                    },
                    new RightBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.None),
                        Size = 0
                    },
                    new InsideHorizontalBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.None),
                        Size = 0
                    },
                    new InsideVerticalBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.None),
                        Size = 0
                    }
                ),
                new VerticalTextAlignmentOnPage() { Val = VerticalJustificationValues.Center }
            );
            return tcp;
        }

        public static TableCellProperties DataCellFormat()
        {
            TableCellProperties tcp = new TableCellProperties(new VerticalTextAlignmentOnPage() { Val = VerticalJustificationValues.Center });
            return tcp;
        }

        public static TableProperties SubTableFormat()
        {
            TableProperties tblProp = new TableProperties(
                new TableBorders(
                    new TopBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 1
                    },
                    new BottomBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 1
                    },
                    new LeftBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.None),
                        Size = 0
                    },
                    new RightBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.None),
                        Size = 0
                    },
                    new InsideHorizontalBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.None),
                        Size = 0
                    },
                    new InsideVerticalBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.None),
                        Size = 0
                    }
                ),
                new TableWidth() { Type = TableWidthUnitValues.Pct, Width = "4580" }
            );
            return tblProp;
        }

        public static void CreateTitleTable(string fileName, string title)
        {
            // Use the file name and path passed in as an argument 
            // to open an existing Word 2007 document.

            using (WordprocessingDocument doc
                = WordprocessingDocument.Open(fileName, true))
            {
                // Create an empty table.
                Table table = new Table();

                // Create a TableProperties object and specify its border information.
                TableProperties tblProp = new TableProperties(
                    new TableBorders(
                        new TopBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(BorderValues.Single),
                            Size = 1
                        },
                        new BottomBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(BorderValues.Single),
                            Size = 1
                        },
                        new LeftBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(BorderValues.Single),
                            Size = 1
                        },
                        new RightBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(BorderValues.Single),
                            Size = 1
                        },
                        new InsideHorizontalBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(BorderValues.Single),
                            Size = 1
                        },
                        new InsideVerticalBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(BorderValues.Single),
                            Size = 1
                        }
                    ),
                    new TableJustification() { Val = TableRowAlignmentValues.Center },
                    new TableWidth() { Type = TableWidthUnitValues.Pct, Width = "4580" }
                );

                // Append the TableProperties object to the empty table.
                table.AppendChild<TableProperties>(tblProp);

                // Create a row.
                TableRow tr = new TableRow(new TableRowProperties(new TableRowHeight() { Val = Convert.ToUInt32(500) }));

                // Create a cell.
                TableCell tc = new TableCell();

                RunProperties rp = new RunProperties(new RunFonts() { Ascii = "Times New Roman" }, new Bold(), new FontSize() { Val = "24" });


                // Specify the table cell content.
                tc.Append(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
                tc.Append(new Paragraph(new ParagraphProperties(new Justification() { Val = JustificationValues.Center }), new Run(rp, new Text(title))));

                // Append the table cell to the table row.
                tr.Append(tc);

                // Append the table row to the table.
                table.Append(tr);

                // Append the table to the document.
                doc.MainDocumentPart.Document.Body.Append(table);
            }
        }

        public static WordprocessingDocument InsertText(this WordprocessingDocument doc, string contentControlTag, string text)
        {
            var filteredBodyContentControls = doc.MainDocumentPart.Document.Body.Descendants<SdtElement>()
            .Where(sdt => sdt.SdtProperties.GetFirstChild<Tag>()?.Val == contentControlTag);

            var header = doc.MainDocumentPart.HeaderParts;
            foreach (var headerPart in header)
            {
                var headerContentControls = headerPart.Header.Descendants<SdtElement>();
                var filteredCCs = headerContentControls.Where(sdt => sdt.SdtProperties.GetFirstChild<Tag>()?.Val == contentControlTag);
                foreach (var contentControl in filteredCCs)
                {
                    contentControl.Descendants<Text>().First().Text = text;
                }
            }

            var footer = doc.MainDocumentPart.FooterParts;
            foreach (var footerPart in footer)
            {
                var footerContentControls = footerPart.Footer.Descendants<SdtElement>();
                var filteredCCs = footerContentControls.Where(sdt => sdt.SdtProperties.GetFirstChild<Tag>()?.Val == contentControlTag);
                foreach (var contentControl in filteredCCs)
                {
                    contentControl.Descendants<Text>().First().Text = text;
                }
            }

            foreach (var contentControl in filteredBodyContentControls)
            {
                contentControl.Descendants<Text>().First().Text = text;
            }

            //element.Descendants<Text>().First().Text = text;
            //element.Descendants<Text>().Skip(1).ToList().ForEach(t => t.Remove());

            return doc;
        }
    }
}