using lyqbing.DGLAB;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Security.Policy;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DGLabGameVibrationController
{
	partial class MainForm
	{
		// TabControl及页面
		private BorderlessTabControl tabMain;
		private TabPage tabInject;
		private TabPage tabOutput;
		private TabPage tabSettings;

		// 注入页面控件
		private Panel pnlInjectMargin;
		private TableLayoutPanel tblInject;
		private TableLayoutPanel tblInjectTop;
		private RoundButton btnRefresh;
		private ListBox lstProcesses;
		private RoundButton btnInject;

		// 输出页面控件
		private Panel pnlOutputMargin;
		private TableLayoutPanel tblOutput;
		private TableLayoutPanel pnlOutputTop;
		private Label applog;
		private RichTextBox txtLog;
		private RoundButton btnClearLog;

		// 设置页面控件
		private Panel pnlSettingsMargin;
		private Panel pnlSettingsScroll;
		private TableLayoutPanel tblSettings;

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.tabMain = new BorderlessTabControl();
			this.tabInject = new System.Windows.Forms.TabPage();
			this.tabOutput = new System.Windows.Forms.TabPage();
			this.tabSettings = new System.Windows.Forms.TabPage();

			// 注入页控件
			this.pnlInjectMargin = new System.Windows.Forms.Panel();
			this.tblInjectTop = new System.Windows.Forms.TableLayoutPanel();
			this.tblInject = new System.Windows.Forms.TableLayoutPanel();
			this.btnRefresh = new DGLabGameVibrationController.RoundButton();
			this.btnInject = new DGLabGameVibrationController.RoundButton();
			this.lstProcesses = new System.Windows.Forms.ListBox();

			// 输出页控件
			this.pnlOutputMargin = new System.Windows.Forms.Panel();
			this.tblOutput = new System.Windows.Forms.TableLayoutPanel();
			this.pnlOutputTop = new System.Windows.Forms.TableLayoutPanel();
			this.applog = new System.Windows.Forms.Label();
			this.btnClearLog = new DGLabGameVibrationController.RoundButton();
			this.txtLog = new System.Windows.Forms.RichTextBox();

			// 设置页控件
			this.pnlSettingsMargin = new Panel();
			this.pnlSettingsScroll = new Panel();
			this.tblSettings = new TableLayoutPanel();


			// TabControl
			this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabMain.TabPages.Add(this.tabInject);
			this.tabMain.TabPages.Add(this.tabOutput);
			this.tabMain.TabPages.Add(this.tabSettings);

			// ============================== 主窗体 ============================== //

			this.BackColor = AppColor.BackgroundColor;
			this.ClientSize = new System.Drawing.Size(350, 575);
			this.Controls.Add(this.tabMain);
			this.ForeColor = Color.White;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "DGLabGameVibrationController";
			this.ResumeLayout(false);

			// ============================== 注入页 ============================== //

			this.tabInject.Text = "注入";
			this.tabInject.BackColor = AppColor.BackgroundColor;
			this.tabInject.Controls.Add(this.pnlInjectMargin);

			// 注入页 Margin
			this.pnlInjectMargin.Dock = DockStyle.Fill;
			this.pnlInjectMargin.Padding = new Padding(4);
			this.pnlInjectMargin.BackColor = AppColor.BackgroundColor;
			this.pnlInjectMargin.Controls.Add(this.tblInject);

			// 注入页 TableLayout
			this.tblInject.ColumnCount = 1;
			this.tblInject.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			this.tblInject.RowCount = 2;
			this.tblInject.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
			this.tblInject.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			this.tblInject.Dock = DockStyle.Fill;
			this.tblInject.Controls.Add(this.tblInjectTop, 0, 0);
			this.tblInject.Controls.Add(this.lstProcesses, 0, 1);

			// 注入页顶部 TableLayout
			this.tblInjectTop.ColumnCount = 2;
			this.tblInjectTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
			this.tblInjectTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
			this.tblInjectTop.RowCount = 1;
			this.tblInjectTop.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			this.tblInjectTop.Dock = DockStyle.Fill;
			this.tblInjectTop.Controls.Add(this.btnRefresh, 0, 0);
			this.tblInjectTop.Controls.Add(this.btnInject, 1, 0);

			// 刷新进程按钮
			this.btnRefresh.BackColor = AppColor.StandardColor;
			this.btnRefresh.CornerRadius = 12;
			this.btnRefresh.Dock = DockStyle.Fill;
			this.btnRefresh.FlatAppearance.BorderSize = 0;
			this.btnRefresh.FlatStyle = FlatStyle.Flat;
			this.btnRefresh.ForeColor = AppColor.BackgroundColor;
			this.btnRefresh.Text = "刷新进程";
			this.btnRefresh.UseVisualStyleBackColor = false;

			// 注入程序按钮
			this.btnInject.BackColor = AppColor.AuxiliaryColor;
			this.btnInject.CornerRadius = 12;
			this.btnInject.Dock = DockStyle.Fill;
			this.btnInject.FlatAppearance.BorderSize = 0;
			this.btnInject.FlatStyle = FlatStyle.Flat;
			this.btnInject.ForeColor = AppColor.HighlightColor;
			this.btnInject.Text = "注入";
			this.btnInject.UseVisualStyleBackColor = false;

			// 进程列表窗体
			this.lstProcesses.BackColor = AppColor.BackgroundColorLight;
			this.lstProcesses.BorderStyle = BorderStyle.None;
			this.lstProcesses.Dock = DockStyle.Fill;
			this.lstProcesses.ForeColor = AppColor.HighlightColor;

			// ============================== 输出页 ============================== //

			this.tabOutput.Text = "输出";
			this.tabOutput.BackColor = AppColor.BackgroundColor;
			this.tabOutput.Controls.Add(this.pnlOutputMargin);

			// 输出页 Margin
			this.pnlOutputMargin.Dock = DockStyle.Fill;
			this.pnlOutputMargin.Padding = new Padding(4);
			this.pnlOutputMargin.BackColor = AppColor.BackgroundColor;
			this.pnlOutputMargin.Controls.Add(this.tblOutput);

			// 输出页 TableLayout
			this.tblOutput.ColumnCount = 1;
			this.tblOutput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			this.tblOutput.RowCount = 2;
			this.tblOutput.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
			this.tblOutput.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			this.tblOutput.Dock = DockStyle.Fill;
			this.tblOutput.Controls.Add(this.pnlOutputTop, 0, 0);
			this.tblOutput.Controls.Add(this.txtLog, 0, 1);

			// 输出页顶部 TableLayout
			this.pnlOutputTop.ColumnCount = 2;
			this.pnlOutputTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
			this.pnlOutputTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
			this.pnlOutputTop.RowCount = 1;
			this.pnlOutputTop.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			this.pnlOutputTop.Dock = DockStyle.Fill;
			this.pnlOutputTop.Controls.Add(this.applog, 0, 0);
			this.pnlOutputTop.Controls.Add(this.btnClearLog, 1, 0);

			// 杂鱼日志标签
			this.applog.Dock = DockStyle.Fill;
			this.applog.ForeColor = AppColor.StandardColor;
			this.applog.Text = "杂鱼日志";
			this.applog.TextAlign = ContentAlignment.MiddleLeft;
			this.applog.Font = new System.Drawing.Font("微软雅黑", 12F, FontStyle.Bold);

			// 清空日志按钮
			this.btnClearLog.BackColor = AppColor.AuxiliaryColor;
			this.btnClearLog.CornerRadius = 12;
			this.btnClearLog.Dock = DockStyle.Fill;
			this.btnClearLog.FlatAppearance.BorderSize = 0;
			this.btnClearLog.FlatStyle = FlatStyle.Flat;
			this.btnClearLog.ForeColor = AppColor.HighlightColor;
			this.btnClearLog.Text = "清空";
			this.btnClearLog.UseVisualStyleBackColor = false;

			// 日志输出窗体
			this.txtLog.BackColor = AppColor.BackgroundColorLight;
			this.txtLog.BorderStyle = BorderStyle.None;
			this.txtLog.Dock = DockStyle.Fill;
			this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
			this.txtLog.ForeColor = AppColor.HighlightColor;
			this.txtLog.Multiline = true;
			this.txtLog.ReadOnly = true;

			// ============================== 设置页 ============================== //

			this.tabSettings.Text = "设置";
			this.tabSettings.BackColor = AppColor.BackgroundColor;
			this.tabSettings.Controls.Add(this.pnlSettingsMargin);
			this.pnlSettingsMargin.Dock = DockStyle.Fill;
			this.pnlSettingsMargin.Padding = new Padding(4);
			this.pnlSettingsMargin.BackColor = AppColor.BackgroundColor;
			this.pnlSettingsMargin.Controls.Add(this.pnlSettingsScroll);

			// 滚动面板
			this.pnlSettingsScroll.Dock = DockStyle.Fill;
			this.pnlSettingsScroll.AutoScroll = true;
			this.pnlSettingsScroll.Controls.Add(this.tblSettings);
			this.pnlSettingsScroll.BorderStyle = BorderStyle.None;

			// 设置表格布局
			this.tblSettings.ColumnCount = 1;
			this.tblSettings.RowCount = 4;
			this.tblSettings.Dock = DockStyle.Top;
			this.tblSettings.AutoSize = true;
			this.tblSettings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tblSettings.Padding = new Padding(8);
			this.tblSettings.BackColor = Color.FromArgb(24, 24, 24);
		}
	}
}