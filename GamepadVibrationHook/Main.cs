using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EasyHook;

namespace GamepadVibrationHook
{
	/// <summary>
	/// 震动数据传输接口
	/// </summary>
	public class VibrationInterface : MarshalByRefObject
	{
		public void ErrorEvent(string code, string error, int level) { }
		public void OnVibrationChanged(int leftMotor, int rightMotor) { }
	}

	public class Main : IEntryPoint
	{
		#region 定义

		/// <summary>
		/// 保存原始 API 指针和创建 Hook
		/// </summary>
		[UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
		private delegate uint XInputSetStateDelegate(uint dwUserIndex, ref XINPUT_VIBRATION pVibration);

		/// <summary>
		/// 结构体声明，原生 API的内容结构
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct XINPUT_VIBRATION
		{
			public ushort wLeftMotorSpeed;
			public ushort wRightMotorSpeed;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		/// <summary>
		/// IPC 通信接口对象，注入端通过它向主程序回传数据
		/// </summary>
		private readonly VibrationInterface _interface;

		/// <summary>
		/// 保存所有已创建的 Hook 对象，便于后续管理和释放
		/// </summary>
		private readonly List<LocalHook> _hooks = new List<LocalHook>();

		/// <summary>
		/// 保存每个 DLL 的原始 XInputSetState 委托，便于在 Hook 回调中调用原始 API
		/// </summary>
		private readonly Dictionary<string, XInputSetStateDelegate> _originalDelegates = new Dictionary<string, XInputSetStateDelegate>();

		#endregion

		public Main(RemoteHooking.IContext context, string channelName)
		{
			_interface = RemoteHooking.IpcConnectClient<VibrationInterface>(channelName);
		}

		/// <summary>
		/// EasyHook 主运行函数，注入后自动调用
		/// </summary>
		public void Run(RemoteHooking.IContext context, string channelName)
		{
			// 遍历当前进程的所有模块，寻找包含 XInputSetState 的 DLL
			int successHook = 0;
			foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
			{
				// 获取指定模块的句柄，如果模块无效就跳过
				var hModule = GetModuleHandle(module.ModuleName);
				if (hModule == IntPtr.Zero) continue;

				// 尝试从模块中获取 XInputSetState 函数的地址，如果获取失败就跳过
				var procAddress = GetProcAddress(hModule, "XInputSetState");
				if (procAddress == IntPtr.Zero) continue;

				// 尝试对该模块进行 Hook
				if (TryHook(module.ModuleName, procAddress)) successHook++;
			}

			// 输出注入结果
			if (successHook > 0)
			{
				_interface?.ErrorEvent($"哇！成功注入 {successHook} 项", "游戏世界通讯成功！现在就拿起你的手柄，开始游戏吧！", 1);
			}
			else
			{
				_interface?.ErrorEvent("哦不！注入失败了", "没有找到任何包含 XInputSetState 的 DLL 模块...", 3);
			}

			// 保持注入进程存活，防止被卸载
			while (true) System.Threading.Thread.Sleep(500);
		}

		/// <summary>
		/// 尝试对指定 DLL 的 XInputSetState 进行 Hook
		/// </summary>
		private bool TryHook(string dll, IntPtr procAddress)
		{
			try
			{
				// 获取原始API的委托，便于后续调用
				XInputSetStateDelegate orig = Marshal.GetDelegateForFunctionPointer(procAddress, typeof(XInputSetStateDelegate)) as XInputSetStateDelegate;
				_originalDelegates[dll] = orig;

				// 创建 Hook
				XInputSetStateDelegate hookDelegate = (uint dwUserIndex, ref XINPUT_VIBRATION pVibration) =>
				XInputSetState_Hooked(dll, dwUserIndex, ref pVibration);

				var hook = LocalHook.Create(procAddress, hookDelegate, this);
				hook.ThreadACL.SetExclusiveACL(new[] { 0 });

				_hooks.Add(hook);
				_interface?.ErrorEvent($"注入 {dll} 成功", "已成功将 DLL 模块注入至目标主线程内", 1);
				return true;
			}
			catch (Exception ex)
			{
				_interface?.ErrorEvent($"注入 {dll} 错误", ex.ToString(), 2);
				return false;
			}
		}

		/// <summary>
		/// XInputSetState Hook 回调，拦截震动数据并调用原始 API
		/// </summary>
		private uint XInputSetState_Hooked(string dll, uint dwUserIndex, ref XINPUT_VIBRATION pVibration)
		{
			try
			{
				_interface.OnVibrationChanged(pVibration.wLeftMotorSpeed, pVibration.wRightMotorSpeed);
			}
			catch (Exception ex)
			{
				_interface.ErrorEvent("链接 IPC 错误", ex.ToString(), 3);
			}

			if (_originalDelegates.TryGetValue(dll, out var orig))
				return orig(dwUserIndex, ref pVibration);

			return 0;
		}
	}
}
