using GameFrameX.Apps.Common.Session;
using GameFrameX.Config;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Message;
using GameFrameX.NetWork.Messages;
using GameFrameX.SuperSocket.Connection;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.WebSocket.Server;

namespace GameFrameX.Hotfix.StartUp;

/// <summary>
/// 业务服务器.最后启动。
/// </summary>
internal partial class AppStartUpHotfixGame
{
    public override async Task StartAsync()
    {
        // 启动网络服务
        // 设置压缩和解压缩
        await StartServerAsync<ClientMessageDecoderHandler, ClientMessageEncoderHandler>(new DefaultMessageCompressHandler(), new DefaultMessageDecompressHandler(), HotfixManager.GetListHttpHandler(), HotfixManager.GetHttpHandler);
        // 启动Http服务
        // await HttpServer.Start(Setting.HttpPort, Setting.HttpsPort, HotfixManager.GetListHttpHandler(), HotfixManager.GetHttpHandler, null, Setting.HttpUrl);
    }

    public async Task RunServer(bool reload = false)
    {
        // 不管是不是重启服务器，都要加载配置
        await ConfigComponent.Instance.LoadConfig();
        if (reload)
        {
            ActorManager.ClearAgent();
            return;
        }

        await StartAsync();
    }


    protected override ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs disconnectEventArgs)
    {
        LogHelper.Info("有外部客户端网络断开连接成功！。断开信息：" + appSession.SessionID + "  " + disconnectEventArgs.Reason);
        SessionManager.Remove(appSession.SessionID);
        return ValueTask.CompletedTask;
    }

    protected override async ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info("有外部客户端网络连接成功！。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
        var netChannel = new DefaultNetWorkChannel(appSession, Setting,  null, appSession is WebSocketSession);
        var count = SessionManager.Count();
        if (count > Setting.MaxClientCount)
        {
            // 达到最大在线人数限制
            await netChannel.WriteAsync(new NotifyServerFullyLoaded(), (int)OperationStatusCode.ServerFullyLoaded);
            netChannel.Close();
            return;
        }

        var session = new Session(appSession.SessionID, netChannel);
        SessionManager.Add(session);
    }

    /// <summary>
    /// 处理收到的消息结果
    /// </summary>
    /// <param name="appSession"></param>
    /// <param name="message"></param>
    protected override async ValueTask PackageHandler(IAppSession appSession, IMessage message)
    {
        if (message is OuterNetworkMessage outerNetworkMessage)
        {
            var netWorkChannel = SessionManager.GetChannel(appSession.SessionID);

            if (netWorkChannel.IsNull())
            {
                return;
            }

            if (outerNetworkMessage.Header.OperationType == MessageOperationType.HeartBeat)
            {
                if (Setting.IsDebug && Setting.IsDebugReceive && Setting.IsDebugReceiveHeartBeat)
                {
                    LogHelper.Debug($"---收到{outerNetworkMessage.ToFormatMessageString()}");
                }

                // 心跳消息回复
                ReplyHeartBeat(netWorkChannel, (MessageObject)outerNetworkMessage.DeserializeMessageObject());
                return;
            }

            if (Setting.IsDebug && Setting.IsDebugReceive)
            {
                LogHelper.Debug($"---收到{outerNetworkMessage.ToFormatMessageString()}");
            }

            var handler = HotfixManager.GetTcpHandler(outerNetworkMessage.Header.MessageId);
            if (handler == null)
            {
                LogHelper.Error($"找不到[{outerNetworkMessage.Header.MessageId}][{message.GetType()}]对应的handler");
                return;
            }

            // 执行消息分发处理
            await InvokeMessageHandler(handler, outerNetworkMessage.DeserializeMessageObject(), netWorkChannel);
        }
    }

    public override async Task StopAsync(string message = "")
    {
        await base.StopAsync(message);
        // 断开所有连接
        await SessionManager.RemoveAll();
        // 取消所有未执行定时器
        await QuartzTimer.Stop();
        // 保证actor之前的任务都执行完毕
        await ActorManager.AllFinish();
        // 存储所有数据
        await GlobalTimer.Stop();
        // 删除所有actor
        await ActorManager.RemoveAll();
    }
}