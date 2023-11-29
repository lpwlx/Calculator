using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Calculator {
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
