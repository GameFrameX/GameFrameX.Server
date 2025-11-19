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
        /// 服务器停止的消息
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.ServerStopped
        /// 用途: 当服务器停止时记录日志信息
        /// 参数: {0} - 服务器类型, {1} - 终止原因, {2} - 配置信息
        /// </remarks>
        public const string ServerStopped = "StartUp.ServerStopped";

        /// <summary>
        /// 与发现中心通信错误的消息
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.DiscoveryCenterCommunicationError
        /// 用途: 当与发现中心通信发生错误时记录
        /// 参数: {0} - 服务器类型, {1} - 连接ID, {2} - 异常信息
        /// </remarks>
        public const string DiscoveryCenterCommunicationError = "StartUp.DiscoveryCenterCommunicationError";

        /// <summary>
        /// 接收到发现中心消息的消息
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.DiscoveryCenterMessageReceived
        /// 用途: 当接收到发现中心消息时记录
        /// 参数: {0} - 服务器类型, {1} - 连接ID, {2} - 消息内容
        /// </remarks>
        public const string DiscoveryCenterMessageReceived = "StartUp.DiscoveryCenterMessageReceived";

        /// <summary>
        /// 与发现中心断开连接的消息
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.DiscoveryCenterDisconnected
        /// 用途: 当与发现中心断开连接时记录
        /// 参数: {0} - 服务器类型, {1} - 连接ID
        /// </remarks>
        public const string DiscoveryCenterDisconnected = "StartUp.DiscoveryCenterDisconnected";

        /// <summary>
        /// 连接到发现中心成功的消息
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.DiscoveryCenterConnected
        /// 用途: 当成功连接到发现中心时记录
        /// 参数: {0} - 服务器类型, {1} - 连接ID
        /// </remarks>
        public const string DiscoveryCenterConnected = "StartUp.DiscoveryCenterConnected";

        /// <summary>
        /// 服务器启动任务为空的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.NoStartupTaskFound
        /// 用途: 当没有找到启动任务时记录
        /// </remarks>
        public const string NoStartupTaskFound = "StartUp.NoStartupTaskFound";

        /// <summary>
        /// 指标端口被占用的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.MetricsPortInUse
        /// 用途: 当指标端口被占用时记录错误
        /// 参数: {0} - 端口号
        /// </remarks>
        public const string MetricsPortInUse = "StartUp.MetricsPortInUse";

        /// <summary>
        /// Prometheus指标端点已启用的消息
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.PrometheusMetricsEndpointEnabled
        /// 用途: 当Prometheus指标端点启用时记录
        /// 参数: {0} - IP地址, {1} - 端口号
        /// </remarks>
        public const string PrometheusMetricsEndpointEnabled = "StartUp.PrometheusMetricsEndpointEnabled";

        /// <summary>
        /// Metrics健康检查端点已启用的消息
        /// </summary>
        /// <remarks>
        /// 键名: StartUp.MetricsHealthCheckEndpointEnabled
        /// 用途: 当Metrics健康检查端点启用时记录
        /// 参数: {0} - IP地址, {1} - 端口号
        /// </remarks>
        public const string MetricsHealthCheckEndpointEnabled = "StartUp.MetricsHealthCheckEndpointEnabled";

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
            /// HTTP服务被禁用的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.ServiceDisabled
            /// 用途: 当HTTP服务被禁用时记录
            /// </remarks>
            public const string ServiceDisabled = "StartUp.HttpServer.ServiceDisabled";

            /// <summary>
            /// 启动HTTP服务器的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.StartingServer
            /// 用途: 当开始启动HTTP服务器时记录
            /// </remarks>
            public const string StartingServer = "StartUp.HttpServer.StartingServer";

            /// <summary>
            /// HTTP端口超出范围的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.PortOutOfRange
            /// 用途: 当HTTP端口超出允许范围时记录警告
            /// 参数: {0} - 端口号, {1} - 最小端口, {2} - 最大端口
            /// </remarks>
            public const string PortOutOfRange = "StartUp.HttpServer.PortOutOfRange";

            /// <summary>
            /// Swagger UI访问地址的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.HttpServer.SwaggerUiAccess
            /// 用途: 显示Swagger UI访问地址时记录
            /// 参数: {0} - IP地址, {1} - 端口号
            /// </remarks>
            public const string SwaggerUiAccess = "StartUp.HttpServer.SwaggerUiAccess";
        }

        /// <summary>
        /// TCP服务器相关消息
        /// </summary>
        public static class TcpServer
        {
            /// <summary>
            /// 客户端断开连接的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.TcpServer.ClientDisconnected
            /// 用途: 当客户端断开连接时记录
            /// 参数: {0} - 会话ID, {1} - 远程端点, {2} - 断开原因
            /// </remarks>
            public const string ClientDisconnected = "StartUp.TcpServer.ClientDisconnected";

            /// <summary>
            /// 新客户端连接的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.TcpServer.NewClientConnection
            /// 用途: 当新客户端连接时记录
            /// 参数: {0} - 会话ID, {1} - 远程端点
            /// </remarks>
            public const string NewClientConnection = "StartUp.TcpServer.NewClientConnection";

            /// <summary>
            /// 接收到消息的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.TcpServer.MessageReceived
            /// 用途: 当接收到消息时记录调试信息
            /// 参数: {0} - 服务器类型, {1} - 消息内容
            /// </remarks>
            public const string MessageReceived = "StartUp.TcpServer.MessageReceived";

            /// <summary>
            /// 启动TCP服务器的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.TcpServer.StartingServer
            /// 用途: 当开始启动TCP服务器时记录
            /// 参数: {0} - 服务器类型, {1} - 地址, {2} - 端口
            /// </remarks>
            public const string StartingServer = "StartUp.TcpServer.StartingServer";

            /// <summary>
            /// TCP服务器启动完成的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.TcpServer.StartupComplete
            /// 用途: 当TCP服务器启动完成时记录
            /// 参数: {0} - 服务器类型, {1} - 地址, {2} - 端口
            /// </remarks>
            public const string StartupComplete = "StartUp.TcpServer.StartupComplete";

            /// <summary>
            /// TCP服务器启动失败的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.TcpServer.StartupFailed
            /// 用途: 当TCP服务器启动失败时记录警告
            /// 参数: {0} - 服务器类型, {1} - 地址, {2} - 端口
            /// </remarks>
            public const string StartupFailed = "StartUp.TcpServer.StartupFailed";

            /// <summary>
            /// TCP服务器被禁用的消息
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
            /// 启动WebSocket服务器的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.WebSocketServer.StartingServer
            /// 用途: 当开始启动WebSocket服务器时记录
            /// 参数: {0} - 服务器类型, {1} - 端口
            /// </remarks>
            public const string StartingServer = "StartUp.WebSocketServer.StartingServer";

            /// <summary>
            /// WebSocket服务器启动完成的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.WebSocketServer.StartupComplete
            /// 用途: 当WebSocket服务器启动完成时记录
            /// 参数: {0} - 服务器类型, {1} - 端口
            /// </remarks>
            public const string StartupComplete = "StartUp.WebSocketServer.StartupComplete";

            /// <summary>
            /// WebSocket服务器启动失败的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.WebSocketServer.StartupFailed
            /// 用途: 当WebSocket服务器启动失败时记录警告
            /// 参数: {0} - 服务器类型, {1} - 端口
            /// </remarks>
            public const string StartupFailed = "StartUp.WebSocketServer.StartupFailed";

            /// <summary>
            /// WebSocket服务未启用的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.WebSocketServer.ServiceNotEnabled
            /// 用途: 当WebSocket服务未启用时记录警告
            /// 参数: {0} - 服务器类型, {1} - 端口
            /// </remarks>
            public const string ServiceNotEnabled = "StartUp.WebSocketServer.ServiceNotEnabled";
        }

        /// <summary>
        /// 应用程序通用消息
        /// </summary>
        public static class Application
        {
            /// <summary>
            /// 指标服务器启动的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.Application.MetricServerStarted
            /// 用途: 当独立指标服务器启动时记录
            /// 参数: {0} - 端口号
            /// </remarks>
            public const string MetricServerStarted = "StartUp.Application.MetricServerStarted";

            /// <summary>
            /// 监听程序退出消息的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.Application.ListeningExitMessage
            /// 用途: 当开始监听程序退出消息时记录
            /// </remarks>
            public const string ListeningExitMessage = "StartUp.Application.ListeningExitMessage";

            /// <summary>
            /// 执行退出程序的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.Application.ExecutingExitProcedure
            /// 用途: 当执行退出程序时记录
            /// </remarks>
            public const string ExecutingExitProcedure = "StartUp.Application.ExecutingExitProcedure";

            /// <summary>
            /// 获取未处理异常的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.Application.GetUnhandledException
            /// 用途: 当获取到未处理异常时记录
            /// 参数: {0} - 异常标签
            /// </remarks>
            public const string GetUnhandledException = "StartUp.Application.GetUnhandledException";

            /// <summary>
            /// 未处理异常的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.Application.UnhandledException
            /// 用途: 当发生未处理异常时记录
            /// 参数: {0} - 异常信息
            /// </remarks>
            public const string UnhandledException = "StartUp.Application.UnhandledException";

            /// <summary>
            /// 所有未处理异常的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.Application.AllUnhandledExceptions
            /// 用途: 当发生多个未处理异常时记录
            /// 参数: {0} - 异常信息集合
            /// </remarks>
            public const string AllUnhandledExceptions = "StartUp.Application.AllUnhandledExceptions";

            /// <summary>
            /// 未处理异常回调的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.Application.UnhandledExceptionCallback
            /// 用途: 在退出回调中记录未处理异常时使用
            /// 参数: {0} - 异常信息
            /// </remarks>
            public const string UnhandledExceptionCallback = "StartUp.Application.UnhandledExceptionCallback";

            /// <summary>
            /// SIGTERM信号注册的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.Application.SigtermSignalReceived
            /// 用途: 当接收到SIGTERM信号时记录
            /// </remarks>
            public const string SigtermSignalReceived = "StartUp.Application.SigtermSignalReceived";
        }
    }
}