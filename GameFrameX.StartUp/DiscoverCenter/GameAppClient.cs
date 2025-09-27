using System.Net;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.Proto.BuiltIn;
using GameFrameX.SuperSocket.ClientEngine;
using GameFrameX.Utility;

namespace GameFrameX.StartUp.DiscoverCenter;

/// <summary>
/// 游戏程序TCP客户端类,用于处理与服务器的TCP连接和消息收发
/// </summary>
internal sealed class GameAppClient
{
    /// <summary>
    /// 连接延迟时间,单位毫秒
    /// </summary>
    public int ConnectDelay { get; }

    /// <summary>
    /// 内部TCP会话实例，负责底层网络通信
    /// </summary>
    private readonly AsyncTcpSession _mTcpClient;

    /// <summary>
    /// 当前重连次数计数器
    /// </summary>
    private int _mRetryCount;

    /// <summary>
    /// 客户端事件回调集合，用于通知外部连接、断开、消息等事件
    /// </summary>
    private readonly GameAppClientEvent _mGameAppClientEvent;

    /// <summary>
    /// 服务器终结点（IP与端口）
    /// </summary>
    private readonly EndPoint _serverHost;

    /// <summary>
    /// 心跳包发送间隔，单位毫秒
    /// </summary>
    private readonly int _heartBeatInterval;

    /// <summary>
    /// 每次重连之间的延迟时间，单位毫秒
    /// </summary>
    private readonly int _retryDelay;

    /// <summary>
    /// 最大重连次数，-1表示无限重试
    /// </summary>
    private readonly int _maxRetryCount;

    /// <summary>
    /// 心跳请求消息实例，复用对象避免频繁创建
    /// </summary>
    private readonly ReqActorHeartBeat _reqActorHeartBeat;

    /// <summary>
    /// 标记当前实例是否已被释放，防止重复释放或空操作
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// 初始化游戏TCP客户端
    /// </summary>
    /// <param name="clientEvent">客户端事件回调结构体，包含连接、断开、消息等事件的处理委托</param>
    /// <param name="endPoint">服务器端点信息（IP和端口）</param>
    /// <param name="heartBeatInterval">心跳包发送间隔（毫秒），默认5000</param>
    /// <param name="connectDelay">连接尝试间隔（毫秒），默认5000</param>
    /// <param name="retryDelay">重连延迟时间（毫秒），默认5000</param>
    /// <param name="maxRetryCount">最大重连次数，-1表示无限重试</param>
    public GameAppClient(GameAppClientEvent clientEvent, EndPoint endPoint, int heartBeatInterval = 5000, int connectDelay = 5000, int retryDelay = 5000, int maxRetryCount = -1)
    {
        ConnectDelay = connectDelay;
        _mGameAppClientEvent = clientEvent;
        _serverHost = endPoint;
        _heartBeatInterval = heartBeatInterval;
        _retryDelay = retryDelay;
        _maxRetryCount = maxRetryCount;
        _reqActorHeartBeat = new ReqActorHeartBeat
        {
            Timestamp = TimeHelper.UnixTimeMilliseconds(),
        };
        _mTcpClient = new AsyncTcpSession();
        _mTcpClient.Connected += OnClientOnConnected;
        _mTcpClient.Closed += OnClientOnClosed;
        _mTcpClient.DataReceived += OnClientOnDataReceived;
        _mTcpClient.Error += OnClientOnError;
    }

    /// <summary>
    /// 停止客户端连接，关闭底层TCP会话
    /// </summary>
    public void Stop()
    {
        _isDisposed = true;
        _mTcpClient.Close();
    }

    /// <summary>
    /// 启动客户端并尝试连接服务器，处理消息编解码和压缩解压缩处理器的初始化。
    /// 内部采用无限循环，在连接成功前持续重试，并在连接成功后周期性发送心跳。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    public async Task EntryAsync()
    {
        // 主循环：负责连接、重连与心跳
        while (true)
        {
            if (_isDisposed)
            {
                break;
            }

            // 如果未连接且未处于连接中，则尝试连接
            if (!_mTcpClient.IsConnected && !_mTcpClient.IsInConnecting)
            {
                LogHelper.Info("尝试连接到发现中心服务器...");
                _mTcpClient.Connect(_serverHost);
                await Task.Delay(ConnectDelay);

                // 若连接成功或正在连接，则跳过本次循环
                if (_mTcpClient.IsConnected || _mTcpClient.IsInConnecting)
                {
                    continue;
                }

                // 未达到最大重连次数（或无限重试）则进行重连
                if (_mRetryCount < _maxRetryCount || _maxRetryCount < 0)
                {
                    LogHelper.Info($"未连接到发现中心服务器, 尝试重连 (尝试次数: {_mRetryCount + 1}/{(_maxRetryCount < 0 ? "∞" : _maxRetryCount.ToString())})...");
                    _mTcpClient.Connect(_serverHost);
                    _mRetryCount++;
                    await Task.Delay(_retryDelay);
                }
                else
                {
                    LogHelper.Info("重连次数已达到上限，停止尝试。");
                    break;
                }
            }
            else
            {
                // 连接成功，重置重连计数
                _mRetryCount = 0;
                // 发送心跳
                SendHeartBeat();
                // 等待下一次心跳间隔
                await Task.Delay(_heartBeatInterval);
            }
        }
    }

    /// <summary>
    /// 发送心跳包到服务器
    /// 更新心跳时间戳后通过SendToServer发送
    /// </summary>
    private void SendHeartBeat()
    {
        _reqActorHeartBeat.Timestamp = TimeHelper.UnixTimeMilliseconds();
        SendToServer(_reqActorHeartBeat);
    }

    /// <summary>
    /// 发送消息到服务器
    /// 内部使用MessageHelper编码后通过TcpClient发送
    /// </summary>
    /// <param name="messageObject">要发送的消息对象</param>
    public void SendToServer(MessageObject messageObject)
    {
        var buffer = MessageHelper.EncoderHandler.Handler(messageObject);
        if (buffer != null)
        {
            _mTcpClient.Send(buffer);
        }
    }

    /// <summary>
    /// 处理客户端错误事件
    /// 将错误信息通过GameAppClientEvent回调给上层
    /// </summary>
    /// <param name="client">触发事件的TcpSession对象</param>
    /// <param name="e">包含异常信息的错误事件参数</param>
    private void OnClientOnError(object client, SuperSocket.ClientEngine.ErrorEventArgs e)
    {
        _mGameAppClientEvent.OnError?.Invoke(e);
    }

    /// <summary>
    /// 处理客户端连接关闭事件
    /// 记录日志并通过GameAppClientEvent通知上层连接已断开
    /// </summary>
    /// <param name="client">触发事件的TcpSession对象</param>
    /// <param name="e">事件参数</param>
    private void OnClientOnClosed(object client, EventArgs e)
    {
        LogHelper.Info("客户端断开连接");
        _mGameAppClientEvent.OnClosed?.Invoke();
    }

    /// <summary>
    /// 处理客户端连接成功事件
    /// 记录日志并通过GameAppClientEvent通知上层连接已建立
    /// </summary>
    /// <param name="client">触发事件的TcpSession对象</param>
    /// <param name="e">事件参数</param>
    private void OnClientOnConnected(object client, EventArgs e)
    {
        LogHelper.Info("客户端成功连接到服务器");
        _mGameAppClientEvent.OnConnected?.Invoke();
    }

    /// <summary>
    /// 处理接收到数据事件
    /// 将接收到的二进制数据解码为消息对象，若为内部网络消息则反序列化后通过回调通知上层
    /// </summary>
    /// <param name="client">触发事件的TcpSession对象</param>
    /// <param name="e">包含接收数据的数据事件参数</param>
    private void OnClientOnDataReceived(object client, DataEventArgs e)
    {
        var message = MessageHelper.DecoderHandler.Handler(e.Data.ReadBytesValue(e.Offset, e.Length));

        if (message is InnerNetworkMessage innerNetworkMessage)
        {
            _mGameAppClientEvent.OnMessage?.Invoke((MessageObject)innerNetworkMessage.DeserializeMessageObject());
        }
    }
}