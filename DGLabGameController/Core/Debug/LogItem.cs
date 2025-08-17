using System.Windows.Media;

namespace DGLabGameController.Core.Debug
{
	/// <summary>
	/// 日志单项样式
	/// </summary>
	public class LogItem
	{
		public DateTime Time { get; set; } // 日志时间
		public string? EventName { get; set; } // 事件名称
		public string? Content { get; set; } // 日志内容
		public LogType Type { get; set; } // 日志类型
		public Brush ContentBrush => EventBrush; // 内容的颜色与事件类型一致
		public static Brush TimeBrush => Brushes.White; // 时间的颜色
		public string TimeString => $"[{Time:HH:mm:ss}]"; // 格式化时间字符串

		public override string ToString() => $"{TimeString} [{EventName}]：{Content}";
		public Brush EventBrush => Type switch
		{
			LogType.Success => Brushes.SpringGreen,
			LogType.Warning => Brushes.Gold,
			LogType.Error => Brushes.Red,
			_ => Brushes.White
		};
	}
}
