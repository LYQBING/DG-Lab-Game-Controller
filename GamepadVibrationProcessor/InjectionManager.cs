using GamepadVibrationProcessor;
using lyqbing.DGLAB;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace DGLabGameController
{
	/// <summary>
	/// 一个内置的简单 DLL 注入器
	/// </summary>
	public static class InjectionManager
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

		private static CancellationTokenSource? _pipeCts;

		private const int PROCESS_CREATE_THREAD = 0x0002;
		private const int PROCESS_QUERY_INFORMATION = 0x0400;
		private const int PROCESS_VM_OPERATION = 0x0008;
		private const int PROCESS_VM_WRITE = 0x0020;
		private const int PROCESS_VM_READ = 0x0010;
		private const uint MEM_COMMIT = 0x1000;
		private const uint MEM_RESERVE = 0x2000;
		private const uint PAGE_READWRITE = 0x04;

		/// <summary>
		/// 注入指定模块至指定线程
		/// </summary>
		public static bool Inject(int pid, string dllPathX86, string dllPathX64, out string error)
		{
			// 尝试打开指定进程
			IntPtr hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, pid);
			if (hProcess == IntPtr.Zero)
			{
				error = $"欸欸欸？主人...无法打开目标进程哦：{Marshal.GetLastWin32Error()}";
				return false;
			}

			try
			{
				bool isWow64;
				if (!IsWow64Process(hProcess, out isWow64))
				{
					error = $"嗯...完全没办法判断目标进程的位数呢";
					return false;
				}

				string dllPath = isWow64 ? dllPathX86 : dllPathX64;
				if (!File.Exists(dllPath))
				{
					error = $"丢...丢失了？！找不到地址为 {dllPath} 的 DLL 文件！是不是主人...您修改了此程序呢？";
					return false;
				}

				byte[] dllBytes = Encoding.Default.GetBytes(dllPath + "\0");
				IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)dllBytes.Length, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
				if (allocMemAddress == IntPtr.Zero)
				{
					error = $"哇！主人...它的权限好像比我要高欸！无法在目标进程分配内存：{Marshal.GetLastWin32Error()}";
					return false;
				}

				if (!WriteProcessMemory(hProcess, allocMemAddress, dllBytes, (uint)dllBytes.Length, out _))
				{
					error = $"哇！主人...它的权限好像比我要高欸！无法写入目标进程内存：{Marshal.GetLastWin32Error()}";
					return false;
				}

				IntPtr hKernel32 = GetModuleHandle("kernel32.dll");
				if (hKernel32 == IntPtr.Zero)
				{
					error = $"找不到...嗯...它在哪里呢？无法获取 kernel32.dll 句柄：{Marshal.GetLastWin32Error()}";
					return false;
				}

				IntPtr loadLibraryAddr = GetProcAddress(hKernel32, "LoadLibraryA");
				if (loadLibraryAddr == IntPtr.Zero)
				{
					error = $"找不到...嗯...它在哪里呢？无法获取 LoadLibraryA 地址：{Marshal.GetLastWin32Error()}";
					return false;
				}

				IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);
				if (hThread == IntPtr.Zero)
				{
					error = $"欸？怎么会这样！无法创建远程线程：{Marshal.GetLastWin32Error()}";
					return false;
				}

				error = $"若长时间没有回执事件，则客户端通讯失败：等待客户端回执中...";
				CloseHandle(hThread);
				return true;
			}
			finally
			{
				CloseHandle(hProcess);
			}
		}

		/// <summary>
		/// 启动通道服务端，开始监听DLL端消息
		/// </summary>
		public static void StartPipeServer()
		{
			if (_pipeCts != null)
			{
				DebugHub.Warning("通讯通道冲突", "通讯通道已经存在！");
				return;
			}

			_pipeCts = new();
			var token = _pipeCts.Token;

			Task.Run(() =>
			{
				while (!token.IsCancellationRequested)
				{
					try
					{
						using NamedPipeServerStream pipe = new
						(
							"XInputVibrationPipe",
							PipeDirection.In,
							NamedPipeServerStream.MaxAllowedServerInstances,
							PipeTransmissionMode.Byte,
							PipeOptions.Asynchronous
						);
						pipe.WaitForConnection();

						while (pipe.IsConnected && !token.IsCancellationRequested)
						{
							int type = pipe.ReadByte();
							if (type == -1) break;

							// 注入结果消息
							if (type == 0 || type == 1)
							{
								byte[] lenBuf = new byte[4];
								int read = pipe.Read(lenBuf, 0, 4);
								if (read != 4)
								{
									DebugHub.Warning("通讯失败", "回执的字节长度异常");
									continue;
								}

								int msgLen = BitConverter.ToInt32(lenBuf, 0);
								var msgBuf = new byte[msgLen];
								read = 0;
								while (read < msgLen)
								{
									int r = pipe.Read(msgBuf, read, msgLen - read);
									if (r <= 0)
									{
										DebugHub.Warning("通讯失败", "读取消息体时发生异常");
										break;
									}
									read += r;
								}
								string msg = Encoding.UTF8.GetString(msgBuf, 0, msgLen);
								bool isSuccess = type == 1;

								if (isSuccess) DebugHub.Success("通讯成功", $"哼哼哼，成功与其建立通讯：{msg}");
								else DebugHub.Error("通讯失败", msg);
							}
							// 震动数据消息
							else if (type == 2)
							{
								byte[] buf = new byte[4];
								int read = pipe.Read(buf, 0, 4);
								if (read != 4) continue;

								float left = BitConverter.ToUInt16(buf, 0);
								float right = BitConverter.ToUInt16(buf, 2);
								float output = Math.Max(left, right) / 65535;
								output = (output * HandleInjection.penaltyValue) + HandleInjection.baseValue;

								DGLab.SetStrength.Set((int)output);

								if (ConfigManager.Current.VerboseLogs)
								{
									DebugHub.Log("震动", $"Left: {left}，Right: {right} || DG-LAB：{output}, PenaltyValue: {HandleInjection.penaltyValue}, baseValue: {HandleInjection.baseValue}");
								}
							}
							else
							{
								DebugHub.Warning("异常的数据", $"嗯？看不懂欸...绝对不会存在这种数据回执吧：{type}");
							}
						}
					}
					catch (Exception ex)
					{
						DebugHub.Error("通道发生异常", ex.Message);
						StopPipeServer();
					}
				}
			}, token);
		}

		/// <summary>
		/// 停止通道服务端，取消监听DLL端消息
		/// </summary>
		public static void StopPipeServer()
		{
			_pipeCts?.Cancel();
			_pipeCts?.Dispose();
			_pipeCts = null;
		}

		/* By.LYQBING */
	}
}