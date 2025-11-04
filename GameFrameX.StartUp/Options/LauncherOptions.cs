// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using GameFrameX.Foundation.Options.Attributes;
using Serilog;
using Serilog.Events;

namespace GameFrameX.StartUp.Options;

/// <summary>
/// Launcher configuration options for GameFrameX server startup
/// </summary>
/// <remarks>
/// 启动参数配置类，包含服务器启动所需的各种配置选项
/// </remarks>
public sealed class LauncherOptions
{
    /// <summary>
    /// Gets or sets the server type
    /// </summary>
    /// <value>The type of server to be started</value>
    /// <remarks>
    /// 服务器类型，当该值无效时，默认为后续所有参数无效
    /// </remarks>
    [Option(nameof(ServerType), Required = true, Description = "服务器类型,当该值无效时,默认为后续所有参数无效")]
    [GrafanaLokiLabelTag]
    public string ServerType { get; set; }

    /// <summary>
    /// Gets or sets the metrics port number
    /// </summary>
    /// <value>The port number for metrics collection service</value>
    /// <remarks>
    /// 指标收集服务的端口号
    /// </remarks>
    [Option(nameof(MetricsPort), Description = "Metrics 端口")]
    public ushort MetricsPort { get; set; }

    /// <summary>
    /// Gets or sets whether OpenTelemetry metrics collection is enabled
    /// </summary>
    /// <value>True if metrics collection is enabled; otherwise, false. Default is false</value>
    /// <remarks>
    /// 是否启用指标收集功能，需要IsOpenTelemetry为true时有效。用于收集和监控应用程序的性能指标数据，默认值为false
    /// </remarks>
    [Option(nameof(IsOpenTelemetryMetrics), DefaultValue = false, Description = "是否启用分布式追踪功能,需要 IsOpenTelemetry 为true时有效,默认值为false")]
    public bool IsOpenTelemetryMetrics { get; set; }

    /// <summary>
    /// Gets or sets whether OpenTelemetry distributed tracing is enabled
    /// </summary>
    /// <value>True if distributed tracing is enabled; otherwise, false. Default is false</value>
    /// <remarks>
    /// 是否启用分布式追踪功能，需要IsOpenTelemetry为true时有效。用于跟踪和分析分布式系统中的请求流程，默认值为false
    /// </remarks>
    [Option(nameof(IsOpenTelemetryTracing), DefaultValue = false, Description = "是否启用分布式追踪功能,需要 IsOpenTelemetry为true时有效,默认值为false")]
    public bool IsOpenTelemetryTracing { get; set; }

    /// <summary>
    /// Gets or sets whether OpenTelemetry telemetry is enabled
    /// </summary>
    /// <value>True if OpenTelemetry telemetry is enabled; otherwise, false. Default is false</value>
    /// <remarks>
    /// 是否启用OpenTelemetry遥测功能。OpenTelemetry是一个开源的可观测性框架，启用后可以统一管理指标、追踪和日志等可观测性数据，默认值为false
    /// </remarks>
    [Option(nameof(IsOpenTelemetry), DefaultValue = false, Description = "是否启用OpenTelemetry遥测功能,默认值为false")]
    public bool IsOpenTelemetry { get; set; }

    /// <summary>
    /// Gets or sets whether message timeout monitoring is enabled
    /// </summary>
    /// <value>True if message timeout monitoring is enabled; otherwise, false. Default is false</value>
    /// <remarks>
    /// 是否监控打印超时日志，默认值为false
    /// </remarks>
    [Option(nameof(IsMonitorMessageTimeOut), DefaultValue = false, Description = "是否打印超时日志,默认值为false")]
    public bool IsMonitorMessageTimeOut { get; set; }

    /// <summary>
    /// Gets or sets the message timeout monitoring threshold in seconds
    /// </summary>
    /// <value>The timeout threshold in seconds. Default is 1</value>
    /// <remarks>
    /// 监控处理器超时时间（秒），默认值为1秒，只有IsMonitorMessageTimeOut为true时有效
    /// </remarks>
    [Option(nameof(MonitorMessageTimeOutSeconds), DefaultValue = 1, Description = "处理器超时时间（秒）,默认值为1秒,只有IsMonitorMessageTimeOut为true时有效")]
    public int MonitorMessageTimeOutSeconds { get; set; }

    /// <summary>
    /// Gets or sets the network send timeout in seconds
    /// </summary>
    /// <value>The network send timeout in seconds. Default is 5</value>
    /// <remarks>
    /// 网络发送等待超时时间（秒），默认值为5秒，最小值为1秒
    /// </remarks>
    [Option(nameof(NetWorkSendTimeOutSeconds), DefaultValue = 5, Description = "网络发送等待超时时间（秒）,默认值为5秒,最小值为1秒")]
    public int NetWorkSendTimeOutSeconds { get; set; }

    /// <summary>
    /// Gets or sets whether debug logging mode is enabled
    /// </summary>
    /// <value>True if debug logging mode is enabled; otherwise, false. Default is false</value>
    /// <remarks>
    /// 是否是Debug打印日志模式，默认值为false
    /// </remarks>
    [Option(nameof(IsDebug), DefaultValue = false, Description = "是否是Debug打印日志模式,默认值为false")]
    public bool IsDebug { get; set; }

    /// <summary>
    /// Gets or sets whether debug send data logging is enabled
    /// </summary>
    /// <value>True if debug send data logging is enabled; otherwise, false. Default is false</value>
    /// <remarks>
    /// 是否打印发送数据，只有在IsDebug为true时有效，默认值为false
    /// </remarks>
    [Option(nameof(IsDebugSend), DefaultValue = false, Description = "是否打印发送数据,只有在IsDebug为true时有效,默认值为false")]
    public bool IsDebugSend { get; set; }

    /// <summary>
    /// Gets or sets whether debug send heartbeat data logging is enabled
    /// </summary>
    /// <value>True if debug send heartbeat data logging is enabled; otherwise, false. Default is false</value>
    /// <remarks>
    /// 是否打印发送的心跳数据，只有在IsDebugSend为true时有效，默认值为false
    /// </remarks>
    [Option(nameof(IsDebugSendHeartBeat), DefaultValue = false, Description = "是否打印发送的心跳数据,只有在IsDebugSend为true时有效,默认值为false")]
    public bool IsDebugSendHeartBeat { get; set; }

    /// <summary>
    /// Gets or sets whether debug receive data logging is enabled
    /// </summary>
    /// <value>True if debug receive data logging is enabled; otherwise, false. Default is false</value>
    /// <remarks>
    /// 是否打印接收数据，只有在IsDebug为true时有效，默认值为false
    /// </remarks>
    [Option(nameof(IsDebugReceive), DefaultValue = false, Description = "是否打印接收数据,只有在IsDebug为true时有效,默认值为false")]
    public bool IsDebugReceive { get; set; }

    /// <summary>
    /// Gets or sets whether debug receive heartbeat data logging is enabled
    /// </summary>
    /// <value>True if debug receive heartbeat data logging is enabled; otherwise, false. Default is false</value>
    /// <remarks>
    /// 是否打印接收的心跳数据，只有在IsDebugReceive为true时有效，默认值为false
    /// </remarks>
    [Option(nameof(IsDebugReceiveHeartBeat), DefaultValue = false, Description = "是否打印接收的心跳数据,只有在IsDebugReceive为true时有效,默认值为false")]
    public bool IsDebugReceiveHeartBeat { get; set; }

    /// <summary>
    /// Gets or sets whether console logging is enabled
    /// </summary>
    /// <value>True if console logging is enabled; otherwise, false. Default is false</value>
    /// <remarks>
    /// 是否输出到控制台，默认为false。控制日志是否同时在控制台显示，便于开发调试
    /// </remarks>
    [Option(nameof(LogIsConsole), DefaultValue = false, Description = "是否输出到控制台,默认为 false。")]
    public bool LogIsConsole { get; set; } = false;

    /// <summary>
    /// Gets or sets whether GrafanaLoki logging is enabled
    /// </summary>
    /// <value>True if GrafanaLoki logging is enabled; otherwise, false. Default is false</value>
    /// <remarks>
    /// 是否输出到GrafanaLoki，默认为false
    /// </remarks>
    [Option(nameof(LogIsGrafanaLoki), DefaultValue = false, Description = "是否输出到 GrafanaLoki,默认为 false。")]
    public bool LogIsGrafanaLoki { get; set; }

    /// <summary>
    /// Gets or sets the GrafanaLoki service URL
    /// </summary>
    /// <value>The URL of the GrafanaLoki service. Default is "http://localhost:3100"</value>
    /// <remarks>
    /// GrafanaLoki服务地址，默认为http://localhost:3100。当LogIsGrafanaLoki为true时生效
    /// </remarks>
    [Option(nameof(LogGrafanaLokiUrl), DefaultValue = "http://localhost:3100", Description = "GrafanaLoki 服务地址,默认为 http://localhost:3100。当LogIsGrafanaLoki为true时生效。")]
    public string LogGrafanaLokiUrl { get; set; } = "http://localhost:3100";

    /// <summary>
    /// Gets or sets the GrafanaLoki username or email
    /// </summary>
    /// <value>The username or email for GrafanaLoki authentication</value>
    /// <remarks>
    /// GrafanaLoki用户名或Email，当LogIsGrafanaLoki为true时生效
    /// </remarks>
    [Option(nameof(LogGrafanaLokiUserName), Description = "GrafanaLoki 用户名或Email,当LogIsGrafanaLoki为true时生效。")]
    public string LogGrafanaLokiUserName { get; set; }

    /// <summary>
    /// Gets or sets the GrafanaLoki password
    /// </summary>
    /// <value>The password for GrafanaLoki authentication</value>
    /// <remarks>
    /// GrafanaLoki密码，当LogIsGrafanaLoki为true时生效
    /// </remarks>
    [Option(nameof(LogGrafanaLokiPassword), Description = "GrafanaLoki 密码,当LogIsGrafanaLoki为true时生效。")]
    public string LogGrafanaLokiPassword { get; set; }

    /// <summary>
    /// Gets or sets the log rolling interval
    /// </summary>
    /// <value>The interval for log file rolling. Default is RollingInterval.Day</value>
    /// <remarks>
    /// 日志滚动间隔，默认为每天（Day）。决定日志文件创建新文件的时间间隔，可以是小时、天、月等
    /// </remarks>
    [Option(nameof(LogRollingInterval), DefaultValue = RollingInterval.Day, Description = "日志滚动间隔,默认为每天(Day),日志滚动间隔(可选值：Minute[分], Hour[时], Day[天], Month[月], Year[年], Infinite[无限])")]
    public RollingInterval LogRollingInterval { get; set; } = RollingInterval.Day;

    /// <summary>
    /// Gets or sets the log event level
    /// </summary>
    /// <value>The minimum log event level to be recorded. Default is LogEventLevel.Debug</value>
    /// <remarks>
    /// 日志输出级别，默认为Debug。控制日志输出的最低级别，低于此级别的日志将不会被记录
    /// </remarks>
    [Option(nameof(LogEventLevel), DefaultValue = LogEventLevel.Debug, Description = "日志输出级别,默认为 Debug,日志级别(可选值：Verbose[详细], Debug[调试], Information[信息], Warning[警告], Error[错误], Fatal[致命])")]
    public LogEventLevel LogEventLevel { get; set; } = LogEventLevel.Debug;

    /// <summary>
    /// Gets or sets whether log file size limit is enabled
    /// </summary>
    /// <value>True if log file size limit is enabled; otherwise, false. Default is true</value>
    /// <remarks>
    /// 是否限制单个文件大小，默认为true。启用此选项可以防止单个日志文件过大
    /// </remarks>
    [Option(nameof(LogIsFileSizeLimit), DefaultValue = true, Description = "是否限制单个文件大小,默认为 true。")]
    public bool LogIsFileSizeLimit { get; set; } = true;

    /// <summary>
    /// Gets or sets the log file size limit in bytes
    /// </summary>
    /// <value>The maximum size of a single log file in bytes. Default is 104857600 (100MB)</value>
    /// <remarks>
    /// 日志单个文件大小限制，默认为100MB。当IsFileSizeLimit为true时有效。当日志文件达到此大小限制时，将创建新的日志文件继续写入
    /// </remarks>
    [Option(nameof(LogFileSizeLimitBytes), DefaultValue = 104857600, Description = "日志单个文件大小限制,默认为 100MB。当 LogIsFileSizeLimit 为 true 时有效。")]
    public int LogFileSizeLimitBytes { get; set; } = 104857600;

    /// <summary>
    /// Gets or sets the retained log file count limit
    /// </summary>
    /// <value>The maximum number of log files to retain. Default is 31</value>
    /// <remarks>
    /// 日志文件保留数量限制，默认为31个文件，即31天的日志文件。当设置值为null时不限制文件数量。用于控制历史日志文件的数量，防止占用过多磁盘空间
    /// </remarks>
    [Option(nameof(LogRetainedFileCountLimit), DefaultValue = 31, Description = "日志文件保留数量限制 默认为 31 个文件,即 31 天的日志文件")]
    public int LogRetainedFileCountLimit { get; set; } = 31;

    /// <summary>
    /// Gets or sets the server ID
    /// </summary>
    /// <value>The unique identifier for the server</value>
    /// <remarks>
    /// 服务器ID。如果需要合服，请确保不同服的ServerId一样，不然合服后数据会无法处理用户数据
    /// </remarks>
    [Option(nameof(ServerId), Description = "服务器ID-如果需要合服，请确保不同服的ServerId一样。不然合服后数据会无法处理用户数据")]
    [GrafanaLokiLabelTag]
    public int ServerId { get; set; }

    /// <summary>
    /// Gets or sets the server instance ID
    /// </summary>
    /// <value>The unique identifier for the server instance. Default is 0</value>
    /// <remarks>
    /// 服务器实例ID，用于区分同一服务器的不同实例，默认值为0，表示不区分
    /// </remarks>
    [Option(nameof(ServerInstanceId), Description = "服务器实例ID-用于区分同一服务器的不同实例")]
    [GrafanaLokiLabelTag]
    public long ServerInstanceId { get; set; }

    /// <summary>
    /// Gets or sets the data save interval in milliseconds
    /// </summary>
    /// <value>The interval for saving data in milliseconds. Default is 30000 (30 seconds)</value>
    /// <remarks>
    /// 保存数据间隔，单位毫秒，默认30000毫秒（30秒），最小值为5秒（5000毫秒）
    /// </remarks>
    [Option(nameof(SaveDataInterval), DefaultValue = 30_000, Description = "保存数据间隔,单位毫秒,默认300秒(5分钟),最小值为5秒(5000毫秒)")]
    public int SaveDataInterval { get; set; }

    /// <summary>
    /// Gets or sets the batch count for data saving
    /// </summary>
    /// <value>The number of data items to save in a single batch. Default is 500</value>
    /// <remarks>
    /// 保存数据的批量数量长度，默认为500
    /// </remarks>
    [Option(nameof(SaveDataBatchCount), DefaultValue = 500, Description = "保存数据的批量数量长度,默认为500")]
    public int SaveDataBatchCount { get; set; } = 500;

    /// <summary>
    /// Gets or sets the batch timeout for data saving in milliseconds
    /// </summary>
    /// <value>The timeout for batch data saving operations in milliseconds. Default is 30000</value>
    /// <remarks>
    /// 保存数据的超时时间（毫秒），默认值为30秒
    /// </remarks>
    [Option(nameof(SaveDataBatchTimeOut), DefaultValue = 30_000, Description = "保存数据的超时时间(毫秒),默认值为30秒")]
    public int SaveDataBatchTimeOut { get; set; } = 30_000;

    /// <summary>
    /// Gets or sets the Actor execution timeout in milliseconds
    /// </summary>
    /// <value>The timeout for Actor task execution in milliseconds. Default is 30000</value>
    /// <remarks>
    /// Actor执行任务超时时间（毫秒），默认值为30秒
    /// </remarks>
    [Option(nameof(ActorTimeOut), DefaultValue = 30_000, Description = "Actor 执行任务超时时间(毫秒),默认值为30秒")]
    public int ActorTimeOut { get; set; } = 30_000;

    /// <summary>
    /// Gets or sets the Actor queue timeout in milliseconds
    /// </summary>
    /// <value>The timeout for Actor task queue in milliseconds. Default is 30000</value>
    /// <remarks>
    /// Actor执行任务队列超时时间（毫秒），默认值为30秒
    /// </remarks>
    [Option(nameof(ActorQueueTimeOut), DefaultValue = 30_000, Description = "Actor 执行任务队列超时时间(毫秒),默认值为30秒")]
    public int ActorQueueTimeOut { get; set; } = 30_000;

    /// <summary>
    /// Gets or sets the Actor recycle time in minutes
    /// </summary>
    /// <value>The idle time before Actor recycling in minutes. Default is 15</value>
    /// <remarks>
    /// Actor空闲多久回收，单位分钟，默认值为15分钟，最小值为1分钟，小于1则强制设置为5分钟
    /// </remarks>
    [Option(nameof(ActorRecycleTime), DefaultValue = 15, Description = "Actor 空闲多久回收,单位分钟,默认值为15分钟,最小值为1分钟,小于1则强制设置为5分钟")]
    public int ActorRecycleTime { get; set; } = 15;

    /// <summary>
    /// Gets or sets whether TCP service is enabled
    /// </summary>
    /// <value>True if TCP service is enabled; otherwise, false. Default is true</value>
    /// <remarks>
    /// 是否启用TCP服务，默认值为true。开启后服务器将监听TCP端口，允许客户端通过TCP协议进行连接。默认值为true，即启用，因为健康检查需要通过TCP端口进行访问
    /// </remarks>
    [Option(nameof(IsEnableTcp), DefaultValue = true, Description = "是否启用 TCP 服务，默认值为 true")]
    public bool IsEnableTcp { get; set; } = true;

    /// <summary>
    /// Gets or sets whether UDP service is enabled
    /// </summary>
    /// <value>True if UDP service is enabled; otherwise, false. Default is false</value>
    /// <remarks>
    /// 是否启用UDP服务，默认值为false。开启后服务器将监听UDP端口，允许客户端通过UDP协议进行连接。默认值为false，即不启用，因为UDP协议是无连接的，不保证数据传输的可靠性
    /// </remarks>
    [Option(nameof(IsEnableUdp), DefaultValue = false, Description = "是否启用 UDP 服务，默认值为 false")]
    public bool IsEnableUdp { get; set; } = false;

    /// <summary>
    /// Gets or sets the inner host address
    /// </summary>
    /// <value>The IP address for internal network communication. Default is "0.0.0.0"</value>
    /// <remarks>
    /// 内部网络通信的IP地址，用于服务器间通信
    /// </remarks>
    [Option(nameof(InnerHost), DefaultValue = "0.0.0.0", Description = "内部IP")]
    // [GrafanaLokiLabelTag]
    public string InnerHost { get; set; }

    /// <summary>
    /// Gets or sets the inner port number
    /// </summary>
    /// <value>The port number for internal network communication. Default is 8888</value>
    /// <remarks>
    /// 内部网络通信端口，用于服务器间通信
    /// </remarks>
    [Option(nameof(InnerPort), DefaultValue = 8888, Description = "内部端口")]
    // [GrafanaLokiLabelTag]
    public ushort InnerPort { get; set; }

    /// <summary>
    /// Gets or sets the outer host address
    /// </summary>
    /// <value>The IP address for external client connections. Default is "0.0.0.0"</value>
    /// <remarks>
    /// 外部客户端连接的IP地址，用于客户端连接服务器
    /// </remarks>
    [Option(nameof(OuterHost), DefaultValue = "0.0.0.0", Description = "外部IP")]
    // [GrafanaLokiLabelTag]
    public string OuterHost { get; set; }

    /// <summary>
    /// Gets or sets the outer port number
    /// </summary>
    /// <value>The port number for external client connections</value>
    /// <remarks>
    /// 外部客户端连接端口，用于客户端连接服务器
    /// </remarks>
    [Option(nameof(OuterPort), Description = "外部端口")]
    // [GrafanaLokiLabelTag]
    public ushort OuterPort { get; set; }

    /// <summary>
    /// Gets or sets the worker ID for Snowflake ID generation
    /// </summary>
    /// <value>The worker ID used in Snowflake algorithm. Default is 1</value>
    /// <remarks>
    /// 雪花ID算法中的工作节点ID，用于生成唯一标识符，默认为1
    /// </remarks>
    [Option(nameof(WorkerId), DefaultValue = 1, Description = "雪花ID的工作ID,默认为1")]
    public ushort WorkerId { get; set; }

    /// <summary>
    /// Gets or sets the data center ID for Snowflake ID generation
    /// </summary>
    /// <value>The data center ID used in Snowflake algorithm. Default is 1</value>
    /// <remarks>
    /// 雪花ID算法中的数据中心ID，用于生成唯一标识符，默认为1
    /// </remarks>
    [Option(nameof(DataCenterId), DefaultValue = 1, Description = "雪花ID的数据中心ID,默认为1")]
    public ushort DataCenterId { get; set; }

    /// <summary>
    /// Gets or sets whether HTTP service is enabled
    /// </summary>
    /// <value>True if HTTP service is enabled; otherwise, false. Default is true</value>
    /// <remarks>
    /// 是否启用HTTP服务，开启后服务器将监听HTTP端口，允许客户端通过HTTP协议进行连接。默认值为true，即启用，因为健康检查需要通过HTTP端口进行访问
    /// </remarks>
    [Option(nameof(IsEnableHttp), DefaultValue = true, Description = "是否启用 HTTP 服务，默认值为 true")]
    public bool IsEnableHttp { get; set; } = true;

    /// <summary>
    /// Gets or sets whether HTTP is in development mode
    /// </summary>
    /// <value>True if HTTP is in development mode; otherwise, false. Default is false</value>
    /// <remarks>
    /// HTTP是否为开发模式，当为开发模式时将启用Swagger文档，默认值为false
    /// </remarks>
    [Option(nameof(HttpIsDevelopment), DefaultValue = false, Description = "HTTP 是否是开发模式,当是开发模式的时候将会启用Swagger")]
    // [GrafanaLokiLabelTag]
    public bool HttpIsDevelopment { get; set; }

    /// <summary>
    /// Gets or sets the API root path
    /// </summary>
    /// <value>The root path for API endpoints, must start and end with '/'. Default is "/game/api/"</value>
    /// <remarks>
    /// API接口根路径，必须以/开头和以/结尾，默认为/game/api/
    /// </remarks>
    [Option(nameof(HttpUrl), DefaultValue = "/game/api/", Description = "API接口根路径,必须以/开头和以/结尾,默认为[/game/api/]")]
    public string HttpUrl { get; set; }

    /// <summary>
    /// Gets or sets the HTTP port number
    /// </summary>
    /// <value>The port number for HTTP service. Default is 8080</value>
    /// <remarks>
    /// HTTP服务端口号，用于HTTP协议通信，默认为8080
    /// </remarks>
    [Option(nameof(HttpPort), DefaultValue = 8080, Description = "HTTP 端口")]
    public ushort HttpPort { get; set; }

    /// <summary>
    /// Gets or sets the HTTPS port number
    /// </summary>
    /// <value>The port number for HTTPS service</value>
    /// <remarks>
    /// HTTPS服务端口号，用于加密的HTTP协议通信
    /// </remarks>
    [Option(nameof(HttpsPort), Description = "HTTPS 端口")]
    public ushort HttpsPort { get; set; }

    /// <summary>
    /// Gets or sets whether WebSocket service is enabled
    /// </summary>
    /// <value>True if WebSocket service is enabled; otherwise, false. Default is false</value>
    /// <remarks>
    /// 是否启用WebSocket服务，开启后服务器将监听WebSocket端口，允许客户端通过WebSocket协议进行连接。默认值为false，即不启用
    /// </remarks>
    [Option(nameof(IsEnableWebSocket), DefaultValue = false, Description = "是否启用 WebSocket 服务，默认值为 false")]
    public bool IsEnableWebSocket { get; set; } = false;

    /// <summary>
    /// Gets or sets the WebSocket port number
    /// </summary>
    /// <value>The port number for WebSocket service. Default is 8889</value>
    /// <remarks>
    /// WebSocket服务端口号，默认值为8889，当IsEnableWebSocket为true时才会启用
    /// </remarks>
    [Option(nameof(WsPort), DefaultValue = 8889, Description = "WebSocket 端口，默认值为 8889，当 IsEnableWebSocket 为 true 时才会启用")]
    public ushort WsPort { get; set; }

    /// <summary>
    /// Gets or sets the minimum module ID for game logic server processing
    /// </summary>
    /// <value>The minimum module ID that this server will handle</value>
    /// <remarks>
    /// 游戏逻辑服务器处理的最小模块ID，用于模块分片
    /// </remarks>
    [Option(nameof(MinModuleId), Description = "游戏逻辑服务器的处理最小模块ID")]
    public short MinModuleId { get; set; }

    /// <summary>
    /// Gets or sets the maximum module ID for game logic server processing
    /// </summary>
    /// <value>The maximum module ID that this server will handle</value>
    /// <remarks>
    /// 游戏逻辑服务器处理的最大模块ID，用于模块分片
    /// </remarks>
    [Option(nameof(MaxModuleId), Description = "游戏逻辑服务器的处理最大模块ID")]
    public short MaxModuleId { get; set; }

    /// <summary>
    /// Gets or sets the WebSocket Secure (WSS) port number
    /// </summary>
    /// <value>The port number for encrypted WebSocket service</value>
    /// <remarks>
    /// WebSocket安全连接端口号，用于加密的WebSocket通信
    /// </remarks>
    [Option(nameof(WssPort), Description = "WebSocket 加密端口")]
    public ushort WssPort { get; set; }

    /// <summary>
    /// Gets or sets the certificate file path for WSS
    /// </summary>
    /// <value>The file path to the SSL certificate used by WSS</value>
    /// <remarks>
    /// WSS使用的SSL证书文件路径
    /// </remarks>
    [Option(nameof(WssCertFilePath), Description = "Wss 使用的证书路径")]
    public string WssCertFilePath { get; set; }

    /// <summary>
    /// Gets or sets the database connection URL
    /// </summary>
    /// <value>The connection string or URL for the database</value>
    /// <remarks>
    /// 数据库连接地址或连接字符串
    /// </remarks>
    [Option(nameof(DataBaseUrl), Description = "数据库 地址")]
    public string DataBaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the database name
    /// </summary>
    /// <value>The name of the database to connect to</value>
    /// <remarks>
    /// 要连接的数据库名称
    /// </remarks>
    [Option(nameof(DataBaseName), Description = "数据库名称")]
    public string DataBaseName { get; set; }

    /// <summary>
    /// Gets or sets the language setting
    /// </summary>
    /// <value>The language code for localization</value>
    /// <remarks>
    /// 语言设置，用于本地化和国际化
    /// </remarks>
    [Option(nameof(Language), Description = "语言")]
    [GrafanaLokiLabelTag]
    public string Language { get; set; }

    /// <summary>
    /// Gets or sets the data center identifier
    /// </summary>
    /// <value>The identifier of the data center where this server is located</value>
    /// <remarks>
    /// 数据中心标识符，用于标识服务器所在的数据中心
    /// </remarks>
    [Option(nameof(DataCenter), Description = "数据中心")]
    public string DataCenter { get; set; }

    /// <summary>
    /// Gets or sets the discovery center host address
    /// </summary>
    /// <value>The host address of the service discovery center</value>
    /// <remarks>
    /// 服务发现中心的主机地址，用于服务注册和发现
    /// </remarks>
    [Option(nameof(DiscoveryCenterHost), Description = "发现中心地址")]
    public string DiscoveryCenterHost { get; set; }

    /// <summary>
    /// Gets or sets the discovery center port number
    /// </summary>
    /// <value>The port number of the service discovery center</value>
    /// <remarks>
    /// 服务发现中心的端口号，用于服务注册和发现
    /// </remarks>
    [Option(nameof(DiscoveryCenterPort), Description = "发现中心端口")]
    public ushort DiscoveryCenterPort { get; set; }

    /// <summary>
    /// Gets or sets the tag name for environment differentiation
    /// </summary>
    /// <value>The tag name used to distinguish servers in different environments. Default is empty string</value>
    /// <remarks>
    /// 标签名称，用于区分不同环境的服务器，没有实际用途，只是方便运维管理，默认为空字符串
    /// </remarks>
    [Option(nameof(TagName), DefaultValue = "", Description = "标签名称-用于区分不同环境的服务器,没有实际用途,只是方便运维管理")]
    [GrafanaLokiLabelTag]
    public string TagName { get; set; }

    /// <summary>
    /// Gets or sets the description of the server's purpose
    /// </summary>
    /// <value>The description text explaining the server's purpose. Default is empty string</value>
    /// <remarks>
    /// 描述信息，用于描述该服务器的用途，没有实际用途，只是方便运维管理，默认为空字符串
    /// </remarks>
    [Option(nameof(Description), DefaultValue = "", Description = "描述信息-用于描述该服务器的用途,没有实际用途,只是方便运维管理")]
    [GrafanaLokiLabelTag]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the note information for the server
    /// </summary>
    /// <value>The note text for additional server information. Default is empty string</value>
    /// <remarks>
    /// 备注信息，用于描述该服务器的备注信息，没有实际用途，只是方便运维管理，默认为空字符串
    /// </remarks>
    [Option(nameof(Note), DefaultValue = "", Description = "备注信息-用于描述该服务器的备注信息,没有实际用途,只是方便运维管理")]
    [GrafanaLokiLabelTag]
    public string Note { get; set; }

    /// <summary>
    /// Gets or sets the label information for the server
    /// </summary>
    /// <value>The label text for server categorization. Default is empty string</value>
    /// <remarks>
    /// 标签信息，用于描述该服务器的标签信息，没有实际用途，只是方便运维管理，默认为空字符串
    /// </remarks>
    [Option(nameof(Label), DefaultValue = "", Description = "标签信息-用于描述该服务器的标签信息,没有实际用途,只是方便运维管理")]
    [GrafanaLokiLabelTag]
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets the client API host address
    /// </summary>
    /// <value>The host address for client API endpoints. Default is empty string</value>
    /// <remarks>
    /// 客户端API服务的主机地址，默认为空字符串
    /// </remarks>
    [Option(nameof(ClientApiHost), DefaultValue = "", Description = "客户端API地址")]
    public string ClientApiHost { get; set; }

    /// <summary>
    /// Gets or sets the Hub API host address
    /// </summary>
    /// <value>The host address for Hub API endpoints. Default is empty string</value>
    /// <remarks>
    /// Hub API服务的主机地址，默认为空字符串
    /// </remarks>
    [Option(nameof(HubApiHost), DefaultValue = "", Description = "HubAPI地址")]
    public string HubApiHost { get; set; }

    /// <summary>
    /// Gets or sets the heartbeat interval for GameApp client in milliseconds
    /// </summary>
    /// <value>The interval between heartbeat messages in milliseconds. Default is 5000</value>
    /// <remarks>
    /// GameApp客户端心跳间隔（毫秒），默认5000毫秒
    /// </remarks>
    [Option(nameof(GameAppClientHeartBeatInterval), DefaultValue = 5000, Description = "心跳间隔（毫秒），默认 5000 毫秒")]
    public int GameAppClientHeartBeatInterval { get; set; } = 5000;

    /// <summary>
    /// Gets or sets the connection delay for GameApp client in milliseconds
    /// </summary>
    /// <value>The delay before attempting connection in milliseconds. Default is 5000</value>
    /// <remarks>
    /// GameApp客户端连接延迟（毫秒），默认5000毫秒
    /// </remarks>
    [Option(nameof(GameAppClientConnectDelay), DefaultValue = 5000, Description = "连接延迟（毫秒），默认 5000 毫秒")]
    public int GameAppClientConnectDelay { get; set; } = 5000;

    /// <summary>
    /// Gets or sets the retry delay for GameApp client in milliseconds
    /// </summary>
    /// <value>The delay between retry attempts in milliseconds. Default is 5000</value>
    /// <remarks>
    /// GameApp客户端重试延迟（毫秒），默认5000毫秒
    /// </remarks>
    [Option(nameof(GameAppClientRetryDelay), DefaultValue = 5000, Description = "重试延迟（毫秒），默认 5000 毫秒")]
    public int GameAppClientRetryDelay { get; set; } = 5000;

    /// <summary>
    /// Gets or sets the maximum retry count for GameApp client
    /// </summary>
    /// <value>The maximum number of retry attempts. Default is -1 for unlimited retries</value>
    /// <remarks>
    /// GameApp客户端最大重试次数，默认-1表示无限重试
    /// </remarks>
    [Option(nameof(GameAppClientMaxRetryCount), DefaultValue = -1, Description = "最大重试次数，默认 -1 表示无限重试")]
    public int GameAppClientMaxRetryCount { get; set; } = -1;
}