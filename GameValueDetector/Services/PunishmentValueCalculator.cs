using GameValueDetector.Models;

namespace GameValueDetector.Services
{
	public static class PunishmentValueCalculator
	{
		/// <summary>
		/// 计算惩罚值
		/// </summary>
		public static float Calculate(ScenarioPunishment config, HistoryValue Historyvalue)
		{
			if (!float.TryParse(Historyvalue.LastValue, out float lastValue)) lastValue = 0; // 上次值
			if (!float.TryParse(Historyvalue.InitialValue, out float initialValue)) initialValue = 0; // 内存值

			float maxValue = (float)Historyvalue.MaxValue; // 最大值
			float targetValue = config.ActionValue; // 目标值
			float baseValue = GameValueDetectorPage.PenaltyValue; // 基值

			return config.ActionMode switch
			{
				// 默认模式 : 返回基值乘以目标值
				"Default" => baseValue * targetValue,

				// 差值模式 : 返回 内存值减去上次值乘以目标值 (内存为 string 时无效)
				"Diff" => float.Min(baseValue, MathF.Abs(initialValue - lastValue) * targetValue),

				// 正百分比模式 : 返回 此时内存与最大值时的比率 (内存为 string 时无效)
				"Percent" => baseValue * (initialValue / maxValue) * targetValue,

				// 反百分比模式 : 返回 1 - 此时内存与最大值时的比率 (内存为 string 时无效)
				"Reverse_Percent" => baseValue *(1f - (initialValue / maxValue)) * targetValue,

				// 变化正百分比模式 : 返回 内存值与上次值的变化比率 (内存为 string 时无效)
				"ChangePercent" => baseValue * (MathF.Abs(lastValue - initialValue) / maxValue) * targetValue,

				// 变化反百分比模式 : 返回 1 - 内存值与上次值的变化比率 (内存为 string 时无效)
				"Reverse_ChangePercent" => baseValue * (1f - (MathF.Abs(lastValue - initialValue) / maxValue)) * targetValue,

				// 未知模式 : 返回0
				_ => 0
			};
		}
	}
}