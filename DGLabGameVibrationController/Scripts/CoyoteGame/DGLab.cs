namespace lyqbing.DGLAB
{
	using Newtonsoft.Json.Linq;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public static class DGLab
	{
		/// <summary>
		/// 获取游戏响应数据
		/// </summary>S
		public static async Task<GameResponse> GetGameResponse()
		{
			string JsonPost = await FTPManager.Get(CoyoteApi.Instance.GameResponseApi);

			JToken token = JToken.Parse(JsonPost);
			return token.ToObject<GameResponse>();
		}

		#region 获取波形列表
		/// <summary>
		/// 获取获取服务器配置的波形列表 API
		/// </summary>
		public static async Task<List<PulseList>> GetPulseList()
		{
			string JsonPost = await FTPManager.Get(CoyoteApi.Instance.PulseListApi);

			JToken token = JToken.Parse(JsonPost);
			return token.ToObject<List<PulseList>>();
		}

		/// <summary>
		/// 获取完整的波形列表，包括客户端自定义波形 API
		/// </summary>
		public static async Task<List<PulseList>> GetAllPulseList()
		{
			string JsonPost = await FTPManager.Get(CoyoteApi.Instance.AllPulseListApi);

			JToken token = JToken.Parse(JsonPost);
			return token.ToObject<List<PulseList>>();
		}
		#endregion

		#region 一键开火
		/// <summary>
		/// 一键开火API
		/// </summary>
		/// <param name="strength">一键开火强度，最高40</param>
		/// <param name="time">一键开火时间，单位：毫秒，默认为5000，最高30000（30秒）</param>
		/// <param name="overrides">多次一键开火时，是否重置时间，true为重置时间，false为叠加时间，默认为false</param>
		/// <param name="pulseId">一键开火的波形ID</param>
		public static void Fire(int strength, int time, bool overrides, string pulseId)
		{
			string JsonPost = "strength=" + strength + "&time=" + time + "&override" + overrides + "&pulseId=" + pulseId;
			FireFTP(JsonPost);
		}

		/// <summary>
		/// 一键开火API
		/// </summary>
		/// <param name="strength">一键开火强度，最高40</param>
		/// <param name="time">一键开火时间，单位：毫秒，默认为5000，最高30000（30秒）</param>
		/// <param name="overrides">多次一键开火时，是否重置时间，true为重置时间，false为叠加时间，默认为false</param>
		public static void Fire(int strength = 20, int time = 5000, bool overrides = false)
		{
			string JsonPost = "strength=" + strength + "&time=" + time + "&override" + overrides;
			FireFTP(JsonPost);
		}

		private static async void FireFTP(string JsonPost)
		{
			await FTPManager.Post(CoyoteApi.Instance.FireApi, JsonPost);
		}
		#endregion

		#region 获取游戏当前波形ID
		/// <summary>
		/// 获取游戏当前波形ID
		/// 注意：与官方文档不同，文档中仅提示 pulseId (列表) 但还存在 currentPulseId (当前)；
		/// 因此，若要获取当前波形ID：请获取 currentPulseId ；若 pulseId 为空，则仅为单一波形
		/// </summary>
		/// <returns></returns>
		public static async Task<PulseId> GetPulseID()
		{
			string JsonPost = await FTPManager.Get(CoyoteApi.Instance.PulseIdApi);
			JToken token = JToken.Parse(JsonPost);
			return token.ToObject<PulseId>();
		}
		#endregion

		#region 设置游戏当前波形ID
		/// <summary>
		/// 设置游戏当前波形ID
		/// </summary>
		/// <param name="pulseId">波形ID</param>
		public static void SetPulseID(string pulseIds)
		{
			string JsonPost = "pulseId=" + pulseIds;
			PulseFTP(JsonPost);
		}

		/// <summary>
		/// 设置游戏当前波形List
		/// </summary>
		/// <param name="pulseIds">波形List</param>
		public static void SetPulseID(List<string> pulseIds)
		{
			string JsonPost = "";
			foreach (string id in pulseIds)
			{
				JsonPost += "pulseId[]=" + id + "&";
			}

			PulseFTP(JsonPost);
		}

		private static async void PulseFTP(string JsonPost)
		{
			await FTPManager.Post(CoyoteApi.Instance.PulseIdApi, JsonPost);
		}
		#endregion

		#region 游戏强度配置相关
		/// <summary>
		/// 设置基本游戏强度配置
		/// </summary>
		public static class SetStrength
		{
			public static void Add(int Add = 1)
			{
				string JsonPost = "strength.add=" + Add;
				StrengthFTP(JsonPost);
			}
			public static void Sub(int Sub = 1)
			{
				string JsonPost = "strength.sub=" + Sub;
				StrengthFTP(JsonPost);
			}
			public static void Set(int Set = 1)
			{
				string JsonPost = "strength.set=" + Set;
				StrengthFTP(JsonPost);
			}
		}

		/// <summary>
		/// 设置随机游戏强度配置
		/// </summary>
		public static class SetRandomStrength
		{
			public static void Add(int Add = 1)
			{
				string JsonPost = "randomStrength.add=" + Add;
				StrengthFTP(JsonPost);
			}
			public static void Sub(int Sub = 1)
			{
				string JsonPost = "randomStrength.sub=" + Sub;
				StrengthFTP(JsonPost);
			}
			public static void Set(int Set = 1)
			{
				string JsonPost = "randomStrength.set=" + Set;
				StrengthFTP(JsonPost);
			}
		}

		/// <summary>
		/// 获取游戏强度信息
		/// </summary>
		public static async Task<StrengthConfigJson> GetStrengthConfig()
		{
			string JsonPost = await FTPManager.Get(CoyoteApi.Instance.StrengthConfigApi);
			JToken token = JToken.Parse(JsonPost);
			return token.ToObject<StrengthConfigJson>();
		}

		private static async void StrengthFTP(string JsonPost)
		{
			await FTPManager.Post(CoyoteApi.Instance.StrengthConfigApi, JsonPost);
		}

		#endregion
	}
}