using DGLabGameController.Core.Debug;
using GameValueDetector.Models;
using System.Diagnostics;
using System.Threading;

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
						if (!IsRunConditionMet(monitor.StartCondition, currentValue, monitor.Data)) continue;

						// 开始执行情景惩罚
						Parallel.ForEach(monitor.Scenarios, scenario =>
						{
							if (ScenarioJudge.Match(scenario.Scenario, scenario.CompareValue, monitor.Data))
							{
								float calcValue = PunishmentValueCalculator.Calculate(scenario, monitor.Data);
								float totalValue = calcValue + scenario.AccumulatedValue;
								int setValue = (int)totalValue;

								if (MathF.Abs(setValue) > 0)
								{
									ScenarioActionExecutor.Execute(scenario, setValue);
									scenario.AccumulatedValue = totalValue - setValue;
								}
								else scenario.AccumulatedValue += calcValue;
							}
						});
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
		private static bool IsRunConditionMet(string condition, float currentValue, DataValue dataValue)
		{
			bool isRun = condition switch
			{
				// 无条件运行
				"Always" => true,

				// 最大值变化时运行
				"MaxValueUpdated" => currentValue > dataValue.MaxValue,

				// 最大值不变时运行
				"MaxValueUnchanged" => currentValue <= dataValue.MaxValue,

				// 最大值小于参考值时运行
				"MaxValueLessThanReference" => dataValue.InitialMaxValue < dataValue.MaxValue,

				// 最大值大于参考值时运行
				"MaxValueGreaterThanReference" => dataValue.InitialMaxValue > dataValue.MaxValue,

				// 当前值大于参考值时运行
				"CurrentValueGreaterThanReference" => currentValue > dataValue.InitialMaxValue,

				// 当前值小于参考值时运行
				"CurrentValueLessThanReference" => currentValue < dataValue.InitialMaxValue,

				// 值不为空时运行
				"ValueNotZero" => dataValue.InitialValue != 0 || dataValue.MaxValue != 0 || dataValue.LastValue != 0,

				_ => false,
			};

			if (dataValue.InitialMaxValue == 0 && isRun == true) dataValue.InitialMaxValue = currentValue;
			dataValue.MaxValue = MathF.Max(dataValue.MaxValue, currentValue);

			dataValue.LastValue = dataValue.InitialValue;
			dataValue.InitialValue = currentValue;

			return isRun;
		}
	}
}