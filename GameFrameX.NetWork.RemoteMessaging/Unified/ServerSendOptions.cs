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
/// 系统消息发送选项。控制超时、重试、一致性选路等行为。
/// </summary>
/// <remarks>
/// Server message send options. Controls timeout, retry, consistent routing and other behaviors.
/// </remarks>
public sealed class ServerSendOptions
{
    /// <summary>
    /// 默认超时毫秒数
    /// </summary>
    /// <remarks>
    /// Default timeout in milliseconds.
    /// </remarks>
    public const int DefaultTimeoutMs = 5000;

    /// <summary>
    /// 超时毫秒数
    /// </summary>
    /// <remarks>
    /// Timeout duration in milliseconds.
    /// </remarks>
    public int TimeoutMs { get; set; } = DefaultTimeoutMs;

    /// <summary>
    /// 是否允许重试（仅幂等消息可设为 true）
    /// </summary>
    /// <remarks>
    /// Whether retry is allowed (should only be set to true for idempotent messages).
    /// </remarks>
    public bool AllowRetry { get; set; }

    /// <summary>
    /// 最大重试次数（当 AllowRetry 为 true 时生效）
    /// </summary>
    /// <remarks>
    /// Maximum retry count (effective when AllowRetry is true).
    /// </remarks>
    public int MaxRetryCount { get; set; } = 2;

    /// <summary>
    /// 一致性哈希路由键。相同 routeKey 在拓扑稳定时持续命中同一实例。
    /// 为 null 时使用默认轮询或随机策略。
    /// </summary>
    /// <remarks>
    /// Consistent hash routing key. The same routeKey consistently hits the same instance when topology is stable.
    /// When null, default round-robin or random strategy is used.
    /// </remarks>
    public string RouteKey { get; set; }

    /// <summary>
    /// 是否要求幂等键（命令类消息应设为 true）
    /// </summary>
    /// <remarks>
    /// Whether an idempotency key is required (should be set to true for command messages).
    /// </remarks>
    public bool RequireIdempotencyKey { get; set; }

    /// <summary>
    /// 幂等键（当 RequireIdempotencyKey 为 true 时必须提供）
    /// </summary>
    /// <remarks>
    /// Idempotency key (must be provided when RequireIdempotencyKey is true).
    /// </remarks>
    public string IdempotencyKey { get; set; }

    /// <summary>
    /// 创建幂等查询选项（允许重试）。适用于读操作和查询接口。
    /// </summary>
    /// <remarks>
    /// Creates idempotent query options (retry allowed). Suitable for read operations and query interfaces.
    /// </remarks>
    /// <param name="timeoutMs">超时毫秒数 / Timeout in milliseconds</param>
    /// <param name="maxRetryCount">最大重试次数 / Maximum retry count</param>
    /// <returns>允许重试的发送选项 / Send options with retry enabled</returns>
    public static ServerSendOptions Query(int timeoutMs = DefaultTimeoutMs, int maxRetryCount = 2)
    {
        return new ServerSendOptions
        {
            TimeoutMs = timeoutMs,
            AllowRetry = true,
            MaxRetryCount = maxRetryCount,
        };
    }

    /// <summary>
    /// 创建非幂等命令选项（禁止重试）。适用于写操作和状态变更接口。
    /// </summary>
    /// <remarks>
    /// Creates non-idempotent command options (retry disabled). Suitable for write operations and state-changing interfaces.
    /// </remarks>
    /// <param name="timeoutMs">超时毫秒数 / Timeout in milliseconds</param>
    /// <returns>禁止重试的发送选项 / Send options with retry disabled</returns>
    public static ServerSendOptions Command(int timeoutMs = DefaultTimeoutMs)
    {
        return new ServerSendOptions
        {
            TimeoutMs = timeoutMs,
            AllowRetry = false,
        };
    }

    /// <summary>
    /// 创建带路由键的选项。相同 routeKey 会一致性哈希到同一实例。
    /// </summary>
    /// <remarks>
    /// Creates options with a routing key. The same routeKey is consistently hashed to the same instance.
    /// </remarks>
    /// <param name="routeKey">路由键 / Routing key</param>
    /// <param name="timeoutMs">超时毫秒数 / Timeout in milliseconds</param>
    /// <returns>带路由键的发送选项 / Send options with routing key</returns>
    public static ServerSendOptions WithRouteKey(string routeKey, int timeoutMs = DefaultTimeoutMs)
    {
        return new ServerSendOptions
        {
            TimeoutMs = timeoutMs,
            RouteKey = routeKey,
        };
    }
}
