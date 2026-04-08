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

using GameFrameX.NetWork.RemoteMessaging.Unified;
using GameFrameX.NetWork.RemoteMessaging.Contracts;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.RemoteMessaging;
using GameFrameX.NetWork.Messages;
using System.Collections;
using Xunit;

namespace GameFrameX.Tests.UnifiedMessaging;

public class ConsistentHashServerInstanceSelectorTests
{
    [Fact]
    public void Select_WithSameRouteKey_ReturnsSameInstance()
    {
        var selector = new ConsistentHashServerInstanceSelector();
        selector.RefreshInstances("TestService", new List<string> { "instance-1", "instance-2", "instance-3" });

        var result1 = selector.Select("TestService", "player-123");
        var result2 = selector.Select("TestService", "player-123");

        Assert.True(result1.HasInstance);
        Assert.True(result2.HasInstance);
        Assert.Equal(result1.InstanceId, result2.InstanceId);
    }

    [Fact]
    public void Select_DifferentRouteKeys_MayReturnDifferentInstances()
    {
        var selector = new ConsistentHashServerInstanceSelector();
        selector.RefreshInstances("TestService", new List<string> { "instance-1", "instance-2", "instance-3" });

        var results = new HashSet<string>();
        for (int i = 0; i < 100; i++)
        {
            var result = selector.Select("TestService", $"player-{i}");
            if (result.HasInstance)
            {
                results.Add(result.InstanceId);
            }
        }

        // With 100 keys and 3 instances, we should hit at least 2 different instances
        Assert.True(results.Count >= 2, $"Expected at least 2 different instances, got {results.Count}");
    }

    [Fact]
    public void Select_WithoutRouteKey_UsesRoundRobin()
    {
        var selector = new ConsistentHashServerInstanceSelector();
        selector.RefreshInstances("TestService", new List<string> { "instance-1", "instance-2", "instance-3" });

        var results = new List<string>();
        for (int i = 0; i < 6; i++)
        {
            var result = selector.Select("TestService");
            results.Add(result.InstanceId);
        }

        // Round-robin should cycle through instances
        Assert.True(results.Distinct().Count() > 1, "Round-robin should return different instances");
    }

    [Fact]
    public void Select_NoInstances_ReturnsNone()
    {
        var selector = new ConsistentHashServerInstanceSelector();

        var result = selector.Select("NonExistentService", "key1");

        Assert.False(result.HasInstance);
    }

    [Fact]
    public void RefreshInstances_RemovesService_WhenEmptyList()
    {
        var selector = new ConsistentHashServerInstanceSelector();
        selector.RefreshInstances("TestService", new List<string> { "instance-1" });

        var resultBefore = selector.Select("TestService", "key1");
        Assert.True(resultBefore.HasInstance);

        selector.RefreshInstances("TestService", new List<string>());

        var resultAfter = selector.Select("TestService", "key1");
        Assert.False(resultAfter.HasInstance);
    }

    [Fact]
    public void Select_InstanceRemoval_OnlyRemapsSubset()
    {
        var selector = new ConsistentHashServerInstanceSelector();
        var allInstances = new List<string> { "instance-1", "instance-2", "instance-3", "instance-4", "instance-5" };
        selector.RefreshInstances("TestService", allInstances);

        // Record mappings for many keys
        var originalMappings = new Dictionary<string, string>();
        for (int i = 0; i < 1000; i++)
        {
            var key = $"player-{i}";
            var result = selector.Select("TestService", key);
            originalMappings[key] = result.InstanceId;
        }

        // Remove some instances
        var remainingInstances = new List<string> { "instance-1", "instance-3", "instance-5" };
        selector.RefreshInstances("TestService", remainingInstances);

        // Count how many keys were remapped vs stayed
        var remappedCount = 0;
        var stayedCount = 0;
        foreach (var kvp in originalMappings)
        {
            var newResult = selector.Select("TestService", kvp.Key);
            if (newResult.InstanceId != kvp.Value)
            {
                remappedCount++;
            }
            else
            {
                stayedCount++;
            }
        }

        // Some keys should be remapped (those on removed instances)
        Assert.True(remappedCount > 0, $"Expected some keys to be remapped, but none were. Remapped: {remappedCount}");
        // Most keys on remaining instances should stay
        Assert.True(stayedCount > 0, $"Expected some keys to stay, but none did. Stayed: {stayedCount}");
        // Not all keys should be remapped
        Assert.True(remappedCount < originalMappings.Count, "Not all keys should be remapped");
    }

    [Fact]
    public void Select_EmptyServiceName_ReturnsNone()
    {
        var selector = new ConsistentHashServerInstanceSelector();

        var result = selector.Select("", "key1");
        Assert.False(result.HasInstance);
    }

    [Fact]
    public void Select_NullServiceName_ReturnsNone()
    {
        var selector = new ConsistentHashServerInstanceSelector();

        var result = selector.Select(null, "key1");
        Assert.False(result.HasInstance);
    }
}

public class PlayerSendResultTests
{
    [Fact]
    public void Ok_LocalDelivered_IsSuccess()
    {
        var result = PlayerSendResult.Ok(PlayerDeliverStatus.LocalDelivered, 123, 50);
        Assert.True(result.IsSuccess);
        Assert.Equal(PlayerDeliverStatus.LocalDelivered, result.Status);
        Assert.Equal(123, result.PlayerId);
        Assert.Equal(50, result.ElapsedMs);
    }

    [Fact]
    public void Ok_RemoteDelivered_IsSuccess()
    {
        var result = PlayerSendResult.Ok(PlayerDeliverStatus.RemoteDelivered, 456, 100);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Ok_OfflineStored_IsSuccess()
    {
        var result = PlayerSendResult.Ok(PlayerDeliverStatus.OfflineStored, 789, 30);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Fail_Offline_IsNotSuccess()
    {
        var result = PlayerSendResult.Fail(PlayerDeliverStatus.Offline, 123, "Player offline");
        Assert.False(result.IsSuccess);
        Assert.Equal("Player offline", result.ErrorMessage);
    }

    [Fact]
    public void Fail_Failed_IsNotSuccess()
    {
        var result = PlayerSendResult.Fail(PlayerDeliverStatus.Failed, 123, "Error");
        Assert.False(result.IsSuccess);
    }
}

public class ServerSendResultTests
{
    [Fact]
    public void Ok_ReturnsSuccess()
    {
        var result = ServerSendResult.Ok("instance-1");
        Assert.True(result.IsSuccess);
        Assert.Equal("instance-1", result.SelectedInstance);
    }

    [Fact]
    public void Fail_ReturnsFailure()
    {
        var result = ServerSendResult.Fail("Connection refused");
        Assert.False(result.IsSuccess);
        Assert.Equal("Connection refused", result.ErrorMessage);
    }
}

public class PlayerSendOptionsTests
{
    [Fact]
    public void Query_AllowsRetry()
    {
        var options = PlayerSendOptions.Query();
        Assert.True(options.AllowRetry);
    }

    [Fact]
    public void Command_DisallowsRetry()
    {
        var options = PlayerSendOptions.Command();
        Assert.False(options.AllowRetry);
    }

    [Fact]
    public void Notification_DefaultTimeout()
    {
        var options = PlayerSendOptions.Notification();
        Assert.Equal(PlayerSendOptions.DefaultTimeoutMs, options.TimeoutMs);
        Assert.False(options.AllowRetry);
    }
}

public class ServerSendOptionsTests
{
    [Fact]
    public void WithRouteKey_SetsRouteKey()
    {
        var options = ServerSendOptions.WithRouteKey("player-123");
        Assert.Equal("player-123", options.RouteKey);
    }

    [Fact]
    public void Query_AllowsRetry()
    {
        var options = ServerSendOptions.Query();
        Assert.True(options.AllowRetry);
    }

    [Fact]
    public void Command_DisallowsRetry()
    {
        var options = ServerSendOptions.Command();
        Assert.False(options.AllowRetry);
    }
}

public class MessageGovernancePolicyTests
{
    [Fact]
    public void QueryPolicy_AllowsRetry_NoIdempotencyKey()
    {
        var policy = MessageGovernancePolicy.QueryPolicy();
        Assert.Equal(MessageSemantics.Query, policy.Semantics);
        Assert.True(policy.MaxRetryCount > 0);
        Assert.False(policy.RequireIdempotencyKey);
    }

    [Fact]
    public void CommandPolicy_NoRetry_RequiresIdempotencyKey()
    {
        var policy = MessageGovernancePolicy.CommandPolicy();
        Assert.Equal(MessageSemantics.Command, policy.Semantics);
        Assert.Equal(0, policy.MaxRetryCount);
        Assert.True(policy.RequireIdempotencyKey);
    }

    [Fact]
    public void NotificationPolicy_NoRetry_NoIdempotencyKey()
    {
        var policy = MessageGovernancePolicy.NotificationPolicy();
        Assert.Equal(MessageSemantics.Notification, policy.Semantics);
        Assert.Equal(0, policy.MaxRetryCount);
        Assert.False(policy.RequireIdempotencyKey);
    }
}

public class PlayerRouteInfoTests
{
    [Fact]
    public void Online_ReturnsOnlineRoute()
    {
        var route = PlayerRouteInfo.Online("Game", 1000, 1);
        Assert.True(route.IsOnline);
        Assert.Equal("Game", route.ServerType);
        Assert.Equal(1000, route.ServerId);
        Assert.Equal(1, route.Version);
    }

    [Fact]
    public void Offline_ReturnsOfflineRoute()
    {
        var route = PlayerRouteInfo.Offline();
        Assert.False(route.IsOnline);
    }
}

public class InstanceSelectionTests
{
    [Fact]
    public void Selected_HasInstance()
    {
        var selection = InstanceSelection.Selected("TestService", "instance-1");
        Assert.True(selection.HasInstance);
        Assert.Equal("instance-1", selection.InstanceId);
        Assert.Equal("TestService", selection.ServiceName);
    }

    [Fact]
    public void None_DoesNotHaveInstance()
    {
        var selection = InstanceSelection.None("TestService");
        Assert.False(selection.HasInstance);
        Assert.Equal("TestService", selection.ServiceName);
    }
}

public class MessageSendMetricsTests
{
    [Fact]
    public void Record_AndGetSnapshot()
    {
        var metrics = new MessageSendMetrics();
        metrics.Record("player", "Game", 123, "LocalDelivered", 50, 0, "trace-1");
        metrics.Record("player", "Game", 456, "LocalDelivered", 100, 0, "trace-2");
        metrics.Record("player", "Game", 789, "Timeout", 5000, 1, "trace-3");

        var snapshots = metrics.GetSnapshots();
        Assert.Single(snapshots);

        var snapshot = snapshots[0];
        Assert.Equal("player:Game", snapshot.Key);
        Assert.Equal(3, snapshot.TotalCalls);
        Assert.Equal(2, snapshot.SuccessCalls);
        Assert.Equal(1, snapshot.TimeoutCalls);
        Assert.Equal(1, snapshot.RetryCalls);
        Assert.True(snapshot.SuccessRate > 0);
        Assert.True(snapshot.AvgElapsedMs > 0);
    }
}

public class EnvironmentServiceInstanceDiscoveryTests
{
    [Fact]
    public void DiscoverInstanceIds_WithAspireVariables_ReturnsDistinctSortedInstanceIds()
    {
        var vars = new Hashtable
        {
            ["services__social-service_2__tcp__0"] = "tcp://127.0.0.1:5002",
            ["services__social-service_1__tcp__0"] = "tcp://127.0.0.1:5001",
            ["services__social-service_1__tcp__1"] = "tcp://127.0.0.1:5101",
            ["services__social-service__tcp__0"] = "tcp://127.0.0.1:5000",
            ["services__other-service_9__tcp__0"] = "tcp://127.0.0.1:5900",
        };

        var ids = EnvironmentServiceInstanceDiscovery.DiscoverInstanceIds("social-service", vars);

        Assert.Equal(2, ids.Count);
        Assert.Equal("1", ids[0]);
        Assert.Equal("2", ids[1]);
    }

    [Fact]
    public void DiscoverInstanceIds_NoInstanceSuffix_ReturnsEmpty()
    {
        var vars = new Hashtable
        {
            ["services__social-service__tcp__0"] = "tcp://127.0.0.1:5000",
        };

        var ids = EnvironmentServiceInstanceDiscovery.DiscoverInstanceIds("social-service", vars);

        Assert.Empty(ids);
    }
}

public class UnifiedMessageSenderMetricsIntegrationTests
{
    [Fact]
    public async Task SendToPlayerAsync_LocalDelivered_RecordsMetrics()
    {
        var metrics = new MessageSendMetrics();
        var sender = new UnifiedMessageSender(
            new FakeRemoteMessageClient(),
            new FakePlayerRouteResolver(PlayerRouteInfo.Offline()),
            new FakePlayerLocalSender(isOnline: true, sendResult: true),
            new ConsistentHashServerInstanceSelector(),
            metrics);

        var result = await sender.SendToPlayerAsync(1001, new ReqSendToPlayerInner());

        Assert.True(result.IsSuccess);
        var snapshot = Assert.Single(metrics.GetSnapshots());
        Assert.Equal("player:Game", snapshot.Key);
        Assert.Equal(1, snapshot.TotalCalls);
        Assert.Equal(1, snapshot.SuccessCalls);
    }

    [Fact]
    public async Task SendToServerAsync_Success_RecordsMetrics()
    {
        var metrics = new MessageSendMetrics();
        var sender = new UnifiedMessageSender(
            new FakeRemoteMessageClient(),
            new FakePlayerRouteResolver(PlayerRouteInfo.Offline()),
            new FakePlayerLocalSender(isOnline: false, sendResult: false),
            new ConsistentHashServerInstanceSelector(),
            metrics);

        var result = await sender.SendToServerAsync<RespSendToPlayerInner>("social-service", new ReqSendToPlayerInner());

        Assert.True(result.IsSuccess);
        var snapshot = Assert.Single(metrics.GetSnapshots());
        Assert.Equal("server:social-service", snapshot.Key);
        Assert.Equal(1, snapshot.TotalCalls);
        Assert.Equal(1, snapshot.SuccessCalls);
    }
}

public class UnifiedMessagingGovernanceTests
{
    [Fact]
    public void LegacyRemoteMessageClientHolderType_ShouldNotExist()
    {
        var holderType = Type.GetType("GameFrameX.NetWork.RemoteMessaging.RemoteMessageClientHolder, GameFrameX.NetWork.RemoteMessaging");
        Assert.Null(holderType);
    }

    [Fact]
    public void HotfixBusinessModules_ShouldNotUseRemoteMessageClientHolderDirectly()
    {
        var root = LocateRepositoryRoot();
        var hotfixLogicPath = Path.Combine(root, "GameFrameX.Hotfix", "Logic");
        Assert.True(Directory.Exists(hotfixLogicPath), $"Directory not found: {hotfixLogicPath}");

        var violations = new List<string>();
        foreach (var filePath in Directory.EnumerateFiles(hotfixLogicPath, "*.cs", SearchOption.AllDirectories))
        {
            var text = File.ReadAllText(filePath);
            if (text.Contains("RemoteMessageClientHolder.Client", StringComparison.Ordinal))
            {
                violations.Add(filePath);
            }
        }

        Assert.True(
            violations.Count == 0,
            $"Found forbidden legacy remote messaging entry usage. Please migrate to UnifiedMessageSenderHolder.Sender. Files: {string.Join(", ", violations)}");
    }

    private static string LocateRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            var hotfixPath = Path.Combine(current.FullName, "GameFrameX.Hotfix");
            var messagingPath = Path.Combine(current.FullName, "GameFrameX.NetWork.RemoteMessaging");
            if (Directory.Exists(hotfixPath) && Directory.Exists(messagingPath))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException(
            $"Unable to locate repository root from base directory: {AppContext.BaseDirectory}");
    }
}

internal sealed class FakePlayerRouteResolver : IPlayerRouteResolver
{
    private readonly PlayerRouteInfo _routeInfo;

    public FakePlayerRouteResolver(PlayerRouteInfo routeInfo)
    {
        _routeInfo = routeInfo;
    }

    public Task<PlayerRouteInfo> ResolveAsync(long playerId)
    {
        return Task.FromResult(_routeInfo);
    }
}

internal sealed class FakePlayerLocalSender : IPlayerLocalSender
{
    private readonly bool _isOnline;
    private readonly bool _sendResult;

    public FakePlayerLocalSender(bool isOnline, bool sendResult)
    {
        _isOnline = isOnline;
        _sendResult = sendResult;
    }

    public bool IsPlayerOnline(long playerId)
    {
        return _isOnline;
    }

    public Task<bool> SendToLocalPlayerAsync(long playerId, MessageObject message)
    {
        return Task.FromResult(_sendResult);
    }
}

internal sealed class FakeRemoteMessageClient : IRemoteMessageClient
{
    public Task<TResponse> CallAsync<TResponse>(string serviceName, MessageObject requestMessage) where TResponse : class, GameFrameX.NetWork.Abstractions.IResponseMessage
    {
        throw new NotImplementedException();
    }

    public Task<TResponse> CallAsync<TResponse>(string serviceName, MessageObject requestMessage, CancellationToken cancellationToken) where TResponse : class, GameFrameX.NetWork.Abstractions.IResponseMessage
    {
        throw new NotImplementedException();
    }

    public Task<TResponse> CallAsync<TResponse>(string serviceName, MessageObject requestMessage, int timeoutMs, CancellationToken cancellationToken = default) where TResponse : class, GameFrameX.NetWork.Abstractions.IResponseMessage
    {
        throw new NotImplementedException();
    }

    public Task<RemoteCallResult<TResponse>> CallWithResultAsync<TResponse>(string serviceName, MessageObject requestMessage) where TResponse : class, GameFrameX.NetWork.Abstractions.IResponseMessage
    {
        return Task.FromResult(BuildSuccessResult<TResponse>());
    }

    public Task<RemoteCallResult<TResponse>> CallWithResultAsync<TResponse>(string serviceName, MessageObject requestMessage, int timeoutMs) where TResponse : class, GameFrameX.NetWork.Abstractions.IResponseMessage
    {
        return Task.FromResult(BuildSuccessResult<TResponse>());
    }

    public Task<RemoteCallResult<TResponse>> CallWithResultAsync<TResponse>(string serviceName, MessageObject requestMessage, int timeoutMs, CancellationToken cancellationToken) where TResponse : class, GameFrameX.NetWork.Abstractions.IResponseMessage
    {
        return Task.FromResult(BuildSuccessResult<TResponse>());
    }

    public Task<RemoteCallResult<TResponse>> CallWithResultAsync<TResponse>(RemoteCallContext context, MessageObject requestMessage) where TResponse : class, GameFrameX.NetWork.Abstractions.IResponseMessage
    {
        return Task.FromResult(BuildSuccessResult<TResponse>());
    }

    public Task<RemoteCallResult<TResponse>> CallWithRetryAsync<TResponse>(string serviceName, MessageObject requestMessage, int timeoutMs = RemoteCallContext.DefaultTimeoutMs, int maxRetryCount = RemoteCallContext.DefaultMaxRetryCount, CancellationToken cancellationToken = default) where TResponse : class, GameFrameX.NetWork.Abstractions.IResponseMessage
    {
        return Task.FromResult(BuildSuccessResult<TResponse>());
    }

    public Task<RemoteCallResult<TResponse>> CallWithoutRetryAsync<TResponse>(string serviceName, MessageObject requestMessage, int timeoutMs = RemoteCallContext.DefaultTimeoutMs, CancellationToken cancellationToken = default) where TResponse : class, GameFrameX.NetWork.Abstractions.IResponseMessage
    {
        return Task.FromResult(BuildSuccessResult<TResponse>());
    }

    public Task<RemoteCallResult<TResponse>> CallWithMetadataAsync<TResponse>(string serviceName, MessageObject requestMessage, Dictionary<string, string> metadata, int timeoutMs = RemoteCallContext.DefaultTimeoutMs, CancellationToken cancellationToken = default) where TResponse : class, GameFrameX.NetWork.Abstractions.IResponseMessage
    {
        return Task.FromResult(BuildSuccessResult<TResponse>());
    }

    public Task SendOneWayAsync(string serviceName, MessageObject requestMessage, int timeoutMs = RemoteCallContext.DefaultTimeoutMs, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<bool> IsServiceAvailableAsync(string serviceName)
    {
        return Task.FromResult(true);
    }

    private static RemoteCallResult<TResponse> BuildSuccessResult<TResponse>() where TResponse : class, GameFrameX.NetWork.Abstractions.IResponseMessage
    {
        if (typeof(TResponse) != typeof(RespSendToPlayerInner))
        {
            throw new NotSupportedException($"Unsupported response type: {typeof(TResponse).Name}");
        }

        var response = new RespSendToPlayerInner
        {
            Success = true,
            PlayerOffline = false,
        };

        return RemoteCallResult<TResponse>.Ok((TResponse)(object)response, elapsedMs: 5, traceId: "trace-test", retryCount: 1);
    }
}
