using DGLabGameController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GamepadVibrationProcessor
{
	public class Main : ModuleBase
	{
		public override string Name => "手柄的振动天罚";
		public override string Info => "V2.2.00 来自 LYQBING";
		public override string Description => "根据游戏向手柄发送的震动数据进行计算惩罚数据，并将惩罚数据输出至设备";

		// 当页面创建时
		protected override UserControl CreatePage()
		{
			InjectionManager.StartPipeServer();
			DebugHub.Log("手柄的振动天罚","主人！接下来请多多指教喽");
			return new HandleInjection();
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
