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
using System.Net.Sockets;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Kcp;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.Utility.Setting;
using Xunit;

namespace GameFrameX.Tests.NetWork.Kcp;

/// <summary>
/// KCP 服务器集成测试 / KCP server integration tests
/// </summary>
public class KcpServerIntegrationTests : IAsyncLifetime
{
    private readonly int _testPort;
    private KcpServer _server;
    private readonly List<IGameAppSession> _connectedSessions = new();
    private readonly List<IMessage> _receivedMessages = new();
    private readonly AppSetting _setting;

    public KcpServerIntegrationTests()
    {
        // 获取可用端口 / Get available port
        _testPort = GetAvailablePort();
        _setting = new AppSetting();
    }

    public async Task InitializeAsync()
    {
        var options = new KcpOptions
        {
            Enable = true,
            SessionTimeout = 30,
            ConnectionTimeout = 5000
        };

        _server = new KcpServer(
            _testPort,
            options,
            _setting,
            OnPackageReceived,
            OnConnected,
            OnDisconnected
        );

        await _server.StartAsync();
    }

    public Task DisposeAsync()
    {
        _server?.Dispose();
        return Task.CompletedTask;
    }

    private ValueTask OnConnected(EndPoint endpoint)
    {
        return ValueTask.CompletedTask;
    }

    private ValueTask OnDisconnected(EndPoint endpoint)
    {
        return ValueTask.CompletedTask;
    }

    private void OnPackageReceived(IGameAppSession session, IMessage message)
    {
        lock (_receivedMessages)
        {
            _receivedMessages.Add(message);
        }
    }

    private static int GetAvailablePort()
    {
        using var listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        return ((IPEndPoint)listener.LocalEndPoint).Port;
    }

    #region 服务器启动/停止测试 / Server Start/Stop Tests

    /// <summary>
    /// 测试服务器启动 / Test server startup
    /// </summary>
    [Fact]
    public void KcpServer_AfterStart_ShouldBeRunning()
    {
        // Assert
        Assert.True(_server.IsRunning);
        Assert.Equal(_testPort, _server.Port);
    }

    /// <summary>
    /// 测试服务器停止 / Test server stop
    /// </summary>
    [Fact]
    public void KcpServer_AfterStop_ShouldNotBeRunning()
    {
        // Act
        _server.Stop();

        // Assert
        Assert.False(_server.IsRunning);
    }

    #endregion

    #region UDP Socket 测试 / UDP Socket Tests

    /// <summary>
    /// 测试 UDP 数据包发送 / Test UDP packet sending
    /// </summary>
    [Fact]
    public async Task KcpServer_ReceiveUdpPacket_ShouldProcess()
    {
        // Arrange
        using var client = new UdpClient();
        var serverEndpoint = new IPEndPoint(IPAddress.Loopback, _testPort);

        // 发送简单的测试数据 / Send simple test data
        var testData = new byte[] { 0x00, 0x00, 0x00, 0x01, 0xFF };
        await client.SendAsync(testData, serverEndpoint);

        // 等待处理 / Wait for processing
        await Task.Delay(100);

        // Assert - 数据被接收（即使无法解析为有效消息）
        // 验证服务器仍在运行 / Verify server is still running
        Assert.True(_server.IsRunning);
    }

    /// <summary>
    /// 测试会话管理器存在 / Test session manager exists
    /// </summary>
    [Fact]
    public void KcpServer_SessionManager_ShouldExist()
    {
        // Assert
        Assert.NotNull(_server.SessionManager);
    }

    #endregion

    #region 会话测试 / Session Tests

    /// <summary>
    /// 测试获取不存在的通道 / Test getting non-existing channel
    /// </summary>
    [Fact]
    public void KcpServer_TryGetChannel_NonExisting_ShouldReturnFalse()
    {
        // Act
        var result = _server.TryGetChannel(999999, out var channel);

        // Assert
        Assert.False(result);
        Assert.Null(channel);
    }

    #endregion

    #region 边界条件测试 / Boundary Condition Tests

    /// <summary>
    /// 测试处理空数据 / Test handling empty data
    /// </summary>
    [Fact]
    public async Task KcpServer_ReceiveEmptyPacket_ShouldNotCrash()
    {
        // Arrange
        using var client = new UdpClient();
        var serverEndpoint = new IPEndPoint(IPAddress.Loopback, _testPort);

        // Act - 发送空数据 / Send empty data
        await client.SendAsync(Array.Empty<byte>(), serverEndpoint);
        await Task.Delay(100);

        // Assert - 服务器应该仍在运行
        Assert.True(_server.IsRunning);
    }

    /// <summary>
    /// 测试处理短数据 / Test handling short data
    /// </summary>
    [Fact]
    public async Task KcpServer_ReceiveShortPacket_ShouldNotCrash()
    {
        // Arrange
        using var client = new UdpClient();
        var serverEndpoint = new IPEndPoint(IPAddress.Loopback, _testPort);

        // Act - 发送少于4字节的数据 / Send data less than 4 bytes
        var shortData = new byte[] { 0x01, 0x02 };
        await client.SendAsync(shortData, serverEndpoint);
        await Task.Delay(100);

        // Assert - 服务器应该仍在运行
        Assert.True(_server.IsRunning);
    }

    #endregion
}

/// <summary>
/// KCP 会话单元测试 / KCP session unit tests
/// </summary>
public class KcpSessionTests
{
    #region 会话属性测试 / Session Property Tests

    /// <summary>
    /// 测试会话初始状态 / Test session initial state
    /// </summary>
    [Fact]
    public void KcpSession_InitialState_ShouldBeConnected()
    {
        // Arrange
        var options = new KcpOptions();
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        var sentData = new List<byte[]>();

        // Act
        using var session = new KcpSession(endpoint, options, (data, ep) =>
        {
            sentData.Add(data.ToArray());
        }, 123456);

        // Assert
        Assert.True(session.IsConnected);
        Assert.Equal(123456u, session.ConversationId);
        Assert.Equal(endpoint, session.RemoteEndPoint);
        Assert.True((DateTime.UtcNow - session.LastActiveTime).TotalSeconds < 1);
    }

    /// <summary>
    /// 测试会话关闭 / Test session close
    /// </summary>
    [Fact]
    public void KcpSession_AfterClose_ShouldNotBeConnected()
    {
        // Arrange
        var options = new KcpOptions();
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        using var session = new KcpSession(endpoint, options, (data, ep) => { }, 123456);

        // Act
        session.Close();

        // Assert
        Assert.False(session.IsConnected);
    }

    /// <summary>
    /// 测试会话释放 / Test session dispose
    /// </summary>
    [Fact]
    public void KcpSession_AfterDispose_ShouldNotBeConnected()
    {
        // Arrange
        var options = new KcpOptions();
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        var session = new KcpSession(endpoint, options, (data, ep) => { }, 123456);

        // Act
        session.Dispose();

        // Assert
        Assert.False(session.IsConnected);
    }

    #endregion

    #region 数据操作测试 / Data Operation Tests

    /// <summary>
    /// 测试发送数据 / Test sending data
    /// </summary>
    [Fact]
    public void KcpSession_SendData_ShouldReturnNonNegative()
    {
        // Arrange
        var options = new KcpOptions();
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        using var session = new KcpSession(endpoint, options, (data, ep) => { }, 123456);
        var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };

        // Act
        var result = session.Send(data);

        // Assert
        Assert.True(result >= 0);
    }

    /// <summary>
    /// 测试关闭后发送数据 / Test sending data after close
    /// </summary>
    [Fact]
    public void KcpSession_SendAfterClose_ShouldReturnNegative()
    {
        // Arrange
        var options = new KcpOptions();
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        using var session = new KcpSession(endpoint, options, (data, ep) => { }, 123456);
        session.Close();
        var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };

        // Act
        var result = session.Send(data);

        // Assert
        Assert.Equal(-1, result);
    }

    /// <summary>
    /// 测试 PeekSize / Test PeekSize
    /// </summary>
    [Fact]
    public void KcpSession_PeekSize_WhenEmpty_ShouldReturnZeroOrNegative()
    {
        // Arrange
        var options = new KcpOptions();
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        using var session = new KcpSession(endpoint, options, (data, ep) => { }, 123456);

        // Act
        var size = session.PeekSize();

        // Assert - 没有数据时应该返回0或负数
        Assert.True(size <= 0);
    }

    #endregion

    #region 自动会话 ID 测试 / Auto Conversation ID Tests

    /// <summary>
    /// 测试自动生成会话 ID / Test auto-generated conversation ID
    /// </summary>
    [Fact]
    public void KcpSession_WithoutConversationId_ShouldGenerateOne()
    {
        // Arrange
        var options = new KcpOptions();
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);

        // Act
        using var session1 = new KcpSession(endpoint, options, (data, ep) => { });
        using var session2 = new KcpSession(endpoint, options, (data, ep) => { });

        // Assert
        Assert.NotEqual(0u, session1.ConversationId);
        Assert.NotEqual(0u, session2.ConversationId);
        // 会话 ID 应该不同（虽然有可能碰撞，但概率很低）
    }

    #endregion
}
