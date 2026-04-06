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

namespace GameFrameX.NetWork.RemoteMessaging.Resilience;

/// <summary>
/// 重试策略接口。仅对幂等接口进行有限重试。
/// </summary>
/// <remarks>
/// Retry policy interface. Only performs limited retries for idempotent operations.
/// </remarks>
public interface IRetryPolicy
{
    /// <summary>
    /// 判断指定调用是否允许重试。
    /// </summary>
    /// <remarks>
    /// Determines whether the specified call is allowed to retry.
    /// </remarks>
    /// <param name="context">调用上下文 / Call context</param>
    /// <param name="statusCode">上一次调用的状态码 / Status code from the last invocation</param>
    /// <param name="attemptCount">已重试次数 / Number of retry attempts already made</param>
    /// <returns>true 允许重试；false 不允许 / true if retry is allowed; false otherwise</returns>
    bool ShouldRetry(RemoteCallContext context, RemoteStatusCode statusCode, int attemptCount);

    /// <summary>
    /// 计算下一次重试前的等待时间（毫秒）。
    /// </summary>
    /// <remarks>
    /// Computes the delay in milliseconds before the next retry attempt.
    /// </remarks>
    /// <param name="attemptCount">已重试次数（从 1 开始） / Number of retry attempts (starting from 1)</param>
    /// <returns>等待毫秒数 / Delay in milliseconds</returns>
    int GetRetryDelayMs(int attemptCount);
}