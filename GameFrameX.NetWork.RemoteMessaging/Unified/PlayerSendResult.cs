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

namespace GameFrameX.NetWork.RemoteMessaging.Unified;

/// <summary>
/// 玩家消息投递状态。
/// </summary>
/// <remarks>
/// Player message delivery status.
/// </remarks>
public enum PlayerDeliverStatus
{
    /// <summary>
    /// 本服投递成功
    /// </summary>
    /// <remarks>
    /// Delivered successfully on local server.
    /// </remarks>
    LocalDelivered = 0,

    /// <summary>
    /// 跨服转发成功
    /// </summary>
    /// <remarks>
    /// Forwarded successfully to remote server.
    /// </remarks>
    RemoteDelivered = 1,

    /// <summary>
    /// 离线存储成功
    /// </summary>
    /// <remarks>
    /// Stored in offline message storage.
    /// </remarks>
    OfflineStored = 2,

    /// <summary>
    /// 玩家离线（未存储）
    /// </summary>
    /// <remarks>
    /// Player is offline (not stored).
    /// </remarks>
    Offline = 3,

    /// <summary>
    /// 投递失败
    /// </summary>
    /// <remarks>
    /// Delivery failed.
    /// </remarks>
    Failed = 4,

    /// <summary>
    /// 路由信息缺失
    /// </summary>
    /// <remarks>
    /// Route information is missing.
    /// </remarks>
    RouteMissing = 5,

    /// <summary>
    /// 超时
    /// </summary>
    /// <remarks>
    /// Timed out.
    /// </remarks>
    Timeout = 6,

    /// <summary>
    /// 已取消
    /// </summary>
    /// <remarks>
    /// Cancelled.
    /// </remarks>
    Cancelled = 7,
}

/// <summary>
/// 玩家消息发送结果。
/// </summary>
/// <remarks>
/// Player message send result.
/// </remarks>
public sealed class PlayerSendResult
{
    /// <summary>
    /// 投递状态
    /// </summary>
    /// <remarks>
    /// Delivery status.
    /// </remarks>
    public PlayerDeliverStatus Status { get; init; }

    /// <summary>
    /// 目标玩家ID
    /// </summary>
    /// <remarks>
    /// Target player ID.
    /// </remarks>
    public long PlayerId { get; init; }

    /// <summary>
    /// 错误描述
    /// </summary>
    /// <remarks>
    /// Error description.
    /// </remarks>
    public string ErrorMessage { get; init; }

    /// <summary>
    /// 调用耗时毫秒数
    /// </summary>
    /// <remarks>
    /// Elapsed time in milliseconds.
    /// </remarks>
    public long ElapsedMs { get; init; }

    /// <summary>
    /// 追踪ID
    /// </summary>
    /// <remarks>
    /// Trace ID for distributed tracing.
    /// </remarks>
    public string TraceId { get; init; }

    /// <summary>
    /// 重试次数
    /// </summary>
    /// <remarks>
    /// Number of retries attempted.
    /// </remarks>
    public int RetryCount { get; init; }

    /// <summary>
    /// 目标服务名（跨服时有值）
    /// </summary>
    /// <remarks>
    /// Target service name (has value for cross-server delivery).
    /// </remarks>
    public string TargetServiceName { get; init; }

    /// <summary>
    /// 是否投递成功
    /// </summary>
    /// <remarks>
    /// Indicates whether delivery was successful.
    /// </remarks>
    public bool IsSuccess
    {
        get { return Status == PlayerDeliverStatus.LocalDelivered || Status == PlayerDeliverStatus.RemoteDelivered || Status == PlayerDeliverStatus.OfflineStored; }
    }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    /// <remarks>
    /// Creates a successful result.
    /// </remarks>
    /// <param name="status">投递状态 / Delivery status</param>
    /// <param name="playerId">玩家ID / Player ID</param>
    /// <param name="elapsedMs">耗时毫秒数 / Elapsed time in milliseconds</param>
    /// <param name="traceId">追踪ID / Trace ID</param>
    /// <param name="targetServiceName">目标服务名 / Target service name</param>
    /// <returns>成功的发送结果 / A successful send result</returns>
    public static PlayerSendResult Ok(PlayerDeliverStatus status, long playerId, long elapsedMs, string traceId = null, string targetServiceName = null, int retryCount = 0)
    {
        return new PlayerSendResult
        {
            Status = status,
            PlayerId = playerId,
            ElapsedMs = elapsedMs,
            TraceId = traceId,
            TargetServiceName = targetServiceName,
            RetryCount = retryCount,
        };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    /// <remarks>
    /// Creates a failure result.
    /// </remarks>
    /// <param name="status">投递状态 / Delivery status</param>
    /// <param name="playerId">玩家ID / Player ID</param>
    /// <param name="errorMessage">错误描述 / Error description</param>
    /// <param name="elapsedMs">耗时毫秒数 / Elapsed time in milliseconds</param>
    /// <param name="traceId">追踪ID / Trace ID</param>
    /// <returns>失败的发送结果 / A failed send result</returns>
    public static PlayerSendResult Fail(PlayerDeliverStatus status, long playerId, string errorMessage, long elapsedMs = 0, string traceId = null, int retryCount = 0)
    {
        return new PlayerSendResult
        {
            Status = status,
            PlayerId = playerId,
            ErrorMessage = errorMessage,
            ElapsedMs = elapsedMs,
            TraceId = traceId,
            RetryCount = retryCount,
        };
    }
}
