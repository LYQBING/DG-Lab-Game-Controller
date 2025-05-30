using System.Drawing;
using System.Windows.Forms;

namespace DGLabGameVibrationController
{
	public partial class MainForm
	{
		// Dock 组件
		private TableLayoutPanel tblBottomNav;
		private Panel pnlNavDivider;
		private RoundButton btnNavInject;
		private RoundButton btnNavOutput;
		private RoundButton btnNavSettings;

		// 导航按钮图片资源
		private readonly Image imgInjectDark = Properties.Resources.ModA;
		private readonly Image imgInjectLight = Properties.Resources.ModB;
		private readonly Image imgOutputDark = Properties.Resources.LogA;
		private readonly Image imgOutputLight = Properties.Resources.LogB;
		private readonly Image imgSettingsDark = Properties.Resources.SetA;
		private readonly Image imgSettingsLight = Properties.Resources.SetB;

		private void InitializeDock()
		{
			if (config.LegacyLabels) return;

			// 隐藏 TabControl 边框
			tabMain.Appearance = TabAppearance.FlatButtons;
			tabMain.ItemSize = new Size(0, 1);
			tabMain.SizeMode = TabSizeMode.Fixed;
			tabMain.TabStop = false;

			// 创建导航栏分隔线
			pnlNavDivider = new Panel
			{
				BackColor = AppColor.BackgroundColorLight,
				Dock = DockStyle.Bottom,
				Height = 1
			};

			// 创建底部导航栏
			tblBottomNav = new TableLayoutPanel
			{
				BackColor = AppColor.BackgroundColor,
				ColumnCount = 3
			};

			// 设置列样式，为底部状态栏创建三等分布局
			tblBottomNav.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
			tblBottomNav.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
			tblBottomNav.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
			tblBottomNav.Dock = DockStyle.Bottom;
			tblBottomNav.Height = 64;

			// 创建导航按钮
			btnNavInject = CreateNavButton("注入", null);
			btnNavOutput = CreateNavButton("输出", null);
			btnNavSettings = CreateNavButton("设置", null);

			// 将按钮添加到底部导航栏
			tblBottomNav.Controls.Add(btnNavInject, 0, 0);
			tblBottomNav.Controls.Add(btnNavOutput, 1, 0);
			tblBottomNav.Controls.Add(btnNavSettings, 2, 0);

			// 添加到窗体
			Controls.Add(pnlNavDivider);
			Controls.Add(tblBottomNav);

			// 底部导航切换
			btnNavInject.Click += (s, e) => SetNavSelected(btnNavInject, tabInject);
			btnNavOutput.Click += (s, e) => SetNavSelected(btnNavOutput, tabOutput);
			btnNavSettings.Click += (s, e) => SetNavSelected(btnNavSettings, tabSettings);

			// 切换到默认页面
			SetNavSelected(btnNavInject, tabInject);
		}

		/// <summary>
		/// 创建底部导航按钮（图片在上，文字在下）
		/// </summary>
		private RoundButton CreateNavButton(string text, Image imgDark)
		{
			var btn = new RoundButton
			{
				AutoSize = false,
				Image = imgDark,
				Dock = DockStyle.Fill,
				FlatStyle = FlatStyle.Flat
			};

			if (config.DockTips)
			{
				btn.Text = text;
				btn.ImageAlign = ContentAlignment.TopCenter;
				btn.TextAlign = ContentAlignment.BottomCenter;
				btn.Font = new Font("微软雅黑", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
			}
			else btn.ImageAlign = ContentAlignment.MiddleCenter;

			btn.FlatAppearance.BorderSize = 0;
			btn.BackColor = AppColor.BackgroundColor;
			btn.ForeColor = AppColor.AuxiliaryColor;
			btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
			btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
			btn.Margin = new Padding(0);

			return btn;
		}

		/// <summary>
		/// 设置导航按钮的选中状态，并切换到对应的 TabPage
		/// </summary>
		private void SetNavSelected(Button selected, TabPage tabPage)
		{
			// 将页面切换到指定页面
			tabMain.SelectedTab = tabPage;

			// 注入按钮状态
			if (selected == btnNavInject)
			{
				btnNavInject.Image = imgInjectLight;
				btnNavInject.ForeColor = AppColor.StandardColor;
			}
			else
			{
				btnNavInject.Image = imgInjectDark;
				btnNavInject.ForeColor = AppColor.AuxiliaryColor;
			}

			// 输出按钮状态
			if (selected == btnNavOutput)
			{
				btnNavOutput.Image = imgOutputLight;
				btnNavOutput.ForeColor = AppColor.StandardColor;
			}
			else
			{
				btnNavOutput.Image = imgOutputDark;
				btnNavOutput.ForeColor = AppColor.AuxiliaryColor;
			}

			// 设置按钮状态
			if (selected == btnNavSettings)
			{
				btnNavSettings.Image = imgSettingsLight;
				btnNavSettings.ForeColor = AppColor.StandardColor;
			}
			else
			{
				btnNavSettings.Image = imgSettingsDark;
				btnNavSettings.ForeColor = AppColor.AuxiliaryColor;
			}
		}
	}
}
