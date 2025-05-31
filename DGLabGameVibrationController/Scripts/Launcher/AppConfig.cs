using lyqbing.DGLAB;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;

namespace DGLabGameVibrationController
{
	/// <summary>
	/// 应用程序颜色配置类，包含标准颜色、辅助颜色、背景颜色等
	/// </summary>
	public static class AppColor
	{
		/// <summary>
		/// 标准颜色（亮黄色）
		/// </summary>
		public static Color StandardColor = ColorTranslator.FromHtml("#FFE99D");
		/// <summary>
		/// 辅助颜色（深黄色）
		/// </summary>
		public static Color AuxiliaryColor = ColorTranslator.FromHtml("#807652");

		/// <summary>
		/// 背景颜色（深灰色）
		/// </summary>
		public static Color BackgroundColor = ColorTranslator.FromHtml("#121212");
		/// <summary>
		/// 背景颜色（浅灰色）
		/// </summary>
		public static Color BackgroundColorLight = ColorTranslator.FromHtml("#272727");
		/// <summary>
		/// 高亮颜色（正白色）
		/// </summary>
		public static Color HighlightColor = ColorTranslator.FromHtml("#FFFFFF");
		/// <summary>
		/// 成功颜色（亮绿色）
		/// </summary>
		public static Color SuccessColor = ColorTranslator.FromHtml("#00FF00");
		/// <summary>
		/// 警告颜色（亮黄色）
		/// </summary>
		public static Color WarningColor = ColorTranslator.FromHtml("#FFFF00");
		/// <summary>
		/// 错误颜色（亮红色）
		/// </summary>
		public static Color ErrorColor = ColorTranslator.FromHtml("#FF0000");
	}

	/// <summary>
	/// 应用程序配置类，包含服务器地址、端口、客户端标识等设置
	/// </summary>
	public class AppConfig
	{
		#region 服务器配置

		/// <summary>
		/// 服务器地址
		/// </summary>
		public string ServerUrl { get; set; } = "127.0.0.1";

		/// <summary>
		/// 服务器端口
		/// </summary>
		public int ServerPort { get; set; } = 8920;

		/// <summary>
		/// 客户端标识
		/// </summary>
		public string ClientId { get; set; } = "all";

		#endregion

		#region DG-Lab 设置

		/// <summary>
		/// 是否启用双频合一模式
		/// </summary>
		public bool DualFreq { get; set; } = false;

		/// <summary>
		/// 是否启用线性输出
		/// </summary>
		public bool LinearOutput { get; set; } = true;

		/// <summary>
		/// 是否启用轻量模式
		/// </summary>
		public bool EasyMode { get; set; } = true;

		/// <summary>
		/// 输出倍数，默认为1.0
		/// </summary>
		public float OutputMultiplier { get; set; } = 20.0f;

		/// <summary>
		/// 基础强度，默认为0
		/// </summary>
		public int BaseStrength { get; set; } = 0;

		/// <summary>
		/// 默认控制器上限，默认为65535（Xbox控制器上限）
		/// </summary>
		public int ControllerLimit { get; set; } = 65535;

		#endregion

		#region 程序设置

		/// <summary>
		/// 是否启用详细日志
		/// </summary>
		public bool VerboseLogs { get; set; } = false;

		/// <summary>
		/// 是否启用动态注入
		/// </summary>
		public bool DynamicHook { get; set; } = false;

		/// <summary>
		/// 是否启用最小化窗口
		/// </summary>
		public bool ExitMenu { get; set; } = true;

		/// <summary>
		/// 是否启用底部 Dock 提示
		/// </summary>
		public bool DockTips { get; set; } = false;

		#endregion
	}

	/// <summary>
	/// 配置管理器，用于加载和保存应用程序配置
	/// </summary>
	public class ConfigManager
	{
		private const string ConfigPath = @"config.json";
		public static AppConfig Current { get; private set; } = new AppConfig();

		public static AppConfig Load()
		{
			if (File.Exists(ConfigPath))
			{
				try
				{
					AppConfig config = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(ConfigPath));
					if (config != null) Current = config;
				}
				catch (JsonException)
				{
					Current = new AppConfig();
				}
			}

			CoyoteApi.CoyotreUrl = Current.ServerUrl + ":" + Current.ServerPort + "/";
			CoyoteApi.ClientID = Current.ClientId;
			return Current;
		}

		public static void Save()
		{
			File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(Current, Formatting.Indented));
			CoyoteApi.CoyotreUrl = Current.ServerUrl + ":" + Current.ServerPort + "/";
			CoyoteApi.ClientID = Current.ClientId;
		}
	}
}