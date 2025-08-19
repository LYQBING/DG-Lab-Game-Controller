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
		public static ObservableCollection<LogItem> Logs { get; } = []; // 日志集合：UI 绑定使用
		private static readonly List<LogItem> _logBuffer = []; // 日志缓冲区：用于存储日志以便批量写入文件
		private static readonly object _bufferLock = new(); // 互斥锁对象：用于保证多线程环境下对 _logBuffer 的安全访问
		private static bool _flushScheduled = false; // 当前是否有刷新任务
		private const int MaxLogCount = 50; // 最大日志数量：超过此数量将自动删除最旧的日志

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
			// 确保日志有效，并创建日志项
			if (verboseLog && !SettingsRepository.Current.VerboseLogs) return;
			LogItem logItem = new()
			{
				Time = DateTime.Now,
				EventName = eventName,
				Content = content,
				Type = type
			};

			// 添加到日志缓存并等待刷新
			lock (_bufferLock)
			{
				_logBuffer.Add(logItem);
				if (!_flushScheduled)
				{
					_flushScheduled = true;
					Application.Current.Dispatcher.InvokeAsync(FlushBuffer);
				}
			}
		}

		/// <summary>
		/// 刷新缓冲区到 UI 日志集合
		/// </summary>
		private static void FlushBuffer()
		{
			lock (_bufferLock)
			{
				// 将缓冲区的日志项批量写入到集合
				foreach (var item in _logBuffer)
				{
					if (Logs.Count >= MaxLogCount) Logs.RemoveAt(0);
					Logs.Add(item);
				}

				// 清空缓冲区
				_logBuffer.Clear();
				_flushScheduled = false;
			}
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
