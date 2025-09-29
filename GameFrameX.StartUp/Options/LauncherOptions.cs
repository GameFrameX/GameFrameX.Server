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

using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Options.Attributes;
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
    [Option(nameof(ServerType), Required = true, Description = "服务器类型,当该值无效时,默认为后续所有参数无效")]
    [GrafanaLokiLabelTag]
    public string ServerType { get; set; }

    /// <summary>
    /// Metrics 端口
    /// </summary>
    [Option(nameof(MetricsPort), Description = "Metrics 端口")]
    public ushort MetricsPort { get; set; }

    /// <summary>
    /// 是否启用指标收集功能,需要IsOpenTelemetry为true时有效
    /// <para>用于收集和监控应用程序的性能指标数据</para>
    /// <para>默认值为false</para>
    /// </summary>
    [Option(nameof(IsOpenTelemetryMetrics), Description = "是否启用分布式追踪功能,需要 IsOpenTelemetry 为true时有效,默认值为false")]
    [DefaultValue(false)]
    public bool IsOpenTelemetryMetrics { get; set; }

    /// <summary>
    /// 是否启用分布式追踪功能,需要IsOpenTelemetry为true时有效
    /// <para>用于跟踪和分析分布式系统中的请求流程</para>
    /// <para>默认值为false</para>
    /// </summary>
    [Option(nameof(IsOpenTelemetryTracing), Description = "是否启用分布式追踪功能,需要 IsOpenTelemetry为true时有效,默认值为false")]
    [DefaultValue(false)]
    public bool IsOpenTelemetryTracing { get; set; }

    /// <summary>
    /// 是否启用OpenTelemetry遥测功能
    /// <para>OpenTelemetry是一个开源的可观测性框架</para>
    /// <para>启用后可以统一管理指标、追踪和日志等可观测性数据</para>
    /// <para>默认值为false</para>
    /// </summary>
    [Option(nameof(IsOpenTelemetry), Description = "是否启用OpenTelemetry遥测功能,默认值为false")]
    [DefaultValue(false)]
    public bool IsOpenTelemetry { get; set; }

    /// <summary>
    /// 是否监控打印超时日志
    /// </summary>
    [Option(nameof(IsMonitorMessageTimeOut), Description = "是否打印超时日志,默认值为false")]
    [DefaultValue(false)]
    public bool IsMonitorMessageTimeOut { get; set; }

    /// <summary>
    /// 监控处理器超时时间（秒）,默认值为1秒,只有IsMonitorTimeOut为true时有效
    /// </summary>
    [Option(nameof(MonitorMessageTimeOutSeconds), Description = "处理器超时时间（秒）,默认值为1秒,只有IsMonitorMessageTimeOut为true时有效")]
    [DefaultValue(1)]
    public int MonitorMessageTimeOutSeconds { get; set; }

    /// <summary>
    /// 网络发送等待超时时间（秒）,默认值为5秒
    /// </summary>
    [Option(nameof(NetWorkSendTimeOutSeconds), Description = "网络发送等待超时时间（秒）,默认值为5秒,最小值为1秒")]
    [DefaultValue(5)]
    public int NetWorkSendTimeOutSeconds { get; set; }

    /// <summary>
    /// 是否是Debug打印日志模式,默认值为false
    /// </summary>
    [Option(nameof(IsDebug), Description = "是否是Debug打印日志模式,默认值为false")]
    [DefaultValue(false)]
    public bool IsDebug { get; set; }

    /// <summary>
    /// 是否打印发送数据,只有在IsDebug为true时有效,默认值为false
    /// </summary>
    [Option(nameof(IsDebugSend), Description = "是否打印发送数据,只有在IsDebug为true时有效,默认值为false")]
    [DefaultValue(false)]
    public bool IsDebugSend { get; set; }

    /// <summary>
    /// 是否打印发送的心跳数据,只有在IsDebugSend为true时有效,默认值为false
    /// </summary>
    [Option(nameof(IsDebugSendHeartBeat), Description = "是否打印发送的心跳数据,只有在IsDebugSend为true时有效,默认值为false")]
    [DefaultValue(false)]
    public bool IsDebugSendHeartBeat { get; set; }

    /// <summary>
    /// 是否打印接收数据,只有在IsDebug为true时有效,默认值为false
    /// </summary>
    [Option(nameof(IsDebugReceive), Description = "是否打印接收数据,只有在IsDebug为true时有效,默认值为false")]
    [DefaultValue(false)]
    public bool IsDebugReceive { get; set; }

    /// <summary>
    /// 是否打印接收的心跳数据,只有在IsDebugReceive为true时有效,默认值为false
    /// </summary>
    [Option(nameof(IsDebugReceiveHeartBeat), Description = "是否打印接收的心跳数据,只有在IsDebugReceive为true时有效,默认值为false")]
    [DefaultValue(false)]
    public bool IsDebugReceiveHeartBeat { get; set; }

    /// <summary>是否输出到控制台,默认为 true。</summary>
    /// <remarks>控制日志是否同时在控制台显示,便于开发调试。</remarks>
    [Option(nameof(LogGrafanaLokiUsername), Description = "是否输出到控制台,默认为 false。")]
    [DefaultValue(false)]
    public bool LogIsConsole { get; set; } = false;

    /// <summary>是否输出到 GrafanaLoki,默认为 false。</summary>
    [Option(nameof(LogIsGrafanaLoki), Description = "是否输出到 GrafanaLoki,默认为 false。")]
    [DefaultValue(false)]
    public bool LogIsGrafanaLoki { get; set; }

    /// <summary>GrafanaLoki 服务地址,默认为 http://localhost:3100。</summary>
    [Option(nameof(LogGrafanaLokiUrl), Description = "GrafanaLoki 服务地址,默认为 http://localhost:3100。当LogIsGrafanaLoki为true时生效。")]
    [DefaultValue("http://localhost:3100")]
    public string LogGrafanaLokiUrl { get; set; } = "http://localhost:3100";

    /// <summary>GrafanaLoki 用户名或Email</summary>
    [Option(nameof(LogGrafanaLokiUsername), Description = "GrafanaLoki 用户名或Email,当LogIsGrafanaLoki为true时生效。")]
    public string LogGrafanaLokiUsername { get; set; }

    /// <summary>GrafanaLoki 密码</summary>
    [Option(nameof(LogGrafanaLokiPassword), Description = "GrafanaLoki 密码,当LogIsGrafanaLoki为true时生效。")]
    public string LogGrafanaLokiPassword { get; set; }

    /// <summary>日志滚动间隔,默认为每天（Day）。</summary>
    /// <remarks>决定日志文件创建新文件的时间间隔,可以是小时、天、月等。</remarks>
    [Option(nameof(LogRollingInterval), Description = "日志滚动间隔,默认为每天(Day),日志滚动间隔(可选值：Minute[分], Hour[时], Day[天], Month[月], Year[年], Infinite[无限])")]
    [DefaultValue(RollingInterval.Day)]
    public RollingInterval LogRollingInterval { get; set; } = RollingInterval.Day;

    /// <summary>日志输出级别,默认为 Debug。</summary>
    /// <remarks>控制日志输出的最低级别,低于此级别的日志将不会被记录。</remarks>
    [Option(nameof(LogEventLevel), Description = "日志输出级别,默认为 Debug,日志级别(可选值：Verbose[详细], Debug[调试], Information[信息], Warning[警告], Error[错误], Fatal[致命])")]
    [DefaultValue(LogEventLevel.Debug)]
    public LogEventLevel LogEventLevel { get; set; } = LogEventLevel.Debug;

    /// <summary>是否限制单个文件大小,默认为 true。</summary>
    /// <remarks>启用此选项可以防止单个日志文件过大。</remarks>
    [Option(nameof(LogIsFileSizeLimit), Description = "是否限制单个文件大小,默认为 true。")]
    [DefaultValue(true)]
    public bool LogIsFileSizeLimit { get; set; } = true;

    /// <summary>
    /// 日志单个文件大小限制,默认为 100MB。
    /// 当 IsFileSizeLimit 为 true 时有效。
    /// </summary>
    /// <remarks>当日志文件达到此大小限制时,将创建新的日志文件继续写入。</remarks>
    [Option(nameof(LogFileSizeLimitBytes), Description = "日志单个文件大小限制,默认为 100MB。当 LogIsFileSizeLimit 为 true 时有效。")]
    [DefaultValue(104857600)]
    public int LogFileSizeLimitBytes { get; set; } = 104857600;

    /// <summary>
    /// 日志文件保留数量限制 默认为 31 个文件,即 31 天的日志文件
    /// 当 设置值为 null 时不限制文件数量
    /// </summary>
    /// <remarks>用于控制历史日志文件的数量,防止占用过多磁盘空间。</remarks>
    [Option(nameof(LogRetainedFileCountLimit), Description = "日志文件保留数量限制 默认为 31 个文件,即 31 天的日志文件")]
    [DefaultValue(31)]
    public int LogRetainedFileCountLimit { get; set; } = 31;

    /// <summary>
    /// 服务器ID-如果需要合服，请确保不同服的ServerId一样。不然合服后数据会无法处理用户数据
    /// </summary>
    [Option(nameof(ServerId), Description = "服务器ID-如果需要合服，请确保不同服的ServerId一样。不然合服后数据会无法处理用户数据")]
    [GrafanaLokiLabelTag]
    public int ServerId { get; set; }

    /// <summary>
    /// 服务器实例ID-用于区分同一服务器的不同实例,默认值为0,表示不区分
    /// </summary>
    [Option(nameof(ServerInstanceId), Description = "服务器实例ID-用于区分同一服务器的不同实例")]
    [GrafanaLokiLabelTag]
    public long ServerInstanceId { get; set; }

    /// <summary>
    /// 保存数据间隔,单位毫秒,默认300秒(5分钟),最小值为5秒(5000毫秒)
    /// </summary>
    [Option(nameof(SaveDataInterval), Description = "保存数据间隔,单位毫秒,默认300秒(5分钟),最小值为5秒(5000毫秒)")]
    [DefaultValue(300_000)]
    public int SaveDataInterval { get; set; }

    /// <summary>
    /// 保存数据的批量数量长度,默认为500
    /// </summary>
    [Option(nameof(SaveDataBatchCount), Description = "保存数据的批量数量长度,默认为500")]
    [DefaultValue(500)]
    public int SaveDataBatchCount { get; set; } = 500;

    /// <summary>
    /// 保存数据的超时时间(毫秒),默认值为30秒
    /// </summary>
    [Option(nameof(SaveDataBatchTimeOut), Description = "保存数据的超时时间(毫秒),默认值为30秒")]
    [DefaultValue(30_000)]
    public int SaveDataBatchTimeOut { get; set; } = 30_000;

    /// <summary>
    /// Actor 执行任务超时时间(毫秒),默认值为30秒
    /// </summary>
    [Option(nameof(ActorTimeOut), Description = "Actor 执行任务超时时间(毫秒),默认值为30秒")]
    [DefaultValue(30_000)]
    public int ActorTimeOut { get; set; } = 30_000;

    /// <summary>
    /// Actor 执行任务队列超时时间(毫秒),默认值为30秒
    /// </summary>
    [Option(nameof(ActorQueueTimeOut), Description = "Actor 执行任务队列超时时间(毫秒),默认值为30秒")]
    [DefaultValue(30_000)]
    public int ActorQueueTimeOut { get; set; } = 30_000;

    /// <summary>
    /// Actor 空闲多久回收,单位分钟,默认值为15分钟,最小值为1分钟,小于1则强制设置为5分钟
    /// </summary>
    [Option(nameof(ActorRecycleTime), Description = "Actor 空闲多久回收,单位分钟,默认值为15分钟,最小值为1分钟,小于1则强制设置为5分钟")]
    [DefaultValue(15)]
    public int ActorRecycleTime { get; set; } = 15;

    /// <summary>
    /// 内部主机地址
    /// </summary>
    [Option(nameof(InnerHost), Description = "内部IP")]
    [DefaultValue("0.0.0.0")]
    // [GrafanaLokiLabelTag]
    public string InnerHost { get; set; }

    /// <summary>
    /// 内部端口
    /// </summary>
    [Option(nameof(InnerPort), Description = "内部端口")]
    // [GrafanaLokiLabelTag]
    public ushort InnerPort { get; set; }

    /// <summary>
    /// 外部主机地址
    /// </summary>
    [Option(nameof(OuterHost), Description = "外部IP")]
    [DefaultValue("0.0.0.0")]
    // [GrafanaLokiLabelTag]
    public string OuterHost { get; set; }

    /// <summary>
    /// 外部端口
    /// </summary>
    [Option(nameof(OuterPort), Description = "外部端口")]
    // [GrafanaLokiLabelTag]
    public ushort OuterPort { get; set; }

    /// <summary>
    /// 雪花ID的工作ID
    /// </summary>
    [Option(nameof(WorkerId), Description = "雪花ID的工作ID,默认为0,表示自动分配")]
    public ushort WorkerId { get; set; }

    /// <summary>
    /// API接口根路径,必须以/开头和以/结尾,默认为[/game/api/]
    /// </summary>
    [Option(nameof(HttpUrl), Description = "API接口根路径,必须以/开头和以/结尾,默认为[/game/api/]")]
    [DefaultValue("/game/api/")]
    public string HttpUrl { get; set; }

    /// <summary>
    /// HTTP 是否是开发模式,当是开发模式的时候将会启用Swagger
    /// </summary>
    [Option(nameof(HttpIsDevelopment), Description = "HTTP 是否是开发模式,当是开发模式的时候将会启用Swagger")]
    [DefaultValue(false)]
    // [GrafanaLokiLabelTag]
    public bool HttpIsDevelopment { get; set; }

    /// <summary>
    /// HTTP 端口
    /// </summary>
    [Option(nameof(HttpPort), DefaultValue = 8080, Description = "HTTP 端口")]
    public ushort HttpPort { get; set; }

    /// <summary>
    /// HTTPS 端口
    /// </summary>
    [Option(nameof(HttpsPort), Description = "HTTPS 端口")]
    public ushort HttpsPort { get; set; }

    /// <summary>
    /// WebSocket 端口
    /// </summary>
    [Option(nameof(WsPort), Description = "WebSocket 端口")]
    public ushort WsPort { get; set; }

    /// <summary>
    /// 游戏逻辑服务器的处理最小模块ID
    /// </summary>
    [Option(nameof(MinModuleId), Description = "游戏逻辑服务器的处理最小模块ID")]
    public short MinModuleId { get; set; }

    /// <summary>
    /// 游戏逻辑服务器的处理最大模块ID
    /// </summary>
    [Option(nameof(MaxModuleId), Description = "游戏逻辑服务器的处理最大模块ID")]
    public short MaxModuleId { get; set; }

    /// <summary>
    /// WebSocket 加密端口
    /// </summary>
    [Option(nameof(WssPort), Description = "WebSocket 加密端口")]
    public ushort WssPort { get; set; }

    /// <summary>
    /// Wss 使用的证书路径
    /// </summary>
    [Option(nameof(WssCertFilePath), Description = "Wss 使用的证书路径")]
    public string WssCertFilePath { get; set; }

    /// <summary>
    /// 数据库 地址
    /// </summary>
    [Option(nameof(DataBaseUrl), Description = "数据库 地址")]
    public string DataBaseUrl { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    [Option(nameof(DataBaseName), Description = "数据库名称")]
    public string DataBaseName { get; set; }

    /// <summary>
    /// 语言
    /// </summary>
    [Option(nameof(Language), Description = "语言")]
    [GrafanaLokiLabelTag]
    public string Language { get; set; }

    /// <summary>
    /// 数据中心
    /// </summary>
    [Option(nameof(DataCenter), Description = "数据中心")]
    public string DataCenter { get; set; }

    /// <summary>
    /// 发现中心地址
    /// </summary>
    [Option(nameof(DiscoveryCenterHost), Description = "发现中心地址")]
    public string DiscoveryCenterHost { get; set; }

    /// <summary>
    /// 发现中心端口
    /// </summary>
    [Option(nameof(DiscoveryCenterPort), Description = "发现中心端口")]
    public ushort DiscoveryCenterPort { get; set; }

    /// <summary>
    /// 标签名称-用于区分不同环境的服务器,没有实际用途,只是方便运维管理
    /// </summary>
    [Option(nameof(TagName), Description = "标签名称-用于区分不同环境的服务器,没有实际用途,只是方便运维管理")]
    [DefaultValue("")]
    [GrafanaLokiLabelTag]
    public string TagName { get; set; }

    /// <summary>
    /// 描述信息-用于描述该服务器的用途,没有实际用途,只是方便运维管理
    /// </summary>
    [Option(nameof(Description), Description = "描述信息-用于描述该服务器的用途,没有实际用途,只是方便运维管理")]
    [DefaultValue("")]
    [GrafanaLokiLabelTag]
    public string Description { get; set; }

    /// <summary>
    /// 备注信息-用于描述该服务器的备注信息,没有实际用途,只是方便运维管理
    /// </summary>
    [Option(nameof(Note), Description = "备注信息-用于描述该服务器的备注信息,没有实际用途,只是方便运维管理")]
    [DefaultValue("")]
    [GrafanaLokiLabelTag]
    public string Note { get; set; }

    /// <summary>
    /// 标签信息-用于描述该服务器的标签信息,没有实际用途,只是方便运维管理
    /// </summary>
    [Option(nameof(Label), Description = "标签信息-用于描述该服务器的标签信息,没有实际用途,只是方便运维管理")]
    [DefaultValue("")]
    [GrafanaLokiLabelTag]
    public string Label { get; set; }

    /// <summary>
    /// 客户端API地址
    /// </summary>
    [Option(nameof(ClientApiHost), Description = "客户端API地址")]
    [DefaultValue("")]
    public string ClientApiHost { get; set; }

    /// <summary>
    /// HubAPI地址
    /// </summary>
    [Option(nameof(HubApiHost), Description = "HubAPI地址")]
    [DefaultValue("")]
    public string HubApiHost { get; set; }

    /// <summary>
    /// 心跳间隔（毫秒），默认 5000 毫秒
    /// </summary>
    [Option(nameof(GameAppClientHeartBeatInterval), Description = "心跳间隔（毫秒），默认 5000 毫秒")]
    [DefaultValue(5000)]
    public int GameAppClientHeartBeatInterval { get; set; } = 5000;

    /// <summary>
    /// 连接延迟（毫秒），默认 5000 毫秒
    /// </summary>
    [Option(nameof(GameAppClientConnectDelay), Description = "连接延迟（毫秒），默认 5000 毫秒")]
    [DefaultValue(5000)]
    public int GameAppClientConnectDelay { get; set; } = 5000;

    /// <summary>
    /// 重试延迟（毫秒），默认 5000 毫秒
    /// </summary>
    [Option(nameof(GameAppClientRetryDelay), Description = "重试延迟（毫秒），默认 5000 毫秒")]
    [DefaultValue(5000)]
    public int GameAppClientRetryDelay { get; set; } = 5000;

    /// <summary>
    /// 最大重试次数，默认 -1 表示无限重试
    /// </summary>
    [Option(nameof(GameAppClientMaxRetryCount), Description = "最大重试次数，默认 -1 表示无限重试")]
    [DefaultValue(-1)]
    public int GameAppClientMaxRetryCount { get; set; } = -1;
}