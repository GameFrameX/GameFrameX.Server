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
/// 自适应负载均衡器
/// 根据实时性能指标和负载情况动态选择最优的负载均衡策略
/// </summary>
public sealed class AdaptiveLoadBalancer : BaseLoadBalancer
{
    private readonly ConcurrentDictionary<string, LoadBalanceStrategy> _activeStrategies = new();
    private readonly ConcurrentDictionary<LoadBalanceStrategy, double> _strategyScores = new();
    private readonly ConcurrentDictionary<LoadBalanceStrategy, System.DateTime> _lastStrategyUpdate = new();
    private readonly AdaptiveConfiguration _config;

    public AdaptiveLoadBalancer() : base(LoadBalanceStrategy.Adaptive)
    {
        _config = new AdaptiveConfiguration();
        InitializeStrategyScores();
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
        var activeInstances = FilterAvailableInstances(availableInstances);
        if (activeInstances.Count == 0)
        {
            return 0;
        }

        // 如果只有一个实例，直接返回
        if (activeInstances.Count == 1)
        {
            return activeInstances[0];
        }

        // 使用自适应算法选择实例
        return SelectAdaptiveInstance(activeInstances, serverType, key);
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

        // 如果只有一个实例，直接返回
        if (activeInstances.Count == 1)
        {
            var instanceId = activeInstances[0];
            var score = CalculateInstanceScore(instanceId, context);
            var selectionTime = (long)System.Diagnostics.Stopwatch.GetTimestamp();

            return LoadBalancingSelection.Success(instanceId, SelectionReason.Adaptive, score, selectionTime);
        }

        // 使用自适应算法选择实例
        var adaptiveResult = SelectAdaptiveInstanceEnhanced(activeInstances, context);
        if (adaptiveResult.InstanceId > 0)
        {
            return LoadBalancingSelection.Success(adaptiveResult.InstanceId, SelectionReason.Adaptive, adaptiveResult.Score, adaptiveResult.SelectionTime);
        }

        return LoadBalancingSelection.Failure("Adaptive selection failed");
    }

    /// <summary>
    /// 自适应选择实例（基础实现）
    /// </summary>
    private long SelectAdaptiveInstance(List<long> instances, string serverType, string key)
    {
        // 确定当前最适合的策略
        var bestStrategy = DetermineBestStrategy(serverType, instances);
        _activeStrategies[serverType] = bestStrategy;

        // 使用最佳策略选择实例
        return ApplyStrategy(instances, bestStrategy, serverType, key);
    }

    /// <summary>
    /// 自适应选择实例（增强实现）
    /// </summary>
    private AdaptiveSelectionResult SelectAdaptiveInstanceEnhanced(List<long> instances, LoadBalancingContext context)
    {
        // 确定当前最适合的策略
        var bestStrategy = DetermineBestStrategyEnhanced(context.ServerType, instances, context);
        _activeStrategies[context.ServerType] = bestStrategy;

        // 使用最佳策略选择实例
        var instanceId = ApplyStrategy(instances, bestStrategy, context.ServerType, context.AffinityKey);
        var score = CalculateInstanceScore(instanceId, context);

        return new AdaptiveSelectionResult
        {
            InstanceId = instanceId,
            Strategy = bestStrategy,
            Score = score,
            SelectionTime = (long)System.Diagnostics.Stopwatch.GetTimestamp()
        };
    }

    /// <summary>
    /// 确定最佳策略（基础实现）
    /// </summary>
    private LoadBalanceStrategy DetermineBestStrategy(string serverType, List<long> instances)
    {
        UpdateStrategyScores(serverType, instances);

        var sortedStrategies = _strategyScores
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var strategy in sortedStrategies)
        {
            if (IsStrategyApplicable(strategy, instances))
            {
                return strategy;
            }
        }

        return LoadBalanceStrategy.RoundRobin; // 默认策略
    }

    /// <summary>
    /// 确定最佳策略（增强实现）
    /// </summary>
    private LoadBalanceStrategy DetermineBestStrategyEnhanced(string serverType, List<long> instances, LoadBalancingContext context)
    {
        UpdateStrategyScoresEnhanced(serverType, instances, context);

        var sortedStrategies = _strategyScores
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var strategy in sortedStrategies)
        {
            if (IsStrategyApplicableEnhanced(strategy, instances, context))
            {
                return strategy;
            }
        }

        return LoadBalanceStrategy.RoundRobin; // 默认策略
    }

    /// <summary>
    /// 更新策略分数（基础实现）
    /// </summary>
    private void UpdateStrategyScores(string serverType, List<long> instances)
    {
        var now = System.DateTime.UtcNow;
        var lastUpdate = _lastStrategyUpdate.GetOrAdd(LoadBalanceStrategy.RoundRobin, now);

        // 限制更新频率
        if ((now - lastUpdate).TotalSeconds < _config.StrategyUpdateIntervalSeconds)
        {
            return;
        }

        var instanceMetrics = instances.Select(GetInstanceMetrics).Where(m => m != null).ToList();
        if (!instanceMetrics.Any()) return;

        // 计算各项指标
        var avgResponseTime = instanceMetrics.Average(m => m.AverageResponseTimeMs);
        var totalConnections = instanceMetrics.Sum(m => m.ActiveConnections);
        var avgErrorRate = instanceMetrics.Average(m => m.ErrorRate);
        var instanceCount = instances.Count;

        // 轮询策略分数（适合稳定负载）
        var roundRobinScore = CalculateRoundRobinScore(avgResponseTime, avgErrorRate, instanceCount);

        // 随机策略分数（适合均匀分布）
        var randomScore = CalculateRandomScore(avgResponseTime, instanceCount);

        // 最少连接策略分数
        var leastConnectionsScore = CalculateLeastConnectionsScore(totalConnections, instanceCount, avgResponseTime);

        // 一致性哈希策略分数
        var consistentHashScore = CalculateConsistentHashScore(instanceCount, avgResponseTime);

        // 加权轮询策略分数
        var weightedScore = CalculateWeightedScore(avgErrorRate, instanceCount);

        // 响应时间策略分数
        var responseTimeScore = CalculateResponseTimeScore(avgResponseTime, avgErrorRate);

        // 更新分数
        _strategyScores[LoadBalanceStrategy.RoundRobin] = roundRobinScore;
        _strategyScores[LoadBalanceStrategy.Random] = randomScore;
        _strategyScores[LoadBalanceStrategy.LeastConnections] = leastConnectionsScore;
        _strategyScores[LoadBalanceStrategy.ConsistentHash] = consistentHashScore;
        _strategyScores[LoadBalanceStrategy.WeightedRoundRobin] = weightedScore;
        _strategyScores[LoadBalanceStrategy.ResponseTime] = responseTimeScore;

        _lastStrategyUpdate[LoadBalanceStrategy.RoundRobin] = now;
    }

    /// <summary>
    /// 更新策略分数（增强实现）
    /// </summary>
    private void UpdateStrategyScoresEnhanced(string serverType, List<long> instances, LoadBalancingContext context)
    {
        var now = System.DateTime.UtcNow;
        var lastUpdate = _lastStrategyUpdate.GetOrAdd(LoadBalanceStrategy.RoundRobin, now);

        // 限制更新频率
        if ((now - lastUpdate).TotalSeconds < _config.StrategyUpdateIntervalSeconds)
        {
            return;
        }

        var instanceMetrics = instances.Select(GetInstanceMetrics).Where(m => m != null).ToList();
        if (!instanceMetrics.Any()) return;

        // 计算各项指标
        var avgResponseTime = instanceMetrics.Average(m => m.AverageResponseTimeMs);
        var totalConnections = instanceMetrics.Sum(m => m.ActiveConnections);
        var avgErrorRate = instanceMetrics.Average(m => m.ErrorRate);
        var instanceCount = instances.Count;

        // 根据上下文计算特殊分数
        var requestPriorityBonus = context.Priority switch
        {
            RequestPriority.Critical => 0.2,
            RequestPriority.High => 0.1,
            RequestPriority.Low => -0.1,
            _ => 0
        };

        // 已移除地理位置支持
        var affinityBonus = !string.IsNullOrEmpty(context.AffinityKey) ? 0.1 : 0;

        // 计算各策略分数（包含上下文加成）
        _strategyScores[LoadBalanceStrategy.RoundRobin] = CalculateRoundRobinScore(avgResponseTime, avgErrorRate, instanceCount) + requestPriorityBonus;
        _strategyScores[LoadBalanceStrategy.Random] = CalculateRandomScore(avgResponseTime, instanceCount) + requestPriorityBonus;
        _strategyScores[LoadBalanceStrategy.LeastConnections] = CalculateLeastConnectionsScore(totalConnections, instanceCount, avgResponseTime) + requestPriorityBonus;
        _strategyScores[LoadBalanceStrategy.ConsistentHash] = CalculateConsistentHashScore(instanceCount, avgResponseTime) + affinityBonus;
        _strategyScores[LoadBalanceStrategy.WeightedRoundRobin] = CalculateWeightedScore(avgErrorRate, instanceCount) + requestPriorityBonus;
        _strategyScores[LoadBalanceStrategy.ResponseTime] = CalculateResponseTimeScore(avgResponseTime, avgErrorRate) + requestPriorityBonus;
        
        _lastStrategyUpdate[LoadBalanceStrategy.RoundRobin] = now;
    }

    /// <summary>
    /// 检查策略是否适用
    /// </summary>
    private bool IsStrategyApplicable(LoadBalanceStrategy strategy, List<long> instances)
    {
        return strategy switch
        {
            LoadBalanceStrategy.ConsistentHash => instances.Count >= 2,
            LoadBalanceStrategy.WeightedRoundRobin => instances.Count >= 2,
            LoadBalanceStrategy.LeastConnections => instances.Count >= 2,
            LoadBalanceStrategy.ResponseTime => instances.Any(id => GetInstanceMetrics(id)?.AverageResponseTimeMs > 0),
            _ => true
        };
    }

    /// <summary>
    /// 检查策略是否适用（增强实现）
    /// </summary>
    private bool IsStrategyApplicableEnhanced(LoadBalanceStrategy strategy, List<long> instances, LoadBalancingContext context)
    {
        return strategy switch
        {
            LoadBalanceStrategy.ConsistentHash => !string.IsNullOrEmpty(context.AffinityKey) && instances.Count >= 2,
            LoadBalanceStrategy.WeightedRoundRobin => instances.Count >= 2,
            LoadBalanceStrategy.LeastConnections => instances.Count >= 2,
            LoadBalanceStrategy.ResponseTime => instances.Any(id => GetInstanceMetrics(id)?.AverageResponseTimeMs > 0),
            LoadBalanceStrategy.Adaptive => true,
            _ => true
        };
    }

    /// <summary>
    /// 应用指定策略选择实例
    /// </summary>
    private long ApplyStrategy(List<long> instances, LoadBalanceStrategy strategy, string serverType, string key)
    {
        return strategy switch
        {
            LoadBalanceStrategy.RoundRobin => ApplyRoundRobinStrategy(instances),
            LoadBalanceStrategy.Random => ApplyRandomStrategy(instances),
            LoadBalanceStrategy.LeastConnections => ApplyLeastConnectionsStrategy(instances),
            LoadBalanceStrategy.ConsistentHash => ApplyConsistentHashStrategy(instances, key),
            LoadBalanceStrategy.WeightedRoundRobin => ApplyWeightedRoundRobinStrategy(instances),
            LoadBalanceStrategy.ResponseTime => ApplyResponseTimeStrategy(instances),
                        _ => ApplyRoundRobinStrategy(instances)
        };
    }

    // 各种策略的具体实现
    private long ApplyRoundRobinStrategy(List<long> instances)
    {
        var index = (int)(_statistics.TotalSelections % instances.Count);
        return instances[index];
    }

    private long ApplyRandomStrategy(List<long> instances)
    {
        var random = new System.Random();
        var index = random.Next(instances.Count);
        return instances[index];
    }

    private long ApplyLeastConnectionsStrategy(List<long> instances)
    {
        return instances
            .OrderBy(id => GetInstanceMetrics(id)?.ActiveConnections ?? 0)
            .First();
    }

    private long ApplyConsistentHashStrategy(List<long> instances, string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return ApplyRoundRobinStrategy(instances);
        }

        var hash = key.GetHashCode();
        var index = Math.Abs(hash) % instances.Count;
        return instances[index];
    }

    private long ApplyWeightedRoundRobinStrategy(List<long> instances)
    {
        // 简化的加权实现，基于性能指标
        return instances
            .OrderByDescending(id => GetInstanceMetrics(id)?.HealthScore ?? 0)
            .First();
    }

    private long ApplyResponseTimeStrategy(List<long> instances)
    {
        return instances
            .Where(id => GetInstanceMetrics(id)?.AverageResponseTimeMs > 0)
            .OrderBy(id => GetInstanceMetrics(id)?.AverageResponseTimeMs)
            .DefaultIfEmpty(instances.First())
            .First();
    }

    
    // 策略分数计算方法
    private double CalculateRoundRobinScore(double avgResponseTime, double errorRate, int instanceCount)
    {
        var responseTimeScore = Math.Max(0.1, 1000.0 / (avgResponseTime + 100.0));
        var errorScore = Math.Max(0.1, (100 - errorRate) / 100.0);
        var instanceScore = Math.Min(1.0, instanceCount / 10.0);

        return (responseTimeScore + errorScore + instanceScore) / 3.0;
    }

    private double CalculateRandomScore(double avgResponseTime, int instanceCount)
    {
        var responseTimeScore = Math.Max(0.1, 500.0 / (avgResponseTime + 100.0));
        var instanceScore = Math.Min(1.0, instanceCount / 15.0);

        return (responseTimeScore + instanceScore) / 2.0;
    }

    private double CalculateLeastConnectionsScore(double totalConnections, int instanceCount, double avgResponseTime)
    {
        var connectionScore = Math.Max(0.1, 100.0 / (totalConnections / instanceCount + 1.0));
        var responseTimeScore = Math.Max(0.1, 800.0 / (avgResponseTime + 100.0));

        return (connectionScore + responseTimeScore) / 2.0;
    }

    private double CalculateConsistentHashScore(int instanceCount, double avgResponseTime)
    {
        var instanceScore = Math.Min(1.0, instanceCount / 5.0);
        var responseTimeScore = Math.Max(0.1, 600.0 / (avgResponseTime + 100.0));

        return (instanceScore + responseTimeScore) / 2.0;
    }

    private double CalculateWeightedScore(double errorRate, int instanceCount)
    {
        var errorScore = Math.Max(0.1, (100 - errorRate) / 100.0);
        var instanceScore = Math.Min(1.0, instanceCount / 8.0);

        return (errorScore + instanceScore) / 2.0;
    }

    private double CalculateResponseTimeScore(double avgResponseTime, double errorRate)
    {
        var responseTimeScore = Math.Max(0.1, 1000.0 / (avgResponseTime + 50.0));
        var errorScore = Math.Max(0.1, (100 - errorRate) / 100.0);

        return (responseTimeScore + errorScore) / 2.0;
    }

    
    /// <summary>
    /// 初始化策略分数
    /// </summary>
    private void InitializeStrategyScores()
    {
        var strategies = new[]
        {
            LoadBalanceStrategy.RoundRobin,
            LoadBalanceStrategy.Random,
            LoadBalanceStrategy.LeastConnections,
            LoadBalanceStrategy.ConsistentHash,
            LoadBalanceStrategy.WeightedRoundRobin,
            LoadBalanceStrategy.ResponseTime,
            LoadBalanceStrategy.Adaptive
        };

        foreach (var strategy in strategies)
        {
            _strategyScores[strategy] = 1.0;
            _lastStrategyUpdate[strategy] = System.DateTime.UtcNow;
        }
    }

    /// <summary>
    /// 获取自适应统计信息
    /// </summary>
    public AdaptiveStatistics GetAdaptiveStatistics()
    {
        return new AdaptiveStatistics
        {
            ActiveStrategies = _activeStrategies.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            StrategyScores = _strategyScores.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            LastStrategyUpdates = _lastStrategyUpdate.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            Configuration = _config.Clone()
        };
    }

    /// <summary>
    /// 更新自适应配置
    /// </summary>
    public void UpdateConfiguration(AdaptiveConfiguration config)
    {
        if (config != null)
        {
            _config.StrategyUpdateIntervalSeconds = config.StrategyUpdateIntervalSeconds;
            _config.EnableContextAwareScoring = config.EnableContextAwareScoring;
            _config.MinimumInstanceCount = config.MinimumInstanceCount;

            LogHelper.Info("自适应负载均衡器配置已更新");
        }
    }

    /// <summary>
    /// 重置负载均衡器状态
    /// </summary>
    public override void Reset()
    {
        base.Reset();
        _activeStrategies.Clear();
        _strategyScores.Clear();
        _lastStrategyUpdate.Clear();
        InitializeStrategyScores();
    }
}

/// <summary>
/// 自适应配置
/// </summary>
public class AdaptiveConfiguration
{
    /// <summary>
    /// 策略更新间隔（秒）
    /// </summary>
    public int StrategyUpdateIntervalSeconds { get; set; } = 30;

    /// <summary>
    /// 启用上下文感知评分
    /// </summary>
    public bool EnableContextAwareScoring { get; set; } = true;

    /// <summary>
    /// 最小实例数量
    /// </summary>
    public int MinimumInstanceCount { get; set; } = 2;

    /// <summary>
    /// 分数平滑因子
    /// </summary>
    public double ScoreSmoothingFactor { get; set; } = 0.1;

    public AdaptiveConfiguration Clone()
    {
        return new AdaptiveConfiguration
        {
            StrategyUpdateIntervalSeconds = StrategyUpdateIntervalSeconds,
            EnableContextAwareScoring = EnableContextAwareScoring,
            MinimumInstanceCount = MinimumInstanceCount,
            ScoreSmoothingFactor = ScoreSmoothingFactor
        };
    }
}

/// <summary>
/// 自适应选择结果
/// </summary>
internal class AdaptiveSelectionResult
{
    public long InstanceId { get; set; }
    public LoadBalanceStrategy Strategy { get; set; }
    public double Score { get; set; }
    public long SelectionTime { get; set; }
}

/// <summary>
/// 自适应统计信息
/// </summary>
public class AdaptiveStatistics
{
    /// <summary>
    /// 活跃策略
    /// </summary>
    public Dictionary<string, LoadBalanceStrategy> ActiveStrategies { get; set; } = new();

    /// <summary>
    /// 策略分数
    /// </summary>
    public Dictionary<LoadBalanceStrategy, double> StrategyScores { get; set; } = new();

    /// <summary>
    /// 最后策略更新时间
    /// </summary>
    public Dictionary<LoadBalanceStrategy, System.DateTime> LastStrategyUpdates { get; set; } = new();

    /// <summary>
    /// 配置
    /// </summary>
    public AdaptiveConfiguration Configuration { get; set; } = new();

    /// <summary>
    /// 最优策略
    /// </summary>
    public LoadBalanceStrategy BestStrategy => StrategyScores.OrderByDescending(kvp => kvp.Value).First().Key;

    public override string ToString()
    {
        return $"AdaptiveStats: Best={BestStrategy}, Strategies={StrategyScores.Count}";
    }
}