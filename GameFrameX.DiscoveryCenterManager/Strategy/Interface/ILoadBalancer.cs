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

using GameFrameX.DiscoveryCenterManager.Strategy.Models;
using GameFrameX.DiscoveryCenterManager.Strategy.Abstractions;
using System.Collections.Generic;

namespace GameFrameX.DiscoveryCenterManager.Strategy.Interface;

/// <summary>
/// 负载均衡器接口
/// </summary>
public interface ILoadBalancer
{
    /// <summary>
    /// 负载均衡策略
    /// </summary>
    LoadBalanceStrategy Strategy { get; }

    /// <summary>
    /// 选择一个服务器实例
    /// </summary>
    /// <param name="serverType">服务器类型</param>
    /// <param name="availableInstances">可用的服务器实例列表</param>
    /// <param name="key">用于一致性哈希的键（可选）</param>
    /// <returns>选择的服务器实例ID，如果失败返回0</returns>
    long SelectInstance(string serverType, List<long> availableInstances, string key = null);

    /// <summary>
    /// 更新实例权重或连接数
    /// </summary>
    /// <param name="serverInstanceId">服务器实例ID</param>
    /// <param name="connections">当前连接数</param>
    void UpdateInstanceMetrics(long serverInstanceId, int connections = 0);

    /// <summary>
    /// 标记实例为不可用（故障转移）
    /// </summary>
    /// <param name="serverInstanceId">服务器实例ID</param>
    void MarkInstanceUnavailable(long serverInstanceId);

    /// <summary>
    /// 标记实例为可用（故障恢复）
    /// </summary>
    /// <param name="serverInstanceId">服务器实例ID</param>
    void MarkInstanceAvailable(long serverInstanceId);

    /// <summary>
    /// 重置负载均衡器状态
    /// </summary>
    void Reset();

    /// <summary>
    /// 获取负载均衡器统计信息
    /// </summary>
    LoadBalancerStatistics GetStatistics();

    /// <summary>
    /// 执行健康检查
    /// </summary>
    bool PerformHealthCheck();

    /// <summary>
    /// 增强的实例选择方法，支持上下文和元数据（可选实现）
    /// </summary>
    /// <param name="context">负载均衡上下文</param>
    /// <returns>负载均衡选择结果</returns>
    LoadBalancingSelection SelectInstanceEnhanced(LoadBalancingContext context)
    {
        // 默认实现：转换为基础调用
        var instanceId = SelectInstance(context.ServerType, context.AvailableInstances, context.AffinityKey);
        if (instanceId > 0)
        {
            return LoadBalancingSelection.Success(instanceId, SelectionReason.Fallback, 0, 0);
        }
        return LoadBalancingSelection.Failure("No instance selected");
    }

    /// <summary>
    /// 更新增强的实例指标（可选实现）
    /// </summary>
    /// <param name="metrics">实例指标</param>
    void UpdateInstanceMetricsEnhanced(InstanceMetrics metrics)
    {
        // 默认实现：转换为基础调用
        if (metrics != null)
        {
            UpdateInstanceMetrics(metrics.InstanceId, metrics.ActiveConnections);
        }
    }
}

/// <summary>
/// 增强负载均衡器接口（可选的高级功能）
/// </summary>
public interface IEnhancedLoadBalancer : ILoadBalancer
{
    /// <summary>
    /// 是否启用熔断器
    /// </summary>
    bool EnableCircuitBreaker { get; set; }

    /// <summary>
    /// 支持熔断器的实例选择
    /// </summary>
    LoadBalancingSelection SelectInstanceWithCircuitBreaker(LoadBalancingContext context);

    /// <summary>
    /// 更新熔断器状态
    /// </summary>
    void UpdateCircuitBreakerState(long serverInstanceId, CircuitBreakerState state);

    /// <summary>
    /// 获取熔断器状态
    /// </summary>
    CircuitBreakerState GetCircuitBreakerState(long serverInstanceId);

    /// <summary>
    /// 触发熔断器
    /// </summary>
    void TriggerCircuitBreaker(long instanceId);

    /// <summary>
    /// 检查熔断器是否开启
    /// </summary>
    bool IsCircuitBreakerOpen(long instanceId);

    /// <summary>
    /// 重置熔断器
    /// </summary>
    void ResetCircuitBreaker(long instanceId);
}