namespace DGLabGameController.Core.DGLabApi
{
	using System.Collections.Generic;

	/// <summary>
	/// 与 DG-Lab-Coyote-Game-Hub 服务器通讯的 API 接口
	/// <para>模块项目不建议直接使用此 API，请使用 DGLab.cs 中的相关函数</para>
	/// </summary>
	public sealed class CoyoteApi
	{
		private static readonly Lazy<CoyoteApi> InstanceLazy = new(() => new CoyoteApi());
		private string _coyotreUrl = "http://127.0.0.1:8920/";
		private const string ApiUrl = "api/v2/game/";

		private CoyoteApi() { }
		public static CoyoteApi Instance => InstanceLazy.Value;

		#region 配置服务器/本机属性

		/// <summary>
		/// Coyote-Game-Hub 服务器地址
		/// </summary>
		public static string CoyotreUrl
		{
			get => Instance._coyotreUrl;
			set => Instance._coyotreUrl = "http://" + value;
		}

		/// <summary>
		/// 客户端ID
		/// </summary>
		public static string ClientID { get; set; } = "all";

		#endregion

		#region 获取对应功能 Api 地址

		/// <summary>
		/// 获取服务器响应数据 API
		/// </summary>
		public string ServerResponseApi => _coyotreUrl + ApiUrl;

		/// <summary>
		/// 获取游戏响应数据 API
		/// </summary>
		public string GameResponseApi => _coyotreUrl + ApiUrl + ClientID;

		/// <summary>
		/// 设置/读取 设置游戏强度配置 API
		/// </summary>
		public string StrengthApi => _coyotreUrl + ApiUrl + ClientID + "/strength";

		/// <summary>
		/// 设置/获取 当前波形ID API
		/// </summary>
		public string PulseApi => _coyotreUrl + ApiUrl + ClientID + "/pulse";

		/// <summary>
		/// 获取完整的波形列表，包括客户端自定义波形 API
		/// </summary>
		public string PulseListApi => _coyotreUrl + ApiUrl + ClientID + "/pulse_list";

		/// <summary>
		/// 一键开火 API
		/// </summary>
		public string FireApi => _coyotreUrl + ApiUrl + ClientID + "/action/fire";

		#endregion
	}

	#region 基础类型

	/// <summary>
	/// 警告信息
	/// </summary>
	public class Warnings
	{
		/// <summary>
		/// 警告代码
		/// </summary>
		public string? Code { get; set; }
		/// <summary>
		/// 警告信息
		/// </summary>
		public string? Message { get; set; }
	}
	#endregion

	#region 波形相关

	/// <summary>
	/// 波形列表配置文件
	/// </summary>
	public class PulseList
	{
		/// <summary>
		/// 脉冲ID
		/// </summary>
		public string? Id { get; set; }

		/// <summary>
		/// 脉冲名称
		/// </summary>
		public string? Name { get; set; }
	}

	/// <summary>
	/// 波形列表回执JSON
	/// </summary>
	public class PulseListJson
	{
		public int Status { get; set; }
		public string? Code { get; set; }
		public string? Message { get; set; }
		public List<Warnings>? Warnings { get; set; }
		public List<PulseList>? PulseList { get; set; }
	}

	/// <summary>
	/// 波形配置文件
	/// </summary>
	public class PulseId
	{
		public int Status { get; set; }
		public string? Code { get; set; }

		/// <summary>
		/// 当前波形ID
		/// </summary>
		public string? CurrentPulseId { get; set; }

		/// <summary>
		/// 当前波形列表
		/// </summary>
		public string[]? PulseIdList { get; set; }
	}

	#endregion

	#region 强度配置相关

	/// <summary>
	/// 强度配置文件
	/// </summary>
	public class StrengthConfig
	{
		/// <summary>
		/// 基础强度
		/// </summary>
		public int Strength { get; set; }

		/// <summary>
		/// 随机强度
		/// </summary>
		public int RandomStrength { get; set; }
	}

	/// <summary>
	/// 强度配置回执JSON
	/// </summary>
	public class StrengthConfigJson
	{
		public int Status { get; set; }
		public string? Code { get; set; }
		public string? Message { get; set; }
		public List<Warnings>? Warnings { get; set; }
		public StrengthConfig? StrengthConfig { get; set; }
	}

	/// <summary>
	/// 服务器响应
	/// </summary>
	public class ServerResponse
	{
		public int Status { get; set; }
		public string? Code { get; set; }
		public string? Message { get; set; }
		public List<Warnings>? Warnings { get; set; }
		public int MinApiVersion { get; set; }
		public int MaxApiVersion { get; set; }
	}

	#endregion

	#region 响应数据相关

	/// <summary>
	/// 当前配置数据
	/// </summary>
	public class GameConfig
	{
		public int? FireStrengthLimit { get; set; }
		public int[]? StrengthChangeInterval { get; set; }
		public bool EnableBChannel { get; set; }
		public float BChannelStrengthMultiplier { get; set; }
		public string? PulseId { get; set; }
		public string? FirePulseId { get; set; }
		public string? PulseMode { get; set; }
		public int PulseChangeInterval { get; set; }
	}

	/// <summary>
	/// 当前用户数据
	/// </summary>
	public class ClientStrength
	{
		public int Strength { get; set; }
		public int Limit { get; set; }
	}

	/// <summary>
	/// 游戏响应数据回执
	/// </summary>
	public class GameResponse
	{
		public int Status { get; set; }
		public string? Code { get; set; }
		public string? Message { get; set; }
		public List<Warnings>? Warnings { get; set; }
		public StrengthConfig? StrengthConfig { get; set; }
		public GameConfig? GameConfig { get; set; }
		public ClientStrength? ClientStrength { get; set; }
		public string? CurrentPulseId { get; set; }
	}

	/// <summary>
	/// 一键开火响应
	/// </summary>
	public class FireJson
	{
		public int Status { get; set; }
		public string? Code { get; set; }
		public string? Message { get; set; }
		public List<Warnings>? Warnings { get; set; }
		public string[]? SuccessClientIds { get; set; }
	}

	#endregion
}