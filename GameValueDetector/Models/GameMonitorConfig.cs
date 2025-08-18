namespace GameValueDetector.Models
{
	/// <summary>
	/// 游戏监控配置类
	/// </summary>
	public class GameMonitorConfig
	{
		/// <summary>脚本介绍</summary>
		public string Description { get; set; } = string.Empty;

		/// <summary>进程名称</summary>
		public string ProcessName { get; set; } = string.Empty;

		/// <summary>是否为 32 位程序</summary>
		public bool Is32Bit { get; set; } = false;

		/// <summary>监控项列表</summary>
		public List<MonitorItem> Monitors { get; set; } = [];
	}

	/// <summary>
	/// 监控单项
	/// </summary>
	public class MonitorItem
	{
		/// <summary> 监控的 DLL </summary>
		public string Module { get; set; } = "";
		/// <summary> 基地址 </summary>
		public string BaseAddress { get; set; } = "";
		/// <summary> 偏移列表 </summary>
		public List<string> Offsets { get; set; } = [];
		/// <summary> 监控项类型：如 Int32, Float, String 等 </summary>
		public string Type { get; set; } = "";
		/// <summary> 惩罚情景 </summary>
		public List<ScenarioPunishment> Scenarios { get; set; } = [];

		/// <summary> 监控项的唯一标识 </summary>
		public string UniqueKey => $"{BaseAddress}_{string.Join("_", Offsets)}";
	}

	/// <summary>
	/// 情景惩罚配置类
	/// </summary>
	public class ScenarioPunishment
	{
		/// <summary>触发条件：检测触发后是否满足此条件，若满足则开始惩罚</summary>
		public string Scenario { get; set; } = string.Empty;

		/// <summary>比较参数：用于触发检测时使用，部分检测需要传入一个检测参数(例：如果内存值大于xxx值)</summary>
		public object? CompareValue { get; set; }

		/// <summary>惩罚动作：如果满足惩罚条件，则根据此值执行对应的惩罚动作</summary>
		public string Action { get; set; } = string.Empty;

		/// <summary>惩罚动作模式</summary>
		public string ActionMode { get; set; } = "Default";

		/// <summary>惩罚动作模式的值</summary>
		public float ActionValue { get; set; } = 1f;

		/// <summary>惩罚动作的持续时间：目前仅用于一键开火，可以设置开火的时常</summary>
		public int Time { get; set; } = 3000;

		/// <summary>是否覆盖参数：用于一键开火</summary>
		public bool Overrides { get; set; } = false;
	}

	/// <summary>
	/// 历史值类
	/// </summary>
	public class ValueHistory
	{
		/// <summary> 上次值 </summary>
		public string LastValue { get; set; } = "0";
		/// <summary> 当前值 </summary>
		public string InitialValue { get; set; } = "0";
		/// <summary> 最大值 </summary>
		public double MaxValue { get; set; } = 0;
	}
}