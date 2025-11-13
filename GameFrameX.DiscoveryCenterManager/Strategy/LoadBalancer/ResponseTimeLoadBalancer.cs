// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
// 均受中华人民共和国及相关国际法律法规保护。
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

using System.Collections.Generic;
using System.Linq;
using GameFrameX.DiscoveryCenterManager.Strategy.Interface;
using GameFrameX.DiscoveryCenterManager.Strategy.Models;
using GameFrameX.DiscoveryCenterManager.Strategy.Abstractions;

namespace GameFrameX.DiscoveryCenterManager.Strategy;

/// <summary>
/// 响应时间优化负载均衡器
/// 选择平均响应时间最短的实例
/// </summary>
public sealed class ResponseTimeLoadBalancer : BaseLoadBalancer
{
    private readonly Dictionary<long, System.DateTime> _lastSelectionTime = new();

    public ResponseTimeLoadBalancer() : base(LoadBalanceStrategy.ResponseTime)
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

        // 选择响应时间最短的实例
        return SelectFastestInstance(activeInstances);
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

            return LoadBalancingSelection.Success(instanceId, SelectionReason.ResponseTime, score, selectionTime);
        }

        // 选择响应时间最优的实例
        var selectedInstanceId = SelectFastestInstanceEnhanced(activeInstances, context);

        if (selectedInstanceId > 0)
        {
            var score = CalculateInstanceScore(selectedInstanceId, context);
            var selectionTime = (long)System.Diagnostics.Stopwatch.GetTimestamp();

            return LoadBalancingSelection.Success(selectedInstanceId, SelectionReason.ResponseTime, score, selectionTime);
        }

        return LoadBalancingSelection.Failure("No suitable instance found");
    }

    /// <summary>
    /// 选择响应时间最快的实例（基础实现）
    /// </summary>
    private long SelectFastestInstance(List<long> instances)
    {
        return instances
            .OrderBy(instanceId => GetInstanceResponseTime(instanceId))
            .First();
    }

    /// <summary>
    /// 选择响应时间最快的实例（增强实现）
    /// </summary>
    private long SelectFastestInstanceEnhanced(List<long> instances, LoadBalancingContext context)
    {
        var scoredInstances = instances
            .Select(instanceId => new
            {
                InstanceId = instanceId,
                Score = CalculateInstanceScore(instanceId, context)
            })
            .OrderByDescending(x => x.Score)
            .ToList();

        return scoredInstances.First().InstanceId;
    }

    /// <summary>
    /// 获取实例响应时间
    /// </summary>
    private double GetInstanceResponseTime(long instanceId)
    {
        var metrics = GetInstanceMetrics(instanceId);
        if (metrics?.AverageResponseTimeMs > 0)
        {
            return metrics.AverageResponseTimeMs;
        }

        // 如果没有历史数据，使用最后选择时间作为估算
        if (_lastSelectionTime.TryGetValue(instanceId, out var lastTime))
        {
            var timeSinceLastSelection = System.DateTime.UtcNow - lastTime;
            // 如果很久没有选择，可能实例已经"冷却"，给予更好的响应时间估算
            if (timeSinceLastSelection.TotalMinutes > 5)
            {
                return Math.Max(1, 100 - (int)timeSinceLastSelection.TotalMinutes);
            }
        }

        return 100.0; // 默认响应时间（毫秒）
    }

    /// <summary>
    /// 计算实例综合分数
    /// </summary>
    protected override double CalculateInstanceScore(long instanceId, LoadBalancingContext? context = null)
    {
        var metrics = GetInstanceMetrics(instanceId);
        if (metrics == null) return 1.0;

        // 基础分数
        var score = base.CalculateInstanceScore(instanceId, context);

        // 响应时间分数（响应时间越短分数越高）
        var responseTimeScore = CalculateResponseTimeScore(metrics.AverageResponseTimeMs);
        score *= responseTimeScore;

        // P95响应时间权重（如果有的话）
        if (metrics.AverageResponseTimeMs > 0)
        {
            var p95Score = Math.Max(0.1, 1000.0 / (metrics.AverageResponseTimeMs + 100.0));
            score *= p95Score;
        }

        // 最后选择时间权重（避免重复选择同一实例）
        var timeSinceLastSelection = GetTimeSinceLastSelection(instanceId);
        var recencyScore = Math.Min(1.0, timeSinceLastSelection.TotalSeconds / 60.0 + 0.1); // 60秒内逐渐降低权重
        score *= recencyScore;

        return score;
    }

    /// <summary>
    /// 计算响应时间分数
    /// </summary>
    private static double CalculateResponseTimeScore(double responseTimeMs)
    {
        if (responseTimeMs <= 0) return 1.0;

        // 使用反比函数，响应时间越短分数越高
        // 同时考虑性能基准
        const double baselineResponseTime = 100.0; // 100ms作为基准

        if (responseTimeMs <= baselineResponseTime)
        {
            return 1.0;
        }
        else if (responseTimeMs <= baselineResponseTime * 2)
        {
            return baselineResponseTime / responseTimeMs;
        }
        else if (responseTimeMs <= baselineResponseTime * 5)
        {
            return (baselineResponseTime * 2) / responseTimeMs;
        }
        else
        {
            return Math.Max(0.1, (baselineResponseTime * 5) / responseTimeMs);
        }
    }

    /// <summary>
    /// 获取距离上次选择的时间
    /// </summary>
    private System.TimeSpan GetTimeSinceLastSelection(long instanceId)
    {
        if (_lastSelectionTime.TryGetValue(instanceId, out var lastTime))
        {
            return System.DateTime.UtcNow - lastTime;
        }
        return System.TimeSpan.FromMinutes(10); // 默认假设10分钟前选择过
    }

    /// <summary>
    /// 更新实例权重或连接数
    /// </summary>
    public override void UpdateInstanceMetrics(long serverInstanceId, int connections = 0)
    {
        base.UpdateInstanceMetrics(serverInstanceId, connections);

        // 更新最后选择时间
        _lastSelectionTime[serverInstanceId] = System.DateTime.UtcNow;
    }

    /// <summary>
    /// 更新增强的实例指标
    /// </summary>
    public override void UpdateInstanceMetricsEnhanced(InstanceMetrics metrics)
    {
        base.UpdateInstanceMetricsEnhanced(metrics);

        if (metrics != null)
        {
            _lastSelectionTime[metrics.InstanceId] = System.DateTime.UtcNow;
        }
    }

    /// <summary>
    /// 重置负载均衡器状态
    /// </summary>
    public override void Reset()
    {
        base.Reset();
        _lastSelectionTime.Clear();
    }

    /// <summary>
    /// 获取响应时间统计信息
    /// </summary>
    public ResponseTimeStatistics GetResponseTimeStatistics()
    {
        var stats = new ResponseTimeStatistics();

        // 统计实例的响应时间
        var responseTimes = _instanceMetrics.Values
            .Where(m => m.AverageResponseTimeMs > 0)
            .Select(m => m.AverageResponseTimeMs)
            .ToList();

        if (responseTimes.Any())
        {
            stats.AverageResponseTime = responseTimes.Average();
            stats.MinResponseTime = responseTimes.Min();
            stats.MaxResponseTime = responseTimes.Max();
            stats.P95ResponseTime = CalculatePercentile(responseTimes, 0.95);
            stats.P99ResponseTime = CalculatePercentile(responseTimes, 0.99);
        }

        // 统计选择频率
        var selectionFrequencies = _lastSelectionTime
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value
            );

        stats.LastSelectionTimes = selectionFrequencies;

        // 计算冷/热实例比例
        var now = System.DateTime.UtcNow;
        var coldThreshold = now.AddMinutes(-5); // 5分钟未选择为冷实例
        var hotThreshold = now.AddSeconds(-30); // 30秒内选择过为热实例

        stats.ColdInstanceCount = selectionFrequencies.Values.Count(timeValue => timeValue < coldThreshold);
        stats.HotInstanceCount = selectionFrequencies.Values.Count(timeValue => timeValue > hotThreshold);
        stats.TotalInstanceCount = selectionFrequencies.Count;

        return stats;
    }

    /// <summary>
    /// 计算百分位数
    /// </summary>
    private static double CalculatePercentile(List<double> sortedValues, double percentile)
    {
        if (!sortedValues.Any()) return 0;

        var sortedList = sortedValues.OrderBy(x => x).ToList();
        var index = (int)Math.Ceiling(sortedList.Count * percentile);
        return sortedList[Math.Min(index, sortedList.Count - 1)];
    }
}

/// <summary>
/// 响应时间统计信息
/// </summary>
public class ResponseTimeStatistics
{
    /// <summary>
    /// 平均响应时间（毫秒）
    /// </summary>
    public double AverageResponseTime { get; set; }

    /// <summary>
    /// 最小响应时间（毫秒）
    /// </summary>
    public double MinResponseTime { get; set; }

    /// <summary>
    /// 最大响应时间（毫秒）
    /// </summary>
    public double MaxResponseTime { get; set; }

    /// <summary>
    /// P95响应时间（毫秒）
    /// </summary>
    public double P95ResponseTime { get; set; }

    /// <summary>
    /// P99响应时间（毫秒）
    /// </summary>
    public double P99ResponseTime { get; set; }

    /// <summary>
    /// 冷实例数（5分钟未选择）
    /// </summary>
    public int ColdInstanceCount { get; set; }

    /// <summary>
    /// 热实例数（30秒内选择过）
    /// </summary>
    public int HotInstanceCount { get; set; }

    /// <summary>
    /// 总实例数
    /// </summary>
    public int TotalInstanceCount { get; set; }

    /// <summary>
    /// 最后选择时间字典
    /// </summary>
    public Dictionary<long, System.DateTime> LastSelectionTimes { get; set; } = new();

    /// <summary>
    /// 冷实例比例
    /// </summary>
    public double ColdInstanceRatio => TotalInstanceCount > 0 ? (double)ColdInstanceCount / TotalInstanceCount : 0;

    /// <summary>
    /// 热实例比例
    /// </summary>
    public double HotInstanceRatio => TotalInstanceCount > 0 ? (double)HotInstanceCount / TotalInstanceCount : 0;

    public override string ToString()
    {
        return $"ResponseTimeStats: Avg={AverageResponseTime:F2}ms, P95={P95ResponseTime:F2}ms, P99={P99ResponseTime:F2}ms";
    }
}