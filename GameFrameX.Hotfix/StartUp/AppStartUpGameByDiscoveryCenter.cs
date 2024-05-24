using System.Net;
using System.Timers;
using GameFrameX.Hotfix.Launcher;
using GameFrameX.Launcher.PipelineFilter;
using GameFrameX.NetWork.Messages;
using GameFrameX.Proto.BuiltIn;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SuperSocket.ClientEngine;
using ErrorEventArgs = System.IO.ErrorEventArgs;
using Timer = System.Timers.Timer;

namespace GameFrameX.Launcher.StartUp.Router;

/// <summary>
/// 路由服务器.最后启动。
/// </summary>
internal partial class AppStartUpGame
{
    private BaseSetting Setting;
    MessageGameDecoderHandler messageDecoderHandler = new MessageGameDecoderHandler();
    MessageGameEncoderHandler messageEncoderHandler = new MessageGameEncoderHandler();

    public void Init(BaseSetting baseSetting)
    {
        Setting = baseSetting;
    }

    public void Start()
    {
        StartDiscoveryCenterClient();
        StartGatewayClient();
    }

    private RespConnectServer _respConnectServer;

    /// <summary>
    /// 重连定时器
    /// </summary>
    protected Timer ReconnectionTimer;


    /// <summary>
    /// 心跳定时器
    /// </summary>
    protected Timer HeartBeatTimer { get; set; }

    /// <summary>
    /// 链接到网关的客户端
    /// </summary>
    AsyncTcpSession _discoveryCenterClient;

    private void SendToDiscoveryCenterMessage(long messageUniqueId, IMessage message)
    {
        var span = messageEncoderHandler.RpcHandler(messageUniqueId, message);
        if (Setting.IsDebug && Setting.IsDebugSend)
        {
            LogHelper.Debug($"---发送[{Setting.ServerType}],[{ServerType.DiscoveryCenter}] {message.ToMessageString()}");
        }

        _discoveryCenterClient.TrySend(span);
        // ArrayPool<byte>.Shared.Return(span);
    }

    /// <summary>
    /// 重连定时器
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void ReconnectionTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        // 重连到发现中心服务器
        ConnectToDiscoveryCenter();
    }

    ReqActorHeartBeat reqHeartBeat = new ReqActorHeartBeat();

    /// <summary>
    /// 心跳定时器回调
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void HeartBeatTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        //心跳包
        if (_discoveryCenterClient.IsConnected)
        {
            reqHeartBeat.Timestamp = TimeHelper.UnixTimeSeconds();
            reqHeartBeat.UniqueId = UtilityIdGenerator.GetNextUniqueId();
            SendToDiscoveryCenterMessage(reqHeartBeat.UniqueId, reqHeartBeat);
        }
    }

    private void ConnectToDiscoveryCenter()
    {
        var endPoint = new IPEndPoint(IPAddress.Parse(Setting.DiscoveryCenterIp), Setting.DiscoveryCenterPort);
        _discoveryCenterClient.Connect(endPoint);
    }

    private void StartDiscoveryCenterClient()
    {
        ReconnectionTimer = new Timer() { Interval = 5000 };
        ReconnectionTimer.Elapsed += ReconnectionTimerOnElapsed;
        HeartBeatTimer = new Timer() { Interval = 5000 };
        HeartBeatTimer.Elapsed += HeartBeatTimerOnElapsed;
        _discoveryCenterClient = new AsyncTcpSession();
        _discoveryCenterClient.Closed += DiscoveryCenterClientOnClosed;
        _discoveryCenterClient.DataReceived += DiscoveryCenterClientOnDataReceived;
        _discoveryCenterClient.Connected += DiscoveryCenterClientOnConnected;
        _discoveryCenterClient.Error += DiscoveryCenterClientOnError;

        LogHelper.Info("开始链接到发现中心服务器 ...");
        ConnectToDiscoveryCenter();
    }


    private void DiscoveryCenterClientOnError(object sender, SuperSocket.ClientEngine.ErrorEventArgs errorEventArgs)
    {
        LogHelper.Info("和发现中心服务器链接链接发生错误!" + errorEventArgs);
        DiscoveryCenterClientOnClosed(_discoveryCenterClient, errorEventArgs);
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
                LogHelper.Debug($"---发送[{ServerType.Game}],[{ServerType.DiscoveryCenter}] {reqConnectServer.ToMessageString()}");
            }

            _discoveryCenterClient.TrySend(span);
        }
    }

    private void DiscoveryCenterClientOnDataReceived(object o, DataEventArgs dataEventArgs)
    {
        var messageData = dataEventArgs.Data.ReadBytes(dataEventArgs.Offset, dataEventArgs.Length);
        var message = messageDecoderHandler.RpcHandler(messageData);

        if (message is IActorResponseMessage actorResponseMessage)
        {
            // rpcSession.Reply(actorResponseMessage);
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
        if (Setting.IsDebug && Setting.IsDebugReceive)
        {
            LogHelper.Debug($"---收到[{ServerType.DiscoveryCenter} To {Setting.ServerType}] {messageObject.ToMessageString()}");
        }
    }

    private void DiscoveryCenterClientOnClosed(object sender, EventArgs eventArgs)
    {
        LogHelper.Info("和网关服务器链接链接断开!");
        // 和网关服务器链接断开，开启重连
        ReconnectionTimer.Start();
    }

    public void Stop()
    {
        if (_discoveryCenterClient != null)
        {
            _discoveryCenterClient.Close();
            _discoveryCenterClient = null;
        }

        if (_gatewayClient != null)
        {
            _gatewayClient.Close();
            _gatewayClient = null;
        }

        ReconnectionTimer.Stop();
        HeartBeatTimer.Stop();
        ReconnectionTimer = null;
        HeartBeatTimer = null;
        _respConnectServer = null;
    }
}