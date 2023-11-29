using System;
using System.Globalization;
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

				// Регулировка размера шрифта в зависимости от ваших критериев
				double fontSize = originalFontSize;

				// Проверка, превышает ли ширина текста ширину TextBox
				while (TextWidthCalculator.GetTextWidth(text, FontFamily.Source, fontSize) > textBoxWidth) {
					// Если да, уменьшить размер шрифта вдвое и повторить проверку
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
}
