using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using lyqbing.DGLAB;
using GamepadVibrationHook;
using System.Drawing;
using EasyHook;

namespace DGLabGameVibrationController
{
	public partial class MainForm : Form
	{
		private float percent;

		public MainForm()
		{
			// 初始化数据绑定
			InitializeComponent();

			// 初始化设置组件
			InitializeSetting();

			// 初始化界面组件
			InitializeDock();

			// 各类事件的绑定
			InitializeEvent();

			// 初始化日志输出
			ClearLog();
		}

		#region 功能相关

		/// <summary>
		/// 初始化事件绑定
		/// </summary>
		private void InitializeEvent()
		{
			VibrationInterface.LogEvent += AppendLog;
			VibrationInterface.VibrationChanged += VibrationChanged;
			btnRefresh.Click += RefreshProcess;
			btnInject.Click += ParseProcess;
			lstProcesses.DoubleClick += ParseProcess;
			btnClearLog.Click += ClearLog;
		}

		/// <summary>
		/// 初始化设置组件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClearLog(object sender = null, EventArgs e = null)
		{
			txtLog.Clear();
			VibrationInterface.Invoke("初始化完成", "本程序将一直保持免费且开源，关注作者项目以表示支持！\r\n欢迎加入官方QQ群聊：928175340");
		}

		/// <summary>
		/// 刷新进程列表
		/// </summary>
		private void RefreshProcess(object sender, EventArgs e)
		{
			var processes = Process.GetProcesses().Where(p => !string.IsNullOrEmpty(p.MainWindowTitle)).OrderBy(p => p.ProcessName).Select(p => $"{p.ProcessName} (PID:{p.Id})").ToList();

			lstProcesses.Items.Clear();
			lstProcesses.Items.AddRange(processes.ToArray());
		}

		/// <summary>
		/// 解析选中的进程 ID 并 注入 DLL
		/// </summary>
		private void ParseProcess(object sender, EventArgs e)
		{
			if (lstProcesses.SelectedItem == null)
			{
				MessageBox.Show("没有选择进程ID", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			string selected = lstProcesses.SelectedItem.ToString();
			int pidStart = selected.LastIndexOf("PID:") + 4;
			int pidEnd = selected.LastIndexOf(")");
			if (pidStart < 4 || pidEnd <= pidStart)
			{
				MessageBox.Show("无法解析进程ID", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (!int.TryParse(selected.Substring(pidStart, pidEnd - pidStart), out int targetPID))
			{
				MessageBox.Show("无法解析进程ID", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			InjectToProcess(targetPID);
		}

		/// <summary>
		/// 向指定进程注入 DLL
		/// </summary>
		private void InjectToProcess(int targetPID)
		{
			VibrationInterface _vibrationInterface = new VibrationInterface();

			string channelName = null;
			try
			{
				EasyHook.RemoteHooking.IpcCreateServer(ref channelName, System.Runtime.Remoting.WellKnownObjectMode.Singleton, _vibrationInterface);
			}
			catch (Exception ex)
			{
				VibrationInterface.Invoke("IPC 通道创建失败", ex.Message, 3);
				MessageBox.Show("IPC 通道创建失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			string GamepadVibrationHook = config.DynamicHook ? @"DLL/GamepadVibrationHook.dll" : @"DLL/GamepadVibrationHook.Specific.dll";

			try
			{
				EasyHook.RemoteHooking.Inject(targetPID, GamepadVibrationHook, GamepadVibrationHook, channelName);
				SetNavSelected(btnNavOutput, tabOutput);
			}
			catch (Exception ex)
			{
				VibrationInterface.Invoke("注入失败", ex.Message, 3);
				MessageBox.Show("注入失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		#endregion

		#region 委托事件相关

		/// <summary>
		/// 震动数据变化事件处理
		/// </summary>
		private void VibrationChanged(int left, int right)
		{
			if (config.EasyMode && percent == 0 && (left + right) <= 0) return;
			percent = 0;

			if (!config.LinearOutput)
			{
				if(Math.Max(left, right) > 0) percent = config.OutputMultiplier;
			}
			else
			{
				percent = config.DualFreq ? (((left + right) * config.OutputMultiplier) / config.ControllerLimit) / 2f
				: (Math.Max(left, right) * config.OutputMultiplier) / config.ControllerLimit;
			}
			percent += config.BaseStrength;

			if (config.VerboseLogs)
			{
				VibrationInterface.Invoke("DG-LAB 输出",$" L: {left} R: {right} DGLab：{(int)percent}");
			}
			DGLab.SetStrength.Set((int)percent);
		}

		/// <summary>
		/// 追加日志到日志文本框
		/// </summary>
		/// <param name="title">标题</param>
		/// <param name="txt">内容</param>
		/// <param name="level">级别：最高 3 级</param>
		private void AppendLog(string title,string txt, int level = 0)
		{
			if (InvokeRequired)
			{
				Invoke(new Action(() => AppendLog($"{title}", txt, level))); return;
			}

			Color color;
			switch (level)
			{
				case 1:
					color = AppColor.SuccessColor;
				break;

				case 2:
					color = AppColor.WarningColor;
				break;

				case 3:
					color = AppColor.ErrorColor;
				break;

				default:
					color = AppColor.HighlightColor;
				break;
			}

			txtLog.SelectionColor = AppColor.HighlightColor;
			txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] ");

			txtLog.SelectionLength = 0;
			txtLog.SelectionColor = color;
			txtLog.AppendText($"[{title}] {txt} \r\n\r\n");
			txtLog.SelectionStart = txtLog.Text.Length;

			txtLog.ScrollToCaret();
		}

		#endregion
	}
}
