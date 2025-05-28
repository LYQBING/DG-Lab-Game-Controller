using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EasyHook;

namespace GamepadVibrationHook
{
	/// <summary>
	/// 数据传输接口
	/// 必须与主程序端的完全一致，否则 IPC 通信将失败
	/// </summary>
	public class VibrationInterface : MarshalByRefObject
	{
		/// <summary>
		/// 执行错误事件
		/// </summary>
		/// <param name="code">错误代码</param>
		/// <param name="error">错误内容</param>
		public void ErrorEvent(string code, Exception error) { }

		/// <summary>
		/// 震动数据变化事件
		/// </summary>
		public void OnVibrationChanged(int leftMotor, int rightMotor) { }
	}

	/// <summary>
	/// EasyHook 注入 DLL 的主入口点
	/// </summary>
	public class Main : IEntryPoint
	{
		/// <summary>
		/// 可能存在的的 XInput DLL 名称
		/// </summary>
		private static readonly string[] XInputDlls = {"xinput1_3.dll","xinput1_4.dll","xinput9_1_0.dll","xinput1_2.dll"};

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

		/// <summary>
		/// 构造函数，建立与主程序的 IPC 通信
		/// </summary>
		/// <param name="context">EasyHook 上下文对象</param>
		/// <param name="channelName">IPC 通道名称，主程序创建时传入</param>
		public Main(RemoteHooking.IContext context, string channelName)
		{
			_interface = RemoteHooking.IpcConnectClient<VibrationInterface>(channelName);
		}

		/// <summary>
		/// XInputSetState 函数的委托声明
		/// 用于保存原始 API 指针和创建 Hook
		/// </summary>
		[UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
		delegate uint XInputSetStateDelegate(uint dwUserIndex, ref XINPUT_VIBRATION pVibration);

		/// <summary>
        /// 结构体声明，必须与原生 API 一致
        /// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct XINPUT_VIBRATION
		{
			public ushort wLeftMotorSpeed;
			public ushort wRightMotorSpeed;
		}

		/// <summary>
		/// EasyHook 的主运行函数，注入后将自动调用
		/// </summary>
		/// <param name="context">EasyHook上下文对象</param>
		/// <param name="channelName">IPC 通道名称</param>
		public void Run(RemoteHooking.IContext context, string channelName)
		{
			// 遍历所有 DLL 并进行 Hook
			foreach (var dll in XInputDlls) TryHook(dll);

			// 保持注入进程存活，防止被卸载
			while (true) System.Threading.Thread.Sleep(500);
		}

		/// <summary>
		/// 尝试对指定 DLL 的 XInputSetState 函数进行 Hook
		/// </summary>
		/// <param name="dll">DLL文件名</param>
		private void TryHook(string dll)
		{
			try
			{
				// 获取目标 DLL 中 XInputSetState 函数的地址
				IntPtr procAddress = LocalHook.GetProcAddress(dll, "XInputSetState");
				bool loaded = IsModuleLoaded(dll);

				// 仅在 DLL 已加载且函数地址有效时才进行 Hook
				if (procAddress != IntPtr.Zero && loaded)
				{
					// 获取原始API的委托，便于后续调用
					XInputSetStateDelegate orig = Marshal.GetDelegateForFunctionPointer(procAddress, typeof(XInputSetStateDelegate)) as XInputSetStateDelegate;
					_originalDelegates[dll] = orig;

					// 创建 Hook
					XInputSetStateDelegate hookDelegate = (uint dwUserIndex, ref XINPUT_VIBRATION pVibration) =>
					XInputSetState_Hooked(dll, dwUserIndex, ref pVibration);

					var hook = LocalHook.Create(procAddress, hookDelegate, this);

					// 只拦截主线程
					hook.ThreadACL.SetExclusiveACL(new int[] { 0 });
					_hooks.Add(hook);
				}
			}
			catch (Exception overr)
			{
				_interface?.ErrorEvent($"注入 {dll} 错误", overr);
			}
		}

		/// <summary>
		/// 判断指定 DLL 模块是否已加载到当前进程
		/// </summary>
		/// <param name="dllName">DLL 文件名</param>
		/// <returns>已加载返回true，否则false</returns>
		private bool IsModuleLoaded(string dllName)
		{
			foreach (System.Diagnostics.ProcessModule module in System.Diagnostics.Process.GetCurrentProcess().Modules)
			{
				if (string.Equals(module.ModuleName, dllName, StringComparison.OrdinalIgnoreCase)) return true;
			}
			return false;
		}

		/// <summary>
		/// 通用的 XInputSetState Hook 回调
		/// 拦截所有 XInputSetState 调用，回传震动数据并调用原始 API
		/// </summary>
		/// <param name="dll">当前 Hook 的 DLL 名称</param>
		/// <param name="dwUserIndex">手柄用户索引</param>
		/// <param name="pVibration">震动参数结构体</param>
		/// <returns>原始 API 的返回值</returns>
		private uint XInputSetState_Hooked(string dll, uint dwUserIndex, ref XINPUT_VIBRATION pVibration)
		{
			try
			{
				// 回传震动数据到主程序
				_interface.OnVibrationChanged(pVibration.wLeftMotorSpeed, pVibration.wRightMotorSpeed);
			}
			catch (Exception overr)
			{
				_interface.ErrorEvent($"链接 IPC 错误", overr);
			}

			// 调用原始 API，保持原有功能不变
			if (_originalDelegates.TryGetValue(dll, out var orig)) return orig(dwUserIndex, ref pVibration);
			return 0;
		}
	}
}
