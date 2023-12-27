using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Calculator {
	public class CalculateFontSize : IMultiValueConverter {
		FontFamily FontFamily = System.Windows.SystemFonts.CaptionFontFamily;
		public double InitialFontSize { get; set; }
		public double MinFontSize { get; set; }
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if (values.Length == 3) {
				double fontSize = InitialFontSize;
				double minFontSize = MinFontSize; // минимальный размер текста
				double textBoxWidth = (double)values[1];
				string text = values[0]?.ToString();

				// превышает ли ширина текста ширину TextBox
				
				while ((TextWidthCalculator.GetTextWidth(text, FontFamily.Source, fontSize) > textBoxWidth - 5) && fontSize >= minFontSize) { // зазор в пять пикселей
					fontSize /= 1.5;
				}
				
				return fontSize;
			}
			return InitialFontSize; // если параметров текстбокса не три, вернуть изначальный размер
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}

	public static class TextWidthCalculator {
		public static double GetTextWidth(string text, string fontFamily, double fontSize) {
			FormattedText formattedText = new FormattedText(
				text,
				CultureInfo.CurrentCulture,
				FlowDirection.LeftToRight,
				new Typeface(new FontFamily(fontFamily), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
				fontSize,
				Brushes.Black,
				new NumberSubstitution(),
				1.0); // PixelsPerDip

			return formattedText.WidthIncludingTrailingWhitespace;
		}
	}
}
