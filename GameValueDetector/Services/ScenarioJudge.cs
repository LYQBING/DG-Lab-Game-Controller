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
		/// <param name="lastValue">上次的值</param>
		/// <param name="currentValue">当前值</param>
		/// <param name="compareValue">比较参数</param>
		/// <returns></returns>
		public static bool Match(string scenario, object? lastValue, object? currentValue, object? compareValue)
        {
			string? lastStr = lastValue?.ToString(); // 上次值转换为字符串
			string? currStr = currentValue?.ToString(); // 当前值转换为字符串
			string? cmpStr = compareValue?.ToString(); // 比较值转换为字符串

			static bool TryGetDouble(string? str, out double result) => double.TryParse(str, out result);

			return scenario switch
			{
				// 检测值是否变化 : 通用
				"Changed" => !Equals(lastValue, currentValue),

				// 检测值是否增加 : 数字类型
				"Increased" when TryGetDouble(lastStr, out double oldNum) && TryGetDouble(currStr, out double newNum) => newNum > oldNum,

				// 检测值是否减少 : 数字类型
				"Decreased" when TryGetDouble(lastStr, out double oldNum) && TryGetDouble(currStr, out double newNum) => newNum < oldNum,

				// 检测值是否等于某个值 : 数字类型
				"EqualTo" when TryGetDouble(currStr, out double newNum) && TryGetDouble(cmpStr, out double cmp) => newNum == cmp,

				// 检测值是否大于某个值 : 数字类型
				"GreaterThan" when TryGetDouble(currStr, out double newNum) && TryGetDouble(cmpStr, out double cmp) => newNum > cmp,

				// 检测值是否小于某个值 : 数字类型
				"LessThan" when TryGetDouble(currStr, out double newNum) && TryGetDouble(cmpStr, out double cmp) => newNum < cmp,

				// 检测值是否不等于某个值 : 数字类型
				"NotEqualTo" when TryGetDouble(currStr, out double newNum) && TryGetDouble(cmpStr, out double cmp) => newNum != cmp,

				// 检测字符串是否相等 : 字符串类型
				"StringEquals" => currStr == cmpStr,

				// 检测字符串是否包含某个子串 : 字符串类型
				"StringContains" => currStr?.Contains(cmpStr ?? "") == true,

				// 正则匹配 : 字符串类型
				"RegexMatch" => cmpStr is not null && currStr is not null && System.Text.RegularExpressions.Regex.IsMatch(currStr, cmpStr),

				// 没有匹配的情景
				_ => false,
			};
		}
    }
}