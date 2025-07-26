using DGLabGameController;
using HealthBarDetector.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace HealthBarDetector
{
	public partial class AreaConfigDialog : Window
	{
		public DetectionAreaConfig Config { get; private set; } // 当前区域的配置文件
		public bool IsDelete { get; private set; } = false; // 是否删除当前配置
		public bool IsLoad { get; private set; } = false; // 是否加载了已有配置

		#region 配置单初始化

		public AreaConfigDialog(DetectionAreaConfig? config = null)
		{
			InitializeComponent();
			Owner = Application.Current.MainWindow;

			if (config != null)
			{
				Config = new DetectionAreaConfig()
				{
					Name = config.Name,
					Area = config.Area,
					TargetColor = config.TargetColor,
					Tolerance = config.Tolerance,
					Threshold = config.Threshold,
					DetectionType = config.DetectionType,
					PenaltyType = config.PenaltyType,
					PenaltyValueA = config.PenaltyValueA,
					Enabled = config.Enabled
				};
				IsLoad = true;
			}
			else Config = new DetectionAreaConfig();

			UpdateUIFromConfig();
		}

		private void UpdateUIFromConfig()
		{
			TxtName.Text = Config.Name;
			TxtArea.Text = $"{Config.Area.X},{Config.Area.Y},{Config.Area.Width},{Config.Area.Height}";
			RectColor.Fill = Config.TargetBrush;
			TxtTolerance.Text = Config.Tolerance.ToString();
			TxtThreshold.Text = $"{Config.Threshold * 100:F2}%";

			foreach (ComboBoxItem item in CmbDetectionType.Items)
				if (item.Tag is DetectionType type && type == Config.DetectionType)
				{
					CmbDetectionType.SelectedItem = item;
					TxtPenaltyTitleA.Text = DetectionManager.GetDetectionTypeText(type, out penaltyTitleAText);
				}
			foreach (ComboBoxItem item in CmbPenaltyType.Items)
				if (item.Tag is PenaltyType type && type == Config.PenaltyType) CmbPenaltyType.SelectedItem = item;

			TxtPenaltyValueA.Text = Config.PenaltyValueA.ToString();
			ChkEnabled.IsChecked = Config.Enabled;
		}

		#endregion

		#region 配置设置事件

		private void Name_Click(object sender, EventArgs e)
		{
			new InputDialog("配置单名称", "主人，请为当前的配置单设置个名称吧...", TxtName.Text, "设定", "取消", data =>
			{
				if (!string.IsNullOrWhiteSpace(data.InputText))
				{
					TxtName.Text = data.InputText;
					Config.Name = data.InputText;
				}
				else DebugHub.Warning("设置未生效", "杂鱼主人！什么都没有是什么意思嘛！？");
				data.Close();
			}, data => data.Close()).ShowDialog();
		}

		private void BtnSelectArea_Click(object sender, RoutedEventArgs e)
		{
			AreaSelectorWindow selector = new();
			if (selector.ShowDialog() == true)
			{
				// 将选中的区域转换为屏幕坐标
				Rect rect = selector.SelectedRect;
				Rectangle selectedArea = new((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
				// 保存选中的区域
				Config.Area = selectedArea;
				TxtArea.Text = $"{selectedArea.X},{selectedArea.Y},{selectedArea.Width},{selectedArea.Height}";

				// 获取当前区域中存在最多的颜色 及 其占比
				Color mostColor = ColorUtils.GetMostFrequentColor(selectedArea);
				double percent = ColorUtils.CalculateColorPercent(selectedArea, mostColor, Config.Tolerance);
				new MessageDialog("推荐的设定值", $"监测颜色：R={mostColor.R}, G={mostColor.G}, B={mostColor.B}\n" + $"最佳百分比：{percent * 100:F2}%\n" +
				$"主人，需要将其设置为推荐值？", "好的", data =>
				{
					Config.Threshold = (float)percent;
					TxtThreshold.Text = $"{percent * 100:F2}%";
					Config.TargetColor = mostColor;
					RectColor.Fill = Config.TargetBrush;
					data.Close();
				}, "不要", data => data.Close()).ShowDialog();
			}
		}

		private void BtnPickColor_Click(object sender, RoutedEventArgs e)
		{
			AreaSelectorWindow selector = new();
			if (selector.ShowDialog() == true)
			{
				// 将选中的区域转换为屏幕坐标，随后获取该区域中存在最多的颜色
				Rect rect = selector.SelectedRect;
				Rectangle selectedArea = new((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
				Color mostColor = ColorUtils.GetMostFrequentColor(selectedArea);

				// 显示选中颜色
				Config.TargetColor = mostColor;
				RectColor.Fill = Config.TargetBrush;
			}
		}

		private void Tolerance_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("色彩容差值", "请输入 0-255 之间的整数，数值越大对颜色要求越宽松哦", TxtTolerance.Text, "设定", "取消", data =>
			{
				if (!string.IsNullOrWhiteSpace(data.InputText) && int.TryParse(data.InputText, out int value) && value >= 0 && value <= 255)
				{
					TxtTolerance.Text = data.InputText;
					Config.Tolerance = value;
				}
				else DebugHub.Warning("设置未生效", "杂鱼主人！必须输入一个有效的容差值哦！");
				data.Close();
			}, data => data.Close()).ShowDialog();
		}

		private void Threshold_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog("最佳百分比", "当观测的颜色占满时的最佳百分比\n例：13.14% 请输入 13.14", Config.Threshold.ToString(), "设定", "取消", data =>
			{
				if (!string.IsNullOrWhiteSpace(data.InputText) && float.TryParse(data.InputText, out float value) && value <= 100 && value >= 0)
				{
					TxtThreshold.Text = $"{value:F2}%";
					Config.Threshold = value / 100;
				}
				else DebugHub.Warning("设置未生效", "喂喂喂！请输入在 0 - 100 之间且是一个正常的 float 数值哦，主人？");
				data.Close();
			}, data => data.Close()).ShowDialog();
		}

		string penaltyTitleAText = "Null";
		private void CmbDetectionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (CmbDetectionType.SelectedItem is ComboBoxItem item && item.Tag is DetectionType type)
			{
				Config.DetectionType = type;
				TxtPenaltyTitleA.Text = DetectionManager.GetDetectionTypeText(type, out penaltyTitleAText);
			}
		}

		private void CmbPenaltyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (CmbPenaltyType.SelectedItem is ComboBoxItem item && item.Tag is PenaltyType type)
				Config.PenaltyType = type;
		}

		private void PenaltyValueA_Click(object sender, RoutedEventArgs e)
		{
			new InputDialog(TxtPenaltyTitleA.Text, penaltyTitleAText, TxtPenaltyValueA.Text, "设定", "取消", data =>
			{
				if (!string.IsNullOrWhiteSpace(data.InputText) && int.TryParse(data.InputText, out int value))
				{
					TxtPenaltyValueA.Text = data.InputText;
					Config.PenaltyValueA = value;
				}
				else DebugHub.Warning("设置未生效", "杂鱼主人！就算不想被调教的喵喵叫，至少也输入个 0 吧？");
				data.Close();
			}, data => data.Close()).ShowDialog();
		}

		private void ChkEnabled_Checked(object sender, RoutedEventArgs e) => Config.Enabled = true;
		private void ChkEnabled_Unchecked(object sender, RoutedEventArgs e) => Config.Enabled = false;

		#endregion

		#region 配置单/存档事件

		private void Back_Click(object sender, RoutedEventArgs e) => DialogResult = false;

		private void BtnOK_Click(object sender, RoutedEventArgs e) => DialogResult = true;

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			if (IsLoad)
			{
				IsDelete = true;
				DialogResult = true;
			}
			else DialogResult = false;
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left) DragMove();
		}

		#endregion
	}
}