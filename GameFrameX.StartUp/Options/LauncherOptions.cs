using CommandLine;
using GameFrameX.Utility.Extensions;
using Serilog;
using Serilog.Events;

namespace GameFrameX.StartUp.Options;

/// <summary>
/// 启动参数
/// </summary>
public sealed class LauncherOptions
{
    /// <summary>
    /// 服务器类型
    /// </summary>
    [Option(nameof(ServerType), Required = true, HelpText = "服务器类型,当该值无效时,默认为后续所有参数无效")]
    [GrafanaLokiLabelTag]
    public string ServerType { get; set; }

    /// <summary>
    /// APM监控端口
    /// </summary>
    [Option(nameof(APMPort), HelpText = "APM监控端口")]
    public ushort APMPort { get; set; }

    /// <summary>
    /// 是否启用指标收集功能,需要IsOpenTelemetry为true时有效
    /// <para>用于收集和监控应用程序的性能指标数据</para>
    /// <para>默认值为false</para>
    /// </summary>
    [Option(nameof(IsOpenTelemetryMetrics), Default = false, HelpText = "是否启用分布式追踪功能,需要 IsOpenTelemetry 为true时有效,默认值为false")]
    public bool IsOpenTelemetryMetrics { get; set; }

    /// <summary>
    /// 是否启用分布式追踪功能,需要IsOpenTelemetry为true时有效
    /// <para>用于跟踪和分析分布式系统中的请求流程</para>
    /// <para>默认值为false</para>
    /// </summary>
    [Option(nameof(IsOpenTelemetryTracing), Default = false, HelpText = "是否启用分布式追踪功能,需要 IsOpenTelemetry为true时有效,默认值为false")]
    public bool IsOpenTelemetryTracing { get; set; }

    /// <summary>
    /// 是否启用OpenTelemetry遥测功能
    /// <para>OpenTelemetry是一个开源的可观测性框架</para>
    /// <para>启用后可以统一管理指标、追踪和日志等可观测性数据</para>
    /// <para>默认值为false</para>
    /// </summary>
    [Option(nameof(IsOpenTelemetry), Default = false, HelpText = "是否启用OpenTelemetry遥测功能,默认值为false")]
    public bool IsOpenTelemetry { get; set; }

    /// <summary>
    /// 是否监控打印超时日志
    /// </summary>
    [Option(nameof(IsMonitorMessageTimeOut), Default = false, HelpText = "是否打印超时日志,默认值为false")]
    public bool IsMonitorMessageTimeOut { get; set; }

    /// <summary>
    /// 监控处理器超时时间（秒）,默认值为1秒,只有IsMonitorTimeOut为true时有效
    /// </summary>
    [Option(nameof(MonitorMessageTimeOutSeconds), Default = 1, HelpText = "处理器超时时间（秒）,默认值为1秒,只有IsMonitorMessageTimeOut为true时有效")]
    public int MonitorMessageTimeOutSeconds { get; set; }

    /// <summary>
    /// 网络发送等待超时时间（秒）,默认值为5秒
    /// </summary>
    [Option(nameof(NetWorkSendTimeOutSeconds), Default = 5, HelpText = "网络发送等待超时时间（秒）,默认值为5秒,最小值为1秒")]
    public int NetWorkSendTimeOutSeconds { get; set; }

    /// <summary>
    /// 是否是Debug打印日志模式,默认值为false
    /// </summary>
    [Option(nameof(IsDebug), Default = false, HelpText = "是否是Debug打印日志模式,默认值为false")]
    public bool IsDebug { get; set; }

    /// <summary>
    /// 是否打印发送数据,只有在IsDebug为true时有效,默认值为false
    /// </summary>
    [Option(nameof(IsDebugSend), Default = false, HelpText = "是否打印发送数据,只有在IsDebug为true时有效,默认值为false")]
    public bool IsDebugSend { get; set; }

    /// <summary>
    /// 是否打印发送的心跳数据,只有在IsDebugSend为true时有效,默认值为false
    /// </summary>
    [Option(nameof(IsDebugSendHeartBeat), Default = false, HelpText = "是否打印发送的心跳数据,只有在IsDebugSend为true时有效,默认值为false")]
    public bool IsDebugSendHeartBeat { get; set; }

    /// <summary>
    /// 是否打印接收数据,只有在IsDebug为true时有效,默认值为false
    /// </summary>
    [Option(nameof(IsDebugReceive), Default = false, HelpText = "是否打印接收数据,只有在IsDebug为true时有效,默认值为false")]
    public bool IsDebugReceive { get; set; }

    /// <summary>
    /// 是否打印接收的心跳数据,只有在IsDebugReceive为true时有效,默认值为false
    /// </summary>
    [Option(nameof(IsDebugReceiveHeartBeat), Default = false, HelpText = "是否打印接收的心跳数据,只有在IsDebugReceive为true时有效,默认值为false")]
    public bool IsDebugReceiveHeartBeat { get; set; }

    /// <summary>是否输出到控制台,默认为 true。</summary>
    /// <remarks>控制日志是否同时在控制台显示,便于开发调试。</remarks>
    [Option(nameof(LogGrafanaLokiUsername), Default = false, HelpText = "是否输出到控制台,默认为 false。")]
    public bool LogIsConsole { get; set; } = false;

    /// <summary>是否输出到 GrafanaLoki,默认为 false。</summary>
    [Option(nameof(LogIsGrafanaLoki), Default = false, HelpText = "是否输出到 GrafanaLoki,默认为 false。")]
    public bool LogIsGrafanaLoki { get; set; }

    /// <summary>GrafanaLoki 服务地址,默认为 http://localhost:3100。</summary>
    [Option(nameof(LogGrafanaLokiUrl), Default = "http://localhost:3100", HelpText = "GrafanaLoki 服务地址,默认为 http://localhost:3100。当LogIsGrafanaLoki为true时生效。")]
    public string LogGrafanaLokiUrl { get; set; } = "http://localhost:3100";

    /// <summary>GrafanaLoki 标签</summary>
    public IEnumerable<string> LogGrafanaLokiLabels { get; set; } = new List<string>();

    /// <summary>GrafanaLoki 用户名或Email</summary>
    [Option(nameof(LogGrafanaLokiUsername), HelpText = "GrafanaLoki 用户名或Email,当LogIsGrafanaLoki为true时生效。")]
    public string LogGrafanaLokiUsername { get; set; }

    /// <summary>GrafanaLoki 密码</summary>
    [Option(nameof(LogGrafanaLokiPassword), HelpText = "GrafanaLoki 密码,当LogIsGrafanaLoki为true时生效。")]
    public string LogGrafanaLokiPassword { get; set; }

    /// <summary>日志滚动间隔,默认为每天（Day）。</summary>
    /// <remarks>决定日志文件创建新文件的时间间隔,可以是小时、天、月等。</remarks>
    [Option(nameof(LogRollingInterval), Default = RollingInterval.Day, HelpText = "日志滚动间隔,默认为每天(Day),日志滚动间隔(可选值：Minute[分], Hour[时], Day[天], Month[月], Year[年], Infinite[无限])")]
    public RollingInterval LogRollingInterval { get; set; } = RollingInterval.Day;

    /// <summary>日志输出级别,默认为 Debug。</summary>
    /// <remarks>控制日志输出的最低级别,低于此级别的日志将不会被记录。</remarks>
    [Option(nameof(LogEventLevel), Default = LogEventLevel.Debug, HelpText = "日志输出级别,默认为 Debug,日志级别(可选值：Verbose[详细], Debug[调试], Information[信息], Warning[警告], Error[错误], Fatal[致命])")]
    public LogEventLevel LogEventLevel { get; set; } = LogEventLevel.Debug;

    /// <summary>是否限制单个文件大小,默认为 true。</summary>
    /// <remarks>启用此选项可以防止单个日志文件过大。</remarks>
    [Option(nameof(LogIsFileSizeLimit), Default = true, HelpText = "是否限制单个文件大小,默认为 true。")]
    public bool LogIsFileSizeLimit { get; set; } = true;

    /// <summary>
    /// 日志单个文件大小限制,默认为 100MB。
    /// 当 IsFileSizeLimit 为 true 时有效。
    /// </summary>
    /// <remarks>当日志文件达到此大小限制时,将创建新的日志文件继续写入。</remarks>
    [Option(nameof(LogFileSizeLimitBytes), Default = 104857600, HelpText = "日志单个文件大小限制,默认为 100MB。当 LogIsFileSizeLimit 为 true 时有效。")]
    public int LogFileSizeLimitBytes { get; set; } = 104857600;

    /// <summary>
    /// 日志文件保留数量限制 默认为 31 个文件,即 31 天的日志文件
    /// 当 设置值为 null 时不限制文件数量
    /// </summary>
    /// <remarks>用于控制历史日志文件的数量,防止占用过多磁盘空间。</remarks>
    [Option(nameof(LogRetainedFileCountLimit), Default = 31, HelpText = "日志文件保留数量限制 默认为 31 个文件,即 31 天的日志文件")]
    public int? LogRetainedFileCountLimit { get; set; } = 31;

    /// <summary>
    /// 服务器ID
    /// </summary>
    [Option(nameof(ServerId), HelpText = "服务器ID")]
    [GrafanaLokiLabelTag]
    public int ServerId { get; set; }

    /// <summary>
    /// 服务器实例ID
    /// </summary>
    [Option(nameof(ServerInstanceId), HelpText = "服务器实例ID")]
    [GrafanaLokiLabelTag]
    public long ServerInstanceId { get; set; }

    /// <summary>
    /// 保存数据间隔,单位毫秒,默认300秒(5分钟),最小值为5秒(5000毫秒)
    /// </summary>
    [Option(nameof(SaveDataInterval), Default = 300_000, HelpText = "保存数据间隔,单位毫秒,默认300秒(5分钟),最小值为5秒(5000毫秒)")]
    public int SaveDataInterval { get; set; }

    /// <summary>
    /// 保存数据的批量数量长度,默认为500
    /// </summary>
    [Option(nameof(SaveDataBatchCount), Default = 500, HelpText = "保存数据的批量数量长度,默认为500")]
    public int SaveDataBatchCount { get; set; } = 500;

    /// <summary>
    /// 保存数据的超时时间(毫秒),默认值为30秒
    /// </summary>
    [Option(nameof(SaveDataBatchTimeOut), Default = 30_000, HelpText = "保存数据的超时时间(毫秒),默认值为30秒")]
    public int SaveDataBatchTimeOut { get; set; } = 30_000;

    /// <summary>
    /// Actor 执行任务超时时间(毫秒),默认值为30秒
    /// </summary>
    [Option(nameof(ActorTimeOut), Default = 30_000, HelpText = "Actor 执行任务超时时间(毫秒),默认值为30秒")]
    public int ActorTimeOut { get; set; } = 30_000;

    /// <summary>
    /// Actor 执行任务队列超时时间(毫秒),默认值为30秒
    /// </summary>
    [Option(nameof(ActorQueueTimeOut), Default = 30_000, HelpText = "Actor 执行任务队列超时时间(毫秒),默认值为30秒")]
    public int ActorQueueTimeOut { get; set; } = 30_000;

    /// <summary>
    /// 内部IP
    /// </summary>
    [Option(nameof(InnerIp), Default = "0.0.0.0", HelpText = "内部IP")]
    // [GrafanaLokiLabelTag]
    public string InnerIp { get; set; }

    /// <summary>
    /// 内部端口
    /// </summary>
    [Option(nameof(InnerPort), HelpText = "内部端口")]
    // [GrafanaLokiLabelTag]
    public ushort InnerPort { get; set; }

    /// <summary>
    /// 外部IP
    /// </summary>
    [Option(nameof(OuterIp), Default = "0.0.0.0", HelpText = "外部IP")]
    // [GrafanaLokiLabelTag]
    public string OuterIp { get; set; }

    /// <summary>
    /// 外部端口
    /// </summary>
    [Option(nameof(OuterPort), HelpText = "外部端口")]
    // [GrafanaLokiLabelTag]
    public ushort OuterPort { get; set; }

    /// <summary>
    /// 雪花ID的工作ID
    /// </summary>
    [Option(nameof(WorkerId), Default = 0, HelpText = "雪花ID的工作ID,默认为0,表示自动分配")]
    public ushort WorkerId { get; set; }

    /// <summary>
    /// API接口根路径,必须以/开头和以/结尾,默认为[/game/api/]
    /// </summary>
    [Option(nameof(HttpUrl), Default = "/game/api/", HelpText = "API接口根路径,必须以/开头和以/结尾,默认为[/game/api/]")]
    public string HttpUrl { get; set; }

    /// <summary>
    /// HTTP 是否是开发模式,当是开发模式的时候将会启用Swagger
    /// </summary>
    [Option(nameof(HttpIsDevelopment), Default = false, HelpText = "HTTP 是否是开发模式,当是开发模式的时候将会启用Swagger")]
    // [GrafanaLokiLabelTag]
    public bool HttpIsDevelopment { get; set; }

    /// <summary>
    /// HTTP 端口
    /// </summary>
    [Option(nameof(HttpPort), Default = 28080, HelpText = "HTTP 端口")]
    public ushort HttpPort { get; set; }

    /// <summary>
    /// HTTPS 端口
    /// </summary>
    [Option(nameof(HttpsPort), HelpText = "HTTPS 端口")]
    public ushort HttpsPort { get; set; }

    /// <summary>
    /// WebSocket 端口
    /// </summary>
    [Option(nameof(WsPort), HelpText = "WebSocket 端口")]
    public ushort WsPort { get; set; }

    /// <summary>
    /// 游戏逻辑服务器的处理最小模块ID
    /// </summary>
    [Option(nameof(MinModuleId), HelpText = "游戏逻辑服务器的处理最小模块ID")]
    public short MinModuleId { get; set; }

    /// <summary>
    /// 游戏逻辑服务器的处理最大模块ID
    /// </summary>
    [Option(nameof(MaxModuleId), HelpText = "游戏逻辑服务器的处理最大模块ID")]
    public short MaxModuleId { get; set; }

    /// <summary>
    /// WebSocket 加密端口
    /// </summary>
    [Option(nameof(WssPort), HelpText = "WebSocket 加密端口")]
    public ushort WssPort { get; set; }

    /// <summary>
    /// Wss 使用的证书路径
    /// </summary>
    [Option(nameof(WssCertFilePath), HelpText = "Wss 使用的证书路径")]
    public string WssCertFilePath { get; set; }

    /// <summary>
    /// 数据库 地址
    /// </summary>
    [Option(nameof(DataBaseUrl), HelpText = "数据库 地址")]
    public string DataBaseUrl { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    [Option(nameof(DataBaseName), HelpText = "数据库名称")]
    public string DataBaseName { get; set; }

    /// <summary>
    /// 语言
    /// </summary>
    [Option(nameof(Language), HelpText = "语言")]
    [GrafanaLokiLabelTag]
    public string Language { get; set; }

    /// <summary>
    /// 数据中心
    /// </summary>
    [Option(nameof(DataCenter), HelpText = "数据中心")]
    public string DataCenter { get; set; }

    /// <summary>
    /// 发现中心地址
    /// </summary>
    [Option(nameof(DiscoveryCenterIp), HelpText = "发现中心地址")]
    public string DiscoveryCenterIp { get; set; }

    /// <summary>
    /// 发现中心端口
    /// </summary>
    [Option(nameof(DiscoveryCenterPort), HelpText = "发现中心端口")]
    public ushort DiscoveryCenterPort { get; set; }

    /// <summary>
    /// 数据库服务连接地址
    /// </summary>
    [Option(nameof(DBIp), HelpText = "数据库服务连接地址")]
    public string DBIp { get; set; }

    /// <summary>
    /// 数据库服务连接端口
    /// </summary>
    [Option(nameof(DBPort), HelpText = "数据库服务连接端口")]
    public ushort DBPort { get; set; }

    /// <summary>
    /// 标签名称
    /// </summary>
    [Option(nameof(TagName), Default = "", HelpText = "标签名称")]
    [GrafanaLokiLabelTag]
    public string TagName { get; set; }

    /// <summary>
    /// 描述信息
    /// </summary>
    [Option(nameof(Description), Default = "", HelpText = "描述信息")]
    [GrafanaLokiLabelTag]
    public string Description { get; set; }

    /// <summary>
    /// 备注信息
    /// </summary>
    [Option(nameof(Note), Default = "", HelpText = "备注信息")]
    [GrafanaLokiLabelTag]
    public string Note { get; set; }

    /// <summary>
    /// 标签信息
    /// </summary>
    [Option(nameof(Label), Default = "", HelpText = "标签信息")]
    [GrafanaLokiLabelTag]
    public string Label { get; set; }

    /// <summary>
    /// 检查APM监控端口
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal void CheckApmPort()
    {
        if (APMPort <= 10000 || APMPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(APMPort), "APMPort必须大于10000且小于等于65535");
        }
    }

    /// <summary>
    /// 检查ServerId
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal void CheckServerId()
    {
        if (ServerId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ServerId), "ServerId必须大于0");
        }
    }

    /// <summary>
    /// 检查InnerIp
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    internal void CheckInnerIp()
    {
        if (string.IsNullOrWhiteSpace(InnerIp))
        {
            throw new ArgumentNullException(nameof(InnerIp), "内部IP不能为空");
        }
    }

    /// <summary>
    /// 检查内部端口
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal void CheckInnerPort()
    {
        if (InnerPort <= 10000 || InnerPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(InnerPort), "内部端口必须大于10000且小于等于65535");
        }
    }

    /// <summary>
    /// 检查OuterIp
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    internal void CheckOuterIp()
    {
        if (string.IsNullOrWhiteSpace(OuterIp))
        {
            throw new ArgumentNullException(nameof(OuterIp), "外部IP不能为空");
        }
    }

    /// <summary>
    /// 检查外部端口
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal void CheckOuterPort()
    {
        if (OuterPort <= 10000 || OuterPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(OuterPort), "外部端口必须大于10000且小于等于65535");
        }
    }

    /// <summary>
    /// 检查HttpUrl
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    internal void CheckHttpUrl()
    {
        if (HttpUrl.IsNullOrEmptyOrWhiteSpace())
        {
            throw new ArgumentNullException(nameof(HttpUrl), "Http 地址不能为空");
        }

        // 根路径必须以/开头和以/结尾
        if (!HttpUrl.StartsWith('/'))
        {
            throw new ArgumentException(nameof(HttpUrl), $"Http 地址必须以/开头");
        }

        if (!HttpUrl.EndsWith('/'))
        {
            throw new ArgumentException(nameof(HttpUrl), $"Http 地址必须以/结尾");
        }
    }

    /// <summary>
    /// 检查HttpPort
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal void CheckHttpPort()
    {
        if (HttpPort <= 10000 || HttpPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(HttpPort), "Http 端口必须大于10000且小于等于65535");
        }
    }

    /// <summary>
    /// 检查HttpsPort
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal void CheckHttpsPort()
    {
        if (HttpsPort <= 10000 || HttpsPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(HttpsPort), "Https 端口必须大于10000且小于等于65535");
        }
    }

    /// <summary>
    /// 检查WsPort
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal void CheckWsPort()
    {
        if (WsPort <= 10000 || WsPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(WsPort), "Ws 端口必须大于10000且小于等于65535");
        }
    }

    /// <summary>
    /// 检查MinModuleId
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal void CheckMinModuleId()
    {
        if (MinModuleId <= 0 || MinModuleId >= short.MaxValue - 10)
        {
            throw new ArgumentOutOfRangeException(nameof(MinModuleId), "游戏逻辑服务器的处理最小模块ID必须大于0且小于等于" + (short.MaxValue - 10));
        }
    }

    /// <summary>
    /// 检查MaxModuleId
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal void CheckMaxModuleId()
    {
        if (MaxModuleId <= 0 || MaxModuleId >= short.MaxValue - 10)
        {
            throw new ArgumentOutOfRangeException(nameof(MaxModuleId), "游戏逻辑服务器的处理最小模块ID必须大于0且小于等于" + (short.MaxValue - 10));
        }
    }

    /// <summary>
    /// 检查WssPort
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal void CheckWssPort()
    {
        if (WssPort <= 10000 || WssPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(WssPort), "Wss 端口必须大于10000且小于等于65535");
        }
    }

    /// <summary>
    /// 检查WssCertFilePath
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    internal void CheckWssCertFilePath()
    {
        if (string.IsNullOrWhiteSpace(WssCertFilePath))
        {
            throw new ArgumentNullException(nameof(WssCertFilePath), "Wss 使用的证书路径不能为空");
        }
    }

    /// <summary>
    /// 检查DataBaseUrl
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    internal void CheckDataBaseUrl()
    {
        if (string.IsNullOrWhiteSpace(DataBaseUrl))
        {
            throw new ArgumentNullException(nameof(DataBaseUrl), "数据库 地址不能为空");
        }
    }

    /// <summary>
    /// 检查DataBaseName
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    internal void CheckDataBaseName()
    {
        if (string.IsNullOrWhiteSpace(DataBaseName))
        {
            throw new ArgumentNullException(nameof(DataBaseName), "数据库名称不能为空");
        }
    }

    /// <summary>
    /// 检查DiscoveryCenterIp
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    internal void CheckDiscoveryCenterIp()
    {
        if (string.IsNullOrWhiteSpace(DiscoveryCenterIp))
        {
            throw new ArgumentNullException(nameof(DiscoveryCenterIp), "发现中心地址不能为空");
        }
    }

    /// <summary>
    /// 检查DiscoveryCenterPort
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal void CheckDiscoveryCenterPort()
    {
        if (DiscoveryCenterPort <= 10000 || DiscoveryCenterPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(DiscoveryCenterPort), "发现中心端口必须大于10000且小于等于65535");
        }
    }

    /// <summary>
    /// 检查DBIp
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    internal void CheckDbIp()
    {
        if (string.IsNullOrWhiteSpace(DBIp))
        {
            throw new ArgumentNullException(nameof(DBIp), "数据库服务连接地址不能为空");
        }
    }

    /// <summary>
    /// 检查数据库服务连接端口
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal void CheckDbPort()
    {
        if (DBPort <= 10000 || DBPort >= ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(DBPort), "数据库服务连接端口必须大于10000且小于等于65535");
        }
    }
}