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

using System.Diagnostics;
using System.Collections.Concurrent;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.RemoteMessaging.Contracts;
using GameFrameX.Utility.Setting;

namespace GameFrameX.NetWork.RemoteMessaging.Unified;

/// <summary>
/// 统一消息发送器默认实现。
/// 内聚玩家消息（本服/跨服/离线）和系统消息（服务发现/一致性选路）的完整链路。
/// </summary>
/// <remarks>
/// Default implementation of the unified message sender.
/// Encapsulates the complete pipeline for player messages (local/cross-server/offline)
/// and server messages (service discovery/consistent routing).
/// </remarks>
public sealed class UnifiedMessageSender : IUnifiedMessageSender
{
    private const int InstanceRefreshIntervalMs = 5000;

    private readonly IRemoteMessageClient _remoteClient;
    private readonly IPlayerRouteResolver _routeResolver;
    private readonly IPlayerLocalSender _localSender;
    private readonly IServerInstanceSelector _instanceSelector;
    private readonly MessageSendMetrics _metrics;
    private readonly ConcurrentDictionary<string, long> _lastInstanceRefreshTicks = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _currentServerType;
    private readonly int _currentServerId;

    /// <summary>
    /// 初始化统一消息发送器。
    /// </summary>
    /// <remarks>
    /// Initializes the unified message sender.
    /// </remarks>
    /// <param name="remoteClient">远程消息客户端 / Remote message client</param>
    /// <param name="routeResolver">玩家路由解析器 / Player route resolver</param>
    /// <param name="localSender">本服玩家发送器 / Local player sender</param>
    /// <param name="instanceSelector">服务实例选择器 / Server instance selector</param>
    /// <param name="metrics">发送指标聚合器；为空时自动创建 / Metrics recorder; auto-created when null</param>
    public UnifiedMessageSender(
        IRemoteMessageClient remoteClient,
        IPlayerRouteResolver routeResolver,
        IPlayerLocalSender localSender,
        IServerInstanceSelector instanceSelector,
        MessageSendMetrics metrics = null)
    {
        _remoteClient = remoteClient ?? throw new ArgumentNullException(nameof(remoteClient));
        _routeResolver = routeResolver ?? throw new ArgumentNullException(nameof(routeResolver));
        _localSender = localSender ?? throw new ArgumentNullException(nameof(localSender));
        _instanceSelector = instanceSelector ?? throw new ArgumentNullException(nameof(instanceSelector));
        _metrics = metrics ?? new MessageSendMetrics();
        _currentServerType = GlobalSettings.CurrentSetting?.ServerType ?? GameServerConst.Game.Name;
        _currentServerId = GlobalSettings.CurrentSetting?.ServerId ?? GameServerConst.Game.Id;
    }

    /// <summary>
    /// 获取统一发送器内部指标聚合器。
    /// </summary>
    public MessageSendMetrics Metrics
    {
        get { return _metrics; }
    }

    #region SendToPlayerAsync

    /// <inheritdoc />
    public async Task<PlayerSendResult> SendToPlayerAsync(
        long playerId,
        MessageObject message,
        PlayerSendOptions options = null,
        CancellationToken ct = default)
    {
        if (playerId <= 0)
        {
            return RecordPlayerResult(PlayerSendResult.Fail(PlayerDeliverStatus.Failed, playerId, "Invalid playerId"));
        }

        if (message == null)
        {
            return RecordPlayerResult(PlayerSendResult.Fail(PlayerDeliverStatus.Failed, playerId, "Message is null"));
        }

        options = options ?? new PlayerSendOptions();
        var sw = Stopwatch.StartNew();
        var traceId = Guid.NewGuid().ToString("N")[..16];

        try
        {
            // Step 1: 优先检查本服在线
            if (_localSender.IsPlayerOnline(playerId))
            {
                var sent = await _localSender.SendToLocalPlayerAsync(playerId, message);
                sw.Stop();

                if (sent)
                {
                    return RecordPlayerResult(PlayerSendResult.Ok(
                        PlayerDeliverStatus.LocalDelivered,
                        playerId,
                        sw.ElapsedMilliseconds,
                        traceId));
                }

                // 本服投递失败（可能在投递瞬间断线），继续走路由解析
            }

            // Step 2: 解析路由
            var routeInfo = await _routeResolver.ResolveAsync(playerId);

            if (routeInfo == null)
            {
                sw.Stop();
                return RecordPlayerResult(PlayerSendResult.Fail(
                    PlayerDeliverStatus.RouteMissing,
                    playerId,
                    "Route info not found",
                    sw.ElapsedMilliseconds,
                    traceId));
            }

            // Step 3: 路由指向本服但 SessionManager 未命中（路由过期）
            if (routeInfo.IsOnline && IsCurrentServer(routeInfo))
            {
                // 路由指向本服但本服投递失败，视为离线
                sw.Stop();
                return RecordPlayerResult(HandleOffline(playerId, options, sw.ElapsedMilliseconds, traceId));
            }

            // Step 4: 路由指向他服
            if (routeInfo.IsOnline)
            {
                var remoteResult = await ForwardToRemoteServerAsync(playerId, message, options, routeInfo, sw, traceId, ct);
                return RecordPlayerResult(remoteResult);
            }

            // Step 5: 路由标记为离线
            sw.Stop();
            return RecordPlayerResult(HandleOffline(playerId, options, sw.ElapsedMilliseconds, traceId));
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            return RecordPlayerResult(PlayerSendResult.Fail(
                PlayerDeliverStatus.Cancelled,
                playerId,
                "Operation cancelled",
                sw.ElapsedMilliseconds,
                traceId));
        }
        catch (Exception ex)
        {
            sw.Stop();
            return RecordPlayerResult(PlayerSendResult.Fail(
                PlayerDeliverStatus.Failed,
                playerId,
                ex.Message,
                sw.ElapsedMilliseconds,
                traceId));
        }
    }

    private async Task<PlayerSendResult> ForwardToRemoteServerAsync(
        long playerId,
        MessageObject message,
        PlayerSendOptions options,
        PlayerRouteInfo routeInfo,
        Stopwatch sw,
        string traceId,
        CancellationToken ct)
    {
        try
        {
            var context = new RemoteCallContext
            {
                ServiceName = routeInfo.ServerType,
                TimeoutMs = options.TimeoutMs,
                AllowRetry = options.AllowRetry,
                MaxRetryCount = options.MaxRetryCount,
                TraceId = traceId,
                Metadata = new Dictionary<string, string>
                {
                    { "TargetPlayerId", playerId.ToString() },
                    { "OriginalServiceName", routeInfo.ServerType },
                },
            };

            var innerRequest = new ReqSendToPlayerInner
            {
                TargetPlayerId = playerId,
                InnerMessage = message,
            };

            var result = await _remoteClient.CallWithResultAsync<RespSendToPlayerInner>(context, innerRequest);
            sw.Stop();

            if (result.IsSuccess && result.Response != null && result.Response.Success)
            {
                return PlayerSendResult.Ok(
                    PlayerDeliverStatus.RemoteDelivered,
                    playerId,
                    sw.ElapsedMilliseconds,
                    traceId,
                    routeInfo.ServerType,
                    result.RetryCount);
            }

            // 跨服转发失败，检查目标服返回的状态
            var remoteFailed = result.Response != null && !result.Response.Success && result.Response.PlayerOffline;
            if (remoteFailed)
            {
                return HandleOffline(playerId, options, sw.ElapsedMilliseconds, traceId, result.RetryCount);
            }

            return PlayerSendResult.Fail(
                PlayerDeliverStatus.Failed,
                playerId,
                result.ErrorMessage ?? "Remote delivery failed",
                sw.ElapsedMilliseconds,
                traceId,
                result.RetryCount);
        }
        catch (Exception ex)
        {
            sw.Stop();
            return PlayerSendResult.Fail(
                PlayerDeliverStatus.Failed,
                playerId,
                $"Cross-server forwarding failed: {ex.Message}",
                sw.ElapsedMilliseconds,
                traceId);
        }
    }

    private PlayerSendResult HandleOffline(long playerId, PlayerSendOptions options, long elapsedMs, string traceId, int retryCount = 0)
    {
        switch (options.OfflineStrategy)
        {
            case PlayerOfflineStrategy.StoreOffline:
                // TODO: 写入离线消息存储（P1 后续完善）
                return PlayerSendResult.Ok(
                    PlayerDeliverStatus.OfflineStored,
                    playerId,
                    elapsedMs,
                    traceId,
                    retryCount: retryCount);

            case PlayerOfflineStrategy.Discard:
                return PlayerSendResult.Fail(
                    PlayerDeliverStatus.Offline,
                    playerId,
                    "Player offline, message discarded",
                    elapsedMs,
                    traceId,
                    retryCount);

            default:
                return PlayerSendResult.Fail(
                    PlayerDeliverStatus.Offline,
                    playerId,
                    "Player offline",
                    elapsedMs,
                    traceId,
                    retryCount);
        }
    }

    private bool IsCurrentServer(PlayerRouteInfo routeInfo)
    {
        if (routeInfo == null)
        {
            return false;
        }

        if (!string.Equals(routeInfo.ServerType, _currentServerType, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return routeInfo.ServerId == _currentServerId;
    }

    #endregion

    #region SendToServerAsync

    /// <inheritdoc />
    public async Task<ServerSendResult<TResp>> SendToServerAsync<TResp>(
        string serviceName,
        MessageObject message,
        ServerSendOptions options = null,
        CancellationToken ct = default)
        where TResp : class, IResponseMessage
    {
        if (string.IsNullOrEmpty(serviceName))
        {
            return RecordServerResult(
                serviceName,
                ServerSendResult<TResp>.Fail(RemoteStatusCode.EndpointNotFound, "Service name is empty"));
        }

        if (message == null)
        {
            return RecordServerResult(
                serviceName,
                ServerSendResult<TResp>.Fail(RemoteStatusCode.UnknownError, "Message is null"));
        }

        options = options ?? new ServerSendOptions();

        RefreshInstancesIfNeeded(serviceName);

        // 实例选择
        var selection = _instanceSelector.Select(serviceName, options.RouteKey);
        var targetService = selection.HasInstance ? $"{serviceName}_{selection.InstanceId}" : serviceName;

        try
        {
            RemoteCallResult<TResp> result;

            if (options.AllowRetry)
            {
                result = await _remoteClient.CallWithRetryAsync<TResp>(
                    targetService,
                    message,
                    options.TimeoutMs,
                    options.MaxRetryCount,
                    ct);
            }
            else
            {
                result = await _remoteClient.CallWithoutRetryAsync<TResp>(
                    targetService,
                    message,
                    options.TimeoutMs,
                    ct);
            }

            return RecordServerResult(
                serviceName,
                ServerSendResult<TResp>.FromRemoteResult(result, selection.InstanceId));
        }
        catch (OperationCanceledException)
        {
            return RecordServerResult(
                serviceName,
                ServerSendResult<TResp>.Fail(RemoteStatusCode.Cancelled, "Operation cancelled"));
        }
        catch (Exception ex)
        {
            return RecordServerResult(
                serviceName,
                ServerSendResult<TResp>.Fail(RemoteStatusCode.UnknownError, ex.Message));
        }
    }

    #endregion

    #region SendToServerOneWayAsync

    /// <inheritdoc />
    public async Task<ServerSendResult> SendToServerOneWayAsync(
        string serviceName,
        MessageObject message,
        ServerSendOptions options = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(serviceName))
        {
            return RecordServerOneWayResult(serviceName, ServerSendResult.Fail("Service name is empty"));
        }

        if (message == null)
        {
            return RecordServerOneWayResult(serviceName, ServerSendResult.Fail("Message is null"));
        }

        options = options ?? new ServerSendOptions();

        RefreshInstancesIfNeeded(serviceName);

        var selection = _instanceSelector.Select(serviceName, options.RouteKey);
        var targetService = selection.HasInstance ? $"{serviceName}_{selection.InstanceId}" : serviceName;

        try
        {
            await _remoteClient.SendOneWayAsync(targetService, message, options.TimeoutMs, ct);
            return RecordServerOneWayResult(serviceName, ServerSendResult.Ok(selection.InstanceId));
        }
        catch (Exception ex)
        {
            return RecordServerOneWayResult(serviceName, ServerSendResult.Fail($"One-way send failed: {ex.Message}"));
        }
    }

    #endregion

    private void RefreshInstancesIfNeeded(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            return;
        }

        var now = Environment.TickCount64;
        var last = _lastInstanceRefreshTicks.GetOrAdd(serviceName, 0);
        if (now - last < InstanceRefreshIntervalMs)
        {
            return;
        }

        _lastInstanceRefreshTicks[serviceName] = now;
        var instanceIds = EnvironmentServiceInstanceDiscovery.DiscoverInstanceIds(serviceName);
        _instanceSelector.RefreshInstances(serviceName, instanceIds);
    }

    private PlayerSendResult RecordPlayerResult(PlayerSendResult result)
    {
        _metrics.Record(
            "player",
            result.TargetServiceName ?? _currentServerType,
            result.PlayerId,
            result.Status.ToString(),
            result.ElapsedMs,
            result.RetryCount,
            result.TraceId);
        return result;
    }

    private ServerSendResult<TResp> RecordServerResult<TResp>(string serviceName, ServerSendResult<TResp> result)
        where TResp : class, IResponseMessage
    {
        _metrics.Record(
            "server",
            serviceName ?? string.Empty,
            0,
            result.StatusCode.ToString(),
            result.ElapsedMs,
            result.RetryCount,
            result.TraceId);
        return result;
    }

    private ServerSendResult RecordServerOneWayResult(string serviceName, ServerSendResult result)
    {
        _metrics.Record(
            "server",
            serviceName ?? string.Empty,
            0,
            result.IsSuccess ? "Success" : "Failed",
            0,
            0,
            string.Empty);
        return result;
    }
}
