using System.Diagnostics;
using GameValueDetector.Models;
using GameValueDetector.Interop;
using DGLabGameController.Core.Debug;
using System.Runtime.InteropServices;

namespace GameValueDetector.Services
{
	/// <summary>
	/// 内存读取服务
	/// </summary>
	public static class MemoryReader
	{
		/// <summary>
		/// 判断目标进程是否为64位进程
		/// </summary>
		/// <param name="process">目标进程</param>
		/// <returns>如果是64位进程返回 true，否则返回 false</returns>
		public static bool IsProcess64Bit(Process process)
		{
			// 如果操作系统本身是32位，则所有进程都是32位
			if (!Environment.Is64BitOperatingSystem) return false;
			// 调用 Win32 API 判断进程是否为 WOW64（即32位进程运行在64位系统上）
			NativeMethods.IsWow64Process(process.Handle, out bool isWow64);
			// 不是 WOW64 即为 64 位进程
			return !isWow64;
		}

		/// <summary>
		/// Win32 API 声明，用于判断进程位数
		/// </summary>
		private static class NativeMethods
		{
			/// <summary>
			/// 判断指定进程是否为 WOW64（即32位进程运行在64位系统上）
			/// </summary>
			/// <param name="hProcess">进程句柄</param>
			/// <param name="wow64Process">返回是否为 WOW64</param>
			/// <returns>成功返回 true</returns>
			[DllImport("kernel32.dll", SetLastError = true)]
			public static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);
		}

		/// <summary>
		/// 读取指定进程的内存值
		/// </summary>
		/// <param name="process">目标进程</param>
		/// <param name="monitor">监控项配置</param>
		/// <param name="hProcess">已打开的进程句柄</param>
		/// <param name="is64Bit">进程位数</param>
		/// <returns>读取到的值，类型由 monitor.Type 决定</returns>
		public static object? ReadValue(Process process, MonitorItem monitor, IntPtr hProcess, bool is64Bit)
		{
			// 获取目标模块（如 engine.dll）
			ProcessModule? module = process.Modules.Cast<ProcessModule>().FirstOrDefault(m => m.ModuleName == monitor.Module);
			if (module == null)
			{
				DebugHub.Error("未找到模块", monitor.Module, true);
				return null;
			}

			// 解析基地址（十六进制字符串转 long）
			if (!long.TryParse(monitor.BaseAddress, System.Globalization.NumberStyles.HexNumber, null, out long baseAddr))
			{
				DebugHub.Error("基地址错误", monitor.BaseAddress, true);
				return null;
			}

			// 解析偏移列表（十六进制字符串转 int）
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
				// 计算初始地址：模块基址 + 配置基地址
				long address = module.BaseAddress.ToInt64() + baseAddr;

				// 多级指针寻址
				foreach (var offset in offsets)
				{
					// 根据进程位数，读取指针（32位读4字节，64位读8字节）
					byte[] buffer = new byte[is64Bit ? 8 : 4];
					if (!Win32Helper.ReadProcessMemory(hProcess, checked((IntPtr)address), buffer, buffer.Length, out _))
					{
						DebugHub.Warning("读取内存失败", $"地址: {address:X}, 偏移: {offset}", true);
						return null;
					}
					// 解析指针值，并加上偏移
					address = is64Bit
						? BitConverter.ToInt64(buffer, 0)
						: BitConverter.ToInt32(buffer, 0);
					address += offset;
				}

				// 根据类型决定读取字节数
				int size = type switch
				{
					"Int32" => 4,
					"Float" => 4,
					"Double" => 8,
					"Int64" => 8,
					"Byte" => 1,
					"String" => 32, // 默认字符串长度，可根据实际需求调整
					_ => 4
				};

				// 读取最终数据
				byte[] valueBuffer = new byte[size];
				if (!Win32Helper.ReadProcessMemory(hProcess, checked((IntPtr)address), valueBuffer, size, out _))
				{
					DebugHub.Warning("读取内存失败", $"地址: {address:X}, 类型: {type}", true);
					return null;
				}

				// 按类型解析数据
				object? value = type switch
				{
					"Int32" => BitConverter.ToInt32(valueBuffer, 0),
					"Float" => BitConverter.ToSingle(valueBuffer, 0),
					"Double" => BitConverter.ToDouble(valueBuffer, 0),
					"Int64" => BitConverter.ToInt64(valueBuffer, 0),
					"Byte" => valueBuffer[0],
					"String" => System.Text.Encoding.UTF8.GetString(valueBuffer).TrimEnd('\0'),
					_ => null
				};

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