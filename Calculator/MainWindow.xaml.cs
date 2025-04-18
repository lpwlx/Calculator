﻿using System;
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
		private Dictionary<Key, char> keys = new Dictionary<Key, char>()
		{
			{Key.D0, '0'}, {Key.D1, '1'}, {Key.D2, '2'}, {Key.D3, '3'}, {Key.D4, '4'},
			{Key.D5, '5'}, {Key.D6, '6'}, {Key.D7, '7'}, {Key.D8, '8'}, {Key.D9, '9'},
			{Key.NumPad0, '0'}, {Key.NumPad1, '1'}, {Key.NumPad2, '2'}, {Key.NumPad3, '3'}, {Key.NumPad4, '4'},
			{Key.NumPad5, '5'}, {Key.NumPad6, '6'}, {Key.NumPad7, '7'}, {Key.NumPad8, '8'}, {Key.NumPad9, '9'},
			{Key.Divide, '÷'}, {Key.Multiply, '×'}, {Key.Add, '+'}, {Key.Subtract, '–'}, {Key.Decimal, '.'},
			{Key.OemQuestion, '÷'}, {Key.OemComma, '.'}, {Key.OemPeriod, '.'}, {Key.OemMinus, '–'}
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
			HandleCopyButtonClick();
			inOut.Focus();
		}
		private void HandleCopyButtonClick() {
			Clipboard.SetText(inOut.Text);
		}
		private void ClearButtonClick(object sender, RoutedEventArgs e) {
			HandleClearButtonClick();
			inOut.Focus();
		}
		private void HandleClearButtonClick() {
			inOut.Text = string.Empty;
		}
		private void EraseButtonClick(object sender, RoutedEventArgs e) {
			HandleEraseButtonClick();
			inOut.Focus();
		}
		private void HandleEraseButtonClick(bool isDelete = false) {
			int caretIndex = inOut.CaretIndex;
			if (isDelete ? caretIndex < inOut.Text.Length : caretIndex > 0) {
				inOut.Text = inOut.Text.Remove(caretIndex + (isDelete ? 0 : -1), 1);
				inOut.CaretIndex = caretIndex + (isDelete ? 0 : -1);
			}
		}
		private void EqualsButtonClick(object sender, RoutedEventArgs e) {
			HandleEqualsButtonClick();
			inOut.Focus();
		}
		private void HandleEqualsButtonClick() {
			Calculate();
			inOut.CaretIndex = inOut.Text.Length;
		}
		private void ButtonClick(object sender, RoutedEventArgs e) {
			HandleButtonClick(sender as Button);
			inOut.Focus();
		}
		private void HandleButtonClick(Button button) {
			int caretIndex = inOut.CaretIndex;
			inOut.Text = inOut.Text.Insert(inOut.CaretIndex, (string)button.Content); // вставка по позиции курсора
			inOut.CaretIndex = caretIndex + 1;
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
					SetButtonPressedState(equals, true);
					HandleEqualsButtonClick();
					break;
				case Key.Back:
					SetButtonPressedState(erase, true);
					HandleEraseButtonClick();
					break;
				case Key.Delete:
					SetButtonPressedState(erase, true);
					HandleEraseButtonClick(true);
					break;
				case Key.C:
					if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
						SetButtonPressedState(copy, true);
						HandleCopyButtonClick();
					}
					break;
				default:
					char sym;
					if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) { // при нажатии shift
						switch (e.Key) {
							case Key.D6:
								sym = '^';
								break;
							case Key.D8:
								sym = '×';
								break;
							case Key.D9:
								sym = '(';
								break;
							case Key.D0:
								sym = ')';
								break;
							case Key.OemPlus:
								sym = '+';
								break;
							default:
								sym = '\0';
								break;
						}
					}
					else {
						if (e.Key == Key.OemPlus) {
							SetButtonPressedState(equals, true);
							HandleEqualsButtonClick();
							break;
						}
						keys.TryGetValue(e.Key, out sym);
					}
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
		private void InOut_PreviewKeyUp(object sender, KeyEventArgs e) {
			switch (e.Key) {
				case Key.Enter:
					SetButtonPressedState(equals, false);
					break;
				case Key.Back:
				case Key.Delete:
					SetButtonPressedState(erase, false);
					break;
				case Key.C:
					SetButtonPressedState(copy, false);
					break;
				default:
					char sym;
					if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
						switch (e.Key) {
							case Key.D6:
								sym = '^';
								break;
							case Key.D8:
								sym = '×';
								break;
							case Key.D9:
								sym = '(';
								break;
							case Key.D0:
								sym = ')';
								break;
							case Key.OemPlus:
								sym = '+';
								break;
							default:
								sym = '\0';
								break;
						}
					}
					else {
						if (e.Key == Key.OemPlus) {
							SetButtonPressedState(equals, false);
							break;
						}
						keys.TryGetValue(e.Key, out sym);
					}

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
						SetButtonPressedState(button, false);
					}
					break;
			}
		}
		private void SetButtonPressedState(Button button, bool isPressed) {
			if (button != null) {
				if (button != null) {
					if (isPressed) {
						VisualStateManager.GoToState(button, "MyPressedState", true);
					}
					else {
						VisualStateManager.GoToState(button, "MyNormalState", true);
					}
				}
			}
		}

		private void Calculate() {
			string patternDecimal = @"(?<=\D|^)\.(\d+)"; // числа без указания нуля в целой части
			Regex regexDecimal = new Regex(patternDecimal);
			string processedInput = regexDecimal.Replace(inOut.Text, "(0.$1)");

			string patternShortMult = @"(\(.+?\)|\d+(\.\d+)?|\w+)\((.+?)\)"; // число перед скобками (включая дробные числа)
			Regex regexShortMult = new Regex(patternShortMult);
			processedInput = regexShortMult.Replace(processedInput, "($1*($3))");

			string patternShortMultSqrt = @"(\(.+?\)|\d+(\.\d+)?|\w+)√(\(.+?\)|\d+(\.\d+)?|\w+)"; // число перед корнем (включая дробные числа)
			Regex regexShortMultSqrt = new Regex(patternShortMultSqrt);
			processedInput = regexShortMultSqrt.Replace(processedInput, "($1*(√($3)))");

			string patternSqrt = @"(√(\d+(\.\d+)?))"; // корень без скобок (включая дробные числа)
			Regex regexSqrt = new Regex(patternSqrt);
			processedInput = regexSqrt.Replace(processedInput, "(√($1))");

			string patternPow = @"(\(.+?\)|\d+(\.\d+)?|\w+)\^(\(.+?\)|\d+(\.\d+)?|\w+)"; // выражения вида x^y (включая дробные числа)
			Regex regexPow = new Regex(patternPow);
			processedInput = regexPow.Replace(processedInput, "(Pow($1, $3))");

			string patternPi = @"(\d+(\.\d+)?)π"; // число перед π (включая дробные числа)
			Regex regexPi = new Regex(patternPi);
			processedInput = regexPi.Replace(processedInput, "($1*Pi)");

			if (inOut.Text != "") {
				try {
					NCalc.Expression expression = new NCalc.Expression(processedInput.Replace('–', '-').Replace('×', '*').Replace('÷', '/').Replace("√", "Sqrt").Replace("π", "Pi"));
					expression.Parameters["Pi"] = Math.PI;

					object result = expression.Evaluate();

					if (result != null) {
						inOut.Text = result.ToString().Replace(',', '.'); // ToString возвращает запятую (?)
					}
					else {
						MessageBox.Show("Error");
						SetButtonPressedState(equals, false);
					}
				}
				catch (/*EvaluationException*/Exception ex) {
					MessageBox.Show("Error: " + ex.Message);
					SetButtonPressedState(equals, false);
				}
			}
		}
	}
}
