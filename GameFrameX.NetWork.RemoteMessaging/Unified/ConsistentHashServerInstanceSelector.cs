// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   均受中华人民共和国及相关国际法律法规保护。
//   使用本项目须严格遵守相版权法律法规及开源许可证之规定。
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

using System.Collections.Concurrent;

namespace GameFrameX.NetWork.RemoteMessaging.Unified;

/// <summary>
/// 基于一致性哈希的服务实例选择器。支持虚拟节点，提供稳定的路由落点。
/// 当实例下线时，仅影响部分 key 的重映射。
/// </summary>
/// <remarks>
/// Consistent hash-based service instance selector. Supports virtual nodes for stable routing.
/// When an instance goes offline, only a portion of keys are remapped.
/// </remarks>
public sealed class ConsistentHashServerInstanceSelector : IServerInstanceSelector
{
    private const int DefaultVirtualNodeCount = 150;

    private readonly int _virtualNodeCount;
    private readonly ConcurrentDictionary<string, ServiceHashRing> _rings = new();
    private readonly ConcurrentDictionary<string, int> _roundRobinCounters = new();

    /// <summary>
    /// 初始化一致性哈希选择器。
    /// </summary>
    /// <remarks>
    /// Initializes the consistent hash selector.
    /// </remarks>
    /// <param name="virtualNodeCount">每个实例的虚拟节点数 / Virtual nodes per instance</param>
    public ConsistentHashServerInstanceSelector(int virtualNodeCount = DefaultVirtualNodeCount)
    {
        _virtualNodeCount = virtualNodeCount;
    }

    /// <inheritdoc />
    public InstanceSelection Select(string serviceName, string routeKey = null)
    {
        if (string.IsNullOrEmpty(serviceName))
        {
            return InstanceSelection.None(serviceName);
        }

        if (!_rings.TryGetValue(serviceName, out var ring) || ring.IsEmpty)
        {
            return InstanceSelection.None(serviceName);
        }

        // 有 routeKey 时使用一致性哈希
        if (!string.IsNullOrEmpty(routeKey))
        {
            var instanceId = ring.GetNode(routeKey);
            return InstanceSelection.Selected(serviceName, instanceId);
        }

        // 无 routeKey 时使用轮询
        var counter = _roundRobinCounters.AddOrUpdate(serviceName, 0, (_, v) => v + 1);
        var instances = ring.GetAllInstances();
        if (instances.Count == 0)
        {
            return InstanceSelection.None(serviceName);
        }

        var index = (int)(counter % instances.Count);
        return InstanceSelection.Selected(serviceName, instances[index]);
    }

    /// <inheritdoc />
    public void RefreshInstances(string serviceName, IReadOnlyList<string> instanceIds)
    {
        if (string.IsNullOrEmpty(serviceName))
        {
            return;
        }

        if (instanceIds == null || instanceIds.Count == 0)
        {
            _rings.TryRemove(serviceName, out _);
            _roundRobinCounters.TryRemove(serviceName, out _);
            return;
        }

        var ring = new ServiceHashRing(serviceName, instanceIds, _virtualNodeCount);
        _rings[serviceName] = ring;
    }

    /// <summary>
    /// 单个服务的哈希环。
    /// </summary>
    /// <remarks>
    /// Hash ring for a single service.
    /// </remarks>
    private sealed class ServiceHashRing
    {
        private readonly SortedList<long, string> _ring = new();
        private readonly List<string> _instances;
        private readonly string _serviceName;

        public ServiceHashRing(string serviceName, IReadOnlyList<string> instances, int virtualNodeCount)
        {
            _serviceName = serviceName;
            _instances = new List<string>(instances);

            foreach (var instance in instances)
            {
                for (int i = 0; i < virtualNodeCount; i++)
                {
                    var key = Hash($"{_serviceName}:{instance}:{i}");
                    _ring[key] = instance;
                }
            }
        }

        public bool IsEmpty
        {
            get { return _ring.Count == 0; }
        }

        public string GetNode(string routeKey)
        {
            if (_ring.Count == 0)
            {
                return null;
            }

            var hash = Hash(routeKey);

            // 查找第一个大于等于 hash 的节点
            var keys = _ring.Keys;
            int index = BinarySearchCeiling(keys, hash);

            // 如果超过末尾，环绕到第一个节点
            if (index >= keys.Count)
            {
                index = 0;
            }

            return _ring[keys[index]];
        }

        public List<string> GetAllInstances()
        {
            return _instances;
        }

        private static long Hash(string key)
        {
            // FNV-1a 哈希算法
            const ulong FnvOffsetBasis = 14695981039346656037UL;
            const ulong FnvPrime = 1099511628211UL;

            ulong hash = FnvOffsetBasis;
            for (int i = 0; i < key.Length; i++)
            {
                hash ^= (byte)key[i];
                hash *= FnvPrime;
            }

            return (long)(hash & 0x7FFFFFFFFFFFFFFFUL);
        }

        private static int BinarySearchCeiling(IList<long> keys, long value)
        {
            int lo = 0;
            int hi = keys.Count - 1;

            while (lo <= hi)
            {
                int mid = lo + (hi - lo) / 2;
                if (keys[mid] < value)
                {
                    lo = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }

            return lo;
        }
    }
}
