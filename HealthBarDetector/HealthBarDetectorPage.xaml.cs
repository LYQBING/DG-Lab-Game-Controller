using DGLabGameController;
using lyqbing.DGLAB;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace HealthBarDetector
{
	public partial class HealthBarDetectorPage : UserControl, IDisposable
	{
		/// <summary>
		/// 目标颜色，默认为白色
		/// </summary>
		private Color targetColor = Color.Black;
		/// <summary>
		/// 色彩容差值，默认为20
		/// </summary>
		private int tolerance = 25;
		/// <summary>
		/// 基础输出值
		/// </summary>
		private int baseValue = 5;
		/// <summary>
		/// 惩罚输出值，当检测到血量低于阈值时输出的值
		/// </summary>
		private int penaltyValue = 20;
		/// <summary>
		/// 检测循环的睡眠时间，单位为毫秒
		/// </summary>
		private int sleepTime = 200;
		/// <summary>
		/// 最佳百分比
		/// </summary>
		private float bestPercentage = 1f;

		/// <summary>
		/// 当前选择的区域
		/// </summary>
		private Rectangle selectedArea;
		/// <summary>
		/// 是否正在检测颜色
		/// </summary>
		private bool isDetecting = false;
		/// <summary>
		/// 用于停止检测循环
		/// </summary>
		private CancellationTokenSource? cts;
		/// <summary>
		/// 上次检测到的血量百分比
		/// </summary>
		private double lastPercent = -1;
		public HealthBarDetectorPage()
		{
			InitializeComponent();
			rectColor.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(targetColor.R, targetColor.G, targetColor.B));
		}

		public void Dispose()
		{
			cts?.Cancel();
			DebugHub.Log("羽翼的色彩", "最终...也只是被黑暗笼罩");

			cts = null;
			rectColor.Fill = null;
			GC.SuppressFinalize(this);
		}

		#region 按钮相关事件

		/// <summary>
		/// 关闭按钮点击事件
		/// </summary>
		private void Back_Click(object sender, RoutedEventArgs e)
		{
			if (Application.Current.MainWindow is MainWindow mw)
			{
				mw.CloseActiveModule();
			}
			else DebugHub.Warning("返回失败", "主人...我不知道该回哪里去呢？");
		}

		/// <summary>
		///	框选区域按钮点击事件
		/// </summary>
		private void BtnSelectArea_Click(object sender, RoutedEventArgs e)
		{
			AreaSelectorWindow selector = new();
			if (selector.ShowDialog() == true)
			{
				Rect rect = selector.SelectedRect;
				selectedArea = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);

				// 获取最多的颜色
				var mostColor = GetMostFrequentColor(selectedArea);
				// 计算该颜色在区域内的占比
				var percent = CalculateHealthPercent(selectedArea, mostColor, tolerance) * 100;
				new MessageDialog(
					"推荐的设定值",
					$"观测色：R={mostColor.R}, G={mostColor.G}, B={mostColor.B}\n" +
					$"最佳百分比：{percent:F2}%\n" +
					$"主人，是否将其设定为推荐值？",
					"设定",
					(data) =>
					{
						targetColor = mostColor;
						rectColor.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(targetColor.R, targetColor.G, targetColor.B));
						bestPercentage = (float)(percent / 100.0);
						txtBestPercentage.Text = $"{percent:F2}%";
						data.Close();
					},
					"取消",
					(data) => data.Close()
				).ShowDialog();
			}
		}

		/// <summary>
		/// 颜色选择按钮点击事件
		/// </summary>
		private void BtnPickColor_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new System.Windows.Forms.ColorDialog
			{
				Color = targetColor
			};
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				targetColor = dlg.Color;
				rectColor.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(targetColor.R, targetColor.G, targetColor.B));
			}
		}

		/// <summary>
		/// 开始检测按钮点击事件
		/// </summary>
		private async void BtnStart_Click(object sender, RoutedEventArgs e)
		{
			if (!isDetecting)
			{
				if (selectedArea.Width == 0 || selectedArea.Height == 0)
				{
					new MessageDialog("观测区域不存在", $"还请主人先框选观测区域呢...", "好的", (data) => data.Close()).ShowDialog();
					return;
				}
				if (!int.TryParse(txtTolerance.Text, out tolerance) || tolerance < 0 || tolerance > 255)
				{
					new MessageDialog("无效的容差值", $"请输入0-255之间的色彩容差值哦！", "好的", (data) => data.Close()).ShowDialog();
					return;
				}
				isDetecting = true;
				btnStart.Content = "停止检测";
				cts = new CancellationTokenSource();
				await Task.Run(() => DetectLoop(cts.Token));

				isDetecting = false;
				btnStart.Content = "开始检测";
			}
			else cts?.Cancel();
		}

		/// <summary>
		/// 检测循环方法
		/// </summary>
		private void DetectLoop(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				int currentTolerance = tolerance;
				Color currentColor = targetColor;
				Dispatcher.Invoke(() =>
				{
					_ = int.TryParse(txtTolerance.Text, out currentTolerance);
					currentColor = targetColor;
				});

				double percent = CalculateHealthPercent(selectedArea, currentColor, currentTolerance);

				// 结果未变则跳过事项
				if (Math.Abs(percent - lastPercent) > 0.01)
				{
					double normalizedPercent = bestPercentage > 0 ? percent / bestPercentage : percent;
					int Value = (int)(penaltyValue * (1 - normalizedPercent)) + baseValue;
					if (Value < 0) Value = 0; if (Value > 100) Value = 100;
					_ = DGLab.SetStrength.Set(Value);

					Dispatcher.Invoke(() => txtCurrentPercent.Text = $"系统识别: {percent * 100:F2}% - 最佳化后: {normalizedPercent * 100:F2}% => {Value}");
					// if (ConfigManager.Current.VerboseLogs) DebugHub.Log("输出", $"系统识别 {percent * 100:F2}% || 最佳化后 {normalizedPercent * 100:F2}% || 输出惩罚 {Value}");
					lastPercent = percent;
				}

				Thread.Sleep(sleepTime);
			}
			DebugHub.Log("羽翼的色彩", "颜色检测服务已结束...");
		}

		/// <summary>
		/// 色彩容差值点击事件处理器
		/// </summary>
		public void ToleranceValue_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("色彩容差值", "请输入0-255之间的整数，数值越大匹配越宽松", txtTolerance.Text, "设定", "取消",
			(data) =>
			{
				if (!string.IsNullOrWhiteSpace(data.InputText))
				{
					if (int.TryParse(data.InputText, out int value) && value >= 0 && value <= 255)
					{
						txtTolerance.Text = data.InputText;
						tolerance = value;
					}
					else DebugHub.Warning("设置未生效", "请输入0-255之间的整数");
				}
				else DebugHub.Warning("设置未生效", "请输入一个有效的容差值");
				data.Close();
			},
			(data) => data.Close()).ShowDialog();
		}

		/// <summary>
		/// 最佳百分比点击事件处理器
		/// </summary>
		public void BestPercentage_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("最佳百分比", "当观测的颜色占满时的最佳百分比\n例：13.14% 请输入 13.14", $"{bestPercentage * 100}", "设定", "取消",
			(data) =>
			{
				if (!string.IsNullOrWhiteSpace(data.InputText))
				{
					if (float.TryParse(data.InputText, out float value) && value <= 100 && value >= 0)
					{
						value /= 100;

						txtBestPercentage.Text = data.InputText + "%";
						bestPercentage = value;
					}
					else DebugHub.Warning("设置未生效", "请确保数值在 0-100 之间且是一个正常 float 数值哦");
				}
				else DebugHub.Warning("设置未生效", "喂喂喂！至少输入点什么吧，主人？");
				data.Close();
			},
			(data) => data.Close()).ShowDialog();
		}

		/// <summary>
		/// 检测睡眠时间点击事件处理器
		/// </summary>
		public void SleepTime_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("检测的间隔", "检测颜色更新的间隔时间：值越小刷新越快但消耗也越大", txtSleepTime.Text, "设定", "取消",
			(data) =>
			{
				if (!string.IsNullOrWhiteSpace(data.InputText))
				{
					if (int.TryParse(data.InputText, out int value))
					{
						txtSleepTime.Text = data.InputText;
						sleepTime = value;
					}
					else DebugHub.Warning("设置未生效", "请输入一个正常的 int 数值");
				}
				else DebugHub.Warning("设置未生效", "喂喂喂！至少输入点什么吧，主人？");
				data.Close();
			},
			(data) => data.Close()).ShowDialog();
		}

		/// <summary>
		/// 基础输出值点击事件处理器
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
		/// 惩罚输出值点击事件处理器
		/// </summary>
		public void PenaltyValue_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("惩罚输出值", "当触发惩罚时输出的数值", PenaltyValue.Text, "设定", "取消",
			(data) =>
			{
				if (!string.IsNullOrWhiteSpace(data.InputText))
				{
					if (int.TryParse(data.InputText, out int value))
					{
						PenaltyValue.Text = data.InputText;
						penaltyValue = value;
					}
					else DebugHub.Warning("设置未生效", "请输入一个正常的 int 数值");
				}
				else DebugHub.Warning("设置未生效", "请输入一个有效的惩罚值");
				data.Close();
			},
			(data) => data.Close()).ShowDialog();
		}

		#endregion

		/// <summary>
		/// 获取指定区域内出现最多的颜色
		/// </summary>
		private static Color GetMostFrequentColor(Rectangle area)
		{
			using var bmp = new Bitmap(area.Width, area.Height);
			using var g = Graphics.FromImage(bmp);
			g.CopyFromScreen(area.X, area.Y, 0, 0, area.Size);

			var colorCount = new System.Collections.Generic.Dictionary<int, int>();
			for (int x = 0; x < bmp.Width; x++)
			{
				for (int y = 0; y < bmp.Height; y++)
				{
					var color = bmp.GetPixel(x, y).ToArgb();
					if (colorCount.TryGetValue(color, out int value))
						colorCount[color] = ++value;
					else
						colorCount[color] = 1;
				}
			}
			if (colorCount.Count == 0) return Color.Red;

			int maxColor = 0, maxCount = 0;
			foreach (var kv in colorCount)
			{
				if (kv.Value > maxCount)
				{
					maxColor = kv.Key;
					maxCount = kv.Value;
				}
			}
			return Color.FromArgb(maxColor);
		}

		/// <summary>
		/// 计算指定区域内目标颜色的百分比
		/// </summary>
		private static double CalculateHealthPercent(Rectangle area, Color targetColor, int tolerance)
		{
			using var bmp = new Bitmap(area.Width, area.Height);
			using var g = Graphics.FromImage(bmp);
			g.CopyFromScreen(area.X, area.Y, 0, 0, area.Size);

			int matchCount = 0, total = bmp.Width * bmp.Height;
			for (int x = 0; x < bmp.Width; x++)
			{
				for (int y = 0; y < bmp.Height; y++)
				{
					var pixel = bmp.GetPixel(x, y);
					if (IsColorMatch(pixel, targetColor, tolerance)) matchCount++;
				}
			}
			return total == 0 ? 0 : (double) matchCount / total;
		}

		/// <summary>
		/// 判断两个颜色是否匹配
		/// </summary>
		private static bool IsColorMatch(Color a, Color b, int tolerance)
		{
			return Math.Abs(a.R - b.R) <= tolerance &&
				   Math.Abs(a.G - b.G) <= tolerance &&
				   Math.Abs(a.B - b.B) <= tolerance;
		}
	}
}
