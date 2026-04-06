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

namespace GameFrameX.NetWork.RemoteMessaging.Contracts;

/// <summary>
/// 远程调用结果。统一包装响应数据和状态码，替代裸 null 返回。
/// </summary>
/// <remarks>
/// Remote call result that uniformly wraps response data and status code, replacing raw null returns.
/// </remarks>
/// <typeparam name="T">响应消息类型 / Response message type</typeparam>
public sealed class RemoteCallResult<T> where T : class, IResponseMessage
{
    /// <summary>
    /// 调用状态码
    /// </summary>
    /// <remarks>
    /// The status code of the remote call.
    /// </remarks>
    public RemoteStatusCode StatusCode { get; init; }

    /// <summary>
    /// 响应对象（仅在 StatusCode == Success 时非 null）
    /// </summary>
    /// <remarks>
    /// The response object (non-null only when StatusCode equals Success).
    /// </remarks>
    public T Response { get; init; }

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
    /// Elapsed time of the call in milliseconds.
    /// </remarks>
    public long ElapsedMs { get; init; }

    /// <summary>
    /// 重试次数
    /// </summary>
    /// <remarks>
    /// Number of retries attempted.
    /// </remarks>
    public int RetryCount { get; init; }

    /// <summary>
    /// 追踪 ID
    /// </summary>
    /// <remarks>
    /// Trace ID for distributed tracing.
    /// </remarks>
    public string TraceId { get; init; }

    /// <summary>
    /// 是否调用成功
    /// </summary>
    /// <remarks>
    /// Indicates whether the call was successful.
    /// </remarks>
    public bool IsSuccess
    {
        get { return StatusCode == RemoteStatusCode.Success; }
    }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    /// <remarks>
    /// Creates a successful result.
    /// </remarks>
    /// <param name="response">响应对象 / The response object</param>
    /// <param name="elapsedMs">调用耗时毫秒数 / Elapsed time in milliseconds</param>
    /// <param name="traceId">追踪 ID / Trace ID for distributed tracing</param>
    /// <param name="retryCount">重试次数 / Number of retries attempted</param>
    /// <returns>成功的远程调用结果 / A successful remote call result</returns>
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
    /// <remarks>
    /// Creates a failure result.
    /// </remarks>
    /// <param name="statusCode">失败状态码 / The failure status code</param>
    /// <param name="errorMessage">错误描述 / Error description</param>
    /// <param name="elapsedMs">调用耗时毫秒数 / Elapsed time in milliseconds</param>
    /// <param name="traceId">追踪 ID / Trace ID for distributed tracing</param>
    /// <returns>失败的远程调用结果 / A failed remote call result</returns>
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