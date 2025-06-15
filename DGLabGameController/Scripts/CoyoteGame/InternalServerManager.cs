using System.Diagnostics;
using System.IO;

namespace DGLabGameController
{
    public static class InternalServerManager
    {
        private static Process? _serverProcess;
        private static readonly object Lock = new();
        private const string ProcessName = "coyote-game-hub-server";

        public static void StartServer()
        {
            var config = ConfigManager.Current;
            if (!config.ServerMode)
                return;

            lock (Lock)
            {
                if (IsServerRunning())
                {
                    DebugHub.Warning("服务器存在", "嗯...是重复的请求呢。服务器正处于运行状态：创建请求驳回！");
                    return;
                }

                if (Process.GetProcessesByName(ProcessName).Length > 0)
                {
                    DebugHub.Warning("服务器异常", "欸？服务器已在运行了哦！非主线程服务器可能存在兼容性问题，若需手动创建或链接外部服务器：请前往 “设置” 关闭 “内部服务器”");
                    return;
                }

                try
                {
                    string exePath = Path.Combine(ConfigManager.ServerPath, $"{ProcessName}.exe");
                    string? workingDir = Path.GetDirectoryName(exePath);
                    if (!File.Exists(exePath))
                    {
                        DebugHub.Warning("服务器启动失败", $"服务器执行文件不存在：{exePath}");
                        return;
                    }

                    _serverProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = exePath, // 进程名称
                            WorkingDirectory = workingDir ?? string.Empty, // 进程的工作目录
                            UseShellExecute = false, // 是否使用操作系统的外壳启动
                            CreateNoWindow = !config.DisplayPowerShell // 是否隐藏界面
                        },
                        EnableRaisingEvents = true
                    };

                    _serverProcess.Exited += OnServerProcessExited;
                    _serverProcess.Start();
                }
                catch (Exception ex)
                {
                    _serverProcess?.Dispose();
                    _serverProcess = null;
                    DebugHub.Error("服务器启动失败", $"对不起...主人...我好像搞砸了...异常：{ex.Message}");
                    return;
                }
            }

            DebugHub.Success("服务器启动成功", "主人...要开始了吗？");
        }

        public static void StopServer()
        {
            Process? processToStop;
            lock (Lock)
            {
                if (_serverProcess == null)
                {
                    DebugHub.Success("服务器关闭成功", "嗯...主人是要到此为止了吗？");
                    return;
                }

                processToStop = _serverProcess;
                _serverProcess = null;
            }

            try
            {
                processToStop.Exited -= OnServerProcessExited;

                if (!processToStop.HasExited)
                    processToStop.Kill();
            }
            catch (Exception ex)
            {
                DebugHub.Error("关闭服务器失败", $"对不起...主人...我好像搞砸了...异常：{ex.Message}");
                return;
            }
            finally
            {
                processToStop.Dispose();
            }

            DebugHub.Success("服务器关闭成功", "嗯...主人是要到此为止了吗？");
        }

        private static bool IsServerRunning()
        {
            if (_serverProcess == null)
                return false;

            try
            {
                return !_serverProcess.HasExited;
            }
            catch (Exception)
            {
                _serverProcess = null;
                return false;
            }
        }

        public static void OnServerProcessExited(object? sender, EventArgs e)
        {
            lock (Lock)
            {
                _serverProcess?.Dispose();
                _serverProcess = null;
            }
        }
    }
}