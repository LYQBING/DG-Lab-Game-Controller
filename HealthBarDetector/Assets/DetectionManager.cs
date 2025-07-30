using DGLabGameController.Core.Debug;
using System.Collections.ObjectModel;

namespace HealthBarDetector.Services
{
	/// <summary>
	/// 区域检测管理器
	/// </summary>
	public class DetectionManager
	{
		private delegate void DetectionStrategy(DetectionAreaConfig area);
		private static readonly Dictionary<DetectionType, DetectionStrategy> Strategies = new()
		{
			[DetectionType.MostFrequentColorIsTarget] = area =>
			{
				var mostColor = ColorUtils.GetMostFrequentColor(area.Area);
				if (mostColor == area.Temps.color) return;
				if (ColorUtils.IsColorMatch(mostColor, area.TargetColor, area.Tolerance))
				{
					PenaltyExecutor.Execute(area.PenaltyType, area.PenaltyValueA);
				}
				area.Temps.color = mostColor;
			},
			[DetectionType.MostFrequentColorIsNotTarget] = area =>
			{
				var mostColor = ColorUtils.GetMostFrequentColor(area.Area);
				if (mostColor == area.Temps.color) return;
				if (!ColorUtils.IsColorMatch(mostColor, area.TargetColor, area.Tolerance))
				{
					PenaltyExecutor.Execute(area.PenaltyType, area.PenaltyValueA);
				}
				area.Temps.color = mostColor;
			},
			[DetectionType.TargetColorPercentage] = area =>
			{
				double percent = ColorUtils.CalculateColorPercent(area.Area, area.TargetColor, area.Tolerance);
				if (Math.Abs(percent - area.Temps.percent) > 0.01 && percent <= area.Threshold)
				{
					area.Temps.percent = percent;
					double penaltyValue = area.PenaltyValueA * percent;

					PenaltyExecutor.Execute(area.PenaltyType, (int)penaltyValue);
				}
			},
			[DetectionType.TargetColorNotPercentage] = area =>
			{
				double percent = ColorUtils.CalculateColorPercent(area.Area, area.TargetColor, area.Tolerance);
				if (Math.Abs(percent - area.Temps.percent) > 0.01 && percent <= area.Threshold)
				{
					area.Temps.percent = percent;
					double penaltyValue = area.PenaltyValueA * (1 - percent);

					PenaltyExecutor.Execute(area.PenaltyType, (int)penaltyValue);
				}
			},
			[DetectionType.TargetColorDifferentTemp] = area =>
			{
				var mostColor = ColorUtils.GetMostFrequentColor(area.Area);
				if(mostColor != area.Temps.color)
				{
					area.Temps.color = mostColor;
					PenaltyExecutor.Execute(area.PenaltyType, area.PenaltyValueA);
				}
			}
		};

		public static string GetDetectionTypeText(DetectionType type, out string Text)
		{
			switch (type)
			{
				case DetectionType.TargetColorPercentage:
					Text = "将根据此数值计算最终的惩罚值：\r若此值为 50 当前比例为 50% 则输出 25";
					return "最大惩罚值";
				case DetectionType.TargetColorNotPercentage:
					Text = "将根据此数值计算最终的惩罚值：\r若此值为 50 当前比例为 50% 则输出 25";
					return "最大惩罚值";
				default:
					Text = "为执行事件设置参数，执行惩罚时将直接使用此值";
					return "惩罚参数";
			}
		}

		#region 监测及惩罚处理

		public Func<int> GetSleepTime { get; set; } = () => 200;

		public ObservableCollection<DetectionAreaConfig> Areas { get; set; } = [];

		public async Task StartDetectionAsync(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				foreach (DetectionAreaConfig? area in Areas.Where(a => a.Enabled))
				{
					try
					{
						if (Strategies.TryGetValue(area.DetectionType, out DetectionStrategy? strategy)) strategy(area);
						else DebugHub.Warning("未知检测类型", $"区域[{area.Name}]检测类型未注册：{area.DetectionType}");
					}
					catch (Exception ex)
					{
						DebugHub.Warning("检测异常", $"区域[{area.Name}]检测时发生异常：{ex.Message}");
					}
				}
				await Task.Delay(GetSleepTime(), token);
			}
		}

		#endregion
	}
}