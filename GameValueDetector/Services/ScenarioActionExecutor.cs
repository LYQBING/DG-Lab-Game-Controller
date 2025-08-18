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
		public static void Execute(ScenarioPunishment scenario, int value)
		{
			switch (scenario.Action)
			{
				case "SetStrengthSet": DGLab.SetStrength.Set(value); break;
				case "SetStrengthAdd": DGLab.SetStrength.Add(Math.Max(value, 1)); break;
				case "SetStrengthSub": DGLab.SetStrength.Sub(Math.Max(value, 1)); break;

				case "SetRandomStrengthSet": DGLab.SetRandomStrength.Set(value); break;
				case "SetRandomStrengthAdd": DGLab.SetRandomStrength.Add(Math.Max(value, 1)); break;
				case "SetRandomStrengthSub": DGLab.SetRandomStrength.Sub(Math.Max(value, 1)); break;

				case "Fire": DGLab.Fire(Math.Max(value, 1), scenario.Time, scenario.Overrides); break;
				default: DebugHub.Warning("未知惩罚动作", $"未识别的惩罚动作：{scenario.Action}"); break;
			}
		}
	}
}