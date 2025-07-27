using DGLabGameController;
using HealthBarDetector.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace HealthBarDetector
{
	public partial class HealthBarDetectorPage : UserControl, IDisposable
	{
		private readonly DetectionManager detectionManager = new(); // 区域检测管理器实例

		private CancellationTokenSource? cts; // 用于取消检测的令牌源

		public int SleepTime { get; set; } = 200; // 检测间隔时间，单位毫秒

		public int BaseValue { get; set; } = 5; // 基础输出值，无论是否触发惩罚都会输出的基础值

		public HealthBarDetectorPage()
		{
			InitializeComponent();

			// 初始化检测区域列表
			AreaList.ItemsSource = detectionManager.Areas;
			AreaList.MouseDoubleClick += AreaList_MouseDoubleClick;

			// 绑定初始值
			SleepTimeText.Text = SleepTime.ToString();
			BaseValueText.Text = BaseValue.ToString();

			// 初始化区域检测管理器
			detectionManager.GetSleepTime = () => SleepTime;
		}

		/// <summary>
		/// 程序结束时释放资源
		/// </summary>
		public void Dispose()
		{
			cts?.Cancel();
			cts = null;
			AreaList.MouseDoubleClick -= AreaList_MouseDoubleClick;
			GC.SuppressFinalize(this);
		}

		#region 检测区域列表事件

		/// <summary>
		/// 编辑或删除区域配置的双击事件
		/// </summary>
		private void AreaList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (AreaList.SelectedItem is DetectionAreaConfig config)
			{
				var dialog = new AreaConfigDialog(config);
				if (dialog.ShowDialog() == true)
				{
					int idx = detectionManager.Areas.IndexOf(config);
					if (dialog.IsDelete && idx >= 0)
					{
						detectionManager.Areas.RemoveAt(idx);
					}
					else if (idx >= 0)
					{
						detectionManager.Areas[idx] = dialog.Config;
					}
				}
			}
		}

		#endregion

		#region 管理按钮事件

		/// <summary>
		/// 返回按钮点击事件
		/// </summary>
		public void Back_Click(object sender, RoutedEventArgs e)
		{
			if (Application.Current.MainWindow is MainWindow mw) mw.CloseActiveModule();
			else DebugHub.Warning("返回失败", "主人...我不知道该回哪里去呢？");
		}

		/// <summary>
		/// 开始/停止检测按钮事件
		/// </summary>
		public async void BtnStart_Click(object sender, RoutedEventArgs e)
		{
			// 如果当前正在检测，则取消检测
			if (cts != null)
			{
				cts.Cancel();
				cts = null;
				return;
			}

			if (detectionManager.Areas.Count == 0)
			{
				new MessageDialog("没有检测区域", "主人！你还没有添加任何检测区域哦！请先添加一个或多个区域配置。", "好的", (data) => data.Close()).ShowDialog();
				return;
			}

			// 启动检测
			cts = new CancellationTokenSource();
			BtnStart.Content = "停止检测";
			try
			{
				await detectionManager.StartDetectionAsync(cts.Token);
			}
			catch (TaskCanceledException) { }
			finally
			{
				Dispatcher.Invoke(() => BtnStart.Content = "开始检测");
				cts = null;
			}
		}

		/// <summary>
		/// 添加区域配置按钮事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnAddArea_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new AreaConfigDialog();
			if (dialog.ShowDialog() == true && dialog.IsDelete == false)
			{
				detectionManager.Areas.Add(dialog.Config);
			}
		}

		#endregion

		#region 游戏输出设置事件

		/// <summary>
		/// 检测时间间隔按钮事件
		/// </summary>
		public void SleepTime_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("检测的间隔", "检测颜色更新的间隔时间：值越小刷新越快但消耗也越大", SleepTimeText.Text, "设定", "取消", data =>
			{
				if (!string.IsNullOrWhiteSpace(data.InputText) && int.TryParse(data.InputText, out int value))
				{
					SleepTimeText.Text = data.InputText;
					SleepTime = value;
				}
				else DebugHub.Warning("设置未生效", "喂喂喂！这根本不是有效的 int 数值哦，主人？");
				data.Close();
			},data => data.Close()).ShowDialog();
		}

		/// <summary>
		/// 基础输出值按钮事件
		/// </summary>
		public void BaseValue_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("基础输出值", "无论是否触发惩罚都会输出的基础值哦", BaseValueText.Text, "设定", "取消", data =>
			{
				if (!string.IsNullOrWhiteSpace(data.InputText) && int.TryParse(data.InputText, out int value))
				{
					BaseValueText.Text = data.InputText;
					BaseValue = value;
				}
				else DebugHub.Warning("设置未生效", "杂鱼主人！就算不想被调教的喵喵叫，至少也输入些正常的参数吧？");
				data.Close();
			}, data => data.Close()).ShowDialog();
		}

		#endregion
	}
}