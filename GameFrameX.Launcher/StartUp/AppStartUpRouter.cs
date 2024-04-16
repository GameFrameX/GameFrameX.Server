using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SuperSocket.Connection;
using SuperSocket.Server;
using SuperSocket.Server.Abstractions;
using SuperSocket.Server.Abstractions.Session;
using SuperSocket.Server.Host;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;


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
        /*var result = tcpClient.TryConnect();
        if (result.ResultCode != ResultCode.Success)
        {
            // 开始重连
            ReconnectionTimer.Start();
            LogHelper.Info(result.ToString());
            return;
        }*/

        LogHelper.Info("链接到网关服务器成功!");
        ReconnectionTimer.Stop();
        HeartBeatTimer.Start();
    }

    private void StartClient()
    {
        /*tcpClient = new TcpClient();
        tcpClient.Connected = ClientOnConnected; //成功连接到服务器
        tcpClient.Disconnected = ClientOnClosed; //从服务器断开连接，当连接不成功时不会触发。
        tcpClient.Received = (client, e) =>
        {
            //从服务器收到信息。但是一般byteBlock和requestInfo会根据适配器呈现不同的值。
            // var mes = Encoding.UTF8.GetString(e.ByteBlock.Buffer, 0, e.ByteBlock.Len);
            // tcpClient.Logger.Info($"客户端接收到信息：{mes}");
            return EasyTask.CompletedTask;
        };

        // var dnsEndPoint = new DnsEndPoint(Setting.CenterUrl, Setting.GrpcPort);
        IPHost ipHost = new IPHost(IPAddress.Parse(Setting.CenterUrl), Setting.GrpcPort);
        var clientConfig = new TouchSocketConfig()
            .SetRemoteIPHost(ipHost)
            .ConfigureContainer(a => a.AddConsoleLogger());
        //载入配置
        tcpClient.Setup(clientConfig);
        LogHelper.Info("开始链接到网关服务器 ...");
        ConnectToGateWay();*/
    }

    /*private Task ClientOnClosed(ITcpClientBase client, DisconnectEventArgs disconnectEventArgs)
    {
        LogHelper.Info("和网关服务器链接链接断开!");
        // 和网关服务器链接断开，开启重连
        ReconnectionTimer.Start();
        return EasyTask.CompletedTask;
    }

    private Task ClientOnConnected(ITcpClient client, ConnectedEventArgs connectedEventArgs)
    {
        LogHelper.Info("和网关服务器链接链接成功!");
        // 和网关服务器链接成功，关闭重连
        ReconnectionTimer.Stop();
        // 开启和网关服务器的心跳
        HeartBeatTimer.Start();
        return EasyTask.CompletedTask;
    }*/


    private async Task StartServer()
    {
        webSocketServer = WebSocketHostBuilder.Create()
            .UseWebSocketMessageHandler(WebSocketMessageHandler)
            .ConfigureAppConfiguration((Action<HostBuilderContext, IConfigurationBuilder>)(ConfigureWebServer)).Build();
        await webSocketServer.StartAsync();
        tcpService = SuperSocketHostBuilder.Create<IMessage>()
            .ConfigureSuperSocket(ConfigureSuperSocket)
            .UseClearIdleSession()
            // .UsePackageDecoder<MessageRouterDecoderHandler>()
            // .UsePackageEncoder<MessageEncoderHandler>()
            .UseSessionHandler(OnConnected, OnDisconnected)
            // .UsePackageHandler(ClientPackageHandler, ClientErrorHandler)
            .UseInProcSessionContainer()
            .BuildAsServer();

        await tcpService.StartAsync();
    }

    private ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs disconnectEventArgs)
    {
        LogHelper.Info("有外部客户端网络断开连接成功！。断开信息：" + appSession.SessionID + "  " + disconnectEventArgs.Reason);
        return ValueTask.CompletedTask;
    }

    private ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info("有外部客户端网络连接成功！。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
        // var gameSession = new GameSession(socketClient.IP, socketClient);
        // socketClient.SetGameSession(gameSession);
        // var netChannel = new DefaultNetChannel(gameSession, messageEncoderHandler);
        // GameClientSessionManager.SetSession(socketClient.IP, netChannel); //移除

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

        var bytes = message.Data;
        var buffer = bytes.ToArray();
        // var messageObject = messageRouterDecoderHandler.Handler(buffer);
        // await MessagePackageHandler(session, messageObject);
    }

    /// <summary>
    /// 处理收到的消息结果
    /// </summary>
    /// <param name="session"></param>
    /// <param name="messageObject"></param>
    private async ValueTask MessagePackageHandler(IAppSession session, IMessage messageObject)
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

            // RespHeartBeat resp = new RespHeartBeat
            // {
            //     Timestamp = TimeHelper.UnixTimeMilliseconds()
            // };
            // var messageData = messageEncoderHandler.Handler(resp);
            // await session.SendAsync(messageData);
            handler.Message = message;
            // handler.Channel = new DefaultNetChannel(session, messageEncoderHandler); // session;
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