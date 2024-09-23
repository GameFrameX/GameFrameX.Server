using GameFrameX.Apps.Common.Session;
using GameFrameX.Config;
using GameFrameX.Config.item;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Message;
using GameFrameX.NetWork.Messages;
using GameFrameX.SuperSocket.Connection;
using GameFrameX.SuperSocket.Primitives;
using GameFrameX.SuperSocket.ProtoBase;
using GameFrameX.SuperSocket.Server.Abstractions;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.Server.Host;
using GameFrameX.SuperSocket.WebSocket;
using GameFrameX.SuperSocket.WebSocket.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using CloseReason = GameFrameX.SuperSocket.Connection.CloseReason;

namespace GameFrameX.Hotfix.StartUp;

/// <summary>
/// 路由服务器.最后启动。
/// </summary>
internal partial class AppStartUpHotfixGame
{
    /// <summary>
    /// 是否启用重连
    /// </summary>
    protected override bool IsEnableReconnection { get; } = false;

    /// <summary>
    /// 是否启用心跳
    /// </summary>
    protected override bool IsEnableHeartBeat { get; } = false;

    public override async Task StartAsync()
    {
        // 启动网络服务
        StartTcpServer();
        StartWebSocketServer();
        // 设置压缩和解压缩
        MessageEncoderHandler.SetCompressionHandler(new BaseMessageCompressHandler());
        MessageDecoderHandler.SetDecompressionHandler(new BaseMessageDecompressHandler());
        // 启动Http服务
        await HttpServer.Start(Setting.HttpPort, Setting.HttpsPort, HotfixManager.GetHttpHandler);
    }

    public async void RunServer(bool reload = false)
    {
        // 不管是不是重启服务器，都要加载配置
        ConfigComponent.Instance.LoadConfig();
        if (reload)
        {
            ActorManager.ClearAgent();
            return;
        }

        await StartAsync();
    }

    /// <summary>
    /// 服务器。对外提供服务
    /// </summary>
    private IServer _tcpService;

    /// <summary>
    /// WS服务器
    /// </summary>
    private IHost _webSocketServer;


    /// <summary>
    /// 启动WebSocket
    /// </summary>
    private async void StartWebSocketServer()
    {
        if (Setting.WsPort > 0)
        {
            LogHelper.Info("启动 WebSocket 服务器开始...");
            _webSocketServer = WebSocketHostBuilder.Create()
                                                   .UseWebSocketMessageHandler(WebSocketMessageHandler)
                                                   .UseSessionHandler(OnConnected, OnDisconnected).ConfigureAppConfiguration((Action<HostBuilderContext, IConfigurationBuilder>)(Action<HostBuilderContext, IConfigurationBuilder>)(ConfigureWebServer)).Build();
            await _webSocketServer.StartAsync();
            LogHelper.Info("启动 WebSocket 服务器完成...");
        }
    }

    /// <summary>
    /// 启动TCP
    /// </summary>
    private async void StartTcpServer()
    {
        if (Setting.InnerPort > 0)
        {
            LogHelper.Info("启动 TCP 服务器开始...");
            _tcpService = SuperSocketHostBuilder
                          .Create<INetworkMessage, MessageObjectPipelineFilter>()
                          .ConfigureSuperSocket(ConfigureSuperSocket)
                          .UseClearIdleSession()
                          .UsePackageDecoder<BaseMessageDecoderHandler>()
                          .UsePackageEncoder<BaseMessageEncoderHandler>()
                          .UseSessionHandler(OnConnected, OnDisconnected)
                          .UsePackageHandler(MessagePackageHandler, ClientErrorHandler)
                          .UseInProcSessionContainer()
                          .ConfigureLogging(ConfigureLogging)
                          .BuildAsServer();

            // 获取消息处理器
            var messageEncoderHandler = (BaseMessageEncoderHandler)_tcpService.ServiceProvider.GetService<IPackageEncoder<INetworkMessage>>();
            var messageDecoderHandler = (BaseMessageDecoderHandler)_tcpService.ServiceProvider.GetService<IPackageDecoder<INetworkMessage>>();
            SetMessageHandler(messageEncoderHandler, messageDecoderHandler);
            await _tcpService.StartAsync();
            LogHelper.Info("启动 TCP 服务器完成...");
        }
    }

    private void ConfigureLogging(HostBuilderContext context, ILoggingBuilder loggingBuilder)
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddSerilog(Serilog.Log.Logger);
    }

    private ValueTask<bool> ClientErrorHandler(IAppSession appSession, PackageHandlingException<INetworkMessage> arg2)
    {
        LogHelper.Error(arg2.ToString());
        return ValueTask.FromResult(true);
    }

    private ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs disconnectEventArgs)
    {
        LogHelper.Info("有外部客户端网络断开连接成功！。断开信息：" + appSession.SessionID + "  " + disconnectEventArgs.Reason);
        SessionManager.Remove(appSession.SessionID);
        return ValueTask.CompletedTask;
    }

    private ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info("有外部客户端网络连接成功！。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
        var netChannel = new DefaultNetWorkChannel(appSession, Setting, MessageEncoderHandler, null, appSession is WebSocketSession);
        var session = new Session(appSession.SessionID, netChannel);
        SessionManager.Add(session);

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 处理收到的WS消息
    /// </summary>
    /// <param name="session"></param>
    /// <param name="message"></param>
    private async ValueTask WebSocketMessageHandler(WebSocketSession session, WebSocketPackage message)
    {
        if (message.OpCode != OpCode.Binary)
        {
            await session.CloseAsync(CloseReason.ProtocolError);
            return;
        }

        var readOnlySequence = message.Data;
        var messageObject = MessageDecoderHandler.Handler(ref readOnlySequence);
        await MessagePackageHandler(session, messageObject);
    }


    /// <summary>
    /// 处理收到的消息结果
    /// </summary>
    /// <param name="appSession"></param>
    /// <param name="messageObject"></param>
    private ValueTask MessagePackageHandler(IAppSession appSession, INetworkMessage messageObject)
    {
        if (messageObject is MessageObject message)
        {
            if (Setting.IsDebug && Setting.IsDebugReceive)
            {
                LogHelper.Debug($"---收到{messageObject.ToFormatMessageString()}");
            }

            var netWorkChannel = SessionManager.GetChannel(appSession.SessionID);
            if (messageObject.OperationType == MessageOperationType.HeartBeat)
            {
                // LogHelper.Info("收到心跳请求:" + req.Timestamp);
                ReplyHeartBeat(netWorkChannel, message);
                // 心跳消息
                return ValueTask.CompletedTask;
            }

            var handler = HotfixManager.GetTcpHandler(message.MessageId);
            if (handler == null)
            {
                LogHelper.Error($"找不到[{message.MessageId}][{messageObject.GetType()}]对应的handler");
                return ValueTask.CompletedTask;
            }

            async void InvokeAction()
            {
                await handler.Init(message, netWorkChannel);
                await handler.InnerAction();
            }

            Task.Run(InvokeAction);
        }

        return ValueTask.CompletedTask;
    }

    private void ConfigureWebServer(HostBuilderContext context, IConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection(new Dictionary<string, string>()
                                          { { "serverOptions:name", "GameServer" }, { "serverOptions:listeners:0:ip", "Any" }, { "serverOptions:listeners:0:port", Setting.WsPort.ToString() } });
    }

    public override async Task StopAsync(string message = "")
    {
        // 关闭网络服务
        if (_webSocketServer != null)
        {
            await _webSocketServer.StopAsync();
        }

        if (_tcpService != null)
        {
            await _tcpService.StopAsync();
        }

        await HttpServer.Stop();
        // 断开所有连接
        await SessionManager.RemoveAll();
        // 取消所有未执行定时器
        await QuartzTimer.Stop();
        // 保证actor之前的任务都执行完毕
        await ActorManager.AllFinish();
        // 存储所有数据
        await GlobalTimer.Stop();
        // 删除所有actor
        await ActorManager.RemoveAll();
    }
}