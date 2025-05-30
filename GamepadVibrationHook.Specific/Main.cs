using System;
using System.Collections.Generic;
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
		delegate uint XInputSetStateDelegate(uint dwUserIndex, ref XINPUT_VIBRATION pVibration);

		/// <summary>
		/// 结构体声明，原生 API的内容结构
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct XINPUT_VIBRATION
		{
			public ushort wLeftMotorSpeed;
			public ushort wRightMotorSpeed;
		}

		/// <summary>
		/// DLL 模块名列表，包含所有可能的 XInput DLL
		/// </summary>
		private static readonly string[] XInputDlls = { "xinput1_1.dll", "xinput1_2.dll", "xinput1_3.dll", "xinput1_4.dll", "xinput9_1_0.dll", "xinputuap.dll" };

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
		/// EasyHook 的主运行函数，注入后将自动调用
		/// </summary>
		/// <param name="context">EasyHook上下文对象</param>
		/// <param name="channelName">IPC 通道名称</param>
		public void Run(RemoteHooking.IContext context, string channelName)
		{
			// 遍历指定的所有模块，并尝试进行 Hook
			int SuccesHook = 0;
			foreach (var dll in XInputDlls) if (TryHook(dll)) SuccesHook++;

			// 输出注入结果
			if (SuccesHook > 0)
			{
				_interface?.ErrorEvent($"哇！成功注入 {SuccesHook} 项", "游戏世界通讯成功！现在就拿起你的手柄，开始游戏吧！", 1);
			}
			else
			{
				_interface?.ErrorEvent("哦不！注入失败了", "居然没有合适的 DLL 模块项，或许是我太杂鱼了...", 3);
			}

			// 保持注入进程存活，防止被卸载
			while (true) System.Threading.Thread.Sleep(500);
		}

		/// <summary>
		/// 尝试对指定 DLL 的 XInputSetState 函数进行 Hook
		/// </summary>、
		private bool TryHook(string dll)
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
					hook.ThreadACL.SetExclusiveACL(new int[] { 0 });

					_hooks.Add(hook);
					_interface?.ErrorEvent($"注入 {dll} 成功", "已成功将 DLL 模块注入至目标主线程内", 1);
					return true;
				}
				else
				{
					_interface?.ErrorEvent($"注入 {dll} 失败", "被注入的 DLL 模块加载失败 或 注入目标的函数地址无效", 3);
					return false;
				}
			}
			catch (Exception overr)
			{
				_interface?.ErrorEvent($"注入 {dll} 错误", overr.ToString(), 2);
				return false;
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
		private uint XInputSetState_Hooked(string dll, uint dwUserIndex, ref XINPUT_VIBRATION pVibration)
		{
			try
			{
				// 回传震动数据到主程序
				_interface.OnVibrationChanged(pVibration.wLeftMotorSpeed, pVibration.wRightMotorSpeed);
			}
			catch (Exception overr)
			{
				_interface.ErrorEvent($"链接 IPC 错误", overr.ToString(), 3);
			}

			// 调用原始 API，保持原有功能不变
			if (_originalDelegates.TryGetValue(dll, out var orig)) return orig(dwUserIndex, ref pVibration);
			return 0;
		}
	}
}
