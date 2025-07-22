using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace DGLabGameController
{
	public enum LogType
	{
		Info,
		Success,
		Warning,
		Error
	}

	public class LogItem
	{
		public DateTime Time { get; set; }
		public string? EventName { get; set; }
		public string? Content { get; set; }
		public LogType Type { get; set; }

		public Brush EventBrush => Type switch
		{
			LogType.Success => Brushes.SpringGreen,
			LogType.Warning => Brushes.Gold,
			LogType.Error => Brushes.Red,
			_ => Brushes.White
		};
		public Brush ContentBrush => EventBrush;

		public static Brush TimeBrush => Brushes.White;

		public string TimeString => $"[{Time:HH:mm:ss}]";

		public override string ToString()
		{
			return $"{TimeString} [{EventName}]：{Content}";
		}
	}

	public static class DebugHub
	{
		/// <summary>
		/// 日志集合：当前的所有日志项
		/// </summary>
		public static ObservableCollection<LogItem> Logs { get; } = new();

		public static void ClearLogs()
		{
			Logs.Clear();
			Log("日志已清空", "我们将一直保持免费且开源：关注开发者项目以表支持\r欢迎加入官方项目群聊：928175340");
		}

		public static void Log(string eventName, string content, LogType type = LogType.Info)
		{
			Application.Current.Dispatcher.InvokeAsync(() =>
			{
				Logs.Add(new LogItem
				{
					Time = DateTime.Now,
					EventName = eventName,
					Content = content,
					Type = type
				});
			});
		}

		public static void Success(string eventName, string content) => Log(eventName, content, LogType.Success);
		public static void Warning(string eventName, string content) => Log(eventName, content, LogType.Warning);
		public static void Error(string eventName, string content) => Log(eventName, content, LogType.Error);
	}
}
