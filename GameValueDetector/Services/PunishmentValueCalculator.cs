using GameValueDetector.Models;

namespace GameValueDetector.Services
{
	public static class PunishmentValueCalculator
	{
		/// <summary>
		/// 计算惩罚值
		/// </summary>
		public static float Calculate(ScenarioPunishment config, DataValue Historyvalue)
		{
			float targetValue = config.ActionValue; // 目标值
			float baseValue = GameValueDetectorPage.PenaltyValue; // 基值

			return config.ActionMode switch
			{
				// 默认模式 : 返回基值乘以目标值
				"Default" => baseValue * targetValue,

				// 固定模式 : 返回 目标值
				"Fixed" => targetValue,

				// 差值模式 : 返回 内存值 与 上次值 的差值 乘 目标值
				"Diff" => baseValue * MathF.Abs(Historyvalue.InitialValue - Historyvalue.LastValue) * targetValue,

				// 内存值模式 : 返回 内存值 乘 目标值
				"MemoryValue" => baseValue * Historyvalue.InitialValue * targetValue,

				// 正百分比模式 : 返回 当前值 除 最大值 乘 目标值
				"Percent" => baseValue * (Historyvalue.InitialValue / Historyvalue.MaxValue) * targetValue,

				// 反百分比模式 : 返回 1 - 当前值 除 最大值 乘 目标值
				"Reverse_Percent" => baseValue *(1f - (Historyvalue.InitialValue / Historyvalue.MaxValue)) * targetValue,

				// 变化正百分比模式 : 返回 内存值 与 上次值 的 变化比率
				"ChangePercent" => baseValue * (MathF.Abs(Historyvalue.LastValue - Historyvalue.InitialValue) / Historyvalue.MaxValue) * targetValue,

				// 变化反百分比模式 : 返回 1 - 内存值 与 上次值 的 变化比率
				"Reverse_ChangePercent" => baseValue * (1f - (MathF.Abs(Historyvalue.LastValue - Historyvalue.InitialValue) / Historyvalue.MaxValue)) * targetValue,

				// 未知模式 : 返回0
				_ => 0
			};
		}
	}
}