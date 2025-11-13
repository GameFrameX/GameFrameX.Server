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

using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using GameFrameX.DiscoveryCenterManager.Strategy.Interface;
using GameFrameX.DiscoveryCenterManager.Strategy.Models;
using GameFrameX.DiscoveryCenterManager.Strategy.Abstractions;
using GameFrameX.Foundation.Logger;

namespace GameFrameX.DiscoveryCenterManager.Strategy;

/// <summary>
/// 一致性哈希负载均衡器（适用于需要会话亲和性的场景）
/// </summary>
public sealed class ConsistentHashLoadBalancer : BaseLoadBalancer
{
    public ConsistentHashLoadBalancer() : base(LoadBalanceStrategy.ConsistentHash)
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

        // 如果没有key，使用轮询
        if (string.IsNullOrEmpty(key))
        {
            var roundRobinIndex = (int)(_statistics.TotalSelections % activeInstances.Count);
            return activeInstances[roundRobinIndex];
        }

        // 使用一致性哈希
        var hash = ComputeHash(key);
        var index = Math.Abs(hash) % activeInstances.Count;
        return activeInstances[index];
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

        long selectedInstance;

        // 如果没有亲和性键，使用轮询
        if (string.IsNullOrEmpty(context.AffinityKey))
        {
            var roundRobinIndex = (int)(_statistics.TotalSelections % activeInstances.Count);
            selectedInstance = activeInstances[roundRobinIndex];
        }
        else
        {
            // 使用一致性哈希
            var hash = ComputeHash(context.AffinityKey);
            var index = Math.Abs(hash) % activeInstances.Count;
            selectedInstance = activeInstances[index];
        }

        if (selectedInstance > 0)
        {
            var score = CalculateInstanceScore(selectedInstance, context);
            var selectionTime = (long)System.Diagnostics.Stopwatch.GetTimestamp();

            return LoadBalancingSelection.Success(selectedInstance, SelectionReason.ConsistentHash, score, selectionTime);
        }

        return LoadBalancingSelection.Failure("ConsistentHash selection failed");
    }

    /// <summary>
    /// 计算哈希值
    /// </summary>
    private static int ComputeHash(string key)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
        return BitConverter.ToInt32(hashBytes, 0);
    }
}