using System.Buffers;
using System.Net;
using GameFrameX.SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork.Kcp;

/// <summary>
/// KCP session implementation / KCP 会话实现
/// </summary>
public sealed class KcpSession : IKcpSession, System.Net.Sockets.Kcp.IKcpCallback, IDisposable
{
    private readonly System.Net.Sockets.Kcp.PoolSegManager.Kcp _kcp;
    private readonly EndPoint _remoteEndPoint;
    private readonly KcpOptions _options;
    private readonly Action<ReadOnlyMemory<byte>, EndPoint> _sendOutput;
    private readonly object _updateLock = new();
    private readonly Timer _updateTimer;
    private bool _disposed;
    private bool _isConnected = true;
    private DateTime _lastActiveTime;
    private readonly uint _conversationId;

    /// <summary>
    /// Conversation ID / 会话 ID
    /// </summary>
    public uint ConversationId
    {
        get { return _conversationId; }
    }


    /// <summary>
    /// Remote endpoint / 远程端点
    /// </summary>
    public EndPoint RemoteEndPoint
    {
        get { return _remoteEndPoint; }
    }

    public ValueTask SendAsync(byte[] data, CancellationToken cancellationToken = new CancellationToken())
    {
        ArgumentNullException.ThrowIfNull(data);
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled(cancellationToken);
        }

        return SendAsync((ReadOnlyMemory<byte>)data, cancellationToken);
    }

    public ValueTask SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = new CancellationToken())
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled(cancellationToken);
        }

        Send(data.Span);
        return ValueTask.CompletedTask;
    }

    public int Send(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        return Send((ReadOnlySpan<byte>)data);
    }

    public int Send(ReadOnlySpan<byte> data)
    {
        if (_disposed || !_isConnected)
        {
            return -1;
        }

        lock (_updateLock)
        {
            var sendResult = _kcp.Send(data, null);
            if (sendResult > 0)
            {
                _sendOutput?.Invoke(data.ToArray(), _remoteEndPoint);
            }

            return sendResult;
        }
    }

    public string SessionID { get; }

    /// <summary>
    /// Is connection active / 连接是否活跃
    /// </summary>
    public bool IsConnected
    {
        get { return _isConnected && !_disposed; }
    }

    /// <summary>
    /// Last active time / 最后活跃时间
    /// </summary>
    public DateTime LastActiveTime
    {
        get { return _lastActiveTime; }
    }

    /// <summary>
    /// Creates a new KCP session / 创建新的 KCP 会话
    /// </summary>
    public KcpSession(EndPoint remoteEndPoint, KcpOptions options, Action<ReadOnlyMemory<byte>, EndPoint> sendOutput, uint conversationId = 0)
    {
        _remoteEndPoint = remoteEndPoint;
        _options = options;
        _sendOutput = sendOutput;
        _conversationId = conversationId == 0 ? (uint)Random.Shared.Next() : conversationId;
        _lastActiveTime = DateTime.UtcNow;

        // Create KCP instance using PoolSegManager.Kcp with IKcpCallback
        _kcp = new System.Net.Sockets.Kcp.PoolSegManager.Kcp(_conversationId, this, null);
        ConfigureKcp();

        // Start update timer
        _updateTimer = new Timer(OnUpdate, null, _options.UpdatePeriod, _options.UpdatePeriod);
    }

    private void ConfigureKcp()
    {
        // Configure NoDelay mode
        lock (_updateLock)
        {
            _kcp.NoDelay(
                nodelay_: _options.NoDelay ? 1 : 0,
                interval_: _options.Interval,
                resend_: _options.Resend,
                nc_: _options.EnableFlowControl ? 0 : 1
            );
            // Configure window size
            _kcp.WndSize(_options.SendWindow, _options.ReceiveWindow);

            // Configure MTU
            _kcp.SetMtu(_options.Mtu);
        }
    }

    /// <summary>
    /// IKcpCallback.Output - Called when KCP needs to send data / 当 KCP 需要发送数据时调用
    /// </summary>
    public void Output(IMemoryOwner<byte> buffer, int avalidLength)
    {
        if (_disposed || !_isConnected)
        {
            buffer.Dispose();
            return;
        }

        try
        {
            var memory = buffer.Memory.Slice(0, avalidLength);
            _sendOutput?.Invoke(memory, _remoteEndPoint);
        }
        finally
        {
            buffer.Dispose();
        }
    }

    private void OnUpdate(object state)
    {
        if (_disposed || !_isConnected)
        {
            return;
        }

        // Check timeout
        var inactiveTime = (DateTime.UtcNow - _lastActiveTime).TotalMilliseconds;
        if (inactiveTime > _options.ConnectionTimeout)
        {
            _isConnected = false;
            return;
        }

        // Update must be called in single thread
        lock (_updateLock)
        {
            _kcp.Update(DateTimeOffset.UtcNow);
        }
    }

    /// <summary>
    /// Input data received from UDP / 输入从 UDP 接收的数据
    /// </summary>
    public void Input(ReadOnlySpan<byte> data)
    {
        if (_disposed || !_isConnected)
        {
            return;
        }

        _lastActiveTime = DateTime.UtcNow;
        lock (_updateLock)
        {
            _kcp.Input(data);
        }
    }

    /// <summary>
    /// Receive data from KCP / 从 KCP 接收数据
    /// </summary>
    public int Recv(Span<byte> buffer)
    {
        if (_disposed || !_isConnected)
        {
            return -1;
        }

        // Recv must be called in single thread
        lock (_updateLock)
        {
            return _kcp.Recv(buffer);
        }
    }

    /// <summary>
    /// Try peek receive size / 尝试获取接收大小
    /// </summary>
    public int PeekSize()
    {
        lock (_updateLock)
        {
            return _kcp.PeekSize();
        }
    }

    /// <summary>
    /// Close connection / 关闭连接
    /// </summary>
    public void Close()
    {
        _isConnected = false;
        _updateTimer?.Dispose();
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
        Close();
        _kcp.Dispose();
    }
}
