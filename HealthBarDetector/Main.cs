using DGLabGameController;
using System.Windows.Controls;

namespace HealthBarDetector
{
	public class Main : ModuleBase
	{
		public override string ModuleId => "HealthBarDetector";
		public override string Name => "羽翼的色彩";
		public override string Description => "实时检测框选区域的颜色数据，并根据用户所设置的检测与惩罚规则计算惩罚数据";
		public override string Version => "V4.0.0";
		public override string Author => "LYQBING";
		public override int CompatibleApiVersion => 10086;

		protected override UserControl CreatePage()
		{
			DebugHub.Log("羽翼的色彩", "只需要判断这里有没有那个颜色吗？嗯...简单！");
			return new HealthBarDetectorPage();
		}
	}
}
