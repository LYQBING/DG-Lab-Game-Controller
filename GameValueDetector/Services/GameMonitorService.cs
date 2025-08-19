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
			try
			{
				while (!token.IsCancellationRequested)
				{
					foreach (MonitorItem monitor in config.Monitors)
					{
						// 读取当前内存的值
						string? currentValue = MemoryReader.ReadValue(process, monitor, config.Is32Bit)?.ToString();
						if (currentValue == null) continue;

						// 获取历史值
						HistoryValue historyValue = monitor.History ?? new();
						historyValue.InitialValue = currentValue;

						// 若为数值类型，更新最大值
						if (monitor.Type != "String" && double.TryParse(currentValue, out double value) && historyValue.MaxValue < value)
						historyValue.MaxValue = value;

						// 检查是否满足惩罚条件
						foreach (ScenarioPunishment scenario in monitor.Scenarios)
						{
							if (ScenarioJudge.Match(scenario.Scenario, scenario.CompareValue, historyValue))
							{
								float calcValue = PunishmentValueCalculator.Calculate(scenario, historyValue);
								float totalValue = calcValue + scenario.AccumulatedValue;
								int setValue = (int)totalValue;
								if (setValue > 0)
								{
									DebugHub.Log("执行事件", $"触发情景：{scenario.Scenario}，惩罚值：{setValue}, 内存值：{currentValue}");
									ScenarioActionExecutor.Execute(scenario, setValue);

									// 扣除已执行的惩罚值，保留小数部分
									scenario.AccumulatedValue = totalValue - setValue;
								}
								else scenario.AccumulatedValue += calcValue;
							}
						}

						historyValue.LastValue = currentValue;
						monitor.History = historyValue;
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