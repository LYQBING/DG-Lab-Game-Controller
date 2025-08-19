using GameValueDetector.Models;

namespace GameValueDetector.Services
{
	/// <summary>
	/// 惩罚情景判断器
	/// </summary>
	public static class ScenarioJudge
    {
		/// <summary>
		/// 惩罚情景匹配
		/// </summary>
		/// <param name="scenario">情景名称</param>
		/// <param name="compareValue">比较参数</param>
		/// <returns></returns>
		public static bool Match(string scenario, float compareValue, HistoryValue valueHistory)
        {
			return scenario switch
			{
				// 检测值是否变化
				"Changed" => Math.Abs(valueHistory.InitialValue - valueHistory.LastValue) > compareValue,

				// 检测值是否增加
				"Increased" => (valueHistory.InitialValue - valueHistory.LastValue) > compareValue,

				// 检测值是否减少
				"Decreased" => (valueHistory.InitialValue - valueHistory.LastValue) < compareValue,

				// 检测值是否等于某个值
				"EqualTo" => valueHistory.InitialValue == compareValue && valueHistory.LastValue != compareValue,

				// 检测值是否大于某个值
				"GreaterThan" => valueHistory.InitialValue >= compareValue,

				// 检测值是否小于某个值
				"LessThan" => valueHistory.InitialValue <= compareValue,

				// 检测值是否不等于某个值
				"NotEqualTo" => valueHistory.InitialValue != compareValue,

				// 检测值小于最大值的百分比
				"PercentLessThan" => valueHistory.InitialValue <= (valueHistory.MaxValue * compareValue),

				// 检测值大于最大值的百分比
				"PercentGreaterThan" => valueHistory.InitialValue >= (valueHistory.MaxValue * compareValue),

				// 没有匹配的情景
				_ => false,
			};
		}
    }
}