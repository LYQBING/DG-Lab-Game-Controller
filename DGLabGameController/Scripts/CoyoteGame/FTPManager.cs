namespace lyqbing.DGLAB
{
	using DGLabGameController;
	using System.Net.Http;
	using System.Threading.Tasks;

	public static class FTPManager
	{
		private static readonly HttpClient Client = new();

		/// <summary>
		/// GET请求
		/// </summary>
		public static Task<string?> GetAsync(string url)
		{
			HttpRequestMessage request = new(HttpMethod.Get, url);
			if (ConfigManager.Current.VerboseLogs) DebugHub.Log("GET请求", $"{url}");
			return SendAsync(request);
		}

		/// <summary>
		/// POST请求
		/// </summary>
		/// <param name="url">请求地址</param>
		/// <param name="formData">表单数据</param>
		/// <returns></returns>
		public static Task<string?> PostAsync(string url, IEnumerable<KeyValuePair<string, string>> formData)
		{
			var content = new FormUrlEncodedContent(formData);
			HttpRequestMessage request = new(HttpMethod.Post, url)
			{
				Content = content
			};
			if (ConfigManager.Current.VerboseLogs) DebugHub.Log("POST请求", string.Join("&", formData.Select(kv => $"{kv.Key}={kv.Value}")));

			return SendAsync(request);
		}

		/// <summary>
		/// 服务器请求回执
		/// </summary>
		public static async Task<string?> SendAsync(HttpRequestMessage request)
		{
			string? responseBody = null;
			try
			{
				using HttpResponseMessage response = await Client.SendAsync(request);
				if (!response.IsSuccessStatusCode)
				{
					DebugHub.Error("通讯失败", $"哦不！主人...与服务器通讯失败了，错误: {response.StatusCode}");
					return null;
				}

				responseBody = await response.Content.ReadAsStringAsync();
			}
			catch (Exception)
			{
				DebugHub.Error("通讯异常", "哦不！主人...与服务器通讯时发生了异常，请检查网络连接或服务器状态。");
			}

			return responseBody;
		}
	}
}