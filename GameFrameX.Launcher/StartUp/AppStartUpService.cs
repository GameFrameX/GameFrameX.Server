using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Message;
using GameFrameX.Proto.BuiltIn;
using Timer = System.Timers.Timer;

namespace GameFrameX.Launcher.StartUp;

public abstract class AppStartUpService : AppStartUpBase
{
    /// <summary>
    /// 链接到发现中心的客户端
    /// </summary>
    AsyncTcpSession discoveryCenterClient;

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
    /// 是否连接到发现中心
    /// </summary>
    protected virtual bool IsConnectDiscoveryServer { get; } = true;

    /// <summary>
    /// 连接的目标信息
    /// </summary>
    protected RespConnectServer ConnectTargetServer { get; private set; }

    protected IMessageEncoderHandler MessageEncoderHandler { get; private set; }
    protected IMessageDecoderHandler MessageDecoderHandler { get; private set; }
    private readonly ReqHeartBeat _reqDiscoveryCenterActorHeartBeat;

    protected void SetMessageHandler(IMessageEncoderHandler messageEncoderHandler, IMessageDecoderHandler messageDecoderHandler)
    {
        messageDecoderHandler.CheckNotNull(nameof(messageDecoderHandler));
        messageEncoderHandler.CheckNotNull(nameof(messageEncoderHandler));
        MessageEncoderHandler = messageEncoderHandler;
        MessageDecoderHandler = messageDecoderHandler;
    }

    protected AppStartUpService()
    {
        RpcSession = new RpcSession();
        _reqDiscoveryCenterActorHeartBeat = new ReqHeartBeat();
    }

    private Timer ConnectTargetServerTimer { get; set; }

    /// <summary>
    /// 启动服务器
    /// </summary>
    /// <returns></returns>
    public override Task StartAsync()
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

        if (IsConnectDiscoveryServer)
        {
            StartDiscoveryCenterClient();
            _ = Task.Run(RpcHandler);
        }

        return Task.CompletedTask;
    }

    private void RpcHandler()
    {
        while (true)
        {
            var message = RpcSession.Handler();
            if (message == null || discoveryCenterClient.IsConnected == false)
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
    protected void SendToDiscoveryCenterMessage(INetworkMessage message)
    {
        var span = MessageEncoderHandler.Handler(message);
        if (Setting.IsDebug && Setting.IsDebugSend)
        {
            LogHelper.Debug(message.ToFormatMessageString());
        }

        discoveryCenterClient.TrySend(span);
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
        if (!discoveryCenterClient.IsConnected)
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
        if (!IsRequestConnectServer)
        {
            return;
        }

        SendConnectTargetServer();
    }

    /// <summary>
    /// 重连定时器
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected override void ReconnectionTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        if (!IsRequestConnectServer)
        {
            return;
        }

        // 重连到发现中心服务器
        ConnectToDiscoveryCenter();
    }

    private void ConnectToDiscoveryCenter()
    {
        var endPoint = new IPEndPoint(IPAddress.Parse(Setting.DiscoveryCenterIp), Setting.DiscoveryCenterPort);
        discoveryCenterClient.Connect(endPoint);
    }

    private void StartDiscoveryCenterClient()
    {
        discoveryCenterClient = new AsyncTcpSession();
        discoveryCenterClient.Closed += DiscoveryCenterClientOnClosed;
        discoveryCenterClient.DataReceived += DiscoveryCenterClientOnDataReceived;
        discoveryCenterClient.Connected += DiscoveryCenterClientOnConnected;
        discoveryCenterClient.Error += DiscoveryCenterClientOnError;

        LogHelper.Info("开始链接到发现中心服务器 ...");
        ReconnectionTimer.Start();
    }


    private void DiscoveryCenterClientOnError(object sender, ErrorEventArgs e)
    {
        LogHelper.Info("和发现中心服务器链接链接发生错误!" + e);
        DiscoveryCenterClientOnClosed(discoveryCenterClient, e);
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
    protected virtual void DiscoveryCenterDataReceived(INetworkMessage message)
    {
    }

    private void DiscoveryCenterClientOnDataReceived(object o, DataEventArgs dataEventArgs)
    {
        var messageData = dataEventArgs.Data.ReadBytes(dataEventArgs.Offset, dataEventArgs.Length);
        var message = MessageDecoderHandler.Handler(messageData);
        if (message == null)
        {
            LogHelper.Error("数据解析失败！");
            return;
        }

        if (Setting.IsDebug && Setting.IsDebugReceive && !MessageProtoHelper.IsHeartbeat(message.GetType()))
        {
            LogHelper.Debug(message.ToMessageString());
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

    /// <summary>
    /// 终止服务器
    /// </summary>
    /// <param name="message">终止原因</param>
    /// <returns></returns>
    public override Task StopAsync(string message = "")
    {
        HeartBeatTimer?.Close();
        ReconnectionTimer?.Close();
        ConnectTargetServerTimer?.Close();
        if (discoveryCenterClient != null)
        {
            discoveryCenterClient.Close();
            discoveryCenterClient = null;
        }

        return base.StopAsync(message);
    }
    /// <summary>
    /// 添加事件总线的注册
    /// </summary>
    protected virtual void AddEventBusRegister()
    {
    }
}