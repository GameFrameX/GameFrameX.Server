// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


using System.Collections.Concurrent;
using System.Net;

namespace GameFrameX.NetWork.Kcp;

/// <summary>
/// KCP session manager / KCP 会话管理器
/// </summary>
public sealed class KcpSessionManager : IDisposable
{
    private readonly Timer _cleanupTimer;
    private readonly ConcurrentDictionary<EndPoint, IKcpSession> _endpointToSession = new();
    private readonly KcpOptions _options;
    private readonly Action<ReadOnlyMemory<byte>, EndPoint> _sendOutput;
    private readonly ConcurrentDictionary<uint, IKcpSession> _sessions = new();
    private bool _disposed;

    /// <summary>
    /// Creates a new KCP session manager / 创建新的 KCP 会话管理器
    /// </summary>
    /// <param name="options">KCP options / KCP 配置选项</param>
    /// <param name="sendOutput">Send output callback / 发送输出回调</param>
    public KcpSessionManager(KcpOptions options, Action<ReadOnlyMemory<byte>, EndPoint> sendOutput)
    {
        _options = options;
        _sendOutput = sendOutput;
        _cleanupTimer = new Timer(OnCleanup, null, TimeSpan.FromSeconds(_options.SessionTimeout / 2), TimeSpan.FromSeconds(_options.SessionTimeout / 2));
    }

    /// <summary>
    /// Gets the number of active sessions / 获取活跃会话数量
    /// </summary>
    public int SessionCount
    {
        get { return _sessions.Count; }
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

        _cleanupTimer?.Dispose();

        foreach (var session in _sessions.Values)
        {
            session.Close();
            (session as IDisposable)?.Dispose();
        }

        _sessions.Clear();
        _endpointToSession.Clear();
    }

    /// <summary>
    /// Get or create a session for an endpoint / 获取或创建端点的会话
    /// </summary>
    /// <param name="endPoint">Remote endpoint / 远程端点</param>
    /// <returns>KCP session / KCP 会话</returns>
    public IKcpSession GetOrCreateSession(EndPoint endPoint)
    {
        if (_endpointToSession.TryGetValue(endPoint, out var existingSession))
        {
            return existingSession;
        }

        var conversationId = (uint)Random.Shared.Next();
        var session = new KcpSession(endPoint, _options, _sendOutput, conversationId);

        var addedSession = _endpointToSession.GetOrAdd(endPoint, session);
        if (addedSession == session)
        {
            _sessions[session.ConversationId] = session;
        }

        return addedSession;
    }

    /// <summary>
    /// Get or create a session with specific conversation ID / 获取或创建指定会话 ID 的会话
    /// </summary>
    /// <param name="endPoint">Remote endpoint / 远程端点</param>
    /// <param name="conversationId">Conversation ID / 会话 ID</param>
    /// <returns>KCP session / KCP 会话</returns>
    public IKcpSession GetOrCreateSession(EndPoint endPoint, uint conversationId)
    {
        if (_sessions.TryGetValue(conversationId, out var existingSession))
        {
            return existingSession;
        }

        var session = new KcpSession(endPoint, _options, _sendOutput, conversationId);

        var addedSession = _sessions.GetOrAdd(conversationId, session);
        if (addedSession == session)
        {
            _endpointToSession[endPoint] = session;
        }

        return addedSession;
    }

    /// <summary>
    /// Get session by conversation ID / 根据会话 ID 获取会话
    /// </summary>
    /// <param name="conversationId">Conversation ID / 会话 ID</param>
    /// <param name="session">KCP session / KCP 会话</param>
    /// <returns>True if session exists / 如果会话存在则返回 true</returns>
    public bool TryGetSession(uint conversationId, out IKcpSession session)
    {
        return _sessions.TryGetValue(conversationId, out session);
    }

    /// <summary>
    /// Get session by endpoint / 根据端点获取会话
    /// </summary>
    /// <param name="endPoint">Remote endpoint / 远程端点</param>
    /// <param name="session">KCP session / KCP 会话</param>
    /// <returns>True if session exists / 如果会话存在则返回 true</returns>
    public bool TryGetSession(EndPoint endPoint, out IKcpSession session)
    {
        return _endpointToSession.TryGetValue(endPoint, out session);
    }

    /// <summary>
    /// Remove a session / 移除会话
    /// </summary>
    /// <param name="conversationId">Conversation ID / 会话 ID</param>
    public void RemoveSession(uint conversationId)
    {
        if (!_sessions.TryRemove(conversationId, out var session))
        {
            return;
        }

        _endpointToSession.TryRemove(session.RemoteEndPoint, out _);
        session.Close();
        (session as IDisposable)?.Dispose();
    }

    /// <summary>
    /// Remove a session by endpoint / 根据端点移除会话
    /// </summary>
    /// <param name="endPoint">Remote endpoint / 远程端点</param>
    public void RemoveSession(EndPoint endPoint)
    {
        if (_endpointToSession.TryRemove(endPoint, out var session))
        {
            _sessions.TryRemove(session.ConversationId, out _);
            session.Close();
            (session as IDisposable)?.Dispose();
        }
    }

    /// <summary>
    /// Get all active sessions / 获取所有活跃会话
    /// </summary>
    /// <returns>Collection of active sessions / 活跃会话集合</returns>
    public IReadOnlyCollection<IKcpSession> GetAllSessions()
    {
        return _sessions.Values.ToList();
    }

    private void OnCleanup(object state)
    {
        if (_disposed)
        {
            return;
        }

        var now = DateTime.UtcNow;
        var timeout = TimeSpan.FromSeconds(_options.SessionTimeout);

        foreach (var session in _sessions.Values)
        {
            if (!session.IsConnected || now - session.LastActiveTime > timeout)
            {
                RemoveSession(session.ConversationId);
            }
        }
    }
}