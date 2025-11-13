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
//  Gitee Repository: https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using System.Collections.Concurrent;
using System.Linq;
using GameFrameX.DiscoveryCenterManager.Strategy.Interface;
using GameFrameX.DiscoveryCenterManager.Strategy.Models;
using GameFrameX.DiscoveryCenterManager.Strategy.Abstractions;
using GameFrameX.Foundation.Logger;

namespace GameFrameX.DiscoveryCenterManager.Strategy;

/// <summary>
/// 加权轮询负载均衡器
/// 根据实例权重分配请求，权重越高获得越多请求
/// </summary>
public sealed class WeightedRoundRobinLoadBalancer : BaseLoadBalancer
{
    private readonly ConcurrentDictionary<string, int> _counters = new();
    private readonly ConcurrentDictionary<string, int> _currentWeights = new();
    private readonly ConcurrentDictionary<long, int> _instanceWeights = new();
    private readonly ConcurrentDictionary<long, int> _effectiveWeights = new();

    public WeightedRoundRobinLoadBalancer() : base(LoadBalanceStrategy.WeightedRoundRobin)
    {
    }

    /// <summary>
    /// 选择一个服务器实例（基础实现）
    /// </summary>
    protected override long SelectInstanceInternal(string serverType, List<long> availableInstances, string key = null)
    {
        if (availableInstances == null || availableInstances.Count == 0)
        {
            return 0;
        }

        // 过滤出可用的实例
        var activeInstances = availableInstances.Where(id => _instanceAvailability.TryGetValue(id, out var available) && available).ToList();

        if (activeInstances.Count == 0)
        {
            return 0;
        }

        // 使用加权轮询算法选择实例
        return SelectWeightedInstance(activeInstances, serverType);
    }

    /// <summary>
    /// 增强的实例选择方法
    /// </summary>
    protected override LoadBalancingSelection SelectInstanceEnhancedInternal(LoadBalancingContext context, List<long> availableInstances)
    {
        if (context?.AvailableInstances == null || context.AvailableInstances.Count == 0)
        {
            return LoadBalancingSelection.Failure("No available instances");
        }

        // 过滤出可用的实例
        var activeInstances = context.AvailableInstances.Where(id => _instanceAvailability.TryGetValue(id, out var available) && available).ToList();

        if (activeInstances.Count == 0)
        {
            return LoadBalancingSelection.Failure("No available instances");
        }

        // 使用加权轮询算法选择实例
        var selectedInstanceId = SelectWeightedInstance(activeInstances, context.ServerType);

        if (selectedInstanceId > 0)
        {
            var selectionTime = (long)System.Diagnostics.Stopwatch.GetTimestamp();
            var score = CalculateInstanceScore(selectedInstanceId, context);

            return LoadBalancingSelection.Success(selectedInstanceId, SelectionReason.Weight, score, selectionTime);
        }

        return LoadBalancingSelection.Failure("Weighted selection failed");
    }

    /// <summary>
    /// 使用加权轮询算法选择实例
    /// </summary>
    private long SelectWeightedInstance(List<long> instances, string serverType)
    {
        if (instances.Count == 0)
            return 0;

        if (instances.Count == 1)
            return instances[0];

        var totalWeight = 0;
        var bestInstance = 0L;
        var bestCurrentWeight = -1;

        // 初始化权重（如果还没有设置）
        InitializeInstanceWeights(instances);

        // 计算总权重并找到当前权重最高的实例
        foreach (var instanceId in instances)
        {
            var weight = CalculateEffectiveWeight(instanceId);
            var currentWeight = _currentWeights.GetOrAdd(instanceId.ToString(), 0) + weight;
            _currentWeights[instanceId.ToString()] = currentWeight;

            totalWeight += weight;

            if (currentWeight > bestCurrentWeight)
            {
                bestInstance = instanceId;
                bestCurrentWeight = currentWeight;
            }
        }

        // 减少所有实例的当前权重
        if (totalWeight > 0 && bestInstance > 0)
        {
            foreach (var instanceId in instances)
            {
                var key = instanceId.ToString();
                _currentWeights[key] -= totalWeight;
            }
        }

        return bestInstance;
    }

    /// <summary>
    /// 初始化实例权重
    /// </summary>
    private void InitializeInstanceWeights(List<long> instances)
    {
        foreach (var instanceId in instances)
        {
            _instanceWeights.TryAdd(instanceId, 1); // 默认权重为1
            _effectiveWeights.TryAdd(instanceId, 1);
        }
    }

    /// <summary>
    /// 获取实例的有效权重（内部计算）
    /// </summary>
    private int CalculateEffectiveWeight(long instanceId)
    {
        var baseWeight = _instanceWeights.TryGetValue(instanceId, out var weight) ? weight : 1;
        var effectiveWeight = _effectiveWeights.TryGetValue(instanceId, out var effective) ? effective : baseWeight;

        // 根据实例健康状态调整有效权重
        var healthMultiplier = CalculateHealthMultiplier(instanceId);

        // 根据实例性能指标调整权重
        var performanceMultiplier = CalculatePerformanceMultiplier(instanceId);

        effectiveWeight = (int)(baseWeight * healthMultiplier * performanceMultiplier);
        effectiveWeight = Math.Max(1, effectiveWeight); // 确保最小权重为1

        _effectiveWeights[instanceId] = effectiveWeight;

        return effectiveWeight;
    }

    /// <summary>
    /// 计算健康状态乘数
    /// </summary>
    private double CalculateHealthMultiplier(long instanceId)
    {
        var metrics = GetInstanceMetrics(instanceId);
        if (metrics == null) return 1.0;

        // 基于成功率的健康乘数
        var successRate = metrics.SuccessRate;
        if (successRate >= 95) return 1.0;          // 优秀
        if (successRate >= 90) return 0.8;          // 良好
        if (successRate >= 80) return 0.6;          // 一般
        if (successRate >= 70) return 0.4;          // 较差
        return 0.2;                                      // 很差
    }

    /// <summary>
    /// 计算性能乘数
    /// </summary>
    private double CalculatePerformanceMultiplier(long instanceId)
    {
        var metrics = GetInstanceMetrics(instanceId);
        if (metrics == null) return 1.0;

        var multiplier = 1.0;

        // 响应时间因子（响应时间越短，乘数越高）
        if (metrics.AverageResponseTimeMs > 0)
        {
            var responseTimeFactor = Math.Max(0.1, 1000.0 / (metrics.AverageResponseTimeMs + 100.0));
            multiplier *= responseTimeFactor;
        }

        // 连接数因子（连接数适中最好）
        if (metrics.ActiveConnections > 0)
        {
            var connectionFactor = Math.Max(0.1, 50.0 / Math.Max(1, metrics.ActiveConnections));
            multiplier *= connectionFactor;
        }

        // 资源使用率因子（使用率越低，乘数越高）
        var resourceUsage = (metrics.CpuUsage + metrics.MemoryUsage) / 2;
        var resourceFactor = Math.Max(0.1, (100 - resourceUsage) / 100);
        multiplier *= resourceFactor;

        return Math.Max(0.1, multiplier);
    }

    /// <summary>
    /// 更新实例权重或连接数
    /// </summary>
    public override void UpdateInstanceMetrics(long serverInstanceId, int connections = 0)
    {
        base.UpdateInstanceMetrics(serverInstanceId, connections);

        // 可以根据连接数动态调整权重
        AdjustWeightBasedOnConnections(serverInstanceId, connections);
    }

    /// <summary>
    /// 根据连接数动态调整权重
    /// </summary>
    private void AdjustWeightBasedOnConnections(long instanceId, int connections)
    {
        // 这里可以实现动态权重调整逻辑
        // 例如：连接数过多时降低权重，连接数过少时提高权重

        // 暂时保持原有权重不变
        // 可以根据实际需求实现具体的调整逻辑
    }

    /// <summary>
    /// 设置实例权重
    /// </summary>
    public void SetInstanceWeight(long instanceId, int weight)
    {
        if (weight <= 0)
        {
            weight = 1; // 确保最小权重为1
        }

        _instanceWeights[instanceId] = weight;

        // 同时更新有效权重
        var healthMultiplier = CalculateHealthMultiplier(instanceId);
        var performanceMultiplier = CalculatePerformanceMultiplier(instanceId);
        var effectiveWeight = (int)(weight * healthMultiplier * performanceMultiplier);
        _effectiveWeights[instanceId] = Math.Max(1, effectiveWeight);

        LogHelper.Info($"设置实例 {instanceId} 权重为 {weight}，有效权重为 {effectiveWeight}");
    }

    /// <summary>
    /// 获取实例权重
    /// </summary>
    public int GetInstanceWeight(long instanceId)
    {
        return _instanceWeights.TryGetValue(instanceId, out var weight) ? weight : 1;
    }

    /// <summary>
    /// 获取实例有效权重
    /// </summary>
    public int GetEffectiveWeight(long instanceId)
    {
        return _effectiveWeights.TryGetValue(instanceId, out var weight) ? weight : 1;
    }

    /// <summary>
    /// 重置权重状态
    /// </summary>
    public void ResetWeights()
    {
        _counters.Clear();
        _currentWeights.Clear();
        _instanceWeights.Clear();
        _effectiveWeights.Clear();
        LogHelper.Info("加权轮询负载均衡器权重状态已重置");
    }

    /// <summary>
    /// 获取权重统计信息
    /// </summary>
    public WeightStatistics GetWeightStatistics()
    {
        return new WeightStatistics
        {
            TotalInstances = _instanceWeights.Count,
            TotalWeight = _instanceWeights.Values.Sum(),
            EffectiveTotalWeight = _effectiveWeights.Values.Sum(),
            InstanceWeights = _instanceWeights.ToDictionary(pair => pair.Key, pair => pair.Value),
            EffectiveWeights = _effectiveWeights.ToDictionary(pair => pair.Key, pair => pair.Value)
        };
    }

    /// <summary>
    /// 重置负载均衡器状态
    /// </summary>
    public override void Reset()
    {
        base.Reset();
        ResetWeights();
    }
}

/// <summary>
/// 权重统计信息
/// </summary>
public class WeightStatistics
{
    /// <summary>
    /// 总实例数
    /// </summary>
    public int TotalInstances { get; set; }

    /// <summary>
    /// 总权重
    /// </summary>
    public int TotalWeight { get; set; }

    /// <summary>
    /// 有效总权重
    /// </summary>
    public int EffectiveTotalWeight { get; set; }

    /// <summary>
    /// 实例权重字典
    /// </summary>
    public Dictionary<long, int> InstanceWeights { get; set; } = new();

    /// <summary>
    /// 实例有效权重字典
    /// </summary>
    public Dictionary<long, int> EffectiveWeights { get; set; } = new();

    /// <summary>
    /// 平均权重
    /// </summary>
    public double AverageWeight => TotalInstances > 0 ? (double)TotalWeight / TotalInstances : 0;

    /// <summary>
    /// 平均有效权重
    /// </summary>
    public double AverageEffectiveWeight => TotalInstances > 0 ? (double)EffectiveTotalWeight / TotalInstances : 0;

    public override string ToString()
    {
        return $"WeightStats: {TotalInstances} instances, Avg Weight: {AverageWeight:F2}, Effective Avg Weight: {AverageEffectiveWeight:F2}";
    }
}