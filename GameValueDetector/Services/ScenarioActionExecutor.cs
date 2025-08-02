using DGLabGameController.Core.Debug;
using DGLabGameController.Core.DGLabApi;
using GameValueDetector.Models;

namespace GameValueDetector.Services
{
	/// <summary>
	/// 惩罚情景执行器
	/// </summary>
	public static class ScenarioActionExecutor
	{
		public static void Execute(ScenarioPunishment scenario, object? lastValue, object? currentValue)
		{
			int value = (int)PunishmentValueCalculator.Calculate(scenario, lastValue, currentValue);

			switch (scenario.Action)
			{
				case "SetStrengthSet": DGLab.SetStrength.Set(value); break;
				case "SetStrengthAdd": DGLab.SetStrength.Add(value); break;
				case "SetStrengthSub": DGLab.SetStrength.Sub(value); break;

				case "SetRandomStrengthSet": DGLab.SetRandomStrength.Set(value); break;
				case "SetRandomStrengthAdd": DGLab.SetRandomStrength.Add(value); break;
				case "SetRandomStrengthSub": DGLab.SetRandomStrength.Sub(value); break;

				case "Fire": DGLab.Fire(value, scenario.Time, scenario.Overrides); break;
				default: DebugHub.Warning("未知惩罚动作", $"未识别的惩罚动作：{scenario.Action}"); break;
			}
		}
	}
}