using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

		static void RotateAndSaveImage(string fileName)
		{
  			//create an object that we can use to examine an image file
			Image img = Image.FromFile(fileName);

   			//rotate the picture by 90 degrees
			img.RotateFlip(RotateFlipType.Rotate90FlipNone);

    		//re-save the picture as a Png
			img.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);

    		//tidy up after we've finished
			img.Dispose();
		}
		
		public static void MakePatientPercentileChart(TestResultGroup testResultGroup,string fileName)
		{

			var entries = new List<Entry>();
			int[] green = new int[]{0,255,0};
			int[] yellow = new int[]{255,255,0};
			int[] red = new int[]{255,0,0};
			double interp;
			string hexcol;
			int percentile;

			foreach(TestResult result in testResultGroup.Tests){
				interp = 0.01 * (double)result.Percentile;
				if(interp < 0.5){
					hexcol = ColToHex(LinearInterpolation(red,yellow,2*interp));
				} else {
					hexcol = ColToHex(LinearInterpolation(yellow,green,2*(interp-0.5)));
				}
				if(result.Percentile == 0){
					percentile = 1;
				} else {
					percentile = result.Percentile;
				}
				entries.Add(new Entry(percentile){
					Label = result.Test.Name,
					ValueLabel = result.Percentile.ToString(),
					Color = SKColor.Parse(hexcol)
					});
			}

			var chart = new BarChart() {
				Entries = entries,
				MaxValue = 100,
				MinValue = 0,
				LabelOrientation = Microcharts.Orientation.Vertical
			};

			int width = entries.Count * 50;
			int height = 600;

			SKImageInfo info = new SKImageInfo(width, height);
			SKSurface surface = SKSurface.Create(info);

			SKCanvas canvas = surface.Canvas;

			chart.Draw(canvas,width,height);

			// create an image and then get the PNG (or any other) encoded data
			using (var data = surface.Snapshot().Encode(SKEncodedImageFormat.Png, 80)) {
    			// save the data to a stream
				using (var stream = File.OpenWrite(fileName + ".png")) {
					data.SaveTo(stream);
				}
			}

			RotateAndSaveImage(fileName + ".png");
		}
	}
}