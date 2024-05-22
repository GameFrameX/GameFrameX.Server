namespace GameFrameX.Setting;

public abstract class BaseSetting
{
    public TaskCompletionSource<bool> AppExitSource = new TaskCompletionSource<bool>();
    public Task<bool> AppExitToken => AppExitSource.Task;

    /// <summary>
    /// 是否是本地
    /// </summary>
    /// <param name="serverId">服务ID</param>
    /// <returns>返回是否是本地</returns>
    public virtual bool IsLocal(int serverId)
    {
        return serverId == ServerId;
    }

    /// <summary>
    /// 启动时间
    /// </summary>
    public DateTime LaunchTime { get; set; }

    bool _appRunning;
    private ServerType serverType;

    public bool AppRunning
    {
        get => _appRunning;
        set
        {
            lock (AppExitSource)
            {
                if (AppExitSource.Task.IsCanceled)
                {
                    if (value)
                    {
                        Serilog.Log.Error("AppRunning已经被设置为退出，不能再次开启...");
                    }

                    _appRunning = false;
                    return;
                }

                _appRunning = value;
                if (!value && !AppExitSource.Task.IsCompleted)
                {
                    Serilog.Log.Information("Set AppRunning false...");
                    AppExitSource.TrySetCanceled();
                }
            }
        }
    }

    /// <summary>
    /// 服务器类型
    /// </summary>
    public ServerType ServerType
    {
        get => serverType;
        set
        {
            serverType = value;
            ServerName = value.ToString();
        }
    }

    #region from config

    /// <summary>
    /// 是否是Debug 模式
    /// </summary>
    public bool IsDebug { get; set; }

    /// <summary>
    /// 是否打印发送数据
    /// </summary>
    public bool IsDebugSend { get; set; }

    /// <summary>
    /// 是否打印接收数据
    /// </summary>
    public bool IsDebugReceive { get; set; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    public int ServerId { get; set; }

    /// <summary>
    /// 服务器名称
    /// </summary>
    public string ServerName { get; protected set; }

    /// <summary>
    /// 保存数据间隔
    /// </summary>
    public int SaveDataInterval { get; set; } = 5000;

    /// <summary>
    /// 内部IP
    /// </summary>
    public string InnerIp { get; set; }

    /// <summary>
    /// 内部端口
    /// </summary>
    public ushort InnerPort { get; set; }

    /// <summary>
    /// 外部IP
    /// </summary>
    public string OuterIp { get; set; }

    /// <summary>
    /// 外部端口
    /// </summary>
    public ushort OuterPort { get; set; }

    /// <summary>
    /// HTTP 响应码
    /// </summary>
    public string HttpCode { get; set; }

    /// <summary>
    /// Http 地址
    /// </summary>
    public string HttpUrl { get; set; }

    /// <summary>
    /// HTTP 端口
    /// </summary>
    public int HttpPort { get; set; }

    /// <summary>
    /// HTTPS 端口
    /// </summary>
    public int HttpsPort { get; set; }

    /// <summary>
    /// WebSocket 端口
    /// </summary>
    public int WsPort { get; set; }

    /// <summary>
    /// WebSocket 加密端口
    /// </summary>
    public int WssPort { get; set; }

    /// <summary>
    /// Wss 使用的证书路径
    /// </summary>
    public string WssCertFilePath { get; set; }

    /// <summary>
    /// 数据库 地址
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
    /// SDK 类型
    /// </summary>
    public int SDKType { get; set; }

    /// <summary>
    /// Application Performance Monitor (应用性能监控) 端口
    /// </summary>
    public int APMPort { get; set; }

    /// <summary>
    /// 最大客户端数量
    /// </summary>
    public int MaxClientCount { get; set; } = 3000;

    #endregion
}