using System.Net;
using GameFrameX.SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork.Kcp;

/// <summary>
/// KCP game app session wrapper / KCP 游戏应用会话包装器
/// Implements IGameAppSession interface for compatibility with the existing framework
/// </summary>
public sealed class KcpGameAppSession : IGameAppSession
{
    private readonly IKcpSession _kcpSession;
    private bool _disposed;

    /// <summary>
    /// Session unique ID / 会话唯一 ID
    /// </summary>
    public string SessionID
    {
        get { return _kcpSession.ConversationId.ToString(); }
    }

    /// <summary>
    /// Session is connected / 会话是否已连接
    /// </summary>
    public bool IsConnected
    {
        get { return _kcpSession.IsConnected && !_disposed; }
    }

    /// <summary>
    /// Remote endpoint / 远程端点
    /// </summary>
    public EndPoint RemoteEndPoint
    {
        get { return _kcpSession.RemoteEndPoint; }
    }

    /// <summary>
    /// KCP session / KCP 会话
    /// </summary>
    public IKcpSession KcpSession
    {
        get { return _kcpSession; }
    }

    /// <summary>
    /// Creates a new KCP game app session / 创建新的 KCP 游戏应用会话
    /// </summary>
    /// <param name="kcpSession">KCP session / KCP 会话</param>
    public KcpGameAppSession(IKcpSession kcpSession)
    {
        _kcpSession = kcpSession ?? throw new ArgumentNullException(nameof(kcpSession));
    }

    /// <summary>
    /// Send data to client / 发送数据到客户端
    /// </summary>
    /// <param name="data">Data to send / 要发送的数据</param>
    /// <param name="cancellationToken">Cancellation token / 取消令牌</param>
    public async ValueTask SendAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        if (_disposed || !_kcpSession.IsConnected)
        {
            return;
        }

        await _kcpSession.SendAsync(data, cancellationToken);
    }

    /// <summary>
    /// Send data to client / 发送数据到客户端
    /// </summary>
    /// <param name="data">Data to send / 要发送的数据</param>
    /// <param name="cancellationToken">Cancellation token / 取消令牌</param>
    public async ValueTask SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        if (_disposed || !_kcpSession.IsConnected)
        {
            return;
        }

        await _kcpSession.SendAsync(data, cancellationToken);
    }

    /// <summary>
    /// Close the session / 关闭会话
    /// </summary>
    public void Close()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _kcpSession.Close();
    }

    /// <summary>
    /// Dispose resources / 释放资源
    /// </summary>
    public void Dispose()
    {
        Close();
    }
}