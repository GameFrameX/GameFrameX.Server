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
/// 透传熔断器。用于关闭统一能力时保持老行为：始终放行，不参与状态管理。
/// </summary>
/// <remarks>
/// Pass-through circuit breaker. Used when the unified resilience capability is disabled to preserve legacy behavior:
/// always allows requests through without participating in state management.
/// </remarks>
internal sealed class PassThroughCircuitBreaker : ICircuitBreaker
{
    
    public bool IsAllowed(string serviceName)
    {
        return true;
    }

    
    public void RecordSuccess(string serviceName)
    {
    }

    
    public void RecordFailure(string serviceName)
    {
    }

    
    public CircuitState GetState(string serviceName)
    {
        return CircuitState.Closed;
    }
}