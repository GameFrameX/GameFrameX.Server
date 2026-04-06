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
/// 远程调用状态码。统一技术层错误分类，与业务错误码分离。
/// </summary>
/// <remarks>
/// Remote call status codes that provide unified technical-level error classification, separate from business error codes.
/// </remarks>
public enum RemoteStatusCode
{
    /// <summary>
    /// 调用成功
    /// </summary>
    /// <remarks>
    /// The call succeeded.
    /// </remarks>
    Success = 0,

    /// <summary>
    /// 超时
    /// </summary>
    /// <remarks>
    /// The call timed out.
    /// </remarks>
    Timeout = 1,

    /// <summary>
    /// 连接失败（目标服务不可达）
    /// </summary>
    /// <remarks>
    /// Connection failed (target service unreachable).
    /// </remarks>
    ConnectionFailed = 2,

    /// <summary>
    /// 端点解析失败（服务发现未找到目标）
    /// </summary>
    /// <remarks>
    /// Endpoint resolution failed (service discovery did not find the target).
    /// </remarks>
    EndpointNotFound = 3,

    /// <summary>
    /// 响应类型不匹配
    /// </summary>
    /// <remarks>
    /// Response type mismatch.
    /// </remarks>
    ResponseTypeMismatch = 4,

    /// <summary>
    /// 调用被取消
    /// </summary>
    /// <remarks>
    /// The call was cancelled.
    /// </remarks>
    Cancelled = 5,

    /// <summary>
    /// 重试次数耗尽
    /// </summary>
    /// <remarks>
    /// Retry attempts exhausted.
    /// </remarks>
    RetryExhausted = 6,

    /// <summary>
    /// 连接已关闭
    /// </summary>
    /// <remarks>
    /// The connection has been closed.
    /// </remarks>
    ConnectionClosed = 7,

    /// <summary>
    /// 意外的响应（类型不匹配或协议错误）
    /// </summary>
    /// <remarks>
    /// Unexpected response (type mismatch or protocol error).
    /// </remarks>
    UnexpectedResponse = 8,

    /// <summary>
    /// 熔断器已打开（请求被拒绝）
    /// </summary>
    /// <remarks>
    /// Circuit breaker is open (request rejected).
    /// </remarks>
    CircuitOpen = 10,

    /// <summary>
    /// 服务不可用（健康评分过低）
    /// </summary>
    /// <remarks>
    /// Service unavailable (health score too low).
    /// </remarks>
    ServiceUnavailable = 11,

    /// <summary>
    /// 未知错误
    /// </summary>
    /// <remarks>
    /// Unknown error.
    /// </remarks>
    UnknownError = 99,
}