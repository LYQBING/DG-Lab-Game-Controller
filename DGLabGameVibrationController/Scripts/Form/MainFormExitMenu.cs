using System;
using System.Windows.Forms;

namespace DGLabGameVibrationController
{
	partial class MainForm
	{
		private NotifyIcon notifyIcon;
		private ContextMenuStrip contextMenuStrip;
		private ToolStripMenuItem exitMenuItem;

		private void InitializeExitMenu(bool exitMenu)
		{
			// 如果不启用退出菜单，则不进行任何操作
			if (!exitMenu) return;

			// 监听窗体关闭事件，防止用户直接关闭程序
			this.FormClosing += MainForm_FormClosing;

			// 托盘菜单
			contextMenuStrip = new ContextMenuStrip();
			exitMenuItem = new ToolStripMenuItem("退出");
			exitMenuItem.Click += (s, e) => Application.Exit();
			contextMenuStrip.Items.Add(exitMenuItem);

			// 托盘图标
			notifyIcon = new NotifyIcon
			{
				Icon = this.Icon,
				Text = "DGLabGameVibrationController",
				Visible = true,
				ContextMenuStrip = contextMenuStrip
			};
			notifyIcon.DoubleClick += (s, e) => ShowMainForm();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				this.Hide();
				notifyIcon.ShowBalloonTip(1000, "别想着逃跑哦！杂鱼！", "我会在后台悄悄盯着你哦~",ToolTipIcon.None);
			}
		}

		private void ShowMainForm()
		{
			this.Show();
			this.WindowState = FormWindowState.Normal;
			this.Activate();
		}
	}
}
