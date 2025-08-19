using DGLabGameController.Core.Debug;
using GameValueDetector.Interop;
using GameValueDetector.Models;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace GameValueDetector.Services
{
	/// <summary>
	/// 内存读取服务
	/// </summary>
	public static class MemoryReader
	{
		/// <summary>
		/// 读取指定进程的内存值
		/// </summary>
		/// <param name="process">目标进程</param>
		/// <param name="monitor">监控项配置</param>
		/// <param name="is32Bit">是否为 32 位进程</param>
		/// <returns>读取到的值</returns>
		public static bool TryReadValue(Process process, MonitorItem monitor, bool is32Bit, out float value, IntPtr processHandle)
		{
			// 获取目标模块
			value = default;
			ProcessModule? module = process.Modules.Cast<ProcessModule>().FirstOrDefault(m => m.ModuleName == monitor.Module);
			if (module == null)
			{
				DebugHub.Error("未找到模块", monitor.Module, true);
				return false;
			}

			// 解析基地址
			if (!long.TryParse(monitor.BaseAddress, System.Globalization.NumberStyles.HexNumber, null, out long baseAddr))
			{
				DebugHub.Error("基地址错误", monitor.BaseAddress, true);
				return false;
			}

			// 解析偏移列表
			List<int> offsets = [];
			try
			{
				offsets = [.. monitor.Offsets.Select(s => int.Parse(s, System.Globalization.NumberStyles.HexNumber))];
			}
			catch
			{
				DebugHub.Error("偏移量错误", $"{string.Join(", ", monitor.Offsets)}", true);
				return false;
			}

			string type = monitor.Type; // 监控项类型
			try
			{
				// 计算最终地址、指针大小和读取内存
				long address = module.BaseAddress.ToInt64() + baseAddr;
				int pointerSize = is32Bit ? 4 : 8;
				byte[] buffer = new byte[pointerSize];

				foreach (int offset in offsets)
				{
					if (!Win32Helper.ReadProcessMemory(processHandle, checked((IntPtr)address), buffer, pointerSize, out _))
					{
						DebugHub.Warning("读取内存失败", $"地址: {address:X}, 偏移: {offset}", true);
						return false;
					}
					address = is32Bit ? BitConverter.ToInt32(buffer, 0) : BitConverter.ToInt64(buffer, 0);
					address += offset;
				}

				// 根据类型读取内存
				int size = type switch { "Int32" => 4, "Float" => 4, "Double" => 8, "Int64" => 8, "Byte" => 1, _ => 4 };
				buffer = new byte[size];

				if (!Win32Helper.ReadProcessMemory(processHandle, checked((IntPtr)address), buffer, size, out _))
				{
					DebugHub.Warning("读取内存失败", $"地址: {address:X}, 类型: {type}", true);
					return false;
				}
				value = type switch
				{
					"Int32" => BitConverter.ToInt32(buffer, 0),
					"Float" => BitConverter.ToSingle(buffer, 0),
					"Double" => (float)BitConverter.ToDouble(buffer, 0),
					"Int64" => BitConverter.ToInt64(buffer, 0),
					"Byte" => buffer[0],
					_ => default
				};
				return true;
			}
			catch (System.Exception ex)
			{
				DebugHub.Warning("读取内存异常", ex.Message, true);
				return false;
			}
		}

		/// <summary>
		/// 打开进程句柄
		/// </summary>
		public static bool OpenProcessHandle(Process process, out IntPtr hProcess)
		{
			DebugHub.Log("尝试打开进程", $"进程 ID: {process.Id}");
			hProcess = Win32Helper.OpenProcess(ProcessAccessFlags.VirtualMemoryRead, false, process.Id);
			if (hProcess == IntPtr.Zero)
			{
				DebugHub.Error("进程启动失败", "似乎是权限不足呢...");
				return false;
			}
			return true;
		}

		/// <summary>
		/// 关闭进程句柄
		/// </summary>
		public static void CloseProcessHandle(IntPtr handle)
		{
			if (handle != IntPtr.Zero) Win32Helper.CloseHandle(handle);
		}
	}
}