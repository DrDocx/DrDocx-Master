using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

using DrDocx.Models;
using static DrDocx.WordDocEditing.ChartAPI;

namespace DrDocx.WordDocEditing
{
	public class WordAPI
	{
		public WordAPI(string templatePath, string docPath, bool readOnly = true)
		{
			if (File.Exists(docPath))
				File.Delete(docPath);
			File.Copy(templatePath, docPath);
			DocPath = docPath;
			WordDoc = WordprocessingDocument.Open(DocPath, !readOnly);
		}

		public WordAPI(string docPath, bool readOnly = true)
		{
			DocPath = docPath;
			WordDoc = WordprocessingDocument.Open(DocPath, !readOnly);
		}

		public void Close()
		{
			WordDoc.Close();
		}
		private string DocPath { get; set; }
		private WordprocessingDocument WordDoc { get; set; }

		public void GenerateReport(Patient patient, string directory)
		{
			
			Dictionary<string, string> patientDict = new Dictionary<string, string>();
			foreach (var fieldValueGroup in patient.FieldValueGroups)
			{
				foreach (var fieldValue in fieldValueGroup.FieldValues)
				{
					patientDict.Add("{{" + fieldValue.Field.MatchText + "}}", fieldValue.FieldTextValue);
				}
			}

			FindAndReplace(patientDict, matchCase: false);

			foreach (var testResultGroup in patient.ResultGroups)
			{
				DisplayTestGroup(testResultGroup);
				LineBreak();
			}

			PageBreak();

			var i = 0;
			const double chartScale = 6.464;
			foreach (var resultGroup in patient.ResultGroups)
			{
				i++;
				MakePatientPercentileChart(resultGroup,directory + patient.Name + i);
				InsertPicturePng(directory + patient.Name + i + ".png",chartScale,chartScale*(resultGroup.Tests.Count*50)/600);
				AddParagraph(resultGroup.TestGroupInfo.Name, bold: true, fontsize: 16, alignment: "center");
			}
		}

		public void FindAndReplace(Dictionary<string, string> findReplacePairs, bool matchCase)
		{
			var findAndReplacer = new WordFindAndReplace(WordDoc, matchCase);
			findAndReplacer.SearchAndReplace(findReplacePairs);
		}

		public bool ContainsText(string matchText, bool matchCase)
		{
			var findAndReplacer = new WordFindAndReplace(WordDoc, matchCase);
			return findAndReplacer.ContainsText(matchText, matchCase);
		}

		private void PageBreak()
		{
			WordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
		}

		private void AddParagraph(string title, bool bold = false, bool italic = false, string alignment = "left",int fontsize = 24)
		{
			RunProperties rp = new RunProperties(new RunFonts() { Ascii = "Times New Roman" }, new FontSize() { Val = (fontsize*2).ToString() });
			if(bold){
				rp.Bold = new Bold();
			}
			if(italic){
				rp.Italic = new Italic();
			}
			JustificationValues val = JustificationValues.Left;
			switch(alignment)
			{
				case "left":
				val = JustificationValues.Left;
				break;
				case "center":
				val = JustificationValues.Center;
				break;
				case "right":
				val = JustificationValues.Right;
				break;
				
			}
			WordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(
				new ParagraphProperties(new Justification() { Val = val }),
				new Run(rp,new Text(title))
				));
		}

		private void LineBreak()
		{
			WordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(new Text("\n"))));
		}

		public void JoinFile(string otherFilePath)
		{
			PageBreak();
			var mainPart = WordDoc.MainDocumentPart;
			const string altChunkId = "AltChunkId1";
			var chunk = mainPart.AddAlternativeFormatImportPart(
				AlternativeFormatImportPartType.WordprocessingML, altChunkId);
			using (var fileStream = File.Open(otherFilePath, FileMode.Open))
			{
				chunk.FeedData(fileStream);
			}

			var altChunk = new AltChunk { Id = altChunkId };
			mainPart.Document.Body.InsertAfter(altChunk, mainPart.Document.Body.Elements<Paragraph>().Last());
			mainPart.Document.Save();
		}

		private void DisplayTestGroup(TestResultGroup testResultGroup){
			WordDoc.MainDocumentPart.Document.Body.Append(CreateTitleTable(testResultGroup.TestGroupInfo.Name));
			LineBreak();
			WordDoc.MainDocumentPart.Document.Body.Append(CreateSubTable(testResultGroup));
		}

		private Table CreateTitleTable(string title)
		{

			var table = new Table();

				// Append the TableProperties object to the empty table.
			table.AppendChild<TableProperties>(WordTableFormats.TitleTableFormat());

				// Create a row.
			var tr = new TableRow(new TableRowProperties(new TableRowHeight() { Val = Convert.ToUInt32(500) }));

				// Create a cell.
			var tc = new TableCell();

			var rp = new RunProperties(new RunFonts() { Ascii = "Times New Roman" }, new Bold(), new FontSize() { Val = "24" });


				// Specify the table cell content.
			tc.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
			tc.AppendChild(new Paragraph(new ParagraphProperties(new Justification() { Val = JustificationValues.Center }), new Run(rp, new Text(title))));

				// Append the table cell to the table row.
			tr.AppendChild(tc);

				// Append the table row to the table.
			table.AppendChild(tr);

			return table;
		}

		private Table CreateSubTable(TestResultGroup testResultGroup)
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
					new Text("StandardizedScore"))));
			TableCell percentile = new TableCell(WordTableFormats.LabelCellFormat(),
				new Paragraph(new Run(new RunProperties(new RunFonts() { Ascii = "Times New Roman" }, new Bold(), new FontSize() { Val = "24" }),
					new Text("Percentile"))));
			tr.Append(testName,zScore,percentile);
			table.AppendChild(tr);


			foreach (TestResult result in testResultGroup.Tests)
			{
				tr = new TableRow(new TableRowProperties(new TableRowHeight() { Val = Convert.ToUInt32(cellHeight) }));
				testName = new TableCell(WordTableFormats.DataCellFormat(),
					new Paragraph(new Run(new Text(result.Test.Name))));
				zScore = new TableCell(WordTableFormats.DataCellFormat(),
					new Paragraph(new Run(new Text(result.StandardizedScore.ToString()))));
				percentile = new TableCell(WordTableFormats.DataCellFormat(),
					new Paragraph(new Run(new Text(result.Percentile.ToString()))));
				tr.AppendChild(testName); tr.AppendChild(zScore); tr.AppendChild(percentile);
				table.AppendChild(tr);
			}

			return table;
		}

		private void InsertPicturePng(string imageFilePath, double scaleWidth, double scaleHeight)
		{
			MainDocumentPart mainPart = WordDoc.MainDocumentPart;

			ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Png);

			using (FileStream stream = new FileStream(imageFilePath, FileMode.Open))
			{
				imagePart.FeedData(stream);
			}

			AddImageToBody(mainPart.GetIdOfPart(imagePart),scaleWidth,scaleHeight);
		}

		private void AddImageToBody(string relationshipId, double scaleWidth, double scaleHeight)
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
			WordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(
				new ParagraphProperties(new Justification() { Val = JustificationValues.Center }),
				new Run(element)
				));
		}
	}
}