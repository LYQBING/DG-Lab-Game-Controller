using System.Diagnostics;

namespace GameValueDetector.Services
{
	/// <summary>
	/// 进程管理器
	/// </summary>
	public static class ProcessManager
    {
		/// <summary>
		/// 获取当前所有进程
		/// </summary>
		/// <returns>当前进程列表</returns>
		public static List<Process> GetProcessList()
        {
            return [.. Process.GetProcesses().OrderBy(p => p.ProcessName)];
        }

		/// <summary>
		/// 查找指定名称的进程
		/// </summary>
		/// <param name="processes">进程列表</param>
		/// <param name="processName">进程名称</param>
		/// <returns></returns>
		public static Process? FindProcessByName(IEnumerable<Process> processes, string? processName)
		{
			if (string.IsNullOrWhiteSpace(processName)) return null;
			return processes.FirstOrDefault(p => string.Equals(p.ProcessName, processName, StringComparison.OrdinalIgnoreCase));
		}
    }
}