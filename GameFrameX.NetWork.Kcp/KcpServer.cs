using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.Utility.Setting;
using GameFrameX.NetWork;

namespace GameFrameX.NetWork.Kcp;

/// <summary>
/// KCP server / KCP 服务器
/// </summary>
public sealed class KcpServer : IDisposable
{
    private readonly KcpOptions _options;
    private readonly AppSetting _setting;
    private readonly KcpSessionManager _sessionManager;
    private readonly KcpMessagePipelineFilter _messageFilter;
    private readonly Socket _udpSocket;
    private readonly byte[] _receiveBuffer;
    private readonly int _port;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly Action<IGameAppSession, IMessage> _packageHandler;
    private readonly Func<EndPoint, ValueTask> _onConnected;
    private readonly Func<EndPoint, ValueTask> _onDisconnected;
    private readonly ConcurrentDictionary<uint, KcpNetWorkChannel> _channels = new();
    private bool _disposed;
    private bool _isRunning;

    /// <summary>
    /// Gets the session manager / 获取会话管理器
    /// </summary>
    public KcpSessionManager SessionManager
    {
        get { return _sessionManager; }
    }

    /// <summary>
    /// Gets whether the server is running / 获取服务器是否正在运行
    /// </summary>
    public bool IsRunning
    {
        get { return _isRunning; }
    }

    /// <summary>
    /// Gets the listening port / 获取监听端口
    /// </summary>
    public int Port
    {
        get { return _port; }
    }

    /// <summary>
    /// Creates a new KCP server / 创建新的 KCP 服务器
    /// </summary>
    /// <param name="port">Listening port / 监听端口</param>
    /// <param name="options">KCP options / KCP 配置选项</param>
    /// <param name="setting">Application settings / 应用配置</param>
    /// <param name="packageHandler">Message handler / 消息处理器</param>
    /// <param name="onConnected">Connected callback / 连接回调</param>
    /// <param name="onDisconnected">Disconnected callback / 断开回调</param>
    public KcpServer(
        int port,
        KcpOptions options,
        AppSetting setting,
        Action<IGameAppSession, IMessage> packageHandler,
        Func<EndPoint, ValueTask> onConnected,
        Func<EndPoint, ValueTask> onDisconnected)
    {
        _port = port;
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
        _packageHandler = packageHandler ?? throw new ArgumentNullException(nameof(packageHandler));
        _onConnected = onConnected;
        _onDisconnected = onDisconnected;
        _receiveBuffer = new byte[options.Mtu * 2];
        _messageFilter = new KcpMessagePipelineFilter();

        _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            ReceiveBufferSize = 1024 * 1024,
            SendBufferSize = 1024 * 1024
        };

        _sessionManager = new KcpSessionManager(_options, SendOutput);
    }

    /// <summary>
    /// Start the KCP server / 启动 KCP 服务器
    /// </summary>
    public async Task StartAsync()
    {
        if (_isRunning)
        {
            return;
        }

        _udpSocket.Bind(new IPEndPoint(IPAddress.Any, _port));
        _isRunning = true;

        LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.KcpServer.StartupComplete, "KCP", "0.0.0.0", _port));

        _ = Task.Run(ReceiveLoop, _cancellationTokenSource.Token);
    }

    /// <summary>
    /// Stop the KCP server / 停止 KCP 服务器
    /// </summary>
    public void Stop()
    {
        if (!_isRunning)
        {
            return;
        }

        _isRunning = false;
        _cancellationTokenSource.Cancel();
        _udpSocket.Close();

        foreach (var channel in _channels.Values)
        {
            channel.Close();
        }

        _channels.Clear();
        _sessionManager.Dispose();

        LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.KcpServer.ClientDisconnected, "Server stopped"));
    }

    private async Task ReceiveLoop()
    {
        var endpoint = new IPEndPoint(IPAddress.Any, 0) as EndPoint;

        while (!_cancellationTokenSource.Token.IsCancellationRequested && _isRunning)
        {
            try
            {
                var result = await _udpSocket.ReceiveFromAsync(_receiveBuffer, endpoint);
                if (result.ReceivedBytes > 0)
                {
                    ProcessReceivedData(result.RemoteEndPoint, _receiveBuffer.AsSpan(0, result.ReceivedBytes));
                }
            }
            catch (SocketException)
            {
                // Socket closed or error, exit loop
                break;
            }
            catch (ObjectDisposedException)
            {
                // Socket disposed, exit loop
                break;
            }
            catch (Exception ex)
            {
                LogHelper.Error<string>("KCP receive error: {message}", ex.Message);
            }
        }
    }

    private void ProcessReceivedData(EndPoint remoteEndPoint, ReadOnlySpan<byte> data)
    {
        if (data.Length < 4)
        {
            return;
        }

        // Try to get conversation ID from the first 4 bytes (KCP header)
        var conversationId = BitConverter.ToUInt32(data.Slice(0, 4));
        if (BitConverter.IsLittleEndian)
        {
            conversationId = (conversationId >> 24) | ((conversationId >> 8) & 0xFF00) | ((conversationId << 8) & 0xFF0000) | (conversationId << 24);
        }

        IKcpSession session;
        bool isNewSession = false;
        if (_sessionManager.TryGetSession(conversationId, out var existingSession))
        {
            session = existingSession;
        }
        else
        {
            session = _sessionManager.GetOrCreateSession(remoteEndPoint, conversationId);
            isNewSession = true;
        }

        // Input data to KCP session
        session.Input(data);

        // Process received messages
        ProcessSessionMessages(session, isNewSession, remoteEndPoint);
    }

    private void ProcessSessionMessages(IKcpSession session, bool isNewSession, EndPoint remoteEndPoint)
    {
        if (isNewSession)
        {
            _ = _onConnected?.Invoke(remoteEndPoint);
        }

        var messages = _messageFilter.Filter(session);
        foreach (var message in messages)
        {
            if (!_channels.TryGetValue(session.ConversationId, out var channel))
            {
                channel = new KcpNetWorkChannel(session, _setting);
                _channels[session.ConversationId] = channel;
            }

            _packageHandler(channel.GameAppSession, message);
        }
    }

    private void SendOutput(ReadOnlyMemory<byte> data, EndPoint remoteEndPoint)
    {
        if (!_isRunning || _disposed)
        {
            return;
        }

        try
        {
            _udpSocket.SendTo(data.Span, SocketFlags.None, remoteEndPoint);
        }
        catch (Exception ex)
        {
            LogHelper.Error<string>("KCP send error: {message}", ex.Message);
        }
    }

    /// <summary>
    /// Send data to a specific session / 发送数据到指定会话
    /// </summary>
    /// <param name="conversationId">Conversation ID / 会话 ID</param>
    /// <param name="data">Data to send / 要发送的数据</param>
    public int Send(uint conversationId, ReadOnlySpan<byte> data)
    {
        if (_sessionManager.TryGetSession(conversationId, out var session))
        {
            return session.Send(data);
        }

        return -1;
    }

    /// <summary>
    /// Send data to a specific endpoint / 发送数据到指定端点
    /// </summary>
    /// <param name="endPoint">Remote endpoint / 远程端点</param>
    /// <param name="data">Data to send / 要发送的数据</param>
    public int Send(EndPoint endPoint, ReadOnlySpan<byte> data)
    {
        if (_sessionManager.TryGetSession(endPoint, out var session))
        {
            return session.Send(data);
        }

        return -1;
    }

    /// <summary>
    /// Get channel by conversation ID / 根据会话 ID 获取通道
    /// </summary>
    /// <param name="conversationId">Conversation ID / 会话 ID</param>
    /// <param name="channel">Network channel / 网络通道</param>
    /// <returns>True if channel exists / 如果通道存在则返回 true</returns>
    public bool TryGetChannel(uint conversationId, out KcpNetWorkChannel channel)
    {
        return _channels.TryGetValue(conversationId, out channel);
    }

    /// <summary>
    /// Dispose resources / 释放资源
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        Stop();
        _cancellationTokenSource.Dispose();
    }
}