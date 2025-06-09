using System;
using System.Diagnostics;
using System.IO;

namespace DGLabGameController
{
	public static class InternalServerManager
	{
		static Process? _serverProcess;
		static readonly object _lock = new();

		public static void StartServer()
		{
			if (!ConfigManager.Current.ServerMode) return;
			lock (_lock)
			{
				try
				{
					
					if (_serverProcess != null)
					{
						try
						{
							if (!_serverProcess.HasExited)
							{
								DebugHub.Warning("服务器存在", "嗯...是重复的请求呢。服务器正处于运行状态：创建请求驳回！");
								return;
							}
						}
						catch
						{
							_serverProcess = null;
						}
					}

					if (Process.GetProcessesByName("coyote-game-hub-server").Length != 0)
					{
						DebugHub.Warning("服务器异常", "欸？服务器已在运行了哦！非主线程服务器可能存在兼容性问题，若需手动创建或链接外部服务器：请前往 “设置” 关闭 “内部服务器”");
						return;
					}

					string exePath = Path.Combine(ConfigManager.ServerPath, "coyote-game-hub-server.exe");
					string? workingDir = Path.GetDirectoryName(exePath);

					_serverProcess = new Process
					{
						StartInfo = new ProcessStartInfo
						{
							FileName = exePath, 										// 进程名称
							WorkingDirectory = workingDir ?? string.Empty,              // 进程的工作目录
							UseShellExecute = false,                                    // 是否使用操作系统的外壳启动
							CreateNoWindow = !ConfigManager.Current.DisplayPowerShell	// 是否隐藏界面
						},
						EnableRaisingEvents = true
					};
					_serverProcess.Exited += (s, e) =>
					{
						lock (_lock)
						{
							_serverProcess?.Dispose();
							_serverProcess = null;
						}
					};

					_serverProcess.Start();
					DebugHub.Success("服务器启动成功", "主人...要开始了吗？");
				}
				catch (Exception ex)
				{
					DebugHub.Error("服务器启动失败", $"对不起...主人...我好像搞砸了...异常：{ex.Message}");
				}
			}
		}

		public static void StopServer()
		{
			lock (_lock)
			{
				try
				{
					if (_serverProcess != null)
					{
						try
						{
							if (!_serverProcess.HasExited)
							{
								_serverProcess.Kill();
							}
						}
						finally
						{
							_serverProcess.Dispose();
							_serverProcess = null;
						}
					}
					DebugHub.Success("服务器关闭成功", "嗯...主人是要到此为止了吗？");
				}
				catch (Exception ex)
				{
					DebugHub.Error("关闭服务器失败", $"对不起...主人...我好像搞砸了...异常：{ex.Message}");
				}
			}
		}
	}
}
