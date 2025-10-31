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

using System.Reflection;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Utility;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Message;
using GameFrameX.StartUp.Extensions;
using GameFrameX.SuperSocket.Connection;
using GameFrameX.SuperSocket.Primitives;
using GameFrameX.SuperSocket.Server;
using GameFrameX.SuperSocket.Server.Abstractions;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.Server.Host;
using GameFrameX.SuperSocket.Udp;
using GameFrameX.SuperSocket.WebSocket;
using GameFrameX.SuperSocket.WebSocket.Server;
using GameFrameX.Utility;
using GameFrameX.Utility.Setting;
using Grafana.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Serilog;
using CloseReason = GameFrameX.SuperSocket.WebSocket.CloseReason;

namespace GameFrameX.StartUp;

/// <summary>
/// 程序启动器基类 - 提供TCP和WebSocket服务器的基础功能实现
/// </summary>
public abstract partial class AppStartUpBase
{
    /// <summary>
    /// 是否将当前服务注册到服务中心
    /// 默认 false：不自动注册；子类可重写为 true 以开启注册逻辑。
    /// </summary>
    protected virtual bool IsRegisterToDiscoveryCenter { get; set; } = false;

    /// <summary>
    /// 启动服务器 - 同时启动TCP和WebSocket服务
    /// </summary>
    /// <typeparam name="TMessageDecoderHandler">消息解码处理器类型，必须实现IMessageDecoderHandler和IPackageDecoder接口</typeparam>
    /// <typeparam name="TMessageEncoderHandler">消息编码处理器类型，必须实现IMessageEncoderHandler和IPackageEncoder接口</typeparam>
    /// <param name="messageCompressHandler">消息编码的时候使用的压缩处理器，如果为空则不处理压缩消息</param>
    /// <param name="messageDecompressHandler">消息解码的时候使用的解压处理器,如果为空则不处理压缩消息</param>
    /// <param name="baseHandler">HTTP处理器列表,用于处理不同的HTTP请求</param>
    /// <param name="httpFactory">HTTP处理器工厂,根据命令标识符创建对应的处理器实例</param>
    /// <param name="aopHandlerTypes">AOP处理器列表,用于在HTTP请求处理前后执行额外的逻辑</param>
    /// <param name="minimumLevelLogLevel">日志记录的最小级别,用于控制日志输出</param>
    protected async Task StartServerAsync<TMessageDecoderHandler, TMessageEncoderHandler>(
        IMessageCompressHandler messageCompressHandler,
        IMessageDecompressHandler messageDecompressHandler,
        List<BaseHttpHandler> baseHandler, Func<string, BaseHttpHandler> httpFactory, List<IHttpAopHandler> aopHandlerTypes = null, LogLevel minimumLevelLogLevel = LogLevel.Debug)
        where TMessageDecoderHandler : class, IMessageDecoderHandler, new()
        where TMessageEncoderHandler : class, IMessageEncoderHandler, new()
    {
        MessageHelper.SetMessageDecoderHandler(Activator.CreateInstance<TMessageDecoderHandler>(), messageDecompressHandler);
        MessageHelper.SetMessageEncoderHandler(Activator.CreateInstance<TMessageEncoderHandler>(), messageCompressHandler);
        // 启动服务器
        await StartServer(baseHandler, httpFactory, aopHandlerTypes, minimumLevelLogLevel);
        if (Setting.ServerType != GlobalConst.DiscoveryCenterServiceName)
        {
            if (IsRegisterToDiscoveryCenter)
            {
                StartGameAppClient();
            }
        }

        // 设置全局启动状态
        GlobalSettings.LaunchTime = DateTime.UtcNow;
        GlobalSettings.IsAppRunning = true;
    }

    /// <summary>
    /// 停止服务器 - 关闭所有网络服务
    /// </summary>
    protected async Task StopServerAsync()
    {
        GlobalSettings.IsAppRunning = false;
        _gameAppServiceClient?.Stop();

        if (_gameServer != null)
        {
            await _gameServer.StopAsync();
            _gameServer = null;
        }
    }

    /// <summary>
    /// 消息处理异常处理方法
    /// </summary>
    /// <param name="appSession">会话对象</param>
    /// <param name="exception">异常信息</param>
    /// <returns>返回true表示继续处理，返回false表示终止处理</returns>
    protected virtual ValueTask<bool> PackageErrorHandler(IAppSession appSession, PackageHandlingException<IMessage> exception)
    {
        return ValueTask.FromResult(true);
    }

    /// <summary>
    /// 客户端断开连接时的处理方法
    /// </summary>
    /// <param name="appSession">断开连接的会话对象</param>
    /// <param name="disconnectEventArgs">断开连接的相关参数</param>
    protected virtual ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs disconnectEventArgs)
    {
        LogHelper.Info($"the client disconnects - SessionID: {appSession.SessionID}, remote terminal: {appSession.RemoteEndPoint}, disconnect cause: {disconnectEventArgs.Reason}");
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 客户端连接成功时的处理方法
    /// </summary>
    /// <param name="appSession">新建立的会话对象</param>
    protected virtual ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info($"new client connection - SessionID: {appSession.SessionID}, remote terminal: {appSession.RemoteEndPoint}");
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 收到消息包的处理方法
    /// </summary>
    /// <param name="session">会话对象</param>
    /// <param name="message">接收到的消息</param>
    protected virtual ValueTask PackageHandler(IAppSession session, IMessage message)
    {
        if (Setting.IsDebug && Setting.IsDebugReceive)
        {
            LogHelper.Debug($"message received- server type: [{ServerType}], message content: {message.ToFormatMessageString()}");
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 异步消息处理方法
    /// </summary>
    /// <param name="handler">消息处理器</param>
    /// <param name="message">网络消息</param>
    /// <param name="netWorkChannel">网络通道</param>
    /// <param name="timeout">超时时间(毫秒)</param>
    /// <param name="cancellationToken">取消令牌</param>
    protected async Task InvokeMessageHandler(IMessageHandler handler, INetworkMessage message, INetWorkChannel netWorkChannel, int timeout = 30000, CancellationToken cancellationToken = default)
    {
        async void InvokeAction()
        {
            bool initSuccess = await handler.Init(message, netWorkChannel);
            if (initSuccess == false)
            {
                return;
            }

            await handler.InnerAction(timeout, cancellationToken);
        }

        await Task.Run(InvokeAction, cancellationToken);
    }

    #region TCP Server

    private IHost _gameServer;

    /// <summary>
    /// 启动TCP服务器
    /// </summary>
    /// <param name="baseHandler">HTTP处理器列表,用于处理不同的HTTP请求</param>
    /// <param name="httpFactory">HTTP处理器工厂,根据命令标识符创建对应的处理器实例</param>
    /// <param name="aopHandlerTypes">AOP处理器列表,用于在HTTP请求处理前后执行额外的逻辑</param>
    /// <param name="minimumLevelLogLevel">日志记录的最小级别,用于控制日志输出</param>
    private async Task StartServer(List<BaseHttpHandler> baseHandler, Func<string, BaseHttpHandler> httpFactory, List<IHttpAopHandler> aopHandlerTypes = null, LogLevel minimumLevelLogLevel = LogLevel.Debug)
    {
        var multipleServerHostBuilder = MultipleServerHostBuilder.Create();
        if (Setting.IsEnableTcp)
        {
            // 检查TCP端口是否可用
            if (Setting.InnerPort > 0 && NetHelper.PortIsAvailable(Setting.InnerPort))
            {
                LogHelper.InfoConsole($"start tcp server type: {ServerType}, address: {Setting.InnerHost}, port: {Setting.InnerPort}");
                multipleServerHostBuilder.AddServer<IMessage, MessageObjectPipelineFilter>(builder =>
                {
                    var serverBuilder = builder
                                        .UseClearIdleSession()
                                        .UseSessionHandler(OnConnected, OnDisconnected)
                                        .UsePackageHandler(PackageHandler, PackageErrorHandler)
                                        .UseInProcSessionContainer();

                    // 启用UDP 检查是否可用
                    if (Setting.IsEnableUdp)
                    {
                        serverBuilder.UseUdp();
                    }

                    serverBuilder.ConfigureServices((context, serviceCollection) =>
                    {
                        serviceCollection.Configure<ServerOptions>(options =>
                        {
                            var listenOptions = new ListenOptions()
                            {
                                Ip = "Any",
                                Port = Setting.InnerPort,
                            };
                            options.AddListener(listenOptions);
                        });
                        // foreach (var serviceDescriptor in serviceCollection)
                        // {
                        //     if (serviceDescriptor.ServiceType == typeof(IPackageDecoder<IMessage>))
                        //     {
                        //         serviceDescriptor.ImplementationInstance ;
                        //         LogHelper.Console("XX");
                        //     }
                        // }
                    });
                });
                LogHelper.InfoConsole($"start tcp server startup complete type: {ServerType}, address: {Setting.InnerHost}, port: {Setting.InnerPort}");
            }
            else
            {
                LogHelper.WarningConsole($"start tcp server start failed type: {ServerType}, address: {Setting.InnerHost}, port: {Setting.InnerPort}, cause: the port is invalid or occupied");
            }
        }
        else
        {
            LogHelper.InfoConsole($"start tcp server type: {ServerType}, address: {Setting.InnerHost}, port: {Setting.InnerPort}, cause: the tcp server is disabled");
        }

        // 检查WebSocket端口是否可用
        if (Setting.IsEnableWebSocket)
        {
            if (Setting.WsPort is > 0 and < ushort.MaxValue && NetHelper.PortIsAvailable(Setting.WsPort))
            {
                LogHelper.InfoConsole("start the websocket server...");

                // 配置并启动WebSocket服务器
                multipleServerHostBuilder.AddWebSocketServer(builder =>
                {
                    builder
                        .UseWebSocketMessageHandler(WebSocketMessageHandler)
                        .UseSessionHandler(OnConnected, OnDisconnected)
                        .ConfigureServices((context, serviceCollection) =>
                        {
                            serviceCollection.Configure<ServerOptions>(options =>
                            {
                                var listenOptions = new ListenOptions()
                                {
                                    Ip = "Any",
                                    Port = Setting.WsPort,
                                };
                                options.AddListener(listenOptions);
                            });
                        });
                });
                LogHelper.InfoConsole($"start websocket server startup complete type: {ServerType}, port: {Setting.WsPort}");
            }
            else
            {
                LogHelper.WarningConsole($"start websocket server start failed type: {ServerType}, port: {Setting.WsPort}, cause: the port is invalid or occupied");
            }
        }
        else
        {
            LogHelper.InfoConsole($"start websocket server start failed type: {ServerType}, port: {Setting.WsPort}, cause: WebSocket service is not enabled");
        }

        // await StartHttpServerAsync(hostBuilder,baseHandler, httpFactory, aopHandlerTypes, minimumLevelLogLevel);
        await StartHttpServer(baseHandler, httpFactory, aopHandlerTypes, minimumLevelLogLevel);

        // 启动独立的指标服务器（如果配置了独立端口）
        var metricsServer = await OpenTelemetryExtensions.CreateMetricsServerAsync(Setting, "TCP");
        if (metricsServer is not null)
        {
            LogHelper.InfoConsole($"The standalone metric server is started on the port: {Setting.MetricsPort}");
        }

        // 配置监控和跟踪
        multipleServerHostBuilder.ConfigureServices(services => { services.AddGameFrameXOpenTelemetry(Setting); });

        // 配置日志
        multipleServerHostBuilder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog(Log.Logger, true);
            logging.SetMinimumLevel(minimumLevelLogLevel);
            logging.AddGameFrameXOpenTelemetryLogging(Setting);
        });

        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
                                      .UseGrafana(config =>
                                      {
                                          config.ServiceName = Setting.ServerName + "-" + Setting.TagName;
                                          config.ServiceVersion = Assembly.GetCallingAssembly().ImageRuntimeVersion;
                                          config.ServiceInstanceId = Setting.ServerId + "-" + Setting.ServerInstanceId;
                                          config.DeploymentEnvironment = EnvironmentHelper.GetEnvironmentName().IsNullOrEmpty() ? Setting.IsDebug ? "Debug" : "Release" : EnvironmentHelper.GetEnvironmentName();
                                      })
                                      .Build();
        // 构建并启动服务器
        _gameServer = multipleServerHostBuilder.Build();

        await _gameServer.StartAsync();
    }

    #endregion

    #region WebSocket

    /// <summary>
    /// WebSocket消息处理方法
    /// </summary>
    /// <param name="session">WebSocket会话对象</param>
    /// <param name="messagePackage">接收到的消息包</param>
    private async ValueTask WebSocketMessageHandler(WebSocketSession session, WebSocketPackage messagePackage)
    {
        // 只处理二进制消息
        if (messagePackage.OpCode != OpCode.Binary)
        {
            await session.CloseAsync(CloseReason.ProtocolError);
            return;
        }

        var readOnlySequence = messagePackage.Data;
        var message = MessageHelper.DecoderHandler.Handler(ref readOnlySequence);
        await PackageHandler(session, message);
    }

    #endregion
}