using DGLabGameController;
using System.Windows.Controls;

namespace HealthBarDetector
{
	public class Main : ModuleBase
	{
		public override string ModuleId => "HealthBarDetector";
		public override string Name => "羽翼的色彩";
		public override string Description => "实时检测框选区域的颜色占据比例计算惩罚数据：可用于血条监测、颜色触发等处罚场景";
		public override string Version => "V3.01.000";
		public override string Author => "LYQBING";
		public override int CompatibleApiVersion => 10086;

		protected override UserControl CreatePage()
		{
			DebugHub.Log("羽翼的色彩", "只需要判断这里有没有那个颜色吗？嗯...简单！");
			return new HealthBarDetectorPage();
		}
	}
}
