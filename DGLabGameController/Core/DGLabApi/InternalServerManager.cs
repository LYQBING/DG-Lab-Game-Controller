using DGLabGameController.Core.Config;
using DGLabGameController.Core.Debug;
using System.Diagnostics;
using System.IO;

namespace DGLabGameController.Core.DGLabApi
{
	/// <summary>
	/// 内部服务器管理器
	/// <para>负责启动或关闭本地 DG-Lab-Coyote-Game-Hub 服务器</para>
	/// <para>除非有特殊需求，否则你不应该调用此类中的任何函数</para>
	/// </summary>
	public static class InternalServerManager
	{
		private static Process? _serverProcess; // 当前服务器进程对象
		private static readonly object Lock = new(); // 线程锁，保证多线程安全

		/// <summary>
		/// 启动服务器
		/// </summary>
		public static void StartServer()
		{
			var config = SettingsRepository.Current;
			if (!config.ServerMode) return;

			lock (Lock)
			{
				if (IsServerRunning())
				{
					DebugHub.Warning("服务器已启动", "嗯...是重复的请求呢。服务器正处于运行状态：创建请求驳回！");
					return;
				}

				try
				{
					// 构造 node.exe 路径和 server/index.js 路径
					string nodePath = Path.Combine(AppConfig.ServerPath, "bin", "node.exe");
					string serverScript = Path.Combine(AppConfig.ServerPath, "server", "index.js");

					if (!File.Exists(nodePath))
					{
						DebugHub.Error("服务器异常", $"嗯？必要的 node.exe 不存在欸：{nodePath}");
						return;
					}
					if (!File.Exists(serverScript))
					{
						DebugHub.Error("服务器异常", $"服务器启动脚本不存在欸：{serverScript}");
						return;
					}
					if(config.DisplayPowerShell)
					{
						DebugHub.Warning("PowerShell", $"警告：控制台处于开启状态：若控制台程序被关闭，则服务器将会立即停止运行！");
					}

					// 启动 node 进程
					_serverProcess = new Process
					{
						StartInfo = new ProcessStartInfo
						{
							FileName = nodePath,
							Arguments = $"\"{serverScript}\"",
							WorkingDirectory = AppConfig.ServerPath,
							UseShellExecute = config.DisplayPowerShell,
							CreateNoWindow = !config.DisplayPowerShell
						},
						EnableRaisingEvents = true
					};

					_serverProcess.Exited += OnServerProcessExited;
					_serverProcess.Start();
				}
				catch (Exception ex)
				{
					_serverProcess?.Dispose();
					_serverProcess = null;
					DebugHub.Error("服务器异常", $"对不起...主人...我好像搞砸了...异常：{ex.Message}");
					return;
				}
			}

			DebugHub.Log("服务器已启动", "主人...这就要开始了吗？");
		}

		/// <summary>
		/// 停止服务器进程
		/// </summary>
		public static void StopServer()
		{
			Process? processToStop;
			lock (Lock)
			{
				if (_serverProcess == null)
				{
					DebugHub.Success("服务器未启动", "嗯...主人是要到此为止了吗？");
					return;
				}

				processToStop = _serverProcess;
				_serverProcess = null;
			}

			try
			{
				processToStop.Exited -= OnServerProcessExited;

				if (!processToStop.HasExited)
					processToStop.Kill();
			}
			catch (Exception ex)
			{
				DebugHub.Error("服务器异常", $"对不起...主人...我好像搞砸了...异常：{ex.Message}");
				return;
			}
			finally
			{
				processToStop.Dispose();
			}

			DebugHub.Log("服务器已关闭", "嗯...主人是要到此为止了吗？");
		}

		/// <summary>
		/// 判断服务器是否正在运行
		/// </summary>
		/// <returns>是否运行中</returns>
		private static bool IsServerRunning()
		{
			if (_serverProcess == null)
				return false;

			try
			{
				return !_serverProcess.HasExited;
			}
			catch (Exception)
			{
				_serverProcess = null;
				return false;
			}
		}

		/// <summary>
		/// 服务器进程退出事件回调
		/// </summary>
		public static void OnServerProcessExited(object? sender, EventArgs e)
		{
			lock (Lock)
			{
				_serverProcess?.Dispose();
				_serverProcess = null;
			}
		}
	}
}
