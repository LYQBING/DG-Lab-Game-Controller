using DGLabGameController.Core.Debug;
using DGLabGameController.Core.DGLabApi;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace DGLabGameController.Core.Config
{
	/// <summary>
	/// 配置管理器
	/// <para>负责加载和保存应用程序配置</para>
	/// <para>除非有特殊需求，否则你不应该调用此类中的任何函数</para>
	/// </summary>
	public static class SettingsRepository
	{
		public static SettingsConfig Current { get; private set; } = new();

		#region 读取配置

		/// <summary>
		/// 加载应用程序配置
		/// </summary>
		public static SettingsConfig Load()
		{
			DebugHub.Clear();
			if (File.Exists(AppConfig.ConfigPath))
			{
				SettingsConfig? config = null;
				try
				{
					config = JsonConvert.DeserializeObject<SettingsConfig>(File.ReadAllText(AppConfig.ConfigPath));
				}
				catch (Exception)
				{
					DebugHub.Warning("配置加载失败", "客户端配置文件错误或已损坏，将使用默认配置。");
				}
				if (config != null) Current = config;
			}

			CoyoteApi.CoyotreUrl = Current.ServerUrl + ":" + Current.ServerPort + "/";
			CoyoteApi.ClientID = Current.ClientId;
			return Current;
		}

		#endregion

		#region 保存配置

		/// <summary>
		/// 保存应用程序配置
		/// </summary>
		public static void SaveConfig()
		{
			try
			{
				if (!Directory.Exists(AppConfig.DataPath)) Directory.CreateDirectory(AppConfig.DataPath);
				File.WriteAllText(AppConfig.ConfigPath, JsonConvert.SerializeObject(Current, Formatting.Indented));
			}
			catch (Exception ex)
			{
				DebugHub.Log("客户端配置保存失败", ex.Message);
			}
		}

		/// <summary>
		/// 保存服务器配置
		/// </summary>
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
				if (!Directory.Exists(AppConfig.ServerPath)) Directory.CreateDirectory(AppConfig.ServerPath);
				File.WriteAllText(AppConfig.ServerConfigPath, yaml.ToString());
			}
			catch (Exception ex)
			{
				DebugHub.Log("服务器配置保存失败", ex.Message);
			}
		}

		/// <summary>
		/// 保存所有配置
		/// </summary>
		public static void SaveAll()
		{
			SaveConfig();
			SaveServerConfig();
		}

		#endregion
	}
}