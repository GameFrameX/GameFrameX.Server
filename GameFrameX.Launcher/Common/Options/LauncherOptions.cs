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

    /// <summary>
    /// 是否是Debug 模式
    /// </summary>
    [Option("Debug", HelpText = "是否是Debug 模式")]
    public bool IsDebug { get; set; }

    /// <summary>
    /// 是否打印发送数据
    /// </summary>
    [Option("IsDebugSend", HelpText = "是否打印发送数据")]
    public bool IsDebugSend { get; set; }

    /// <summary>
    /// 是否打印接收数据
    /// </summary>
    [Option("IsDebugReceive", HelpText = "是否打印接收数据")]
    public bool IsDebugReceive { get; set; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    [Option("ServerId", HelpText = "服务器ID")]
    public int ServerId { get; set; }

    /// <summary>
    /// 保存数据间隔
    /// </summary>
    [Option("SaveDataInterval", HelpText = "保存数据间隔")]
    public int SaveDataInterval { get; set; } = 5000;

    /// <summary>
    /// 内部IP
    /// </summary>
    [Option("InnerIp", HelpText = "内部IP")]
    public string InnerIp { get; set; }

    /// <summary>
    /// 内部端口
    /// </summary>
    [Option("InnerPort", HelpText = "内部端口")]
    public ushort InnerPort { get; set; }

    /// <summary>
    /// 外部IP
    /// </summary>
    [Option("OuterIp", HelpText = "外部IP")]
    public string OuterIp { get; set; }

    /// <summary>
    /// 外部端口
    /// </summary>
    [Option("OuterPort", HelpText = "外部端口")]
    public ushort OuterPort { get; set; }

    /// <summary>
    /// HTTP 响应码
    /// </summary>
    [Option("HttpCode", HelpText = "HTTP 响应码")]
    public string HttpCode { get; set; }

    /// <summary>
    /// Http 地址
    /// </summary>
    [Option("HttpUrl", HelpText = "Http 地址")]
    public string HttpUrl { get; set; }

    /// <summary>
    /// HTTP 端口
    /// </summary>
    [Option("HttpPort", HelpText = "HTTP 端口")]
    public int HttpPort { get; set; }

    /// <summary>
    /// HTTPS 端口
    /// </summary>
    [Option("HttpsPort", HelpText = "HTTPS 端口")]
    public int HttpsPort { get; set; }

    /// <summary>
    /// WebSocket 端口
    /// </summary>
    [Option("WsPort", HelpText = "WebSocket 端口")]
    public int WsPort { get; set; }

    /// <summary>
    /// WebSocket 加密端口
    /// </summary>
    [Option("WssPort", HelpText = "WebSocket 加密端口")]
    public int WssPort { get; set; }

    /// <summary>
    /// Wss 使用的证书路径
    /// </summary>
    [Option("WssCertFilePath", HelpText = "Wss 使用的证书路径")]
    public string WssCertFilePath { get; set; }

    /// <summary>
    /// 数据库 地址
    /// </summary>
    [Option("DataBaseUrl", HelpText = "数据库 地址")]
    public string DataBaseUrl { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    [Option("DataBaseName", HelpText = "数据库名称")]
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
    [Option("DiscoveryCenterIp", HelpText = "发现中心地址")]
    public string DiscoveryCenterIp { get; set; }

    /// <summary>
    /// 发现中心端口
    /// </summary>
    [Option("DiscoveryCenterPort", HelpText = "发现中心端口")]
    public ushort DiscoveryCenterPort { get; set; }

    /// <summary>
    /// 数据库服务连接地址
    /// </summary>
    [Option("DBIp", HelpText = "数据库服务连接地址")]
    public string DBIp { get; set; }

    /// <summary>
    /// 数据库服务连接端口
    /// </summary>
    [Option("DBPort", HelpText = "数据库服务连接端口")]
    public ushort DBPort { get; set; }
}