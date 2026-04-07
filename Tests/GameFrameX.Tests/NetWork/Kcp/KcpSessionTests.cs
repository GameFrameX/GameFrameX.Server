using System.Net;
using GameFrameX.NetWork.Kcp;
using Xunit;

namespace GameFrameX.Tests.NetWork.Kcp;

/// <summary>
/// KCP 会话单元测试
/// </summary>
public class KcpSessionTests
{
    [Fact]
    public void KcpSession_InitialState_ShouldBeConnected()
    {
        var options = new KcpOptions();
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        var sentData = new List<byte[]>();

        using var session = new KcpSession(endpoint, options, (data, ep) =>
        {
            sentData.Add(data.ToArray());
        }, 123456);

        Assert.True(session.IsConnected);
        Assert.Equal(123456u, session.ConversationId);
        Assert.Equal(endpoint, session.RemoteEndPoint);
        Assert.True((DateTime.UtcNow - session.LastActiveTime).TotalSeconds < 1);
    }

    [Fact]
    public void KcpSession_AfterClose_ShouldNotBeConnected()
    {
        var options = new KcpOptions();
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        using var session = new KcpSession(endpoint, options, (data, ep) => { }, 123456);

        session.Close();

        Assert.False(session.IsConnected);
    }

    [Fact]
    public void KcpSession_AfterDispose_ShouldNotBeConnected()
    {
        var options = new KcpOptions();
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        var session = new KcpSession(endpoint, options, (data, ep) => { }, 123456);

        session.Dispose();

        Assert.False(session.IsConnected);
    }

    [Fact]
    public void KcpSession_SendData_ShouldReturnNonNegative()
    {
        var options = new KcpOptions();
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        using var session = new KcpSession(endpoint, options, (data, ep) => { }, 123456);
        var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };

        var result = session.Send(data);

        Assert.True(result >= 0);
    }

    [Fact]
    public void KcpSession_SendAfterClose_ShouldReturnNegative()
    {
        var options = new KcpOptions();
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        using var session = new KcpSession(endpoint, options, (data, ep) => { }, 123456);
        session.Close();
        var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };

        var result = session.Send(data);

        Assert.Equal(-1, result);
    }

    [Fact]
    public void KcpSession_PeekSize_WhenEmpty_ShouldReturnZeroOrNegative()
    {
        var options = new KcpOptions();
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        using var session = new KcpSession(endpoint, options, (data, ep) => { });

        var size = session.PeekSize();

        Assert.True(size <= 0);
    }

    [Fact]
    public void KcpSession_WithoutConversationId_ShouldGenerateOne()
    {
        var options = new KcpOptions();
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);

        using var session1 = new KcpSession(endpoint, options, (data, ep) => { });
        using var session2 = new KcpSession(endpoint, options, (data, ep) => { });

        Assert.NotEqual(0u, session1.ConversationId);
        Assert.NotEqual(0u, session2.ConversationId);
    }
}
