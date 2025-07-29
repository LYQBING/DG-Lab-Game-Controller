using DGLabGameController.Core.Debug;
using Newtonsoft.Json;

namespace DGLabGameController.Core.Network
{
	/// <summary>
	/// API 辅助类：简化与服务器间的数据交互
	/// </summary>
	public static class ApiHelper
	{
		/// <summary>
		/// 通用 GET 请求并反序列化为指定类型
		/// </summary>
		/// <typeparam name="T">类型</typeparam>
		/// <param name="api">地址</param>
		/// <returns></returns>
		public static async Task<T?> GetAndParseAsync<T>(string api)
		{
			string? json = await FTPManager.GetAsync(api);
			return ParseJson<T>(json);
		}

		/// <summary>
		/// 通用 POST 请求并反序列化为指定类型
		/// </summary>
		/// <typeparam name="T">类型</typeparam>
		/// <param name="api">地址</param
		/// <param name="formData">键值集合</param>
		/// <returns></returns>
		public static async Task<T?> PostAndParseAsync<T>(string api, IEnumerable<KeyValuePair<string, string>> formData)
		{
			string? json = await FTPManager.PostAsync(api, formData);
			return ParseJson<T>(json);
		}

		/// <summary>
		/// 通用 JSON 解析方法
		/// </summary>
		/// <typeparam name="T">类型</typeparam>
		/// <param name="json"></param>
		/// <returns></returns>
		public static T? ParseJson<T>(string? json)
		{
			if (string.IsNullOrWhiteSpace(json)) return default;
			try
			{
				return JsonConvert.DeserializeObject<T>(json);
			}
			catch (JsonException)
			{
				DebugHub.Error("解析失败", $"无法解析的 JSON 数据：{json}",true);
				return default;
			}
		}
	}
}