using System.Net;
using System.Timers;
using GameFrameX.NetWork.Messages;
using GameFrameX.Proto.BuiltIn;
using SuperSocket.ClientEngine;
using ErrorEventArgs = System.IO.ErrorEventArgs;
using Timer = System.Timers.Timer;

namespace GameFrameX.Launcher.StartUp.Router;

internal partial class AppStartUpGame
{
    AsyncTcpSession _gatewayClient;

    protected Timer GateWayReconnectionTimer;

    private void SendToGatewayMessage(long messageUniqueId, IMessage message)
    {
        if (!_gatewayClient.IsConnected)
        {
            return;
        }

        var span = messageEncoderHandler.Handler(message);
        if (Setting.IsDebug && Setting.IsDebugSend)
        {
            LogHelper.Debug(message.ToSendMessageString(Setting.ServerType, ServerType.Gateway));
        }

        _gatewayClient.TrySend(span);
    }

    private void StartGatewayClient()
    {
        GateWayReconnectionTimer = new Timer
        {
            Interval = 5000
        };
        GateWayReconnectionTimer.Elapsed += GateWayReconnectionTimerOnElapsed;
        GateWayReconnectionTimer.Start();
        _gatewayClient = new AsyncTcpSession();
        _gatewayClient.Closed += GateWayClientOnClosed;
        _gatewayClient.DataReceived += GateWayClientOnDataReceived;
        _gatewayClient.Connected += GateWayClientOnConnected;
        _gatewayClient.Error += GateWayClientOnError;
    }

    private void GateWayReconnectionTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        ConnectToGateWay();
    }

    private void GateWayClientOnError(object sender, SuperSocket.ClientEngine.ErrorEventArgs errorEventArgs)
    {
        LogHelper.Info("和发现中心服务器链接链接发生错误!" + errorEventArgs);
        GateWayReconnectionTimer.Start();
        // DiscoveryCenterClientOnClosed(_discoveryCenterClient, e);
    }

    private void GateWayClientOnConnected(object sender, EventArgs e)
    {
        // 和网关服务器链接成功，关闭重连
        GateWayReconnectionTimer.Stop();
        LogHelper.Info("和发现中心服务器链接链接成功!");
    }

    private void GateWayClientOnDataReceived(object o, DataEventArgs dataEventArgs)
    {
        var messageData = dataEventArgs.Data.ReadBytes(dataEventArgs.Offset, dataEventArgs.Length);
        // var message = messageRouterDecoderHandler.RpcHandler(messageData);
        // if (message is IActorResponseMessage actorResponseMessage)
        // {
        //     rpcSession.Reply(actorResponseMessage);
        // }


        // var messageObject = (BaseMessageObject)message;
        // LogHelper.Info($"收到发现中心服务器消息：{dataEventArgs.Data}: {messageObject}");
    }

    private void GateWayClientOnClosed(object sender, EventArgs eventArgs)
    {
        LogHelper.Info("和网关服务器链接链接断开!");
        // 和网关服务器链接断开，开启重连
        GateWayReconnectionTimer.Start();
    }

    private void ConnectToGateWay()
    {
        if (_respConnectServer == null)
        {
            return;
        }

        var endPoint = new IPEndPoint(IPAddress.Parse(_respConnectServer.TargetIP), _respConnectServer.TargetPort);
        _gatewayClient.Connect(endPoint);
    }

    private void DisconnectToGateWay()
    {
        _gatewayClient?.Close();
    }
}