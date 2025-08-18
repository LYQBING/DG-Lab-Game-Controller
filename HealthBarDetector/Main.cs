using DGLabGameController.Core.Debug;
using DGLabGameController.Core.Module;
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
		public override int CompatibleApiVersion => 10087;

		protected override UserControl CreatePage()
		{
			DebugHub.Clear();
			return new HealthBarDetectorPage(ModuleId);
		}

		public override void OnModulePageClosed()
		{
			if (_page is HealthBarDetectorPage healthBarDetectorPage)
			{
				healthBarDetectorPage.Dispose(); // 释放资源
			}
			_page = null;
		}
	}
}
