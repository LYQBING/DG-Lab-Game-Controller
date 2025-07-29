using DGLabGameController.Core.Debug;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace GamepadVibrationProcessor.Services
{
	/// <summary>
	/// 进程服务：用于获取当前系统中所有正在运行的进程列表
	/// </summary>
	public static class ProcessService
    {
		/// <summary>
		/// 获取当前系统中所有正在运行的进程列表
		/// </summary>
		public static ObservableCollection<ProcessInfo> GetProcessList()
        {
            var list = new ObservableCollection<ProcessInfo>();
            foreach (var proc in Process.GetProcesses())
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(proc.MainWindowTitle)) continue;
                    string filePath = proc.MainModule?.FileName ?? "";
                    var icon = GetIcon(filePath);
                    string appName = string.IsNullOrWhiteSpace(proc.MainWindowTitle) ? proc.ProcessName : proc.MainWindowTitle;
                    list.Add(new ProcessInfo
                    {
                        Name = appName,
                        Id = proc.Id,
                        FilePath = filePath,
                        Icon = icon
                    });
                }
                catch (Exception ex)
                {
                    DebugHub.Log("进程访问异常", $"无法访问程序: {proc.ProcessName}:{ex.Message}",true);
                }
            }
            return list;
        }

		/// <summary>
		/// 获取指定文件路径的图标
		/// </summary>
		private static BitmapImage? GetIcon(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return null;
            try
            {
                using var icon = Icon.ExtractAssociatedIcon(filePath);
                if (icon != null)
                {
                    using var bmp = icon.ToBitmap();
                    using var stream = new MemoryStream();
                    bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Seek(0, SeekOrigin.Begin);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            catch { }
            return null;
        }
    }
}