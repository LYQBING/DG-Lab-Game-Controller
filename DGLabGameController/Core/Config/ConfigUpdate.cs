using DGLabGameController.Core.Debug;
using DGLabGameController.Core.Network;
using System.Diagnostics;

namespace DGLabGameController.Core.Config
{
	public static class ConfigUpdate
	{
		public static async void Update()
		{
			string updateUrl = "https://raw.githubusercontent.com/LYQBING/DG-Lab-Game-Controller/main/version.json";
			CloudConfigItem? cloudConfig = await ApiHelper.GetAndParseAsync<CloudConfigItem>(updateUrl);

			if (cloudConfig == null)
			{
				DebugHub.Warning("无法获取云端配置", "尝试连接至 Github 远程仓库时发生错误：请检查您的网络环境...");
				return;
			}
			if (AppConfig.AppVersion != cloudConfig.VersionNumber)
			{
				new MessageDialog(cloudConfig.VersionName, cloudConfig.VersionDescription, "前往", data =>
				{
					try
					{
						Process.Start(new ProcessStartInfo
						{
							FileName = cloudConfig.DownloadUrl,
							UseShellExecute = true
						});
					}
					catch
					{
						DebugHub.Warning(cloudConfig.VersionName, cloudConfig.DownloadUrl);
						data.Close();
					}
				}, "取消").ShowDialog();
			}
		}
	}

	public class CloudConfigItem
	{
		public string VersionNumber { get; set; } = string.Empty;         // 版本号
		public string VersionName { get; set; } = string.Empty;           // 版本名称
		public string VersionDescription { get; set; } = string.Empty;    // 版本介绍
		public string DownloadUrl { get; set; } = string.Empty;           // 下载链接
	}
}
