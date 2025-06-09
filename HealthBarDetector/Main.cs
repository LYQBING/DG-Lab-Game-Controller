using DGLabGameController;
using System.Windows.Controls;

namespace HealthBarDetector
{
	public class Main : ModuleBase
	{
		public override string Name => "羽翼的色彩";

		public override string Info => "V2.0.00 来自 LYQBING";

		public override string Description => "实时检测框选区域的颜色占据比例计算惩罚数据：可用于血条监测、颜色触发等处罚场景";

		protected override UserControl CreatePage()
		{
			DebugHub.Log("羽翼的色彩", "只需要判断这里有没有那个颜色吗？嗯...简单！");
			return new HealthBarDetectorPage();
		}
	}
}
