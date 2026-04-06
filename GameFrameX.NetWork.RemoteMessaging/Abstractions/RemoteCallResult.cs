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

namespace GameFrameX.NetWork.RemoteMessaging.Abstractions;

/// <summary>
/// 远程调用结果。统一包装响应数据和状态码，替代裸 null 返回。
/// </summary>
/// <typeparam name="T">响应消息类型</typeparam>
public sealed class RemoteCallResult<T> where T : class
{
    /// <summary>
    /// 调用状态码
    /// </summary>
    public RemoteStatusCode StatusCode { get; init; }

    /// <summary>
    /// 响应对象（仅在 StatusCode == Success 时非 null）
    /// </summary>
    public T Response { get; init; }

    /// <summary>
    /// 错误描述
    /// </summary>
    public string ErrorMessage { get; init; }

    /// <summary>
    /// 调用耗时毫秒数
    /// </summary>
    public long ElapsedMs { get; init; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; init; }

    /// <summary>
    /// 追踪 ID
    /// </summary>
    public string TraceId { get; init; }

    /// <summary>
    /// 是否调用成功
    /// </summary>
    public bool IsSuccess
    {
        get { return StatusCode == RemoteStatusCode.Success; }
    }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static RemoteCallResult<T> Ok(T response, long elapsedMs, string traceId = null, int retryCount = 0)
    {
        return new RemoteCallResult<T>
        {
            StatusCode = RemoteStatusCode.Success,
            Response = response,
            ElapsedMs = elapsedMs,
            TraceId = traceId,
            RetryCount = retryCount,
        };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static RemoteCallResult<T> Fail(RemoteStatusCode statusCode, string errorMessage, long elapsedMs = 0, string traceId = null)
    {
        return new RemoteCallResult<T>
        {
            StatusCode = statusCode,
            ErrorMessage = errorMessage,
            ElapsedMs = elapsedMs,
            TraceId = traceId,
        };
    }
}
