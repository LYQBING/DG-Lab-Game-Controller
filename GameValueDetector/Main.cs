using DGLabGameController.Core.Module;
using System.Windows.Controls;

namespace GameValueDetector
{
	internal class Main : ModuleBase
	{
		public override string ModuleId => "GameValueDetector";
		public override string Name => "天使的心跳";
		public override string Description => "用于加载用户或第三方所编写的基址脚本，根据内存基址中的数据配合脚本规则进行惩罚计算";
		public override string Author => "LYQBING";
		public override string Version => "V2.1.00";
		public override int CompatibleApiVersion => 10087;

		protected override UserControl CreatePage()
		{
			return new GameValueDetectorPage(ModuleId);
		}
	}
}
