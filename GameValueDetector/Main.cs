using DGLabGameController.Core.Module;
using System.Windows.Controls;

namespace GameValueDetector
{
	internal class Main : ModuleBase
	{
		public override string ModuleId => "GameValueDetector";
		public override string Name => "游戏数值检测器";
		public override string Description => "这里是介绍";
		public override string Author => "LYQBING";
		public override string Version => "0.00.001";
		public override int CompatibleApiVersion => 10087;

		protected override UserControl CreatePage()
		{
			return new GameValueDetectorPage(ModuleId);
		}
	}
}
