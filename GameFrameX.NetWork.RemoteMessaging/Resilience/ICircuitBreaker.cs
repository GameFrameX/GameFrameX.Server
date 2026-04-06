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
/// 熔断器接口。按服务维度管理熔断状态，防止故障雪崩。
/// 三态模型：Closed（正常放行）→ Open（拒绝请求）→ HalfOpen（允许探测）。
/// </summary>
/// <remarks>
/// Circuit breaker interface. Manages circuit state per service to prevent cascading failures.
/// Three-state model: Closed (pass-through) → Open (reject requests) → HalfOpen (allow probing).
/// </remarks>
public interface ICircuitBreaker
{
    /// <summary>
    /// 检查指定服务是否允许请求通过。
    /// </summary>
    /// <remarks>
    /// Checks whether the specified service is allowed to pass a request.
    /// </remarks>
    /// <param name="serviceName">服务名 / Service name</param>
    /// <returns>true 允许通过；false 被熔断拒绝 / true if allowed; false if rejected by circuit breaker</returns>
    bool IsAllowed(string serviceName);

    /// <summary>
    /// 记录一次成功调用。在 HalfOpen 状态下成功会关闭熔断器。
    /// </summary>
    /// <remarks>
    /// Records a successful invocation. A success in HalfOpen state closes the circuit breaker.
    /// </remarks>
    /// <param name="serviceName">服务名 / Service name</param>
    void RecordSuccess(string serviceName);

    /// <summary>
    /// 记录一次失败调用。连续失败达到阈值会触发熔断。
    /// </summary>
    /// <remarks>
    /// Records a failed invocation. Consecutive failures reaching the threshold trigger the circuit breaker.
    /// </remarks>
    /// <param name="serviceName">服务名 / Service name</param>
    void RecordFailure(string serviceName);

    /// <summary>
    /// 获取指定服务的熔断器状态。
    /// </summary>
    /// <remarks>
    /// Gets the circuit breaker state for the specified service.
    /// </remarks>
    /// <param name="serviceName">服务名 / Service name</param>
    /// <returns>当前熔断状态 / Current circuit state</returns>
    CircuitState GetState(string serviceName);
}