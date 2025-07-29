using DGLabGameController;
using DGLabGameController.Core.Config;
using DGLabGameController.Core.Debug;
using GamepadVibrationProcessor.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace GamepadVibrationProcessor
{
	public class ProcessInfo
	{
		public string? Name { get; set; }
		public int Id { get; set; }
		public string? FilePath { get; set; }
		public System.Windows.Media.ImageSource? Icon { get; set; }
	}

	public partial class HandleInjection : UserControl
	{
		public ObservableCollection<ProcessInfo> ProcessList { get; set; } = [];
		public static int BaseValue { get; set; } = 5;
		public static int PenaltyValue { get; set; } = 20;
		public string ModuleFolderPath { get; set; } = "";

		public HandleInjection(string moduleId)
		{
			InitializeComponent();
			ProcessListView.ItemsSource = ProcessList;
			ModuleFolderPath = Path.Combine(AppConfig.ModulesPath, moduleId);

			Refresh_Click();
			BaseValueText.Text = BaseValue.ToString();
			PenaltyValueText.Text = PenaltyValue.ToString();
		}

		/// <summary>
		/// 返回按钮事件
		/// </summary>
		public void Back_Click(object sender, RoutedEventArgs e)
		{
			if (Application.Current.MainWindow is MainWindow mw) mw.CloseActiveModule();
			else DebugHub.Warning("返回失败", "主人...我不知道该回哪里去呢？");
		}

		/// <summary>
		/// 基础输出值按钮事件
		/// </summary>
		public void BaseValue_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("基础输出值", "无论是否触发惩罚都会输出的基础值哦", BaseValueText.Text, "设定",
			(data) =>
			{
				if (!string.IsNullOrWhiteSpace(data.InputText) && int.TryParse(data.InputText, out int value))
				{
					BaseValueText.Text = data.InputText;
					BaseValue = value;
				}
				else
				{
					DebugHub.Warning("设置未生效", "主人...请输入一个正常的 int 数值吧");
				}
				data.Close();
			}, "取消",
			(data) => data.Close()).ShowDialog();
		}

		/// <summary>
		/// 惩罚输出值按钮事件
		/// </summary>
		public void PenaltyValue_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("惩罚输出值", "主人触发惩罚时就会根据这个值输出哦", PenaltyValueText.Text, "设定",
			(data) =>
			{
				if (!string.IsNullOrWhiteSpace(data.InputText) && int.TryParse(data.InputText, out int value))
				{
					PenaltyValueText.Text = data.InputText;
					PenaltyValue = value;
				}
				else
				{
					DebugHub.Warning("设置未生效", "主人...请输入一个正常的 int 数值吧");
				}
				data.Close();
			}, "取消",
			(data) => data.Close()).ShowDialog();
		}

		/// <summary>
		/// 注入按钮事件
		/// </summary>
		private void Inject_Click(object sender, RoutedEventArgs e)
		{
			if (ProcessListView.SelectedItem is not ProcessInfo selected)
			{
				new MessageDialog("未选择进程", "主人，您还没有选择想要注入的进程哦！", "知道了", (data) => data.Close()).ShowDialog();
				return;
			}
			if (selected.Id == Environment.ProcessId)
			{
				new MessageDialog("禁止自注入", "主人...无论如何也不能对自己下手哦！", "知道了", (data) => data.Close()).ShowDialog();
				return;
			}
			// 注入 DLL
			string dllPath = Path.Combine(ModuleFolderPath, "GamepadVibrationHook.dll");
			if (InjectionManager.Inject(selected.Id, dllPath, out var error))
			{
				DebugHub.Success("等待客户端", "等待客户端回执中...");
				DebugHub.Log("特别注意", "无法向已注入模块的客户端再次注入！这将不会返回任何回执数据。若需重新注入，请重启对应客户端。");
			}
			else DebugHub.Error("注入失败", "尝试对客户端释放 DLL 时发生了意料之外的错误！");
			if (Application.Current.MainWindow is MainWindow mw) mw.NavLog_Click();
		}

		/// <summary>
		/// 刷新进程列表按钮事件
		/// </summary>
		private async void Refresh_Click(object? sender = null, RoutedEventArgs? e = null)
		{
			ProcessList.Clear();
			await Task.Delay(1);
			foreach (var proc in ProcessService.GetProcessList()) ProcessList.Add(proc);
		}
	}
}