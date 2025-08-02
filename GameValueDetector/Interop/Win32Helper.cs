using System.Runtime.InteropServices;

namespace GameValueDetector.Interop
{
	/// <summary>
	/// 进程访问标志
	/// </summary>
	[Flags]
	public enum ProcessAccessFlags : uint
	{
		/// <summary> 虚拟内存读取权限 </summary>
		VirtualMemoryRead = 0x0010
	}

	/// <summary>
	/// Win32 API 调用帮助类
	/// </summary>
	public static class Win32Helper
	{
		/// <summary>
		/// 打开一个进程的句柄
		/// </summary>
		/// <param name="dwDesiredAccess">指定所需的访问权限</param>
		/// <param name="bInheritHandle">是否需要继承句柄</param>
		/// <param name="dwProcessId">打开的进程的 ID</param>
		/// <returns>指向指定进程的句柄</returns>
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

		/// <summary>
		/// 读取进程内存
		/// </summary>
		/// <param name="hProcess">进程句柄</param>
		/// <param name="lpBaseAddress">基地址</param>
		/// <param name="lpBuffer">输出缓冲区</param>
		/// <param name="dwSize">要读取的字节数</param>
		/// <param name="lpNumberOfBytesRead">实际读取的字节数</param>
		/// <returns>函数是否成功：可通过 GetLastError 函数来获取更多错误信息</returns>
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

		/// <summary>
		/// 关闭一个句柄
		/// </summary>
		/// <param name="hObject">目前已打开的对象</param>
		/// <returns>函数是否成功：可通过 GetLastError 函数来获取更多错误信息</returns>
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(IntPtr hObject);
	}
}