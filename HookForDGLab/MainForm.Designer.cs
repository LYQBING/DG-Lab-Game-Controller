using lyqbing.DGLAB;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Policy;
using System.Windows.Forms;

namespace DGLabGameVibrationController
{
	partial class MainForm
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		// 底部导航
		private TableLayoutPanel tblBottomNav;
		private Button btnNavInject;
		private Button btnNavOutput;
		private Button btnNavSettings;

		// 主区Panel
		private Panel pnlInject;
		private Panel pnlOutput;
		private Panel pnlSettings;

		// 外边距Panel
		private Panel pnlInjectMargin;
		private Panel pnlOutputMargin;
		private Panel pnlSettingsMargin;

		// 注入区原有控件
		private GroupBox grpInject;
		private TableLayoutPanel tblInject;
		private Button btnRefresh;
		private ListBox lstProcesses;
		private Button btnInject;

		// 输出区新控件
		private TableLayoutPanel tblOutput;
		private TableLayoutPanel pnlOutputTop;
		private Label applog;
		private TextBox txtLog;

		// 设置区原有控件
		private GroupBox grpServerSettings;
		private TableLayoutPanel tblServerSettings;
		private Label lblServerUrl;
		private TextBox txtServerUrl;
		private Label lblServerPort;
		private TextBox txtServerPort;
		private Label lblClientId;
		private TextBox txtClientId;
		private Button btnSetServer;
		private Button btnSetClient;

		private GroupBox grpAppSettings;
		private TableLayoutPanel tblAppSettings;
		private CheckBox chkDualFreq;
		private Label lblDualFreqInfo;
		private Label lblBaseStrength;
		private TextBox txtBaseStrength;
		private Label lblStrengthLimit;
		private TextBox txtStrengthLimit;
		private Label lblOutputMultiplier;
		private TrackBar trbOutputMultiplier;
		private Label lblOutputMultiplierValue;
		private Label lblSendFrequency;
		private TrackBar trbSendFrequency;
		private Label lblSendFrequencyValue;
		// 新增设置区控件
		private Label lblControllerLimit;
		private TextBox txtControllerLimit;
		private Label lblControllerLimitTip;

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.tblBottomNav = new System.Windows.Forms.TableLayoutPanel();
			this.btnNavInject = new System.Windows.Forms.Button();
			this.btnNavOutput = new System.Windows.Forms.Button();
			this.btnNavSettings = new System.Windows.Forms.Button();
			this.pnlInject = new System.Windows.Forms.Panel();
			this.pnlInjectMargin = new System.Windows.Forms.Panel();
			this.grpInject = new System.Windows.Forms.GroupBox();
			this.tblInject = new System.Windows.Forms.TableLayoutPanel();
			this.btnRefresh = new System.Windows.Forms.Button();
			this.btnInject = new System.Windows.Forms.Button();
			this.lstProcesses = new System.Windows.Forms.ListBox();
			this.pnlOutput = new System.Windows.Forms.Panel();
			this.pnlOutputMargin = new System.Windows.Forms.Panel();
			this.tblOutput = new System.Windows.Forms.TableLayoutPanel();
			this.pnlOutputTop = new System.Windows.Forms.TableLayoutPanel();
			this.applog = new System.Windows.Forms.Label();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.pnlSettings = new System.Windows.Forms.Panel();
			this.pnlSettingsMargin = new System.Windows.Forms.Panel();
			this.grpAppSettings = new System.Windows.Forms.GroupBox();
			this.tblAppSettings = new System.Windows.Forms.TableLayoutPanel();
			this.chkDualFreq = new System.Windows.Forms.CheckBox();
			this.lblDualFreqInfo = new System.Windows.Forms.Label();
			this.lblBaseStrength = new System.Windows.Forms.Label();
			this.txtBaseStrength = new System.Windows.Forms.TextBox();
			this.lblStrengthLimit = new System.Windows.Forms.Label();
			this.txtStrengthLimit = new System.Windows.Forms.TextBox();
			this.lblControllerLimit = new System.Windows.Forms.Label();
			this.txtControllerLimit = new System.Windows.Forms.TextBox();
			this.lblControllerLimitTip = new System.Windows.Forms.Label();
			this.lblOutputMultiplier = new System.Windows.Forms.Label();
			this.trbOutputMultiplier = new System.Windows.Forms.TrackBar();
			this.lblOutputMultiplierValue = new System.Windows.Forms.Label();
			this.lblSendFrequency = new System.Windows.Forms.Label();
			this.trbSendFrequency = new System.Windows.Forms.TrackBar();
			this.lblSendFrequencyValue = new System.Windows.Forms.Label();
			this.grpServerSettings = new System.Windows.Forms.GroupBox();
			this.tblServerSettings = new System.Windows.Forms.TableLayoutPanel();
			this.lblServerUrl = new System.Windows.Forms.Label();
			this.txtServerUrl = new System.Windows.Forms.TextBox();
			this.lblServerPort = new System.Windows.Forms.Label();
			this.txtServerPort = new System.Windows.Forms.TextBox();
			this.btnSetServer = new System.Windows.Forms.Button();
			this.lblClientId = new System.Windows.Forms.Label();
			this.txtClientId = new System.Windows.Forms.TextBox();
			this.btnSetClient = new System.Windows.Forms.Button();
			this.tblBottomNav.SuspendLayout();
			this.pnlInject.SuspendLayout();
			this.pnlInjectMargin.SuspendLayout();
			this.grpInject.SuspendLayout();
			this.tblInject.SuspendLayout();
			this.pnlOutput.SuspendLayout();
			this.pnlOutputMargin.SuspendLayout();
			this.tblOutput.SuspendLayout();
			this.pnlOutputTop.SuspendLayout();
			this.pnlSettings.SuspendLayout();
			this.pnlSettingsMargin.SuspendLayout();
			this.grpAppSettings.SuspendLayout();
			this.tblAppSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trbOutputMultiplier)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trbSendFrequency)).BeginInit();
			this.grpServerSettings.SuspendLayout();
			this.tblServerSettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// tblBottomNav
			// 
			this.tblBottomNav.BackColor = System.Drawing.Color.White;
			this.tblBottomNav.ColumnCount = 3;
			this.tblBottomNav.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
			this.tblBottomNav.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
			this.tblBottomNav.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
			this.tblBottomNav.Controls.Add(this.btnNavInject, 0, 0);
			this.tblBottomNav.Controls.Add(this.btnNavOutput, 1, 0);
			this.tblBottomNav.Controls.Add(this.btnNavSettings, 2, 0);
			this.tblBottomNav.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tblBottomNav.Location = new System.Drawing.Point(0, 552);
			this.tblBottomNav.Name = "tblBottomNav";
			this.tblBottomNav.RowCount = 1;
			this.tblBottomNav.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
			this.tblBottomNav.Size = new System.Drawing.Size(420, 48);
			this.tblBottomNav.TabIndex = 3;
			// 
			// btnNavInject
			// 
			this.btnNavInject.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnNavInject.Location = new System.Drawing.Point(3, 3);
			this.btnNavInject.Name = "btnNavInject";
			this.btnNavInject.Size = new System.Drawing.Size(134, 42);
			this.btnNavInject.TabIndex = 0;
			this.btnNavInject.Text = "注入";
			this.btnNavInject.Click += new System.EventHandler(this.BtnNavInject_Click);
			// 
			// btnNavOutput
			// 
			this.btnNavOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnNavOutput.Location = new System.Drawing.Point(143, 3);
			this.btnNavOutput.Name = "btnNavOutput";
			this.btnNavOutput.Size = new System.Drawing.Size(134, 42);
			this.btnNavOutput.TabIndex = 1;
			this.btnNavOutput.Text = "输出";
			this.btnNavOutput.Click += new System.EventHandler(this.BtnNavOutput_Click);
			// 
			// btnNavSettings
			// 
			this.btnNavSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnNavSettings.Location = new System.Drawing.Point(283, 3);
			this.btnNavSettings.Name = "btnNavSettings";
			this.btnNavSettings.Size = new System.Drawing.Size(134, 42);
			this.btnNavSettings.TabIndex = 2;
			this.btnNavSettings.Text = "设置";
			this.btnNavSettings.Click += new System.EventHandler(this.BtnNavSettings_Click);
			// 
			// pnlInject
			// 
			this.pnlInject.BackColor = System.Drawing.Color.White;
			this.pnlInject.Controls.Add(this.pnlInjectMargin);
			this.pnlInject.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlInject.Location = new System.Drawing.Point(0, 0);
			this.pnlInject.Name = "pnlInject";
			this.pnlInject.Size = new System.Drawing.Size(420, 552);
			this.pnlInject.TabIndex = 0;
			// 
			// pnlInjectMargin
			// 
			this.pnlInjectMargin.BackColor = System.Drawing.Color.White;
			this.pnlInjectMargin.Controls.Add(this.grpInject);
			this.pnlInjectMargin.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlInjectMargin.Location = new System.Drawing.Point(0, 0);
			this.pnlInjectMargin.Name = "pnlInjectMargin";
			this.pnlInjectMargin.Padding = new System.Windows.Forms.Padding(16);
			this.pnlInjectMargin.Size = new System.Drawing.Size(420, 552);
			this.pnlInjectMargin.TabIndex = 0;
			// 
			// grpInject
			// 
			this.grpInject.Controls.Add(this.tblInject);
			this.grpInject.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grpInject.Location = new System.Drawing.Point(16, 16);
			this.grpInject.Name = "grpInject";
			this.grpInject.Size = new System.Drawing.Size(388, 520);
			this.grpInject.TabIndex = 0;
			this.grpInject.TabStop = false;
			// 
			// tblInject
			// 
			this.tblInject.ColumnCount = 2;
			this.tblInject.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
			this.tblInject.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tblInject.Controls.Add(this.btnRefresh, 0, 0);
			this.tblInject.Controls.Add(this.btnInject, 1, 0);
			this.tblInject.Controls.Add(this.lstProcesses, 0, 1);
			this.tblInject.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tblInject.Location = new System.Drawing.Point(3, 17);
			this.tblInject.Name = "tblInject";
			this.tblInject.RowCount = 2;
			this.tblInject.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.tblInject.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tblInject.Size = new System.Drawing.Size(382, 500);
			this.tblInject.TabIndex = 0;
			// 
			// btnRefresh
			// 
			this.btnRefresh.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnRefresh.Location = new System.Drawing.Point(3, 3);
			this.btnRefresh.Name = "btnRefresh";
			this.btnRefresh.Size = new System.Drawing.Size(299, 29);
			this.btnRefresh.TabIndex = 0;
			this.btnRefresh.Text = "刷新进程";
			// 
			// btnInject
			// 
			this.btnInject.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnInject.Location = new System.Drawing.Point(308, 3);
			this.btnInject.Name = "btnInject";
			this.btnInject.Size = new System.Drawing.Size(71, 29);
			this.btnInject.TabIndex = 1;
			this.btnInject.Text = "注入";
			// 
			// lstProcesses
			// 
			this.tblInject.SetColumnSpan(this.lstProcesses, 2);
			this.lstProcesses.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstProcesses.ItemHeight = 12;
			this.lstProcesses.Location = new System.Drawing.Point(3, 38);
			this.lstProcesses.Name = "lstProcesses";
			this.lstProcesses.Size = new System.Drawing.Size(376, 459);
			this.lstProcesses.TabIndex = 2;
			// 
			// pnlOutput
			// 
			this.pnlOutput.BackColor = System.Drawing.Color.White;
			this.pnlOutput.Controls.Add(this.pnlOutputMargin);
			this.pnlOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlOutput.Location = new System.Drawing.Point(0, 0);
			this.pnlOutput.Name = "pnlOutput";
			this.pnlOutput.Size = new System.Drawing.Size(420, 552);
			this.pnlOutput.TabIndex = 1;
			// 
			// pnlOutputMargin
			// 
			this.pnlOutputMargin.BackColor = System.Drawing.Color.White;
			this.pnlOutputMargin.Controls.Add(this.tblOutput);
			this.pnlOutputMargin.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlOutputMargin.Location = new System.Drawing.Point(0, 0);
			this.pnlOutputMargin.Name = "pnlOutputMargin";
			this.pnlOutputMargin.Padding = new System.Windows.Forms.Padding(16);
			this.pnlOutputMargin.Size = new System.Drawing.Size(420, 552);
			this.pnlOutputMargin.TabIndex = 0;
			// 
			// tblOutput
			// 
			this.tblOutput.ColumnCount = 1;
			this.tblOutput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
			this.tblOutput.Controls.Add(this.pnlOutputTop, 0, 0);
			this.tblOutput.Controls.Add(this.txtLog, 0, 1);
			this.tblOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tblOutput.Location = new System.Drawing.Point(16, 16);
			this.tblOutput.Name = "tblOutput";
			this.tblOutput.RowCount = 2;
			this.tblOutput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.tblOutput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tblOutput.Size = new System.Drawing.Size(388, 520);
			this.tblOutput.TabIndex = 0;
			// 
			// pnlOutputTop
			// 
			this.pnlOutputTop.ColumnCount = 2;
			this.pnlOutputTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.pnlOutputTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.pnlOutputTop.Controls.Add(this.applog, 0, 0);
			this.pnlOutputTop.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlOutputTop.Location = new System.Drawing.Point(3, 3);
			this.pnlOutputTop.Name = "pnlOutputTop";
			this.pnlOutputTop.RowCount = 1;
			this.pnlOutputTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.pnlOutputTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.pnlOutputTop.Size = new System.Drawing.Size(382, 34);
			this.pnlOutputTop.TabIndex = 0;
			// 
			// applog
			// 
			this.applog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.applog.Location = new System.Drawing.Point(3, 0);
			this.applog.Name = "applog";
			this.applog.Size = new System.Drawing.Size(185, 34);
			this.applog.TabIndex = 0;
			this.applog.Text = "程序通讯状态：失败";
			this.applog.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtLog
			// 
			this.txtLog.BackColor = System.Drawing.Color.White;
			this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
			this.txtLog.Location = new System.Drawing.Point(3, 43);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ReadOnly = true;
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtLog.Size = new System.Drawing.Size(382, 474);
			this.txtLog.TabIndex = 1;
			// 
			// pnlSettings
			// 
			this.pnlSettings.BackColor = System.Drawing.Color.White;
			this.pnlSettings.Controls.Add(this.pnlSettingsMargin);
			this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlSettings.Location = new System.Drawing.Point(0, 0);
			this.pnlSettings.Name = "pnlSettings";
			this.pnlSettings.Size = new System.Drawing.Size(420, 552);
			this.pnlSettings.TabIndex = 2;
			// 
			// pnlSettingsMargin
			// 
			this.pnlSettingsMargin.BackColor = System.Drawing.Color.White;
			this.pnlSettingsMargin.Controls.Add(this.grpAppSettings);
			this.pnlSettingsMargin.Controls.Add(this.grpServerSettings);
			this.pnlSettingsMargin.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlSettingsMargin.Location = new System.Drawing.Point(0, 0);
			this.pnlSettingsMargin.Name = "pnlSettingsMargin";
			this.pnlSettingsMargin.Padding = new System.Windows.Forms.Padding(16);
			this.pnlSettingsMargin.Size = new System.Drawing.Size(420, 552);
			this.pnlSettingsMargin.TabIndex = 0;
			// 
			// grpAppSettings
			// 
			this.grpAppSettings.Controls.Add(this.tblAppSettings);
			this.grpAppSettings.Dock = System.Windows.Forms.DockStyle.Top;
			this.grpAppSettings.Location = new System.Drawing.Point(16, 126);
			this.grpAppSettings.Name = "grpAppSettings";
			this.grpAppSettings.Size = new System.Drawing.Size(388, 200);
			this.grpAppSettings.TabIndex = 0;
			this.grpAppSettings.TabStop = false;
			this.grpAppSettings.Text = "应用设置";
			// 
			// tblAppSettings
			// 
			this.tblAppSettings.ColumnCount = 4;
			this.tblAppSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
			this.tblAppSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
			this.tblAppSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
			this.tblAppSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.tblAppSettings.Controls.Add(this.chkDualFreq, 0, 0);
			this.tblAppSettings.Controls.Add(this.lblDualFreqInfo, 1, 0);
			this.tblAppSettings.Controls.Add(this.lblBaseStrength, 0, 1);
			this.tblAppSettings.Controls.Add(this.txtBaseStrength, 1, 1);
			this.tblAppSettings.Controls.Add(this.lblStrengthLimit, 2, 1);
			this.tblAppSettings.Controls.Add(this.txtStrengthLimit, 3, 1);
			this.tblAppSettings.Controls.Add(this.lblControllerLimit, 0, 2);
			this.tblAppSettings.Controls.Add(this.txtControllerLimit, 1, 2);
			this.tblAppSettings.Controls.Add(this.lblControllerLimitTip, 2, 2);
			this.tblAppSettings.Controls.Add(this.lblOutputMultiplier, 0, 3);
			this.tblAppSettings.Controls.Add(this.trbOutputMultiplier, 1, 3);
			this.tblAppSettings.Controls.Add(this.lblOutputMultiplierValue, 3, 3);
			this.tblAppSettings.Controls.Add(this.lblSendFrequency, 0, 4);
			this.tblAppSettings.Controls.Add(this.trbSendFrequency, 1, 4);
			this.tblAppSettings.Controls.Add(this.lblSendFrequencyValue, 3, 4);
			this.tblAppSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tblAppSettings.Location = new System.Drawing.Point(3, 17);
			this.tblAppSettings.Name = "tblAppSettings";
			this.tblAppSettings.RowCount = 5;
			this.tblAppSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tblAppSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tblAppSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tblAppSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.tblAppSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.tblAppSettings.Size = new System.Drawing.Size(382, 180);
			this.tblAppSettings.TabIndex = 0;
			// 
			// chkDualFreq
			// 
			this.chkDualFreq.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.chkDualFreq.Location = new System.Drawing.Point(3, 3);
			this.chkDualFreq.Name = "chkDualFreq";
			this.chkDualFreq.Size = new System.Drawing.Size(84, 24);
			this.chkDualFreq.TabIndex = 0;
			this.chkDualFreq.Text = "双频合一";
			// 
			// lblDualFreqInfo
			// 
			this.lblDualFreqInfo.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.tblAppSettings.SetColumnSpan(this.lblDualFreqInfo, 3);
			this.lblDualFreqInfo.Location = new System.Drawing.Point(93, 3);
			this.lblDualFreqInfo.Name = "lblDualFreqInfo";
			this.lblDualFreqInfo.Size = new System.Drawing.Size(100, 23);
			this.lblDualFreqInfo.TabIndex = 1;
			this.lblDualFreqInfo.Text = "输出值可能更小";
			// 
			// lblBaseStrength
			// 
			this.lblBaseStrength.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.lblBaseStrength.Location = new System.Drawing.Point(3, 33);
			this.lblBaseStrength.Name = "lblBaseStrength";
			this.lblBaseStrength.Size = new System.Drawing.Size(84, 23);
			this.lblBaseStrength.TabIndex = 2;
			this.lblBaseStrength.Text = "基础强度：";
			// 
			// txtBaseStrength
			// 
			this.txtBaseStrength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.txtBaseStrength.Location = new System.Drawing.Point(93, 34);
			this.txtBaseStrength.Name = "txtBaseStrength";
			this.txtBaseStrength.Size = new System.Drawing.Size(74, 21);
			this.txtBaseStrength.TabIndex = 3;
			this.txtBaseStrength.Text = "0";
			// 
			// lblStrengthLimit
			// 
			this.lblStrengthLimit.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.lblStrengthLimit.Location = new System.Drawing.Point(173, 33);
			this.lblStrengthLimit.Name = "lblStrengthLimit";
			this.lblStrengthLimit.Size = new System.Drawing.Size(84, 23);
			this.lblStrengthLimit.TabIndex = 4;
			this.lblStrengthLimit.Text = "震动强度：";
			// 
			// txtStrengthLimit
			// 
			this.txtStrengthLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.txtStrengthLimit.Location = new System.Drawing.Point(263, 34);
			this.txtStrengthLimit.Name = "txtStrengthLimit";
			this.txtStrengthLimit.Size = new System.Drawing.Size(116, 21);
			this.txtStrengthLimit.TabIndex = 5;
			this.txtStrengthLimit.Text = "20";
			// 
			// lblControllerLimit
			// 
			this.lblControllerLimit.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.lblControllerLimit.Location = new System.Drawing.Point(3, 63);
			this.lblControllerLimit.Name = "lblControllerLimit";
			this.lblControllerLimit.Size = new System.Drawing.Size(84, 23);
			this.lblControllerLimit.TabIndex = 6;
			this.lblControllerLimit.Text = "控制器值：";
			// 
			// txtControllerLimit
			// 
			this.txtControllerLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.txtControllerLimit.Location = new System.Drawing.Point(93, 64);
			this.txtControllerLimit.Name = "txtControllerLimit";
			this.txtControllerLimit.Size = new System.Drawing.Size(74, 21);
			this.txtControllerLimit.TabIndex = 7;
			this.txtControllerLimit.Text = "65535";
			// 
			// lblControllerLimitTip
			// 
			this.lblControllerLimitTip.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.tblAppSettings.SetColumnSpan(this.lblControllerLimitTip, 2);
			this.lblControllerLimitTip.Location = new System.Drawing.Point(173, 63);
			this.lblControllerLimitTip.Name = "lblControllerLimitTip";
			this.lblControllerLimitTip.Size = new System.Drawing.Size(100, 23);
			this.lblControllerLimitTip.TabIndex = 8;
			this.lblControllerLimitTip.Text = "震动最大值";
			// 
			// lblOutputMultiplier
			// 
			this.lblOutputMultiplier.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.lblOutputMultiplier.Location = new System.Drawing.Point(3, 103);
			this.lblOutputMultiplier.Name = "lblOutputMultiplier";
			this.lblOutputMultiplier.Size = new System.Drawing.Size(84, 23);
			this.lblOutputMultiplier.TabIndex = 9;
			this.lblOutputMultiplier.Text = "输出倍数：";
			// 
			// trbOutputMultiplier
			// 
			this.tblAppSettings.SetColumnSpan(this.trbOutputMultiplier, 2);
			this.trbOutputMultiplier.Dock = System.Windows.Forms.DockStyle.Fill;
			this.trbOutputMultiplier.Location = new System.Drawing.Point(93, 93);
			this.trbOutputMultiplier.Maximum = 100;
			this.trbOutputMultiplier.Minimum = 1;
			this.trbOutputMultiplier.Name = "trbOutputMultiplier";
			this.trbOutputMultiplier.Size = new System.Drawing.Size(164, 44);
			this.trbOutputMultiplier.TabIndex = 10;
			this.trbOutputMultiplier.TickFrequency = 10;
			this.trbOutputMultiplier.Value = 10;
			// 
			// lblOutputMultiplierValue
			// 
			this.lblOutputMultiplierValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblOutputMultiplierValue.Location = new System.Drawing.Point(263, 103);
			this.lblOutputMultiplierValue.Name = "lblOutputMultiplierValue";
			this.lblOutputMultiplierValue.Size = new System.Drawing.Size(100, 23);
			this.lblOutputMultiplierValue.TabIndex = 11;
			this.lblOutputMultiplierValue.Text = "1.0";
			// 
			// lblSendFrequency
			// 
			this.lblSendFrequency.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.lblSendFrequency.Location = new System.Drawing.Point(3, 153);
			this.lblSendFrequency.Name = "lblSendFrequency";
			this.lblSendFrequency.Size = new System.Drawing.Size(84, 23);
			this.lblSendFrequency.TabIndex = 12;
			this.lblSendFrequency.Text = "发送频率：";
			// 
			// trbSendFrequency
			// 
			this.tblAppSettings.SetColumnSpan(this.trbSendFrequency, 2);
			this.trbSendFrequency.Dock = System.Windows.Forms.DockStyle.Fill;
			this.trbSendFrequency.Location = new System.Drawing.Point(93, 143);
			this.trbSendFrequency.Maximum = 300;
			this.trbSendFrequency.Name = "trbSendFrequency";
			this.trbSendFrequency.Size = new System.Drawing.Size(164, 44);
			this.trbSendFrequency.TabIndex = 13;
			this.trbSendFrequency.TickFrequency = 30;
			this.trbSendFrequency.Value = 50;
			// 
			// lblSendFrequencyValue
			// 
			this.lblSendFrequencyValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblSendFrequencyValue.Location = new System.Drawing.Point(263, 153);
			this.lblSendFrequencyValue.Name = "lblSendFrequencyValue";
			this.lblSendFrequencyValue.Size = new System.Drawing.Size(100, 23);
			this.lblSendFrequencyValue.TabIndex = 14;
			this.lblSendFrequencyValue.Text = "50";
			// 
			// grpServerSettings
			// 
			this.grpServerSettings.Controls.Add(this.tblServerSettings);
			this.grpServerSettings.Dock = System.Windows.Forms.DockStyle.Top;
			this.grpServerSettings.Location = new System.Drawing.Point(16, 16);
			this.grpServerSettings.Name = "grpServerSettings";
			this.grpServerSettings.Size = new System.Drawing.Size(388, 110);
			this.grpServerSettings.TabIndex = 1;
			this.grpServerSettings.TabStop = false;
			this.grpServerSettings.Text = "服务器设置";
			// 
			// tblServerSettings
			// 
			this.tblServerSettings.ColumnCount = 4;
			this.tblServerSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
			this.tblServerSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
			this.tblServerSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
			this.tblServerSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.tblServerSettings.Controls.Add(this.lblServerUrl, 0, 0);
			this.tblServerSettings.Controls.Add(this.txtServerUrl, 1, 0);
			this.tblServerSettings.Controls.Add(this.lblServerPort, 2, 0);
			this.tblServerSettings.Controls.Add(this.txtServerPort, 3, 0);
			this.tblServerSettings.Controls.Add(this.btnSetServer, 3, 1);
			this.tblServerSettings.Controls.Add(this.lblClientId, 0, 1);
			this.tblServerSettings.Controls.Add(this.txtClientId, 1, 1);
			this.tblServerSettings.Controls.Add(this.btnSetClient, 2, 1);
			this.tblServerSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tblServerSettings.Location = new System.Drawing.Point(3, 17);
			this.tblServerSettings.Name = "tblServerSettings";
			this.tblServerSettings.RowCount = 2;
			this.tblServerSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.tblServerSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.tblServerSettings.Size = new System.Drawing.Size(382, 90);
			this.tblServerSettings.TabIndex = 0;
			// 
			// lblServerUrl
			// 
			this.lblServerUrl.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.lblServerUrl.Location = new System.Drawing.Point(3, 6);
			this.lblServerUrl.Name = "lblServerUrl";
			this.lblServerUrl.Size = new System.Drawing.Size(84, 23);
			this.lblServerUrl.TabIndex = 0;
			this.lblServerUrl.Text = "服务器地址：";
			// 
			// txtServerUrl
			// 
			this.txtServerUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.txtServerUrl.Location = new System.Drawing.Point(93, 7);
			this.txtServerUrl.Name = "txtServerUrl";
			this.txtServerUrl.Size = new System.Drawing.Size(74, 21);
			this.txtServerUrl.TabIndex = 1;
			this.txtServerUrl.Text = "127.0.0.1";
			// 
			// lblServerPort
			// 
			this.lblServerPort.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.lblServerPort.Location = new System.Drawing.Point(173, 6);
			this.lblServerPort.Name = "lblServerPort";
			this.lblServerPort.Size = new System.Drawing.Size(84, 23);
			this.lblServerPort.TabIndex = 2;
			this.lblServerPort.Text = "服务器端口：";
			// 
			// txtServerPort
			// 
			this.txtServerPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.txtServerPort.Location = new System.Drawing.Point(263, 7);
			this.txtServerPort.Name = "txtServerPort";
			this.txtServerPort.Size = new System.Drawing.Size(116, 21);
			this.txtServerPort.TabIndex = 3;
			this.txtServerPort.Text = "8920";
			// 
			// btnSetServer
			// 
			this.btnSetServer.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.btnSetServer.Location = new System.Drawing.Point(263, 51);
			this.btnSetServer.Name = "btnSetServer";
			this.btnSetServer.Size = new System.Drawing.Size(75, 23);
			this.btnSetServer.TabIndex = 4;
			this.btnSetServer.Text = "设置服务器";
			// 
			// lblClientId
			// 
			this.lblClientId.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.lblClientId.Location = new System.Drawing.Point(3, 51);
			this.lblClientId.Name = "lblClientId";
			this.lblClientId.Size = new System.Drawing.Size(84, 23);
			this.lblClientId.TabIndex = 5;
			this.lblClientId.Text = "客户端ID：";
			// 
			// txtClientId
			// 
			this.txtClientId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.txtClientId.Location = new System.Drawing.Point(93, 52);
			this.txtClientId.Name = "txtClientId";
			this.txtClientId.Size = new System.Drawing.Size(74, 21);
			this.txtClientId.TabIndex = 6;
			this.txtClientId.Text = "all";
			// 
			// btnSetClient
			// 
			this.btnSetClient.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.btnSetClient.Location = new System.Drawing.Point(173, 51);
			this.btnSetClient.Name = "btnSetClient";
			this.btnSetClient.Size = new System.Drawing.Size(75, 23);
			this.btnSetClient.TabIndex = 7;
			this.btnSetClient.Text = "设置客户端";
			// 
			// MainForm
			// 
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(420, 600);
			this.Controls.Add(this.pnlInject);
			this.Controls.Add(this.pnlOutput);
			this.Controls.Add(this.pnlSettings);
			this.Controls.Add(this.tblBottomNav);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "HookForDGLab";
			this.tblBottomNav.ResumeLayout(false);
			this.pnlInject.ResumeLayout(false);
			this.pnlInjectMargin.ResumeLayout(false);
			this.grpInject.ResumeLayout(false);
			this.tblInject.ResumeLayout(false);
			this.pnlOutput.ResumeLayout(false);
			this.pnlOutputMargin.ResumeLayout(false);
			this.tblOutput.ResumeLayout(false);
			this.tblOutput.PerformLayout();
			this.pnlOutputTop.ResumeLayout(false);
			this.pnlSettings.ResumeLayout(false);
			this.pnlSettingsMargin.ResumeLayout(false);
			this.grpAppSettings.ResumeLayout(false);
			this.tblAppSettings.ResumeLayout(false);
			this.tblAppSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trbOutputMultiplier)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trbSendFrequency)).EndInit();
			this.grpServerSettings.ResumeLayout(false);
			this.tblServerSettings.ResumeLayout(false);
			this.tblServerSettings.PerformLayout();
			this.ResumeLayout(false);

		}

		/// <summary>
		/// 初始化数据绑定和基础事件处理
		/// </summary>
		private void InitializeBinding()
		{
			// 初始化数据绑定
			InitializeComponent();

			// 底部导航切换
			btnNavInject.Click += BtnNavInject_Click;
			btnNavOutput.Click += BtnNavOutput_Click;
			btnNavSettings.Click += BtnNavSettings_Click;

			// 默认显示注入页
			pnlInject.BringToFront();
			btnNavInject.BackColor = System.Drawing.Color.LightSteelBlue;

			// 日志区只读
			txtLog.ReadOnly = true;
			txtLog.Text = "";

			// 加载配置
			LoadConfig();
			ApplyConfigToControls();

			// 应用设置事件绑定
			chkDualFreq.CheckedChanged += ChkDualFreq_CheckedChanged;
			trbOutputMultiplier.ValueChanged += TrbOutputMultiplier_ValueChanged;
			trbSendFrequency.ValueChanged += TrbSendFrequency_ValueChanged;
			txtBaseStrength.TextChanged += TxtBaseStrength_TextChanged;
			txtStrengthLimit.TextChanged += TxtStrengthLimit_TextChanged;
			txtControllerLimit.TextChanged += TxtControllerLimit_TextChanged;

			// 服务器设置按钮事件绑定
			btnSetServer.Click += BtnSetServer_Click;
			btnSetClient.Click += BtnSetClient_Click;

			// 注入按钮事件绑定
			btnRefresh.Click += BtnRefresh_Click;
			btnInject.Click += BtnInject_Click;
			lstProcesses.DoubleClick += LstProcesses_DoubleClick;
		}

		#region 底部导航切换
		private void BtnNavInject_Click(object sender, EventArgs e)
		{
			pnlInject.BringToFront();
			btnNavInject.BackColor = System.Drawing.Color.LightSteelBlue;
			btnNavOutput.BackColor = System.Drawing.SystemColors.Control;
			btnNavSettings.BackColor = System.Drawing.SystemColors.Control;
		}

		private void BtnNavOutput_Click(object sender, EventArgs e)
		{
			pnlOutput.BringToFront();
			btnNavInject.BackColor = System.Drawing.SystemColors.Control;
			btnNavOutput.BackColor = System.Drawing.Color.LightSteelBlue;
			btnNavSettings.BackColor = System.Drawing.SystemColors.Control;
		}

		private void BtnNavSettings_Click(object sender, EventArgs e)
		{
			pnlSettings.BringToFront();
			btnNavInject.BackColor = System.Drawing.SystemColors.Control;
			btnNavOutput.BackColor = System.Drawing.SystemColors.Control;
			btnNavSettings.BackColor = System.Drawing.Color.LightSteelBlue;
		}
		#endregion

		#region 应用设置事件
		private void ChkDualFreq_CheckedChanged(object sender, EventArgs e)
		{
			SaveConfig();
		}

		private void TrbOutputMultiplier_ValueChanged(object sender, EventArgs e)
		{
			lblOutputMultiplierValue.Text = (trbOutputMultiplier.Value / 10.0).ToString("0.0");
			SaveConfig();
		}

		private void TrbSendFrequency_ValueChanged(object sender, EventArgs e)
		{
			lblSendFrequencyValue.Text = trbSendFrequency.Value.ToString();
			SaveConfig();
		}

		private void TxtBaseStrength_TextChanged(object sender, EventArgs e)
		{
			SaveConfig();
		}

		private void TxtStrengthLimit_TextChanged(object sender, EventArgs e)
		{
			SaveConfig();
		}

		private void TxtControllerLimit_TextChanged(object sender, EventArgs e)
		{
			if (int.TryParse(txtControllerLimit.Text, out int val))
			{
				_config.ControllerLimit = val;
				SaveConfig();
			}
		}
		#endregion

		#region 服务器设置事件
		private void BtnSetServer_Click(object sender, EventArgs e)
		{
			string url = txtServerUrl.Text.Trim();
			string port = txtServerPort.Text.Trim();
			if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(port))
			{
				MessageBox.Show("服务器地址和端口不能为空！");
				return;
			}
			CoyoteApi.CoyotreUrl = url + ":" + port;
			SaveConfig();
			MessageBox.Show("服务器地址已设置！");
		}

		private void BtnSetClient_Click(object sender, EventArgs e)
		{
			string clientId = txtClientId.Text.Trim();
			if (string.IsNullOrEmpty(clientId))
			{
				MessageBox.Show("客户端ID不能为空！");
				return;
			}
			CoyoteApi.ClientID = clientId;
			SaveConfig();
			MessageBox.Show("客户端ID已设置！");
		}
		#endregion

		#region 配置文件读写
		private void LoadConfig()
		{
			if (File.Exists(ConfigPath))
			{
				try
				{
					_config = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(ConfigPath));
					if (_config != null)
					{
						CoyoteApi.CoyotreUrl = _config.ServerUrl + ":" + _config.ServerPort;
						CoyoteApi.ClientID = _config.ClientId;
					}
				}
				catch
				{
					_config = new AppConfig();
				}
			}
		}

		private void SaveConfig()
		{
			_config.ServerUrl = txtServerUrl.Text.Trim();
			_config.ServerPort = int.TryParse(txtServerPort.Text, out int port) ? port : 8920;
			_config.ClientId = txtClientId.Text.Trim();
			_config.DualFreq = chkDualFreq.Checked;
			_config.OutputMultiplier = (float)(trbOutputMultiplier.Value / 10.0);
			_config.SendFrequency = trbSendFrequency.Value;
			_config.BaseStrength = int.TryParse(txtBaseStrength.Text, out int baseStr) ? baseStr : 0;
			_config.StrengthLimit = int.TryParse(txtStrengthLimit.Text, out int limit) ? limit : 20;
			_config.ControllerLimit = int.TryParse(txtControllerLimit.Text, out int ctrlLimit) ? ctrlLimit : 65535;

			File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(_config, Formatting.Indented));
		}

		private void ApplyConfigToControls()
		{
			txtServerUrl.Text = _config.ServerUrl;
			txtServerPort.Text = _config.ServerPort.ToString();
			txtClientId.Text = _config.ClientId;
			chkDualFreq.Checked = _config.DualFreq;
			trbOutputMultiplier.Value = (int)(_config.OutputMultiplier * 10);
			lblOutputMultiplierValue.Text = _config.OutputMultiplier.ToString("0.0");
			trbSendFrequency.Value = _config.SendFrequency;
			lblSendFrequencyValue.Text = _config.SendFrequency.ToString();
			txtBaseStrength.Text = _config.BaseStrength.ToString();
			txtStrengthLimit.Text = _config.StrengthLimit.ToString();
			txtControllerLimit.Text = _config.ControllerLimit.ToString();
		}
		#endregion

		#region 配置结构体
		public class AppConfig
		{
			/// <summary>
			/// 服务器地址
			/// </summary>
			public string ServerUrl { get; set; } = "127.0.0.1";
			/// <summary>
			/// 服务器端口
			/// </summary>
			public int ServerPort { get; set; } = 8920;
			/// <summary>
			/// 客户端ID，默认为"all"，表示接收所有客户端数据
			/// </summary>
			public string ClientId { get; set; } = "all";
			/// <summary>
			/// 是否启用双频合一模式
			/// </summary>
			public bool DualFreq { get; set; } = false;
			/// <summary>
			/// 输出倍数，默认为1.0
			/// </summary>
			public float OutputMultiplier { get; set; } = 1.0f;
			/// <summary>
			/// 发送频率，单位为毫秒，默认为50
			/// </summary>
			public int SendFrequency { get; set; } = 50;
			/// <summary>
			/// 基础强度，默认为0
			/// </summary>
			public int BaseStrength { get; set; } = 0;
			/// <summary>
			/// 强度上限，默认为20
			/// </summary>
			public int StrengthLimit { get; set; } = 20;
			/// <summary>
			/// 默认控制器上限，默认为65535（Xbox控制器上限）
			/// </summary>
			public int ControllerLimit { get; set; } = 65535; // 新增
		}
		#endregion
	}
}
