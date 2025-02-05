using GameFrameX.Apps.Common.Session;
using GameFrameX.Config;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Message;
using GameFrameX.NetWork.Messages;
using GameFrameX.SuperSocket.Connection;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.WebSocket.Server;
using GameFrameX.Utility.Log;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Hotfix.StartUp;

/// <summary>
/// 路由服务器.最后启动。
/// </summary>
internal partial class AppStartUpHotfixGame
{
    public override async Task StartAsync()
    {
        // 启动网络服务
        await StartServerAsync<ClientMessageDecoderHandler, ClientMessageEncoderHandler>(new DefaultMessageCompressHandler(), new DefaultMessageDecompressHandler());
        // 启动Http服务
        await HttpServer.Start(Setting.HttpPort, Setting.HttpsPort, HotfixManager.GetListHttpHandler(), HotfixManager.GetHttpHandler);
    }

    public async void RunServer(bool reload = false)
    {
        // 不管是不是重启服务器，都要加载配置
        ConfigComponent.Instance.LoadConfig();
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
        if (message is OuterNetworkMessage outerNetworkMessage)
        {
            if (Setting.IsDebug && Setting.IsDebugReceive)
            {
                LogHelper.Debug($"---收到{outerNetworkMessage.ToFormatMessageString()}");
            }

            var netWorkChannel = SessionManager.GetChannel(appSession.SessionID);
            if (outerNetworkMessage.Header.OperationType == MessageOperationType.HeartBeat)
            {
                // LogHelper.Info("收到心跳请求:" + req.Timestamp);
                ReplyHeartBeat(netWorkChannel, (MessageObject)outerNetworkMessage.DeserializeMessageObject());
                // 心跳消息
                await ValueTask.CompletedTask;
                return;
            }

            var handler = HotfixManager.GetTcpHandler(outerNetworkMessage.Header.MessageId);
            if (handler == null)
            {
                LogHelper.Error($"找不到[{outerNetworkMessage.Header.MessageId}][{message.GetType()}]对应的handler");
                await ValueTask.CompletedTask;
                return;
            }

            async void InvokeAction()
            {
                await handler.Init((MessageObject)outerNetworkMessage.DeserializeMessageObject(), netWorkChannel);
                await handler.InnerAction();
            }

            await Task.Run(InvokeAction);
        }

        await ValueTask.CompletedTask;
    }

    public override async Task StopAsync(string message = "")
    {
        await base.StopAsync(message);

        await HttpServer.Stop();
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