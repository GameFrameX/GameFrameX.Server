using GameFrameX.Proto.BuiltIn;
using Timer = System.Timers.Timer;

namespace GameFrameX.Launcher.StartUp.Router;

internal partial class AppStartUpRouter
{
    AsyncTcpSession _gatewayClient;

    protected Timer GateWayReconnectionTimer;
    private Timer GateWayHeartBeatTimer;

    private void SendToGatewayMessage(IMessage message)
    {
        if (!_gatewayClient.IsConnected)
        {
            return;
        }

        var span = messageEncoderHandler.Handler(message);
        if (Setting.IsDebug && Setting.IsDebugSend)
        {
            LogHelper.Debug(message.ToSendMessageString(ServerType, ServerType.Gateway));
        }

        bool result = _gatewayClient.TrySend(span);
        if (result)
        {
            GateWayHeartBeatTimer.Reset();
        }
    }

    ReqActorHeartBeat reqGatewayActorHeartBeat;

    private void StartGatewayClient()
    {
        GateWayReconnectionTimer = new Timer
        {
            Interval = 5000
        };
        GateWayReconnectionTimer.Elapsed += GateWayReconnectionTimerOnElapsed;
        GateWayReconnectionTimer.Start();

        GateWayHeartBeatTimer = new Timer
        {
            Interval = 5000
        };
        GateWayHeartBeatTimer.Elapsed += GateWayHeartBeatTimerOnElapsed;
        GateWayHeartBeatTimer.Start();
        reqGatewayActorHeartBeat = new ReqActorHeartBeat();
        _gatewayClient = new AsyncTcpSession();
        _gatewayClient.Closed += GateWayClientOnClosed;
        _gatewayClient.DataReceived += GateWayClientOnDataReceived;
        _gatewayClient.Connected += GateWayClientOnConnected;
        _gatewayClient.Error += GateWayClientOnError;
    }

    private void GateWayHeartBeatTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        reqGatewayActorHeartBeat.Timestamp = TimeHelper.UnixTimeSeconds();
        reqGatewayActorHeartBeat.UpdateUniqueId();
        SendToGatewayMessage(reqGatewayActorHeartBeat);
    }

    private void GateWayReconnectionTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        ConnectToGateWay();
    }

    private void GateWayClientOnError(object sender, ErrorEventArgs e)
    {
        LogHelper.Info("和网关服务器链接链接发生错误!" + e.Exception.Message);
        GateWayClientOnClosed(sender, e);
    }

    private void GateWayClientOnConnected(object sender, EventArgs e)
    {
        // 和网关服务器链接成功，关闭重连
        GateWayReconnectionTimer.Stop();
        GateWayHeartBeatTimer.Start();
        LogHelper.Info("和网关服务器链接链接成功!");
    }

    private void GateWayClientOnDataReceived(object o, DataEventArgs dataEventArgs)
    {
        var messageData = dataEventArgs.Data.ReadBytes(dataEventArgs.Offset, dataEventArgs.Length);
        var message = messageDecoderHandler.Handler(messageData);
        if (Setting.IsDebug && Setting.IsDebugReceive)
        {
            if (message is MessageObject baseMessageObject)
            {
                LogHelper.Info($"收到网关服务器消息：{baseMessageObject.ToMessageString()}");
            }
        }
        // if (message is IActorResponseMessage actorResponseMessage)
        // {
        //     rpcSession.Reply(actorResponseMessage);
        // }


        // var messageObject = (BaseMessageObject)message;
        // LogHelper.Info($"收到发现中心服务器消息：{dataEventArgs.Data}: {messageObject}");
    }

    private void GateWayClientOnClosed(object sender, EventArgs eventArgs)
    {
        LogHelper.Info("和网关服务器链接链接断开! 开启重连");
        // 和网关服务器链接断开，开启重连
        GateWayReconnectionTimer.Start();
        GateWayHeartBeatTimer.Stop();
    }

    private void ConnectToGateWay()
    {
        if (ConnectTargetServer == null)
        {
            return;
        }

        var endPoint = new IPEndPoint(IPAddress.Parse(ConnectTargetServer.TargetIP), ConnectTargetServer.TargetPort);
        _gatewayClient.Connect(endPoint);
    }

    private void DisconnectToGateWay()
    {
        _gatewayClient?.Close();
    }
}