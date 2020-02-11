using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace WordDocEditing
{
	static class WordTableFormats
	{
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

		public static TableProperties TitleTableFormat()
		{
			TableBorders tblBorders = new TableBorders(
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
				);

			TableProperties tblProp = new TableProperties(
				tblBorders,
				new TableJustification() { Val = TableRowAlignmentValues.Center },
				new TableWidth() { Type = TableWidthUnitValues.Pct, Width = "4580" }
				);

			return tblProp;
		}
	}
}