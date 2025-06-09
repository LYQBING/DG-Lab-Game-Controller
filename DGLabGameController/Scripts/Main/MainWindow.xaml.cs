using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DGLabGameController
{
	public partial class MainWindow : Window
	{
		// 缓存的标准页面
		private FuncSelectPage? _funcPage;
		private LogPage? _logPage;
		private SettingPage? _settingPage;
		// 缓存的模块页面
		private IModule? _activeModule;
		private string? NowPage;

		#region 启动及关闭事件
		public MainWindow()
		{
			InitializeComponent();
			NavFunc_Click();
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs eventArgs)
		{
			if (ConfigManager.Current.ExitMenu)
			{
				eventArgs.Cancel = true;
				this.Hide();
				var notifyIcon = Application.Current.Resources["MyNotifyIcon"];
				return;
			}

			// 移除模块页面
			_activeModule?.OnModulePageClosed();
		}

		#endregion

		#region Dock相关

		public void NavFunc_Click(object? sender = null, RoutedEventArgs? e = null)
		{
			SetNavImages("Func");

			// 如果当前页面是模块页面，移除模块页面
			if (MainContent.Content == _activeModule?.GetPage() && _activeModule != null)
			{
				_activeModule.OnModulePageClosed();
				_activeModule = null;
			}
			// 否则如果模块已启动，显示模块页面
			else if (_activeModule != null)
			{
				MainContent.Content = _activeModule.GetPage();
				return;
			}

			_funcPage ??= new FuncSelectPage();
			MainContent.Content = _funcPage;
		}

		public void NavLog_Click(object? sender = null, RoutedEventArgs? e = null)
		{
			SetNavImages("Log");
			_logPage ??= new LogPage();
			MainContent.Content = _logPage;
		}

		public void NavSetting_Click(object? sender = null, RoutedEventArgs? e = null)
		{
			SetNavImages("Setting");
			_settingPage ??= new SettingPage();
			MainContent.Content = _settingPage;
		}

		private void SetNavImages(string page)
		{
			if (NowPage == page) return;
			else NowPage = page;

			FuncImage.Source = new BitmapImage(new Uri(
				page == "Func" ? "pack://application:,,,/Assets/icon/func_selected.png" : "pack://application:,,,/Assets/icon/func.png"));
			LogImage.Source = new BitmapImage(new Uri(
				page == "Log" ? "pack://application:,,,/Assets/icon/log_selected.png" : "pack://application:,,,/Assets/icon/log.png"));
			SettingImage.Source = new BitmapImage(new Uri(
				page == "Setting" ? "pack://application:,,,/Assets/icon/setting_selected.png" : "pack://application:,,,/Assets/icon/setting.png"));
		}

		#endregion

		#region 模块页面相关

		public void ShowModulePage(IModule module)
		{
			_activeModule = module;
			MainContent.Content = module.GetPage();
		}

		public void CloseActiveModule()
		{
			_activeModule?.OnModulePageClosed();
			_activeModule = null;

			NavFunc_Click();
		}

		#endregion
	}
}
