using GameFrameX.Launcher.PipelineFilter;
using GameFrameX.Launcher.StartUp.Gateway;
using GameFrameX.Proto.BuiltIn;


/// <summary>
/// 网关服务器
/// </summary>
[StartUpTag(ServerType.Gateway)]
internal sealed partial class AppStartUpGateway : AppStartUpService
{
    private IServer tcpService;
    protected override int HeartBeatInterval { get; } = 10000;

    /// <summary>
    /// 和游戏逻辑服链接的客户端
    /// </summary>
    private List<ClientSession> _gameClientList = new List<ClientSession>();


    public override async Task EnterAsync()
    {
        try
        {
            LogHelper.Info($"启动服务器{Setting.ServerType} 开始! address: {Setting.InnerIp}  port: {Setting.InnerPort}");
            await StartServer();
            await base.EnterAsync();
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
        LogHelper.Info("有客户端网络断开连接成功！。断开信息：" + appSession.SessionID + "  " + disconnectEventArgs.Reason);
        GameClientSessionManager.RemoveSession(appSession.SessionID); //移除
        return ValueTask.CompletedTask;
    }

    private ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info("有客户端网络连接成功！。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
        // var gameSession = new GameSession(socketClient.IP, socketClient);
        var netChannel = new DefaultNetWorkChannel(appSession, messageEncoderHandler, RpcSession);
        GameClientSessionManager.SetSession(appSession.SessionID, netChannel); //移除
        return ValueTask.CompletedTask;
    }

    private async ValueTask PackageHandler(IAppSession session, IMessage message)
    {
        if (Setting.IsDebug && Setting.IsDebugReceive && message is BaseMessageObject baseMessageObject)
        {
            LogHelper.Debug($"---收到[{ServerType}] {baseMessageObject.ToMessageString()}");
        }

        if (message is ReqActorHeartBeat reqActorHeartBeat)
        {
            var respActorHeartBeat = new RespActorHeartBeat()
            {
                UniqueId = reqActorHeartBeat.UniqueId,
                Timestamp = TimeHelper.UnixTimeSeconds()
            };
            SendMessage(session, respActorHeartBeat);
            return;
        }

        if (message is MessageActorObject messageActorObject)
        {
            // 发送
            var response = new RespActorHeartBeat()
            {
                Timestamp = TimeHelper.UnixTimeSeconds()
            };
            SendMessage(session, response);
        }
        else if (message is MessageObject messageObject)
        {
            // 这里要拿到对应的逻辑服务器
        }
    }

    #endregion

    private async void SendMessage(IAppSession session, IMessage message)
    {
        if (session == null || session.Connection.IsClosed)
        {
            LogHelper.Error("目标链接已断开，取消发送");
            return;
        }

        var result = messageEncoderHandler.Handler(message);
        if (Setting.IsDebug && Setting.IsDebugSend && message is BaseMessageObject baseMessageObject)
        {
            LogHelper.Debug($"---发送[{ServerType}] {baseMessageObject.ToMessageString()}");
        }

        await session.SendAsync(result);
    }

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
        SendToDiscoveryCenterMessage(reqRegisterServer.UniqueId, reqRegisterServer);
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

    protected override void Init()
    {
        if (Setting == null)
        {
            Setting = new AppSetting
            {
                ServerId = 22000,
                ServerType = ServerType.Gateway,
                InnerIp = "127.0.0.1",
                InnerPort = 22001,
                OuterIp = "127.0.0.1",
                OuterPort = 22001,
                APMPort = 22090,
                DiscoveryCenterPort = 21001,
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

    private static MessageActorGatewayEncoderHandler messageEncoderHandler = new MessageActorGatewayEncoderHandler();

    private static MessageActorGatewayDecoderHandler messageDecoderHandler = new MessageActorGatewayDecoderHandler();

    protected override bool IsRequestConnectServer { get; } = false;

    public AppStartUpGateway() : base(messageEncoderHandler, messageDecoderHandler)
    {
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