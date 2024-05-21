using GameFrameX.Launcher.PipelineFilter;
using GameFrameX.Launcher.StartUp.Gateway;
using GameFrameX.Proto.BuiltIn;

/// <summary>
/// 网关服务器
/// </summary>
[StartUpTag(ServerType.Gateway)]
internal sealed class AppStartUpGateway : AppStartUpBase
{
    private IServer tcpService;

    private AsyncTcpSession client;
    MessageActorGatewayEncoderHandler messageEncoderHandler = new MessageActorGatewayEncoderHandler();

    MessageActorGatewayDecoderHandler messageDecoderHandler = new MessageActorGatewayDecoderHandler();
    ReqActorHeartBeat reqHeartBeat = new ReqActorHeartBeat();
    RpcSession rpcSession = new RpcSession();

    public override async Task EnterAsync()
    {
        try
        {
            LogHelper.Info($"启动服务器{Setting.ServerType} 开始! address: {Setting.LocalIp}  port: {Setting.TcpPort}");
            await StartServer();

            StartClient();

            TimeSpan delay = TimeSpan.FromSeconds(5);
            await Task.Delay(delay);

            await AppExitToken;
            await Stop("用户退出");
            Console.Write("全部断开...");
            LogHelper.Info("Done!");
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            await Stop(e.Message);
            AppExitSource.TrySetException(e);
        }
    }

    private void StartClient()
    {
        client = new AsyncTcpSession();
        client.Connected += ClientOnConnected;
        client.Closed += ClientOnClosed;
        client.DataReceived += ClientOnDataReceived;
        client.Error += ClientOnError;
        ConnectToDiscovery();
    }

    public override async Task Stop(string message = "")
    {
        LogHelper.Info($"服务器{Setting.ServerType} 停止! address: {Setting.LocalIp}  port: {Setting.TcpPort}");
        client.Close();
        await tcpService.StopAsync();
        await base.Stop(message);
    }

    #region Server

    private async Task StartServer()
    {
        tcpService = SuperSocketHostBuilder.Create<IMessage, MessageObjectPipelineFilter>()
            .ConfigureSuperSocket(ConfigureSuperSocket)
            .UseClearIdleSession()
            .UsePackageDecoder<MessageActorGatewayDecoderHandler>()
            // .UsePackageEncoder<MessageActorDiscoveryEncoderHandler>()
            .UseSessionHandler(OnConnected, OnDisconnected)
            .UsePackageHandler(PackageHandler, ClientErrorHandler)
            .UseInProcSessionContainer()
            .BuildAsServer();

        await tcpService.StartAsync();
    }

    private ValueTask<bool> ClientErrorHandler(IAppSession appSession, PackageHandlingException<IMessage> exception)
    {
        return ValueTask.FromResult(true);
    }

    private ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs disconnectEventArgs)
    {
        LogHelper.Info("有路由客户端网络断开连接成功！。断开信息：" + appSession.SessionID + "  " + disconnectEventArgs.Reason);
        GameClientSessionManager.RemoveSession(appSession.SessionID); //移除
        return ValueTask.CompletedTask;
    }

    private ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info("有路由客户端网络连接成功！。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
        // var gameSession = new GameSession(socketClient.IP, socketClient);
        var netChannel = new DefaultNetChannel(appSession, messageEncoderHandler, rpcSession);
        GameClientSessionManager.SetSession(appSession.SessionID, netChannel); //移除
        return ValueTask.CompletedTask;
    }


    // readonly MessageActorDiscoveryEncoderHandler messageEncoderHandler = new MessageActorDiscoveryEncoderHandler();

    private async ValueTask PackageHandler(IAppSession session, IMessage messageObject)
    {
        if (messageObject is MessageActorObject msg)
        {
            var messageId = msg.MessageId;
            if (Setting.IsDebug && Setting.IsDebugReceive)
            {
                LogHelper.Debug($"---收到消息ID:[{messageId}] ==>消息类型:{msg.GetType()} 消息内容:{messageObject}");
            }

            // 发送
            var response = new RespActorHeartBeat()
            {
                Timestamp = TimeHelper.UnixTimeSeconds()
            };
            var result = messageEncoderHandler.RpcReplyHandler(msg.UniqueId, response);
            await session.SendAsync(result);
        }
    }

    #endregion

    #region Client

    protected override void HeartBeatTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        //心跳包
        /*if (client.IsConnected)
        {
            reqHeartBeat.Timestamp = TimeHelper.UnixTimeSeconds();
            reqHeartBeat.UniqueId = UtilityIdGenerator.GetNextUniqueId();
            SendMessage(reqHeartBeat);
        }*/
    }

    protected override void ReconnectionTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        ConnectToDiscovery();
    }

    private void ConnectToDiscovery()
    {
        client.Connect(new DnsEndPoint(Setting.CenterUrl, Setting.GrpcPort));
    }

    private void ClientOnDataReceived(object sender, DataEventArgs e)
    {
        var messageObject = (MessageObject)messageDecoderHandler.Handler(e.Data.ReadBytes(e.Offset, e.Length));
        if (Setting.IsDebug && Setting.IsDebugReceive)
        {
            LogHelper.Debug($"---收到消息 ==>消息类型:{messageObject.GetType()} 消息内容:{messageObject}");
        }
    }

    private void ClientOnConnected(object sender, EventArgs e)
    {
        LogHelper.Info("和中心服务器链接成功, 开始心跳");
        HeartBeatTimer.Start();
        ReconnectionTimer.Stop();
        ReqRegisterServer reqRegisterServer = new ReqRegisterServer
        {
            ServerID = Setting.ServerId,
            ServerType = Setting.ServerType,
            ServerName = Setting.ServerName,
            ServerIP = Setting.LocalIp,
            ServerPort = Setting.TcpPort
        };
        SendMessage(reqRegisterServer);
    }

    private void ClientOnClosed(object sender, EventArgs eventArgs)
    {
        LogHelper.Info("和中心服务器网络连接断开, 开始重连：断开信息:" + eventArgs);
        HeartBeatTimer.Stop();
        ReconnectionTimer.Start();
    }

    private void ClientOnError(object sender, SuperSocket.ClientEngine.ErrorEventArgs errorEventArgs)
    {
        LogHelper.Info("和中心服务器连接错误, 开始重连:错误信息：" + errorEventArgs.Exception);
        // 开启重连
        HeartBeatTimer.Stop();
        ReconnectionTimer.Start();
    }

    private void SendMessage(IMessage message)
    {
        var span = messageEncoderHandler.Handler(message);
        if (Setting.IsDebug && Setting.IsDebugSend)
        {
            LogHelper.Debug($"---发送消息ID:[{ProtoMessageIdHandler.GetReqMessageIdByType(message.GetType())}] ==>消息类型:{message.GetType()} 消息内容:{message}");
        }

        client.TrySend(span);
        // ArrayPool<byte>.Shared.Return(span);
    }

    #endregion


    protected override void Init()
    {
        if (Setting == null)
        {
            Setting = new AppSetting
            {
                ServerId = 2000,
                ServerType = ServerType.Gateway,
                TcpPort = 22000,
                GrpcPort = 33300,
                CenterUrl = "127.0.0.1",
                LocalIp = "127.0.0.1"
            };
            if (PlatformRuntimeHelper.IsLinux)
            {
                Setting.CenterUrl = "discovery";
                Setting.LocalIp = "gateway";
            }
        }

        base.Init();
    }
}