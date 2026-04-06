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

namespace GameFrameX.NetWork.RemoteMessaging.Resilience;

/// <summary>
/// 默认重试策略。仅对标记为幂等的调用进行有限指数退避重试。
/// </summary>
/// <remarks>
/// Default retry policy. Only performs limited exponential backoff retries for calls marked as idempotent.
/// </remarks>
internal sealed class DefaultRetryPolicy : IRetryPolicy
{
    private readonly int _baseDelayMs;
    private readonly int _maxRetryCount;

    /// <summary>
    /// 初始化默认重试策略。
    /// </summary>
    /// <remarks>
    /// Initializes the default retry policy.
    /// </remarks>
    /// <param name="maxRetryCount">最大重试次数（默认 2） / Maximum retry count (default 2)</param>
    /// <param name="baseDelayMs">基础延迟毫秒数（默认 200） / Base delay in milliseconds (default 200)</param>
    public DefaultRetryPolicy(int maxRetryCount = 2, int baseDelayMs = 200)
    {
        _maxRetryCount = maxRetryCount;
        _baseDelayMs = baseDelayMs;
    }

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
    public bool ShouldRetry(RemoteCallContext context, RemoteStatusCode statusCode, int attemptCount)
    {
        if (!context.AllowRetry)
        {
            return false;
        }

        if (attemptCount >= _maxRetryCount)
        {
            return false;
        }

        return statusCode == RemoteStatusCode.Timeout || statusCode == RemoteStatusCode.ConnectionFailed;
    }

    /// <summary>
    /// 计算下一次重试前的等待时间（毫秒）。
    /// </summary>
    /// <remarks>
    /// Computes the delay in milliseconds before the next retry attempt.
    /// </remarks>
    /// <param name="attemptCount">已重试次数（从 1 开始） / Number of retry attempts (starting from 1)</param>
    /// <returns>等待毫秒数 / Delay in milliseconds</returns>
    public int GetRetryDelayMs(int attemptCount)
    {
        return _baseDelayMs * (1 << Math.Min(attemptCount, 4));
    }
}