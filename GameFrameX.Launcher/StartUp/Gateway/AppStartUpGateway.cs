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

    /// <summary>
    /// 和发现中心链接的客户端
    /// </summary>
    private AsyncTcpSession _discoveryCenterClient;

    /// <summary>
    /// 和游戏逻辑服链接的客户端
    /// </summary>
    private List<ClientSession> _gameClientList = new List<ClientSession>();

    MessageActorGatewayEncoderHandler messageEncoderHandler = new MessageActorGatewayEncoderHandler();

    MessageActorGatewayDecoderHandler messageDecoderHandler = new MessageActorGatewayDecoderHandler();
    ReqActorHeartBeat reqHeartBeat = new ReqActorHeartBeat();
    RpcSession rpcSession = new RpcSession();

    public override async Task EnterAsync()
    {
        try
        {
            LogHelper.Info($"启动服务器{Setting.ServerType} 开始! address: {Setting.InnerIp}  port: {Setting.InnerPort}");
            await StartServer();
            StartDiscoveryCenterClient();
            StartGameClient();
            await AppExitToken;
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            AppExitSource.TrySetException(e);
            await Stop(e.Message);
        }
    }


    public override async Task Stop(string message = "")
    {
        LogHelper.Info($"服务器{Setting.ServerType} 停止! address: {Setting.InnerIp}  port: {Setting.InnerPort}");
        _discoveryCenterClient?.Close();
        foreach (var kv in _gameClientList)
        {
            kv.AsyncTcpSession?.Close();
        }

        _gameClientList.Clear();
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
        var netChannel = new DefaultNetWorkChannel(appSession, messageEncoderHandler, rpcSession);
        GameClientSessionManager.SetSession(appSession.SessionID, netChannel); //移除
        return ValueTask.CompletedTask;
    }


    // readonly MessageActorDiscoveryEncoderHandler messageEncoderHandler = new MessageActorDiscoveryEncoderHandler();

    private async ValueTask PackageHandler(IAppSession session, IMessage message)
    {
        if (Setting.IsDebug && Setting.IsDebugReceive && message is BaseMessageObject baseMessageObject)
        {
            LogHelper.Debug($"---收到[{ServerType}] {baseMessageObject.ToMessageString()}");
        }

        if (message is MessageActorObject messageActorObject)
        {
            // 发送
            var response = new RespActorHeartBeat()
            {
                Timestamp = TimeHelper.UnixTimeSeconds()
            };
            var result = messageEncoderHandler.RpcReplyHandler(messageActorObject.UniqueId, response);
            await session.SendAsync(result);
        }
        else if (message is MessageObject messageObject)
        {
        }
    }

    #endregion


    private void StartGameClient()
    {
        // _gameClientList = new AsyncTcpSession();
        // _gameClientList.Connected += GameClientOnConnected;
        // _gameClientList.Closed += GameClientOnClosed;
        // _gameClientList.DataReceived += GameClientOnDataReceived;
        // _gameClientList.Error += GameClientOnError;
    }

    private void ConnectToGame()
    {
        // _gameClientList.Connect(new DnsEndPoint(Setting.DiscoveryCenterIp, Setting.DiscoveryCenterPort));
    }

    private void GameClientOnDataReceived(object sender, DataEventArgs e)
    {
        var messageObject = messageDecoderHandler.Handler(e.Data.ReadBytes(e.Offset, e.Length));
        if (messageObject == null)
        {
            LogHelper.Error("数据解析失败：" + e.Data.ReadBytes(e.Offset, e.Length));
            return;
        }

        if (Setting.IsDebug && Setting.IsDebugReceive)
        {
            LogHelper.Debug($"---收到[{ServerType}] {messageObject.ToMessageString()}");
        }
    }

    private void GameClientOnConnected(object sender, EventArgs e)
    {
        LogHelper.Info("和中心服务器链接成功, 开始心跳");
        HeartBeatTimer.Start();
        ReconnectionTimer.Stop();
        // 这里要注册到发现中心
        ReqRegisterServer reqRegisterServer = new ReqRegisterServer
        {
            ServerID = Setting.ServerId,
            ServerType = Setting.ServerType,
            ServerName = Setting.ServerName,
            InnerIP = Setting.InnerIp,
            InnerPort = Setting.InnerPort,
            OuterIP = Setting.OuterIp,
            OuterPort = Setting.OuterPort
        };
        SendMessage(reqRegisterServer);
    }

    private void GameClientOnClosed(object sender, EventArgs eventArgs)
    {
        LogHelper.Info("和游戏服务器网络连接断开, 开始重连：断开信息:" + eventArgs);
        HeartBeatTimer.Stop();
        ReconnectionTimer.Start();
    }

    private void GameClientOnError(object sender, SuperSocket.ClientEngine.ErrorEventArgs errorEventArgs)
    {
        LogHelper.Info("和游戏服务器连接错误, 开始重连:错误信息：" + errorEventArgs.Exception);
        // 开启重连
        HeartBeatTimer.Stop();
        ReconnectionTimer.Start();
    }

    #region Client

    protected override void HeartBeatTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        //心跳包
        if (_discoveryCenterClient.IsConnected)
        {
            reqHeartBeat.Timestamp = TimeHelper.UnixTimeSeconds();
            reqHeartBeat.UniqueId = UtilityIdGenerator.GetNextUniqueId();
            SendMessage(reqHeartBeat);
        }
    }

    protected override void ReconnectionTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        ConnectToDiscovery();
    }

    private void StartDiscoveryCenterClient()
    {
        _discoveryCenterClient = new AsyncTcpSession();
        _discoveryCenterClient.Connected += DiscoveryCenterClientOnConnected;
        _discoveryCenterClient.Closed += DiscoveryCenterClientOnClosed;
        _discoveryCenterClient.DataReceived += DiscoveryCenterClientOnDataReceived;
        _discoveryCenterClient.Error += DiscoveryCenterClientOnError;
        ConnectToDiscovery();
    }

    private void ConnectToDiscovery()
    {
        _discoveryCenterClient.Connect(new DnsEndPoint(Setting.DiscoveryCenterIp, Setting.DiscoveryCenterPort));
    }

    private void DiscoveryCenterClientOnDataReceived(object sender, DataEventArgs e)
    {
        var messageObject = messageDecoderHandler.Handler(e.Data.ReadBytes(e.Offset, e.Length));
        if (messageObject == null)
        {
            LogHelper.Error("数据解析失败：" + e.Data.ReadBytes(e.Offset, e.Length));
            return;
        }

        if (Setting.IsDebug && Setting.IsDebugReceive)
        {
            LogHelper.Debug($"---收到[{ServerType}] {messageObject.ToMessageString()}");
        }
    }

    private void DiscoveryCenterClientOnConnected(object sender, EventArgs e)
    {
        LogHelper.Info("和中心服务器链接成功, 开始心跳");
        HeartBeatTimer.Start();
        ReconnectionTimer.Stop();
        // 这里要注册到发现中心
        ReqRegisterServer reqRegisterServer = new ReqRegisterServer
        {
            ServerID = Setting.ServerId,
            ServerType = Setting.ServerType,
            ServerName = Setting.ServerName,
            InnerIP = Setting.InnerIp,
            InnerPort = Setting.InnerPort,
            OuterIP = Setting.OuterIp,
            OuterPort = Setting.OuterPort
        };
        SendMessage(reqRegisterServer);
    }

    private void DiscoveryCenterClientOnClosed(object sender, EventArgs eventArgs)
    {
        LogHelper.Info("和中心服务器网络连接断开, 开始重连：断开信息:" + eventArgs);
        HeartBeatTimer.Stop();
        ReconnectionTimer.Start();
    }

    private void DiscoveryCenterClientOnError(object sender, SuperSocket.ClientEngine.ErrorEventArgs errorEventArgs)
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
            LogHelper.Debug($"---发送[{ServerType}] {message.ToMessageString()}");
        }

        _discoveryCenterClient.TrySend(span);
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
                InnerIp = "127.0.0.1",
                InnerPort = 22000,
                APMPort = 22001,
                DiscoveryCenterPort = 33300,
                DiscoveryCenterIp = "127.0.0.1",
            };
            if (PlatformRuntimeHelper.IsLinux)
            {
                Setting.DiscoveryCenterIp = "discovery";
                Setting.InnerIp = "gateway";
            }
        }

        base.Init();
    }
}


public sealed class ClientSession
{
    public ClientSession(long sessionId, AsyncTcpSession asyncTcpSession)
    {
        SessionId = sessionId;
        AsyncTcpSession = asyncTcpSession;
    }

    public long SessionId { get; }
    public AsyncTcpSession AsyncTcpSession { get; }
}