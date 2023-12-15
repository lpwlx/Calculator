using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NCalc;

namespace Calculator {
	public partial class MainWindow : Window {
		Button pressedButton = null;

		private Dictionary<Key, char> keys = new Dictionary<Key, char>()
		{
			{Key.D0, '0'}, {Key.D1, '1'}, {Key.D2, '2'}, {Key.D3, '3'}, {Key.D4, '4'},
			{Key.D5, '5'}, {Key.D6, '6'}, {Key.D7, '7'}, {Key.D8, '8'}, {Key.D9, '9'},
			{Key.NumPad0, '0'}, {Key.NumPad1, '1'}, {Key.NumPad2, '2'}, {Key.NumPad3, '3'}, {Key.NumPad4, '4'},
			{Key.NumPad5, '5'}, {Key.NumPad6, '6'}, {Key.NumPad7, '7'}, {Key.NumPad8, '8'}, {Key.NumPad9, '9'},
			{Key.Divide, '÷'}, {Key.Multiply, '×'}, {Key.Add, '+'}, {Key.Subtract, '–'},
			{Key.Decimal, '.'}, {Key.OemComma, '.'}, {Key.OemPeriod, '.'}
		};

		public MainWindow() {
			InitializeComponent();
			foreach (UIElement el in ButtonGrid.Children) {
				if (el is Button) {
					switch (((Button)el).Name) {
						case "erase":
							((Button)el).Click += EraseButtonClick;
							break;
						case "clear":
							((Button)el).Click += ClearButtonClick;
							break;
						case "copy":
							((Button)el).Click += CopyButtonClick;
							break;
						case "equals":
							((Button)el).Click += EqualsButtonClick;
							break;
						default:
							if (((Button)el).Content != null) {
								((Button)el).Click += ButtonClick;
							}
							break;
					}
				}
			}
			inOut.Focus();
		}

		private void CopyButtonClick(object sender, RoutedEventArgs e) {
			Clipboard.SetText(inOut.Text);
		}

		private void ClearButtonClick(object sender, RoutedEventArgs e) {
			HandleClearButtonClick();
		}

		private void HandleClearButtonClick() {
			inOut.Text = string.Empty;
		}

		private void EraseButtonClick(object sender, RoutedEventArgs e) {
			HandleEraseButtonClick();
		}

		private void HandleEraseButtonClick(bool isDelete = false) {
			pressedButton = erase;
			int caretIndex = inOut.CaretIndex;
			if (caretIndex > 0) {
				inOut.Text = inOut.Text.Remove(caretIndex -1 /*+ (isDelete ? 0 : -1)*/, 1);
				inOut.CaretIndex = caretIndex -1 /*+ (isDelete ? 0 : -1)*/;
			}
			inOut.Focus();
		}

		private void EqualsButtonClick(object sender, RoutedEventArgs e) {
			HandleEqualsButtonClick();
		}

		private void HandleEqualsButtonClick() {
			pressedButton = equals; //---------
			Calculate();
			inOut.CaretIndex = inOut.Text.Length;
		}

		private void ButtonClick(object sender, RoutedEventArgs e) {
			HandleButtonClick(sender as Button);
		}
		private void HandleButtonClick(Button button) {
			pressedButton = button; //----------
			int caretIndex = inOut.CaretIndex;
			inOut.Text = inOut.Text.Insert(inOut.CaretIndex, (string)button.Content); // вставка по позиции курсора
			inOut.CaretIndex = caretIndex + 1;
			inOut.Focus();
		}

		private void InOut_PreviewKeyDown(object sender, KeyEventArgs e) {
			e.Handled = true;
			switch (e.Key) {
				case Key.Left:
					if (inOut.CaretIndex > 0) {
						inOut.CaretIndex--;
					}
					break;
				case Key.Right:
					if (inOut.CaretIndex < inOut.Text.Length) {
						inOut.CaretIndex++;
					}
					break;
				case Key.Up:
					inOut.CaretIndex = 0;
					break;
				case Key.Down:
					inOut.CaretIndex = inOut.Text.Length;
					break;
				case Key.Enter:
					HandleEqualsButtonClick();
					break;
				case Key.Back: // нельзя падать по кейсам, ошибка CS0163
					HandleEraseButtonClick();
					break;
				case Key.Delete:
					HandleEraseButtonClick(true);
					break;
				default:
					char sym;
					keys.TryGetValue(e.Key, out sym);
					Button button = null;
					bool flag = false;
					foreach (UIElement el in ButtonGrid.Children) {
						if (el is Button && ((Button)el).Content != null) {
							if (sym == ((string)((Button)el).Content)[0]) {
								button = (Button)el;
								flag = true;
							}
						}
						if (flag) break;
					}
					if (button != null) {
						HandleButtonClick(button);
						SetButtonPressedState(button, true);
					}
					break;
			}
		}

		private void InOut_PreviewTextInput(object sender, TextCompositionEventArgs e) {
			//e.Handled = true; // блокировка ввода текста
		}
		private void InOut_PreviewKeyUp(object sender, KeyEventArgs e) {
			SetButtonPressedState(pressedButton, false);
		}
		private void SetButtonPressedState(Button button, bool isPressed) {
			if (button != null) {
				//button.SetValue(Button.IsPressedProperty, isPressed);
			}
		}
		/*
		private void Calculate() {
			string pattern = @"√(\d+)"; // поиск символов корня и их следующих цифр
			Regex regex = new Regex(pattern);
			string processedInput = regex.Replace(inOut.Text, "√($1)");
			try {
				NCalc.Expression expression = new NCalc.Expression(processedInput.Replace('–', '-').Replace('×', '*').Replace('÷', '/').Replace("√", "Sqrt"));

				object result = expression.Evaluate();

				if (result != null) {
					inOut.Text = result.ToString().Replace(',', '.'); // ToString возвращает запятую (?)
				}
				else {
					MessageBox.Show("Error");
				}
			}
			catch (EvaluationException ex) {
				MessageBox.Show("Error: " + ex.Message);
			}
		}
		*/
		private void Calculate() {
			string patternDecimal = @"(?<=\D|^)\.(\d+)"; // поиск чисел вида ".123"
			Regex regexDecimal = new Regex(patternDecimal);
			string processedInput = regexDecimal.Replace(inOut.Text, "0.$1"); // добавление нуля перед точкой

			string patternSqrt = @"√(\d+(\.\d+)?)"; // поиск символов корня и их следующих цифр (включая дробные числа)
			Regex regexSqrt = new Regex(patternSqrt);
			processedInput = regexSqrt.Replace(processedInput, "√($1)");

			string patternPow = @"(\(.+?\)|\d+(\.\d+)?|\w+)\^(\(.+?\)|\d+(\.\d+)?|\w+)"; // поиск выражений вида x^y (включая дробные числа)
			Regex regexPow = new Regex(patternPow);
			processedInput = regexPow.Replace(processedInput, "Pow($1, $3)");

			try {
				NCalc.Expression expression = new NCalc.Expression(processedInput.Replace('–', '-').Replace('×', '*').Replace('÷', '/').Replace("√", "Sqrt"));

				object result = expression.Evaluate();

				if (result != null) {
					inOut.Text = result.ToString().Replace(',', '.'); // ToString возвращает запятую (?)
				}
				else {
					MessageBox.Show("Error");
				}
			}
			catch (EvaluationException ex) {
				MessageBox.Show("Error: " + ex.Message);
			}
		}
	}
}
