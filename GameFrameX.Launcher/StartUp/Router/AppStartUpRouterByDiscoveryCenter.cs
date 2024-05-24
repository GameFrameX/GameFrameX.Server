using GameFrameX.Launcher.PipelineFilter;
using GameFrameX.Proto.BuiltIn;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace GameFrameX.Launcher.StartUp.Router;

/// <summary>
/// 路由服务器.最后启动。
/// </summary>
internal partial class AppStartUpRouter
{
    /// <summary>
    /// 链接到网关的客户端
    /// </summary>
    AsyncTcpSession _discoveryCenterClient;

    private void SendToDiscoveryCenterMessage(long messageUniqueId, IMessage message)
    {
        var span = messageEncoderHandler.RpcHandler(messageUniqueId, message);
        if (Setting.IsDebug && Setting.IsDebugSend)
        {
            LogHelper.Debug($"---发送[{ServerType}],[{ServerType.DiscoveryCenter}] {message.ToMessageString()}");
        }

        _discoveryCenterClient.TrySend(span);
        // ArrayPool<byte>.Shared.Return(span);
    }

    /// <summary>
    /// 重连定时器
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected override void ReconnectionTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        // 重连到发现中心服务器
        ConnectToDiscoveryCenter();
    }

    private void ConnectToDiscoveryCenter()
    {
        var endPoint = new IPEndPoint(IPAddress.Parse(Setting.DiscoveryCenterIp), Setting.DiscoveryCenterPort);
        _discoveryCenterClient.Connect(endPoint);
    }

    private void StartDiscoveryCenterClient()
    {
        _discoveryCenterClient = new AsyncTcpSession();
        _discoveryCenterClient.Closed += DiscoveryCenterClientOnClosed;
        _discoveryCenterClient.DataReceived += DiscoveryCenterClientOnDataReceived;
        _discoveryCenterClient.Connected += DiscoveryCenterClientOnConnected;
        _discoveryCenterClient.Error += DiscoveryCenterClientOnError;

        LogHelper.Info("开始链接到发现中心服务器 ...");
        ConnectToDiscoveryCenter();
    }


    private void DiscoveryCenterClientOnError(object sender, ErrorEventArgs e)
    {
        LogHelper.Info("和发现中心服务器链接链接发生错误!" + e);
        DiscoveryCenterClientOnClosed(_discoveryCenterClient, e);
    }

    private void DiscoveryCenterClientOnConnected(object sender, EventArgs e)
    {
        LogHelper.Info("和发现中心服务器链接链接成功!");
        // 和网关服务器链接成功，关闭重连
        ReconnectionTimer.Stop();
        // 开启和网关服务器的心跳
        HeartBeatTimer.Start();

        if (_respConnectServer == null)
        {
            ReqConnectServer reqConnectServer = new ReqConnectServer
            {
                ServerType = ServerType.Gateway
            };
            var span = messageEncoderHandler.RpcHandler(reqConnectServer.UniqueId, reqConnectServer);
            if (Setting.IsDebug && Setting.IsDebugSend)
            {
                LogHelper.Debug($"---发送[{ServerType}],[{ServerType.DiscoveryCenter}] {reqConnectServer.ToMessageString()}");
            }

            _discoveryCenterClient.TrySend(span);
        }
    }

    private void DiscoveryCenterClientOnDataReceived(object o, DataEventArgs dataEventArgs)
    {
        var messageData = dataEventArgs.Data.ReadBytes(dataEventArgs.Offset, dataEventArgs.Length);
        var message = messageRouterDecoderHandler.RpcHandler(messageData);
        
        if (message is IActorResponseMessage actorResponseMessage)
        {
            rpcSession.Reply(actorResponseMessage);
        }

        if (message is RespConnectServer respConnectServer && _respConnectServer == null)
        {
            _respConnectServer = respConnectServer;
            ConnectToGateWay();
            return;
        }

        if (message is RespServerOfflineServer respServerOfflineServer)
        {
            if (respServerOfflineServer.ServerType == _respConnectServer?.ServerType && respServerOfflineServer.ServerID == _respConnectServer?.ServerID)
            {
                _respConnectServer = null;
                DisconnectToGateWay();
            }

            return;
        }

        var messageObject = (BaseMessageObject)message;
        LogHelper.Info($"收到发现中心服务器消息：{dataEventArgs.Data}: {messageObject}");
    }

    private void DiscoveryCenterClientOnClosed(object sender, EventArgs eventArgs)
    {
        LogHelper.Info("和网关服务器链接链接断开!");
        // 和网关服务器链接断开，开启重连
        ReconnectionTimer.Start();
    }
}