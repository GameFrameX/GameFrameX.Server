using GameFrameX.Launcher.PipelineFilter;
using GameFrameX.Launcher.StartUp.Router;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;


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
    AsyncTcpSession _gatewayClient;

    /// <summary>
    /// 服务器。对外提供服务
    /// </summary>
    private IServer _tcpService;

    /// <summary>
    /// WS服务器
    /// </summary>
    private IHost _webSocketServer;

    RpcSession rpcSession = new RpcSession();

    public override async Task EnterAsync()
    {
        try
        {
            await StartServer();
            LogHelper.Info($"启动服务器 {ServerType} 端口: {Setting.InnerPort} 结束!");
            StartClient();
            _ = Task.Run(RpcHandler);
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

    private void RpcHandler()
    {
        while (true)
        {
            var message = rpcSession.Handler();
            if (message == null)
            {
                Thread.Sleep(1);
                continue;
            }

            SendMessage(message.UniqueId, message.RequestMessage);
        }
    }

    private void SendMessage(long messageUniqueId, IMessage message)
    {
        var span = messageEncoderHandler.RpcHandler(messageUniqueId, message);
        if (Setting.IsDebug && Setting.IsDebugSend)
        {
            LogHelper.Debug($"---发送[{ServerType}] {message.ToMessageString()}");
        }

        _gatewayClient.TrySend(span);
        // ArrayPool<byte>.Shared.Return(span);
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
        var endPoint = new IPEndPoint(IPAddress.Parse(Setting.DiscoveryCenterIp), Setting.DiscoveryCenterPort);
        _gatewayClient.Connect(endPoint);
    }

    private void StartClient()
    {
        _gatewayClient = new AsyncTcpSession();
        _gatewayClient.Closed += ClientOnClosed;
        _gatewayClient.DataReceived += ClientOnDataReceived;
        _gatewayClient.Connected += ClientOnConnected;
        _gatewayClient.Error += ClientOnError;

        LogHelper.Info("开始链接到网关服务器 ...");
        ConnectToGateWay();
    }

    private void ClientOnError(object sender, ErrorEventArgs e)
    {
        LogHelper.Info("和网关服务器链接链接发生错误!" + e);
        ClientOnClosed(_gatewayClient, e);
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
        var messageData = dataEventArgs.Data.ReadBytes(dataEventArgs.Offset, dataEventArgs.Length);
        var messageObject = (IActorResponseMessage)messageRouterDecoderHandler.RpcHandler(messageData);
        rpcSession.Reply(messageObject);
        LogHelper.Info($"收到网关服务器消息：{dataEventArgs.Data}: {messageObject}");
    }

    private void ClientOnClosed(object sender, EventArgs eventArgs)
    {
        LogHelper.Info("和网关服务器链接链接断开!");
        // 和网关服务器链接断开，开启重连
        ReconnectionTimer.Start();
    }

    private async Task StartServer()
    {
        _webSocketServer = WebSocketHostBuilder.Create()
            .UseWebSocketMessageHandler(WebSocketMessageHandler)
            .UseSessionHandler(OnConnected, OnDisconnected)
            .ConfigureAppConfiguration((Action<HostBuilderContext, IConfigurationBuilder>)(ConfigureWebServer)).Build();
        await _webSocketServer.StartAsync();
        _tcpService = SuperSocketHostBuilder.Create<IMessage, MessageObjectPipelineFilter>()
            .ConfigureSuperSocket(ConfigureSuperSocket)
            .UseClearIdleSession()
            .UsePackageDecoder<MessageRouterDecoderHandler>()
            .UseSessionHandler(OnConnected, OnDisconnected)
            .UsePackageHandler(MessagePackageHandler, ClientErrorHandler)
            .UseInProcSessionContainer()
            .BuildAsServer();

        await _tcpService.StartAsync();
    }

    private ValueTask<bool> ClientErrorHandler(IAppSession appSession, PackageHandlingException<IMessage> eventArgs)
    {
        LogHelper.Error(eventArgs.ToString());
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
        var netChannel = new DefaultNetWorkChannel(appSession, messageEncoderHandler, rpcSession, appSession is WebSocketSession);
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
    private ValueTask MessagePackageHandler(IAppSession appSession, IMessage messageObject)
    {
        if (messageObject is MessageObject message)
        {
            if (Setting.IsDebug && Setting.IsDebugReceive)
            {
                LogHelper.Debug($"---收到[{ServerType}] 转发到[{ServerType.Gateway}] [{messageObject}]");
            }

            SendMessage(message.UniqueId, message);
            /*
            var handler = HotfixManager.GetTcpHandler(message.MessageId);
            if (handler == null)
            {
                LogHelper.Error($"找不到[{message.MessageId}][{messageObject.GetType()}]对应的handler");
                return;
            }

            handler.Message = message;
            handler.NetWorkChannel = GameClientSessionManager.GetSession(appSession.SessionID);
            await handler.Init();
            await handler.InnerAction();*/
        }

        return ValueTask.CompletedTask;
    }

    private void ConfigureWebServer(HostBuilderContext context, IConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection(new Dictionary<string, string>()
            { { "serverOptions:name", "TestServer" }, { "serverOptions:listeners:0:ip", "Any" }, { "serverOptions:listeners:0:port", Setting.WsPort.ToString() } });
    }

    public override async Task Stop(string message = "")
    {
        HeartBeatTimer?.Close();
        ReconnectionTimer?.Close();
        _gatewayClient?.Close();
        await _webSocketServer.StopAsync();
        await _tcpService.StopAsync();
        await base.Stop(message);
    }

    protected override void Init()
    {
        if (Setting == null)
        {
            Setting = new AppSetting
            {
                ServerId = 3000,
                ServerType = ServerType.Router,
                InnerPort = 23001,
                WsPort = 23110,
                // 网关配置
                DiscoveryCenterIp = "127.0.0.1",
                DiscoveryCenterPort = 21001,
                // 最大连接数
                MaxClientCount = 3000,
            };
            if (PlatformRuntimeHelper.IsLinux)
            {
                Setting.DiscoveryCenterIp = "gateway";
            }
        }

        base.Init();
    }
}