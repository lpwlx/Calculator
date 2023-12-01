using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Calculator {
	public class CalculateFontSize : IMultiValueConverter {
		FontFamily FontFamily = System.Windows.SystemFonts.CaptionFontFamily;
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if (values.Length == 3) {
				double originalFontSize = 40.0;
				double textBoxWidth = (double)values[1];
				double textBoxHeight = (double)values[2];
				string text = values[0]?.ToString();

				double fontSize = originalFontSize;

				// Проверка, превышает ли ширина текста ширину TextBox
				while (TextWidthCalculator.GetTextWidth(text, FontFamily.Source, fontSize) > textBoxWidth) {
					fontSize /= 1.5;
				}

				return fontSize;
			}

			return 28.0; // Размер шрифта по умолчанию
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

			return formattedText.Width;
		}
	}
}
