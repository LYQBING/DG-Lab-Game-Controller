using DGLabGameController;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DGLabGameController
{
	public partial class MessageDialog : Window
	{
		public MessageDialog(string title, string message, string button1Text, Action<MessageDialog> button1Action, string? button2Text = null, Action<MessageDialog>? button2Action = null)
		{
			InitializeComponent();
			Owner = Application.Current.MainWindow;

			TitleText.Text = title;
			MessageText.Text = message;

			// 设置按钮1
			Button1.Content = button1Text;
			Button1.Click += (s, e) => button1Action?.Invoke(this);

			// 设置按钮2
			if (!string.IsNullOrWhiteSpace(button2Text))
			{
				Button2.Content = button2Text;
				Button2.Click += (s, e) => button2Action?.Invoke(this);

				Button2.Visibility = Visibility.Visible;
			}
			else Button2.Visibility = Visibility.Collapsed;
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left) this.DragMove();
		}
	}
}
