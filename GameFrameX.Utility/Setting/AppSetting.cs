// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


using System.Text.Json.Serialization;
using GameFrameX.Foundation.Json;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Localization.Core;

namespace GameFrameX.Utility.Setting;

/// <summary>
/// 应用程序配置类
/// </summary>
/// <remarks>
/// Application configuration class containing server settings, network options, and runtime parameters.
/// </remarks>
public sealed class AppSetting
{
    /// <summary>
    /// 用于通知应用程序退出的任务完成源
    /// </summary>
    /// <remarks>
    /// Task completion source for notifying application exit.
    /// </remarks>
    [JsonIgnore] public readonly TaskCompletionSource<bool> AppExitSource = new();

    private bool _appRunning;
    private string _serverType;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <remarks>
    /// Constructor that initializes default values.
    /// </remarks>
    public AppSetting()
    {
#if DEBUG
        IsDebug = true;
        IsDebugReceive = true;
        IsDebugSend = true;
        IsDebugSendHeartBeat = true;
        IsDebugReceiveHeartBeat = true;
#endif
        LaunchTime = DateTime.UtcNow;
    }

    /// <summary>
    /// 获取应用程序退出的任务标记
    /// </summary>
    /// <remarks>
    /// Gets the task token for application exit.
    /// </remarks>
    [JsonIgnore]
    public Task<bool> AppExitToken
    {
        get { return AppExitSource.Task; }
    }

    /// <summary>
    /// 应用程序启动时间
    /// </summary>
    /// <remarks>
    /// Application launch time.
    /// </remarks>
    public DateTime LaunchTime { get; set; }

    /// <summary>
    /// 获取或设置应用程序是否正在运行
    /// </summary>
    /// <remarks>
    /// Gets or sets whether the application is running.
    /// </remarks>
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
                        LogHelper.Error<string>("AppSetting.AppRunning {value}", LocalizationService.GetString(Localization.Keys.Utility.Settings.AppAlreadyExited));
                    }

                    _appRunning = false;
                    return;
                }

                _appRunning = value;
                if (!value && !AppExitSource.Task.IsCompleted)
                {
                    LogHelper.Info<string>("AppSetting.AppRunning {value}", LocalizationService.GetString(Localization.Keys.Utility.AppSettings.SetAppRunningFalse));
                    AppExitSource.TrySetCanceled();
                }
            }
        }
    }

    /// <summary>
    /// 获取或设置服务器类型
    /// </summary>
    /// <remarks>
    /// Gets or sets the server type.
    /// </remarks>
    public string ServerType
    {
        get { return _serverType; }
        set
        {
            _serverType = value;
            ServerName = value;
        }
    }

    /// <summary>
    /// 判断指定的服务ID是否为本地服务
    /// </summary>
    /// <remarks>
    /// Determines whether the specified server ID is a local service.
    /// </remarks>
    /// <param name="serverId">服务ID / Server ID</param>
    /// <returns>返回是否是本地服务 / Returns true if it is a local service</returns>
    public bool IsLocal(int serverId)
    {
        return serverId == ServerId;
    }

    /// <summary>
    /// 将对象序列化为JSON字符串
    /// </summary>
    /// <remarks>
    /// Serializes the object to a JSON string.
    /// </remarks>
    /// <returns>JSON字符串 / JSON string</returns>
    public override string ToString()
    {
        return JsonHelper.Serialize(this);
    }

    /// <summary>
    /// 将对象序列化为格式化的JSON字符串
    /// </summary>
    /// <remarks>
    /// Serializes the object to a formatted JSON string.
    /// </remarks>
    /// <returns>格式化的JSON字符串 / Formatted JSON string</returns>
    public string ToFormatString()
    {
        return JsonHelper.SerializeFormat(this);
    }

    #region 从配置文件读取的属性

    /// <summary>
    /// 是否启用指标收集功能,需要IsOpenTelemetry为true时有效
    /// <para>用于收集和监控应用程序的性能指标数据</para>
    /// <para>默认值为false</para>
    /// </summary>
    /// <remarks>
    /// Whether to enable metrics collection, effective when IsOpenTelemetry is true.
    /// Used for collecting and monitoring application performance metrics data.
    /// Default value is false.
    /// </remarks>
    public bool IsOpenTelemetryMetrics { get; set; }

    /// <summary>
    /// 是否启用分布式追踪功能,需要IsOpenTelemetry为true时有效
    /// <para>用于跟踪和分析分布式系统中的请求流程</para>
    /// <para>默认值为false</para>
    /// </summary>
    /// <remarks>
    /// Whether to enable distributed tracing, effective when IsOpenTelemetry is true.
    /// Used for tracking and analyzing request flow in distributed systems.
    /// Default value is false.
    /// </remarks>
    public bool IsOpenTelemetryTracing { get; set; }

    /// <summary>
    /// 是否启用OpenTelemetry遥测功能
    /// <para>OpenTelemetry是一个开源的可观测性框架</para>
    /// <para>启用后可以统一管理指标、追踪和日志等可观测性数据</para>
    /// <para>默认值为false</para>
    /// </summary>
    /// <remarks>
    /// Whether to enable OpenTelemetry telemetry.
    /// OpenTelemetry is an open-source observability framework.
    /// When enabled, it provides unified management of metrics, traces, and logs.
    /// Default value is false.
    /// </remarks>
    public bool IsOpenTelemetry { get; set; }

    /// <summary>
    /// 是否是Debug打印日志模式,默认值为false
    /// </summary>
    /// <remarks>
    /// Whether to enable debug log mode. Default value is false.
    /// </remarks>
    public bool IsDebug { get; set; }

    /// <summary>
    /// 是否打印超时日志
    /// </summary>
    /// <remarks>
    /// Whether to print timeout logs.
    /// </remarks>
    public bool IsMonitorTimeOut { get; set; }

    /// <summary>
    /// 处理器超时时间（秒）,默认值为1秒
    /// </summary>
    /// <remarks>
    /// Handler timeout in seconds. Default value is 1 second.
    /// </remarks>
    public int MonitorTimeOutSeconds { get; set; }

    /// <summary>
    /// 网络发送等待超时时间（秒）,默认值为5秒
    /// </summary>
    /// <remarks>
    /// Network send timeout in seconds. Default value is 5 seconds.
    /// </remarks>
    public int NetWorkSendTimeOutSeconds { get; set; }

    /// <summary>
    /// 是否打印发送数据,只有在IsDebug为true时有效,默认值为false
    /// </summary>
    /// <remarks>
    /// Whether to print sent data, effective when IsDebug is true. Default value is false.
    /// </remarks>
    public bool IsDebugSend { get; set; }

    /// <summary>
    /// 是否打印发送的心跳数据,只有在IsDebugSend为true时有效,默认值为false
    /// </summary>
    /// <remarks>
    /// Whether to print sent heartbeat data, effective when IsDebugSend is true. Default value is false.
    /// </remarks>
    public bool IsDebugSendHeartBeat { get; set; }

    /// <summary>
    /// 是否打印接收数据,只有在IsDebug为true时有效,默认值为false
    /// </summary>
    /// <remarks>
    /// Whether to print received data, effective when IsDebug is true. Default value is false.
    /// </remarks>
    public bool IsDebugReceive { get; set; }

    /// <summary>
    /// 是否打印接收的心跳数据,只有在IsDebugReceive为true时有效,默认值为false
    /// </summary>
    /// <remarks>
    /// Whether to print received heartbeat data, effective when IsDebugReceive is true. Default value is false.
    /// </remarks>
    public bool IsDebugReceiveHeartBeat { get; set; }

    /// <summary>
    /// 是否启用HTTP调试日志总开关
    /// <para>只有在IsDebug为true时有效</para>
    /// <para>默认值为true</para>
    /// </summary>
    /// <remarks>
    /// Whether to enable HTTP debug log master switch.
    /// Effective when IsDebug is true. Default value is true.
    /// </remarks>
    public bool IsDebugHttp { get; set; } = true;

    /// <summary>
    /// 是否打印HTTP请求参数日志
    /// <para>包括请求方法和参数内容</para>
    /// <para>只有在IsDebugHttp为true时有效</para>
    /// <para>默认值为true</para>
    /// </summary>
    /// <remarks>
    /// Whether to print HTTP request parameter logs.
    /// Includes request method and parameter content.
    /// Effective when IsDebugHttp is true. Default value is true.
    /// </remarks>
    public bool IsDebugHttpRequest { get; set; } = true;

    /// <summary>
    /// 是否打印HTTP响应结果日志
    /// <para>在执行时间日志中包含结果内容</para>
    /// <para>只有在IsDebugHttp为true时有效</para>
    /// <para>默认值为true</para>
    /// </summary>
    /// <remarks>
    /// Whether to print HTTP response result logs.
    /// Includes result content in execution time logs.
    /// Effective when IsDebugHttp is true. Default value is true.
    /// </remarks>
    public bool IsDebugHttpResponse { get; set; } = true;

    /// <summary>
    /// 服务器ID
    /// </summary>
    /// <remarks>
    /// Server ID.
    /// </remarks>
    public int ServerId { get; set; }

    /// <summary>
    /// 服务器实例ID
    /// </summary>
    /// <remarks>
    /// Server instance ID.
    /// </remarks>
    public long ServerInstanceId { get; set; }

    /// <summary>
    /// 服务器名称
    /// </summary>
    /// <remarks>
    /// Server name.
    /// </remarks>
    public string ServerName { get; set; }

    /// <summary>
    /// 标记名称
    /// </summary>
    /// <remarks>
    /// Tag name.
    /// </remarks>
    public string TagName { get; set; }

    /// <summary>
    /// 保存数据的时间间隔（毫秒）
    /// </summary>
    /// <remarks>
    /// Data save interval in milliseconds. Default is 300,000 (5 minutes).
    /// </remarks>
    public int SaveDataInterval { get; set; } = 300_000;

    /// <summary>
    /// 保存数据的批量数量长度，默认为500
    /// </summary>
    /// <remarks>
    /// Batch count for saving data. Default is 500.
    /// </remarks>
    public int SaveDataBatchCount { get; set; } = 500;

    /// <summary>
    /// 保存数据的超时时间（毫秒）,默认值为30秒
    /// </summary>
    /// <remarks>
    /// Data save batch timeout in milliseconds. Default is 30,000 (30 seconds).
    /// </remarks>
    public int SaveDataBatchTimeOut { get; set; } = 30_000;

    /// <summary>
    /// Actor 执行任务超时时间（毫秒）,默认值为30秒
    /// </summary>
    /// <remarks>
    /// Actor task execution timeout in milliseconds. Default is 30,000 (30 seconds).
    /// </remarks>
    public int ActorTimeOut { get; set; } = 30_000;

    /// <summary>
    /// Actor 空闲多久回收,单位分钟,默认值为15分钟
    /// </summary>
    /// <remarks>
    /// Actor idle recycle time in minutes. Default is 15 minutes.
    /// </remarks>
    public int ActorRecycleTime { get; set; } = 15;

    /// <summary>
    /// Actor 执行任务队列超时时间（毫秒）,默认值为30秒
    /// </summary>
    /// <remarks>
    /// Actor queue timeout in milliseconds. Default is 30,000 (30 seconds).
    /// </remarks>
    public int ActorQueueTimeOut { get; set; } = 30_000;

    /// <summary>
    /// 是否启用TCP
    /// </summary>
    /// <remarks>
    /// Whether to enable TCP.
    /// </remarks>
    public bool IsEnableTcp { get; set; }

    /// <summary>
    /// 是否启用UDP
    /// </summary>
    /// <remarks>
    /// Whether to enable UDP. Default is false.
    /// </remarks>
    public bool IsEnableUdp { get; set; } = false;

    /// <summary>
    /// 是否启用KCP
    /// </summary>
    /// <remarks>
    /// Whether to enable KCP. Default is false.
    /// </remarks>
    public bool IsEnableKcp { get; set; } = false;

    /// <summary>
    /// KCP端口
    /// </summary>
    /// <remarks>
    /// KCP server port. Default is 0 (uses same port as TCP).
    /// </remarks>
    public int KcpPort { get; set; } = 0;

    /// <summary>
    /// 内部主机地址
    /// </summary>
    /// <remarks>
    /// Internal host address.
    /// </remarks>
    public string InnerHost { get; set; }

    /// <summary>
    /// 内部端口
    /// </summary>
    /// <remarks>
    /// Internal port.
    /// </remarks>
    public ushort InnerPort { get; set; }

    /// <summary>
    /// 雪花ID的工作ID
    /// </summary>
    /// <remarks>
    /// Snowflake ID worker ID.
    /// </remarks>
    public ushort WorkerId { get; set; }

    /// <summary>
    /// 雪花ID的数据中心ID
    /// </summary>
    /// <remarks>
    /// Snowflake ID data center ID.
    /// </remarks>
    public ushort DataCenterId { get; set; }

    /// <summary>
    /// 外部主机地址
    /// </summary>
    /// <remarks>
    /// External host address.
    /// </remarks>
    public string OuterHost { get; set; }

    /// <summary>
    /// 外部端口
    /// </summary>
    /// <remarks>
    /// External port.
    /// </remarks>
    public ushort OuterPort { get; set; }

    /// <summary>
    /// HTTP地址
    /// </summary>
    /// <remarks>
    /// HTTP URL path.
    /// </remarks>
    public string HttpUrl { get; set; }

    /// <summary>
    /// 是否启用 HTTP 服务
    /// </summary>
    /// <remarks>
    /// Whether to enable HTTP service.
    /// </remarks>
    public bool IsEnableHttp { get; set; }

    /// <summary>
    /// HTTP 是否是开发模式
    /// </summary>
    /// <remarks>
    /// Whether HTTP is in development mode.
    /// </remarks>
    public bool HttpIsDevelopment { get; set; }

    /// <summary>
    /// HTTP端口
    /// </summary>
    /// <remarks>
    /// HTTP port.
    /// </remarks>
    public ushort HttpPort { get; set; }

    /// <summary>
    /// HTTPS端口
    /// </summary>
    /// <remarks>
    /// HTTPS port.
    /// </remarks>
    public ushort HttpsPort { get; set; }

    /// <summary>
    /// Prometheus指标端口（如果为0则使用HTTP端口）
    /// </summary>
    /// <remarks>
    /// Prometheus metrics port (uses HTTP port if 0).
    /// </remarks>
    public ushort MetricsPort { get; set; }

    /// <summary>
    /// 是否启用 WebSocket 服务
    /// <para>开启后服务器将监听 WebSocket 端口，允许客户端通过 WebSocket 协议进行连接</para>
    /// <para>默认值为 false，即不启用</para>
    /// </summary>
    /// <remarks>
    /// Whether to enable WebSocket service.
    /// When enabled, the server will listen on WebSocket port for client connections.
    /// Default value is false.
    /// </remarks>
    public bool IsEnableWebSocket { get; set; } = false;

    /// <summary>
    /// WebSocket端口
    /// </summary>
    /// <remarks>
    /// WebSocket port.
    /// </remarks>
    public ushort WsPort { get; set; }

    /// <summary>
    /// WebSocket加密端口
    /// </summary>
    /// <remarks>
    /// WebSocket secure port.
    /// </remarks>
    public ushort WssPort { get; set; }

    /// <summary>
    /// Wss使用的证书路径
    /// </summary>
    /// <remarks>
    /// Certificate file path for WSS.
    /// </remarks>
    public string WssCertFilePath { get; set; }

    /// <summary>
    /// 数据库地址
    /// </summary>
    /// <remarks>
    /// Database URL.
    /// </remarks>
    public string DataBaseUrl { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    /// <remarks>
    /// Database name.
    /// </remarks>
    public string DataBaseName { get; set; }

    /// <summary>
    /// 是否使用时区时间记录
    /// <para>启用后数据库时间戳将包含时区偏移量</para>
    /// <para>默认值为 false</para>
    /// </summary>
    /// <remarks>
    /// Whether to use time zone for time recording.
    /// When enabled, database timestamps will include time zone offset.
    /// Default value is false.
    /// </remarks>
    public bool IsUseTimeZone { get; set; } = false;

    /// <summary>
    /// 语言
    /// </summary>
    /// <remarks>
    /// Language.
    /// </remarks>
    public string Language { get; set; }

    /// <summary>
    /// 数据中心
    /// </summary>
    /// <remarks>
    /// Data center.
    /// </remarks>
    public string DataCenter { get; set; }

    /// <summary>
    /// 最大客户端数量
    /// </summary>
    /// <remarks>
    /// Maximum client count. Default is 3000.
    /// </remarks>
    public int MaxClientCount { get; set; } = 3000;

    /// <summary>
    /// 游戏逻辑服务器的处理最小模块ID
    /// </summary>
    /// <remarks>
    /// Minimum module ID for game logic server.
    /// </remarks>
    public short MinModuleId { get; set; }

    /// <summary>
    /// 游戏逻辑服务器的处理最大模块ID
    /// </summary>
    /// <remarks>
    /// Maximum module ID for game logic server.
    /// </remarks>
    public short MaxModuleId { get; set; }

    /// <summary>
    /// 描述信息
    /// </summary>
    /// <remarks>
    /// Description.
    /// </remarks>
    public string Description { get; set; }

    /// <summary>
    /// 备注信息
    /// </summary>
    /// <remarks>
    /// Note.
    /// </remarks>
    public string Note { get; set; }

    /// <summary>
    /// 标签信息
    /// </summary>
    /// <remarks>
    /// Label.
    /// </remarks>
    public string Label { get; set; }

    /// <summary>
    /// 客户端API地址
    /// </summary>
    /// <remarks>
    /// Client API host.
    /// </remarks>
    public string ClientApiHost { get; set; }

    /// <summary>
    /// HubAPI地址
    /// </summary>
    /// <remarks>
    /// Hub API host.
    /// </remarks>
    public string HubApiHost { get; set; }

    #endregion
}
