using GameValueDetector.Models;

namespace GameValueDetector.Services
{
    public static class PunishmentValueCalculator
    {
        /// <summary>
        /// 计算惩罚值
        /// </summary>
        public static float Calculate(ScenarioPunishment config, object? lastValue, object? memoryValueTemp)
        {
			float oldNum = ToFloat(lastValue);
			float memoryValue = ToFloat(memoryValueTemp);
			float targetValue = config.ActionValue;
			float baseValue = GameValueDetectorPage.PenaltyValue;

			return config.ActionMode switch
            {
				// 默认模式 : 返回基值乘以目标值
				"Default" => baseValue * targetValue,

				// 差值模式 : 返回内存值减去上次值 (内存为 string 时无效)
				"Diff" => memoryValue - oldNum,

				// 反向差值模式 : 返回上次值减去内存值  (内存为 string 时无效)
				"Reverse_Diff" => oldNum - memoryValue,

				// 百分比模式 : 返回基值乘以内存值与目标值的比率  (内存为 string 时无效)
				"Percent" when targetValue != 0 => baseValue * (memoryValue / targetValue),

				// 反向百分比模式 : 返回基值乘以1减去内存值与目标值的比率  (内存为 string 时无效)
				"Reverse_Percent" when targetValue != 0 => baseValue * (1f - (memoryValue / targetValue)),

				// 未知模式 : 返回0
				_ => 0f,
            };

			static float ToFloat(object? val)
			{
				return val switch
				{
					float f => f,
					double d => (float)d,
					int i => i,
					string s when float.TryParse(s, out var v) => v,
					_ => 0f
				};
			}
		}
    }
}