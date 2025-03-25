using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Message;
using GameFrameX.StartUp.Abstractions;
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
/// 程序启动器基类
/// </summary>
public abstract partial class AppStartUpBase : IAppStartUp
{
    /// <summary>
    /// 消息编码处理器
    /// </summary>
    protected IMessageEncoderHandler MessageEncoderHandler { get; private set; }

    /// <summary>
    /// 消息解码处理器
    /// </summary>
    protected IMessageDecoderHandler MessageDecoderHandler { get; private set; }

    /// <summary>
    /// 启动服务器
    /// </summary>
    /// <param name="messageCompressHandler">消息编码的时候使用的压缩处理器，如果为空则不处理压缩消息</param>
    /// <param name="messageDecompressHandler">消息解码的时候使用的解压处理器,如果为空则不处理压缩消息</param>
    /// <typeparam name="TMessageDecoderHandler">消息解码处理器类型</typeparam>
    /// <typeparam name="TMessageEncoderHandler">消息编码处理器类型</typeparam>
    protected async Task StartServerAsync<TMessageDecoderHandler, TMessageEncoderHandler>(IMessageCompressHandler messageCompressHandler = null, IMessageDecompressHandler messageDecompressHandler = null) where TMessageDecoderHandler : class, IMessageDecoderHandler, IPackageDecoder<IMessage>, new() where TMessageEncoderHandler : class, IMessageEncoderHandler, IPackageEncoder<IMessage>, new()
    {
        await StartTcpServer<TMessageDecoderHandler, TMessageEncoderHandler>();
        await StartWebSocketServer();

        if (MessageDecoderHandler.IsNull())
        {
            MessageDecoderHandler = Activator.CreateInstance<TMessageDecoderHandler>();
        }

        if (MessageEncoderHandler.IsNull())
        {
            MessageEncoderHandler = Activator.CreateInstance<TMessageEncoderHandler>();
        }

        if (MessageDecoderHandler.IsNotNull())
        {
            MessageDecoderHandler.SetDecompressionHandler(messageDecompressHandler);
        }

        if (MessageEncoderHandler.IsNotNull())
        {
            MessageEncoderHandler.SetCompressionHandler(messageCompressHandler);
        }

        GlobalSettings.LaunchTime = DateTime.Now;
        GlobalSettings.IsAppRunning = true;
    }

    /// <summary>
    /// 停止服务器
    /// </summary>
    protected void StopServer()
    {
        GlobalSettings.IsAppRunning = false;
        StopTcpServer();
        StopWebSocketServer();
    }

    /// <summary>
    /// 消息处理异常
    /// </summary>
    /// <param name="appSession"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    protected virtual ValueTask<bool> PackageErrorHandler(IAppSession appSession, PackageHandlingException<IMessage> exception)
    {
        return ValueTask.FromResult(true);
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    /// <param name="appSession"></param>
    /// <param name="disconnectEventArgs"></param>
    /// <returns></returns>
    protected virtual ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs disconnectEventArgs)
    {
        LogHelper.Info("有外部客户端网络断开连接成功！。断开信息：" + appSession.SessionID + "  " + disconnectEventArgs.Reason);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 有连接连上的时候触发
    /// </summary>
    /// <param name="appSession"></param>
    /// <returns></returns>
    protected virtual ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info("有外部客户端网络连接成功！。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 有消息包收到的时候触发
    /// </summary>
    /// <param name="session"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    protected virtual ValueTask PackageHandler(IAppSession session, IMessage message)
    {
        if (Setting.IsDebug && Setting.IsDebugReceive)
        {
            LogHelper.Debug($"---收到外部发给[{ServerType}]的消息  {message.ToFormatMessageString()}");
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 消息处理
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="message"></param>
    /// <param name="netWorkChannel"></param>
    /// <param name="timeout">处理超时时间</param>
    /// <param name="cancellationToken">用于取消的令牌</param>
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
    /// <typeparam name="TMessageDecoderHandler"></typeparam>
    /// <typeparam name="TMessageEncoderHandler"></typeparam>
    private async Task StartTcpServer<TMessageDecoderHandler, TMessageEncoderHandler>() where TMessageDecoderHandler : class, IMessageDecoderHandler, IPackageDecoder<IMessage>, new() where TMessageEncoderHandler : class, IMessageEncoderHandler, IPackageEncoder<IMessage>, new()
    {
        if (Setting.InnerPort > 0 && Net.PortIsAvailable(Setting.InnerPort))
        {
            LogHelper.InfoConsole($"启动 TCP 服务器 {ServerType} 开始! address: {Setting.InnerIp}  port: {Setting.InnerPort}");
            var hostBuilder = SuperSocketHostBuilder
                              .Create<IMessage, MessageObjectPipelineFilter>()
                              .ConfigureSuperSocket(ConfigureSuperSocket)
                              .UseClearIdleSession()
                              .UsePackageDecoder<TMessageDecoderHandler>()
                              .UsePackageEncoder<TMessageEncoderHandler>()
                              .UseSessionHandler(OnConnected, OnDisconnected)
                              .UsePackageHandler(PackageHandler, PackageErrorHandler)
                              .UseInProcSessionContainer()
                ;

            hostBuilder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog(Serilog.Log.Logger, true);
            });
            _tcpService = hostBuilder.BuildAsServer();
            var messageEncoderHandler = (IMessageEncoderHandler)_tcpService.ServiceProvider.GetService<IPackageEncoder<IMessage>>();
            var messageDecoderHandler = (IMessageDecoderHandler)_tcpService.ServiceProvider.GetService<IPackageDecoder<IMessage>>();

            MessageDecoderHandler = messageDecoderHandler;
            MessageEncoderHandler = messageEncoderHandler;

            await _tcpService.StartAsync();

            LogHelper.InfoConsole($"启动 TCP 服务器 {ServerType} 端口: {Setting.InnerPort} 结束!");
        }
        else
        {
            LogHelper.WarnConsole($"启动 TCP 服务器 {ServerType} 失败，内网端口不能小于0,且内网端口不能大于65535 或者 端口被占用,检查端口值是否正确");
        }
    }

    /// <summary>
    /// 关闭TCP服务器
    /// </summary>
    private async void StopTcpServer()
    {
        // 关闭Tcp网络服务
        if (_tcpService != null)
        {
            await _tcpService.StopAsync();
        }

        _tcpService = null;
    }

    #endregion


    #region WebSocket

    /// <summary>
    /// WS服务器
    /// </summary>
    private IHost _webSocketServer;

    /// <summary>
    /// 启动WebSocket
    /// </summary>
    protected async Task StartWebSocketServer()
    {
        if (Setting.WsPort is > 0 and < ushort.MaxValue && Net.PortIsAvailable(Setting.WsPort))
        {
            LogHelper.InfoConsole("启动 WebSocket 服务器开始...");
            _webSocketServer = WebSocketHostBuilder.Create()
                                                   .UseWebSocketMessageHandler(WebSocketMessageHandler)
                                                   .UseSessionHandler(OnConnected, OnDisconnected).ConfigureAppConfiguration((Action<HostBuilderContext, IConfigurationBuilder>)ConfigureWebServer).Build();
            await _webSocketServer.StartAsync();
            LogHelper.InfoConsole($"启动 WebSocket 服务器 {ServerType} 完成...端口:{Setting.WsPort}");
        }
        else
        {
            LogHelper.WarnConsole($"启动 WebSocket 服务器 {ServerType} 失败，内网端口不能小于0,内网端口不能大于65535 或者 端口被占用，检查端口值是否正确");
        }
    }

    /// <summary>
    /// 关闭WebSocket
    /// </summary>
    private async void StopWebSocketServer()
    {
        // 关闭WS网络服务
        if (_webSocketServer != null)
        {
            await _webSocketServer.StopAsync();
        }

        _webSocketServer = null;
    }

    private void ConfigureWebServer(HostBuilderContext context, IConfigurationBuilder builder)
    {
        var paramsDict = new Dictionary<string, string>();
        paramsDict.Add("serverOptions:listeners:0:port", Setting.WsPort.ToString());
        paramsDict.Add("serverOptions:listeners:0:ip", Setting.InnerIp.IsNullOrWhiteSpace() ? "Any" : Setting.InnerIp);
        paramsDict.Add("serverOptions:name", Setting.ServerName);
        builder.AddInMemoryCollection(paramsDict);
    }

    /// <summary>
    /// 处理收到的WS消息
    /// </summary>
    /// <param name="session">连接对象</param>
    /// <param name="messagePackage">消息包对象</param>
    private async ValueTask WebSocketMessageHandler(WebSocketSession session, WebSocketPackage messagePackage)
    {
        if (messagePackage.OpCode != OpCode.Binary)
        {
            // 不是二进制消息，直接关闭网络隧道
            await session.CloseAsync(CloseReason.ProtocolError);
            return;
        }

        var readOnlySequence = messagePackage.Data;
        var message = MessageDecoderHandler.Handler(ref readOnlySequence);
        await PackageHandler(session, message);
    }

    #endregion
}