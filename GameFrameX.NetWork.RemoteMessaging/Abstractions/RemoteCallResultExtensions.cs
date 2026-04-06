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
/// 远程调用错误语义目录。定义技术错误、业务错误和重试语义的统一映射规则。
///
/// 重试语义：
/// - 可重试（Retryable）：超时、连接失败、熔断半开探测 → 允许有限重试
/// - 不可重试（NonRetryable）：端点未找到、取消、响应不匹配、熔断已打开 → 立即失败
/// - 条件重试（Conditional）：未知错误 → 根据异常类型决定
/// </summary>
public static class RemoteErrorCatalog
{
    /// <summary>
    /// 判断指定状态码是否允许重试。
    /// </summary>
    /// <param name="statusCode">技术状态码</param>
    /// <returns>重试语义</returns>
    public static RetrySemantics GetRetrySemantics(this RemoteStatusCode statusCode)
    {
        switch (statusCode)
        {
            case RemoteStatusCode.Success:
                return RetrySemantics.None;
            case RemoteStatusCode.Timeout:
            case RemoteStatusCode.ConnectionFailed:
            case RemoteStatusCode.ConnectionClosed:
                return RetrySemantics.Retryable;
            case RemoteStatusCode.EndpointNotFound:
            case RemoteStatusCode.ResponseTypeMismatch:
            case RemoteStatusCode.UnexpectedResponse:
            case RemoteStatusCode.Cancelled:
            case RemoteStatusCode.RetryExhausted:
            case RemoteStatusCode.CircuitOpen:
            case RemoteStatusCode.ServiceUnavailable:
                return RetrySemantics.NonRetryable;
            case RemoteStatusCode.UnknownError:
                return RetrySemantics.Conditional;
            default:
                return RetrySemantics.NonRetryable;
        }
    }

    /// <summary>
    /// 获取状态码的可读描述。
    /// </summary>
    /// <param name="statusCode">技术状态码</param>
    /// <returns>可读描述</returns>
    public static string GetDescription(this RemoteStatusCode statusCode)
    {
        switch (statusCode)
        {
            case RemoteStatusCode.Success:
                return "调用成功";
            case RemoteStatusCode.Timeout:
                return "调用超时，对端未在指定时间内响应";
            case RemoteStatusCode.ConnectionFailed:
                return "连接失败，对端服务不可达";
            case RemoteStatusCode.ConnectionClosed:
                return "连接已关闭，对端主动断开";
            case RemoteStatusCode.EndpointNotFound:
                return "端点未找到，服务发现未解析到目标地址";
            case RemoteStatusCode.ResponseTypeMismatch:
                return "响应类型不匹配";
            case RemoteStatusCode.UnexpectedResponse:
                return "意外的响应";
            case RemoteStatusCode.Cancelled:
                return "调用被取消";
            case RemoteStatusCode.RetryExhausted:
                return "重试次数耗尽";
            case RemoteStatusCode.CircuitOpen:
                return "熔断器已打开，请求被拒绝";
            case RemoteStatusCode.ServiceUnavailable:
                return "服务不可用，健康评分过低";
            case RemoteStatusCode.UnknownError:
                return "未知错误";
            default:
                return $"未知状态码: {statusCode}";
        }
    }

    /// <summary>
    /// 判断状态码是否表示调用成功。
    /// </summary>
    public static bool IsSuccess(this RemoteStatusCode statusCode)
    {
        return statusCode == RemoteStatusCode.Success;
    }

    /// <summary>
    /// 判断状态码是否为可恢复的临时错误（允许重试）。
    /// </summary>
    public static bool IsTransient(this RemoteStatusCode statusCode)
    {
        return statusCode.GetRetrySemantics() == RetrySemantics.Retryable;
    }
}