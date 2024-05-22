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
    private IServer server;

    readonly MessageActorDiscoveryEncoderHandler messageEncoderHandler = new MessageActorDiscoveryEncoderHandler();


    public override async Task EnterAsync()
    {
        try
        {
            LogHelper.Info($"开始启动服务器{ServerType}");

            NamingServiceManager.Instance.AddSelf(Setting);

            LogHelper.Info($"启动服务器{ServerType} 开始!");

            StartServer();

            LogHelper.Info($"启动服务器 {ServerType} 端口: {Setting.InnerPort} 结束!");

            await AppExitToken;
        }
        catch (Exception e)
        {
            LogHelper.Info($"服务器执行异常，e:{e}");
            LogHelper.Fatal(e);
        }

        LogHelper.Info($"退出服务器开始");
        await Stop();
        LogHelper.Info($"退出服务器成功");
    }

    private async void StartServer()
    {
        server = SuperSocketHostBuilder.Create<IMessage, MessageObjectPipelineFilter>()
            .ConfigureSuperSocket(ConfigureSuperSocket)
            .UseClearIdleSession()
            .UsePackageDecoder<MessageActorDiscoveryDecoderHandler>()
            .UsePackageEncoder<MessageActorDiscoveryEncoderHandler>()
            .UseSessionHandler(OnConnected, OnDisconnected)
            .UsePackageHandler(PackageHandler)
            .UseInProcSessionContainer()
            .BuildAsServer();

        await server.StartAsync();
    }


    private async ValueTask PackageHandler(IAppSession session, IMessage messageObject)
    {
        if (messageObject is MessageObject message)
        {
            var messageId = message.MessageId;
            if (Setting.IsDebug && Setting.IsDebugReceive)
            {
                LogHelper.Debug($"---收到消息ID:[{messageId}] ==>消息类型:{message.GetType()} 消息内容:{messageObject}");
            }

            if (message is ReqRegisterServer reqRegisterServer)
            {
                // 注册服务
                ServerInfo serverInfo = new ServerInfo(reqRegisterServer.ServerType, session.SessionID, reqRegisterServer.ServerName, reqRegisterServer.ServerID, reqRegisterServer.InnerIP, reqRegisterServer.InnerPort, reqRegisterServer.OuterIP, reqRegisterServer.OuterPort);
                NamingServiceManager.Instance.Add(serverInfo);
                LogHelper.Info($"注册服务成功：{reqRegisterServer.ServerType}  {reqRegisterServer.ServerName}  {reqRegisterServer}");
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
                await session.SendAsync(messageEncoderHandler, response);
            }
        }
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
        await server.StopAsync();
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
                ServerId = 3300,
                ServerType = ServerType.DiscoveryCenter,
                InnerPort = 33300
            };
        }

        base.Init();
    }
}