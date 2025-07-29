using DGLabGameController.Core.Config;
using DGLabGameController.Core.Debug;
using DGLabGameController.Core.DGLabApi;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DGLabGameController
{
	public partial class SettingPage : UserControl
	{
		private readonly SettingsConfig config = SettingsRepository.Current;

		public SettingPage()
		{
			InitializeComponent();
			LoadSettingPage();
		}

		private void LoadSettingPage()
		{
			// 服务器模式
			ServerInternalMode.Visibility = config.ServerMode ? Visibility.Visible : Visibility.Collapsed;
			UseInternalServerToggle.IsChecked = config.ServerMode;

			// 服务器连接设置
			ServerIP.Text = config.ServerUrl;
			ServerPortText.Text = config.ServerPort.ToString();
			ClientIdText.Text = config.ClientId;

			// 内置服务器设置
			ListenAddressText.Text = config.ServerHost;
			WaveFilePathText.Text = config.PulseConfigPath;
			BroadcastAllClientsToggle.IsChecked = config.AllowBroadcastToClients;
			ShowTerminalManagerToggle.IsChecked = config.DisplayPowerShell;
			OpenConsoleOnStartToggle.IsChecked = config.OpenBrowser;

			// 客户端设置
			VerboseLogToggle.IsChecked = config.VerboseLogs;
			MinimizeOnExitToggle.IsChecked = config.ExitMenu;
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

		#region 服务器模式设置

		// 是否启用内置服务器按钮
		private void UseInternalServerToggle_Checked(object sender, RoutedEventArgs e) => SetServerMode(true);
		private void UseInternalServerToggle_Unchecked(object sender, RoutedEventArgs e) => SetServerMode(false);

		private void SetServerMode(bool mode)
		{
			ServerInternalMode.Visibility = mode ? Visibility.Visible : Visibility.Collapsed;
			config.ServerMode = mode;
		}

		#endregion

		#region 服务器连接设置

		// 服务器地址
		private void ServerAddress_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("服务器地址", "null", ServerIP.Text, "设定",
			(data) =>
			{
				if (!string.IsNullOrEmpty(data.InputText))
				{
					ServerIP.Text = data.InputText;
					config.ServerUrl = data.InputText;
				}
				else DebugHub.Warning("设置未生效", "杂鱼主人！居然什么也不输入？！如果身体真的不行就逃走吧");

				data.Close();
			}, "取消",
			(data) => data.Close()).ShowDialog();
		}

		// 服务器端口
		private void ServerPort_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("服务器端口", "null", ServerPortText.Text, "设定", data =>
			{
				if (!string.IsNullOrEmpty(data.InputText) && int.TryParse(data.InputText, out int value))
				{
					ServerPortText.Text = data.InputText;
					config.ServerPort = value;
				}
				else DebugHub.Warning("设置未生效", "嗯...您确定这是一个正常的端口号吗？主人！");

				data.Close();
			}, "取消",data => data.Close()).ShowDialog();
		}

		// 客户端标识
		private void ClientId_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("客户端标识", "控制指定设备的唯一标识，默认参数为全部设备", ClientIdText.Text, "设定",
			(data) =>
			{
				if (!string.IsNullOrEmpty(data.InputText))
				{
					ClientIdText.Text = data.InputText;
					config.ClientId = data.InputText;
				}
				else DebugHub.Warning("设置未生效", "杂鱼主人！您确定要留空吗？如果不想被调教就赶紧逃走吧！");
				data.Close();
			}, "取消",
			(data) => data.Close()).ShowDialog();
		}

		#endregion

		#region 内置服务器设置

		// 监听地址
		private void ListenAddress_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("监听地址", "服务器所使用的网络接口：若你不知道是什么请保持默认", ListenAddressText.Text, "设定", 
			(data) =>
			{
				if (!string.IsNullOrEmpty(data.InputText))
				{
					ListenAddressText.Text = data.InputText;
					config.ServerHost = data.InputText;
				}
				data.Close();
			}, "取消",
			(data) => data.Close()).ShowDialog();
		}

		// 波形文件路径
		private void WaveFilePath_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("波形文件路径", "服务器端将会读取此路径的所有波形哦", WaveFilePathText.Text, "设定", 
			(data) =>
			{
				if (!string.IsNullOrEmpty(data.InputText))
				{ 
					WaveFilePathText.Text = data.InputText;
					config.PulseConfigPath = data.InputText; 
				}
				data.Close();
			}, "取消",
			(data) => data.Close()).ShowDialog();
		}

		// 向所有客户端广播消息
		private void BroadcastAllClientsToggle_Checked(object sender, RoutedEventArgs e) => config.AllowBroadcastToClients = true;
		private void BroadcastAllClientsToggle_Unchecked(object sender, RoutedEventArgs e) => config.AllowBroadcastToClients = false;


		// 显示终端管理器
		private void ShowTerminalManagerToggle_Checked(object sender, RoutedEventArgs e) => config.DisplayPowerShell = true;
		private void ShowTerminalManagerToggle_Unchecked(object sender, RoutedEventArgs e) => config.DisplayPowerShell = false;

		// 启动时打开控制台
		private void OpenConsoleOnStartToggle_Checked(object sender, RoutedEventArgs e) => config.OpenBrowser = true;
		private void OpenConsoleOnStartToggle_Unchecked(object sender, RoutedEventArgs e) => config.OpenBrowser = false;

		#endregion

		#region 客户端设置

		// 详细日志输出
		private void VerboseLogToggle_Checked(object sender, RoutedEventArgs e) => config.VerboseLogs = true;
		private void VerboseLogToggle_Unchecked(object sender, RoutedEventArgs e) => config.VerboseLogs = false;

		// 退出时最小化
		private void MinimizeOnExitToggle_Checked(object sender, RoutedEventArgs e) => config.ExitMenu = true;
		private void MinimizeOnExitToggle_Unchecked(object sender, RoutedEventArgs e) => config.ExitMenu = false;

		#endregion
	}
}
