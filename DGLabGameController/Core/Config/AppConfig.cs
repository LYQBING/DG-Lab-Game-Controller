using System.IO;

namespace DGLabGameController.Core.Config
{
	/// <summary>
	/// APP 配置类
	/// <para>应用程序的全局配置类，包含 API 版本号、应用程序版本号和各种路径配置</para>
	/// </summary>
	public static class AppConfig
	{
		public const int ApiVersion = 10087; // API 版本号
		public const string AppVersion = "v3.6.30"; // 主程序版本号

		public static readonly string DataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"); // 数据存储路径
		public static readonly string ModulesPath = Path.Combine(DataPath, "Modules"); // 模块存储路径
		public static readonly string ServerPath = Path.Combine(DataPath, "CoyoteGameHub"); // 服务器存储路径

		public static readonly string ConfigPath = Path.Combine(DataPath, "config.json");
		public static readonly string ServerConfigPath = Path.Combine(ServerPath, "config.yaml");

	}
}