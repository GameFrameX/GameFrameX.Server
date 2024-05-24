using GameFrameX.Proto.BuiltIn;


/// <summary>
/// 网关服务器
/// </summary>
internal sealed partial class AppStartUpGateway
{
    /// <summary>
    /// 和发现中心链接的客户端
    /// </summary>
    private AsyncTcpSession _discoveryCenterClient;

    ReqActorHeartBeat reqHeartBeat = new ReqActorHeartBeat();

    #region DiscoveryCenterClient

    protected override void HeartBeatTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        //心跳包
        if (_discoveryCenterClient.IsConnected)
        {
            reqHeartBeat.Timestamp = TimeHelper.UnixTimeSeconds();
            reqHeartBeat.UniqueId = UtilityIdGenerator.GetNextUniqueId();
            SendToDiscoveryCenterMessage(reqHeartBeat);
        }
    }

    protected override void ReconnectionTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        ConnectToDiscovery();
    }

    private void StartDiscoveryCenterClient()
    {
        _discoveryCenterClient = new AsyncTcpSession();
        _discoveryCenterClient.Connected += DiscoveryCenterClientOnConnected;
        _discoveryCenterClient.Closed += DiscoveryCenterClientOnClosed;
        _discoveryCenterClient.DataReceived += DiscoveryCenterClientOnDataReceived;
        _discoveryCenterClient.Error += DiscoveryCenterClientOnError;
        ConnectToDiscovery();
    }

    private void ConnectToDiscovery()
    {
        _discoveryCenterClient.Connect(new DnsEndPoint(Setting.DiscoveryCenterIp, Setting.DiscoveryCenterPort));
    }

    /// <summary>
    /// 从发现中心断开
    /// </summary>
    private void DisconnectToDiscovery()
    {
        _discoveryCenterClient?.Close();
    }

    private void DiscoveryCenterClientOnDataReceived(object sender, DataEventArgs e)
    {
        var messageObject = messageDecoderHandler.Handler(e.Data.ReadBytes(e.Offset, e.Length));
        if (messageObject == null)
        {
            LogHelper.Error("数据解析失败：" + e.Data.ReadBytes(e.Offset, e.Length));
            return;
        }

        if (Setting.IsDebug && Setting.IsDebugReceive)
        {
            LogHelper.Debug($"---收到[{ServerType}],[{ServerType.DiscoveryCenter}] {messageObject.ToMessageString()}");
        }
    }

    private void DiscoveryCenterClientOnConnected(object sender, EventArgs e)
    {
        LogHelper.Info("和中心服务器链接成功, 开始心跳");
        HeartBeatTimer.Start();
        ReconnectionTimer.Stop();
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

    private void DiscoveryCenterClientOnClosed(object sender, EventArgs eventArgs)
    {
        LogHelper.Info("和中心服务器网络连接断开, 开始重连：断开信息:" + eventArgs);
        HeartBeatTimer.Stop();
        ReconnectionTimer.Start();
    }

    private void DiscoveryCenterClientOnError(object sender, SuperSocket.ClientEngine.ErrorEventArgs errorEventArgs)
    {
        LogHelper.Info("和中心服务器连接错误, 开始重连:错误信息：" + errorEventArgs.Exception);
        // 开启重连
        HeartBeatTimer.Stop();
        ReconnectionTimer.Start();
    }

    /// <summary>
    /// 发送消息到发现中心
    /// </summary>
    /// <param name="message"></param>
    private void SendToDiscoveryCenterMessage(IMessage message)
    {
        var span = messageEncoderHandler.Handler(message);
        if (Setting.IsDebug && Setting.IsDebugSend)
        {
            LogHelper.Debug($"---发送[{ServerType}],[{ServerType.DiscoveryCenter}] {message.ToMessageString()}");
        }

        _discoveryCenterClient.TrySend(span);
    }

    #endregion
}