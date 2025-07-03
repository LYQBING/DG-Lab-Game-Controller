using DGLabGameController;
using Newtonsoft.Json;

namespace lyqbing.DGLAB
{
	/// <summary>
	/// DGLab 静态类，封装所有与游戏控制相关的 API 调用
	/// </summary>
	public static class DGLab
	{
		#region 通用私有方法

		/// <summary>
		/// 通用 GET 请求并反序列化为指定类型
		/// </summary>
		private static async Task<T?> GetAndParseAsync<T>(string api)
		{
			string? json = await FTPManager.GetAsync(api);
			return ParseJson<T>(json);
		}

		/// <summary>
		/// 通用 POST 请求并反序列化为指定类型
		/// </summary>
		private static async Task<T?> PostAndParseAsync<T>(string api, IEnumerable<KeyValuePair<string, string>> formData)
		{
			string? json = await FTPManager.PostAsync(api, formData);
			return ParseJson<T>(json);
		}

		/// <summary>
		/// 通用 JSON 解析
		/// </summary>
		private static T? ParseJson<T>(string? json)
		{
			if (string.IsNullOrWhiteSpace(json)) return default;
			try
			{
				return JsonConvert.DeserializeObject<T>(json);
			}
			catch (JsonException)
			{
				DebugHub.Error("解析失败", "无法解析服务器返回的 JSON 数据");
				return default;
			}
		}

		#endregion

		#region 响应数据

		/// <summary>
		/// 获取服务器响应数据
		/// </summary>
		public static Task<ServerResponse?> GetServerResponse() => GetAndParseAsync<ServerResponse>(CoyoteApi.Instance.ServerResponseApi);

		/// <summary>
		/// 获取游戏响应数据
		/// </summary>
		public static Task<GameResponse?> GetGameResponse() => GetAndParseAsync<GameResponse>(CoyoteApi.Instance.GameResponseApi);

		#endregion

		#region 波形相关

		/// <summary>
		/// 获取波形列表
		/// </summary>
		public static Task<PulseListJson?> GetAllPulseList() => GetAndParseAsync<PulseListJson>(CoyoteApi.Instance.PulseListApi);

		/// <summary>
		/// 获取游戏当前波形ID
		/// </summary>
		public static Task<PulseId?> GetPulseID() => GetAndParseAsync<PulseId>(CoyoteApi.Instance.PulseApi);

		/// <summary>
		/// 设置游戏当前波形ID
		/// </summary>
		/// <param name="pulseId">波形ID</param>
		public static Task<PulseId?> SetPulseID(string pulseId) => PostAndParseAsync<PulseId>(CoyoteApi.Instance.PulseApi,[new KeyValuePair<string, string>("pulseId", pulseId)]);

		/// <summary>
		/// 设置游戏当前波形ID
		/// </summary>
		/// <param name="pulseIds">波形ID列表</param>
		public static Task<PulseId?> SetPulseID(List<string> pulseIds) => PostAndParseAsync<PulseId>(CoyoteApi.Instance.PulseApi,pulseIds.Select(id => new KeyValuePair<string, string>("pulseId[]", id)));

		#endregion

		#region 一键开火

		/// <summary>
		/// 一键开火API
		/// </summary>
		/// <param name="strength">一键开火强度，最高40</param>
		/// <param name="time">一键开火时间，单位：毫秒，默认为5000，最高30000（30秒）</param>
		/// <param name="overrides">多次一键开火时，是否重置时间，true为重置时间，false为叠加时间，默认为false</param>
		/// <param name="pulseId">一键开火的波形ID</param>
		public static Task<FireJson?> Fire(int strength = 20, int time = 5000, bool overrides = false, string pulseId = "") =>
		PostAndParseAsync<FireJson>(CoyoteApi.Instance.FireApi,
			[
				new KeyValuePair<string, string>("strength", strength.ToString()),
				new KeyValuePair<string, string>("time", time.ToString()),
				new KeyValuePair<string, string>("overrides", overrides.ToString()),
				new KeyValuePair<string, string>("pulseId", pulseId)
			]
		);

		#endregion

		#region 游戏强度配置相关

		/// <summary>
		/// 设置基本游戏强度配置
		/// </summary>
		public static class SetStrength
		{
			/// <summary>
			/// 增加强度
			/// </summary>
			public static Task<StrengthConfigJson?> Add(int value = 1) => PostAndParseAsync<StrengthConfigJson>(CoyoteApi.Instance.StrengthApi,[new KeyValuePair<string, string>("strength.add", value.ToString())]);

			/// <summary>
			/// 减少强度
			/// </summary>
			public static Task<StrengthConfigJson?> Sub(int value = 1) => PostAndParseAsync<StrengthConfigJson>(CoyoteApi.Instance.StrengthApi,[new KeyValuePair<string, string>("strength.sub", value.ToString())]);

			/// <summary>
			/// 设置强度
			/// </summary>
			public static Task<StrengthConfigJson?> Set(int value = 1) => PostAndParseAsync<StrengthConfigJson>(CoyoteApi.Instance.StrengthApi,[new KeyValuePair<string, string>("strength.set", value.ToString())]);
		}

		/// <summary>
		/// 设置随机游戏强度配置
		/// </summary>
		public static class SetRandomStrength
		{
			/// <summary>
			/// 增加随机强度
			/// </summary>
			public static Task<StrengthConfigJson?> Add(int value = 1) => PostAndParseAsync<StrengthConfigJson>(CoyoteApi.Instance.StrengthApi,[new KeyValuePair<string, string>("randomStrength.add", value.ToString())]);

			/// <summary>
			/// 减少随机强度
			/// </summary>
			public static Task<StrengthConfigJson?> Sub(int value = 1) => PostAndParseAsync<StrengthConfigJson>(CoyoteApi.Instance.StrengthApi,[new KeyValuePair<string, string>("randomStrength.sub", value.ToString())]);

			/// <summary>
			/// 设置随机强度
			/// </summary>
			public static Task<StrengthConfigJson?> Set(int value = 1) => PostAndParseAsync<StrengthConfigJson>(CoyoteApi.Instance.StrengthApi,[new KeyValuePair<string, string>("randomStrength.set", value.ToString())]);
		}

		/// <summary>
		/// 获取游戏强度信息
		/// </summary>
		public static Task<StrengthConfigJson?> GetStrengthConfig() => GetAndParseAsync<StrengthConfigJson>(CoyoteApi.Instance.StrengthApi);

		#endregion
	}
}
