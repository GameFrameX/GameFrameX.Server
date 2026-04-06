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

using System.Diagnostics;

namespace GameFrameX.NetWork.RemoteMessaging.Contracts;

/// <summary>
/// 远程调用上下文。携带超时、重试策略、追踪信息、环境参数等元数据。
/// </summary>
/// <remarks>
/// Remote call context that carries metadata such as timeout, retry policy, tracing information, and environment parameters.
/// </remarks>
public sealed class RemoteCallContext
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
    /// 目标服务名
    /// </summary>
    /// <remarks>
    /// Target service name.
    /// </remarks>
    public string ServiceName { get; init; }

    /// <summary>
    /// 超时毫秒数
    /// </summary>
    /// <remarks>
    /// Timeout duration in milliseconds.
    /// </remarks>
    public int TimeoutMs { get; init; } = DefaultTimeoutMs;

    /// <summary>
    /// 取消令牌
    /// </summary>
    /// <remarks>
    /// Cancellation token for cancelling the remote call.
    /// </remarks>
    public CancellationToken CancellationToken { get; init; } = CancellationToken.None;

    /// <summary>
    /// 是否允许重试（仅幂等接口应设为 true）
    /// </summary>
    /// <remarks>
    /// Whether retry is allowed (should only be set to true for idempotent interfaces).
    /// </remarks>
    public bool AllowRetry { get; init; }

    /// <summary>
    /// 最大重试次数（当 AllowRetry 为 true 时生效）
    /// </summary>
    /// <remarks>
    /// Maximum retry count (effective when AllowRetry is true).
    /// </remarks>
    public int MaxRetryCount { get; init; } = DefaultMaxRetryCount;

    /// <summary>
    /// 追踪 ID（跨服务链路追踪用）
    /// </summary>
    /// <remarks>
    /// Trace ID used for cross-service distributed tracing.
    /// </remarks>
    public string TraceId { get; set; }

    /// <summary>
    /// 环境参数字典。用于传递调用方上下文信息（如 ServerId、PlayerId、环境标识等）。
    /// 拦截器可读取这些参数进行日志记录、链路追踪或动态路由。
    /// </summary>
    /// <remarks>
    /// Environment parameter dictionary. Used to pass caller context information (e.g., ServerId, PlayerId, environment tag).
    /// Interceptors can read these parameters for logging, distributed tracing, or dynamic routing.
    /// </remarks>
    public Dictionary<string, string> Metadata { get; init; }

    /// <summary>
    /// 调用期间持有的追踪活动对象。
    /// </summary>
    internal Activity TraceActivity { get; set; }

    /// <summary>
    /// 从基础参数快速创建上下文
    /// </summary>
    /// <remarks>
    /// Creates a call context from basic parameters.
    /// </remarks>
    /// <param name="serviceName">服务名 / Service name</param>
    /// <param name="timeoutMs">超时毫秒 / Timeout in milliseconds</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>调用上下文 / The created call context</returns>
    public static RemoteCallContext Create(string serviceName, int timeoutMs = DefaultTimeoutMs, CancellationToken cancellationToken = default)
    {
        return new RemoteCallContext
        {
            ServiceName = serviceName,
            TimeoutMs = timeoutMs,
            CancellationToken = cancellationToken,
        };
    }

    /// <summary>
    /// 创建幂等查询上下文（AllowRetry = true）。适用于读操作和查询接口。
    /// </summary>
    /// <remarks>
    /// Creates an idempotent query context (AllowRetry = true). Suitable for read operations and query interfaces.
    /// </remarks>
    /// <param name="serviceName">目标服务名 / Target service name</param>
    /// <param name="timeoutMs">超时毫秒数 / Timeout in milliseconds</param>
    /// <param name="maxRetryCount">最大重试次数 / Maximum retry count</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>允许重试的调用上下文 / A call context with retry enabled</returns>
    public static RemoteCallContext CreateIdempotent(string serviceName, int timeoutMs = DefaultTimeoutMs, int maxRetryCount = DefaultMaxRetryCount, CancellationToken cancellationToken = default)
    {
        return new RemoteCallContext
        {
            ServiceName = serviceName,
            TimeoutMs = timeoutMs,
            AllowRetry = true,
            MaxRetryCount = maxRetryCount,
            CancellationToken = cancellationToken,
        };
    }

    /// <summary>
    /// 创建非幂等写操作上下文（AllowRetry = false）。适用于写操作和状态变更接口。
    /// </summary>
    /// <remarks>
    /// Creates a non-idempotent write context (AllowRetry = false). Suitable for write operations and state-changing interfaces.
    /// </remarks>
    /// <param name="serviceName">目标服务名 / Target service name</param>
    /// <param name="timeoutMs">超时毫秒数 / Timeout in milliseconds</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>禁止重试的调用上下文 / A call context with retry disabled</returns>
    public static RemoteCallContext CreateNonIdempotent(string serviceName, int timeoutMs = DefaultTimeoutMs, CancellationToken cancellationToken = default)
    {
        return new RemoteCallContext
        {
            ServiceName = serviceName,
            TimeoutMs = timeoutMs,
            AllowRetry = false,
            CancellationToken = cancellationToken,
        };
    }

    /// <summary>
    /// 创建携带环境参数的上下文。
    /// </summary>
    /// <remarks>
    /// Creates a call context with environment parameters.
    /// </remarks>
    /// <param name="serviceName">目标服务名 / Target service name</param>
    /// <param name="metadata">环境参数字典 / Environment parameter dictionary</param>
    /// <param name="timeoutMs">超时毫秒数 / Timeout in milliseconds</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>携带环境参数的调用上下文 / A call context with environment parameters</returns>
    public static RemoteCallContext CreateWithMetadata(string serviceName, Dictionary<string, string> metadata, int timeoutMs = DefaultTimeoutMs, CancellationToken cancellationToken = default)
    {
        return new RemoteCallContext
        {
            ServiceName = serviceName,
            TimeoutMs = timeoutMs,
            Metadata = metadata,
            CancellationToken = cancellationToken,
        };
    }

    /// <summary>
    /// 向上下文中添加一个环境参数。如果 Metadata 为空则自动创建字典。
    /// </summary>
    /// <remarks>
    /// Adds an environment parameter to the context. Creates the Metadata dictionary if it is null.
    /// </remarks>
    /// <param name="key">参数键 / Parameter key</param>
    /// <param name="value">参数值 / Parameter value</param>
    /// <returns>当前上下文实例（支持链式调用） / The current context instance (supports fluent chaining)</returns>
    public RemoteCallContext WithMetadata(string key, string value)
    {
        var metadata = Metadata ?? new Dictionary<string, string>();
        metadata[key] = value;

        return new RemoteCallContext
        {
            ServiceName = ServiceName,
            TimeoutMs = TimeoutMs,
            CancellationToken = CancellationToken,
            AllowRetry = AllowRetry,
            MaxRetryCount = MaxRetryCount,
            TraceId = TraceId,
            Metadata = metadata,
        };
    }
}