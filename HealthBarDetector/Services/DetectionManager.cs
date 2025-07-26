using DGLabGameController;
using System.Collections.ObjectModel;

namespace HealthBarDetector.Services
{
	/// <summary>
	/// 区域检测管理器
	/// </summary>
	public class DetectionManager
	{
		public ObservableCollection<DetectionAreaConfig> Areas { get; set; } = [];
		public Func<int> GetSleepTime { get; set; } = () => 200;

		// 规则委托
		private delegate (bool trigger, int penaltyValue) DetectionStrategy(DetectionAreaConfig area);
		private static readonly Dictionary<DetectionType, DetectionStrategy> Strategies = new()
		{
			[DetectionType.MostFrequentColorIsTarget] = area =>
			{
				var mostColor = ColorUtils.GetMostFrequentColor(area.Area);
				bool trigger = ColorUtils.IsColorMatch(mostColor, area.TargetColor, area.Tolerance);
				return (trigger, area.PenaltyValueA);
			},
			[DetectionType.MostFrequentColorIsNotTarget] = area =>
			{
				var mostColor = ColorUtils.GetMostFrequentColor(area.Area);
				bool trigger = !ColorUtils.IsColorMatch(mostColor, area.TargetColor, area.Tolerance);
				return (trigger, area.PenaltyValueA);
			},
			[DetectionType.TargetColorPercentage] = area =>
			{
				double percent = ColorUtils.CalculateColorPercent(area.Area, area.TargetColor, area.Tolerance);
				bool trigger = Math.Abs(percent - area.Temps.percent) > 0.01 && percent <= area.Threshold;
				area.Temps.percent = percent;
				double penaltyValue = area.PenaltyValueA * percent;
				return (trigger, (int)penaltyValue);
			},
			[DetectionType.TargetColorNotPercentage] = area =>
			{
				double percent = ColorUtils.CalculateColorPercent(area.Area, area.TargetColor, area.Tolerance);
				bool trigger = Math.Abs(percent - area.Temps.percent) > 0.01 && percent <= area.Threshold;
				area.Temps.percent = percent;
				double penaltyValue = area.PenaltyValueA * (1 - percent);
				return (trigger, (int)penaltyValue);
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

		public async Task StartDetectionAsync(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				foreach (var area in Areas.Where(a => a.Enabled))
				{
					try
					{
						if (Strategies.TryGetValue(area.DetectionType, out var strategy))
						{
							var (trigger, penaltyValue) = strategy(area);
							if (trigger) Executor.Execute(area.PenaltyType, penaltyValue);
						}
						else
						{
							DebugHub.Warning("未知检测类型", $"区域[{area.Name}]检测类型未注册：{area.DetectionType}");
						}
					}
					catch (Exception ex)
					{
						DebugHub.Warning("检测异常", $"区域[{area.Name}]检测时发生异常：{ex.Message}");
					}
				}
				await Task.Delay(GetSleepTime(), token);
			}
		}
	}
}