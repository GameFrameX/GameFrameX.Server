/*using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Message;
using GameFrameX.Proto.BuiltIn;
using Timer = System.Timers.Timer;

namespace GameFrameX.Launcher.StartUp.Router;

internal partial class AppStartUpRouter
{
    /// <summary>
    /// 和网关之间的TCP
    /// </summary>
    AsyncTcpSession _gatewayClient;

    /// <summary>
    /// 和网关之间的心跳对象
    /// </summary>
    private ReqHeartBeat _reqGatewayActorHeartBeat;

    /// <summary>
    /// 网关重连定时器
    /// </summary>
    protected Timer GateWayReconnectionTimer;

    /// <summary>
    /// 和网关之间的心跳
    /// </summary>
    private Timer GateWayHeartBeatTimer { get; set; }

    /// <summary>
    /// 发送消息到网关
    /// </summary>
    /// <param name="networkMessage"></param>
    private void SendToGatewayMessage(IInnerNetworkMessage networkMessage)
    {
        if (!_gatewayClient.IsConnected)
        {
            return;
        }

        var buffer = MessageEncoderHandler.Handler((INetworkMessage)networkMessage);
        if (Setting.IsDebug && Setting.IsDebugSend && !MessageProtoHelper.IsHeartbeat(networkMessage.GetType()))
        {
            // LogHelper.Debug(message.ToSendMessageString(ServerType, ServerType.Gateway));
        }

        bool result = _gatewayClient.TrySend(buffer);
        if (result)
        {
            GateWayHeartBeatTimer.Reset();
        }
    }


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
        _reqGatewayActorHeartBeat = new ReqHeartBeat();
        _reqGatewayActorHeartBeat.SetMessageId(MessageProtoHelper.GetMessageIdByType(typeof(ReqHeartBeat)));
        _gatewayClient = new AsyncTcpSession();
        _gatewayClient.Closed += GateWayClientOnClosed;
        _gatewayClient.DataReceived += GateWayClientOnDataReceived;
        _gatewayClient.Connected += GateWayClientOnConnected;
        _gatewayClient.Error += GateWayClientOnError;
    }

    private void GateWayHeartBeatTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        _reqGatewayActorHeartBeat.Timestamp = TimeHelper.UnixTimeSeconds();
        _reqGatewayActorHeartBeat.UpdateUniqueId();
        InnerNetworkMessage innerNetworkMessage = InnerNetworkMessage.Create(_reqGatewayActorHeartBeat, MessageOperationType.HeartBeat);
        SendToGatewayMessage(innerNetworkMessage);
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
        var message = MessageDecoderHandler.Handler(messageData);
        if (message is MessageObject baseMessageObject)
        {
            if (Setting.IsDebug && Setting.IsDebugReceive && !MessageProtoHelper.IsHeartbeat(message.GetType()))
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
}*/

