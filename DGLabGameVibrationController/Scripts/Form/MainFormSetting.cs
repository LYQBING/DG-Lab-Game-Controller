using System.Drawing;
using System.Windows.Forms;

namespace DGLabGameVibrationController
{
	public partial class MainForm
	{
		// 服务器设置小组件
		private GroupBox grpServer;
		private Label lblServerUrl, lblServerPort, lblClientId;
		private TextBox txtServerUrl, txtServerPort, txtClientId;

		// DG-LAB设置小组件
		private GroupBox grpDGLab;
		private CheckBox chkDualFreq, chkLinearOutput, chkLightweight;
		private Label lblBaseStrength, lblOutputMultiplier, lblControllerLimit;
		private TextBox txtBaseStrength, txtOutputMultiplier, txtControllerLimit;

		// 程序设置小组件
		private GroupBox grpProgram;
		private CheckBox chkVerboseLog, chkLegacyLabel, chkDockTips;

		private bool isLoadingConfig = false;
		private AppConfig config;

		private void InitializeSetting()
		{
			tblSettings.Controls.Add(GrpServer(), 0, 0);
			tblSettings.Controls.Add(GrpDGLab(), 0, 1);
			tblSettings.Controls.Add(GrpProgram(), 0, 2);

			LoadConfig();
		}

		/// <summary>
		/// 加载界面配置
		/// </summary>
		private void LoadConfig()
		{
			isLoadingConfig = true;
			config = ConfigManager.Load();

			txtServerUrl.Text = config.ServerUrl;
			txtServerPort.Text = config.ServerPort.ToString();
			txtClientId.Text = config.ClientId;

			chkDualFreq.Checked = config.DualFreq;
			chkLinearOutput.Checked = config.LinearOutput;
			chkLightweight.Checked = config.EasyMode;
			txtBaseStrength.Text = config.BaseStrength.ToString();
			txtOutputMultiplier.Text = config.OutputMultiplier.ToString();
			txtControllerLimit.Text = config.ControllerLimit.ToString();
			
			chkVerboseLog.Checked = config.VerboseLogs;
			chkLegacyLabel.Checked = config.LegacyLabels;
			chkDockTips.Checked = config.DockTips;

			isLoadingConfig = false;
			InitializeDock();
		}

		#region 设置页面的小组件

		/// <summary>
		/// 创建服务器设置小组件
		/// </summary>
		public GroupBox GrpServer()
		{
			lblServerUrl = CreateBaseLabel("服务器地址：");
			txtServerUrl = CreateBaseTextBox("127.0.0.1");
			lblServerPort = CreateBaseLabel("服务器端口：");
			txtServerPort = CreateBaseTextBox("8920");
			lblClientId = CreateBaseLabel("客户端标识：");
			txtClientId = CreateBaseTextBox("all");

			var tblServer = new TableLayoutPanel
			{
				ColumnCount = 2,
				RowCount = 3,
				Dock = DockStyle.Top,
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowAndShrink
			};
			tblServer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tblServer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tblServer.Controls.Add(lblServerUrl, 0, 0);
			tblServer.Controls.Add(txtServerUrl, 1, 0);
			tblServer.Controls.Add(lblServerPort, 0, 1);
			tblServer.Controls.Add(txtServerPort, 1, 1);
			tblServer.Controls.Add(lblClientId, 0, 2);
			tblServer.Controls.Add(txtClientId, 1, 2);

			grpServer = CreateBaseGroupBox("服务器设置");
			grpServer.Controls.Add(tblServer);
			return grpServer;
		}

		/// <summary>
		/// 创建DG-LAB设置小组件
		/// </summary>
		public GroupBox GrpDGLab()
		{
			chkDualFreq = CreateBaseCheckBox("启用双频合一");
			chkLinearOutput = CreateBaseCheckBox("启用线性输出", true);
			chkLightweight = CreateBaseCheckBox("启用轻量模式");
			lblBaseStrength = CreateBaseLabel("基础输出值：");
			txtBaseStrength = CreateBaseTextBox("10");
			lblOutputMultiplier = CreateBaseLabel("输出的倍率：");
			txtOutputMultiplier = CreateBaseTextBox("1");
			lblControllerLimit = CreateBaseLabel("控制器标值：");
			txtControllerLimit = CreateBaseTextBox("65535");

			var tblDGLab = new TableLayoutPanel
			{
				ColumnCount = 2,
				RowCount = 5,
				Dock = DockStyle.Top,
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowAndShrink
			};
			tblDGLab.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tblDGLab.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tblDGLab.Controls.Add(chkDualFreq, 0, 0);
			tblDGLab.Controls.Add(chkLinearOutput, 1, 0);
			tblDGLab.Controls.Add(chkLightweight, 0, 1);
			tblDGLab.Controls.Add(lblBaseStrength, 0, 2);
			tblDGLab.Controls.Add(txtBaseStrength, 1, 2);
			tblDGLab.Controls.Add(lblOutputMultiplier, 0, 3);
			tblDGLab.Controls.Add(txtOutputMultiplier, 1, 3);
			tblDGLab.Controls.Add(lblControllerLimit, 0, 4);
			tblDGLab.Controls.Add(txtControllerLimit, 1, 4);

			grpDGLab = CreateBaseGroupBox("DG-LAB设置");
			grpDGLab.Controls.Add(tblDGLab);
			return grpDGLab;
		}

		/// <summary>
		/// 创建程序设置小组件
		/// </summary>
		public GroupBox GrpProgram()
		{
			chkVerboseLog = CreateBaseCheckBox("详细日志内容");
			chkLegacyLabel = CreateBaseCheckBox("恢复旧版标签");
			chkDockTips = CreateBaseCheckBox("启用底部标签");

			var tblProgram = new TableLayoutPanel
			{
				ColumnCount = 2,
				RowCount = 2,
				Dock = DockStyle.Top,
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowAndShrink
			};
			tblProgram.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tblProgram.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tblProgram.Controls.Add(chkVerboseLog, 0, 0);
			tblProgram.Controls.Add(chkLegacyLabel, 1, 0);
			tblProgram.Controls.Add(chkDockTips, 0, 1);

			grpProgram = CreateBaseGroupBox("程序设置");
			grpProgram.Controls.Add(tblProgram);
			return grpProgram;
		}

		#endregion

		#region 基础控件创建函数

		public Label CreateBaseLabel(string text = null) => new Label()
		{
			Text = text ?? "",
			ForeColor = AppColor.HighlightColor,
			AutoSize = false,
			Dock = DockStyle.Fill,
			TextAlign = ContentAlignment.MiddleCenter,
		};

		public TextBox CreateBaseTextBox(string text = null)
		{
			TextBox textBox = new TextBox()
			{
				Text = text ?? "",
				Dock = DockStyle.Fill,
				TextAlign = HorizontalAlignment.Center,
				BackColor = AppColor.BackgroundColorLight,
				ForeColor = AppColor.StandardColor,
				BorderStyle = BorderStyle.FixedSingle,
			};

			textBox.Leave += (s, e) => Save();
			return textBox;
		}

		public CheckBox CreateBaseCheckBox(string text = null, bool check = false)
		{
			CheckBox checkBox = new CheckBox()
			{
				Text = text ?? "",
				ForeColor = AppColor.HighlightColor,
				AutoSize = false,
				Checked = check,
				Dock = DockStyle.Fill,
				TextAlign = ContentAlignment.MiddleCenter,
				BackColor = Color.Transparent,
				Anchor = AnchorStyles.None,
				Margin = new Padding(0),
			};

			checkBox.CheckedChanged += (s, e) => Save();
			return checkBox;
		}

		public RoundButton CreateBaseRoundButton(string text = null)
		{
			RoundButton button = new RoundButton()
			{
				Text = text ?? "",
				Dock = DockStyle.Fill,
				FlatStyle = FlatStyle.Flat,
				ForeColor = AppColor.HighlightColor,
				BackColor = AppColor.StandardColor,
			};

			button.Click += (sender, e) => Save();
			button.FlatAppearance.BorderSize = 0;
			return button;
		}

		public GroupBox CreateBaseGroupBox(string text = null) => new GroupBox()
		{
			Text = text ?? "",
			ForeColor = AppColor.HighlightColor,
			BackColor = Color.Transparent,
			Dock = DockStyle.Top,
			Padding = new Padding(8),
			AutoSize = true,
		};

		private void Save()
		{
			if (isLoadingConfig) return;
			AppendLog("数据保存成功","部分功能将在下一次启动时生效。");

			config.ServerUrl = txtServerUrl.Text.Trim();
			config.ServerPort = int.TryParse(txtServerPort.Text, out int port) ? port : 8920;
			config.ClientId = txtClientId.Text.Trim();

			config.DualFreq = chkDualFreq.Checked;
			config.LinearOutput = chkLinearOutput.Checked;
			config.EasyMode = chkLightweight.Checked;
			config.BaseStrength = int.TryParse(txtBaseStrength.Text, out int baseStr) ? baseStr : 0;
			config.OutputMultiplier = float.TryParse(txtOutputMultiplier.Text, out float mul) ? mul : 1.0f;
			config.ControllerLimit = int.TryParse(txtControllerLimit.Text, out int ctrlLimit) ? ctrlLimit : 65535;

			config.VerboseLogs = chkVerboseLog.Checked;
			config.LegacyLabels = chkLegacyLabel.Checked;
			config.DockTips = chkDockTips.Checked;

			ConfigManager.Save();
		}

		#endregion
	}
}