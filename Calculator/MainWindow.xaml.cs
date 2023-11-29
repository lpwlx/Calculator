using System;
using System.Collections.Generic;
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
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
			foreach (UIElement el in ButtonGrid.Children) {
				if (el is Button) {
					((Button)el).Click += ButtonClick;
				}
			}
			//inOut.Cursor = Cursors.IBeam;

		}

		private void Calculate() {
			string pattern = @"√(\d+)"; // поиск символов корня и их следующих цифр
			Regex regex = new Regex(pattern);
			string processedInput = regex.Replace(inOut.Text, "√($1)");
			try {
				NCalc.Expression expression = new NCalc.Expression(processedInput.Replace(',', '.').Replace("√", "Sqrt"));
				object result = expression.Evaluate();

				if (result != null) {
					inOut.Text = result.ToString();
				}
				else {
					inOut.Text = "Error";
				}
			}
			catch (EvaluationException ex) {
				inOut.Text = "Error: " + ex.Message;
			}
		}

		private void ButtonClick(object sender, RoutedEventArgs e) {
			//string str = e.OriginalSource as string;
			string str = (string)((Button)e.OriginalSource).Content;
			inOut.Focus();
		}

		private void InOutTextBox_PreviewKeyDown(object sender, KeyEventArgs e) {
			switch (e.Key) {
				case Key.Left:
					if (inOut.CaretIndex > 0)
						inOut.CaretIndex--;
					e.Handled = true;
					break;
				case Key.Right:
					if (inOut.CaretIndex < inOut.Text.Length)
						inOut.CaretIndex++;
					e.Handled = true;
					break;
				case Key.Space:
					e.Handled = true;
					break;
				case Key.Enter:
					e.Handled = true;
					Calculate();
					inOut.CaretIndex = inOut.Text.Length;
					break;
			}
		}

		private void InOutTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
			e.Handled = true; // блокировка ввода текста
			if (Char.IsDigit(e.Text[0]) || e.Text == "+" || e.Text == "-" || e.Text == "*" || e.Text == "/" || e.Text == "^" || e.Text == "." || e.Text == "," || e.Text == "(" || e.Text == ")") {
				int caretIndex = inOut.CaretIndex;
				inOut.Text = inOut.Text.Insert(inOut.CaretIndex, e.Text); // вставка по позиции курсора
				inOut.CaretIndex = caretIndex + 1;
			}
		}
	}


	/*
	private void InOut_PreviewKeyDown(object sender, KeyEventArgs e) {
		TextBox textBox = (TextBox)sender;

		switch (e.Key) {
			case Key.Left:
				if (textBox.CaretIndex > 0) {
					textBox.CaretIndex--;
				}
				e.Handled = true;
				break;
			case Key.Right:
				if (textBox.CaretIndex < textBox.Text.Length) {
					textBox.CaretIndex++;
				}
				e.Handled = true;
				break;
				// Другие обработки клавиш, если необходимо
		}
	}
	*/
}
