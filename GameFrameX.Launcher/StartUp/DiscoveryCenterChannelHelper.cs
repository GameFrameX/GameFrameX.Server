using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Message;
using GameFrameX.Proto.BuiltIn;
using Timer = System.Timers.Timer;

namespace GameFrameX.Launcher.StartUp;

public sealed class DiscoveryCenterChannelHelper
{
    /// <summary>
    /// 链接到发现中心的客户端
    /// </summary>
    readonly AsyncTcpSession _discoveryCenterClient;

    /// <summary>
    /// 配置信息
    /// </summary>
    private readonly AppSetting _setting;

    private readonly RpcSession _rpcSession;

    private readonly IMessageEncoderHandler _messageEncoderHandler;
    private readonly IMessageDecoderHandler _messageDecoderHandler;

    /// <summary>
    /// 心跳计时器
    /// </summary>
    private readonly Timer _heartBeatTimer;

    /// <summary>
    /// 重连计时器
    /// </summary>
    private readonly Timer _reconnectionTimer;

    /// <summary>
    /// 心跳对象
    /// </summary>
    private readonly ReqHeartBeat _reqHeartBeat;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="setting"></param>
    /// <param name="messageEncoderHandler"></param>
    /// <param name="messageDecoderHandler"></param>
    /// <param name="heartBeatInterval"></param>
    /// <param name="reconnectInterval"></param>
    public DiscoveryCenterChannelHelper(AppSetting setting, IMessageEncoderHandler messageEncoderHandler, IMessageDecoderHandler messageDecoderHandler, int heartBeatInterval = 15000, int reconnectInterval = 5000)
    {
        _setting = setting;
        _rpcSession = new RpcSession();
        _messageEncoderHandler = messageEncoderHandler;
        _messageDecoderHandler = messageDecoderHandler;
        _heartBeatTimer = new Timer(heartBeatInterval);
        _heartBeatTimer.Elapsed += HeartBeatTimerOnElapsed;
        _reconnectionTimer = new Timer(reconnectInterval);
        _reconnectionTimer.Elapsed += ReconnectionTimerOnElapsed;
        _reqHeartBeat = new ReqHeartBeat();
        MessageProtoHelper.SetMessageIdAndOperationType(_reqHeartBeat);
        _discoveryCenterClient = new AsyncTcpSession();
        _discoveryCenterClient.Closed += DiscoveryCenterClientOnClosed;
        _discoveryCenterClient.DataReceived += DiscoveryCenterClientOnDataReceived;
        _discoveryCenterClient.Connected += DiscoveryCenterClientOnConnected;
        _discoveryCenterClient.Error += DiscoveryCenterClientOnError;
        _ = Task.Run(RpcHandler);
    }

    private void RpcHandler()
    {
        while (true)
        {
            var message = _rpcSession.Handler();
            if (message == null || IsConnected == false)
            {
                Thread.Sleep(1);
                continue;
            }

            MessageProtoHelper.SetMessageIdAndOperationType(message.RequestMessage);
            InnerMessageObjectHeader messageObjectHeader = new InnerMessageObjectHeader
            {
                ServerId = _setting.ServerId,
            };
            var message2 = InnerNetworkMessage.Create(message.RequestMessage, messageObjectHeader);
            Send(message2);
        }
    }

    /// <summary>
    /// 心跳定时器回调
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void HeartBeatTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        _reqHeartBeat.UpdateUniqueId();
        _reqHeartBeat.Timestamp = TimeHelper.UnixTimeMilliseconds();

        InnerMessageObjectHeader messageObjectHeader = new InnerMessageObjectHeader
        {
            ServerId = _setting.ServerId,
        };

        var innerNetworkMessage = InnerNetworkMessage.Create(_reqHeartBeat, messageObjectHeader);
        Send(innerNetworkMessage);
    }

    /// <summary>
    /// 重连定时器回调
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void ReconnectionTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
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
        _reconnectionTimer.Stop();
        // 开启和网关服务器的心跳
        _heartBeatTimer.Start();
        DiscoveryCenterClientOnConnectedHandler(sender, e);
    }

    /// <summary>
    /// 链接到发现中心服务器
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DiscoveryCenterClientOnConnectedHandler(object sender, EventArgs e)
    {
        // 这里要注册到发现中心
        var reqRegisterServer = new ReqRegisterServer
        {
            ServerId = _setting.ServerId,
            ServerType = _setting.ServerType,
            ServerName = _setting.ServerName,
            InnerIp = _setting.InnerIp,
            InnerPort = _setting.InnerPort,
            OuterIp = _setting.OuterIp,
            OuterPort = _setting.OuterPort,
        };
        MessageProtoHelper.SetMessageIdAndOperationType(reqRegisterServer);
        InnerMessageObjectHeader messageObjectHeader = new InnerMessageObjectHeader
        {
            ServerId = _setting.ServerId,
        };
        var innerNetworkMessage = InnerNetworkMessage.Create(reqRegisterServer, messageObjectHeader);
        Send(innerNetworkMessage);
    }

    private void DiscoveryCenterClientOnDataReceived(object o, DataEventArgs dataEventArgs)
    {
        var messageData = dataEventArgs.Data.ReadBytes(dataEventArgs.Offset, dataEventArgs.Length);
        var message = _messageDecoderHandler.Handler(messageData);
        if (message == null)
        {
            LogHelper.Error("数据解析失败！");
            return;
        }

        if (_setting.IsDebug && _setting.IsDebugReceive && !MessageProtoHelper.IsHeartbeat(message.GetType()))
        {
            LogHelper.Debug("---收到发现中心发来的消息:" + message.ToFormatMessageString());
        }

        if (message is IResponseMessage actorResponseMessage)
        {
            bool result = _rpcSession.Reply(actorResponseMessage);
            if (result)
            {
                return;
            }
        }

        /*
        if (message is RespConnectServer respConnectServer && ConnectTargetServer == null)
        {
            ConnectTargetServer = respConnectServer;
            ConnectTargetServerTimer?.Stop();
            ConnectServerHandler();
            return;
        }

        if (message is RespServerOfflineServer respServerOfflineServer)
        {
            if (respServerOfflineServer.ServerType == ConnectTargetServer?.ServerType && respServerOfflineServer.ServerID == ConnectTargetServer?.ServerID)
            {
                ConnectTargetServer = null;
                ConnectTargetServerTimer?.Start();
                DisconnectServerHandler();
            }

            return;
        }


        DiscoveryCenterDataReceived(message);*/
    }

    private void DiscoveryCenterClientOnClosed(object sender, EventArgs eventArgs)
    {
        LogHelper.Info("和网关服务器链接链接断开!");
        // 和网关服务器链接断开，开启重连
        _reconnectionTimer.Start();
    }

    /// <summary>
    /// 给发现中心发送消息
    /// </summary>
    /// <param name="message"></param>
    public bool Call<T>(IRequestMessage message) where T : class, IResponseMessage, new()
    {
        var buffer = _messageEncoderHandler.Handler(message);
        if (_setting.IsDebug && _setting.IsDebugSend)
        {
            LogHelper.Debug("--发送到发现中心 " + message.ToFormatMessageString());
        }

        if (buffer == null)
        {
            return false;
        }

        return _discoveryCenterClient.TrySend(buffer);
    }

    /// <summary>
    /// 给发现中心发送消息
    /// </summary>
    /// <param name="networkMessage"></param>
    public bool Send(IInnerNetworkMessage networkMessage)
    {
        var buffer = _messageEncoderHandler.Handler(networkMessage);
        if (_setting.IsDebug && _setting.IsDebugSend)
        {
            LogHelper.Debug("--发送到发现中心 " + networkMessage.ToFormatMessageString());
        }

        if (buffer == null)
        {
            return false;
        }

        return _discoveryCenterClient.TrySend(buffer);
    }

    private void ConnectToDiscoveryCenter()
    {
        var endPoint = new IPEndPoint(IPAddress.Parse(_setting.DiscoveryCenterIp), _setting.DiscoveryCenterPort);
        _discoveryCenterClient.Connect(endPoint);
    }


    /// <summary>
    /// 是否已经链接状态
    /// </summary>
    public bool IsConnected
    {
        get { return _discoveryCenterClient.IsConnected; }
    }

    public void Start()
    {
        LogHelper.Info("开始链接到发现中心服务器 ...");
        _reconnectionTimer.Start();
    }

    public void Stop()
    {
        _reconnectionTimer.Stop();
        _heartBeatTimer.Stop();
        _discoveryCenterClient.Close();
    }
}