using GameFrameX.Apps.Common.Session;
using GameFrameX.Config;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Message;
using GameFrameX.NetWork.Messages;
using GameFrameX.SuperSocket.Connection;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.Server.Host;
using GameFrameX.SuperSocket.WebSocket;
using GameFrameX.SuperSocket.WebSocket.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using CloseReason = GameFrameX.SuperSocket.Connection.CloseReason;

namespace GameFrameX.Hotfix.StartUp;

/// <summary>
/// 路由服务器.最后启动。
/// </summary>
internal partial class AppStartUpHotfixGame
{
    public override async Task StartAsync()
    {
        // 启动网络服务
        await StartServer();
        StartWebSocketServer();
        // 设置压缩和解压缩
        MessageEncoderHandler.SetCompressionHandler(new DefaultMessageCompressHandler());
        MessageDecoderHandler.SetDecompressionHandler(new DefaultMessageDecompressHandler());
        // 启动Http服务
        await HttpServer.Start(Setting.HttpPort, Setting.HttpsPort, HotfixManager.GetHttpHandler);
        await base.StartAsync();
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

    /// <summary>
    /// WS服务器
    /// </summary>
    private IHost _webSocketServer;


    /// <summary>
    /// 启动WebSocket
    /// </summary>
    private async void StartWebSocketServer()
    {
        if (Setting.WsPort > 0)
        {
            LogHelper.Info("启动 WebSocket 服务器开始...");
            _webSocketServer = WebSocketHostBuilder.Create()
                                                   .UseWebSocketMessageHandler(WebSocketMessageHandler)
                                                   .UseSessionHandler(OnConnected, OnDisconnected).ConfigureAppConfiguration((Action<HostBuilderContext, IConfigurationBuilder>)(Action<HostBuilderContext, IConfigurationBuilder>)(ConfigureWebServer)).Build();
            await _webSocketServer.StartAsync();
            LogHelper.Info("启动 WebSocket 服务器完成...");
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
    /// 处理收到的WS消息
    /// </summary>
    /// <param name="session"></param>
    /// <param name="messagePackage"></param>
    private async ValueTask WebSocketMessageHandler(WebSocketSession session, WebSocketPackage messagePackage)
    {
        if (messagePackage.OpCode != OpCode.Binary)
        {
            await session.CloseAsync(CloseReason.ProtocolError);
            return;
        }

        var readOnlySequence = messagePackage.Data;
        var message = MessageDecoderHandler.Handler(ref readOnlySequence);
        await PackageHandler(session, message);
    }

    /// <summary>
    /// 处理收到的消息结果
    /// </summary>
    /// <param name="appSession"></param>
    /// <param name="message"></param>
    protected override ValueTask PackageHandler(IAppSession appSession, IMessage message)
    {
        if (message is MessageObject messageObject)
        {
            if (Setting.IsDebug && Setting.IsDebugReceive)
            {
                LogHelper.Debug($"---收到{message.ToFormatMessageString()}");
            }

            var netWorkChannel = SessionManager.GetChannel(appSession.SessionID);
            if (messageObject.OperationType == MessageOperationType.HeartBeat)
            {
                // LogHelper.Info("收到心跳请求:" + req.Timestamp);
                ReplyHeartBeat(netWorkChannel, messageObject);
                // 心跳消息
                return ValueTask.CompletedTask;
            }

            var handler = HotfixManager.GetTcpHandler(messageObject.MessageId);
            if (handler == null)
            {
                LogHelper.Error($"找不到[{messageObject.MessageId}][{message.GetType()}]对应的handler");
                return ValueTask.CompletedTask;
            }

            async void InvokeAction()
            {
                await handler.Init(messageObject, netWorkChannel);
                await handler.InnerAction();
            }

            Task.Run(InvokeAction);
        }

        return ValueTask.CompletedTask;
    }

    private void ConfigureWebServer(HostBuilderContext context, IConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection(new Dictionary<string, string>()
                                          { { "serverOptions:name", "GameServer" }, { "serverOptions:listeners:0:ip", "Any" }, { "serverOptions:listeners:0:port", Setting.WsPort.ToString() } });
    }

    public override async Task StopAsync(string message = "")
    {
        // 关闭网络服务
        if (_webSocketServer != null)
        {
            await _webSocketServer.StopAsync();
        }

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