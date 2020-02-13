using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

using Microcharts;
using Entry = Microcharts.Entry;

using SkiaSharp;

using DrDocx.Models;

namespace DrDocx.WordDocEditing
{
	public static class ChartAPI
	{
		private static int[] LinearInterpolation(int[] c1,int[] c2,double interp)
		{
			int[] newcol = new int[3];
			for (int i = 0; i < 3; i++){
				newcol[i] = (int)(interp*(c2[i]-c1[i])) + c1[i];
			}
			return newcol;
		}

		private static string ColToHex(int[] col)
		{
			string hex = "#";
			for (int i = 0; i < 3;i++){
				if(col[i] < 16){
					hex += "0";
				}
				hex += col[i].ToString("X");
			}
			return hex;
		}

		static void RotateAndSaveImage(String input, String output)
		{
  			//create an object that we can use to examine an image file
			Image img = Image.FromFile(input);

   			//rotate the picture by 90 degrees
			img.RotateFlip(RotateFlipType.Rotate90FlipNone);

    		//re-save the picture as a Jpeg
			img.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);

			img.Save(output, System.Drawing.Imaging.ImageFormat.Jpeg);

    		//tidy up after we've finished
			img.Dispose();
		}

		public static void MakePatientPercentileChart(Patient patient,string ImageFileName)
		{
			List<Entry> entries = new List<Entry>();
			int[] green = new int[]{0,255,0};
			int[] yellow = new int[]{255,255,0};
			int[] red = new int[]{255,0,0};
			double interp;
			string hexcol;
			int percentile;

			foreach(var testResultGroup in patient.ResultGroups){
				foreach(TestResult result in testResultGroup.Tests){
					interp = 0.01 * (double)result.Percentile;
					if(interp < 0.5){
						hexcol = ColToHex(LinearInterpolation(red,yellow,interp*2));
					} else {
						hexcol = ColToHex(LinearInterpolation(yellow,green,interp*2 - 1));
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
				using (var stream = File.OpenWrite(ImageFileName + ".png")) {
					data.SaveTo(stream);
				}
			}
			RotateAndSaveImage(ImageFileName + ".png");
		}
	}
}