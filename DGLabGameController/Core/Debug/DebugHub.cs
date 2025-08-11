using DGLabGameController.Core.Config;
using System.Collections.ObjectModel;
using System.Windows;

namespace DGLabGameController.Core.Debug
{
	/// <summary>
	/// 日志中心：所有日志都将通过此类记录
	/// </summary>
	public static class DebugHub
	{
		public static ObservableCollection<LogItem> Logs { get; } = [];

		public static void Log(string eventName, string content, bool verboseLog = false) => Log(eventName, content, verboseLog, LogType.Info);
		public static void Success(string eventName, string content, bool verboseLog = false) => Log(eventName, content, verboseLog, LogType.Success);
		public static void Warning(string eventName, string content, bool verboseLog = false) => Log(eventName, content, verboseLog, LogType.Warning);
		public static void Error(string eventName, string content, bool verboseLog = false) => Log(eventName, content, verboseLog, LogType.Error);

		/// <summary>
		/// 日志记录方法
		/// </summary>
		/// <param name="eventName">日志名称</param>
		/// <param name="content">日志内容</param>
		/// <param name="verboseLog">是否为详细日志</param>
		/// <param name="type">日志类型</param>
		private static void Log(string eventName, string content, bool verboseLog, LogType type)
		{
			if (verboseLog && !SettingsRepository.Current.VerboseLogs) return;
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

		/// <summary>
		/// 清空日志
		/// </summary>
		public static void Clear()
		{
			Logs.Clear();
			Log("欢迎回来", "我们将一直保持免费且开源：关注开发者项目以表支持");
			Success("欢迎加入官方群聊", "更多教程、模块、脚本等资源尽在 QQ 群聊：928175340");
		}
	}

	public enum LogType
	{
		Info,
		Success,
		Warning,
		Error
	}
}
