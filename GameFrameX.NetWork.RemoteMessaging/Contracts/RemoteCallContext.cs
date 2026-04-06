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

namespace GameFrameX.NetWork.RemoteMessaging.Contracts;

/// <summary>
/// 远程调用上下文。携带超时、重试策略、追踪信息等元数据。
/// </summary>
public sealed class RemoteCallContext
{
    /// <summary>
    /// 默认超时毫秒数
    /// </summary>
    public const int DefaultTimeoutMs = 5000;

    /// <summary>
    /// 目标服务名
    /// </summary>
    public string ServiceName { get; init; }

    /// <summary>
    /// 超时毫秒数
    /// </summary>
    public int TimeoutMs { get; init; } = DefaultTimeoutMs;

    /// <summary>
    /// 取消令牌
    /// </summary>
    public CancellationToken CancellationToken { get; init; } = CancellationToken.None;

    /// <summary>
    /// 是否允许重试（仅幂等接口应设为 true）
    /// </summary>
    public bool AllowRetry { get; init; }

    /// <summary>
    /// 最大重试次数（当 AllowRetry 为 true 时生效）
    /// </summary>
    public int MaxRetryCount { get; init; } = 2;

    /// <summary>
    /// 追踪 ID（跨服务链路追踪用）
    /// </summary>
    public string TraceId { get; set; }

    /// <summary>
    /// 调用期间持有的追踪活动对象。
    /// </summary>
    internal Activity TraceActivity { get; set; }

    /// <summary>
    /// 从基础参数快速创建上下文
    /// </summary>
    /// <param name="serviceName">服务名</param>
    /// <param name="timeoutMs">超时毫秒</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>调用上下文</returns>
    public static RemoteCallContext Create(string serviceName, int timeoutMs = DefaultTimeoutMs, CancellationToken cancellationToken = default)
    {
        return new RemoteCallContext
        {
            ServiceName = serviceName,
            TimeoutMs = timeoutMs,
            CancellationToken = cancellationToken,
        };
    }
}
