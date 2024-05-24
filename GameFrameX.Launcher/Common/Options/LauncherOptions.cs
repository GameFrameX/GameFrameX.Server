using CommandLine;

namespace GameFrameX.Launcher.Common.Options;

public sealed class LauncherOptions
{
    /// <summary>
    /// 服务器类型
    /// </summary>
    [Option("ServerType", HelpText = "服务器类型,当该值无效时，默认为后续所有参数无效")]
    public string ServerType { get; set; }

    /// <summary>
    /// APM监控端口
    /// </summary>
    [Option("APMPort", HelpText = "APM监控端口")]
    public int APMPort { get; set; }

    public void CheckAPMPort()
    {
        if (APMPort <= 10000 || APMPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(APMPort), "APMPort必须大于10000且小于等于65535");
        }
    }

    /// <summary>
    /// 是否是Debug 模式
    /// </summary>
    [Option("Debug", Default = true, HelpText = "是否是Debug 模式")]
    public bool IsDebug { get; set; }

    /// <summary>
    /// 是否打印发送数据
    /// </summary>
    [Option("IsDebugSend", Default = true, HelpText = "是否打印发送数据")]
    public bool IsDebugSend { get; set; }

    /// <summary>
    /// 是否打印接收数据
    /// </summary>
    [Option("IsDebugReceive", Default = true, HelpText = "是否打印接收数据")]
    public bool IsDebugReceive { get; set; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    [Option("ServerId", HelpText = "服务器ID")]
    public int ServerId { get; set; }

    public void CheckServerId()
    {
        if (ServerId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ServerId), "ServerId必须大于0");
        }
    }

    /// <summary>
    /// 保存数据间隔
    /// </summary>
    [Option("SaveDataInterval", Default = 5000, HelpText = "保存数据间隔，单位毫秒")]
    public int SaveDataInterval { get; set; }

    /// <summary>
    /// 内部IP
    /// </summary>
    [Option("InnerIp", Default = "127.0.0.1", HelpText = "内部IP")]
    public string InnerIp { get; set; }

    public void CheckInnerIp()
    {
        if (InnerIp.IsNullOrWhiteSpace())
        {
            throw new ArgumentNullException(nameof(InnerIp), "内部IP不能为空");
        }
    }

    /// <summary>
    /// 内部端口
    /// </summary>
    [Option("InnerPort", HelpText = "内部端口")]
    public ushort InnerPort { get; set; }

    public void CheckInnerPort()
    {
        if (InnerPort <= 10000 || InnerPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(InnerPort), "内部端口必须大于10000且小于等于65535");
        }
    }

    /// <summary>
    /// 外部IP
    /// </summary>
    [Option("OuterIp", HelpText = "外部IP")]
    public string OuterIp { get; set; }

    public void CheckOuterIp()
    {
        if (OuterIp.IsNullOrWhiteSpace())
        {
            throw new ArgumentNullException(nameof(OuterIp), "外部IP不能为空");
        }
    }

    /// <summary>
    /// 外部端口
    /// </summary>
    [Option("OuterPort", HelpText = "外部端口")]
    public ushort OuterPort { get; set; }

    public void CheckOuterPort()
    {
        if (OuterPort <= 10000 || OuterPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(OuterPort), "外部端口必须大于10000且小于等于65535");
        }
    }

    /// <summary>
    /// HTTP 响应码
    /// </summary>
    [Option("HttpCode", HelpText = "HTTP 响应码")]
    public string HttpCode { get; set; }

    public void CheckHttpCode()
    {
        if (HttpCode.IsNullOrWhiteSpace())
        {
            throw new ArgumentNullException(nameof(HttpCode), "HTTP 响应码不能为空");
        }
    }

    /// <summary>
    /// Http 地址
    /// </summary>
    [Option("HttpUrl", HelpText = "Http 地址")]
    public string HttpUrl { get; set; }

    public void CheckHttpUrl()
    {
        if (HttpUrl.IsNullOrWhiteSpace())
        {
            throw new ArgumentNullException(nameof(HttpUrl), "Http 地址不能为空");
        }
    }

    /// <summary>
    /// HTTP 端口
    /// </summary>
    [Option("HttpPort", HelpText = "HTTP 端口")]
    public int HttpPort { get; set; }

    public void CheckHttpPort()
    {
        if (HttpPort <= 10000 || HttpPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(HttpPort), "Http 端口必须大于10000且小于等于65535");
        }
    }

    /// <summary>
    /// HTTPS 端口
    /// </summary>
    [Option("HttpsPort", HelpText = "HTTPS 端口")]
    public int HttpsPort { get; set; }

    public void CheckHttpsPort()
    {
        if (HttpsPort <= 10000 || HttpsPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(HttpsPort), "Https 端口必须大于10000且小于等于65535");
        }
    }

    /// <summary>
    /// WebSocket 端口
    /// </summary>
    [Option("WsPort", HelpText = "WebSocket 端口")]
    public int WsPort { get; set; }

    public void CheckWsPort()
    {
        if (WsPort <= 10000 || WsPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(WsPort), "Ws 端口必须大于10000且小于等于65535");
        }
    }

    /// <summary>
    /// WebSocket 加密端口
    /// </summary>
    [Option("WssPort", HelpText = "WebSocket 加密端口")]
    public int WssPort { get; set; }

    public void CheckWssPort()
    {
        if (WssPort <= 10000 || WssPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(WssPort), "Wss 端口必须大于10000且小于等于65535");
        }
    }

    /// <summary>
    /// Wss 使用的证书路径
    /// </summary>
    [Option("WssCertFilePath", HelpText = "Wss 使用的证书路径")]
    public string WssCertFilePath { get; set; }

    public void CheckWssCertFilePath()
    {
        if (WssCertFilePath.IsNullOrWhiteSpace())
        {
            throw new ArgumentNullException(nameof(WssCertFilePath), "Wss 使用的证书路径不能为空");
        }
    }

    /// <summary>
    /// 数据库 地址
    /// </summary>
    [Option("DataBaseUrl", HelpText = "数据库 地址")]
    public string DataBaseUrl { get; set; }

    public void CheckDataBaseUrl()
    {
        if (DataBaseUrl.IsNullOrWhiteSpace())
        {
            throw new ArgumentNullException(nameof(DataBaseUrl), "数据库 地址不能为空");
        }
    }

    /// <summary>
    /// 数据库名称
    /// </summary>
    [Option("DataBaseName", HelpText = "数据库名称")]
    public string DataBaseName { get; set; }

    public void CheckDataBaseName()
    {
        if (DataBaseName.IsNullOrWhiteSpace())
        {
            throw new ArgumentNullException(nameof(DataBaseName), "数据库名称不能为空");
        }
    }

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
    [Option("DiscoveryCenterIp", HelpText = "发现中心地址")]
    public string DiscoveryCenterIp { get; set; }

    public void CheckDiscoveryCenterIp()
    {
        if (DiscoveryCenterIp.IsNullOrWhiteSpace())
        {
            throw new ArgumentNullException(nameof(DiscoveryCenterIp), "发现中心地址不能为空");
        }
    }

    /// <summary>
    /// 发现中心端口
    /// </summary>
    [Option("DiscoveryCenterPort", HelpText = "发现中心端口")]
    public ushort DiscoveryCenterPort { get; set; }

    public void CheckDiscoveryCenterPort()
    {
        if (DiscoveryCenterPort <= 10000 || DiscoveryCenterPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(DiscoveryCenterPort), "发现中心端口必须大于10000且小于等于65535");
        }
    }

    /// <summary>
    /// 数据库服务连接地址
    /// </summary>
    [Option("DBIp", HelpText = "数据库服务连接地址")]
    public string DBIp { get; set; }

    public void CheckDBIp()
    {
        if (DBIp.IsNullOrWhiteSpace())
        {
            throw new ArgumentNullException(nameof(DBIp), "数据库服务连接地址不能为空");
        }
    }

    /// <summary>
    /// 数据库服务连接端口
    /// </summary>
    [Option("DBPort", HelpText = "数据库服务连接端口")]
    public ushort DBPort { get; set; }

    /// <summary>
    /// 检查数据库服务连接端口
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void CheckDBPort()
    {
        if (DBPort <= 10000 || DBPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(DBPort), "数据库服务连接端口必须大于10000且小于等于65535");
        }
    }
}