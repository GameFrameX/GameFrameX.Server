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

using System.Net;
using GameFrameX.AppHost.ServiceDefaults;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Kcp;
using GameFrameX.NetWork.Message;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using CloseReason = GameFrameX.SuperSocket.WebSocket.CloseReason;

namespace GameFrameX.StartUp;

/// <summary>
/// 程序启动器基类 - 提供服务器的基础功能实现。
/// </summary>
/// <remarks>
/// Application startup base class - provides server basic functionality implementation.
/// This partial class specifically handles TCP and WebSocket server startup and configuration functionality.
/// </remarks>
public abstract partial class AppStartUpBase
{
    /// <summary>
    /// 启动服务器 - 同时启动 TCP 和 WebSocket 服务。
    /// </summary>
    /// <remarks>
    /// Start server - simultaneously start TCP and WebSocket services.
    /// This method is responsible for initializing message encoders/decoders, starting various network services, and setting the global startup status.
    /// </remarks>
    /// <typeparam name="TMessageDecoderHandler">消息解码处理器类型，必须实现 <see cref="IMessageDecoderHandler"/> 和 <see cref="IPackageDecoder"/> 接口 / Message decoder handler type, must implement IMessageDecoderHandler and IPackageDecoder interfaces</typeparam>
    /// <typeparam name="TMessageEncoderHandler">消息编码处理器类型，必须实现 <see cref="IMessageEncoderHandler"/> 和 <see cref="IPackageEncoder"/> 接口 / Message encoder handler type, must implement IMessageEncoderHandler and IPackageEncoder interfaces</typeparam>
    /// <param name="messageCompressHandler">消息编码时使用的压缩处理器；如果为空则不处理压缩消息 / Compression handler used when encoding messages; no compression processing if null</param>
    /// <param name="messageDecompressHandler">消息解码时使用的解压处理器；如果为空则不处理压缩消息 / Decompression handler used when decoding messages; no decompression processing if null</param>
    /// <param name="baseHandler">HTTP 处理器列表，用于处理不同的 HTTP 请求 / HTTP handler list for processing different HTTP requests</param>
    /// <param name="httpFactory">HTTP 处理器工厂，根据命令标识符创建对应的处理器实例 / HTTP handler factory that creates corresponding handler instances based on command identifiers</param>
    /// <param name="aopHandlerTypes">AOP 处理器列表，用于在 HTTP 请求处理前后执行额外的逻辑 / AOP handler list for executing additional logic before and after HTTP request processing</param>
    /// <param name="minimumLevelLogLevel">日志记录的最小级别，用于控制日志输出 / Minimum level for logging to control log output</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
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
        // 设置全局启动状态
        GlobalSettings.LaunchTime = DateTime.UtcNow;
        GlobalSettings.IsAppRunning = true;
    }

    /// <summary>
    /// 停止服务器 - 关闭所有网络服务。
    /// </summary>
    /// <remarks>
    /// Stop server - close all network services.
    /// This method is responsible for gracefully closing all network services and connections.
    /// </remarks>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    protected async Task StopServerAsync()
    {
        GlobalSettings.IsAppRunning = false;

        if (_gameServer != null)
        {
            await _gameServer.StopAsync();
            _gameServer = null;
        }
    }

    /// <summary>
    /// 消息处理异常处理方法。
    /// </summary>
    /// <remarks>
    /// Message processing exception handler.
    /// Handles exceptions that occur during message processing.
    /// </remarks>
    /// <param name="appSession">会话对象 / Session object</param>
    /// <param name="exception">异常信息 / Exception information</param>
    /// <returns>返回 <c>true</c> 表示继续处理；返回 <c>false</c> 表示终止处理 / Returns <c>true</c> to continue processing; returns <c>false</c> to terminate processing</returns>
    protected virtual ValueTask<bool> PackageErrorHandler(IAppSession appSession, PackageHandlingException<IMessage> exception)
    {
        return ValueTask.FromResult(true);
    }

    /// <summary>
    /// 客户端断开连接时的处理方法。
    /// </summary>
    /// <remarks>
    /// Handler when client disconnects.
    /// Handles the situation when a client disconnects, recording relevant information.
    /// </remarks>
    /// <param name="appSession">断开连接的会话对象 / Session object that disconnected</param>
    /// <param name="disconnectEventArgs">断开连接的相关参数 / Parameters related to disconnection</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    protected virtual ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs disconnectEventArgs)
    {
        LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.TcpServer.ClientDisconnected, appSession.SessionID, appSession.RemoteEndPoint, disconnectEventArgs.Reason));
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// KCP客户端连接成功处理
    /// </summary>
    /// <param name="remoteEndPoint">远程端点</param>
    /// <returns></returns>
    protected virtual ValueTask OnKcpConnected(EndPoint remoteEndPoint)
    {
        LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.KcpServer.NewClientConnection, remoteEndPoint));
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// KCP消息处理方法
    /// </summary>
    /// <param name="session">游戏应用会话</param>
    /// <param name="message">接收到的消息</param>
    protected virtual void KcpPackageHandler(IGameAppSession session, IMessage message)
    {
        if (Setting.IsDebug && Setting.IsDebugReceive)
        {
            LogHelper.Debug(LocalizationService.GetString(Localization.Keys.StartUp.TcpServer.MessageReceived, ServerType, message.ToFormatMessageString()));
        }
    }

    /// <summary>
    /// KCP客户端断开连接处理
    /// </summary>
    /// <param name="remoteEndPoint">远程端点</param>
    /// <returns></returns>
    protected virtual ValueTask OnKcpDisconnected(EndPoint remoteEndPoint)
    {
        LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.KcpServer.ClientDisconnected, remoteEndPoint));
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 客户端连接成功时的处理方法。
    /// </summary>
    /// <remarks>
    /// Handler when client connects successfully.
    /// Handles the situation when a new client connects, recording relevant information.
    /// </remarks>
    /// <param name="appSession">新建立的会话对象 / Newly established session object</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    protected virtual ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.TcpServer.NewClientConnection, appSession.SessionID, appSession.RemoteEndPoint));
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 收到消息包的处理方法。
    /// </summary>
    /// <remarks>
    /// Handler for received message packages.
    /// Handles received message packages; in debug mode, message content will be logged.
    /// </remarks>
    /// <param name="session">会话对象 / Session object</param>
    /// <param name="message">接收到的消息 / Received message</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    protected virtual ValueTask PackageHandler(IAppSession session, IMessage message)
    {
        if (Setting.IsDebug && Setting.IsDebugReceive)
        {
            LogHelper.Debug(LocalizationService.GetString(Localization.Keys.StartUp.TcpServer.MessageReceived, ServerType, message.ToFormatMessageString()));
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 异步消息处理方法。
    /// </summary>
    /// <remarks>
    /// Asynchronous message processing method.
    /// Asynchronously invokes the message handler, including initialization and execution logic.
    /// </remarks>
    /// <param name="handler">消息处理器 / Message handler</param>
    /// <param name="message">网络消息 / Network message</param>
    /// <param name="netWorkChannel">网络通道 / Network channel</param>
    /// <param name="timeout">超时时间（毫秒）/ Timeout (milliseconds)</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    protected async Task InvokeMessageHandler(IMessageHandler handler, INetworkMessage message, INetWorkChannel netWorkChannel, int timeout = 30000, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.Run(async () =>
            {
                var initSuccess = await handler.Init(message, netWorkChannel);
                if (!initSuccess)
                {
                    return;
                }

                await handler.InnerAction(timeout, cancellationToken);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            LogHelper.Error($"Message handler error: {ex}");
        }
    }

    #region TCP Server

    /// <summary>
    /// 游戏服务器主机实例。
    /// </summary>
    /// <remarks>
    /// Game server host instance.
    /// Used to manage the lifecycle of TCP and WebSocket servers.
    /// </remarks>
    /// <value>用于管理游戏服务器生命周期的主机实例 / Host instance for managing the game server lifecycle</value>
    private IHost _gameServer;

    /// <summary>
    /// 启动 TCP 服务器。
    /// </summary>
    /// <remarks>
    /// Start TCP server.
    /// This method is responsible for configuring and starting TCP, WebSocket, and HTTP servers, as well as related monitoring and logging functionality.
    /// </remarks>
    /// <param name="baseHandler">HTTP 处理器列表，用于处理不同的 HTTP 请求 / HTTP handler list for processing different HTTP requests</param>
    /// <param name="httpFactory">HTTP 处理器工厂，根据命令标识符创建对应的处理器实例 / HTTP handler factory that creates corresponding handler instances based on command identifiers</param>
    /// <param name="aopHandlerTypes">AOP 处理器列表，用于在 HTTP 请求处理前后执行额外的逻辑 / AOP handler list for executing additional logic before and after HTTP request processing</param>
    /// <param name="minimumLevelLogLevel">日志记录的最小级别，用于控制日志输出 / Minimum level for logging to control log output</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    private async Task StartServer(List<BaseHttpHandler> baseHandler, Func<string, BaseHttpHandler> httpFactory, List<IHttpAopHandler> aopHandlerTypes = null, LogLevel minimumLevelLogLevel = LogLevel.Debug)
    {
        var multipleServerHostBuilder = MultipleServerHostBuilder.Create();
        if (Setting.IsEnableTcp)
        {
            // 检查TCP端口是否可用
            if (Setting.InnerPort > 0 && NetHelper.PortIsAvailable(Setting.InnerPort))
            {
                LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.TcpServer.StartingServer, ServerType, Setting.InnerHost, Setting.InnerPort));
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
                            var listenOptions = new ListenOptions
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
                        //         LogHelper.Info($"XX");
                        //     }
                        // }
                    });
                });
                LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.TcpServer.StartupComplete, ServerType, Setting.InnerHost, Setting.InnerPort));
            }
            else
            {
                LogHelper.Warning(LocalizationService.GetString(Localization.Keys.StartUp.TcpServer.StartupFailed, ServerType, Setting.InnerHost, Setting.InnerPort));
            }
        }
        else
        {
            LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.TcpServer.ServerDisabled, ServerType, Setting.InnerHost, Setting.InnerPort));
        }

        // 检查WebSocket端口是否可用
        if (Setting.IsEnableWebSocket)
        {
            if (Setting.WsPort is > 0 and < ushort.MaxValue && NetHelper.PortIsAvailable(Setting.WsPort))
            {
                LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.WebSocketServer.StartingServer, ServerType, Setting.WsPort));

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
                                var listenOptions = new ListenOptions
                                {
                                    Ip = "Any",
                                    Port = Setting.WsPort,
                                };
                                options.AddListener(listenOptions);
                            });
                        });
                });
                LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.WebSocketServer.StartupComplete, ServerType, Setting.WsPort));
            }
            else
            {
                LogHelper.Warning(LocalizationService.GetString(Localization.Keys.StartUp.WebSocketServer.StartupFailed, ServerType, Setting.WsPort));
            }
        }
        else
        {
            LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.WebSocketServer.ServiceNotEnabled, ServerType, Setting.WsPort));
        }

        // 启动KCP服务器
        if (Setting.IsEnableKcp)
        {
            var kcpPort = Setting.KcpPort > 0 ? Setting.KcpPort : Setting.InnerPort;
            if (kcpPort > 0 && NetHelper.PortIsAvailable(kcpPort))
            {
                LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.KcpServer.StartingServer, ServerType, Setting.InnerHost, kcpPort));
                var kcpServer = new KcpServer(
                    kcpPort,
                    new KcpOptions { Enable = true },
                    Setting,
                    KcpPackageHandler,
                    OnKcpConnected,
                    OnKcpDisconnected
                );
                _ = kcpServer.StartAsync();
                LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.KcpServer.StartupComplete, ServerType, Setting.InnerHost, kcpPort));
            }
            else
            {
                LogHelper.Warning(LocalizationService.GetString(Localization.Keys.StartUp.KcpServer.StartupFailed, ServerType, Setting.InnerHost, kcpPort));
            }
        }
        else
        {
            LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.KcpServer.ServerDisabled, ServerType, Setting.InnerHost, Setting.KcpPort));
        }

        // await StartHttpServerAsync(hostBuilder,baseHandler, httpFactory, aopHandlerTypes, minimumLevelLogLevel);
        await StartHttpServer(baseHandler, httpFactory, aopHandlerTypes, minimumLevelLogLevel);

        // 配置日志
        multipleServerHostBuilder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog(Log.Logger, true);
            logging.SetMinimumLevel(minimumLevelLogLevel);
            logging.ConfigureOpenTelemetryLogger();
        });
        // 配置监控和跟踪
        multipleServerHostBuilder.ConfigureServices(services => { services.AddServiceDefaults(); });

        // 构建并启动服务器
        _gameServer = multipleServerHostBuilder.Build();

        await _gameServer.StartAsync();
    }

    #endregion

    #region WebSocket

    /// <summary>
    /// WebSocket 消息处理方法。
    /// </summary>
    /// <remarks>
    /// WebSocket message processing method.
    /// Handles WebSocket messages; only processes binary message types.
    /// </remarks>
    /// <param name="session">WebSocket 会话对象 / WebSocket session object</param>
    /// <param name="messagePackage">接收到的消息包 / Received message package</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
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
