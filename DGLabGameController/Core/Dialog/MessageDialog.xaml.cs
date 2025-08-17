using System.Windows;
using System.Windows.Input;

namespace DGLabGameController
{
	/// <summary>
	/// 基础型对话框
	/// </summary>
	public partial class MessageDialog : Window
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="title">标题</param>
		/// <param name="message">内容</param>
		/// <param name="button1Text">主按钮</param>
		/// <param name="button1Action">主按钮事件</param>
		/// <param name="button2Text">副按钮</param>
		/// <param name="button2Action">副按钮事件</param>
		/// <param name="owner">窗口位置</param>
		public MessageDialog(string title, string message, string button1Text, Action<MessageDialog> button1Action, string? button2Text = null, Action<MessageDialog>? button2Action = null, Window? owner = null)
		{
			InitializeComponent();
			Owner = owner ?? Application.Current.MainWindow;

			TitleText.Text = title;
			MessageText.Text = message;

			// 设置按钮1
			Button1.Content = button1Text;
			Button1.Click += (s, e) => button1Action?.Invoke(this);

			// 设置按钮2
			if (!string.IsNullOrWhiteSpace(button2Text))
			{
				Button2.Content = button2Text;
				Button2.Click += (s, e) => (button2Action ?? (_ => Close()))(this);
				Button2.Visibility = Visibility.Visible;
			}
			else Button2.Visibility = Visibility.Collapsed;
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left) DragMove();
		}
	}
}
