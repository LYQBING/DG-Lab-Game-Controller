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
	public class GameMonitorService(GameMonitorConfig config, Dictionary<string, ValueHistory> valueHistories)
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
						// 获取唯一标识符
						string key = monitor.BaseAddress;
						if (!valueHistories.TryGetValue(key, out ValueHistory? valueHistory)) valueHistory = new ValueHistory();

						// 读取当前内存的值
						string? currentValue = MemoryReader.ReadValue(process, monitor, config.Is32Bit)?.ToString();
						if (currentValue == null) continue;

						// 更新当前值
						valueHistory.InitialValue = currentValue;
						if (monitor.Type != "String" && double.TryParse(currentValue, out double value) && valueHistory.MaxValue < value)
						valueHistory.MaxValue = value;

						// 检查是否满足惩罚条件
						foreach (ScenarioPunishment scenario in monitor.Scenarios)
						{
							if (ScenarioJudge.Match(scenario.Scenario, scenario.CompareValue, valueHistory))
							{
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
		}
	}
}