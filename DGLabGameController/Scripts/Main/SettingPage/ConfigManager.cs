using lyqbing.DGLAB;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace DGLabGameController
{
    /// <summary>
    /// 配置管理器，用于加载和保存应用程序配置
    /// </summary>
    public static class ConfigManager
    {
        public static readonly string DataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        public static readonly string ServerPath = Path.Combine(DataPath, "CoyoteGameHub");
        private static readonly string AppConfigPath = Path.Combine(DataPath, "config.json");
        private static readonly string ServerConfigPath = Path.Combine(ServerPath, "config.yaml");
        public static AppConfig Current { get; private set; } = new();

        public static AppConfig Load()
        {
            if (File.Exists(AppConfigPath))
            {
                AppConfig? config = null;
                try
                {
                    config = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(AppConfigPath));
                }
                catch (Exception)
                {
                    // ignored
                }

                if (config != null)
                    Current = config;
            }

            CoyoteApi.CoyotreUrl = Current.ServerUrl + ":" + Current.ServerPort + "/";
            CoyoteApi.ClientID = Current.ClientId;
            return Current;
        }

        public static void SaveConfig()
        {
            try
            {
                if (!Directory.Exists(DataPath))
                    Directory.CreateDirectory(DataPath);

                File.WriteAllText(AppConfigPath, JsonConvert.SerializeObject(Current, Formatting.Indented));
            }
            catch (Exception ex)
            {
                DebugHub.Log("客户端配置保存失败", ex.Message);
            }
        }

        public static void SaveServerConfig()
        {
            StringBuilder yaml = new();
            yaml.AppendLine($"port: {Current.ServerPort}");
            yaml.AppendLine($"host: \"{Current.ServerHost}\"");
            yaml.AppendLine($"pulseConfigPath: \"{Current.PulseConfigPath}\"");
            yaml.AppendLine($"openBrowser: {Current.OpenBrowser.ToString().ToLower()}");
            yaml.AppendLine($"allowBroadcastToClients: {Current.AllowBroadcastToClients.ToString().ToLower()}");

            try
            {
                if (!Directory.Exists(ServerPath))
                    Directory.CreateDirectory(ServerPath);

                File.WriteAllText(ServerConfigPath, yaml.ToString());
            }
            catch (Exception ex)
            {
                DebugHub.Log("服务器配置保存失败", ex.Message);
            }
        }
    }
}