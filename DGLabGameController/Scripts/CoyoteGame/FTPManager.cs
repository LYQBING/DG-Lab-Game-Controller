namespace lyqbing.DGLAB
{
	using DGLabGameController;
	using System;
	using System.Net.Http;
	using System.Text;
	using System.Threading.Tasks;

	public static class FTPManager
	{
		/// <summary>
		/// Get请求
		/// </summary>
		public static async Task<string?> Get(string Url)
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Url);
			return await IsSuccessStatusCode(request);
		}

		/// <summary>
		/// POST请求
		/// </summary>
		public static async Task<string?> Post(string Url, string jsonParas)
		{
			try
			{
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Url)
				{
					Content = new StringContent(jsonParas, Encoding.UTF8, "application/x-www-form-urlencoded")
				};
				return await IsSuccessStatusCode(request);
			}
			catch
			{
				DebugHub.Error("灾难性故障", $"主人...服务器配置信息错误，或不存在指定服务器哦！URL：{Url} JSON：{jsonParas}");
				return null;
			}
		}

		/// <summary>
		/// 服务器请求回执
		/// </summary>
		public static async Task<string?> IsSuccessStatusCode(HttpRequestMessage request)
		{
			HttpClient httpClient = new HttpClient();
			HttpResponseMessage response = await httpClient.SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				string responseBody = await response.Content.ReadAsStringAsync();
				return responseBody;
			}
			else
			{
				DebugHub.Error("通讯失败", $"哦不！主人...与服务器通讯失败了，错误: {response.StatusCode}");
				return null;
			}
		}
	}
}