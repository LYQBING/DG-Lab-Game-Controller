using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using System.Diagnostics;
using lyqbing.DGLAB;

namespace DGLabGameController
{
	public partial class App : Application
	{
		private const string UniqueMutexName = "DGLabGameController_SingleInstanceMutex";
		public const int ApiVersion = 10086;
		private static Mutex? _mutex;

		protected override void OnStartup(StartupEventArgs e)
		{
			// 确保单实例
			_mutex = new Mutex(true, UniqueMutexName, out bool createdNew);
			if (createdNew)
			{
				// 初始化程序
				base.OnStartup(e);
				ConfigManager.Load();
				DebugHub.Log("一切准备就绪", "欢迎回来！我们将一直保持免费且开源：关注开发者项目以表支持");
                DebugHub.Log("程序 API 版本", $"{ApiVersion}");
                DebugHub.Success("欢迎加入官方项目群聊", "928175340");
                // 创建程序托盘
                if (Current.Resources["MyNotifyIcon"] is TaskbarIcon notifyIcon) notifyIcon.TrayLeftMouseDown += ShowMainWindow_Click;
			}
			else
			{
                MessageBox.Show("再来一个是真的会坏掉的啦！", "程序正在运行", MessageBoxButton.OK, MessageBoxImage.Information);
                Current.Shutdown();
                return;
            }
		}

		protected override void OnExit(ExitEventArgs e)
		{
			// 关闭服务
			InternalServerManager.StopServer();

			// 保存数据
			ConfigManager.SaveConfig();
			ConfigManager.SaveServerConfig();

			if (_mutex != null)
			{
				_mutex.ReleaseMutex();
				_mutex.Dispose();
			}

			base.OnExit(e);
		}

		// 启动主程序
		private void ShowMainWindow_Click(object sender, RoutedEventArgs e)
		{
			// 确保窗口不为空且未被关闭
			if (Current.MainWindow == null)
				return;

			Current.MainWindow.Show();
			Current.MainWindow.WindowState = WindowState.Normal;
			Current.MainWindow.Activate();
		}

		// 启动控制台浏览器
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

		// 退出主程序
		private void ExitMenu_Click(object sender, RoutedEventArgs e) => Current.Shutdown();
	}
}