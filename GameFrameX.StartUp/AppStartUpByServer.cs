using System.Net;
using System.Reflection;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Message;
using GameFrameX.StartUp.Options;
using GameFrameX.SuperSocket.Connection;
using GameFrameX.SuperSocket.Primitives;
using GameFrameX.SuperSocket.ProtoBase;
using GameFrameX.SuperSocket.Server;
using GameFrameX.SuperSocket.Server.Abstractions;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.Server.Host;
using GameFrameX.SuperSocket.WebSocket;
using GameFrameX.SuperSocket.WebSocket.Server;
using GameFrameX.Utility;
using GameFrameX.Utility.Extensions;
using GameFrameX.Utility.Setting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
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
    /// 消息编码处理器 - 用于将消息编码成二进制格式
    /// </summary>
    protected IMessageEncoderHandler MessageEncoderHandler { get; private set; }

    /// <summary>
    /// 消息解码处理器 - 用于将二进制数据解码成消息对象
    /// </summary>
    protected IMessageDecoderHandler MessageDecoderHandler { get; private set; }

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
        IMessageDecompressHandler messageDecompressHandler, List<BaseHttpHandler> baseHandler, Func<string, BaseHttpHandler> httpFactory, List<IHttpAopHandler> aopHandlerTypes = null, LogLevel minimumLevelLogLevel = LogLevel.Debug)
        where TMessageDecoderHandler : class, IMessageDecoderHandler, IPackageDecoder<IMessage>, new()
        where TMessageEncoderHandler : class, IMessageEncoderHandler, IPackageEncoder<IMessage>, new()
    {
        // 先启动TCP服务器
        await StartServer<TMessageDecoderHandler, TMessageEncoderHandler>(baseHandler, httpFactory, aopHandlerTypes, minimumLevelLogLevel);

        // 初始化消息处理器
        if (MessageDecoderHandler.IsNull())
        {
            MessageDecoderHandler = Activator.CreateInstance<TMessageDecoderHandler>();
        }

        if (MessageEncoderHandler.IsNull())
        {
            MessageEncoderHandler = Activator.CreateInstance<TMessageEncoderHandler>();
        }

        // 设置压缩/解压处理器
        if (MessageDecoderHandler.IsNotNull())
        {
            MessageDecoderHandler.SetDecompressionHandler(messageDecompressHandler);
        }

        if (MessageEncoderHandler.IsNotNull())
        {
            MessageEncoderHandler.SetCompressionHandler(messageCompressHandler);
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
        LogHelper.Info($"客户端断开连接 - SessionID: {appSession.SessionID}, 断开原因: {disconnectEventArgs.Reason}");
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 客户端连接成功时的处理方法
    /// </summary>
    /// <param name="appSession">新建立的会话对象</param>
    protected virtual ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info($"新客户端连接 - SessionID: {appSession.SessionID}, 远程终端: {appSession.RemoteEndPoint}");
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
            LogHelper.Debug($"收到消息 - 服务器类型: [{ServerType}], 消息内容: {message.ToFormatMessageString()}");
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
            await handler.Init(message, netWorkChannel);
            await handler.InnerAction(timeout, cancellationToken);
        }

        await Task.Run(InvokeAction, cancellationToken);
    }

    #region TCP Server

    private IServer _gameServer;

    /// <summary>
    /// 启动TCP服务器
    /// </summary>
    /// <typeparam name="TMessageDecoderHandler">消息解码处理器类型</typeparam>
    /// <typeparam name="TMessageEncoderHandler">消息编码处理器类型</typeparam>
    /// <param name="baseHandler">HTTP处理器列表,用于处理不同的HTTP请求</param>
    /// <param name="httpFactory">HTTP处理器工厂,根据命令标识符创建对应的处理器实例</param>
    /// <param name="aopHandlerTypes">AOP处理器列表,用于在HTTP请求处理前后执行额外的逻辑</param>
    /// <param name="minimumLevelLogLevel">日志记录的最小级别,用于控制日志输出</param>
    private async Task StartServer<TMessageDecoderHandler, TMessageEncoderHandler>(List<BaseHttpHandler> baseHandler, Func<string, BaseHttpHandler> httpFactory, List<IHttpAopHandler> aopHandlerTypes = null, LogLevel minimumLevelLogLevel = LogLevel.Debug)
        where TMessageDecoderHandler : class, IMessageDecoderHandler, IPackageDecoder<IMessage>, new()
        where TMessageEncoderHandler : class, IMessageEncoderHandler, IPackageEncoder<IMessage>, new()
    {
        var hostBuilder = MultipleServerHostBuilder.Create();
        // 检查TCP端口是否可用
        if (Setting.InnerPort > 0 && NetHelper.PortIsAvailable(Setting.InnerPort))
        {
            LogHelper.InfoConsole($"启动 [TCP] 服务器 - 类型: {ServerType}, 地址: {Setting.InnerIp}, 端口: {Setting.InnerPort}");
            hostBuilder.AddServer<IMessage, MessageObjectPipelineFilter>(builder =>
            {
                builder.ConfigureSuperSocket(ConfigureSuperSocket)
                       .UseClearIdleSession()
                       .UsePackageDecoder<TMessageDecoderHandler>()
                       .UsePackageEncoder<TMessageEncoderHandler>()
                       .UseSessionHandler(OnConnected, OnDisconnected)
                       .UsePackageHandler(PackageHandler, PackageErrorHandler)
                       .UseInProcSessionContainer();
            });
            LogHelper.InfoConsole($"启动 [TCP] 服务器启动完成 - 类型: {ServerType}, 地址: {Setting.InnerIp}, 端口: {Setting.InnerPort}");
        }
        else
        {
            LogHelper.WarnConsole($"启动 [TCP] 服务器启动失败 - 类型: {ServerType}, 地址: {Setting.InnerIp}, 端口: {Setting.InnerPort}, 原因: 端口无效或已被占用");
        }

        // 检查WebSocket端口是否可用
        if (Setting.WsPort is > 0 and < ushort.MaxValue && NetHelper.PortIsAvailable(Setting.WsPort))
        {
            LogHelper.InfoConsole("启动 [WebSocket] 服务器...");

            // 配置并启动WebSocket服务器
            hostBuilder.AddWebSocketServer(builder =>
            {
                builder.ConfigureSuperSocket(ConfigureWebServer)
                       .UseWebSocketMessageHandler(WebSocketMessageHandler)
                       .UseSessionHandler(OnConnected, OnDisconnected);
            });
            LogHelper.InfoConsole($"启动 [WebSocket] 服务器启动完成 - 类型: {ServerType}, 端口: {Setting.WsPort}");
        }
        else
        {
            LogHelper.WarnConsole($"启动 [WebSocket] 服务器启动失败 - 类型: {ServerType}, 端口: {Setting.WsPort}, 原因: 端口无效或已被占用");
        }

        await StartHttpServer(baseHandler, httpFactory, aopHandlerTypes, minimumLevelLogLevel);

        // 配置日志
        hostBuilder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog(Log.Logger, true);
            logging.SetMinimumLevel(minimumLevelLogLevel);
        });

        // 配置监控和跟踪
        hostBuilder.ConfigureServices(services =>
        {
            services.AddOpenTelemetry()
                    .ConfigureResource(configure => { configure.AddService(Setting.ServerName + "-" + Setting.TagName, "GameFrameX").AddTelemetrySdk(); })
                    .WithTracing(configure =>
                    {
                        configure.AddAspNetCoreInstrumentation();
                        configure.AddConsoleExporter();
                    });
        });

        // 构建并启动服务器
        _gameServer = hostBuilder.BuildAsServer();

        var messageEncoderHandler = (IMessageEncoderHandler)_gameServer.ServiceProvider.GetService<IPackageEncoder<IMessage>>();
        var messageDecoderHandler = (IMessageDecoderHandler)_gameServer.ServiceProvider.GetService<IPackageDecoder<IMessage>>();

        MessageDecoderHandler = messageDecoderHandler;
        MessageEncoderHandler = messageEncoderHandler;

        await _gameServer.StartAsync();
    }

    #endregion

    #region WebSocket

    /// <summary>
    /// 配置WebSocket服务器参数
    /// </summary>
    private void ConfigureWebServer(ServerOptions serverOptions)
    {
        var listenOptions = new ListenOptions
        {
            Ip = IPAddress.Any.ToString(),
            Port = Setting.WsPort,
        };
        serverOptions.AddListener(listenOptions);
    }

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
        var message = MessageDecoderHandler.Handler(ref readOnlySequence);
        await PackageHandler(session, message);
    }

    #endregion
}