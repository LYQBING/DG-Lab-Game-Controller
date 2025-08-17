using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using System.Diagnostics;
using DGLabGameController.Core.DGLabApi;
using DGLabGameController.Core.Config;

namespace DGLabGameController
{
	public partial class App : Application
	{
		private static Mutex? _mutex;

		// 程序启动时的唯一入口
		protected override void OnStartup(StartupEventArgs e)
		{
			// 确保单实例
			_mutex = new Mutex(true, "DGLabGameController_SingleInstanceMutex", out bool createdNew);
			if (!createdNew)
			{
				MessageBox.Show("再来一个是真的会坏掉的啦！", "程序正在运行", MessageBoxButton.OK, MessageBoxImage.Information);
				Current.Shutdown();
				return;
			}

			base.OnStartup(e);
			SettingsRepository.Load();
			InternalServerManager.StartServer();
			if (Current.Resources["MyNotifyIcon"] is TaskbarIcon notifyIcon) notifyIcon.TrayLeftMouseDown += ShowMainWindow_Click;
		}

		// 程序退出时的唯一入口
		protected override void OnExit(ExitEventArgs e)
		{
			// 停止服务器并保存数据
			InternalServerManager.StopServer();
			SettingsRepository.SaveAll();

			// 释放各种资源
			_mutex?.ReleaseMutex();
			_mutex?.Dispose();
			base.OnExit(e);
		}


		#region 托盘菜单事件

		// 启动主程序按钮事件
		private void ShowMainWindow_Click(object sender, RoutedEventArgs e)
		{
			// 确保窗口不为空且未被关闭
			if (Current.MainWindow == null) return;

			Current.MainWindow.Show();
			Current.MainWindow.WindowState = WindowState.Normal;
			Current.MainWindow.Activate();
		}

		// 启动控制台浏览器按钮事件
		private void OpenConsole_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = CoyoteApi.CoyotreUrl,
					UseShellExecute = true
				});
			}
			catch (Exception ex)
			{
				MessageBox.Show("无法打开浏览器: " + ex.Message);
			}
		}

		// 退出主程序按钮事件
		private void ExitMenu_Click(object sender, RoutedEventArgs e) => Current.Shutdown();
	}

	#endregion
}