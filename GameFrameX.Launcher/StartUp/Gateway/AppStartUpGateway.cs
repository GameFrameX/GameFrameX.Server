using GameFrameX.Launcher.PipelineFilter;
using GameFrameX.Launcher.StartUp.Gateway;
using GameFrameX.Proto.BuiltIn;
using GameFrameX.ServerManager;


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
            // _namingServiceManager.OnServerAdd = OnServerAdd;
            // _namingServiceManager.OnServerRemove = OnServerRemove;
            _namingServiceManager.AddSelf(Setting);
            await base.EnterAsync();
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
        if (_namingServiceManager.TrySessionRemove(appSession.SessionID))
        {
            LogHelper.Info("有游戏业务客户端网络断开连接成功！。断开信息：" + appSession.SessionID + "  " + disconnectEventArgs.Reason);
        }

        return ValueTask.CompletedTask;
    }

    private ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info("有客户端网络连接成功！。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
        var netChannel = new DefaultNetWorkChannel(appSession, messageEncoderHandler, RpcSession);
        GameClientSessionManager.SetSession(appSession.SessionID, netChannel); //移除
        return ValueTask.CompletedTask;
    }

    private ValueTask PackageHandler(IAppSession session, IMessage message)
    {
        if (message is MessageObject messageObject)
        {
            if (Setting.IsDebug && Setting.IsDebugReceive && message is not (IReqHeartBeatMessage or IRespHeartBeatMessage))
            {
                LogHelper.Debug($"---收到[{ServerType}] {messageObject.ToMessageString()}");
            }

            if (message is ReqActorHeartBeat reqActorHeartBeat)
            {
                var respActorHeartBeat = new RespActorHeartBeat()
                {
                    UniqueId = reqActorHeartBeat.UniqueId,
                    Timestamp = TimeHelper.UnixTimeSeconds()
                };
                SendMessage(session, respActorHeartBeat);
                return ValueTask.CompletedTask;
            }

            if (message is RespActorHeartBeat discoveryCenterRespActorHeartBeat)
            {
                // 发现中心的心跳返回
                return ValueTask.CompletedTask;
            }

            if (message is ReqRegisterGameServer reqRegisterGameServer)
            {
                GameServiceInfo gameServiceInfo = new GameServiceInfo(reqRegisterGameServer.ServerType,
                    session,
                    session.SessionID,
                    reqRegisterGameServer.ServerName,
                    reqRegisterGameServer.ServerID,
                    reqRegisterGameServer.MinModuleMessageID,
                    reqRegisterGameServer.MaxModuleMessageID
                );

                _namingServiceManager.Add(gameServiceInfo);
                return ValueTask.CompletedTask;
            }

            var mainId = MessageManager.GetMainId(messageObject.MessageId);
            var serviceInfos = _namingServiceManager.GetNodesByType(ServerType.Game);
            foreach (var serviceInfo in serviceInfos)
            {
                if (serviceInfo is GameServiceInfo gameServiceInfo)
                {
                    if (mainId >= gameServiceInfo.MinModuleMessageId && mainId <= gameServiceInfo.MaxModuleMessageId)
                    {
                        SendMessage((IAppSession)gameServiceInfo.Session, messageObject);
                        break;
                    }
                }
            }
        }

        return ValueTask.CompletedTask;
    }

    #endregion

    private async void SendMessage(IAppSession session, IMessage message)
    {
        if (session == null || session.Connection.IsClosed)
        {
            LogHelper.Error("目标链接已断开，取消发送");
            return;
        }

        if (message is MessageObject messageObject)
        {
            var result = messageEncoderHandler.Handler(message);
            if (Setting.IsDebug && Setting.IsDebugSend && message is not (IReqHeartBeatMessage or IRespHeartBeatMessage))
            {
                LogHelper.Debug($"---发送[{ServerType}] {messageObject.ToMessageString()}");
            }

            await session.SendAsync(result);
        }
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

    private NamingServiceManager _namingServiceManager;

    public AppStartUpGateway() : base(messageEncoderHandler, messageDecoderHandler)
    {
        _namingServiceManager = new NamingServiceManager();
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