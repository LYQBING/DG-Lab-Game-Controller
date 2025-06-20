﻿namespace lyqbing.DGLAB
{
    using System.Collections.Generic;

    /// <summary>
    /// 实现 DG-Lab-Coyote-Game-Hub API
    /// </summary>
    public sealed class CoyoteApi
    {
        private static readonly Lazy<CoyoteApi> InstanceLazy = new(() => new CoyoteApi());
        private string _coyotreUrl = "http://127.0.0.1:8920/";
        private const string ApiUrl = "api/v2/game/";

        private CoyoteApi()
        {
        }

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
        /// 一键开火 API
        /// </summary>
        public string FireApi => _coyotreUrl + ApiUrl + ClientID + "/action/fire";

        /// <summary>
        /// 设置/读取 设置游戏强度配置 API
        /// </summary>
        public string StrengthConfigApi => _coyotreUrl + ApiUrl + ClientID + "/strength";

        /// <summary>
        /// 获取获取服务器配置的波形列表 API
        /// </summary>
        public string PulseListApi => _coyotreUrl + "api/v2/pulse_list";

        /// <summary>
        /// 获取完整的波形列表，包括客户端自定义波形 API
        /// </summary>
        public string AllPulseListApi => _coyotreUrl + ApiUrl + ClientID + "/pulse_list";

        /// <summary>
        /// 设置/获取 当前波形ID API
        /// </summary>
        public string PulseIdApi => _coyotreUrl + ApiUrl + ClientID + "/pulse";

        /// <summary>
        /// 获取游戏响应数据 API
        /// </summary>
        public string GameResponseApi => _coyotreUrl + ApiUrl + ClientID;

        #endregion
    }

    #region 波形列表相关

    /// <summary>
    /// 波形列表回执JSON
    /// </summary>
    public class PulseListJson
    {
        public int status;
        public string? code;
        public List<PulseList>? pulseList;
    }

    /// <summary>
    /// 波形列表配置文件
    /// </summary>
    public class PulseList
    {
        /// <summary>
        /// 脉冲ID
        /// </summary>
        public string? id;

        /// <summary>
        /// 脉冲名称
        /// </summary>
        public string? name;
    }

    #endregion

    #region 波形配置文件

    /// <summary>
    /// 波形配置文件
    /// </summary>
    public class PulseId
    {
        public int status;
        public string? code;

        /// <summary>
        /// 当前波形ID
        /// </summary>
        public string? currentPulseId;

        /// <summary>
        /// 当前波形列表
        /// </summary>
        public string[]? pulseId;
    }

    #endregion

    #region 强度配置相关

    /// <summary>
    /// 强度配置回执JSON
    /// </summary>
    public class StrengthConfigJson
    {
        public int status;
        public string? code;
        public StrengthConfig? strengthConfig;
    }

    /// <summary>
    /// 强度配置文件
    /// </summary>
    public class StrengthConfig
    {
        /// <summary>
        /// 基础强度
        /// </summary>
        public int strength;

        /// <summary>
        /// 随机强度
        /// </summary>
        public int randomStrength;
    }

    #endregion

    #region 响应数据相关

    /// <summary>
    /// 当前配置数据
    /// </summary>
    public class GameConfig
    {
        public int[]? strengthChangeInterval;
        public bool enableBChannel;
        public float bChannelStrengthMultiplier;
        public string? pulseId;
        public string? pulseMode;
        public int pulseChangeInterval;
    }

    /// <summary>
    /// 当前用户数据
    /// </summary>
    public class ClientStrength
    {
        public int strength;
        public int limit;
    }

    /// <summary>
    /// 游戏响应数据回执
    /// </summary>
    public class GameResponse
    {
        public int status;
        public string? code;
        public StrengthConfig? strengthConfig;
        public GameConfig? gameConfig;
        public ClientStrength? clientStrength;
        public string? currentPulseId;
    }

    #endregion
}