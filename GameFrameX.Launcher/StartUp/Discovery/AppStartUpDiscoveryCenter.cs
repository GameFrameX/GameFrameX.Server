using GameFrameX.Launcher.PipelineFilter;
using GameFrameX.Proto.BuiltIn;
using GameFrameX.ServerManager;

namespace GameFrameX.Launcher.StartUp.Discovery;

/// <summary>
/// 服务发现中心服务器
/// </summary>
[StartUpTag(ServerType.DiscoveryCenter, 0)]
internal sealed class AppStartUpDiscoveryCenter : AppStartUpBase
{
    private IServer _server;

    readonly MessageActorDiscoveryEncoderHandler messageEncoderHandler = new MessageActorDiscoveryEncoderHandler();


    public override async Task EnterAsync()
    {
        try
        {
            LogHelper.Info($"开始启动服务器{ServerType}");
            NamingServiceManager.Instance.OnServerAdd = OnServerAdd;
            NamingServiceManager.Instance.OnServerRemove = OnServerRemove;
            NamingServiceManager.Instance.AddSelf(Setting);

            LogHelper.Info($"启动服务器{ServerType} 开始!");

            StartServer();

            LogHelper.Info($"启动服务器 {ServerType} 端口: {Setting.InnerPort} 结束!");

            await AppExitToken;
        }
        catch (Exception e)
        {
            LogHelper.Info($"服务器{ServerType}执行异常，e:{e}");
            LogHelper.Fatal(e);
        }

        LogHelper.Info($"退出服务器{ServerType}开始");
        await Stop();
        LogHelper.Info($"退出服务器{ServerType}成功");
    }

    private void OnServerRemove(ServerInfo serverInfo)
    {
        var serverList = NamingServiceManager.Instance.GetAllNodes().Where(m => m.ServerId != 0 && m.ServerId != serverInfo.ServerId).ToList();

        RespServerOfflineServer respServerOnlineServer = new RespServerOfflineServer()
        {
            ServerType = serverInfo.Type,
            ServerName = serverInfo.ServerName,
            ServerID = serverInfo.ServerId
        };
        foreach (var info in serverList)
        {
            if (serverInfo.SessionId == info.SessionId)
            {
                continue;
            }

            var appSession = (IAppSession)info.Session;
            SendMessage(appSession, respServerOnlineServer);
        }
    }

    private void OnServerAdd(ServerInfo serverInfo)
    {
        var serverList = NamingServiceManager.Instance.GetOuterNodes().Where(m => m.ServerId != serverInfo.ServerId).ToList();

        RespServerOnlineServer respServerOnlineServer = new RespServerOnlineServer()
        {
            ServerType = serverInfo.Type,
            ServerName = serverInfo.ServerName,
            ServerID = serverInfo.ServerId
        };
        foreach (var info in serverList)
        {
            var appSession = (IAppSession)info.Session;
            SendMessage(appSession, respServerOnlineServer);
        }
    }

    /// <summary>
    /// 发送消息给注册的服务
    /// </summary>
    /// <param name="session"></param>
    /// <param name="message"></param>
    private async void SendMessage(IAppSession session, IMessage message)
    {
        Guard.NotNull(message, nameof(message));
        if (session == null)
        {
            return;
        }

        var data = messageEncoderHandler.Handler(message);
        if (Setting.IsDebug && Setting.IsDebugReceive)
        {
            var serverInfo = NamingServiceManager.Instance.GetNodeBySessionId(session.SessionID);
            if (serverInfo != null)
            {
                LogHelper.Info(message.ToSendMessageString(ServerType, serverInfo.Type));
            }
        }

        await session.SendAsync(data);
    }

    private async void StartServer()
    {
        _server = SuperSocketHostBuilder.Create<IMessage, MessageObjectPipelineFilter>()
            .ConfigureSuperSocket(ConfigureSuperSocket)
            .UseClearIdleSession()
            .UsePackageDecoder<MessageActorDiscoveryDecoderHandler>()
            .UsePackageEncoder<MessageActorDiscoveryEncoderHandler>()
            .UseSessionHandler(OnConnected, OnDisconnected)
            .UsePackageHandler(PackageHandler)
            .UseInProcSessionContainer()
            .BuildAsServer();

        await _server.StartAsync();
    }


    private ValueTask PackageHandler(IAppSession session, IMessage messageObject)
    {
        if (Setting.IsDebug && Setting.IsDebugReceive && messageObject is BaseMessageObject baseMessageObject)
        {
            var serverInfo = NamingServiceManager.Instance.GetNodeBySessionId(session.SessionID);
            if (serverInfo != null)
            {
                LogHelper.Debug($"---收到[{serverInfo.Type} To {ServerType}]  {baseMessageObject.ToMessageString()}");
            }
            else
            {
                LogHelper.Debug($"---收到[{ServerType}]  {baseMessageObject.ToMessageString()}");
            }
        }

        if (messageObject is MessageObject message)
        {
            if (message is ReqRegisterServer reqRegisterServer)
            {
                // 注册服务
                ServerInfo serverInfo = new ServerInfo(reqRegisterServer.ServerType, session, session.SessionID, reqRegisterServer.ServerName, reqRegisterServer.ServerID, reqRegisterServer.InnerIP, reqRegisterServer.InnerPort, reqRegisterServer.OuterIP, reqRegisterServer.OuterPort);
                NamingServiceManager.Instance.Add(serverInfo);
                LogHelper.Info($"注册服务成功：{reqRegisterServer.ServerType}  {reqRegisterServer.ServerName}  {reqRegisterServer}");
                return ValueTask.CompletedTask;
            }
            else if (message is ReqConnectServer reqConnectServer)
            {
                var serverList = NamingServiceManager.Instance.GetNodesByType(reqConnectServer.ServerType);
                if (reqConnectServer.ServerID > 0)
                {
                    serverList = serverList.Where(m => m.ServerId == reqConnectServer.ServerID).ToList();
                }

                if (serverList.Count > 0)
                {
                    var serverInfo = serverList.Random();

                    RespConnectServer respConnectServer = new RespConnectServer
                    {
                        UniqueId = reqConnectServer.UniqueId,
                        ServerType = serverInfo.Type,
                        ServerName = serverInfo.ServerName,
                        ServerID = serverInfo.ServerId,
                        TargetIP = serverInfo.OuterIp,
                        TargetPort = serverInfo.OuterPort
                    };
                    SendMessage(session, respConnectServer);
                }
            }
        }
        else if (messageObject is MessageActorObject messageActorObject)
        {
            if (messageActorObject is ReqActorHeartBeat reqActorHeartBeat)
            {
                // 心跳相应
                var response = new RespActorHeartBeat()
                {
                    UniqueId = reqActorHeartBeat.UniqueId,
                    Timestamp = TimeHelper.UnixTimeSeconds()
                };
                SendMessage(session, response);
                return ValueTask.CompletedTask;
            }
        }

        return ValueTask.CompletedTask;
    }


    private ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info("有外部服务连接到中心服务器成功" + "。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
        return ValueTask.CompletedTask;
    }

    private ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs args)
    {
        LogHelper.Info("有外部服务从中心服务器断开。链接信息：断开原因:" + args.Reason);
        NamingServiceManager.Instance.TrySessionRemove(appSession.SessionID);
        return ValueTask.CompletedTask;
    }

    public override async Task Stop(string message = "")
    {
        LogHelper.Info($"{ServerType} Server stopping...");
        await _server.StopAsync();
        LogHelper.Info($"{ServerType} Server Done!");
    }

    protected override void ConfigureSuperSocket(ServerOptions options)
    {
        base.ConfigureSuperSocket(options);
        options.ClearIdleSessionInterval = 30;
    }

    protected override void Init()
    {
        if (Setting == null)
        {
            Setting = new AppSetting
            {
                ServerId = 21000,
                ServerType = ServerType.DiscoveryCenter,
                InnerPort = 21001,
                APMPort = 21090
            };
        }

        base.Init();
    }
}