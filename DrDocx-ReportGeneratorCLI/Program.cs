using System;
using System.IO;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Packaging;

using Microcharts;
using Entry = Microcharts.Entry;

using SkiaSharp;

using DrDocx.Models;
using DrDocx.WordDocEditing;

namespace DrDocx.ReportGenCLI
{
	static class Program
	{

		static int[] ColorInterpolation(int[] c1,int[] c2,double interp){
			int[] newcol = new int[3];
			for (int i = 0; i < 3; i++){
				newcol[i] = (int)(interp*(c2[i]-c1[i])) + c1[i];
			}
			return newcol;
		}

		static string ColToHex(int[] col){
			string hex = "#";
			for (int i = 0; i < 3;i++){
				if(col[i] < 16){
					hex += "0";
				}
				hex += col[i].ToString("X");
			}
			return hex;
		}

		static void Main(string[] args)
		{
			
			Random random = new Random();
			List<TestResult> results = new List<TestResult>();
			for (int i = 0; i < 21; i++)
			{
				double r = random.NextDouble() * 6 - 3;
				results.Add(new TestResult(){
					RelatedTest = new Test(){Name = "LD" + i.ToString()},
					ZScore = (int)(random.NextDouble()*6 - 3),
					Percentile = (int)(random.NextDouble()*100)
					});
			}

			List<TestResultGroup> resultGroups = new List<TestResultGroup>(new TestResultGroup[]{
				new TestResultGroup(){
					TestGroupInfo = new TestGroup(){Name = "Symptom Checklist - 90 - Revised"},
					Tests = results
				}
				});

			Patient johnDoe = new Patient(){
				Name = "John Doe",
				PreferredName = "Johnny",
				DateOfBirth = new DateTime(628243894905389400),
				DateOfTesting = new DateTime(628243894905389400),
				MedicalRecordNumber = 123456,
				ResultGroups = resultGroups
			};

			Patient patient = johnDoe;

			Console.WriteLine(patient.DateOfBirth);
			

			string templatePath = @"Reports\ReportTemplate\Report_Template.dotx";
			string newfilePath = @"Reports\GeneratedReports\" + patient.Name + ".docx";
			string vizPath = @"Reports\GeneratedReports\Visualization.docx";
			string imagePath = @"Reports\GeneratedReports\renderedVisualization2.png";

			if (File.Exists(newfilePath))
			{
				File.Delete(newfilePath);
			}

			File.Copy(templatePath, newfilePath);

			var entries = new List<Entry>();
			int[] green = new int[]{0,255,0};
			int[] yellow = new int[]{255,255,0};
			int[] red = new int[]{255,0,0};
			double interp;
			string hexcol;
			int percentile;
			
			using(WordprocessingDocument myDoc = WordprocessingDocument.Open(newfilePath,true)){

				myDoc.ChangeDocumentType(DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
				var wordAPI = new WordAPI(myDoc);
				wordAPI.InsertPatientData(patient);
				foreach(TestResultGroup testResultGroup in patient.ResultGroups){
					wordAPI.DisplayTestGroup(testResultGroup);

					foreach(TestResult result in testResultGroup.Tests){
						interp = 2*Math.Abs(0.01 * (double)result.Percentile - 0.5);
						if(interp < 0.5){
							hexcol = ColToHex(ColorInterpolation(green,yellow,2*interp));
						} else {
							hexcol = ColToHex(ColorInterpolation(yellow,red,2*(interp-0.5)));
						}
						if(result.Percentile == 0){
							percentile = 1;
						} else {
							percentile = result.Percentile;
						}
						entries.Add(new Entry(percentile){
							Label = result.RelatedTest.Name,
							ValueLabel = result.Percentile.ToString(),
							Color = SKColor.Parse(hexcol)
							});
					}
				}
				wordAPI.PageBreak();
				//InsertPicturePng(myDoc, imagePath,7,1.2);
				//JoinFile(myDoc,vizPath);

			}

			var chart = new BarChart() {
				Entries = entries,
				MaxValue = 100,
				MinValue = 0,
				LabelOrientation = Microcharts.Orientation.Vertical
			};

			int width = 800;
			int height = 300;

			SKImageInfo info = new SKImageInfo(width, height);
			SKSurface surface = SKSurface.Create(info);

			SKCanvas canvas = surface.Canvas;

			chart.Draw(canvas,width,height);

			// create an image and then get the PNG (or any other) encoded data
			using (var data = surface.Snapshot().Encode(SKEncodedImageFormat.Png, 80)) {
    			// save the data to a stream
				using (var stream = File.OpenWrite("one.png")) {
					data.SaveTo(stream);
				}
			}

			using(WordprocessingDocument myDoc = WordprocessingDocument.Open(newfilePath,true)){
				var wordAPI = new WordAPI(myDoc);
				wordAPI.InsertPicturePng("one.png",6,3);
			}

			//Console.WriteLine("Modified");

		}
	}
}