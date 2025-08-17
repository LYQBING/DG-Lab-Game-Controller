namespace DGLabGameController.Core.Network
{
	using DGLabGameController.Core.Debug;
	using System.Net.Http;
	using System.Threading.Tasks;

	/// <summary>
	/// 网络请求管理器
	/// </summary>
	public static class FTPManager
	{
		private static readonly HttpClient Client = new();

		/// <summary>
		/// GET请求
		/// </summary>
		/// <param name="url">请求地址</param>
		/// <returns></returns>
		public static Task<string?> GetAsync(string url)
		{
			HttpRequestMessage request = new(HttpMethod.Get, url);
			DebugHub.Log("GET", $"URL:{url}", true);
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
			DebugHub.Log("POST", $"URL:{url} Data:" + string.Join("&", formData.Select(kv => $"{kv.Key}={kv.Value}")),true);

			return SendAsync(request);
		}

		/// <summary>
		/// 服务器请求的通用方法
		/// </summary>
		/// <param name="request">对象</param>
		/// <returns></returns>
		public static async Task<string?> SendAsync(HttpRequestMessage request)
		{
			string? responseBody = null;
			try
			{
				using HttpResponseMessage response = await Client.SendAsync(request);
				if (!response.IsSuccessStatusCode)
				{
					DebugHub.Error("连接异常", response.StatusCode.ToString());
					return null;
				}
				responseBody = await response.Content.ReadAsStringAsync();
			}
			catch (Exception)
			{
				DebugHub.Error("通讯异常", "哦不！尝试与服务器通讯时发生了意料之外的错误");
				return null;
			}

			return responseBody;
		}
	}
}