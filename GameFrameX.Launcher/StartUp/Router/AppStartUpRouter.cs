using GameFrameX.Apps.Common.Session;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.Proto.BuiltIn;

namespace GameFrameX.Launcher.StartUp.Router;

/// <summary>
/// 路由服务器.最后启动。
/// </summary>
[StartUpTag(ServerType.Router, int.MaxValue)]
internal class AppStartUpRouter : AppStartUpService
{
    /// <summary>
    /// 从发现中心请求的目标服务器类型
    /// </summary>
    protected override ServerType GetServerType
    {
        get { return ServerType.Gateway; }
    }

    public override async Task StartAsync()
    {
        try
        {
            StartServer();
            LogHelper.Info($"启动服务器 {ServerType} 端口: {Setting.InnerPort} 结束!");
            await base.StartAsync();
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

    protected override ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs disconnectEventArgs)
    {
        LogHelper.Info("有外部客户端网络断开连接成功！。断开信息：" + appSession.SessionID + "  " + disconnectEventArgs.Reason);
        SessionManager.Remove(appSession.SessionID);
        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info("有外部客户端网络连接成功！。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
        var netChannel = new DefaultNetWorkChannel(appSession, Setting, MessageEncoderHandler, null, appSession is WebSocketSession);
        var session = new Session(appSession.SessionID, netChannel);
        SessionManager.Add(session);

        return ValueTask.CompletedTask;
    }


    /// <summary>
    /// 处理收到的消息结果
    /// </summary>
    /// <param name="appSession"></param>
    /// <param name="message"></param>
    protected override async ValueTask PackageHandler(IAppSession appSession, IMessage message)
    {
        if (message is IOuterNetworkMessage outerMessage)
        {
            if (Setting.IsDebug && Setting.IsDebugReceive)
            {
                LogHelper.Debug(outerMessage.ToFormatMessageString());
            }

            if (outerMessage.Header.OperationType == MessageOperationType.HeartBeat)
            {
                var reqHeartBeat = (ReqHeartBeat)outerMessage.DeserializeMessageObject();
                var response = new NotifyHeartBeat
                {
                    UniqueId = reqHeartBeat.UniqueId,
                    Timestamp = TimeHelper.UnixTimeMilliseconds(),
                };
                SendToClient(appSession, response);
                await ValueTask.CompletedTask;
                return;
            }

            if (outerMessage.Header.OperationType is MessageOperationType.Game or MessageOperationType.None)
            {
                if (Setting.IsDebug && Setting.IsDebugReceive)
                {
                    LogHelper.Debug($"转发到[{ServerType.Gateway}] {outerMessage.ToFormatMessageString()}");
                }

                var reqConnectServer = new ReqConnectServer
                {
                    //ServerId = Setting.ServerId,
                    ServerType = ServerType.Gateway,
                };

                var respConnectServer = await DiscoveryCenterChannelHelper.Call<RespConnectServer>(reqConnectServer);
                // LogHelper.Info(respConnectServer);

                // var innerNetworkMessage = InnerNetworkMessage.Create(outerMessage, MessageOperationType.Game);
                // innerNetworkMessage.SetData(GlobalConst.SessionIdKey, appSession.SessionID);
                // SendToGatewayMessage(innerNetworkMessage);
            }
        }
        else if (message is IInnerNetworkMessage innerMessage)
        {
            if (Setting.IsDebug && Setting.IsDebugReceive)
            {
                // LogHelper.Debug($"转发到[{ServerType.Gateway}] [{innerMessage.ToReceiveMessageString(ServerType, ServerType.Client)}]");
            }
        }

        await ValueTask.CompletedTask;
    }

    private async void SendToClient(IAppSession appSession, MessageObject messageObject)
    {
        if (appSession.Connection.IsClosed)
        {
            return;
        }

        LogHelper.Debug(messageObject.ToFormatMessageString());
        var result = MessageEncoderHandler.Handler(messageObject);
        await appSession.SendAsync(result);
    }


    public override async Task StopAsync(string message = "")
    {
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
                InnerPort = 23110,
                WsPort = 23111,
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
}