using DGLabGameController.Core.Debug;
using GameValueDetector.Models;
using System.Diagnostics;

namespace GameValueDetector.Services
{
	/// <summary>
	/// 游戏监控服务
	/// </summary>
	/// <param name="config">配置文件</param>
	/// <param name="valueHistories">全部的历史值表</param>
	public class GameMonitorService(GameMonitorConfig config)
	{
		/// <summary>
		/// 异步监控循环
		/// </summary>
		/// <param name="process">目标进程</param>
		/// <param name="token">令牌</param>
		/// <returns></returns>
		public async Task MonitorLoopAsync(Process process, CancellationToken token)
		{
			IntPtr hProcess = IntPtr.Zero;
			try
			{
				while (!token.IsCancellationRequested)
				{
					// 检查进程是否仍在运行
					if (process.HasExited)
					{
						DebugHub.Warning("进程已关闭", "等待进程重启中...");
						while (!token.IsCancellationRequested)
						{
							Process? newProcess = ProcessManager.FindProcessByName(Process.GetProcesses(), config.ProcessName);
							if (newProcess != null && !newProcess.HasExited)
							{
								process = newProcess;
								DebugHub.Success("进程已重启", "已重新连接至新进程！");
								break;
							}
							await Task.Delay(GameValueDetectorPage.SleepTime, token);
						}
						continue;
					}

					// 遍历并执行所有监控项
					if (hProcess == IntPtr.Zero && !MemoryReader.OpenProcessHandle(process,out hProcess)) continue;
					foreach (MonitorItem monitor in config.Monitors)
					{
						// 读取当前内存的值
						if (!MemoryReader.TryReadValue(process, monitor, config.Is32Bit, out float currentValue, hProcess)) continue;

						// 更新历史值
						HistoryValue historyValue = monitor.History;
						historyValue.InitialValue = currentValue;
						if (historyValue.MaxValue < currentValue) historyValue.MaxValue = currentValue;

						// 检查是否满足惩罚条件
						Parallel.ForEach(monitor.Scenarios, scenario =>
						{
							if (ScenarioJudge.Match(scenario.Scenario, scenario.CompareValue, historyValue))
							{
								float calcValue = PunishmentValueCalculator.Calculate(scenario, historyValue);
								float totalValue = calcValue + scenario.AccumulatedValue;
								int setValue = (int)totalValue;

								if (MathF.Abs(setValue) > 0)
								{
									DebugHub.Log("执行事件", $"触发情景：{scenario.Scenario}，惩罚值：{setValue}, 内存值：{currentValue}");
									ScenarioActionExecutor.Execute(scenario, setValue);

									// 扣除已执行的惩罚值，保留小数部分
									scenario.AccumulatedValue = totalValue - setValue;
								}
								else scenario.AccumulatedValue += calcValue;
							}
						});

						historyValue.LastValue = currentValue;
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
				MemoryReader.CloseProcessHandle(hProcess);
				hProcess = IntPtr.Zero;
			}
		}
	}
}