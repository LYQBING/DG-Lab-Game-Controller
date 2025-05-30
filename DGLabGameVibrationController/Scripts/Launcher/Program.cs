using System;
using System.Windows.Forms;
using DGLabGameVibrationController;

namespace GamepadVibrationHook
{
	static class Program
	{
		/// <summary>
		/// 应用程序界面的入口点
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}

	/// <summary>
	/// 震动数据传输接口
	/// <para>需与进程通信接口一致：否则无法通信</para>
	/// </summary>
	public class VibrationInterface : MarshalByRefObject
	{
		/// <summary>
		/// 震动数据变化事件
		/// </summary>
		public event Action<int, int> VibrationChanged;
		public event Action<string, string, int> LogEvent;

		/// <summary>
		/// 通知错误事件
		/// </summary>
		/// <param name="code">错误代码</param>
		/// <param name="error">错误内容</param>
		public void ErrorEvent(string code, string error, int level)
		{
			LogEvent?.Invoke(code, error, level);
		}

		/// <summary>
		/// 通知震动数据变化
		/// </summary>
		public void OnVibrationChanged(int leftMotor, int rightMotor)
		{
			VibrationChanged?.Invoke(leftMotor, rightMotor);
		}
	}
}
