using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DGLabGameController
{
	public partial class InputDialog : Window
	{
		public string InputText => InputTextBox.Text;

		public InputDialog(string title, string message, string inputText, string button1Text, string button2Text, Action<InputDialog>? button1Action, Action<InputDialog>? button2Action)
		{
			InitializeComponent();
			Owner = Application.Current.MainWindow;

			TitleText.Text = title;
			InputTextBox.Text = inputText;
			if (message == "null") MessageText.Visibility = Visibility.Collapsed;
			else MessageText.Text = message;

			// 设置按钮1
			Button1.Content = button1Text;
			Button1.Click += (s, e) => button1Action?.Invoke(this);

			// 设置按钮2
			Button2.Content = button2Text;
			Button2.Click += (s, e) => button2Action?.Invoke(this);
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left) this.DragMove();
		}

	}
}