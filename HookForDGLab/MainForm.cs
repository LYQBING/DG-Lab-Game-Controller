using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using lyqbing.DGLAB;
using GamepadVibrationHook;

namespace DGLabGameVibrationController
{
	public partial class MainForm : Form
	{
		private const string ConfigPath = @"config.json";
		private AppConfig _config = new AppConfig();
		private VibrationInterface _vibrationInterface;
		public MainForm() => InitializeBinding();

		#region 注入区事件

		private void BtnRefresh_Click(object sender, EventArgs e)
		{
			var processes = Process.GetProcesses().Where(p => !string.IsNullOrEmpty(p.MainWindowTitle)).OrderBy(p => p.ProcessName).Select(p => $"{p.ProcessName} (PID:{p.Id})").ToList();

			lstProcesses.Items.Clear();
			lstProcesses.Items.AddRange(processes.ToArray());
		}

		private void BtnInject_Click(object sender, EventArgs e)
		{
			if (lstProcesses.SelectedItem == null)
			{
				MessageBox.Show("请先选择要注入的进程！");
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

		private void LstProcesses_DoubleClick(object sender, EventArgs e)
		{
			BtnInject_Click(sender, e);
		}

		private void InjectToProcess(int targetPID)
		{
			_vibrationInterface = new VibrationInterface();
			_vibrationInterface.LogEvent += (code, error) => AppendLog($"错误[{code}]: {error}");
			_vibrationInterface.VibrationChanged += (left, right) =>
			{
				float percent = _config.DualFreq
					? (((left + right) * _config.OutputMultiplier) / _config.ControllerLimit * _config.StrengthLimit) / 2f + _config.BaseStrength
					: ((Math.Max(left, right) * _config.OutputMultiplier) / _config.ControllerLimit * _config.StrengthLimit) + _config.BaseStrength;

				AppendLog($"输出[DGLub]: L: {left} R: {right} DGLab：{percent}");
				DGLab.SetStrength.Set((int)percent);
			};

			string channelName = null;
			try
			{
				EasyHook.RemoteHooking.IpcCreateServer(ref channelName, System.Runtime.Remoting.WellKnownObjectMode.Singleton, _vibrationInterface);
			}
			catch (Exception ex)
			{
				AppendLog("IPC 通道创建失败: " + ex.Message);
				MessageBox.Show("IPC 通道创建失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			string injectionLibrary = @"GamepadVibrationHook.dll";
			try
			{
				EasyHook.RemoteHooking.Inject(targetPID, injectionLibrary, injectionLibrary, channelName);
				AppendLog("注入成功！");

				applog.Text = $"程序通讯状态: 成功";
				MessageBox.Show("注入成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				AppendLog("注入失败: " + ex.Message);
				MessageBox.Show("注入失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		public void AppendLog(string msg)
		{
			if (txtLog.InvokeRequired)
			{
				txtLog.Invoke(new Action(() => AppendLog(msg)));
				return;
			}
			txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\r\n");
			txtLog.SelectionStart = txtLog.Text.Length;
			txtLog.ScrollToCaret();
		}
	}
}
