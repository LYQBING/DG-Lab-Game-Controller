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
        public static async Task<GameResponse?> GetGameResponse()
        {
            string? jsonPost = await FTPManager.GetAsync(CoyoteApi.Instance.GameResponseApi);
            if (jsonPost is null)
                return null;

            JToken token = JToken.Parse(jsonPost);
            return token.ToObject<GameResponse>();
        }

        #region 获取波形列表

        /// <summary>
        /// 获取获取服务器配置的波形列表 API
        /// </summary>
        public static async Task<List<PulseList>?> GetPulseList()
        {
            string? jsonPost = await FTPManager.GetAsync(CoyoteApi.Instance.PulseListApi);
            if (jsonPost is null)
                return null;

            JToken token = JToken.Parse(jsonPost);
            return token.ToObject<List<PulseList>>();
        }

        /// <summary>
        /// 获取完整的波形列表，包括客户端自定义波形 API
        /// </summary>
        public static async Task<List<PulseList>?> GetAllPulseList()
        {
            string? jsonPost = await FTPManager.GetAsync(CoyoteApi.Instance.AllPulseListApi);
            if (jsonPost is null)
                return null;

            JToken token = JToken.Parse(jsonPost);
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
        public static Task Fire(int strength, int time, bool overrides, string pulseId)
        {
            Dictionary<string, string> formData = new()
            {
                { "strength", strength.ToString() },
                { "time", time.ToString() },
                { "overrides", overrides.ToString() },
                { "pulseId", pulseId }
            };
            return FTPManager.PostAsync(CoyoteApi.Instance.FireApi, formData);
        }

        /// <summary>
        /// 一键开火API
        /// </summary>
        /// <param name="strength">一键开火强度，最高40</param>
        /// <param name="time">一键开火时间，单位：毫秒，默认为5000，最高30000（30秒）</param>
        /// <param name="overrides">多次一键开火时，是否重置时间，true为重置时间，false为叠加时间，默认为false</param>
        public static Task Fire(int strength = 20, int time = 5000, bool overrides = false)
        {
            Dictionary<string, string> formData = new()
            {
                { "strength", strength.ToString() },
                { "time", time.ToString() },
                { "overrides", overrides.ToString() }
            };
            return FTPManager.PostAsync(CoyoteApi.Instance.FireApi, formData);
        }

        #endregion

        #region 获取游戏当前波形ID

        /// <summary>
        /// 获取游戏当前波形ID
        /// 注意：与官方文档不同，文档中仅提示 pulseId (列表) 但还存在 currentPulseId (当前)；
        /// 因此，若要获取当前波形ID：请获取 currentPulseId ；若 pulseId 为空，则仅为单一波形
        /// </summary>
        /// <returns></returns>
        public static async Task<PulseId?> GetPulseID()
        {
            string? jsonPost = await FTPManager.GetAsync(CoyoteApi.Instance.PulseIdApi);
            if (jsonPost is null)
                return null;

            JToken token = JToken.Parse(jsonPost);
            return token.ToObject<PulseId>();
        }

        #endregion

        #region 设置游戏当前波形ID

        /// <summary>
        /// 设置游戏当前波形ID
        /// </summary>
        /// <param name="pulseId">波形ID</param>
        public static Task SetPulseID(string pulseId)
        {
            Dictionary<string, string> formData = new()
            {
                { "pulseId", pulseId }
            };
            return FTPManager.PostAsync(CoyoteApi.Instance.PulseIdApi, formData);
        }

        /// <summary>
        /// 设置游戏当前波形List
        /// </summary>
        /// <param name="pulseIds">波形List</param>
        public static Task SetPulseID(List<string> pulseIds)
        {
            var formData = pulseIds.Select(id => new KeyValuePair<string, string>("pulseId[]", id));
            return FTPManager.PostAsync(CoyoteApi.Instance.PulseIdApi, formData);
        }

        #endregion

        #region 游戏强度配置相关

        /// <summary>
        /// 设置基本游戏强度配置
        /// </summary>
        public static class SetStrength
        {
            public static Task Add(int value = 1)
            {
                Dictionary<string, string> formData = new()
                {
                    { "strength.add", value.ToString() }
                };
                return FTPManager.PostAsync(CoyoteApi.Instance.StrengthConfigApi, formData);
            }

            public static Task Sub(int value = 1)
            {
                Dictionary<string, string> formData = new()
                {
                    { "strength.sub", value.ToString() }
                };
                return FTPManager.PostAsync(CoyoteApi.Instance.StrengthConfigApi, formData);
            }

            public static Task Set(int value = 1)
            {
                Dictionary<string, string> formData = new()
                {
                    { "strength.set", value.ToString() }
                };
                return FTPManager.PostAsync(CoyoteApi.Instance.StrengthConfigApi, formData);
            }
        }

        /// <summary>
        /// 设置随机游戏强度配置
        /// </summary>
        public static class SetRandomStrength
        {
            public static Task Add(int value = 1)
            {
                Dictionary<string, string> formData = new()
                {
                    { "randomStrength.add", value.ToString() }
                };
                return FTPManager.PostAsync(CoyoteApi.Instance.StrengthConfigApi, formData);
            }

            public static Task Sub(int value = 1)
            {
                Dictionary<string, string> formData = new()
                {
                    { "randomStrength.sub", value.ToString() }
                };
                return FTPManager.PostAsync(CoyoteApi.Instance.StrengthConfigApi, formData);
            }

            public static Task Set(int value = 1)
            {
                Dictionary<string, string> formData = new()
                {
                    { "randomStrength.set", value.ToString() }
                };
                return FTPManager.PostAsync(CoyoteApi.Instance.StrengthConfigApi, formData);
            }
        }

        /// <summary>
        /// 获取游戏强度信息
        /// </summary>
        public static async Task<StrengthConfigJson?> GetStrengthConfig()
        {
            string? jsonPost = await FTPManager.GetAsync(CoyoteApi.Instance.StrengthConfigApi);
            if (jsonPost is null)
                return null;

            JToken token = JToken.Parse(jsonPost);
            return token.ToObject<StrengthConfigJson>();
        }

        #endregion
    }
}