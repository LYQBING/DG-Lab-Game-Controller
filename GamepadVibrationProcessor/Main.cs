using DGLabGameController.Core.Debug;
using DGLabGameController.Core.Module;
using GamepadVibrationProcessor.Services;
using System.Windows.Controls;

namespace GamepadVibrationProcessor
{
	public class Main : ModuleBase
	{
		public override string ModuleId => "GamepadVibrationProcessor";
		public override string Name => "手柄的振动天罚";
		public override string Description => "根据游戏向手柄发送的震动数据进行计算惩罚数据，并将惩罚数据输出至设备";
		public override string Author => "LYQBING";
		public override string Version => "V3.2.10";
		public override int CompatibleApiVersion => 10087;

		// 当页面创建时
		protected override UserControl CreatePage()
		{
			InjectionManager.StartPipeServer();
			DebugHub.Log("手柄的振动天罚", "主人！接下来请多多指教喽");
			return new HandleInjection(ModuleId);
		}

		// 当模块页面关闭时
		public override void OnModulePageClosed()
		{
			if (_page is HandleInjection handleInjection)
			{
				handleInjection.ProcessList.Clear();
				handleInjection.ProcessListView.ItemsSource = null;
			}

			base.OnModulePageClosed();
			InjectionManager.StopPipeServer();
		}
	}
}
