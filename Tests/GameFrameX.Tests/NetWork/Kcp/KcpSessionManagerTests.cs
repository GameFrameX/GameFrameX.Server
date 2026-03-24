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
using GameFrameX.NetWork.Kcp;
using Xunit;

namespace GameFrameX.Tests.NetWork.Kcp;

/// <summary>
/// KcpSessionManager 单元测试 / KcpSessionManager unit tests
/// </summary>
public class KcpSessionManagerTests : IDisposable
{
    private readonly KcpOptions _options;
    private readonly KcpSessionManager _manager;
    private readonly List<ReadOnlyMemory<byte>> _sentPackets = new();

    public KcpSessionManagerTests()
    {
        _options = new KcpOptions
        {
            SessionTimeout = 10, // 短超时便于测试
            UpdatePeriod = 5
        };
        _manager = new KcpSessionManager(_options, (data, endpoint) =>
        {
            _sentPackets.Add(data.ToArray());
        });
    }

    public void Dispose()
    {
        _manager.Dispose();
    }

    #region GetOrCreateSession 测试 / GetOrCreateSession Tests

    /// <summary>
    /// 测试通过端点创建会话 / Test creating session by endpoint
    /// </summary>
    [Fact]
    public void GetOrCreateSession_ByEndpoint_ShouldCreateNewSession()
    {
        // Arrange
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);

        // Act
        var session = _manager.GetOrCreateSession(endpoint);

        // Assert
        Assert.NotNull(session);
        Assert.Equal(endpoint, session.RemoteEndPoint);
        Assert.True(session.IsConnected);
        Assert.Equal(1, _manager.SessionCount);
    }

    /// <summary>
    /// 测试重复获取相同端点的会话 / Test getting session for same endpoint twice
    /// </summary>
    [Fact]
    public void GetOrCreateSession_SameEndpoint_ShouldReturnSameSession()
    {
        // Arrange
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);

        // Act
        var session1 = _manager.GetOrCreateSession(endpoint);
        var session2 = _manager.GetOrCreateSession(endpoint);

        // Assert
        Assert.Same(session1, session2);
        Assert.Equal(1, _manager.SessionCount);
    }

    /// <summary>
    /// 测试通过会话 ID 创建会话 / Test creating session by conversation ID
    /// </summary>
    [Fact]
    public void GetOrCreateSession_ByConversationId_ShouldCreateSessionWithSpecifiedId()
    {
        // Arrange
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        const uint conversationId = 123456;

        // Act
        var session = _manager.GetOrCreateSession(endpoint, conversationId);

        // Assert
        Assert.NotNull(session);
        Assert.Equal(conversationId, session.ConversationId);
        Assert.Equal(endpoint, session.RemoteEndPoint);
    }

    /// <summary>
    /// 测试创建多个会话 / Test creating multiple sessions
    /// </summary>
    [Fact]
    public void GetOrCreateSession_MultipleEndpoints_ShouldCreateMultipleSessions()
    {
        // Arrange
        var endpoints = new[]
        {
            new IPEndPoint(IPAddress.Loopback, 12345),
            new IPEndPoint(IPAddress.Loopback, 12346),
            new IPEndPoint(IPAddress.Loopback, 12347)
        };

        // Act
        foreach (var endpoint in endpoints)
        {
            _manager.GetOrCreateSession(endpoint);
        }

        // Assert
        Assert.Equal(3, _manager.SessionCount);
    }

    #endregion

    #region TryGetSession 测试 / TryGetSession Tests

    /// <summary>
    /// 测试通过会话 ID 获取存在的会话 / Test getting existing session by conversation ID
    /// </summary>
    [Fact]
    public void TryGetSession_ExistingConversationId_ShouldReturnTrue()
    {
        // Arrange
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        const uint conversationId = 123456;
        var createdSession = _manager.GetOrCreateSession(endpoint, conversationId);

        // Act
        var result = _manager.TryGetSession(conversationId, out var session);

        // Assert
        Assert.True(result);
        Assert.NotNull(session);
        Assert.Same(createdSession, session);
    }

    /// <summary>
    /// 测试通过不存在的会话 ID 获取会话 / Test getting non-existing session by conversation ID
    /// </summary>
    [Fact]
    public void TryGetSession_NonExistingConversationId_ShouldReturnFalse()
    {
        // Act
        var result = _manager.TryGetSession(999999, out var session);

        // Assert
        Assert.False(result);
        Assert.Null(session);
    }

    /// <summary>
    /// 测试通过端点获取存在的会话 / Test getting existing session by endpoint
    /// </summary>
    [Fact]
    public void TryGetSession_ExistingEndpoint_ShouldReturnTrue()
    {
        // Arrange
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        var createdSession = _manager.GetOrCreateSession(endpoint);

        // Act
        var result = _manager.TryGetSession(endpoint, out var session);

        // Assert
        Assert.True(result);
        Assert.NotNull(session);
        Assert.Same(createdSession, session);
    }

    /// <summary>
    /// 测试通过不存在的端点获取会话 / Test getting non-existing session by endpoint
    /// </summary>
    [Fact]
    public void TryGetSession_NonExistingEndpoint_ShouldReturnFalse()
    {
        // Arrange
        var endpoint = new IPEndPoint(IPAddress.Loopback, 59999);

        // Act
        var result = _manager.TryGetSession(endpoint, out var session);

        // Assert
        Assert.False(result);
        Assert.Null(session);
    }

    #endregion

    #region RemoveSession 测试 / RemoveSession Tests

    /// <summary>
    /// 测试通过会话 ID 移除会话 / Test removing session by conversation ID
    /// </summary>
    [Fact]
    public void RemoveSession_ByConversationId_ShouldRemoveSession()
    {
        // Arrange
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        const uint conversationId = 123456;
        _manager.GetOrCreateSession(endpoint, conversationId);

        // Act
        _manager.RemoveSession(conversationId);

        // Assert
        Assert.Equal(0, _manager.SessionCount);
        Assert.False(_manager.TryGetSession(conversationId, out _));
        Assert.False(_manager.TryGetSession(endpoint, out _));
    }

    /// <summary>
    /// 测试通过端点移除会话 / Test removing session by endpoint
    /// </summary>
    [Fact]
    public void RemoveSession_ByEndpoint_ShouldRemoveSession()
    {
        // Arrange
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        var session = _manager.GetOrCreateSession(endpoint);

        // Act
        _manager.RemoveSession(endpoint);

        // Assert
        Assert.Equal(0, _manager.SessionCount);
        Assert.False(_manager.TryGetSession(endpoint, out _));
        Assert.False(_manager.TryGetSession(session.ConversationId, out _));
    }

    /// <summary>
    /// 测试移除不存在的会话 / Test removing non-existing session
    /// </summary>
    [Fact]
    public void RemoveSession_NonExisting_ShouldNotThrow()
    {
        // Act & Assert - 不应抛出异常
        var exception = Record.Exception(() => _manager.RemoveSession(999999));
        Assert.Null(exception);
    }

    #endregion

    #region GetAllSessions 测试 / GetAllSessions Tests

    /// <summary>
    /// 测试获取所有会话 / Test getting all sessions
    /// </summary>
    [Fact]
    public void GetAllSessions_WithMultipleSessions_ShouldReturnAll()
    {
        // Arrange
        var endpoints = new[]
        {
            new IPEndPoint(IPAddress.Loopback, 12345),
            new IPEndPoint(IPAddress.Loopback, 12346),
            new IPEndPoint(IPAddress.Loopback, 12347)
        };

        foreach (var endpoint in endpoints)
        {
            _manager.GetOrCreateSession(endpoint);
        }

        // Act
        var sessions = _manager.GetAllSessions();

        // Assert
        Assert.Equal(3, sessions.Count);
    }

    /// <summary>
    /// 测试空管理器获取所有会话 / Test getting all sessions from empty manager
    /// </summary>
    [Fact]
    public void GetAllSessions_EmptyManager_ShouldReturnEmpty()
    {
        // Act
        var sessions = _manager.GetAllSessions();

        // Assert
        Assert.Empty(sessions);
    }

    #endregion

    #region Dispose 测试 / Dispose Tests

    /// <summary>
    /// 测试释放资源 / Test disposing resources
    /// </summary>
    [Fact]
    public void Dispose_WithSessions_ShouldClearAllSessions()
    {
        // Arrange
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        _manager.GetOrCreateSession(endpoint);

        // Act
        _manager.Dispose();

        // Assert
        Assert.Equal(0, _manager.SessionCount);
    }

    #endregion
}
