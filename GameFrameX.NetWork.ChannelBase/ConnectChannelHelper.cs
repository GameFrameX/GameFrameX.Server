using System.Net;
using System.Timers;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Message;
using GameFrameX.NetWork.Messages;
using GameFrameX.Proto.BuiltIn;
using GameFrameX.Setting;
using GameFrameX.SuperSocket.ClientEngine;
using GameFrameX.Utility;
using Microsoft.Extensions.ObjectPool;
using Timer = System.Timers.Timer;

namespace GameFrameX.NetWork.ChannelBase;

/// <summary>
/// 连接到其他服务器的客户端
/// </summary>
public sealed class ConnectChannelHelper
{
    /// <summary>
    /// 链接到其他服务器的客户端
    /// </summary>
    readonly AsyncTcpSession _connectClient;

    /// <summary>
    /// 配置信息
    /// </summary>
    private readonly AppSetting _setting;

    /// <summary>
    /// RPC会话 对象
    /// </summary>
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
    private readonly ReqActorHeartBeat _reqActorHeartBeat;

    /// <summary>
    /// 消息头的对象池
    /// </summary>
    private readonly ObjectPool<InnerMessageObjectHeader> _innerMessageObjectHeader;

    /// <summary>
    /// 非RPC消息处理回调
    /// </summary>
    private readonly Action<IMessage> _messageHandler;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="setting">设置</param>
    /// <param name="messageEncoderHandler">消息编码器</param>
    /// <param name="messageDecoderHandler">消息解码器</param>
    /// <param name="messageHandler">非RPC消息处理回调</param>
    /// <param name="heartBeatInterval">心跳时间间隔,单位毫秒</param>
    /// <param name="reconnectInterval">重连时间间隔,单位毫秒</param>
    public ConnectChannelHelper(AppSetting setting, IMessageEncoderHandler messageEncoderHandler, IMessageDecoderHandler messageDecoderHandler, Action<IMessage> messageHandler, int heartBeatInterval = 15000, int reconnectInterval = 5000)
    {
        setting.CheckNotNull(nameof(setting));
        messageHandler.CheckNotNull(nameof(messageHandler));
        messageEncoderHandler.CheckNotNull(nameof(messageEncoderHandler));
        messageDecoderHandler.CheckNotNull(nameof(messageDecoderHandler));
        _setting = setting;
        _rpcSession = new RpcSession();
        _messageHandler = messageHandler;
        _messageEncoderHandler = messageEncoderHandler;
        _messageDecoderHandler = messageDecoderHandler;
        _heartBeatTimer = new Timer(heartBeatInterval);
        _heartBeatTimer.Elapsed += HeartBeatTimerOnElapsed;
        _reconnectionTimer = new Timer(reconnectInterval);
        _reconnectionTimer.Elapsed += ReconnectionTimerOnElapsed;
        _reqActorHeartBeat = new ReqActorHeartBeat();
        _innerMessageObjectHeader = new DefaultObjectPoolProvider().Create<InnerMessageObjectHeader>();
        MessageProtoHelper.SetMessageIdAndOperationType(_reqActorHeartBeat);
        _connectClient = new AsyncTcpSession();
        _connectClient.Closed += ConnectClientOnClosed;
        _connectClient.DataReceived += ConnectClientOnDataReceived;
        _connectClient.Connected += ConnectClientOnConnected;
        _connectClient.Error += ConnectClientOnError;
        _ = Task.Run(RpcHandler);
    }

    private void RpcHandler()
    {
        while (true)
        {
            _rpcSession.Tick(1);
            if (IsConnected == false)
            {
                Thread.Sleep(1);
                continue;
            }

            var message = _rpcSession.TryPeek();
            if (message == null)
            {
                Thread.Sleep(1);
                continue;
            }

            MessageProtoHelper.SetMessageIdAndOperationType(message.RequestMessage);
            InnerMessageObjectHeader messageObjectHeader = _innerMessageObjectHeader.Get();
            messageObjectHeader.ServerId = _setting.ServerId;
            var innerNetworkMessage = InnerNetworkMessage.Create(message.RequestMessage, messageObjectHeader);
            var isSuccess = Send(innerNetworkMessage);
            if (isSuccess)
            {
                _rpcSession.Handler();
            }

            _innerMessageObjectHeader.Return(messageObjectHeader);
        }
    }

    /// <summary>
    /// 心跳定时器回调
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void HeartBeatTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        _reqActorHeartBeat.UpdateUniqueId();
        _reqActorHeartBeat.Timestamp = TimeHelper.UnixTimeMilliseconds();
        _rpcSession.Send(_reqActorHeartBeat);
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

    private void ConnectClientOnError(object sender, SuperSocket.ClientEngine.ErrorEventArgs errorEventArgs)
    {
        LogHelper.Info($"和服务器{TargetEndPoint}链接链接发生错误!{errorEventArgs}");
        ConnectClientOnClosed(_connectClient, errorEventArgs);
    }

    private void ConnectClientOnConnected(object sender, EventArgs eventArgs)
    {
        LogHelper.Info($"和服务器{TargetEndPoint}链接链接成功!");
        // 和服务器链接成功，关闭重连
        _reconnectionTimer.Stop();
        // 开启和服务器的心跳
        _heartBeatTimer.Start();
        RegisterServerHandler();
    }

    /// <summary>
    /// 注册消息到目标服务器
    /// </summary>
    private void RegisterServerHandler()
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
        _rpcSession.Send(reqRegisterServer);
    }

    /// <summary>
    /// 接收到服务器发回的数据
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="dataEventArgs"></param>
    private void ConnectClientOnDataReceived(object sender, DataEventArgs dataEventArgs)
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
            LogHelper.Debug($"---收到服务器[{TargetEndPoint}]发来的消息:{message.ToFormatMessageString()}");
        }

        if (message is IResponseMessage actorResponseMessage)
        {
            bool result = _rpcSession.Reply(actorResponseMessage);
            if (result)
            {
                return;
            }
        }

        _messageHandler?.Invoke(message);

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

    /// <summary>
    /// 和服务器连接断开
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    private void ConnectClientOnClosed(object sender, EventArgs eventArgs)
    {
        LogHelper.Info($"和服务器[{TargetEndPoint}]链接链接断开!");
        // 和服务器链接断开，开启重连
        _reconnectionTimer.Start();
    }

    /// <summary>
    /// 给服务器发送消息,并等待响应
    /// </summary>
    /// <param name="message">消息内容对象</param>
    public async Task<T> Call<T>(IRequestMessage message) where T : class, IResponseMessage, new()
    {
        var value = await _rpcSession.Call(message);
        return value as T;
    }

    /// <summary>
    /// 给服务器发送消息,不等待响应.
    /// </summary>
    /// <param name="networkMessage"></param>
    /// <returns>返回是否发送成功</returns>
    private bool Send(IInnerNetworkMessage networkMessage)
    {
        var buffer = _messageEncoderHandler.Handler(networkMessage);

        if (buffer == null || !IsConnected)
        {
            return false;
        }

        if (_setting.IsDebug && _setting.IsDebugSend)
        {
            LogHelper.Debug($"--发送到服务器[{TargetEndPoint}] {networkMessage.ToFormatMessageString()}");
        }

        try
        {
            return _connectClient.TrySend(buffer);
        }
        catch (Exception e)
        {
            LogHelper.Error(e);
            return false;
        }
    }

    private void ConnectToDiscoveryCenter()
    {
        if (_connectClient.IsInConnecting)
        {
            return;
        }

        _connectClient.Connect(TargetEndPoint);
    }


    /// <summary>
    /// 是否已经链接状态
    /// </summary>
    public bool IsConnected
    {
        get { return _connectClient.IsConnected; }
    }

    /// <summary>
    /// 链接到目标服务器地址
    /// </summary>
    public EndPoint TargetEndPoint { get; private set; }

    /// <summary>
    /// 开始链接
    /// </summary>
    /// <param name="serverIp">服务器IP地址</param>
    /// <param name="serverPort">服务器端口</param>
    public void Start(string serverIp, int serverPort)
    {
        if (IpHelper.IsValidIpAddress(serverIp, out var value))
        {
            TargetEndPoint = new IPEndPoint(value, serverPort);
        }
        else
        {
            TargetEndPoint = new DnsEndPoint(serverIp, serverPort);
        }

        LogHelper.Info($"开始链接到目标服务器[{TargetEndPoint}]...");
        _reconnectionTimer.Start();
    }

    /// <summary>
    /// 停止链接
    /// </summary>
    public void Stop()
    {
        _reconnectionTimer.Stop();
        _heartBeatTimer.Stop();
        _connectClient.Close();
        _rpcSession.Stop();
    }
}