using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

using DrDocxModels;

namespace WordDocEditing
{
	public static class WordAPI
	{
		public static void FindAndReplace(WordprocessingDocument myDoc, string search, string replace, bool matchCase)
		{
			WordFindAndReplace.SearchAndReplace(myDoc, search, replace, matchCase);
		}

		public static void PageBreak(WordprocessingDocument myDoc)
		{
			myDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
		}

		public static void LineBreak(WordprocessingDocument myDoc)
		{
			myDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(new Text("\n"))));
		}

		public static void JoinFile(WordprocessingDocument myDoc, string otherFilePath)
		{
			PageBreak(myDoc);
			MainDocumentPart mainPart = myDoc.MainDocumentPart;
			string altChunkId = "AltChunkId1";
			AlternativeFormatImportPart chunk = mainPart.AddAlternativeFormatImportPart(
				AlternativeFormatImportPartType.WordprocessingML, altChunkId);
			using (FileStream fileStream = File.Open(otherFilePath, FileMode.Open))
			{
				chunk.FeedData(fileStream);
			}
			AltChunk altChunk = new AltChunk();
			altChunk.Id = altChunkId;
			mainPart.Document.Body.InsertAfter(altChunk, mainPart.Document.Body.Elements<Paragraph>().Last());
			mainPart.Document.Save();
			myDoc.Close();
		}

		public static void InsertPatientData(WordprocessingDocument myDoc, Patient patient)
		{
			InsertTextInLabel(myDoc,"NAME",patient.Name);
			InsertTextInLabel(myDoc,"PREFERRED_NAME",patient.PreferredName);
			InsertTextInLabel(myDoc,"DOB",patient.DateOfBirth.ToString());
			InsertTextInLabel(myDoc,"TEST_DATE",patient.DateOfTesting.ToString());
			InsertTextInLabel(myDoc,"MEDICAL_RECORD_NUMBER",patient.MedicalRecordNumber.ToString());
			InsertTextInLabel(myDoc,"ADDRESS",patient.Address);
			InsertTextInLabel(myDoc,"MEDICATION",patient.Medications);
		}

		public static void InsertTextInLabel(WordprocessingDocument myDoc, string contentControlTag, string text)
		{
			var filteredBodyContentControls = myDoc.MainDocumentPart.Document.Body.Descendants<SdtElement>()
			.Where(sdt => sdt.SdtProperties.GetFirstChild<Tag>()?.Val == contentControlTag);

			var header = myDoc.MainDocumentPart.HeaderParts;
			foreach (var headerPart in header)
			{
				var headerContentControls = headerPart.Header.Descendants<SdtElement>();
				var filteredCCs = headerContentControls.Where(sdt => sdt.SdtProperties.GetFirstChild<Tag>()?.Val == contentControlTag);
				foreach (var contentControl in filteredCCs)
				{
					contentControl.Descendants<Text>().First().Text = text;
				}
			}

			var footer = myDoc.MainDocumentPart.FooterParts;
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
		}

		public static void DisplayTestGroup(WordprocessingDocument myDoc, TestResultGroup testResultGroup){
			myDoc.MainDocumentPart.Document.Body.Append(CreateTitleTable(testResultGroup.TestGroupInfo.Name));
			LineBreak(myDoc);
			myDoc.MainDocumentPart.Document.Body.Append(CreateSubTable(testResultGroup));
		}

		private static Table CreateTitleTable(string title)
		{

			Table table = new Table();

				// Append the TableProperties object to the empty table.
			table.AppendChild<TableProperties>(WordTableFormats.TitleTableFormat());

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

			return table;
		}

		private static Table CreateSubTable(TestResultGroup testResultGroup)
		{
			int numResults = testResultGroup.Tests.Count();

			Table table = new Table();

				// Append the TableProperties object to the empty table.
			table.AppendChild<TableProperties>(WordTableFormats.SubTableFormat());


				// Create a row.
			int cellHeight = 375;
			TableRow tr = new TableRow(new TableRowProperties(new TableRowHeight() { Val = Convert.ToUInt32(cellHeight) }));
			TableCell testName = new TableCell(WordTableFormats.LabelCellFormat(),
				new Paragraph(new Run(new RunProperties(new RunFonts() { Ascii = "Times New Roman" }, new Bold(), new FontSize() { Val = "24" }),
					new Text("Tests"))));
			TableCell zScore = new TableCell(WordTableFormats.LabelCellFormat(),
				new Paragraph(new Run(new RunProperties(new RunFonts() { Ascii = "Times New Roman" }, new Bold(), new FontSize() { Val = "24" }),
					new Text("Z-Score"))));
			TableCell percentile = new TableCell(WordTableFormats.LabelCellFormat(),
				new Paragraph(new Run(new RunProperties(new RunFonts() { Ascii = "Times New Roman" }, new Bold(), new FontSize() { Val = "24" }),
					new Text("Percentile"))));
			tr.Append(testName,zScore,percentile);
			table.Append(tr);


			foreach (TestResult result in testResultGroup.Tests)
			{
				tr = new TableRow(new TableRowProperties(new TableRowHeight() { Val = Convert.ToUInt32(cellHeight) }));
				testName = new TableCell(WordTableFormats.DataCellFormat(),
					new Paragraph(new Run(new Text(result.RelatedTest.Name))));
				zScore = new TableCell(WordTableFormats.DataCellFormat(),
					new Paragraph(new Run(new Text(result.ZScore.ToString()))));
				percentile = new TableCell(WordTableFormats.DataCellFormat(),
					new Paragraph(new Run(new Text(result.Percentile.ToString()))));
				tr.Append(testName); tr.Append(zScore); tr.Append(percentile);
				table.Append(tr);
			}

			return table;
		}

		public static void InsertPicturePng(WordprocessingDocument myDoc, string imageFilePath, double scaleWidth, double scaleHeight)
		{
			MainDocumentPart mainPart = myDoc.MainDocumentPart;

			ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Png);

			using (FileStream stream = new FileStream(imageFilePath, FileMode.Open))
			{
				imagePart.FeedData(stream);
			}

			AddImageToBody(myDoc, mainPart.GetIdOfPart(imagePart),scaleWidth,scaleHeight);
		}

		private static void AddImageToBody(WordprocessingDocument wordDoc, string relationshipId, double scaleWidth, double scaleHeight)
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

			int finalWidth = (int)(sdtWidth*scaleWidth);
			int finalHeight = (int)(sdtHeight*scaleHeight);
			
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
	}
}