using GameFrameX.Apps.Common.Session;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Message;
using GameFrameX.Proto.BuiltIn;
using GameFrameX.SuperSocket.Primitives;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace GameFrameX.Launcher.StartUp.Router;

/// <summary>
/// 路由服务器.最后启动。
/// </summary>
// [StartUpTag(ServerType.Router, int.MaxValue)]
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


    public override async Task StartAsync()
    {
        try
        {
            await StartServer();
            LogHelper.Info($"启动服务器 {ServerType} 端口: {Setting.InnerPort} 结束!");
            await base.StartAsync();
            StartGatewayClient();
            await AppExitToken;
            LogHelper.Info("全部断开...");
            await StopAsync();
            LogHelper.Info("Done!");
        }
        catch (Exception e)
        {
            await StopAsync(e.Message);
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

    protected override void DiscoveryCenterDataReceived(INetworkMessage message)
    {
        if (message is MessageObject messageObject)
        {
            if (Setting.IsDebug && Setting.IsDebugReceive && !MessageProtoHelper.IsHeartbeat(message.GetType()))
            {
                LogHelper.Info(messageObject.ToReceiveMessageString(ServerType.DiscoveryCenter, ServerType));
            }
        }
    }


    private async Task StartServer()
    {
        _webSocketServer = WebSocketHostBuilder.Create()
                                               .UseWebSocketMessageHandler(WebSocketMessageHandler)
                                               .UseSessionHandler(OnConnected, OnDisconnected)
                                               .ConfigureAppConfiguration((Action<HostBuilderContext, IConfigurationBuilder>)(ConfigureWebServer)).Build();
        await _webSocketServer.StartAsync();
        _tcpService = SuperSocketHostBuilder.Create<INetworkMessage, MessageObjectPipelineFilter>()
                                            .ConfigureSuperSocket(ConfigureSuperSocket)
                                            .UseClearIdleSession()
                                            .UsePackageDecoder<MessageRouterDecoderHandler>()
                                            .UseSessionHandler(OnConnected, OnDisconnected)
                                            .UsePackageHandler(MessagePackageHandler, ClientErrorHandler)
                                            .UseInProcSessionContainer()
                                            .BuildAsServer();

        await _tcpService.StartAsync();
    }

    private ValueTask<bool> ClientErrorHandler(IAppSession appSession, PackageHandlingException<INetworkMessage> eventArgs)
    {
        LogHelper.Error(eventArgs.ToString());
        return ValueTask.FromResult(true);
    }

    private ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs disconnectEventArgs)
    {
        LogHelper.Info("有外部客户端网络断开连接成功！。断开信息：" + appSession.SessionID + "  " + disconnectEventArgs.Reason);
        SessionManager.Remove(appSession.SessionID);
        return ValueTask.CompletedTask;
    }

    private ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info("有外部客户端网络连接成功！。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
        var netChannel = new DefaultNetWorkChannel(appSession, Setting, messageEncoderHandler, RpcSession, appSession is WebSocketSession);
        var session = new Session(appSession.SessionID, netChannel);
        SessionManager.Add(session);

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
    /// <param name="message"></param>
    private ValueTask MessagePackageHandler(IAppSession appSession, INetworkMessage message)
    {
        if (message is IOuterMessage outerMessage)
        {
            if (Setting.IsDebug && Setting.IsDebugReceive)
            {
                LogHelper.Debug(outerMessage.ToReceiveMessageString(ServerType.Client, ServerType));
            }

            if (outerMessage.OperationType == MessageOperationType.HeartBeat)
            {
                var reqHeartBeat = (ReqHeartBeat)outerMessage.DeserializeMessageObject();
                var response = new NotifyHeartBeat()
                {
                    UniqueId = reqHeartBeat.UniqueId,
                    Timestamp = TimeHelper.UnixTimeSeconds()
                };
                SendToClient(appSession, response);
                return ValueTask.CompletedTask;
            }

            if (outerMessage.OperationType == MessageOperationType.Game)
            {
                if (Setting.IsDebug && Setting.IsDebugReceive)
                {
                    LogHelper.Debug($"转发到[{ServerType.Gateway}] {outerMessage.ToReceiveMessageString(ServerType, ServerType.Client)}");
                }

                InnerMessage innerMessage = InnerMessage.Create(outerMessage, MessageOperationType.Game);
                innerMessage.SetData(GlobalConst.SessionIdKey, appSession.SessionID);
                SendToGatewayMessage(innerMessage);
            }
        }
        else if (message is IInnerMessage innerMessage)
        {
            if (Setting.IsDebug && Setting.IsDebugReceive)
            {
                LogHelper.Debug($"转发到[{ServerType.Gateway}] [{innerMessage.ToReceiveMessageString(ServerType, ServerType.Client)}]");
            }
        }

        return ValueTask.CompletedTask;
    }

    private static async void SendToClient(IAppSession appSession, MessageObject messageObject)
    {
        if (appSession.Connection.IsClosed)
        {
            return;
        }

        LogHelper.Debug(messageObject.ToSendMessageString(ServerType.Router, ServerType.Client));
        var result = messageEncoderHandler.Handler(messageObject);
        await appSession.SendAsync(result);
    }


    private void ConfigureWebServer(HostBuilderContext context, IConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection(new Dictionary<string, string>()
                                          { { "serverOptions:name", "TestServer" }, { "serverOptions:listeners:0:ip", "Any" }, { "serverOptions:listeners:0:port", Setting.WsPort.ToString() } });
    }

    public override async Task StopAsync(string message = "")
    {
        DisconnectToGateWay();
        await _webSocketServer.StopAsync();
        await _tcpService.StopAsync();
        await base.StopAsync(message);
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