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

namespace GameFrameX.NetWork.RemoteMessaging.Contracts;

/// <summary>
/// 远程调用状态码。统一技术层错误分类，与业务错误码分离。
/// </summary>
public enum RemoteStatusCode
{
    /// <summary>
    /// 调用成功
    /// </summary>
    Success = 0,

    /// <summary>
    /// 超时
    /// </summary>
    Timeout = 1,

    /// <summary>
    /// 连接失败（目标服务不可达）
    /// </summary>
    ConnectionFailed = 2,

    /// <summary>
    /// 端点解析失败（服务发现未找到目标）
    /// </summary>
    EndpointNotFound = 3,

    /// <summary>
    /// 响应类型不匹配
    /// </summary>
    ResponseTypeMismatch = 4,

    /// <summary>
    /// 调用被取消
    /// </summary>
    Cancelled = 5,

    /// <summary>
    /// 重试次数耗尽
    /// </summary>
    RetryExhausted = 6,

    /// <summary>
    /// 连接已关闭
    /// </summary>
    ConnectionClosed = 7,

    /// <summary>
    /// 意外的响应（类型不匹配或协议错误）
    /// </summary>
    UnexpectedResponse = 8,

    /// <summary>
    /// 熔断器已打开（请求被拒绝）
    /// </summary>
    CircuitOpen = 10,

    /// <summary>
    /// 服务不可用（健康评分过低）
    /// </summary>
    ServiceUnavailable = 11,

    /// <summary>
    /// 未知错误
    /// </summary>
    UnknownError = 99,
}