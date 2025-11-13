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
// 侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository: https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using System.Collections.Concurrent;
using System.Diagnostics;
using GameFrameX.DiscoveryCenterManager.Strategy.Interface;
using GameFrameX.DiscoveryCenterManager.Strategy.Models;
using GameFrameX.DiscoveryCenterManager.Strategy.Abstractions;
using GameFrameX.Foundation.Logger;

namespace GameFrameX.DiscoveryCenterManager.Strategy;

/// <summary>
/// 负载均衡器基类
/// 提供通用功能和统计收集
/// </summary>
public abstract class BaseLoadBalancer : ILoadBalancer
{
    protected readonly ConcurrentDictionary<long, bool> _instanceAvailability = new();
    protected readonly ConcurrentDictionary<long, InstanceMetrics> _instanceMetrics = new();
    protected readonly LoadBalancerStatistics _statistics = new();
    protected readonly LoadBalanceStrategy _strategy;
    protected readonly Stopwatch _stopwatch = new Stopwatch();

    /// <summary>
    /// 负载均衡策略
    /// </summary>
    public LoadBalanceStrategy Strategy => _strategy;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="strategy">负载均衡策略</param>
    protected BaseLoadBalancer(LoadBalanceStrategy strategy)
    {
        _strategy = strategy;
    }

    /// <summary>
    /// 选择一个服务器实例（基础实现，保持向后兼容）
    /// </summary>
    public virtual long SelectInstance(string serverType, List<long> availableInstances, string key = null)
    {
        try
        {
            _stopwatch.Restart();

            // 验证输入参数
            if (string.IsNullOrEmpty(serverType) || availableInstances == null || availableInstances.Count == 0)
            {
                return 0;
            }

            // 过滤可用实例
            var activeInstances = FilterAvailableInstances(availableInstances);
            if (activeInstances.Count == 0)
            {
                // 如果没有可用实例，重置状态并使用所有实例
                ResetAvailability(availableInstances);
                activeInstances = availableInstances;
            }

            // 调用具体的选择算法
            var selectedInstanceId = SelectInstanceInternal(serverType, activeInstances, key);

            _stopwatch.Stop();

            // 更新统计信息
            UpdateStatistics(selectedInstanceId > 0, _stopwatch.ElapsedMilliseconds);

            return selectedInstanceId;
        }
        catch (Exception ex)
        {
            LogHelper.Error($"负载均衡器选择实例时发生错误: {ex.Message}");
            _statistics.FailedSelections++;
            return 0;
        }
    }

    /// <summary>
    /// 增强的实例选择方法
    /// </summary>
    public virtual LoadBalancingSelection SelectInstanceEnhanced(LoadBalancingContext context)
    {
        try
        {
            _stopwatch.Restart();

            // 验证上下文
            if (context == null || string.IsNullOrEmpty(context.ServerType) || context.AvailableInstances == null || context.AvailableInstances.Count == 0)
            {
                return LoadBalancingSelection.Failure("Invalid context or no available instances");
            }

            // 过滤可用实例
            var activeInstances = FilterAvailableInstances(context.AvailableInstances);
            if (activeInstances.Count == 0)
            {
                // 如果没有可用实例，重置状态并使用所有实例
                ResetAvailability(context.AvailableInstances);
                activeInstances = context.AvailableInstances;
            }

            // 调用增强的选择算法
            var selection = SelectInstanceEnhancedInternal(context, activeInstances);

            _stopwatch.Stop();

            // 更新统计信息
            UpdateStatistics(selection.IsSuccess, _stopwatch.ElapsedMilliseconds);

            if (selection.IsSuccess)
            {
                UpdateInstanceSelectionCount(selection.InstanceId);
            }

            return selection;
        }
        catch (Exception ex)
        {
            LogHelper.Error($"增强负载均衡器选择实例时发生错误: {ex.Message}");
            _statistics.FailedSelections++;
            return LoadBalancingSelection.Failure(ex.Message);
        }
    }

    /// <summary>
    /// 更新实例权重或连接数（基础实现）
    /// </summary>
    public virtual void UpdateInstanceMetrics(long serverInstanceId, int connections = 0)
    {
        try
        {
            var metrics = _instanceMetrics.GetOrAdd(serverInstanceId, _ => new InstanceMetrics { InstanceId = serverInstanceId });
            metrics.ActiveConnections = connections;
            metrics.LastUpdated = System.DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            LogHelper.Error($"更新实例指标时发生错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 更新增强的实例指标
    /// </summary>
    public virtual void UpdateInstanceMetricsEnhanced(InstanceMetrics metrics)
    {
        try
        {
            if (metrics == null)
            {
                return;
            }

            _instanceMetrics.AddOrUpdate(metrics.InstanceId, metrics, (_, existing) => metrics.Clone());
        }
        catch (Exception ex)
        {
            LogHelper.Error($"更新增强实例指标时发生错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 标记实例为不可用（故障转移）
    /// </summary>
    public virtual void MarkInstanceUnavailable(long serverInstanceId)
    {
        try
        {
            _instanceAvailability[serverInstanceId] = false;
            LogHelper.Info($"标记服务实例 {serverInstanceId} 为不可用");
        }
        catch (Exception ex)
        {
            LogHelper.Error($"标记实例不可用时发生错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 标记实例为可用（故障恢复）
    /// </summary>
    public virtual void MarkInstanceAvailable(long serverInstanceId)
    {
        try
        {
            _instanceAvailability[serverInstanceId] = true;
            LogHelper.Info($"标记服务实例 {serverInstanceId} 为可用");
        }
        catch (Exception ex)
        {
            LogHelper.Error($"标记实例可用时发生错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 重置负载均衡器状态
    /// </summary>
    public virtual void Reset()
    {
        try
        {
            _instanceAvailability.Clear();
            _instanceMetrics.Clear();
            _statistics.Reset();
            LogHelper.Info($"{_strategy} 负载均衡器状态已重置");
        }
        catch (Exception ex)
        {
            LogHelper.Error($"重置负载均衡器状态时发生错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取负载均衡器统计信息
    /// </summary>
    public virtual LoadBalancerStatistics GetStatistics()
    {
        try
        {
            // 更新实例选择次数统计
            foreach (var kvp in _instanceMetrics)
            {
                _statistics.SelectionsByInstance[kvp.Key] = kvp.Value.TotalRequests;
            }

            _statistics.LastUpdated = DateTime.UtcNow;
            return _statistics;
        }
        catch (Exception ex)
        {
            LogHelper.Error($"获取统计信息时发生错误: {ex.Message}");
            return _statistics;
        }
    }

    /// <summary>
    /// 执行健康检查
    /// </summary>
    public virtual bool PerformHealthCheck()
    {
        try
        {
            // 检查是否有可用的实例
            var availableCount = _instanceAvailability.Values.Count(available => available);
            return availableCount > 0;
        }
        catch (Exception ex)
        {
            LogHelper.Error($"健康检查时发生错误: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 子类实现的具体选择算法
    /// </summary>
    /// <param name="serverType">服务器类型</param>
    /// <param name="availableInstances">可用实例列表</param>
    /// <param name="key">用于一致性哈希的键</param>
    /// <returns>选择的实例ID</returns>
    protected abstract long SelectInstanceInternal(string serverType, List<long> availableInstances, string key);

    /// <summary>
    /// 子类实现的增强选择算法
    /// </summary>
    /// <param name="context">负载均衡上下文</param>
    /// <param name="availableInstances">可用实例列表</param>
    /// <returns>选择结果</returns>
    protected virtual LoadBalancingSelection SelectInstanceEnhancedInternal(LoadBalancingContext context, List<long> availableInstances)
    {
        // 默认实现：调用基础选择方法
        var instanceId = SelectInstanceInternal(context.ServerType, availableInstances, context.AffinityKey);

        if (instanceId > 0)
        {
            return LoadBalancingSelection.Success(instanceId, SelectionReason.Fallback, 0, _stopwatch.ElapsedMilliseconds);
        }

        return LoadBalancingSelection.Failure("No suitable instance found");
    }

    /// <summary>
    /// 过滤可用实例
    /// </summary>
    protected virtual List<long> FilterAvailableInstances(List<long> instances)
    {
        return instances.Where(instanceId =>
            _instanceAvailability.TryGetValue(instanceId, out var available) && available).ToList();
    }

    /// <summary>
    /// 重置实例可用性状态
    /// </summary>
    protected virtual void ResetAvailability(List<long> instances)
    {
        foreach (var instance in instances)
        {
            _instanceAvailability.TryAdd(instance, true);
        }
    }

    /// <summary>
    /// 更新统计信息
    /// </summary>
    protected virtual void UpdateStatistics(bool success, long selectionTimeMs)
    {
        _statistics.TotalSelections++;

        if (success)
        {
            _statistics.SuccessfulSelections++;
        }
        else
        {
            _statistics.FailedSelections++;
        }

        // 更新选择时间统计
        UpdateSelectionTimeStatistics(selectionTimeMs);
    }

    /// <summary>
    /// 更新选择时间统计
    /// </summary>
    private void UpdateSelectionTimeStatistics(long selectionTimeMs)
    {
        if (selectionTimeMs < _statistics.MinSelectionTimeMs || _statistics.MinSelectionTimeMs == 0)
        {
            _statistics.MinSelectionTimeMs = selectionTimeMs;
        }

        if (selectionTimeMs > _statistics.MaxSelectionTimeMs)
        {
            _statistics.MaxSelectionTimeMs = selectionTimeMs;
        }

        // 使用指数移动平均计算平均时间
        const double alpha = 0.1;
        var newAverage = _statistics.AverageSelectionTimeMs * (1 - alpha) + selectionTimeMs * alpha;
        _statistics.AverageSelectionTimeMs = newAverage;
    }

    /// <summary>
    /// 更新实例选择次数
    /// </summary>
    protected virtual void UpdateInstanceSelectionCount(long instanceId)
    {
        var metrics = _instanceMetrics.GetOrAdd(instanceId, _ => new InstanceMetrics { InstanceId = instanceId });
        metrics.TotalRequests++;
        metrics.LastUpdated = DateTime.UtcNow;
    }

    /// <summary>
    /// 获取实例指标
    /// </summary>
    protected InstanceMetrics GetInstanceMetrics(long instanceId)
    {
        _instanceMetrics.TryGetValue(instanceId, out var metrics);
        return metrics;
    }

    /// <summary>
    /// 检查实例是否可用
    /// </summary>
    protected bool IsInstanceAvailable(long instanceId)
    {
        return _instanceAvailability.TryGetValue(instanceId, out var available) && available;
    }

    /// <summary>
    /// 计算实例分数（用于负载均衡决策）
    /// </summary>
    protected virtual double CalculateInstanceScore(long instanceId, LoadBalancingContext context = null)
    {
        var metrics = GetInstanceMetrics(instanceId);
        if (metrics == null)
        {
            return 1.0;
        }

        // 基础分数
        var score = 1.0;

        // 响应时间因子（响应时间越短分数越高）
        if (metrics.AverageResponseTimeMs > 0)
        {
            var responseTimeScore = Math.Max(0.1, 1000.0 / (metrics.AverageResponseTimeMs + 100.0));
            score *= responseTimeScore;
        }

        // 成功率因子（成功率越高分数越高）
        var successRateScore = metrics.SuccessRate / 100.0;
        score *= successRateScore;

        // 连接数因子（连接数适中最好）
        if (metrics.ActiveConnections > 0)
        {
            var connectionScore = Math.Max(0.1, 50.0 / metrics.ActiveConnections);
            score *= connectionScore;
        }

        // 资源使用率因子
        var resourceScore = Math.Max(0.1, 200 - (metrics.CpuUsage + metrics.MemoryUsage)) / 2;
        score *= resourceScore;

        return score;
    }

    }