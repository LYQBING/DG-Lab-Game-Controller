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
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
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
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
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
                // ignored
            }

            return responseBody;
        }
    }
}