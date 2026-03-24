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
/// GameFrameX 服务器启动配置选项
/// </summary>
/// <remarks>
/// Startup configuration options for GameFrameX server startup, containing various configuration options required for host and server startup.
/// </remarks>
public class StartupOptions
{
    /// <summary>
    /// 服务器类型
    /// </summary>
    /// <value>要启动的服务器类型 / The type of server to be started</value>
    /// <remarks>
    /// Server type. When this value is invalid, all subsequent parameters are considered invalid by default.
    /// </remarks>
    [Option(nameof(ServerType), Description = "服务器类型,当该值无效时,默认为后续所有参数无效")]
    [GrafanaLokiLabelTag]
    public string ServerType { get; set; }

    /// <summary>
    /// 是否启用单进程模式。
    /// </summary>
    /// <value>如果启用单进程模式则为 <c>true</c>；否则为 <c>false</c>（多进程模式）。默认值为 <c>false</c> / <c>true</c> if single-process mode is enabled; otherwise, <c>false</c> (multi-process mode). Default is <c>false</c></value>
    /// <remarks>
    /// Whether to enable single-process mode. Default is <c>false</c> (multi-process mode).
    /// When <c>true</c>, only one service type will be started in the current process.
    /// When <c>false</c>, multiple service types will be orchestrated based on TopologyProfile.
    /// </remarks>
    [Option(nameof(IsSingleMode), DefaultValue = false, Description = "是否单进程模式,默认值为false(多进程)")]
    public bool IsSingleMode { get; set; }

    /// <summary>
    /// 多进程模式的拓扑配置名称。
    /// </summary>
    /// <value>拓扑模板名称。默认值为 "default" / The topology profile name. Default is "default"</value>
    /// <remarks>
    /// Topology profile name for multi-process mode. Default is "default".
    /// Supported values: "default" (Game + Social), "game" (Game only), "social" (Social only),
    /// or comma-separated list like "Game,Social,Gateway".
    /// Only effective when IsSingleMode is <c>false</c>.
    /// </remarks>
    [Option(nameof(TopologyProfile), DefaultValue = "default", Description = "多进程模式拓扑模板名称")]
    public string TopologyProfile { get; set; } = "default";

    /// <summary>
    /// 指标收集服务端口号
    /// </summary>
    /// <value>指标收集服务的端口号 / The port number for metrics collection service</value>
    /// <remarks>
    /// Port number for the metrics collection service.
    /// </remarks>
    [Option(nameof(MetricsPort), Description = "Metrics 端口")]
    public ushort MetricsPort { get; set; }

    /// <summary>
    /// 是否启用指标收集功能
    /// </summary>
    /// <value>如果启用指标收集则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>false</c> / <c>true</c> if metrics collection is enabled; otherwise, <c>false</c>. Default is <c>false</c></value>
    /// <remarks>
    /// Whether to enable metrics collection functionality. Requires IsOpenTelemetry to be <c>true</c> to take effect. Used for collecting and monitoring application performance metrics data. Default value is <c>false</c>.
    /// </remarks>
    [Option(nameof(IsOpenTelemetryMetrics), DefaultValue = false, Description = "是否启用分布式追踪功能,需要 IsOpenTelemetry 为true时有效,默认值为false")]
    public bool IsOpenTelemetryMetrics { get; set; }

    /// <summary>
    /// 是否启用分布式追踪功能
    /// </summary>
    /// <value>如果启用分布式追踪则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>false</c> / <c>true</c> if distributed tracing is enabled; otherwise, <c>false</c>. Default is <c>false</c></value>
    /// <remarks>
    /// Whether to enable distributed tracing functionality. Requires IsOpenTelemetry to be <c>true</c> to take effect. Used for tracking and analyzing request flow in distributed systems. Default value is <c>false</c>.
    /// </remarks>
    [Option(nameof(IsOpenTelemetryTracing), DefaultValue = false, Description = "是否启用分布式追踪功能,需要 IsOpenTelemetry为true时有效,默认值为false")]
    public bool IsOpenTelemetryTracing { get; set; }

    /// <summary>
    /// 是否启用 OpenTelemetry 遥测功能
    /// </summary>
    /// <value>如果启用 OpenTelemetry 遥测则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>false</c> / <c>true</c> if OpenTelemetry telemetry is enabled; otherwise, <c>false</c>. Default is <c>false</c></value>
    /// <remarks>
    /// Whether to enable OpenTelemetry telemetry functionality. OpenTelemetry is an open-source observability framework that, when enabled, can unify management of metrics, traces, and logs. Default value is <c>false</c>.
    /// </remarks>
    [Option(nameof(IsOpenTelemetry), DefaultValue = false, Description = "是否启用OpenTelemetry遥测功能,默认值为false")]
    public bool IsOpenTelemetry { get; set; }

    /// <summary>
    /// 是否监控打印超时日志
    /// </summary>
    /// <value>如果启用消息超时监控则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>false</c> / <c>true</c> if message timeout monitoring is enabled; otherwise, <c>false</c>. Default is <c>false</c></value>
    /// <remarks>
    /// Whether to monitor and print timeout logs. Default value is <c>false</c>.
    /// </remarks>
    [Option(nameof(IsMonitorMessageTimeOut), DefaultValue = false, Description = "是否打印超时日志,默认值为false")]
    public bool IsMonitorMessageTimeOut { get; set; }

    /// <summary>
    /// 监控处理器超时时间（秒）
    /// </summary>
    /// <value>超时阈值（秒）。默认值为 1 / The timeout threshold in seconds. Default is 1</value>
    /// <remarks>
    /// Monitor handler timeout in seconds. Default value is 1 second. Only effective when IsMonitorMessageTimeOut is <c>true</c>.
    /// </remarks>
    [Option(nameof(MonitorMessageTimeOutSeconds), DefaultValue = 1, Description = "处理器超时时间（秒）,默认值为1秒,只有IsMonitorMessageTimeOut为true时有效")]
    public int MonitorMessageTimeOutSeconds { get; set; }

    /// <summary>
    /// 网络发送等待超时时间（秒）
    /// </summary>
    /// <value>网络发送超时时间（秒）。默认值为 5 / The network send timeout in seconds. Default is 5</value>
    /// <remarks>
    /// Network send wait timeout in seconds. Default value is 5 seconds, minimum value is 1 second.
    /// </remarks>
    [Option(nameof(NetWorkSendTimeOutSeconds), DefaultValue = 5, Description = "网络发送等待超时时间（秒）,默认值为5秒,最小值为1秒")]
    public int NetWorkSendTimeOutSeconds { get; set; }

    /// <summary>
    /// 是否是 Debug 打印日志模式
    /// </summary>
    /// <value>如果启用调试日志模式则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>false</c> / <c>true</c> if debug logging mode is enabled; otherwise, <c>false</c>. Default is <c>false</c></value>
    /// <remarks>
    /// Whether to enable Debug log printing mode. Default value is <c>false</c>.
    /// </remarks>
    [Option(nameof(IsDebug), DefaultValue = false, Description = "是否是Debug打印日志模式,默认值为false")]
    public bool IsDebug { get; set; }

    /// <summary>
    /// 是否打印发送数据
    /// </summary>
    /// <value>如果启用发送数据调试日志则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>false</c> / <c>true</c> if debug send data logging is enabled; otherwise, <c>false</c>. Default is <c>false</c></value>
    /// <remarks>
    /// Whether to print send data. Only effective when IsDebug is <c>true</c>. Default value is <c>false</c>.
    /// </remarks>
    [Option(nameof(IsDebugSend), DefaultValue = false, Description = "是否打印发送数据,只有在IsDebug为true时有效,默认值为false")]
    public bool IsDebugSend { get; set; }

    /// <summary>
    /// 是否打印发送的心跳数据
    /// </summary>
    /// <value>如果启用发送心跳数据调试日志则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>false</c> / <c>true</c> if debug send heartbeat data logging is enabled; otherwise, <c>false</c>. Default is <c>false</c></value>
    /// <remarks>
    /// Whether to print sent heartbeat data. Only effective when IsDebugSend is <c>true</c>. Default value is <c>false</c>.
    /// </remarks>
    [Option(nameof(IsDebugSendHeartBeat), DefaultValue = false, Description = "是否打印发送的心跳数据,只有在IsDebugSend为true时有效,默认值为false")]
    public bool IsDebugSendHeartBeat { get; set; }

    /// <summary>
    /// 是否打印接收数据
    /// </summary>
    /// <value>如果启用接收数据调试日志则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>false</c> / <c>true</c> if debug receive data logging is enabled; otherwise, <c>false</c>. Default is <c>false</c></value>
    /// <remarks>
    /// Whether to print receive data. Only effective when IsDebug is <c>true</c>. Default value is <c>false</c>.
    /// </remarks>
    [Option(nameof(IsDebugReceive), DefaultValue = false, Description = "是否打印接收数据,只有在IsDebug为true时有效,默认值为false")]
    public bool IsDebugReceive { get; set; }

    /// <summary>
    /// 是否打印接收的心跳数据
    /// </summary>
    /// <value>如果启用接收心跳数据调试日志则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>false</c> / <c>true</c> if debug receive heartbeat data logging is enabled; otherwise, <c>false</c>. Default is <c>false</c></value>
    /// <remarks>
    /// Whether to print received heartbeat data. Only effective when IsDebugReceive is <c>true</c>. Default value is <c>false</c>.
    /// </remarks>
    [Option(nameof(IsDebugReceiveHeartBeat), DefaultValue = false, Description = "是否打印接收的心跳数据,只有在IsDebugReceive为true时有效,默认值为false")]
    public bool IsDebugReceiveHeartBeat { get; set; }

    /// <summary>
    /// 是否启用 HTTP 调试日志总开关
    /// </summary>
    /// <value>如果启用 HTTP 调试日志则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>true</c> / <c>true</c> if HTTP debug logging is enabled; otherwise, <c>false</c>. Default is <c>true</c></value>
    /// <remarks>
    /// Whether to enable HTTP debug logging master switch. Default value is <c>true</c>. When enabled, request and response logging can be controlled separately via IsDebugHttpRequest and IsDebugHttpResponse.
    /// </remarks>
    [Option(nameof(IsDebugHttp), DefaultValue = true, Description = "是否启用HTTP调试日志总开关,只有在IsDebug为true时有效,默认值为true")]
    public bool IsDebugHttp { get; set; } = true;

    /// <summary>
    /// 是否打印 HTTP 请求参数日志
    /// </summary>
    /// <value>如果启用 HTTP 请求参数日志则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>true</c> / <c>true</c> if HTTP request parameter logging is enabled; otherwise, <c>false</c>. Default is <c>true</c></value>
    /// <remarks>
    /// Whether to print HTTP request parameter logs, including request method and parameter content. Only effective when IsDebugHttp is <c>true</c>. Default value is <c>true</c>.
    /// </remarks>
    [Option(nameof(IsDebugHttpRequest), DefaultValue = true, Description = "是否打印HTTP请求参数日志,只有在IsDebugHttp为true时有效,默认值为true")]
    public bool IsDebugHttpRequest { get; set; } = true;

    /// <summary>
    /// 是否打印 HTTP 响应结果日志
    /// </summary>
    /// <value>如果启用 HTTP 响应结果日志则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>true</c> / <c>true</c> if HTTP response result logging is enabled; otherwise, <c>false</c>. Default is <c>true</c></value>
    /// <remarks>
    /// Whether to print HTTP response result logs, including result content in execution time logs. Only effective when IsDebugHttp is <c>true</c>. Default value is <c>true</c>.
    /// </remarks>
    [Option(nameof(IsDebugHttpResponse), DefaultValue = true, Description = "是否打印HTTP响应结果日志,只有在IsDebugHttp为true时有效,默认值为true")]
    public bool IsDebugHttpResponse { get; set; } = true;

    /// <summary>
    /// 服务器时区
    /// </summary>
    /// <value>服务器使用的时区标识符，默认值为 "Asia/Shanghai" / The time zone identifier used by the server. Default is "Asia/Shanghai"</value>
    /// <remarks>
    /// Server time zone setting for unified time handling logic, ensuring consistency of time-related functions such as logs and scheduled tasks.
    /// Supports standard IANA time zone database identifiers such as "Asia/Shanghai", "UTC", "America/New_York", etc.
    /// Default value is "Asia/Shanghai", suitable for servers deployed in mainland China.
    /// Changing this value will affect all internal time calculations, log timestamps, scheduled task trigger times, etc.
    /// </remarks>
    [Option(nameof(TimeZone), DefaultValue = "Asia/Shanghai", Description = "服务器时区设置，默认为 Asia/Shanghai，支持 IANA 时区数据库标准标识符")]
    public string TimeZone { get; set; } = "Asia/Shanghai";

    /// <summary>
    /// 是否启用自定义时区
    /// </summary>
    /// <value>如果启用自定义时区则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>false</c> / <c>true</c> if custom time zone is enabled; otherwise, <c>false</c>. Default is <c>false</c></value>
    /// <remarks>
    /// Whether to enable custom time zone setting. Default is <c>false</c>. When enabled, the time zone specified by the TimeZone property will be used.
    /// When disabled, the system default time zone is used.
    /// </remarks>
    [Option(nameof(IsUseTimeZone), DefaultValue = false, Description = "是否启用自定义时区设置，默认为 false，禁用时使用系统默认时区")]
    public bool IsUseTimeZone { get; set; } = false;

    /// <summary>
    /// 是否输出到控制台
    /// </summary>
    /// <value>如果启用控制台日志则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>true</c> / <c>true</c> if console logging is enabled; otherwise, <c>false</c>. Default is <c>true</c></value>
    /// <remarks>
    /// Whether to output to console. Default is <c>true</c>. Controls whether logs are displayed in the console for development and debugging convenience.
    /// </remarks>
    [Option(nameof(LogIsConsole), DefaultValue = true, Description = "是否输出到控制台,默认为 true。")]
    public bool LogIsConsole { get; set; } = true;

    /// <summary>
    /// 是否将日志输出到文件
    /// </summary>
    /// <value>如果启用日志文件写入则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>true</c> / <c>true</c> if log file writing is enabled; otherwise, <c>false</c>. Default is <c>true</c></value>
    /// <remarks>
    /// Whether to output logs to files. Default is <c>true</c>. When enabled, log information will be written to the local file system for long-term storage and subsequent analysis.
    /// This configuration is typically used together with log rolling and file size limit settings to control log file generation and management.
    /// It is recommended to keep this enabled in production environments to ensure critical log information is persistently recorded.
    /// </remarks>
    [Option(nameof(LogIsWriteToFile), DefaultValue = true, Description = "是否将日志输出到文件,默认为 true。")]
    public bool LogIsWriteToFile { get; set; } = true;

    /// <summary>
    /// 是否输出到 GrafanaLoki
    /// </summary>
    /// <value>如果启用 GrafanaLoki 日志则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>false</c> / <c>true</c> if GrafanaLoki logging is enabled; otherwise, <c>false</c>. Default is <c>false</c></value>
    /// <remarks>
    /// Whether to output to GrafanaLoki. Default is <c>false</c>.
    /// </remarks>
    [Option(nameof(LogIsGrafanaLoki), DefaultValue = false, Description = "是否输出到 GrafanaLoki,默认为 false。")]
    public bool LogIsGrafanaLoki { get; set; }

    /// <summary>
    /// GrafanaLoki 服务地址
    /// </summary>
    /// <value>GrafanaLoki 服务的 URL。默认值为 "http://localhost:3100" / The URL of the GrafanaLoki service. Default is "http://localhost:3100"</value>
    /// <remarks>
    /// GrafanaLoki service address. Default is http://localhost:3100. Effective when LogIsGrafanaLoki is <c>true</c>.
    /// </remarks>
    [Option(nameof(LogGrafanaLokiUrl), DefaultValue = "http://localhost:3100", Description = "GrafanaLoki 服务地址,默认为 http://localhost:3100。当LogIsGrafanaLoki为true时生效。")]
    public string LogGrafanaLokiUrl { get; set; } = "http://localhost:3100";

    /// <summary>
    /// GrafanaLoki 用户名或 Email
    /// </summary>
    /// <value>GrafanaLoki 认证的用户名或邮箱 / The username or email for GrafanaLoki authentication</value>
    /// <remarks>
    /// GrafanaLoki username or email. Effective when LogIsGrafanaLoki is <c>true</c>.
    /// </remarks>
    [Option(nameof(LogGrafanaLokiUserName), Description = "GrafanaLoki 用户名或Email,当LogIsGrafanaLoki为true时生效。")]
    public string LogGrafanaLokiUserName { get; set; }

    /// <summary>
    /// GrafanaLoki 密码
    /// </summary>
    /// <value>GrafanaLoki 认证的密码 / The password for GrafanaLoki authentication</value>
    /// <remarks>
    /// GrafanaLoki password. Effective when LogIsGrafanaLoki is <c>true</c>.
    /// </remarks>
    [Option(nameof(LogGrafanaLokiPassword), Description = "GrafanaLoki 密码,当LogIsGrafanaLoki为true时生效。")]
    public string LogGrafanaLokiPassword { get; set; }

    /// <summary>
    /// 日志滚动间隔
    /// </summary>
    /// <value>日志文件滚动间隔。默认值为 RollingInterval.Day / The interval for log file rolling. Default is RollingInterval.Day</value>
    /// <remarks>
    /// Log rolling interval. Default is Day. Determines the time interval for creating new log files, which can be hour, day, month, etc.
    /// </remarks>
    [Option(nameof(LogRollingInterval), DefaultValue = RollingInterval.Day, Description = "日志滚动间隔,默认为每天(Day),日志滚动间隔(可选值：Minute[分], Hour[时], Day[天], Month[月], Year[年], Infinite[无限])")]
    public RollingInterval LogRollingInterval { get; set; } = RollingInterval.Day;

    /// <summary>
    /// 日志输出级别
    /// </summary>
    /// <value>要记录的最低日志事件级别。默认值为 LogEventLevel.Debug / The minimum log event level to be recorded. Default is LogEventLevel.Debug</value>
    /// <remarks>
    /// Log output level. Default is Debug. Controls the minimum level of log output. Logs below this level will not be recorded.
    /// </remarks>
    [Option(nameof(LogEventLevel), DefaultValue = LogEventLevel.Debug, Description = "日志输出级别,默认为 Debug,日志级别(可选值：Verbose[详细], Debug[调试], Information[信息], Warning[警告], Error[错误], Fatal[致命])")]
    public LogEventLevel LogEventLevel { get; set; } = LogEventLevel.Debug;

    /// <summary>
    /// 是否限制单个日志文件大小
    /// </summary>
    /// <value>如果启用日志文件大小限制则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>true</c> / <c>true</c> if log file size limit is enabled; otherwise, <c>false</c>. Default is <c>true</c></value>
    /// <remarks>
    /// Whether to limit individual file size. Default is <c>true</c>. Enabling this option can prevent individual log files from becoming too large.
    /// </remarks>
    [Option(nameof(LogIsFileSizeLimit), DefaultValue = true, Description = "是否限制单个文件大小,默认为 true。")]
    public bool LogIsFileSizeLimit { get; set; } = true;

    /// <summary>
    /// 日志单个文件大小限制（字节）
    /// </summary>
    /// <value>单个日志文件的最大大小（字节）。默认值为 104857600（100MB） / The maximum size of a single log file in bytes. Default is 104857600 (100MB)</value>
    /// <remarks>
    /// Log file size limit. Default is 100MB. Effective when IsFileSizeLimit is <c>true</c>. When a log file reaches this size limit, a new log file will be created to continue writing.
    /// </remarks>
    [Option(nameof(LogFileSizeLimitBytes), DefaultValue = 104857600, Description = "日志单个文件大小限制,默认为 100MB。当 LogIsFileSizeLimit 为 true 时有效。")]
    public int LogFileSizeLimitBytes { get; set; } = 104857600;

    /// <summary>
    /// 日志文件保留数量限制
    /// </summary>
    /// <value>要保留的日志文件最大数量。默认值为 31 / The maximum number of log files to retain. Default is 31</value>
    /// <remarks>
    /// Log file retention count limit. Default is 31 files, representing 31 days of log files. When set to null, there is no file count limit. Used to control the number of historical log files to prevent excessive disk space usage.
    /// </remarks>
    [Option(nameof(LogRetainedFileCountLimit), DefaultValue = 31, Description = "日志文件保留数量限制 默认为 31 个文件,即 31 天的日志文件")]
    public int LogRetainedFileCountLimit { get; set; } = 31;

    /// <summary>
    /// 服务器 ID
    /// </summary>
    /// <value>服务器的唯一标识符 / The unique identifier for the server</value>
    /// <remarks>
    /// Server ID. If server merging is needed, ensure different servers have the same ServerId, otherwise user data cannot be processed after merging.
    /// </remarks>
    [Option(nameof(ServerId), Description = "服务器ID-如果需要合服，请确保不同服的ServerId一样。不然合服后数据会无法处理用户数据")]
    [GrafanaLokiLabelTag]
    public int ServerId { get; set; }

    /// <summary>
    /// 服务器实例 ID
    /// </summary>
    /// <value>服务器实例的唯一标识符。默认值为 0 / The unique identifier for the server instance. Default is 0</value>
    /// <remarks>
    /// Server instance ID, used to distinguish different instances of the same server. Default value is 0, meaning no distinction.
    /// </remarks>
    [Option(nameof(ServerInstanceId), Description = "服务器实例ID-用于区分同一服务器的不同实例")]
    [GrafanaLokiLabelTag]
    public long ServerInstanceId { get; set; }

    /// <summary>
    /// 保存数据间隔（毫秒）
    /// </summary>
    /// <value>保存数据的间隔（毫秒）。默认值为 30000（30 秒） / The interval for saving data in milliseconds. Default is 30000 (30 seconds)</value>
    /// <remarks>
    /// Data save interval in milliseconds. Default is 30000 milliseconds (30 seconds), minimum value is 5 seconds (5000 milliseconds).
    /// </remarks>
    [Option(nameof(SaveDataInterval), DefaultValue = 30_000, Description = "保存数据间隔,单位毫秒,默认30000毫秒(30秒),最小值为5秒(5000毫秒)")]
    public int SaveDataInterval { get; set; }

    /// <summary>
    /// 保存数据的批量数量
    /// </summary>
    /// <value>单批保存的数据项数量。默认值为 500 / The number of data items to save in a single batch. Default is 500</value>
    /// <remarks>
    /// Batch count length for saving data. Default is 500.
    /// </remarks>
    [Option(nameof(SaveDataBatchCount), DefaultValue = 500, Description = "保存数据的批量数量长度,默认为500")]
    public int SaveDataBatchCount { get; set; } = 500;

    /// <summary>
    /// 保存数据的超时时间（毫秒）
    /// </summary>
    /// <value>批量数据保存操作的超时时间（毫秒）。默认值为 30000 / The timeout for batch data saving operations in milliseconds. Default is 30000</value>
    /// <remarks>
    /// Timeout for saving data in milliseconds. Default value is 30 seconds.
    /// </remarks>
    [Option(nameof(SaveDataBatchTimeOut), DefaultValue = 30_000, Description = "保存数据的超时时间(毫秒),默认值为30秒")]
    public int SaveDataBatchTimeOut { get; set; } = 30_000;

    /// <summary>
    /// Actor 执行任务超时时间（毫秒）
    /// </summary>
    /// <value>Actor 任务执行的超时时间（毫秒）。默认值为 30000 / The timeout for Actor task execution in milliseconds. Default is 30000</value>
    /// <remarks>
    /// Actor task execution timeout in milliseconds. Default value is 30 seconds.
    /// </remarks>
    [Option(nameof(ActorTimeOut), DefaultValue = 30_000, Description = "Actor 执行任务超时时间(毫秒),默认值为30秒")]
    public int ActorTimeOut { get; set; } = 30_000;

    /// <summary>
    /// Actor 执行任务队列超时时间（毫秒）
    /// </summary>
    /// <value>Actor 任务队列的超时时间（毫秒）。默认值为 30000 / The timeout for Actor task queue in milliseconds. Default is 30000</value>
    /// <remarks>
    /// Actor task queue timeout in milliseconds. Default value is 30 seconds.
    /// </remarks>
    [Option(nameof(ActorQueueTimeOut), DefaultValue = 30_000, Description = "Actor 执行任务队列超时时间(毫秒),默认值为30秒")]
    public int ActorQueueTimeOut { get; set; } = 30_000;

    /// <summary>
    /// Actor 空闲回收时间（分钟）
    /// </summary>
    /// <value>Actor 回收前的空闲时间（分钟）。默认值为 15 / The idle time before Actor recycling in minutes. Default is 15</value>
    /// <remarks>
    /// How long an Actor is idle before recycling, in minutes. Default value is 15 minutes, minimum value is 1 minute. Values less than 1 will be forced to 5 minutes.
    /// </remarks>
    [Option(nameof(ActorRecycleTime), DefaultValue = 15, Description = "Actor 空闲多久回收,单位分钟,默认值为15分钟,最小值为1分钟,小于1则强制设置为5分钟")]
    public int ActorRecycleTime { get; set; } = 15;

    /// <summary>
    /// 是否启用 TCP 服务
    /// </summary>
    /// <value>如果启用 TCP 服务则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>true</c> / <c>true</c> if TCP service is enabled; otherwise, <c>false</c>. Default is <c>true</c></value>
    /// <remarks>
    /// Whether to enable TCP service. Default value is <c>true</c>. When enabled, the server will listen on the TCP port, allowing clients to connect via TCP protocol.
    /// Default is <c>true</c> (enabled) because health checks need to access via TCP port.
    /// </remarks>
    [Option(nameof(IsEnableTcp), DefaultValue = true, Description = "是否启用 TCP 服务，默认值为 true")]
    public bool IsEnableTcp { get; set; } = true;

    /// <summary>
    /// 是否启用 UDP 服务
    /// </summary>
    /// <value>如果启用 UDP 服务则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>false</c> / <c>true</c> if UDP service is enabled; otherwise, <c>false</c>. Default is <c>false</c></value>
    /// <remarks>
    /// Whether to enable UDP service. Default value is <c>false</c>. When enabled, the server will listen on the UDP port, allowing clients to connect via UDP protocol.
    /// Default is <c>false</c> (disabled) because UDP protocol is connectionless and does not guarantee reliable data transmission.
    /// </remarks>
    [Option(nameof(IsEnableUdp), DefaultValue = false, Description = "是否启用 UDP 服务，默认值为 false")]
    public bool IsEnableUdp { get; set; } = false;

    /// <summary>
    /// 内部网络通信 IP 地址
    /// </summary>
    /// <value>内部网络通信的 IP 地址。默认值为 "0.0.0.0" / The IP address for internal network communication. Default is "0.0.0.0"</value>
    /// <remarks>
    /// IP address for internal network communication, used for inter-server communication.
    /// </remarks>
    [Option(nameof(InnerHost), DefaultValue = "0.0.0.0", Description = "内部IP")]
    // [GrafanaLokiLabelTag]
    public string InnerHost { get; set; }

    /// <summary>
    /// 内部网络通信端口
    /// </summary>
    /// <value>内部网络通信的端口号。默认值为 8888 / The port number for internal network communication. Default is 8888</value>
    /// <remarks>
    /// Port for internal network communication, used for inter-server communication.
    /// </remarks>
    [Option(nameof(InnerPort), DefaultValue = 8888, Description = "内部端口")]
    // [GrafanaLokiLabelTag]
    public ushort InnerPort { get; set; }

    /// <summary>
    /// 外部客户端连接 IP 地址
    /// </summary>
    /// <value>外部客户端连接的 IP 地址。默认值为 "0.0.0.0" / The IP address for external client connections. Default is "0.0.0.0"</value>
    /// <remarks>
    /// IP address for external client connections, used for clients to connect to the server.
    /// </remarks>
    [Option(nameof(OuterHost), DefaultValue = "0.0.0.0", Description = "外部IP")]
    // [GrafanaLokiLabelTag]
    public string OuterHost { get; set; }

    /// <summary>
    /// 外部客户端连接端口
    /// </summary>
    /// <value>外部客户端连接的端口号 / The port number for external client connections</value>
    /// <remarks>
    /// Port for external client connections, used for clients to connect to the server.
    /// </remarks>
    [Option(nameof(OuterPort), Description = "外部端口")]
    // [GrafanaLokiLabelTag]
    public ushort OuterPort { get; set; }

    /// <summary>
    /// 雪花 ID 工作节点 ID
    /// </summary>
    /// <value>雪花算法中使用的工作节点 ID。默认值为 1 / The worker ID used in Snowflake algorithm. Default is 1</value>
    /// <remarks>
    /// Worker node ID in the Snowflake ID algorithm, used for generating unique identifiers. Default is 1.
    /// </remarks>
    [Option(nameof(WorkerId), DefaultValue = 1, Description = "雪花ID的工作ID,默认为1")]
    public ushort WorkerId { get; set; }

    /// <summary>
    /// 雪花 ID 数据中心 ID
    /// </summary>
    /// <value>雪花算法中使用的数据中心 ID。默认值为 1 / The data center ID used in Snowflake algorithm. Default is 1</value>
    /// <remarks>
    /// Data center ID in the Snowflake ID algorithm, used for generating unique identifiers. Default is 1.
    /// </remarks>
    [Option(nameof(DataCenterId), DefaultValue = 1, Description = "雪花ID的数据中心ID,默认为1")]
    public ushort DataCenterId { get; set; }

    /// <summary>
    /// 是否启用 HTTP 服务
    /// </summary>
    /// <value>如果启用 HTTP 服务则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>true</c> / <c>true</c> if HTTP service is enabled; otherwise, <c>false</c>. Default is <c>true</c></value>
    /// <remarks>
    /// Whether to enable HTTP service. When enabled, the server will listen on the HTTP port, allowing clients to connect via HTTP protocol.
    /// Default value is <c>true</c> (enabled) because health checks need to access via HTTP port.
    /// </remarks>
    [Option(nameof(IsEnableHttp), DefaultValue = true, Description = "是否启用 HTTP 服务，默认值为 true")]
    public bool IsEnableHttp { get; set; } = true;

    /// <summary>
    /// HTTP 是否为开发模式
    /// </summary>
    /// <value>如果 HTTP 为开发模式则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>false</c> / <c>true</c> if HTTP is in development mode; otherwise, <c>false</c>. Default is <c>false</c></value>
    /// <remarks>
    /// Whether HTTP is in development mode. When in development mode, Swagger documentation will be enabled. Default value is <c>false</c>.
    /// </remarks>
    [Option(nameof(HttpIsDevelopment), DefaultValue = false, Description = "HTTP 是否是开发模式,当是开发模式的时候将会启用Swagger")]
    // [GrafanaLokiLabelTag]
    public bool HttpIsDevelopment { get; set; }

    /// <summary>
    /// API 接口根路径
    /// </summary>
    /// <value>API 端点的根路径，必须以 / 开头和结尾。默认值为 "/game/api/" / The root path for API endpoints, must start and end with '/'. Default is "/game/api/"</value>
    /// <remarks>
    /// API interface root path, must start and end with /. Default is /game/api/.
    /// </remarks>
    [Option(nameof(HttpUrl), DefaultValue = "/game/api/", Description = "API接口根路径,必须以/开头和以/结尾,默认为[/game/api/]")]
    public string HttpUrl { get; set; }

    /// <summary>
    /// HTTP 服务端口号
    /// </summary>
    /// <value>HTTP 服务的端口号。默认值为 8080 / The port number for HTTP service. Default is 8080</value>
    /// <remarks>
    /// HTTP service port number, used for HTTP protocol communication. Default is 8080.
    /// </remarks>
    [Option(nameof(HttpPort), DefaultValue = 8080, Description = "HTTP 端口")]
    public ushort HttpPort { get; set; }

    /// <summary>
    /// HTTPS 服务端口号
    /// </summary>
    /// <value>HTTPS 服务的端口号 / The port number for HTTPS service</value>
    /// <remarks>
    /// HTTPS service port number, used for encrypted HTTP protocol communication.
    /// </remarks>
    [Option(nameof(HttpsPort), Description = "HTTPS 端口")]
    public ushort HttpsPort { get; set; }

    /// <summary>
    /// 是否启用 WebSocket 服务
    /// </summary>
    /// <value>如果启用 WebSocket 服务则为 <c>true</c>；否则为 <c>false</c>。默认值为 <c>false</c> / <c>true</c> if WebSocket service is enabled; otherwise, <c>false</c>. Default is <c>false</c></value>
    /// <remarks>
    /// Whether to enable WebSocket service. When enabled, the server will listen on the WebSocket port, allowing clients to connect via WebSocket protocol.
    /// Default value is <c>false</c> (disabled).
    /// </remarks>
    [Option(nameof(IsEnableWebSocket), DefaultValue = false, Description = "是否启用 WebSocket 服务，默认值为 false")]
    public bool IsEnableWebSocket { get; set; } = false;

    /// <summary>
    /// WebSocket 服务端口号
    /// </summary>
    /// <value>WebSocket 服务的端口号。默认值为 8889 / The port number for WebSocket service. Default is 8889</value>
    /// <remarks>
    /// WebSocket service port number. Default value is 8889. Only enabled when IsEnableWebSocket is <c>true</c>.
    /// </remarks>
    [Option(nameof(WsPort), DefaultValue = 8889, Description = "WebSocket 端口，默认值为 8889，当 IsEnableWebSocket 为 true 时才会启用")]
    public ushort WsPort { get; set; }

    /// <summary>
    /// 游戏逻辑服务器处理的最小模块 ID
    /// </summary>
    /// <value>此服务器将处理的最小模块 ID / The minimum module ID that this server will handle</value>
    /// <remarks>
    /// Minimum module ID for game logic server processing, used for module sharding.
    /// </remarks>
    [Option(nameof(MinModuleId), Description = "游戏逻辑服务器的处理最小模块ID")]
    public short MinModuleId { get; set; }

    /// <summary>
    /// 游戏逻辑服务器处理的最大模块 ID
    /// </summary>
    /// <value>此服务器将处理的最大模块 ID / The maximum module ID that this server will handle</value>
    /// <remarks>
    /// Maximum module ID for game logic server processing, used for module sharding.
    /// </remarks>
    [Option(nameof(MaxModuleId), Description = "游戏逻辑服务器的处理最大模块ID")]
    public short MaxModuleId { get; set; }

    /// <summary>
    /// WebSocket 安全连接端口号
    /// </summary>
    /// <value>加密 WebSocket 服务的端口号 / The port number for encrypted WebSocket service</value>
    /// <remarks>
    /// WebSocket Secure (WSS) port number, used for encrypted WebSocket communication.
    /// </remarks>
    [Option(nameof(WssPort), Description = "WebSocket 加密端口")]
    public ushort WssPort { get; set; }

    /// <summary>
    /// WSS 使用的 SSL 证书文件路径
    /// </summary>
    /// <value>WSS 使用的 SSL 证书文件路径 / The file path to the SSL certificate used by WSS</value>
    /// <remarks>
    /// SSL certificate file path used by WSS.
    /// </remarks>
    [Option(nameof(WssCertFilePath), Description = "Wss 使用的证书路径")]
    public string WssCertFilePath { get; set; }

    /// <summary>
    /// 数据库连接地址
    /// </summary>
    /// <value>数据库的连接字符串或 URL / The connection string or URL for the database</value>
    /// <remarks>
    /// Database connection address or connection string.
    /// </remarks>
    [Option(nameof(DataBaseUrl), Description = "数据库 地址")]
    public string DataBaseUrl { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    /// <value>要连接的数据库名称 / The name of the database to connect to</value>
    /// <remarks>
    /// Name of the database to connect to.
    /// </remarks>
    [Option(nameof(DataBaseName), Description = "数据库名称")]
    public string DataBaseName { get; set; }

    /// <summary>
    /// 数据库密码
    /// </summary>
    /// <value>数据库连接的密码 / The password for the database connection</value>
    /// <remarks>
    /// Password for the database name to connect to.
    /// </remarks>
    [Option(nameof(DataBasePassword), Description = "数据库密码")]
    public string DataBasePassword { get; set; }

    /// <summary>
    /// 语言设置
    /// </summary>
    /// <value>本地化的语言代码 / The language code for localization</value>
    /// <remarks>
    /// Language setting, used for localization and internationalization.
    /// </remarks>
    [Option(nameof(Language), Description = "语言")]
    [GrafanaLokiLabelTag]
    public string Language { get; set; }

    /// <summary>
    /// 数据中心标识符
    /// </summary>
    /// <value>此服务器所在的数据中心标识符 / The identifier of the data center where this server is located</value>
    /// <remarks>
    /// Data center identifier, used to identify the data center where the server is located.
    /// </remarks>
    [Option(nameof(DataCenter), Description = "数据中心")]
    public string DataCenter { get; set; }

    /// <summary>
    /// 标签名称
    /// </summary>
    /// <value>用于区分不同环境服务器的标签名称。默认值为空字符串 / The tag name used to distinguish servers in different environments. Default is empty string</value>
    /// <remarks>
    /// Tag name, used to distinguish servers in different environments. Has no actual use, just for convenient operations management. Default is empty string.
    /// </remarks>
    [Option(nameof(TagName), DefaultValue = "", Description = "标签名称-用于区分不同环境的服务器,没有实际用途,只是方便运维管理")]
    [GrafanaLokiLabelTag]
    public string TagName { get; set; }

    /// <summary>
    /// 描述信息
    /// </summary>
    /// <value>描述服务器用途的文本。默认值为空字符串 / The description text explaining the server's purpose. Default is empty string</value>
    /// <remarks>
    /// Description information, used to describe the purpose of this server. Has no actual use, just for convenient operations management. Default is empty string.
    /// </remarks>
    [Option(nameof(Description), DefaultValue = "", Description = "描述信息-用于描述该服务器的用途,没有实际用途,只是方便运维管理")]
    [GrafanaLokiLabelTag]
    public string Description { get; set; }

    /// <summary>
    /// 备注信息
    /// </summary>
    /// <value>服务器附加信息的备注文本。默认值为空字符串 / The note text for additional server information. Default is empty string</value>
    /// <remarks>
    /// Note information, used to describe the notes for this server. Has no actual use, just for convenient operations management. Default is empty string.
    /// </remarks>
    [Option(nameof(Note), DefaultValue = "", Description = "备注信息-用于描述该服务器的备注信息,没有实际用途,只是方便运维管理")]
    [GrafanaLokiLabelTag]
    public string Note { get; set; }

    /// <summary>
    /// 标签信息
    /// </summary>
    /// <value>用于服务器分类的标签文本。默认值为空字符串 / The label text for server categorization. Default is empty string</value>
    /// <remarks>
    /// Label information, used to describe the label information for this server. Has no actual use, just for convenient operations management. Default is empty string.
    /// </remarks>
    [Option(nameof(Label), DefaultValue = "", Description = "标签信息-用于描述该服务器的标签信息,没有实际用途,只是方便运维管理")]
    [GrafanaLokiLabelTag]
    public string Label { get; set; }

    /// <summary>
    /// 客户端 API 主机地址
    /// </summary>
    /// <value>客户端 API 端点的主机地址。默认值为空字符串 / The host address for client API endpoints. Default is empty string</value>
    /// <remarks>
    /// Host address for client API service. Default is empty string.
    /// </remarks>
    [Option(nameof(ClientApiHost), DefaultValue = "", Description = "客户端API地址")]
    public string ClientApiHost { get; set; }

    /// <summary>
    /// Hub API 主机地址
    /// </summary>
    /// <value>Hub API 端点的主机地址。默认值为空字符串 / The host address for Hub API endpoints. Default is empty string</value>
    /// <remarks>
    /// Host address for Hub API service. Default is empty string.
    /// </remarks>
    [Option(nameof(HubApiHost), DefaultValue = "", Description = "HubAPI地址")]
    public string HubApiHost { get; set; }

}
