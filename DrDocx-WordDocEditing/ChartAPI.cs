using Microcharts;
using Entry = Microcharts.Entry;

using SkiaSharp;

namespace DrDocx.WordDocEditing
{
	public static class ChartAPI
	{
		public static int[] ColorInterpolation(int[] c1,int[] c2,double interp)
		{
			int[] newcol = new int[3];
			for (int i = 0; i < 3; i++){
				newcol[i] = (int)(interp*(c2[i]-c1[i])) + c1[i];
			}
			return newcol;
		}

		public static string ColToHex(int[] col)
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
			img.Save(output, System.Drawing.Imaging.ImageFormat.Jpeg);

    		//tidy up after we've finished
			img.Dispose();
		}

		//public static void MakeChart(string chartType,List<int> data,string labelOrientation,string chartOrientation)
		//{
		// Implement this later
		//}
	}
}