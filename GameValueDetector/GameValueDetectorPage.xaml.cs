using DGLabGameController;
using DGLabGameController.Core.Config;
using DGLabGameController.Core.Debug;
using GameValueDetector.Models;
using GameValueDetector.Services;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace GameValueDetector
{
	public partial class GameValueDetectorPage : UserControl
	{
		public static int SleepTime { get; set; } = 200; // 检测间隔时间，单位毫秒
		public static int PenaltyValue { get; set; } = 30; // 惩罚输出值，单位为游戏内数值

		private GameMonitorConfig? _config; // 当前脚本配置
		private readonly Dictionary<string, ValueHistory> _valueHistories = []; // 历史值字典
		private CancellationTokenSource? _cts; // 取消令牌源，用于停止检测
		private readonly string _moduleFolderPath = ""; // 模块目录路径

		public GameValueDetectorPage(string moduleId)
		{
			InitializeComponent();

			SleepTimeText.Text = SleepTime.ToString();
			PenaltyValueText.Text = PenaltyValue.ToString();
			_moduleFolderPath = Path.Combine(AppConfig.ModulesPath, moduleId, "Archive");
			RefreshProcessButton_Click();
		}

		#region 管理按钮事件

		public void Back_Click(object sender, RoutedEventArgs e)
		{
			if (Application.Current.MainWindow is MainWindow mw) mw.CloseActiveModule();
			else DebugHub.Warning("返回失败", "主人...我不知道该回哪里去呢？");
		}

		private void Edit_Click(object sender, RoutedEventArgs e)
		{
			// 打开模块目录
			if (!Directory.Exists(_moduleFolderPath)) Directory.CreateDirectory(_moduleFolderPath);
			new MessageDialog("管理基址脚本", "管理脚本操作将在资源管理器中进行，是否继续前往？", "继续", data =>
			{
				Process.Start("explorer.exe", _moduleFolderPath);
				data.Close();
			}, "取消").ShowDialog();
		}

		private void RefreshProcessButton_Click(object ?sender = null, RoutedEventArgs ?e = null)
		{
			ProcessComboBox.ItemsSource = ProcessManager.GetProcessList(); // 刷新进程列表
			JsonFileListBox.ItemsSource = ScriptManager.GetScriptFiles(_moduleFolderPath); // 刷新脚本列表
		}

		private async void ToggleCheckButton_Click(object sender, RoutedEventArgs e)
		{
			if (_cts != null)
			{
				_cts.Cancel();
				_cts = null;
				return;
			}

			if (_config is null)
			{
				new MessageDialog("未选择脚本", "主人，还没有选择需要执行的脚本哦", "好的", d => d.Close()).ShowDialog();
				return;
			}

			Process? process = ProcessComboBox.SelectedItem as Process;
			if (process == null)
			{
				ProcessComboBox.ItemsSource = ProcessManager.GetProcessList();
				if (ProcessComboBox.ItemsSource is IEnumerable<Process> processes) 
				{
					process = ProcessManager.FindProcessByName(processes, _config.ProcessName);
					if (process == null)
					{
						new MessageDialog("请选择进程", "未能自动找到指定进程，请手动完成进程选择", "好的", d => d.Close()).ShowDialog();
						ProcessComboGrid.Visibility = Visibility.Visible;
						return;
					}
					ProcessComboGrid.Visibility = Visibility.Collapsed;
				}
				else
				{
					new MessageDialog("进程列表异常", "无法获取进程列表，请重试", "好的", d => d.Close()).ShowDialog();
					return;
				}
			}

			_cts = new CancellationTokenSource();
			ToggleCheckButton.Content = "停止检测";
			var token = _cts.Token;

			try
			{
				var monitorService = new GameMonitorService(_config, _valueHistories);
				DebugHub.Log(_config.ProcessName, _config.Description);
				await monitorService.MonitorLoopAsync(process, token);
			}
			finally
			{
				_cts = null;
				ToggleCheckButton.Content = "开始检测";
			}
		}

		#endregion

		#region 脚本管理相关事件

		private void JsonFileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (JsonFileListBox.SelectedItem is not string fileName) return;
			try
			{
				_config = ScriptManager.LoadConfig(_moduleFolderPath, fileName);
				if (_config == null)
				{
					DebugHub.Warning("脚本加载失败", "无法对其完成解析，无效的 JSON 脚本");
					return;
				}
				ProcessComboBox.ItemsSource = ProcessManager.GetProcessList();
				ProcessComboGrid.Visibility = Visibility.Collapsed;
			}
			catch (Exception ex)
			{
				DebugHub.Error("脚本解析失败", $"{ex.Message}\n{ex.StackTrace}");
			}
		}

		public void PenaltyValue_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("惩罚输出值", "主人触发惩罚时就会根据这个值计算实际输出哦", PenaltyValueText.Text, "设定", data =>
			{
				if (!string.IsNullOrWhiteSpace(data.InputText) && int.TryParse(data.InputText, out int value))
				{
					PenaltyValueText.Text = data.InputText;
					PenaltyValue = value;
				}
				else DebugHub.Warning("设置未生效", "主人...请输入一个正常的 int 数值吧");
				data.Close();
			}, "取消", data => data.Close()).ShowDialog();
		}

		public void SleepTime_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("检测间隔", "检测内存更新的间隔时间：值越小刷新越快但消耗也越大", SleepTimeText.Text, "设定", data =>
			{
				if (!string.IsNullOrWhiteSpace(data.InputText) && int.TryParse(data.InputText, out int value))
				{
					SleepTimeText.Text = data.InputText;
					SleepTime = value;
				}
				else DebugHub.Warning("设置未生效", "喂喂喂！这根本不是有效的 int 数值哦，主人？");
				data.Close();
			}, "取消", data => data.Close()).ShowDialog();
		}

		#endregion
	}
}