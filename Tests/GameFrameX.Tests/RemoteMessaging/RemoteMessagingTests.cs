// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   均受中华人民共和国及相关国际法律法规保护。
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   侵犯他人合法权益等法律法规所禁止的行为！
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   本项目组织与贡献者概不承担。
//   GitHub 仓库：https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using GameFrameX.NetWork.RemoteMessaging;
using GameFrameX.NetWork.RemoteMessaging.Contracts;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Tests.RemoteMessaging;

/// <summary>
/// RemoteMessageClientHolder 单元测试
/// </summary>
public class RemoteMessageClientHolderTests
{
    [Fact]
    public void Client_ShouldReturnNonNullInstance()
    {
        var client = RemoteMessageClientHolder.Client;
        Assert.NotNull(client);
    }

    [Fact]
    public void Client_ShouldReturnSameInstance()
    {
        var client1 = RemoteMessageClientHolder.Client;
        var client2 = RemoteMessageClientHolder.Client;
        Assert.Same(client1, client2);
    }

    [Fact]
    public void Client_ShouldImplementIRemoteMessageClient()
    {
        var client = RemoteMessageClientHolder.Client;
        Assert.IsAssignableFrom<IRemoteMessageClient>(client);
    }
}

/// <summary>
/// IRemoteMessageClient 调用测试
/// </summary>
public class RemoteMessageClientCallTests
{
    [Fact]
    public async Task CallAsync_ShouldReturnNull_WhenEndpointNotResolved()
    {
        var client = RemoteMessageClientHolder.Client;
        var result = await client.CallAsync<TestResponseMessage>(
            "NonExistentService_12345",
            new TestMessageObject(),
            1000);
        Assert.Null(result);
    }

    [Fact]
    public async Task CallWithResultAsync_ShouldReturnEndpointNotFound_WhenServiceNotResolved()
    {
        var client = RemoteMessageClientHolder.Client;
        var context = RemoteCallContext.Create("NonExistentService_12345", 1000);
        var result = await client.CallWithResultAsync<TestResponseMessage>(context, new TestMessageObject());
        Assert.False(result.IsSuccess);
        Assert.Equal(RemoteStatusCode.EndpointNotFound, result.StatusCode);
    }
}

/// <summary>
/// RemoteCallContext 单元测试
/// </summary>
public class RemoteCallContextTests
{
    [Fact]
    public void Create_ShouldSetDefaults()
    {
        var context = RemoteCallContext.Create("TestService");
        Assert.Equal("TestService", context.ServiceName);
        Assert.Equal(RemoteCallContext.DefaultTimeoutMs, context.TimeoutMs);
        Assert.False(context.AllowRetry);
        Assert.Equal(CancellationToken.None, context.CancellationToken);
    }

    [Fact]
    public void Create_ShouldAcceptCustomTimeout()
    {
        var context = RemoteCallContext.Create("TestService", 3000);
        Assert.Equal(3000, context.TimeoutMs);
    }

    [Fact]
    public void AllowRetry_ShouldDefaultFalse()
    {
        var context = new RemoteCallContext { ServiceName = "Test" };
        Assert.False(context.AllowRetry);
    }
}

/// <summary>
/// RemoteCallResult 单元测试
/// </summary>
public class RemoteCallResultTests
{
    [Fact]
    public void Ok_ShouldCreateSuccessResult()
    {
        var response = new TestResponseMessage();
        var result = RemoteCallResult<TestResponseMessage>.Ok(response, 100, "trace123");
        Assert.True(result.IsSuccess);
        Assert.Same(response, result.Response);
        Assert.Equal(100, result.ElapsedMs);
        Assert.Equal("trace123", result.TraceId);
        Assert.Equal(RemoteStatusCode.Success, result.StatusCode);
    }

    [Fact]
    public void Fail_ShouldCreateFailedResult()
    {
        var result = RemoteCallResult<TestResponseMessage>.Fail(RemoteStatusCode.Timeout, "timed out", 5000, "trace456");
        Assert.False(result.IsSuccess);
        Assert.Null(result.Response);
        Assert.Equal(RemoteStatusCode.Timeout, result.StatusCode);
        Assert.Equal("timed out", result.ErrorMessage);
    }

    [Fact]
    public void Ok_ShouldTrackRetryCount()
    {
        var result = RemoteCallResult<TestResponseMessage>.Ok(new TestResponseMessage(), 100, retryCount: 2);
        Assert.Equal(2, result.RetryCount);
    }
}

/// <summary>
/// RemoteStatusCode 单元测试
/// </summary>
public class RemoteStatusCodeTests
{
    [Fact]
    public void AllStatusCodes_ShouldBeDistinct()
    {
        var values = Enum.GetValues<RemoteStatusCode>();
        var set = new HashSet<int>();
        foreach (var value in values)
        {
            Assert.True(set.Add((int)value), $"Duplicate status code value: {value}");
        }
    }
}

/// <summary>
/// TestMessageObject 用于测试的消息对象
/// </summary>
public class TestMessageObject : MessageObject
{
    public override void Clear()
    {
    }
}

/// <summary>
/// TestResponseMessage 用于测试的响应消息对象
/// </summary>
public class TestResponseMessage : MessageObject, IResponseMessage
{
    public int ErrorCode { get; set; }

    public override void Clear()
    {
    }
}
