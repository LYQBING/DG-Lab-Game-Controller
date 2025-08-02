using DGLabGameController.Core.Debug;
using GameValueDetector.Models;
using System.Diagnostics;

namespace GameValueDetector.Services
{
	/// <summary>
	/// 游戏监控服务
	/// </summary>
	/// <param name="config">配置文件</param>
	/// <param name="lastValues">上次读取值</param>
	public class GameMonitorService(GameMonitorConfig config, Dictionary<string, object?> lastValues)
	{
		/// <summary>
		/// 异步监控循环
		/// </summary>
		/// <param name="process">目标进程</param>
		/// <param name="token">令牌</param>
		/// <returns></returns>
		public async Task MonitorLoopAsync(Process process, CancellationToken token)
		{
			try
			{
				while (!token.IsCancellationRequested)
				{
					foreach (MonitorItem monitor in config.Monitors)
					{
						// 获取上次读取的值
						string key = monitor.BaseAddress;
						lastValues.TryGetValue(key, out object? lastValue);

						// 读取当前值
						object? currentValue = MemoryReader.ReadValue(process, monitor);
						if (currentValue == null) continue;

						// 检查是否满足惩罚条件
						foreach (ScenarioPunishment scenario in monitor.Scenarios)
						{
							if (ScenarioJudge.Match(scenario.Scenario, lastValue, currentValue, scenario.CompareValue))
							{
								DebugHub.Log("惩罚触发", $"触发情景[{scenario.Scenario}]，执行[{scenario.Action}]");
								ScenarioActionExecutor.Execute(scenario, lastValue, currentValue);
							}
						}
						lastValues[key] = currentValue;
					}
					await Task.Delay(GameValueDetectorPage.SleepTime, token);
				}
			}
			catch (TaskCanceledException) { }
			catch (Exception ex)
			{
				DebugHub.Error("检测线程异常", $"{ex.Message}\n{ex.StackTrace}");
			}
		}
	}
}