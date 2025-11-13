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

using System.Collections.Concurrent;
using GameFrameX.DiscoveryCenterManager.Strategy.Interface;
using GameFrameX.DiscoveryCenterManager.Strategy.Abstractions;

namespace GameFrameX.DiscoveryCenterManager.Strategy;

/// <summary>
/// 负载均衡器工厂
/// </summary>
public static class LoadBalancerFactory
{
    private static readonly ConcurrentDictionary<LoadBalanceStrategy, ILoadBalancer> LoadBalancerCache = new();

    /// <summary>
    /// 创建负载均衡器
    /// </summary>
    /// <typeparam name="TLoadBalancer">负载均衡器类型</typeparam>
    /// <returns>负载均衡器实例</returns>
    public static ILoadBalancer Create<TLoadBalancer>() where TLoadBalancer : class, ILoadBalancer, new()
    {
        return Activator.CreateInstance<TLoadBalancer>();
    }

    /// <summary>
    /// 根据策略创建负载均衡器
    /// </summary>
    /// <param name="strategy">负载均衡策略</param>
    /// <returns>负载均衡器实例</returns>
    public static ILoadBalancer Create(LoadBalanceStrategy strategy)
    {
        return LoadBalancerCache.GetOrAdd(strategy, CreateLoadBalancer);
    }

    /// <summary>
    /// 创建增强型负载均衡器
    /// </summary>
    /// <param name="strategy">负载均衡策略</param>
    /// <returns>增强型负载均衡器实例</returns>
    public static IEnhancedLoadBalancer CreateEnhanced(LoadBalanceStrategy strategy)
    {
        var loadBalancer = Create(strategy);
        if (loadBalancer is IEnhancedLoadBalancer enhanced)
        {
            return enhanced;
        }

        // 如果不是增强型，包装成增强型
        return new EnhancedLoadBalancerWrapper(loadBalancer);
    }

    /// <summary>
    /// 获取支持的策略列表
    /// </summary>
    /// <returns>支持的策略列表</returns>
    public static List<LoadBalanceStrategy> GetSupportedStrategies()
    {
        return new List<LoadBalanceStrategy>
        {
            LoadBalanceStrategy.RoundRobin,
            LoadBalanceStrategy.Random,
            LoadBalanceStrategy.LeastConnections,
            LoadBalanceStrategy.ConsistentHash,
            LoadBalanceStrategy.WeightedRoundRobin,
            LoadBalanceStrategy.ResponseTime,
            LoadBalanceStrategy.Adaptive
        };
    }

    /// <summary>
    /// 创建负载均衡器
    /// </summary>
    /// <param name="strategy">负载均衡策略</param>
    /// <returns>负载均衡器实例</returns>
    private static ILoadBalancer CreateLoadBalancer(LoadBalanceStrategy strategy)
    {
        switch (strategy)
        {
            case LoadBalanceStrategy.RoundRobin:
                return new RoundRobinLoadBalancer();
            case LoadBalanceStrategy.Random:
                return new RandomLoadBalancer();
            case LoadBalanceStrategy.LeastConnections:
                return new LeastConnectionsLoadBalancer();
            case LoadBalanceStrategy.ConsistentHash:
                return new ConsistentHashLoadBalancer();
            case LoadBalanceStrategy.WeightedRoundRobin:
                return new WeightedRoundRobinLoadBalancer();
            case LoadBalanceStrategy.ResponseTime:
                return new ResponseTimeLoadBalancer();
            case LoadBalanceStrategy.Adaptive:
                return new AdaptiveLoadBalancer();
            default:
                return new RoundRobinLoadBalancer(); // 默认轮询
        }
    }

    /// <summary>
    /// 清除负载均衡器缓存
    /// </summary>
    public static void ClearCache()
    {
        LoadBalancerCache.Clear();
    }

    /// <summary>
    /// 预热缓存
    /// </summary>
    public static void WarmupCache()
    {
        var strategies = GetSupportedStrategies();
        foreach (var strategy in strategies)
        {
            _ = Create(strategy);
        }
    }

    /// <summary>
    /// 获取工厂统计信息
    /// </summary>
    /// <returns>工厂统计信息</returns>
    public static LoadBalancerFactoryStatistics GetStatistics()
    {
        return new LoadBalancerFactoryStatistics
        {
            CachedInstancesCount = LoadBalancerCache.Count,
            SupportedStrategies = GetSupportedStrategies(),
            CacheKeys = LoadBalancerCache.Keys.ToList()
        };
    }
}