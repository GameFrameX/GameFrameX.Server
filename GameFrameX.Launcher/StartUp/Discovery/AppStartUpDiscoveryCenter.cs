using GameFrameX.NetWork.Message;
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
    NamingServiceManager _namingServiceManager = new NamingServiceManager();

    public override async Task EnterAsync()
    {
        try
        {
            LogHelper.Info($"开始启动服务器{ServerType}");
            _namingServiceManager.OnServerAdd = OnServerAdd;
            _namingServiceManager.OnServerRemove = OnServerRemove;
            _namingServiceManager.AddSelf(Setting);

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

    private void OnServerRemove(IServiceInfo serverInfo)
    {
        var serverList = _namingServiceManager.GetAllNodes().Where(m => m.ServerId != 0 && m.ServerId != serverInfo.ServerId).ToList();

        RespServerOfflineServer respServerOnlineServer = new RespServerOfflineServer()
        {
            ServerType = serverInfo.Type,
            ServerName = serverInfo.ServerName,
            ServerID = serverInfo.ServerId
        };
        foreach (var serverInfo1 in serverList)
        {
            var info = (ServiceInfo)serverInfo1;
            if (serverInfo.SessionId == info.SessionId)
            {
                continue;
            }

            var appSession = (IAppSession)info.Session;
            SendMessage(appSession, respServerOnlineServer);
        }
    }

    private void OnServerAdd(IServiceInfo serverInfo)
    {
        var serverList = _namingServiceManager.GetOuterNodes().Where(m => m.ServerId != serverInfo.ServerId).ToList();

        RespServerOnlineServer respServerOnlineServer = new RespServerOnlineServer()
        {
            ServerType = serverInfo.Type,
            ServerName = serverInfo.ServerName,
            ServerID = serverInfo.ServerId
        };
        foreach (var serverInfo1 in serverList)
        {
            var info = (ServiceInfo)serverInfo1;
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
        message.CheckNotNull(nameof(message));
        if (session == null || session.Connection.IsClosed)
        {
            return;
        }

        InnerMessage innerMessage = InnerMessage.Create(message, MessageProtoHelper.IsHeartbeat(message.GetType()) ? MessageOperationType.HeartBeat : MessageOperationType.Game);
        var data = messageEncoderHandler.InnerHandler(innerMessage);
        if (Setting.IsDebug && Setting.IsDebugReceive)
        {
            var serverInfo = _namingServiceManager.GetNodeBySessionId(session.SessionID);
            if (serverInfo != null)
            {
                LogHelper.Info(innerMessage.ToSendMessageString(ServerType, serverInfo.Type));
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


    private ValueTask PackageHandler(IAppSession session, IMessage message)
    {
        if (message is IInnerMessage messageObject)
        {
            if (Setting.IsDebug && Setting.IsDebugReceive)
            {
                var serverInfo = _namingServiceManager.GetNodeBySessionId(session.SessionID);
                if (serverInfo != null)
                {
                    LogHelper.Debug($"---收到[{serverInfo.Type} To {ServerType}]  {messageObject.ToMessageString()}");
                }
                else
                {
                    LogHelper.Debug($"---收到[{ServerType}]  {messageObject.ToMessageString()}");
                }
            }

            if (messageObject.OperationType == MessageOperationType.HeartBeat)
            {
                // 心跳相应
                var reqHeartBeat = messageObject.DeserializeMessageObject();
                var response = new NotifyHeartBeat()
                {
                    UniqueId = reqHeartBeat.UniqueId,
                    Timestamp = TimeHelper.UnixTimeSeconds()
                };
                SendMessage(session, response);
                return ValueTask.CompletedTask;
            }

            if (messageObject.OperationType == MessageOperationType.Game)
            {
                if (messageObject.MessageType == typeof(ReqRegisterServer))
                {
                    ReqRegisterServer reqRegisterServer = (ReqRegisterServer)messageObject.DeserializeMessageObject();
                    // 注册服务
                    ServiceInfo serviceInfo = new ServiceInfo(reqRegisterServer.ServerType, session, session.SessionID, reqRegisterServer.ServerName, reqRegisterServer.ServerID, reqRegisterServer.InnerIP, reqRegisterServer.InnerPort, reqRegisterServer.OuterIP, reqRegisterServer.OuterPort);
                    _namingServiceManager.Add(serviceInfo);
                    LogHelper.Info($"注册服务成功：{reqRegisterServer.ServerType}  {reqRegisterServer.ServerName}  {reqRegisterServer}");
                    return ValueTask.CompletedTask;
                }

                if (messageObject.MessageType == typeof(ReqConnectServer))
                {
                    ReqConnectServer reqConnectServer = (ReqConnectServer)messageObject.DeserializeMessageObject();
                    var serverList = _namingServiceManager.GetNodesByType(reqConnectServer.ServerType);
                    if (reqConnectServer.ServerID > 0)
                    {
                        serverList = serverList.Where(m => m.ServerId == reqConnectServer.ServerID).ToList();
                    }

                    if (serverList.Count > 0)
                    {
                        var serverInfo = (ServiceInfo)serverList.Random();

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
        _namingServiceManager.TrySessionRemove(appSession.SessionID);
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