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
/// 消息语义分层。
/// </summary>
/// <remarks>
/// Message semantic classification.
/// </remarks>
public enum MessageSemantics
{
    /// <summary>
    /// 查询（允许重试）
    /// </summary>
    /// <remarks>
    /// Query (retry allowed).
    /// </remarks>
    Query = 0,

    /// <summary>
    /// 命令（默认不重试，必须幂等键）
    /// </summary>
    /// <remarks>
    /// Command (no retry by default, idempotency key required).
    /// </remarks>
    Command = 1,

    /// <summary>
    /// 通知（OneWay，失败告警）
    /// </summary>
    /// <remarks>
    /// Notification (one-way, alert on failure).
    /// </remarks>
    Notification = 2,
}

/// <summary>
/// 消息治理策略配置。每类消息都应有对应的策略。
/// </summary>
/// <remarks>
/// Message governance policy configuration. Each message type should have a corresponding policy.
/// </remarks>
public sealed class MessageGovernancePolicy
{
    /// <summary>
    /// 默认超时毫秒数
    /// </summary>
    /// <remarks>
    /// Default timeout in milliseconds.
    /// </remarks>
    public const int DefaultTimeoutMs = 5000;

    /// <summary>
    /// 默认最大重试次数
    /// </summary>
    /// <remarks>
    /// Default maximum retry count.
    /// </remarks>
    public const int DefaultMaxRetryCount = 2;

    /// <summary>
    /// 消息语义
    /// </summary>
    /// <remarks>
    /// Message semantics.
    /// </remarks>
    public MessageSemantics Semantics { get; set; } = MessageSemantics.Query;

    /// <summary>
    /// 超时毫秒数
    /// </summary>
    /// <remarks>
    /// Timeout in milliseconds.
    /// </remarks>
    public int TimeoutMs { get; set; } = DefaultTimeoutMs;

    /// <summary>
    /// 最大重试次数
    /// </summary>
    /// <remarks>
    /// Maximum retry count.
    /// </remarks>
    public int MaxRetryCount { get; set; } = DefaultMaxRetryCount;

    /// <summary>
    /// 是否要求幂等键
    /// </summary>
    /// <remarks>
    /// Whether an idempotency key is required.
    /// </remarks>
    public bool RequireIdempotencyKey { get; set; }

    /// <summary>
    /// 查询类策略：允许重试，不要求幂等键
    /// </summary>
    /// <remarks>
    /// Query policy: retry allowed, no idempotency key required.
    /// </remarks>
    public static MessageGovernancePolicy QueryPolicy(int timeoutMs = DefaultTimeoutMs, int maxRetryCount = DefaultMaxRetryCount)
    {
        return new MessageGovernancePolicy
        {
            Semantics = MessageSemantics.Query,
            TimeoutMs = timeoutMs,
            MaxRetryCount = maxRetryCount,
            RequireIdempotencyKey = false,
        };
    }

    /// <summary>
    /// 命令类策略：默认不重试，强制幂等键
    /// </summary>
    /// <remarks>
    /// Command policy: no retry by default, idempotency key enforced.
    /// </remarks>
    public static MessageGovernancePolicy CommandPolicy(int timeoutMs = DefaultTimeoutMs)
    {
        return new MessageGovernancePolicy
        {
            Semantics = MessageSemantics.Command,
            TimeoutMs = timeoutMs,
            MaxRetryCount = 0,
            RequireIdempotencyKey = true,
        };
    }

    /// <summary>
    /// 通知类策略：OneWay，失败告警
    /// </summary>
    /// <remarks>
    /// Notification policy: one-way, alert on failure.
    /// </remarks>
    public static MessageGovernancePolicy NotificationPolicy(int timeoutMs = DefaultTimeoutMs)
    {
        return new MessageGovernancePolicy
        {
            Semantics = MessageSemantics.Notification,
            TimeoutMs = timeoutMs,
            MaxRetryCount = 0,
            RequireIdempotencyKey = false,
        };
    }
}
