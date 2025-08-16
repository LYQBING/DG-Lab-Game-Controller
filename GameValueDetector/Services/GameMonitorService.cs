using DGLabGameController.Core.Debug;
using GameValueDetector.Models;
using GameValueDetector.Interop;
using System.Diagnostics;

namespace GameValueDetector.Services
{
	/// <summary>
	/// 游戏监控服务
	/// </summary>
	/// <param name="config">配置文件</param>
	/// <param name="valueHistories">全部的历史值表</param>
	public class GameMonitorService(GameMonitorConfig config, Dictionary<string, ValueHistory> valueHistories)
	{
		/// <summary>
		/// 异步监控循环
		/// </summary>
		/// <param name="process">目标进程</param>
		/// <param name="token">取消令牌</param>
		/// <returns>异步任务</returns>
		public async Task MonitorLoopAsync(Process process, CancellationToken token)
		{
			IntPtr hProcess = IntPtr.Zero; // 缓存进程句柄
			bool is64Bit = false;          // 缓存进程位数

			try
			{
				hProcess = Win32Helper.OpenProcess(ProcessAccessFlags.VirtualMemoryRead, false, process.Id);
				if (hProcess == IntPtr.Zero)
				{
					DebugHub.Warning("无法打开进程", "主人...似乎是没有权限呢", true);
					return;
				}
				is64Bit = MemoryReader.IsProcess64Bit(process);

				while (!token.IsCancellationRequested)
				{
					foreach (MonitorItem monitor in config.Monitors)
					{
						// 获取唯一标识符
						string key = monitor.BaseAddress;

						// 尝试从历史值表中获取对应的值历史记录
						valueHistories.TryGetValue(key, out ValueHistory? valueHistory);
						valueHistory ??= new();

						// 读取当前内存的值，传入已打开的句柄和进程位数，避免重复打开和判断
						string? currentValue = MemoryReader.ReadValue(process, monitor, hProcess, is64Bit)?.ToString();
						if (currentValue == null) continue;

						// 更新当前值
						valueHistory.InitialValue = currentValue;
						if (monitor.Type != "String" && double.TryParse(currentValue, out double value) && valueHistory.MaxValue < value) valueHistory.MaxValue = value;

						// 检查是否满足惩罚条件
						foreach (ScenarioPunishment scenario in monitor.Scenarios)
						{
							if (ScenarioJudge.Match(scenario.Scenario, scenario.CompareValue, valueHistory))
							{
								DebugHub.Log("惩罚触发", $"触发情景[{scenario.Scenario}]，执行[{scenario.Action}]");
								int setValue = (int)PunishmentValueCalculator.Calculate(scenario, valueHistory);
								ScenarioActionExecutor.Execute(scenario, setValue);
							}
						}

						valueHistory.LastValue = currentValue;
						valueHistories[key] = valueHistory;
					}
					await Task.Delay(GameValueDetectorPage.SleepTime, token);
				}
			}
			catch (TaskCanceledException) { }
			catch (Exception ex)
			{
				DebugHub.Error("检测线程异常", $"{ex.Message}\n{ex.StackTrace}");
			}
			finally
			{
				if (hProcess != IntPtr.Zero) Win32Helper.CloseHandle(hProcess);
			}
		}
	}
}