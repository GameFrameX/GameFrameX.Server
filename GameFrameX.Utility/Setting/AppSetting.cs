using System.Text.Json.Serialization;
using GameFrameX.Foundation.Json;
using GameFrameX.Utility.Log;

namespace GameFrameX.Utility.Setting;

/// <summary>
/// 应用程序配置类
/// </summary>
public sealed class AppSetting
{
    /// <summary>
    /// 用于通知应用程序退出的任务完成源
    /// </summary>
    [JsonIgnore] public readonly TaskCompletionSource<bool> AppExitSource = new();

    private bool _appRunning;
    private ServerType _serverType;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AppSetting()
    {
#if DEBUG
        IsDebug = true;
        IsDebugReceive = true;
        IsDebugSend = true;
#endif
    }

    /// <summary>
    /// 获取应用程序退出的任务标记
    /// </summary>
    [JsonIgnore]
    public Task<bool> AppExitToken
    {
        get { return AppExitSource.Task; }
    }

    /// <summary>
    /// 应用程序启动时间
    /// </summary>
    public DateTime LaunchTime { get; set; }

    /// <summary>
    /// 获取或设置应用程序是否正在运行
    /// </summary>
    [JsonIgnore]
    public bool AppRunning
    {
        get { return _appRunning; }
        set
        {
            lock (AppExitSource)
            {
                if (AppExitSource.Task.IsCanceled)
                {
                    if (value)
                    {
                        LogHelper.Error("AppRunning已经被设置为退出，不能再次开启...");
                    }

                    _appRunning = false;
                    return;
                }

                _appRunning = value;
                if (!value && !AppExitSource.Task.IsCompleted)
                {
                    LogHelper.Info("Set AppRunning false...");
                    AppExitSource.TrySetCanceled();
                }
            }
        }
    }

    /// <summary>
    /// 获取或设置服务器类型
    /// </summary>
    public ServerType ServerType
    {
        get { return _serverType; }
        set
        {
            _serverType = value;
            ServerName = value.ToString();
        }
    }

    /// <summary>
    /// 判断指定的服务ID是否为本地服务
    /// </summary>
    /// <param name="serverId">服务ID</param>
    /// <returns>返回是否是本地服务</returns>
    public bool IsLocal(int serverId)
    {
        return serverId == ServerId;
    }

    /// <summary>
    /// 将对象序列化为JSON字符串
    /// </summary>
    /// <returns>JSON字符串</returns>
    public override string ToString()
    {
        return JsonHelper.Serialize(this);
    }

    /// <summary>
    /// 将对象序列化为格式化的JSON字符串
    /// </summary>
    /// <returns>格式化的JSON字符串</returns>
    public string ToFormatString()
    {
        return JsonHelper.SerializeFormat(this);
    }

    #region 从配置文件读取的属性

    /// <summary>
    /// 是否是调试模式
    /// </summary>
    public bool IsDebug { get; set; }

    /// <summary>
    /// 是否打印发送的数据
    /// </summary>
    public bool IsDebugSend { get; set; }

    /// <summary>
    /// 是否打印接收的数据
    /// </summary>
    public bool IsDebugReceive { get; set; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    public int ServerId { get; set; }

    /// <summary>
    /// 服务器名称
    /// </summary>
    public string ServerName { get; set; }

    /// <summary>
    /// 标记名称
    /// </summary>
    public string TagName { get; set; }

    /// <summary>
    /// 保存数据的时间间隔（毫秒）
    /// </summary>
    public int SaveDataInterval { get; set; } = 300_000;

    /// <summary>
    /// 内部IP地址
    /// </summary>
    public string InnerIp { get; set; }

    /// <summary>
    /// 内部端口
    /// </summary>
    public ushort InnerPort { get; set; }

    /// <summary>
    /// 外部IP地址
    /// </summary>
    public string OuterIp { get; set; }

    /// <summary>
    /// 外部端口
    /// </summary>
    public ushort OuterPort { get; set; }

    /// <summary>
    /// HTTP响应码
    /// </summary>
    public string HttpCode { get; set; }

    /// <summary>
    /// HTTP地址
    /// </summary>
    public string HttpUrl { get; set; }

    /// <summary>
    /// HTTP端口
    /// </summary>
    public int HttpPort { get; set; }

    /// <summary>
    /// HTTPS端口
    /// </summary>
    public int HttpsPort { get; set; }

    /// <summary>
    /// WebSocket端口
    /// </summary>
    public int WsPort { get; set; }

    /// <summary>
    /// WebSocket加密端口
    /// </summary>
    public int WssPort { get; set; }

    /// <summary>
    /// Wss使用的证书路径
    /// </summary>
    public string WssCertFilePath { get; set; }

    /// <summary>
    /// 数据库地址
    /// </summary>
    public string DataBaseUrl { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    public string DataBaseName { get; set; }

    /// <summary>
    /// 语言
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// 数据中心
    /// </summary>
    public string DataCenter { get; set; }

    /// <summary>
    /// 发现中心地址
    /// </summary>
    public string DiscoveryCenterIp { get; set; }

    /// <summary>
    /// 发现中心端口
    /// </summary>
    public ushort DiscoveryCenterPort { get; set; }

    /// <summary>
    /// 数据库服务连接地址
    /// </summary>
    public string DBIp { get; set; }

    /// <summary>
    /// 数据库服务连接端口
    /// </summary>
    public ushort DBPort { get; set; }

    /// <summary>
    /// SDK类型
    /// </summary>
    public int SDKType { get; set; }

    /// <summary>
    /// 应用性能监控端口
    /// </summary>
    public int APMPort { get; set; }

    /// <summary>
    /// 最大客户端数量
    /// </summary>
    public int MaxClientCount { get; set; } = 3000;

    /// <summary>
    /// 游戏逻辑服务器的处理最小模块ID
    /// </summary>
    public short MinModuleId { get; set; }

    /// <summary>
    /// 游戏逻辑服务器的处理最大模块ID
    /// </summary>
    public short MaxModuleId { get; set; }

    #endregion
}