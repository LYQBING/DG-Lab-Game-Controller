using System;
using System.Windows.Forms;

namespace DGLabGameVibrationController
{
	public class BorderlessTabControl : TabControl
	{
		protected override void WndProc(ref Message m)
		{
			// TCM_ADJUSTRECT 消息，去除边框
			if (m.Msg == 0x1328 && this.Appearance == TabAppearance.FlatButtons)
			{
				m.Result = (IntPtr)1;
				return;
			}
			base.WndProc(ref m);
		}
	}
}
