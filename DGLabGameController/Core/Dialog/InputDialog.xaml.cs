using System.Windows;
using System.Windows.Input;

namespace DGLabGameController
{
	/// <summary>
	/// 输入型对话框
	/// </summary>
	public partial class InputDialog : Window
	{
		public string InputText => InputTextBox.Text;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="title">标题</param>
		/// <param name="message">内容</param>
		/// <param name="inputText">输入框中的默认内容</param>
		/// <param name="button1Text">主按钮</param>
		/// <param name="button1Action">主按钮事件</param>
		/// <param name="button2Text">副按钮</param>
		/// <param name="button2Action">副按钮事件</param>
		/// <param name="owner">窗口位置</param>
		public InputDialog(string title, string message, string inputText, string button1Text, Action<InputDialog>? button1Action, string button2Text, Action<InputDialog>? button2Action, Window ?owner = null)
		{
			InitializeComponent();
			if(owner != null) Owner = owner;
			else Owner = Application.Current.MainWindow;

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
			if (e.ChangedButton == MouseButton.Left) DragMove();
		}

	}
}