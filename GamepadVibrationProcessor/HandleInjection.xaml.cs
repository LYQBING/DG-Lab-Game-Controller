using DGLabGameController;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GamepadVibrationProcessor
{
	public class ProcessInfo
	{
		public string? Name { get; set; }
		public int Id { get; set; }
		public string? FilePath { get; set; }
		public ImageSource? Icon { get; set; }
	}

	public partial class HandleInjection : UserControl
	{
		/// <summary>
		/// 存储进程信息的集合
		/// </summary>
		public ObservableCollection<ProcessInfo> ProcessList { get; set; } = new();

		public static int baseValue { get; set; } = 5;
		public static int penaltyValue { get; set; } = 20;

		public HandleInjection()
		{
			// 初始化组件
			InitializeComponent();
			ProcessListView.ItemsSource = ProcessList;
			Refresh_Click();

			// 绑定初始值
			BaseValue.Text = baseValue.ToString();
			PenaltyValue.Text = penaltyValue.ToString();
		}

		~HandleInjection() => DebugHub.Log("手柄的振动天罚", "主人是要抛弃我了吗...");

		#region 按钮事件

		/// <summary>
		/// 返回按钮点击事件
		/// </summary>
		public void Back_Click(object sender, RoutedEventArgs e)
		{
			if (Application.Current.MainWindow is MainWindow mw)
			{
				mw.CloseActiveModule();
			}
			else DebugHub.Warning("返回失败", "主人...我不知道该回哪里去呢？");
		}

		/// <summary>
		/// 设置基础输出值按钮点击事件
		/// </summary>
		public void BaseValue_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("基础输出值", "无论是否触发惩罚都会输出的基础值哦", BaseValue.Text, "设定", "取消",
			(data) =>
			{
				if (!string.IsNullOrWhiteSpace(data.InputText))
				{
					if (int.TryParse(data.InputText, out int value))
					{
						BaseValue.Text = data.InputText;
						baseValue = value;
					}
					else DebugHub.Warning("设置未生效", "主人...请输入一个正常的 int 数值吧");
				}
				else DebugHub.Warning("设置未生效", "杂鱼主人！就算不想随时随地被调教，至少也输入个 0 吧？");
				data.Close();
			},
			(data) => data.Close()).ShowDialog();
		}

		/// <summary>
		/// 设置惩罚输出值按钮点击事件
		/// </summary>
		public void PenaltyValue_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("惩罚输出值", "主人触发惩罚时就会根据这个值输出哦", PenaltyValue.Text, "设定", "取消",
			(data) =>
			{
				if (!string.IsNullOrWhiteSpace(data.InputText))
				{
					if (int.TryParse(data.InputText, out int value))
					{
						PenaltyValue.Text = data.InputText;
						penaltyValue = value;
					}
					else DebugHub.Warning("设置未生效", "主人...请输入一个正常的 int 数值吧");
				}
				else DebugHub.Warning("设置未生效", "杂鱼主人！这么害怕被惩罚嘛？那就赶紧逃掉吧！");
				data.Close();
			},
			(data) => data.Close()).ShowDialog();
		}

		/// <summary>
		/// 注入按钮点击事件
		/// </summary>
		private void Inject_Click(object sender, RoutedEventArgs e)
		{
			if (ProcessListView.SelectedItem is not ProcessInfo selected)
			{
				new MessageDialog("未选择进程", "主人，您还没有选择想要注入的进程哦！", "知道了", (data) =>data.Close()).ShowDialog();
				return;
			}

			if (selected.Id == Environment.ProcessId)
			{
				new MessageDialog("禁止自注入", "主人...无论如何也不能对自己下手哦！", "知道了", (data) => data.Close()).ShowDialog();
				return;
			}

			try
			{
				string dllPathX64 = Path.Combine(ConfigManager.DataPath, "GamepadVibrationHook", "GamepadVibrationHook_X64.dll");
				string dllPathX86 = Path.Combine(ConfigManager.DataPath, "GamepadVibrationHook", "GamepadVibrationHook_X86.dll");

				if (InjectionManager.Inject(selected.Id, dllPathX86, dllPathX64, out string? error))
				{
					DebugHub.Success("等待客户端", error);
					DebugHub.Log("特别注意", "已注入模块的客户端再次注入时将不会回执任何数据：若需要重新注入，请重启对应客户端");
				}
				else
				{
					DebugHub.Error("注入失败", error);
				}
			}
			catch (Exception ex)
			{
				DebugHub.Error("注入异常", ex.Message);
			}

			if (Application.Current.MainWindow is MainWindow mw)
			{
				mw.NavLog_Click();
			}
		}

		/// <summary>
		/// 刷新进程列表按钮点击事件
		/// </summary>
		private async void Refresh_Click(object? sender = null, RoutedEventArgs? e = null)
		{
			ProcessList.Clear();
			await Task.Delay(1);

			foreach (var proc in Process.GetProcesses())
			{
				try
				{
					// 仅显示有效的进程：排除系统/服务/无界面
					if (string.IsNullOrWhiteSpace(proc.MainWindowTitle)) continue;

					// 获取应用的文件路径和图标
					string filePath = proc.MainModule?.FileName ?? "";
					var icon = GetIcon(filePath);

					// 优先调用真实的程序名称
					string appName = string.IsNullOrWhiteSpace(proc.MainWindowTitle) ? proc.ProcessName : proc.MainWindowTitle;

					// 将其添加至列表
					ProcessList.Add(new ProcessInfo
					{
						Name = appName,
						Id = proc.Id,
						FilePath = filePath,
						Icon = icon
					});
				}
				catch (Exception ex)
				{
					if (ConfigManager.Current.VerboseLogs)
						DebugHub.Log("进程访问异常", $"无法访问程序: {proc.ProcessName}:{ex.Message}");
				}
			}
		}

		#endregion 按钮事件

		#region 其他事件

		/// <summary>
		/// 获取进程图标
		/// </summary>
		private static BitmapImage? GetIcon(string filePath)
		{
			if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return null;
			try
			{
				using (var icon = Icon.ExtractAssociatedIcon(filePath))
				{
					if (icon != null)
					{
						using (var bmp = icon.ToBitmap())
						using (var stream = new MemoryStream())
						{
							bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
							stream.Seek(0, SeekOrigin.Begin);
							var bitmap = new BitmapImage();
							bitmap.BeginInit();
							bitmap.StreamSource = stream;
							bitmap.CacheOption = BitmapCacheOption.OnLoad;
							bitmap.EndInit();
							bitmap.Freeze();
							return bitmap;
						}
					}
				}
			}
			catch { }
			return null;
		}

		#endregion 其他事件
	}
}