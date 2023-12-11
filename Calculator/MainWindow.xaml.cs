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

namespace Calculator
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		Button pressedButton = null;

		public MainWindow()
		{
			InitializeComponent();
			foreach (UIElement el in ButtonGrid.Children)
			{
				if (el is Button)
				{
					switch (((Button)el).Name)
					{
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
							((Button)el).Click += ButtonClick;
							break;
					}
				}
			}
			inOut.Focus();
		}

		private void CopyButtonClick(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException(); //---------------
		}

		private void ClearButtonClick(object sender, RoutedEventArgs e)
		{
			HandleClearButtonClick();
		}

		private void HandleClearButtonClick()
		{
			throw new NotImplementedException(); //-----------
		}

		private void EraseButtonClick(object sender, RoutedEventArgs e)
		{
			HandleEraseButtonClick();
		}

		private void HandleEraseButtonClick()
		{
			pressedButton = erase;
			int caretIndex = inOut.CaretIndex;
			if (caretIndex > 0)
			{
				inOut.Text = inOut.Text.Remove(caretIndex - 1, 1);
				inOut.CaretIndex = caretIndex - 1;
			}
			inOut.Focus();
		}

		private void EqualsButtonClick(object sender, RoutedEventArgs e)
		{
			HandleEqualsButtonClick();
		}

		private void HandleEqualsButtonClick()
		{
			pressedButton = equals;
			//-------------------
		}

		private void ButtonClick(object sender, RoutedEventArgs e)
		{
			/*
			//string str = e.OriginalSource as string;
			string str = (string)((Button)e.OriginalSource).Content;
			int caretIndex = inOut.CaretIndex;
			inOut.Text = inOut.Text.Insert(inOut.CaretIndex, str); // вставка по позиции курсора
			inOut.CaretIndex = caretIndex + 1;
			inOut.Focus(); 
			*/
			HandleButtonClick(sender as Button);
		}
		private void HandleButtonClick(Button button)
		{
			pressedButton = button;
			int caretIndex = inOut.CaretIndex;
			inOut.Text = inOut.Text.Insert(inOut.CaretIndex, (string)button.Content); // вставка по позиции курсора
			inOut.CaretIndex = caretIndex + 1;
			inOut.Focus();
		}

		private void InOut_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Left:
					if (inOut.CaretIndex > 0)
					{
						inOut.CaretIndex--;
					}
					e.Handled = true;
					break;
				case Key.Right:
					if (inOut.CaretIndex < inOut.Text.Length)
					{
						inOut.CaretIndex++;
					}
					e.Handled = true;
					break;
				case Key.Space:
					e.Handled = true;
					break;
				case Key.Enter:
					e.Handled = true;
					HandleEqualsButtonClick();
					Calculate();
					inOut.CaretIndex = inOut.Text.Length;
					break;
				case Key.Back:
					e.Handled = true;
					HandleEraseButtonClick();
					break;
				case Key.OemComma:
					e.Handled = true;
					HandleButtonClick(point);
					break;
				default:
					e.Handled = true;
					string str = e.Key.ToString();
					Button button = null;
					bool flag = false;
					foreach (UIElement el in ButtonGrid.Children)
					{
						if (el is Button && ((Button)el).Content != null)
						{
							if (str == ((string)((Button)el).Content))
							{
								button = (Button)el;
								flag = true;
							}
							else
							{
								switch (str)
								{
									case ",":
										button = point;
										flag = true;
										break;
									case "*":
										button = multiply;
										flag = true;
										break;
									case "/":
										button = divide;
										flag = true;
										break;
									case "-":
										button = minus;
										flag = true;
										break;
										/*
										case '\r':
											button = erase;
											flag = true;
											break;
										case '\u007F':
											button = clear;
											flag = true;
											break;
										*/
								}
							}
						}
						if (flag) break;
					}
					if (button != null)
					{
						HandleButtonClick(button);
						SetButtonPressedState(button, true);
					}
					break;
			}
		}

		private void InOut_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = true; // блокировка ввода текста
			/*
			if (Char.IsDigit(e.Text[0]) || e.Text == "+" || e.Text == "-" || e.Text == "*" || e.Text == "/" || e.Text == "^" || e.Text == "." || e.Text == "," || e.Text == "(" || e.Text == ")") {
				int caretIndex = inOut.CaretIndex;
				inOut.Text = inOut.Text.Insert(inOut.CaretIndex, e.Text); // вставка по позиции курсора
				inOut.CaretIndex = caretIndex + 1;
			}
			*/
			/*
			Button button = null;
			foreach (UIElement el in ButtonGrid.Children) {
				if (el is Button) {
					if ((string)((Button)el).Content == e.Text) {
						button = (Button)el;
					}
					else {
						switch (e.Text[0]) {
							case ',':
								button = point;
								break;
							case '*':
								button = multiply;
								break;
							case '/':
								button = divide;
								break;
							case '\r':
								button = erase;
								break;
							case '\u007F':
								button = clear;
								break;
						}
					}
					//button.SetValue(Button.IsPressedProperty, isPressed);
				}
			}
			*/
		}
		private void InOut_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			SetButtonPressedState(pressedButton, false);
		}
		private void SetButtonPressedState(Button button, bool isPressed)
		{
			if (button != null)
			{
				//button.SetValue(Button.IsPressedProperty, isPressed);
			}
		}
		private void Calculate()
		{
			string pattern = @"√(\d+)"; // поиск символов корня и их следующих цифр
			Regex regex = new Regex(pattern);
			string processedInput = regex.Replace(inOut.Text, "√($1)");
			try
			{
				NCalc.Expression expression = new NCalc.Expression(processedInput.Replace('–', '-')/*.Replace(',', '.')*/.Replace("√", "Sqrt"));
				object result = expression.Evaluate();

				if (result != null)
				{
					inOut.Text = result.ToString();
				}
				else
				{
					inOut.Text = "Error";
				}
			}
			catch (EvaluationException ex)
			{
				inOut.Text = "Error: " + ex.Message;
			}
		}
	}
}
