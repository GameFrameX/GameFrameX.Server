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

namespace GameFrameX.Localization;

/// <summary>
/// 本地化键常量定义 - StartUp 分部类
/// </summary>
public static partial class Keys
{
    /// <summary>
    /// StartUp模块相关日志和错误消息资源键
    /// </summary>
    public static class StartUp
    {
        /// <summary>
        /// 服务器类型:{0} 停止! 终止原因：{1} 配置信息: {2}
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.ServerStopped
        /// 用途: 当服务器停止时记录日志信息
        /// 参数: {0} - 服务器类型, {1} - 终止原因, {2} - 配置信息
        /// </remarks>
        public const string ServerStopped = "StartUp.ServerStopped";

        /// <summary>
        /// 服务器{0}与发现中心通信发生错误 ，id:{1}，e:{2}
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.DiscoveryCenterCommunicationError
        /// 用途: 当与发现中心通信发生错误时记录
        /// 参数: {0} - 服务器类型, {1} - 连接ID, {2} - 异常信息
        /// </remarks>
        public const string DiscoveryCenterCommunicationError = "StartUp.DiscoveryCenterCommunicationError";

        /// <summary>
        /// 服务器{0}接收到发现中心消息 ,id:{1},{2}
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.DiscoveryCenterMessageReceived
        /// 用途: 当接收到发现中心消息时记录
        /// 参数: {0} - 服务器类型, {1} - 连接ID, {2} - 消息内容
        /// </remarks>
        public const string DiscoveryCenterMessageReceived = "StartUp.DiscoveryCenterMessageReceived";

        /// <summary>
        /// 服务器{0}与发现中心断开连接 ,id:{1}...
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.DiscoveryCenterDisconnected
        /// 用途: 当与发现中心断开连接时记录
        /// 参数: {0} - 服务器类型, {1} - 连接ID
        /// </remarks>
        public const string DiscoveryCenterDisconnected = "StartUp.DiscoveryCenterDisconnected";

        /// <summary>
        /// 服务器{0}连接到发现中心成功 ,id:{1}...
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.DiscoveryCenterConnected
        /// 用途: 当成功连接到发现中心时记录
        /// 参数: {0} - 服务器类型, {1} - 连接ID
        /// </remarks>
        public const string DiscoveryCenterConnected = "StartUp.DiscoveryCenterConnected";

        /// <summary>
        /// The type of server that is launched: {0}
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.LaunchServerType
        /// 用途: 当启动指定类型的服务器时记录信息
        /// 参数: {0} - 服务器类型
        /// </remarks>
        public const string LaunchServerType = "StartUp.LaunchServerType";

        /// <summary>
        /// Grafana Loki label {0} already exists, will be ignored
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.GrafanaLokiLabelExists
        /// 用途: 当Grafana Loki标签已存在时记录警告
        /// 参数: {0} - 标签名称
        /// </remarks>
        public const string GrafanaLokiLabelExists = "StartUp.GrafanaLokiLabelExists";

        /// <summary>
        /// Finding the boot configuration for the corresponding server type in the configuration file will be configured to boot=>{0}
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.FindingConfigurationForServerType
        /// 用途: 当在配置文件中找到对应服务器类型的启动配置时记录
        /// 参数: {0} - 服务器类型
        /// </remarks>
        public const string FindingConfigurationForServerType = "StartUp.FindingConfigurationForServerType";

        /// <summary>
        /// If no startup configuration is found for the server type, it will start with the default configuration=>{0}
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.NoConfigurationUseDefault
        /// 用途: 当未找到服务器类型的启动配置时记录警告
        /// 参数: {0} - 服务器类型
        /// </remarks>
        public const string NoConfigurationUseDefault = "StartUp.NoConfigurationUseDefault";

        /// <summary>
        /// ----------------------------The Startup Server Is Over------------------------------
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.StartupOver
        /// 用途: 当所有服务器启动完成后记录
        /// </remarks>
        public const string StartupOver = "StartUp.StartupOver";

        /// <summary>
        /// Start Starting [{0}] Server- Configuration Information
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.StartingServerWithConfiguration
        /// 用途: 当开始启动服务器并显示配置信息时记录
        /// 参数: {0} - 服务器类型
        /// </remarks>
        public const string StartingServerWithConfiguration = "StartUp.StartingServerWithConfiguration";

        /// <summary>
        /// The server has been stopped, please check the log for details. (服务器已停止，详情请查看日志.因为启动任务为空。没有找到启动任务。)
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.NoStartupTaskFound
        /// 用途: 当没有找到启动任务时记录
        /// </remarks>
        public const string NoStartupTaskFound = "StartUp.NoStartupTaskFound";

        /// <summary>
        /// 指标端口 [{0}] 被占用，无法启动独立指标服务器
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.MetricsPortInUse
        /// 用途: 当指标端口被占用时记录错误
        /// 参数: {0} - 端口号
        /// </remarks>
        public const string MetricsPortInUse = "StartUp.MetricsPortInUse";

        /// <summary>
        /// 独立 Prometheus metrics 端点已启用: http://{0}:{1}/metrics
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.PrometheusMetricsEndpointEnabled
        /// 用途: 当Prometheus指标端点启用时记录
        /// 参数: {0} - IP地址, {1} - 端口号
        /// </remarks>
        public const string PrometheusMetricsEndpointEnabled = "StartUp.PrometheusMetricsEndpointEnabled";

        /// <summary>
        /// 独立 Metrics 健康检查端点: http://{0}:{1}/health
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.MetricsHealthCheckEndpointEnabled
        /// 用途: 当Metrics健康检查端点启用时记录
        /// 参数: {0} - IP地址, {1} - 端口号
        /// </remarks>
        public const string MetricsHealthCheckEndpointEnabled = "StartUp.MetricsHealthCheckEndpointEnabled";

        /// <summary>
        /// Prometheus metrics endpoint is enabled: http://{0}:{1}/metrics
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.PrometheusMetricsEndpointEnabledInline
        /// 用途: 当内联Prometheus指标端点启用时记录
        /// 参数: {0} - IP地址, {1} - 端口号
        /// </remarks>
        public const string PrometheusMetricsEndpointEnabledInline = "StartUp.PrometheusMetricsEndpointEnabledInline";

        /// <summary>
        /// Prometheus metrics service will be provided on the standalone port {0}
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.PrometheusMetricsServiceOnStandalonePort
        /// 用途: 当Prometheus指标服务将在独立端口提供时记录
        /// 参数: {0} - 端口号
        /// </remarks>
        public const string PrometheusMetricsServiceOnStandalonePort = "StartUp.PrometheusMetricsServiceOnStandalonePort";

        /// <summary>
        /// HTTP服务器相关消息
        /// </summary>
        public static class HttpServer
        {
            /// <summary>
            /// 处理POST请求的描述
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.HandlePostRequest
            /// 用途: HTTP API文档中使用
            /// </remarks>
            public const string HandlePostRequest = "StartUp.HttpServer.HandlePostRequest";

            /// <summary>
            /// 处理游戏客户端POST请求的描述
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.HandleGameClientPostRequest
            /// 用途: HTTP API文档中使用
            /// </remarks>
            public const string HandleGameClientPostRequest = "StartUp.HttpServer.HandleGameClientPostRequest";

            /// <summary>
            /// HTTP服务已禁用，启动已忽略
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.ServiceDisabled
            /// 用途: 当HTTP服务被禁用时记录
            /// </remarks>
            public const string ServiceDisabled = "StartUp.HttpServer.ServiceDisabled";

            /// <summary>
            /// 启动 [HTTP] 服务器...
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.StartingServer
            /// 用途: 当开始启动HTTP服务器时记录
            /// </remarks>
            public const string StartingServer = "StartUp.HttpServer.StartingServer";

            /// <summary>
            /// 启动 [HTTP] 服务器端口 [{0}] 超出范围 [{1}-{2}]，HTTP服务无法启动，启动已忽略
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.PortOutOfRange
            /// 用途: 当HTTP端口超出允许范围时记录警告
            /// 参数: {0} - 端口号, {1} - 最小端口, {2} - 最大端口
            /// </remarks>
            public const string PortOutOfRange = "StartUp.HttpServer.PortOutOfRange";

            /// <summary>
            /// Swagger UI 可通过 http://{0}:{1}/swagger 访问
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.SwaggerUiAccess
            /// 用途: 显示Swagger UI访问地址时记录
            /// 参数: {0} - IP地址, {1} - 端口号
            /// </remarks>
            public const string SwaggerUiAccess = "StartUp.HttpServer.SwaggerUiAccess";

            /// <summary>
            /// /swagger/{0}/swagger.json
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.SwaggerEndpointFormat
            /// 用途: Swagger端点路径格式
            /// 参数: {0} - 版本号
            /// </remarks>
            public const string SwaggerEndpointFormat = "StartUp.HttpServer.SwaggerEndpointFormat";

            /// <summary>
            /// swagger
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.SwaggerRoutePrefix
            /// 用途: Swagger路由前缀
            /// </remarks>
            public const string SwaggerRoutePrefix = "StartUp.HttpServer.SwaggerRoutePrefix";

            /// <summary>
            /// GameFrameX API
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.ApiTitle
            /// 用途: API文档标题
            /// </remarks>
            public const string ApiTitle = "StartUp.HttpServer.ApiTitle";

            /// <summary>
            /// GameFrameX HTTP API documentation
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.ApiDescription
            /// 用途: API文档描述
            /// </remarks>
            public const string ApiDescription = "StartUp.HttpServer.ApiDescription";

            /// <summary>
            /// GameFrameX
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.ApiLicenseName
            /// 用途: API许可证名称
            /// </remarks>
            public const string ApiLicenseName = "StartUp.HttpServer.ApiLicenseName";

            /// <summary>
            /// Blank
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.ApiContactName
            /// 用途: API联系人姓名
            /// </remarks>
            public const string ApiContactName = "StartUp.HttpServer.ApiContactName";

            /// <summary>
            /// HTTP
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.OpenTelemetryServiceType
            /// 用途: OpenTelemetry服务类型
            /// </remarks>
            public const string OpenTelemetryServiceType = "StartUp.HttpServer.OpenTelemetryServiceType";

            /// <summary>
            /// HTTP服务器启动完成 - 端口: {0}
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.StartupComplete
            /// 用途: 当HTTP服务器启动完成时记录
            /// 参数: {0} - 端口号
            /// </remarks>
            public const string StartupComplete = "StartUp.HttpServer.StartupComplete";

            /// <summary>
            /// HTTP服务器端口 [{0}] 被占用，HTTP服务无法启动
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.PortOccupied
            /// 用途: 当HTTP端口被占用无法启动服务时记录错误
            /// 参数: {0} - 端口号
            /// </remarks>
            public const string PortOccupied = "StartUp.HttpServer.PortOccupied";
        }

        /// <summary>
        /// 应用程序配置相关消息
        /// </summary>
        public static class ApplicationSettings
        {
            /// <summary>
            /// Configs/app_config.json
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.ApplicationSettings.ConfigFilePath
            /// 用途: 应用程序配置文件路径
            /// </remarks>
            public const string ConfigFilePath = "StartUp.ApplicationSettings.ConfigFilePath";

            /// <summary>
            /// _
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.ApplicationSettings.LogTypeSeparator
            /// 用途: 日志类型分隔符
            /// </remarks>
            public const string LogTypeSeparator = "StartUp.ApplicationSettings.LogTypeSeparator";

            /// <summary>
            /// [Warning] {0}
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.ApplicationSettings.WarningMessage
            /// 用途: 警告消息格式
            /// 参数: {0} - 警告消息内容
            /// </remarks>
            public const string WarningMessage = "StartUp.ApplicationSettings.WarningMessage";
        }

        /// <summary>
        /// TCP服务器相关消息
        /// </summary>
        public static class TcpServer
        {
            /// <summary>
            /// 客户端断开连接 - 会话ID: {0}, 远程终端: {1}, 断开原因: {2}
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.TcpServer.ClientDisconnected
            /// 用途: 当客户端断开连接时记录
            /// 参数: {0} - 会话ID, {1} - 远程端点, {2} - 断开原因
            /// </remarks>
            public const string ClientDisconnected = "StartUp.TcpServer.ClientDisconnected";

            /// <summary>
            /// 新客户端连接 - 会话ID: {0}, 远程终端: {1}
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.TcpServer.NewClientConnection
            /// 用途: 当新客户端连接时记录
            /// 参数: {0} - 会话ID, {1} - 远程端点
            /// </remarks>
            public const string NewClientConnection = "StartUp.TcpServer.NewClientConnection";

            /// <summary>
            /// 接收到消息 - 服务器类型: [{0}], 消息内容: {1}
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.TcpServer.MessageReceived
            /// 用途: 当接收到消息时记录调试信息
            /// 参数: {0} - 服务器类型, {1} - 消息内容
            /// </remarks>
            public const string MessageReceived = "StartUp.TcpServer.MessageReceived";

            /// <summary>
            /// 启动TCP服务器 类型: {0}, 地址: {1}, 端口: {2}
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.TcpServer.StartingServer
            /// 用途: 当开始启动TCP服务器时记录
            /// 参数: {0} - 服务器类型, {1} - 地址, {2} - 端口
            /// </remarks>
            public const string StartingServer = "StartUp.TcpServer.StartingServer";

            /// <summary>
            /// 启动TCP服务器完成 类型: {0}, 地址: {1}, 端口: {2}
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.TcpServer.StartupComplete
            /// 用途: 当TCP服务器启动完成时记录
            /// 参数: {0} - 服务器类型, {1} - 地址, {2} - 端口
            /// </remarks>
            public const string StartupComplete = "StartUp.TcpServer.StartupComplete";

            /// <summary>
            /// 启动TCP服务器失败 类型: {0}, 地址: {1}, 端口: {2}, 原因: 端口无效或被占用
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.TcpServer.StartupFailed
            /// 用途: 当TCP服务器启动失败时记录警告
            /// 参数: {0} - 服务器类型, {1} - 地址, {2} - 端口
            /// </remarks>
            public const string StartupFailed = "StartUp.TcpServer.StartupFailed";

            /// <summary>
            /// 启动TCP服务器 类型: {0}, 地址: {1}, 端口: {2}, 原因: TCP服务器被禁用
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.TcpServer.ServerDisabled
            /// 用途: 当TCP服务器被禁用时记录
            /// 参数: {0} - 服务器类型, {1} - 地址, {2} - 端口
            /// </remarks>
            public const string ServerDisabled = "StartUp.TcpServer.ServerDisabled";
        }

        /// <summary>
        /// WebSocket服务器相关消息
        /// </summary>
        public static class WebSocketServer
        {
            /// <summary>
            /// 启动WebSocket服务器 类型: {0}, 端口: {1}
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.WebSocketServer.StartingServer
            /// 用途: 当开始启动WebSocket服务器时记录
            /// 参数: {0} - 服务器类型, {1} - 端口
            /// </remarks>
            public const string StartingServer = "StartUp.WebSocketServer.StartingServer";

            /// <summary>
            /// 启动WebSocket服务器完成 类型: {0}, 端口: {1}
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.WebSocketServer.StartupComplete
            /// 用途: 当WebSocket服务器启动完成时记录
            /// 参数: {0} - 服务器类型, {1} - 端口
            /// </remarks>
            public const string StartupComplete = "StartUp.WebSocketServer.StartupComplete";

            /// <summary>
            /// 启动WebSocket服务器失败 类型: {0}, 端口: {1}, 原因: 端口无效或被占用
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.WebSocketServer.StartupFailed
            /// 用途: 当WebSocket服务器启动失败时记录警告
            /// 参数: {0} - 服务器类型, {1} - 端口
            /// </remarks>
            public const string StartupFailed = "StartUp.WebSocketServer.StartupFailed";

            /// <summary>
            /// 启动WebSocket服务器失败 类型: {0}, 端口: {1}, 原因: WebSocket服务未启用
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.WebSocketServer.ServiceNotEnabled
            /// 用途: 当WebSocket服务未启用时记录警告
            /// 参数: {0} - 服务器类型, {1} - 端口
            /// </remarks>
            public const string ServiceNotEnabled = "StartUp.WebSocketServer.ServiceNotEnabled";
        }

        /// <summary>
        /// 发现中心相关消息
        /// </summary>
        public static class DiscoveryCenter
        {
            /// <summary>
            /// DiscoveryCenterHost is not configured; unable to start connection to DiscoveryCenter. Please check the configuration item DiscoveryCenterHost
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.DiscoveryCenter.HostNotConfigured
            /// 用途: 当发现中心主机地址未配置时记录错误
            /// </remarks>
            public const string HostNotConfigured = "StartUp.DiscoveryCenter.HostNotConfigured";

            /// <summary>
            /// DiscoveryCenterPort is not configured; unable to start connection to DiscoveryCenter. Please check the configuration item DiscoveryCenterPort
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.DiscoveryCenter.PortNotConfigured
            /// 用途: 当发现中心端口未配置时记录错误
            /// </remarks>
            public const string PortNotConfigured = "StartUp.DiscoveryCenter.PortNotConfigured";

            /// <summary>
            /// DiscoveryCenterHost: {0} is not a valid IP address; unable to start connection to DiscoveryCenter. Please check the configuration item DiscoveryCenterHost
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.DiscoveryCenter.InvalidHostAddress
            /// 用途: 当发现中心主机地址无效时记录错误
            /// 参数: {0} - 无效的主机地址
            /// </remarks>
            public const string InvalidHostAddress = "StartUp.DiscoveryCenter.InvalidHostAddress";
        }

        /// <summary>
        /// HTTP服务器相关异常消息
        /// </summary>
        public static class HttpExceptions
        {
            /// <summary>
            /// The HTTP address must start with /
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpExceptions.AddressMustStartWithSlash
            /// 用途: 当HTTP地址不以/开始时抛出异常
            /// </remarks>
            public const string AddressMustStartWithSlash = "StartUp.HttpExceptions.AddressMustStartWithSlash";

            /// <summary>
            /// The HTTP address must end in /
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpExceptions.AddressMustEndWithSlash
            /// 用途: 当HTTP地址不以/结束时抛出异常
            /// </remarks>
            public const string AddressMustEndWithSlash = "StartUp.HttpExceptions.AddressMustEndWithSlash";

            /// <summary>
            /// If HTTPS is not implemented, cancel the HTTPS port configuration
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpExceptions.HttpsNotImplemented
            /// 用途: 当尝试配置HTTPS但HTTPS功能未实现时抛出异常
            /// </remarks>
            public const string HttpsNotImplemented = "StartUp.HttpExceptions.HttpsNotImplemented";
        }

        /// <summary>
        /// 健康检查相关消息
        /// </summary>
        public static class HealthCheck
        {
            /// <summary>
            /// the health check endpoint is enabled:
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HealthCheck.EndpointEnabled
            /// 用途: 当健康检查端点启用时记录信息
            /// </remarks>
            public const string EndpointEnabled = "StartUp.HealthCheck.EndpointEnabled";

            /// <summary>
            /// - detailed health checks: http://{0}:{1}{2}
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HealthCheck.DetailedEndpointUrl
            /// 用途: 显示详细健康检查端点URL时记录
            /// 参数: {0} - IP地址, {1} - 端口号, {2} - 路径
            /// </remarks>
            public const string DetailedEndpointUrl = "StartUp.HealthCheck.DetailedEndpointUrl";

            /// <summary>
            /// - simple health check: http://{0}:{1}{2}
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HealthCheck.SimpleEndpointUrl
            /// 用途: 显示简单健康检查端点URL时记录
            /// 参数: {0} - IP地址, {1} - 端口号, {2} - 路径
            /// </remarks>
            public const string SimpleEndpointUrl = "StartUp.HealthCheck.SimpleEndpointUrl";

            /// <summary>
            /// - OpenTelemetry check: http://{0}:{1}{2}
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HealthCheck.OpenTelemetryEndpointUrl
            /// 用途: 显示OpenTelemetry检查端点URL时记录
            /// 参数: {0} - IP地址, {1} - 端口号, {2} - 路径
            /// </remarks>
            public const string OpenTelemetryEndpointUrl = "StartUp.HealthCheck.OpenTelemetryEndpointUrl";

            /// <summary>
            /// 应用程序运行正常
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HealthCheck.ApplicationWorkingFine
            /// 用途: 健康检查返回成功状态时的消息
            /// </remarks>
            public const string ApplicationWorkingFine = "StartUp.HealthCheck.ApplicationWorkingFine";

            /// <summary>
            /// OpenTelemetry the configuration is normal
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HealthCheck.OpenTelemetryConfigurationNormal
            /// 用途: OpenTelemetry健康检查返回成功状态时的消息
            /// </remarks>
            public const string OpenTelemetryConfigurationNormal = "StartUp.HealthCheck.OpenTelemetryConfigurationNormal";

            /// <summary>
            /// OpenTelemetry is configured normally
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HealthCheck.OpenTelemetryConfiguredNormally
            /// 用途: OpenTelemetry健康检查端点返回的配置正常消息
            /// </remarks>
            public const string OpenTelemetryConfiguredNormally = "StartUp.HealthCheck.OpenTelemetryConfiguredNormally";

            /// <summary>
            /// OK
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HealthCheck.Ok
            /// 用途: 简单健康检查端点返回的成功消息
            /// </remarks>
            public const string Ok = "StartUp.HealthCheck.Ok";
        }

        /// <summary>
        /// 应用程序通用消息
        /// </summary>
        public static class Application
        {
            /// <summary>
            /// 独立指标服务器已在端口: {0} 启动
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.Application.MetricServerStarted
            /// 用途: 当独立指标服务器启动时记录
            /// 参数: {0} - 端口号
            /// </remarks>
            public const string MetricServerStarted = "StartUp.Application.MetricServerStarted";

            /// <summary>
            /// 监听程序退出消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.Application.ListeningExitMessage
            /// 用途: 当开始监听程序退出消息时记录
            /// </remarks>
            public const string ListeningExitMessage = "StartUp.Application.ListeningExitMessage";

            /// <summary>
            /// 执行退出程序
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.Application.ExecutingExitProcedure
            /// 用途: 当执行退出程序时记录
            /// </remarks>
            public const string ExecutingExitProcedure = "StartUp.Application.ExecutingExitProcedure";

            /// <summary>
            /// 获取未处理异常 标签:{0}
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.Application.GetUnhandledException
            /// 用途: 当获取到未处理异常时记录
            /// 参数: {0} - 异常标签
            /// </remarks>
            public const string GetUnhandledException = "StartUp.Application.GetUnhandledException";

            /// <summary>
            /// 未处理异常:{0}
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.Application.UnhandledException
            /// 用途: 当发生未处理异常时记录
            /// 参数: {0} - 异常信息
            /// </remarks>
            public const string UnhandledException = "StartUp.Application.UnhandledException";

            /// <summary>
            /// 所有未处理异常:{0}
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.Application.AllUnhandledExceptions
            /// 用途: 当发生多个未处理异常时记录
            /// 参数: {0} - 异常信息集合
            /// </remarks>
            public const string AllUnhandledExceptions = "StartUp.Application.AllUnhandledExceptions";

            /// <summary>
            /// 未处理异常回调:{0}
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.Application.UnhandledExceptionCallback
            /// 用途: 在退出回调中记录未处理异常时使用
            /// 参数: {0} - 异常信息
            /// </remarks>
            public const string UnhandledExceptionCallback = "StartUp.Application.UnhandledExceptionCallback";

            /// <summary>
            /// 接收到SIGTERM信号并注册退出处理程序
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.Application.SigtermSignalReceived
            /// 用途: 当接收到SIGTERM信号时记录
            /// </remarks>
            public const string SigtermSignalReceived = "StartUp.Application.SigtermSignalReceived";
        }
    }
}