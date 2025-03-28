using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Message;
using GameFrameX.SuperSocket.Connection;
using GameFrameX.SuperSocket.Primitives;
using GameFrameX.SuperSocket.ProtoBase;
using GameFrameX.SuperSocket.Server.Abstractions;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.Server.Host;
using GameFrameX.SuperSocket.WebSocket;
using GameFrameX.SuperSocket.WebSocket.Server;
using GameFrameX.Utility;
using GameFrameX.Utility.Extensions;
using GameFrameX.Utility.Setting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
    /// <param name="messageCompressHandler">消息编码的时候使用的压缩处理器，如果为空则不处理压缩消息</param>
    /// <param name="messageDecompressHandler">消息解码的时候使用的解压处理器,如果为空则不处理压缩消息</param>
    /// <typeparam name="TMessageDecoderHandler">消息解码处理器类型，必须实现IMessageDecoderHandler和IPackageDecoder接口</typeparam>
    /// <typeparam name="TMessageEncoderHandler">消息编码处理器类型，必须实现IMessageEncoderHandler和IPackageEncoder接口</typeparam>
    protected async Task StartServerAsync<TMessageDecoderHandler, TMessageEncoderHandler>(
        IMessageCompressHandler messageCompressHandler = null,
        IMessageDecompressHandler messageDecompressHandler = null)
        where TMessageDecoderHandler : class, IMessageDecoderHandler, IPackageDecoder<IMessage>, new()
        where TMessageEncoderHandler : class, IMessageEncoderHandler, IPackageEncoder<IMessage>, new()
    {
        // 先启动TCP服务器
        await StartTcpServer<TMessageDecoderHandler, TMessageEncoderHandler>();
        // 再启动WebSocket服务器
        await StartWebSocketServer();

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
        GlobalSettings.LaunchTime = DateTime.Now;
        GlobalSettings.IsAppRunning = true;
    }

    /// <summary>
    /// 停止服务器 - 关闭所有网络服务
    /// </summary>
    protected void StopServer()
    {
        GlobalSettings.IsAppRunning = false;
        StopTcpServer();
        StopWebSocketServer();
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

    private IServer _tcpService;

    /// <summary>
    /// 启动TCP服务器
    /// </summary>
    /// <typeparam name="TMessageDecoderHandler">消息解码处理器类型</typeparam>
    /// <typeparam name="TMessageEncoderHandler">消息编码处理器类型</typeparam>
    private async Task StartTcpServer<TMessageDecoderHandler, TMessageEncoderHandler>()
        where TMessageDecoderHandler : class, IMessageDecoderHandler, IPackageDecoder<IMessage>, new()
        where TMessageEncoderHandler : class, IMessageEncoderHandler, IPackageEncoder<IMessage>, new()
    {
        // 检查端口是否可用
        if (Setting.InnerPort > 0 && Net.PortIsAvailable(Setting.InnerPort))
        {
            LogHelper.InfoConsole($"启动TCP服务器 - 类型: {ServerType}, 地址: {Setting.InnerIp}, 端口: {Setting.InnerPort}");

            // 配置并构建TCP服务器
            var hostBuilder = SuperSocketHostBuilder
                              .Create<IMessage, MessageObjectPipelineFilter>()
                              .ConfigureSuperSocket(ConfigureSuperSocket)
                              .UseClearIdleSession()
                              .UsePackageDecoder<TMessageDecoderHandler>()
                              .UsePackageEncoder<TMessageEncoderHandler>()
                              .UseSessionHandler(OnConnected, OnDisconnected)
                              .UsePackageHandler(PackageHandler, PackageErrorHandler)
                              .UseInProcSessionContainer();

            // 配置日志
            hostBuilder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog(Log.Logger, true);
            });

            // 构建并启动服务器
            _tcpService = hostBuilder.BuildAsServer();
            var messageEncoderHandler = (IMessageEncoderHandler)_tcpService.ServiceProvider.GetService<IPackageEncoder<IMessage>>();
            var messageDecoderHandler = (IMessageDecoderHandler)_tcpService.ServiceProvider.GetService<IPackageDecoder<IMessage>>();

            MessageDecoderHandler = messageDecoderHandler;
            MessageEncoderHandler = messageEncoderHandler;

            await _tcpService.StartAsync();

            LogHelper.InfoConsole($"TCP服务器启动完成 - 类型: {ServerType}, 端口: {Setting.InnerPort}");
        }
        else
        {
            LogHelper.WarnConsole($"TCP服务器启动失败 - 类型: {ServerType}, 原因: 端口无效或已被占用");
        }
    }

    /// <summary>
    /// 停止TCP服务器
    /// </summary>
    private async void StopTcpServer()
    {
        if (_tcpService != null)
        {
            await _tcpService.StopAsync();
            _tcpService = null;
        }
    }

    #endregion

    #region WebSocket

    /// <summary>
    /// WebSocket服务器实例
    /// </summary>
    private IHost _webSocketServer;

    /// <summary>
    /// 启动WebSocket服务器
    /// </summary>
    protected async Task StartWebSocketServer()
    {
        // 检查WebSocket端口是否可用
        if (Setting.WsPort is > 0 and < ushort.MaxValue && Net.PortIsAvailable(Setting.WsPort))
        {
            LogHelper.InfoConsole("启动WebSocket服务器...");

            // 配置并启动WebSocket服务器
            _webSocketServer = WebSocketHostBuilder.Create()
                                                   .UseWebSocketMessageHandler(WebSocketMessageHandler)
                                                   .UseSessionHandler(OnConnected, OnDisconnected)
                                                   .ConfigureAppConfiguration((Action<HostBuilderContext, IConfigurationBuilder>)ConfigureWebServer)
                                                   .Build();

            await _webSocketServer.StartAsync();
            LogHelper.InfoConsole($"WebSocket服务器启动完成 - 类型: {ServerType}, 端口: {Setting.WsPort}");
        }
        else
        {
            LogHelper.WarnConsole($"WebSocket服务器启动失败 - 类型: {ServerType}, 原因: 端口无效或已被占用");
        }
    }

    /// <summary>
    /// 停止WebSocket服务器
    /// </summary>
    private async void StopWebSocketServer()
    {
        if (_webSocketServer != null)
        {
            await _webSocketServer.StopAsync();
            _webSocketServer = null;
        }
    }

    /// <summary>
    /// 配置WebSocket服务器参数
    /// </summary>
    private void ConfigureWebServer(HostBuilderContext context, IConfigurationBuilder builder)
    {
        var paramsDict = new Dictionary<string, string>
        {
            ["serverOptions:listeners:0:port"] = Setting.WsPort.ToString(),
            ["serverOptions:listeners:0:ip"] = Setting.InnerIp.IsNullOrWhiteSpace() ? "Any" : Setting.InnerIp,
            ["serverOptions:name"] = Setting.ServerName
        };
        builder.AddInMemoryCollection(paramsDict);
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