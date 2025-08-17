using System.Diagnostics;
using GameValueDetector.Models;
using GameValueDetector.Interop;
using DGLabGameController.Core.Debug;

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
		public static object? ReadValue(Process process, MonitorItem monitor, bool is32Bit)
		{
			// 获取目标模块
			ProcessModule? module = process.Modules.Cast<ProcessModule>().FirstOrDefault(m => m.ModuleName == monitor.Module);
			if (module == null)
			{
				DebugHub.Error("未找到模块", monitor.Module, true);
				return null;
			}

			// 解析基地址
			if (!long.TryParse(monitor.BaseAddress, System.Globalization.NumberStyles.HexNumber, null, out long baseAddr))
			{
				DebugHub.Error("基地址错误", monitor.BaseAddress, true);
				return null;
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
				return null;
			}

			string type = monitor.Type;
			try
			{
				IntPtr hProcess = Win32Helper.OpenProcess(ProcessAccessFlags.VirtualMemoryRead, false, process.Id);
				if (hProcess == IntPtr.Zero)
				{
					DebugHub.Warning("无法打开进程", "主人...似乎是没有权限呢", true);
					return null;
				}

				long address = module.BaseAddress.ToInt64() + baseAddr;
				int pointerSize = is32Bit ? 4 : 8;
				byte[] buffer = new byte[pointerSize];
				foreach (var offset in offsets)
				{
					if (!Win32Helper.ReadProcessMemory(hProcess, checked((IntPtr)address), buffer, pointerSize, out _))
					{
						Win32Helper.CloseHandle(hProcess);
						DebugHub.Warning("读取内存失败", $"地址: {address:X}, 偏移: {offset}", true);
						return null;
					}
					address = is32Bit
						? BitConverter.ToInt32(buffer, 0)
						: BitConverter.ToInt64(buffer, 0);
					address += offset;
				}

				int size = type switch
				{
					"Int32" => 4,
					"Float" => 4,
					"Double" => 8,
					"Int64" => 8,
					"Byte" => 1,
					"String" => 32,
					_ => 4
				};
				buffer = new byte[size];
				if (!Win32Helper.ReadProcessMemory(hProcess, checked((IntPtr)address), buffer, size, out _))
				{
					Win32Helper.CloseHandle(hProcess);
					DebugHub.Warning("读取内存失败", $"地址: {address:X}, 类型: {type}", true);
					return null;
				}
				object? value = type switch
				{
					"Int32" => BitConverter.ToInt32(buffer, 0),
					"Float" => BitConverter.ToSingle(buffer, 0),
					"Double" => BitConverter.ToDouble(buffer, 0),
					"Int64" => BitConverter.ToInt64(buffer, 0),
					"Byte" => buffer[0],
					"String" => System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0'),
					_ => null
				};
				Win32Helper.CloseHandle(hProcess);
				return value;
			}
			catch (System.Exception ex)
			{
				DebugHub.Warning("读取内存异常", ex.Message, true);
				return null;
			}
		}
	}
}