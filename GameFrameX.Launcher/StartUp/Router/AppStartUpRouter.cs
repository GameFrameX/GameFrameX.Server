using GameFrameX.Launcher.PipelineFilter;
using GameFrameX.Proto.BuiltIn;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace GameFrameX.Launcher.StartUp.Router;

/// <summary>
/// 路由服务器.最后启动。
/// </summary>
[StartUpTag(ServerType.Router, int.MaxValue)]
internal partial class AppStartUpRouter : AppStartUpService
{
    /// <summary>
    /// 服务器。对外提供服务
    /// </summary>
    private IServer _tcpService;

    /// <summary>
    /// WS服务器
    /// </summary>
    private IHost _webSocketServer;


    public override async Task EnterAsync()
    {
        try
        {
            await StartServer();
            LogHelper.Info($"启动服务器 {ServerType} 端口: {Setting.InnerPort} 结束!");
            await base.EnterAsync();
            StartGatewayClient();
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

    protected override void ConnectServerHandler()
    {
        ConnectToGateWay();
    }

    protected override void DisconnectServerHandler()
    {
        DisconnectToGateWay();
    }

    protected override void DiscoveryCenterDataReceived(IMessage message)
    {
        LogHelper.Debug(message.ToMessageString());
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
        var netChannel = new DefaultNetWorkChannel(appSession, messageEncoderHandler, RpcSession, appSession is WebSocketSession);
        GameClientSessionManager.SetSession(appSession.SessionID, netChannel); //移除

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
        var messageObject = messageDecoderHandler.Handler(buffer);
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
            if (messageObject is ReqHeartBeat reqHeartBeat)
            {
                if (Setting.IsDebug && Setting.IsDebugReceive)
                {
                    LogHelper.Debug(messageObject.ToReceiveMessageString(ServerType.Client, ServerType));
                }

                ReplyHeartBeat(appSession, reqHeartBeat);
                return ValueTask.CompletedTask;
            }

            if (Setting.IsDebug && Setting.IsDebugReceive)
            {
                LogHelper.Debug($"转发到[{ServerType.Gateway}] [{messageObject.ToReceiveMessageString(ServerType, ServerType.Client)}]");
            }

            SendToGatewayMessage(message);
        }

        return ValueTask.CompletedTask;
    }

    private static async void ReplyHeartBeat(IAppSession appSession, ReqHeartBeat reqHeartBeat)
    {
        var response = new RespHeartBeat()
        {
            UniqueId = reqHeartBeat.UniqueId,
            Timestamp = TimeHelper.UnixTimeSeconds()
        };
        var result = messageEncoderHandler.Handler(response);
        await appSession.SendAsync(result);
    }

    private void ConfigureWebServer(HostBuilderContext context, IConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection(new Dictionary<string, string>()
            { { "serverOptions:name", "TestServer" }, { "serverOptions:listeners:0:ip", "Any" }, { "serverOptions:listeners:0:port", Setting.WsPort.ToString() } });
    }

    public override async Task Stop(string message = "")
    {
        DisconnectToGateWay();
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

    /// <summary>
    /// 从发现中心请求的目标服务器类型
    /// </summary>
    protected override ServerType GetServerType => ServerType.Gateway;

    private static readonly MessageRouterEncoderHandler messageEncoderHandler = new MessageRouterEncoderHandler();
    private static readonly MessageRouterDecoderHandler messageDecoderHandler = new MessageRouterDecoderHandler();

    public AppStartUpRouter() : base(messageEncoderHandler, messageDecoderHandler)
    {
    }
}