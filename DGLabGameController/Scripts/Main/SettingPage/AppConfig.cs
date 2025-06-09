using lyqbing.DGLAB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DGLabGameController
{
	public class AppConfig
	{
		/// <summary>
		/// 是否启用服务器模式，默认为 true
		/// </summary>
		public bool ServerMode { get; set; } = true;
		/// <summary>
		/// 服务器主机地址
		/// </summary>
		public string ServerUrl { get; set; } = "127.0.0.1";
		/// <summary>
		/// 服务器端口，默认为 8920
		/// </summary>
		public int ServerPort { get; set; } = 8920;
		/// <summary>
		/// 客户端 ID，默认为 "all"，表示所有客户端
		/// </summary>
		public string ClientId { get; set; } = "all";
		/// <summary>
		/// 服务器监听地址
		/// </summary>
		public string ServerHost { get; set; } = "0.0.0.0";
		/// <summary>
		/// 服务器波形配置文件路径
		/// </summary>
		public string PulseConfigPath { get; set; } = "pulse.yaml";
		/// <summary>
		/// 是否允许向客户端广播消息
		/// </summary>
		public bool AllowBroadcastToClients { get; set; } = true;
		/// <summary>
		/// 是否显示 PowerShell 窗口
		/// </summary>
		public bool DisplayPowerShell { get; set; } = false;
		/// <summary>
		/// 是否在启动时打开浏览器访问服务器地址
		/// </summary>
		public bool OpenBrowser { get; set; } = true;
		/// <summary>
		/// 是否启用调试日志
		/// </summary>
		public bool VerboseLogs { get; set; } = false;
		/// <summary>
		/// 是否在退出时显示菜单
		/// </summary>
		public bool ExitMenu { get; set; } = true;
	}
}