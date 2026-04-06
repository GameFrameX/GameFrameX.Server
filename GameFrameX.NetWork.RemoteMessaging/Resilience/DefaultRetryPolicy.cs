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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public int GetRetryDelayMs(int attemptCount)
    {
        return _baseDelayMs * (1 << Math.Min(attemptCount, 4));
    }
}