// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using System.Net;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.SuperSocket.ClientEngine;

namespace GameFrameX.StartUp.ServiceClient;

/// <summary>
/// Game application service client for connecting to game servers
/// </summary>
/// <remarks>
/// 游戏应用服务客户端，用于连接游戏服务器
/// </remarks>
internal sealed class GameAppServiceClient : IDisposable
{
    /// <summary>
    /// Internal TCP session instance responsible for underlying network communication
    /// </summary>
    /// <remarks>
    /// 内部TCP会话实例，负责底层网络通信
    /// </remarks>
    private readonly AsyncTcpSession _mTcpClient;

    /// <summary>
    /// Gets the current retry count for connection attempts
    /// </summary>
    /// <remarks>
    /// 获取当前重连次数计数器
    /// </remarks>
    /// <value>
    /// Returns the number of retry attempts made for connection
    /// </value>
    public int RetryCount { get; private set; }

    /// <summary>
    /// RPC session instance for handling RPC requests and responses
    /// </summary>
    /// <remarks>
    /// RPC会话实例，用于处理RPC请求和响应
    /// </remarks>
    private readonly IRpcSession _rpcSession;

    /// <summary>
    /// Server endpoint (IP and port)
    /// </summary>
    /// <remarks>
    /// 服务器终结点（IP与端口）
    /// </remarks>
    private readonly EndPoint _serverHost;

    /// <summary>
    /// Gets the server endpoint (IP and port)
    /// </summary>
    /// <remarks>
    /// 获取服务器终结点（IP与端口）
    /// </remarks>
    /// <value>
    /// Returns the server endpoint information including IP address and port number
    /// </value>
    public EndPoint ServerHost
    {
        get { return _serverHost; }
    }

    /// <summary>
    /// Game application service configuration
    /// </summary>
    /// <remarks>
    /// 游戏应用服务配置
    /// </remarks>
    private readonly GameAppServiceConfiguration _configuration;

    /// <summary>
    /// Flag indicating whether the current instance has been disposed to prevent duplicate disposal or null operations
    /// </summary>
    /// <remarks>
    /// 标记当前实例是否已被释放，防止重复释放或空操作
    /// </remarks>
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the GameAppServiceClient class
    /// </summary>
    /// <remarks>
    /// 初始化GameAppServiceClient类的新实例
    /// </remarks>
    /// <param name="endPoint">The server endpoint to connect to / 要连接的服务器终结点</param>
    /// <param name="configuration">The client configuration settings / 客户端配置设置</param>
    /// <exception cref="ArgumentNullException">Thrown when endPoint is null / 当endPoint为null时抛出</exception>
    /// <exception cref="ArgumentNullException">Thrown when configuration is null / 当configuration为null时抛出</exception>
    public GameAppServiceClient(EndPoint endPoint, GameAppServiceConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(endPoint, nameof(endPoint));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        _serverHost = endPoint;
        _rpcSession = new RpcSession();
        _configuration = configuration;
        _mTcpClient = new AsyncTcpSession();
        _mTcpClient.Connected += OnClientOnConnected;
        _mTcpClient.Closed += OnClientOnClosed;
        _mTcpClient.DataReceived += OnClientOnDataReceived;
        _mTcpClient.Error += OnClientOnError;
        Task.Run(Handler);
        Task.Run(StartAsync);
    }

    private async Task Handler()
    {
        DateTime lastTickTime = DateTime.UtcNow;
        while (_isDisposed == false)
        {
            await Task.Delay(1);
            var deltaTime = DateTime.UtcNow - lastTickTime;
            _rpcSession.Tick((int)deltaTime.TotalMilliseconds);

            while (true)
            {
                var sessionData = _rpcSession.Handler();

                if (sessionData != null)
                {
                    MessageSendHandle(sessionData?.RequestMessage);
                }
                else
                {
                    break;
                }
            }

            lastTickTime = DateTime.UtcNow;
        }
    }


    /// <summary>
    /// 启动客户端并尝试连接服务器，处理消息编解码和压缩解压缩处理器的初始化。
    /// 内部采用无限循环，在连接成功前持续重试，并在连接成功后周期性发送心跳。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    private async Task StartAsync()
    {
        // 主循环：负责连接、重连
        while (true)
        {
            if (_isDisposed)
            {
                break;
            }

            // 如果未连接且未处于连接中，则尝试连接
            if (!_mTcpClient.IsConnected && !_mTcpClient.IsInConnecting)
            {
                LogHelper.Debug($"try to connect to the target server...{_serverHost}");
                _mTcpClient.Connect(_serverHost);
                if (_configuration.IsEnableConnectDelay)
                {
                    // 添加连接延迟，避免瞬间发起多个连接请求
                    await Task.Delay(_configuration.ConnectDelay);
                }

                // 若连接成功或正在连接，则跳过本次循环
                if (_mTcpClient.IsConnected || _mTcpClient.IsInConnecting)
                {
                    continue;
                }

                // 未达到最大重连次数（或无限重试）则进行重连
                if (RetryCount < _configuration.MaxRetryCount || _configuration.MaxRetryCount < 0)
                {
                    LogHelper.Info($"Not connecting to the target server, attempts to reconnect (number of attempts: {RetryCount + 1}/{(_configuration.MaxRetryCount < 0 ? "∞" : _configuration.MaxRetryCount.ToString())})...");
                    _mTcpClient.Connect(_serverHost);
                    RetryCount++;
                    await Task.Delay(_configuration.RetryDelay);
                }
                else
                {
                    LogHelper.Info($"Reconnect attempts have reached the upper limit ({_configuration.MaxRetryCount}), and no more attempts will be made.");
                    break;
                }
            }
            else
            {
                // 连接成功，重置重连计数
                RetryCount = 0;
                if (_configuration.IsEnableHeartBeat)
                {
                    // 发送心跳
                    SendHeartBeat();
                    // 等待下一次心跳间隔
                    await Task.Delay(_configuration.HeartBeatInterval);
                }
            }
        }
    }

    /// <summary>
    /// Sends a heartbeat message to the server
    /// </summary>
    /// <remarks>
    /// 发送心跳包到服务器，更新心跳时间戳后通过Send发送
    /// </remarks>
    private void SendHeartBeat()
    {
        var messageObject = _configuration.OnHeartBeat?.Invoke();
        if (messageObject != null)
        {
            Send(messageObject);
        }
    }

    /// <summary>
    /// Sends a message to the server
    /// </summary>
    /// <remarks>
    /// 发送消息到服务器，内部使用MessageHelper编码后通过TcpClient发送
    /// </remarks>
    /// <param name="messageObject">The message object to send / 要发送的消息对象</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when messageObject.MessageId is greater than or equal to 0 / 当messageObject.MessageId大于等于0时抛出</exception>
    public void Send(MessageObject messageObject)
    {
        if (messageObject.MessageId >= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(messageObject), "messageObject.MessageId must be less than 0");
        }

        _rpcSession.Send(messageObject as IRequestMessage);
    }

    /// <summary>
    /// Handles message sending to the server internally
    /// </summary>
    /// <remarks>
    /// 内部处理发送消息到服务器，使用MessageHelper编码后通过TcpClient发送
    /// </remarks>
    /// <param name="messageObject">The message object to send / 要发送的消息对象</param>
    private void MessageSendHandle(INetworkMessage messageObject)
    {
        var buffer = MessageHelper.EncoderHandler.Handler(messageObject);
        if (buffer != null)
        {
            _mTcpClient.Send(buffer);
        }
    }

    /// <summary>
    /// Sends a message to the server and waits for a response
    /// </summary>
    /// <remarks>
    /// 发送消息到服务器并等待响应，内部使用MessageHelper编码后通过TcpClient发送
    /// </remarks>
    /// <param name="messageObject">The message object to send / 要发送的消息对象</param>
    /// <param name="timeOut">Timeout in milliseconds / 超时时间，单位毫秒</param>
    /// <typeparam name="T">Response message type that must implement IResponseMessage interface / 响应消息类型，必须实现IResponseMessage接口</typeparam>
    /// <returns>A task representing the asynchronous operation with IRpcResult as result / 表示异步操作的任务，任务结果为IRpcResult对象</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when messageObject.MessageId is greater than or equal to 0 / 当messageObject.MessageId大于等于0时抛出</exception>
    public Task<IRpcResult> Call<T>(MessageObject messageObject, int timeOut = 10000) where T : IResponseMessage, new()
    {
        if (messageObject.MessageId >= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(messageObject), "messageObject.MessageId must be less than 0");
        }

        var result = _rpcSession.Call<T>(messageObject as IRequestMessage, timeOut);
        return result;
    }

    /// <summary>
    /// Handles client error events
    /// </summary>
    /// <remarks>
    /// 处理客户端错误事件，将错误信息通过GameAppClientEvent回调给上层
    /// </remarks>
    /// <param name="client">The TcpSession object that triggered the event / 触发事件的TcpSession对象</param>
    /// <param name="e">Error event arguments containing exception information / 包含异常信息的错误事件参数</param>
    private void OnClientOnError(object client, SuperSocket.ClientEngine.ErrorEventArgs e)
    {
        LogHelper.Error($"Client error occurred: {e.Exception.Message}");
        _configuration.OnError?.Invoke(e);
    }

    /// <summary>
    /// Handles client connection closed events
    /// </summary>
    /// <remarks>
    /// 处理客户端连接关闭事件，记录日志并通过GameAppClientEvent通知上层连接已断开
    /// </remarks>
    /// <param name="client">The TcpSession object that triggered the event / 触发事件的TcpSession对象</param>
    /// <param name="e">Event arguments / 事件参数</param>
    private void OnClientOnClosed(object client, EventArgs e)
    {
        LogHelper.Info($"Client disconnected from the server: {_serverHost}");
        _configuration.OnClosed?.Invoke();
    }

    /// <summary>
    /// Handles client connection success events
    /// </summary>
    /// <remarks>
    /// 处理客户端连接成功事件，记录日志并通过GameAppClientEvent通知上层连接已建立
    /// </remarks>
    /// <param name="client">The TcpSession object that triggered the event / 触发事件的TcpSession对象</param>
    /// <param name="e">Event arguments / 事件参数</param>
    private void OnClientOnConnected(object client, EventArgs e)
    {
        LogHelper.Info($"Client successfully connected to the server: {_serverHost}");
        _configuration.OnConnected?.Invoke();
    }

    /// <summary>
    /// Handles data received events
    /// </summary>
    /// <remarks>
    /// 处理接收到数据事件，将接收到的二进制数据解码为消息对象，若为内部网络消息则反序列化后通过回调通知上层
    /// </remarks>
    /// <param name="client">The TcpSession object that triggered the event / 触发事件的TcpSession对象</param>
    /// <param name="e">Data event arguments containing received data / 包含接收数据的数据事件参数</param>
    private void OnClientOnDataReceived(object client, DataEventArgs e)
    {
        var message = MessageHelper.DecoderHandler.Handler(e.Data.ReadBytesValue(e.Offset, e.Length));
        // 只处理内部消息
        if (message is NetworkMessagePackage innerNetworkMessage && innerNetworkMessage.Header.MessageId < 0)
        {
            var messageObject = (MessageObject)innerNetworkMessage.DeserializeMessageObject();
            var reply = _rpcSession.Reply(messageObject as IResponseMessage);
            if (!reply)
            {
                _configuration.OnMessage?.Invoke(messageObject);
            }
        }
    }

    /// <summary>
    /// Stops the client and releases resources
    /// </summary>
    /// <remarks>
    /// 停止客户端并释放资源
    /// </remarks>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        Stop();
    }

    /// <summary>
    /// Stops the client connection and closes the underlying TCP session
    /// </summary>
    /// <remarks>
    /// 停止客户端连接，关闭底层TCP会话
    /// </remarks>
    public async void Stop()
    {
        _isDisposed = true;
        await Task.Delay(5);
        _mTcpClient.Close();
        _rpcSession.Stop();
    }
}