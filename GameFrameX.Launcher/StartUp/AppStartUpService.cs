using GameFrameX.Proto.BuiltIn;
using Timer = System.Timers.Timer;

namespace GameFrameX.Launcher.StartUp;

public abstract class AppStartUpService : AppStartUpBase
{
    /// <summary>
    /// 链接到发现中心的客户端
    /// </summary>
    AsyncTcpSession _discoveryCenterClient;

    protected RpcSession RpcSession { get; private set; }

    /// <summary>
    /// 从发现中心请求的目标服务器类型
    /// </summary>
    protected virtual ServerType GetServerType { get; } = ServerType.None;

    /// <summary>
    /// 是否请求其他服务信息
    /// </summary>
    protected virtual bool IsRequestConnectServer { get; } = true;

    /// <summary>
    /// 连接的目标信息
    /// </summary>
    protected RespConnectServer ConnectTargetServer { get; private set; }

    private readonly IMessageEncoderHandler _messageEncoderHandler;
    private readonly IMessageDecoderHandler _messageDecoderHandler;
    readonly ReqHeartBeat _reqDiscoveryCenterActorHeartBeat;

    protected AppStartUpService(IMessageEncoderHandler messageEncoderHandler, IMessageDecoderHandler messageDecoderHandler)
    {
        RpcSession = new RpcSession();
        _reqDiscoveryCenterActorHeartBeat = new ReqHeartBeat();
        _messageEncoderHandler = messageEncoderHandler;
        _messageDecoderHandler = messageDecoderHandler;
    }

    private Timer ConnectTargetServerTimer { get; set; }

    public override Task EnterAsync()
    {
        if (IsRequestConnectServer)
        {
            ConnectTargetServerTimer = new Timer
            {
                Interval = 3000
            };
            ConnectTargetServerTimer.Elapsed += ConnectTargetServerTimerOnElapsed;
            ConnectTargetServerTimer.Start();
        }

        StartDiscoveryCenterClient();
        _ = Task.Run(RpcHandler);
        return Task.CompletedTask;
    }

    private void RpcHandler()
    {
        while (true)
        {
            var message = RpcSession.Handler();
            if (message == null || _discoveryCenterClient.IsConnected == false)
            {
                Thread.Sleep(1);
                continue;
            }

            SendToDiscoveryCenterMessage(message.RequestMessage);
        }
    }

    /// <summary>
    /// 给发现中心发送消息
    /// </summary>
    /// <param name="message"></param>
    protected void SendToDiscoveryCenterMessage(IMessage message)
    {
        var span = _messageEncoderHandler.Handler(message);
        if (Setting.IsDebug && Setting.IsDebugSend)
        {
            LogHelper.Debug(message.ToSendMessageString(ServerType, ServerType.DiscoveryCenter));
        }

        _discoveryCenterClient.TrySend(span);
    }

    protected override void HeartBeatTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        _reqDiscoveryCenterActorHeartBeat.Timestamp = TimeHelper.UnixTimeSeconds();
        _reqDiscoveryCenterActorHeartBeat.UpdateUniqueId();
        SendToDiscoveryCenterMessage(_reqDiscoveryCenterActorHeartBeat);
    }

    /// <summary>
    /// 请求链接目标
    /// </summary>
    void SendConnectTargetServer()
    {
        if (!_discoveryCenterClient.IsConnected)
        {
            return;
        }

        if (ConnectTargetServer == null)
        {
            ReqConnectServer reqConnectServer = new ReqConnectServer
            {
                ServerType = GetServerType
            };
            SendToDiscoveryCenterMessage(reqConnectServer);
        }
    }

    /// <summary>
    /// 链接目标重试
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void ConnectTargetServerTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        SendConnectTargetServer();
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
        ReconnectionTimer.Start();
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
        DiscoveryCenterClientOnConnectedHandler(sender, e);
        if (IsRequestConnectServer)
        {
            SendConnectTargetServer();
        }
    }

    /// <summary>
    /// 链接到发现中心服务器
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DiscoveryCenterClientOnConnectedHandler(object sender, EventArgs e)
    {
        // 这里要注册到发现中心
        ReqRegisterServer reqRegisterServer = new ReqRegisterServer
        {
            ServerID = Setting.ServerId,
            ServerType = Setting.ServerType,
            ServerName = Setting.ServerName,
            InnerIP = Setting.InnerIp,
            InnerPort = Setting.InnerPort,
            OuterIP = Setting.OuterIp,
            OuterPort = Setting.OuterPort
        };
        SendToDiscoveryCenterMessage(reqRegisterServer);
    }

    /// <summary>
    /// 获取到连接的目标
    /// </summary>
    protected virtual void ConnectServerHandler()
    {
    }

    /// <summary>
    /// 连接的目标下线
    /// </summary>
    protected virtual void DisconnectServerHandler()
    {
    }

    /// <summary>
    /// 收到发现中心推送的非特殊消息
    /// </summary>
    /// <param name="message"></param>
    protected virtual void DiscoveryCenterDataReceived(IMessage message)
    {
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

        if (Setting.IsDebug && Setting.IsDebugReceive && !MessageProtoHelper.IsHeartbeat(message.GetType()))
        {
            LogHelper.Debug(message.ToReceiveMessageString(ServerType.DiscoveryCenter, ServerType));
        }

        if (message is IResponseMessage actorResponseMessage)
        {
            RpcSession.Reply(actorResponseMessage);
        }

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


        DiscoveryCenterDataReceived(message);
    }

    private void DiscoveryCenterClientOnClosed(object sender, EventArgs eventArgs)
    {
        LogHelper.Info("和网关服务器链接链接断开!");
        // 和网关服务器链接断开，开启重连
        ReconnectionTimer.Start();
    }

    public override Task Stop(string message = "")
    {
        HeartBeatTimer?.Close();
        ReconnectionTimer?.Close();
        ConnectTargetServerTimer?.Close();
        if (_discoveryCenterClient != null)
        {
            _discoveryCenterClient.Close();
            _discoveryCenterClient = null;
        }

        return base.Stop(message);
    }
}