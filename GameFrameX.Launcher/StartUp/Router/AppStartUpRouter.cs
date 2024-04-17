using GameFrameX.Launcher;
using GameFrameX.Launcher.PipelineFilter;
using GameFrameX.NetWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SuperSocket;
using SuperSocket.ClientEngine;
using SuperSocket.Connection;
using SuperSocket.Server;
using SuperSocket.Server.Abstractions;
using SuperSocket.Server.Abstractions.Session;
using SuperSocket.Server.Host;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;
using ErrorEventArgs = SuperSocket.ClientEngine.ErrorEventArgs;


/// <summary>
/// 路由服务器.最后启动。
/// </summary>
[StartUpTag(ServerType.Router, int.MaxValue)]
internal sealed class AppStartUpRouter : AppStartUpBase
{
    /// <summary>
    /// 链接到网关的客户端
    /// </summary>
    // private TcpClient tcpClient;
    AsyncTcpSession tcpClient;

    /// <summary>
    /// 服务器。对外提供服务
    /// </summary>
    private IServer tcpService;

    /// <summary>
    /// WS服务器
    /// </summary>
    private IHost webSocketServer;

    public override async Task EnterAsync()
    {
        try
        {
            await StartServer();
            LogHelper.Info($"启动服务器 {ServerType} 端口: {Setting.TcpPort} 结束!");
            StartClient();
            await AppExitToken;
            LogHelper.Info("全部断开...");
            await Stop();
            LogHelper.Info("Done!");
        }
        catch (Exception e)
        {
            await Stop(e.Message);
        }
    }

    /// <summary>
    /// 重连定时器
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected override void ReconnectionTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        // 重连到网关服务器
        ConnectToGateWay();
    }

    private void ConnectToGateWay()
    {
        var endPoint = new IPEndPoint(IPAddress.Parse(Setting.CenterUrl), Setting.GrpcPort);
        tcpClient.Connect(endPoint);
    }

    private void StartClient()
    {
        tcpClient = new AsyncTcpSession();
        tcpClient.Closed += ClientOnClosed;
        tcpClient.DataReceived += ClientOnDataReceived;
        tcpClient.Connected += ClientOnConnected;
        tcpClient.Error += ClientOnError;

        LogHelper.Info("开始链接到网关服务器 ...");
        ConnectToGateWay();
    }

    private void ClientOnError(object sender, ErrorEventArgs e)
    {
        LogHelper.Info("和网关服务器链接链接发生错误!" + e);
        ClientOnClosed(tcpClient, e);
    }

    private void ClientOnConnected(object sender, EventArgs e)
    {
        LogHelper.Info("和网关服务器链接链接成功!");
        // 和网关服务器链接成功，关闭重连
        ReconnectionTimer.Stop();
        // 开启和网关服务器的心跳
        HeartBeatTimer.Start();
    }

    private void ClientOnDataReceived(object o, DataEventArgs dataEventArgs)
    {
        LogHelper.Info($"收到网关服务器消息：{dataEventArgs.Data}");
    }

    private void ClientOnClosed(object sender, EventArgs eventArgs)
    {
        LogHelper.Info("和网关服务器链接链接断开!");
        // 和网关服务器链接断开，开启重连
        ReconnectionTimer.Start();
    }

    private async Task StartServer()
    {
        webSocketServer = WebSocketHostBuilder.Create()
            .UseWebSocketMessageHandler(WebSocketMessageHandler)
            .ConfigureAppConfiguration((Action<HostBuilderContext, IConfigurationBuilder>)(ConfigureWebServer)).Build();
        await webSocketServer.StartAsync();
        tcpService = SuperSocketHostBuilder.Create<IMessage, MessageObjectPipelineFilter>()
            .ConfigureSuperSocket(ConfigureSuperSocket)
            .UseClearIdleSession()
            .UsePackageDecoder<MessageRouterDecoderHandler>()
            .UseSessionHandler(OnConnected, OnDisconnected)
            .UsePackageHandler(MessagePackageHandler, ClientErrorHandler)
            .UseInProcSessionContainer()
            .BuildAsServer();

        await tcpService.StartAsync();
    }

    private ValueTask<bool> ClientErrorHandler(IAppSession appSession, PackageHandlingException<IMessage> arg2)
    {
        LogHelper.Error(arg2.ToString());
        return ValueTask.FromResult(true);
    }

    private ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs disconnectEventArgs)
    {
        LogHelper.Info("有外部客户端网络断开连接成功！。断开信息：" + appSession.SessionID + "  " + disconnectEventArgs.Reason);
        GameClientSessionManager.RemoveSession(appSession.SessionID); //移除
        return ValueTask.CompletedTask;
    }

    private ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info("有外部客户端网络连接成功！。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
        var netChannel = new DefaultNetChannel(appSession, messageEncoderHandler);
        GameClientSessionManager.SetSession(appSession.SessionID, netChannel); //移除

        return ValueTask.CompletedTask;
    }

    private static readonly MessageRouterEncoderHandler messageEncoderHandler = new MessageRouterEncoderHandler();
    private static readonly MessageRouterDecoderHandler messageRouterDecoderHandler = new MessageRouterDecoderHandler();

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

        var bytes = message.Data;
        var buffer = bytes.ToArray();
        var messageObject = messageRouterDecoderHandler.Handler(buffer);
        await MessagePackageHandler(session, messageObject);
    }

    /// <summary>
    /// 处理收到的消息结果
    /// </summary>
    /// <param name="appSession"></param>
    /// <param name="messageObject"></param>
    private async ValueTask MessagePackageHandler(IAppSession appSession, IMessage messageObject)
    {
        if (messageObject is MessageObject message)
        {
            var messageId = message.MessageId;
            if (Setting.IsDebug && Setting.IsDebugReceive)
            {
                LogHelper.Debug($"---收到消息ID:[{messageId}] ==>消息类型:{message.GetType()} 消息内容:{messageObject}");
            }

            var handler = HotfixMgr.GetTcpHandler(message.MessageId);
            if (handler == null)
            {
                LogHelper.Error($"找不到[{message.MessageId}][{messageObject.GetType()}]对应的handler");
                return;
            }

            handler.Message = message;
            handler.Channel = GameClientSessionManager.GetSession(appSession.SessionID);
            await handler.Init();
            await handler.InnerAction();
        }
    }

    private void ConfigureWebServer(HostBuilderContext context, IConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection(new Dictionary<string, string>()
            { { "serverOptions:name", "TestServer" }, { "serverOptions:listeners:0:ip", "Any" }, { "serverOptions:listeners:0:port", Setting.WsPort.ToString() } });
    }

    public override async Task Stop(string message = "")
    {
        HeartBeatTimer.Close();
        ReconnectionTimer.Close();
        // tcpClient.Close();
        await webSocketServer.StopAsync();
        await tcpService.StopAsync();
        await base.Stop(message);
    }

    protected override void Init()
    {
        if (Setting == null)
        {
            Setting = new AppSetting
            {
                ServerId = 1000,
                ServerType = ServerType.Router,
                TcpPort = 21000,
                WsPort = 21100,
                WssPort = 21200,
                // 网关配置
                GrpcPort = 22000,
                CenterUrl = "127.0.0.1",
            };
            if (PlatformRuntimeHelper.IsLinux)
            {
                Setting.CenterUrl = "gateway";
            }
        }

        base.Init();
    }
}