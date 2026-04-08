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

using GameFrameX.NetWork.RemoteMessaging.Contracts;

namespace GameFrameX.NetWork.RemoteMessaging.Unified;

/// <summary>
/// 系统消息发送结果（请求-响应模式）。
/// </summary>
/// <remarks>
/// Server message send result (request-response pattern).
/// </remarks>
/// <typeparam name="TResp">响应消息类型 / Response message type</typeparam>
public sealed class ServerSendResult<TResp> where TResp : class, IResponseMessage
{
    /// <summary>
    /// 调用状态码
    /// </summary>
    /// <remarks>
    /// The status code of the remote call.
    /// </remarks>
    public RemoteStatusCode StatusCode { get; init; }

    /// <summary>
    /// 响应对象（仅在成功时非 null）
    /// </summary>
    /// <remarks>
    /// The response object (non-null only on success).
    /// </remarks>
    public TResp Response { get; init; }

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
    /// 选中的目标实例ID
    /// </summary>
    /// <remarks>
    /// Selected target instance ID.
    /// </remarks>
    public string SelectedInstance { get; init; }

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
    /// 从远程调用结果创建
    /// </summary>
    /// <remarks>
    /// Creates from a remote call result.
    /// </remarks>
    /// <param name="result">远程调用结果 / Remote call result</param>
    /// <param name="selectedInstance">选中的实例 / Selected instance</param>
    /// <returns>系统消息发送结果 / Server send result</returns>
    public static ServerSendResult<TResp> FromRemoteResult(RemoteCallResult<TResp> result, string selectedInstance = null)
    {
        return new ServerSendResult<TResp>
        {
            StatusCode = result.StatusCode,
            Response = result.Response,
            ErrorMessage = result.ErrorMessage,
            ElapsedMs = result.ElapsedMs,
            TraceId = result.TraceId,
            RetryCount = result.RetryCount,
            SelectedInstance = selectedInstance,
        };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    /// <remarks>
    /// Creates a failure result.
    /// </remarks>
    /// <param name="statusCode">状态码 / Status code</param>
    /// <param name="errorMessage">错误描述 / Error description</param>
    /// <param name="elapsedMs">耗时毫秒数 / Elapsed time in milliseconds</param>
    /// <returns>失败的发送结果 / A failed send result</returns>
    public static ServerSendResult<TResp> Fail(RemoteStatusCode statusCode, string errorMessage, long elapsedMs = 0)
    {
        return new ServerSendResult<TResp>
        {
            StatusCode = statusCode,
            ErrorMessage = errorMessage,
            ElapsedMs = elapsedMs,
        };
    }
}

/// <summary>
/// 系统消息发送结果（单向模式，无响应）。
/// </summary>
/// <remarks>
/// Server message send result (one-way pattern, no response).
/// </remarks>
public sealed class ServerSendResult
{
    /// <summary>
    /// 是否发送成功
    /// </summary>
    /// <remarks>
    /// Indicates whether the send was successful.
    /// </remarks>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// 错误描述
    /// </summary>
    /// <remarks>
    /// Error description.
    /// </remarks>
    public string ErrorMessage { get; init; }

    /// <summary>
    /// 选中的目标实例ID
    /// </summary>
    /// <remarks>
    /// Selected target instance ID.
    /// </remarks>
    public string SelectedInstance { get; init; }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    /// <remarks>
    /// Creates a successful result.
    /// </remarks>
    /// <param name="selectedInstance">选中的实例 / Selected instance</param>
    /// <returns>成功的发送结果 / A successful send result</returns>
    public static ServerSendResult Ok(string selectedInstance = null)
    {
        return new ServerSendResult
        {
            IsSuccess = true,
            SelectedInstance = selectedInstance,
        };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    /// <remarks>
    /// Creates a failure result.
    /// </remarks>
    /// <param name="errorMessage">错误描述 / Error description</param>
    /// <returns>失败的发送结果 / A failed send result</returns>
    public static ServerSendResult Fail(string errorMessage)
    {
        return new ServerSendResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
        };
    }
}
