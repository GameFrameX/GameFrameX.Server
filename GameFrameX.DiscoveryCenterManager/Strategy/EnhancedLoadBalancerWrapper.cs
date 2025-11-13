// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
// 
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
// 
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
// 
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
// 
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using GameFrameX.DiscoveryCenterManager.Strategy.Abstractions;
using GameFrameX.DiscoveryCenterManager.Strategy.Interface;
using GameFrameX.DiscoveryCenterManager.Strategy.Models;

namespace GameFrameX.DiscoveryCenterManager.Strategy;

/// <summary>
/// 增强型负载均衡器包装器
/// </summary>
internal class EnhancedLoadBalancerWrapper : IEnhancedLoadBalancer
{
    private readonly ILoadBalancer _innerLoadBalancer;

    public EnhancedLoadBalancerWrapper(ILoadBalancer loadBalancer)
    {
        _innerLoadBalancer = loadBalancer ?? throw new ArgumentNullException(nameof(loadBalancer));
    }

    public LoadBalanceStrategy Strategy
    {
        get { return _innerLoadBalancer.Strategy; }
    }

    public long SelectInstance(string serverType, List<long> availableInstances, string key = null)
    {
        return _innerLoadBalancer.SelectInstance(serverType, availableInstances, key);
    }

    public LoadBalancingSelection SelectInstanceEnhanced(LoadBalancingContext context)
    {
        try
        {
            var instanceId = _innerLoadBalancer.SelectInstance(context.ServerType, context.AvailableInstances, context.AffinityKey);
            if (instanceId > 0)
            {
                return LoadBalancingSelection.Success(instanceId, SelectionReason.Fallback, 0, 0);
            }
            return LoadBalancingSelection.Failure("No instance selected by wrapped load balancer");
        }
        catch (Exception ex)
        {
            return LoadBalancingSelection.Failure(ex.Message);
        }
    }

    public void UpdateInstanceMetrics(long serverInstanceId, int connections = 0)
    {
        _innerLoadBalancer.UpdateInstanceMetrics(serverInstanceId, connections);
    }

    public void UpdateInstanceMetricsEnhanced(InstanceMetrics metrics)
    {
        _innerLoadBalancer.UpdateInstanceMetricsEnhanced(metrics);
    }

    public void MarkInstanceUnavailable(long serverInstanceId)
    {
        _innerLoadBalancer.MarkInstanceUnavailable(serverInstanceId);
    }

    public void MarkInstanceAvailable(long serverInstanceId)
    {
        _innerLoadBalancer.MarkInstanceAvailable(serverInstanceId);
    }

    public void Reset()
    {
        _innerLoadBalancer.Reset();
    }

    public LoadBalancerStatistics GetStatistics()
    {
        return _innerLoadBalancer.GetStatistics();
    }

    public bool PerformHealthCheck()
    {
        return _innerLoadBalancer.PerformHealthCheck();
    }

    public bool EnableCircuitBreaker { get; set; } = false;

    public LoadBalancingSelection SelectInstanceWithCircuitBreaker(LoadBalancingContext context)
    {
        // 简化实现：直接调用基础选择方法
        return SelectInstanceEnhanced(context);
    }

    public void UpdateCircuitBreakerState(long serverInstanceId, CircuitBreakerState state)
    {
        // 包装器默认不实现熔断器功能
    }

    public CircuitBreakerState GetCircuitBreakerState(long serverInstanceId)
    {
        return CircuitBreakerState.Closed; // 默认关闭状态
    }

    public void TriggerCircuitBreaker(long instanceId)
    {
        // 包装器默认不实现熔断器功能
    }

    public bool IsCircuitBreakerOpen(long instanceId)
    {
        return false;
    }

    public void ResetCircuitBreaker(long instanceId)
    {
        // 包装器默认不实现熔断器功能
    }
}