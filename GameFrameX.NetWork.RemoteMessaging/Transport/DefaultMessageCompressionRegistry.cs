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

using System.Collections.Concurrent;

namespace GameFrameX.NetWork.RemoteMessaging.Transport;

/// <summary>
/// 默认消息压缩算法注册表。
/// </summary>
/// <remarks>
/// Default message compression algorithm registry.
/// </remarks>
public sealed class DefaultMessageCompressionRegistry : IMessageCompressionRegistry
{
    private readonly ConcurrentDictionary<byte, IMessageCompressionAlgorithm> _algorithms = new();

    /// <summary>
    /// 初始化新的默认消息压缩算法注册表，并注册内置的 Deflate 算法。
    /// </summary>
    /// <remarks>
    /// Initializes a new default message compression registry and registers the built-in Deflate algorithm.
    /// </remarks>
    public DefaultMessageCompressionRegistry()
    {
        Register(new DeflateMessageCompressionAlgorithm());
    }

    /// <summary>
    /// 注册一个压缩算法实现。算法 ID 为 0 保留给未压缩载荷，不允许注册。
    /// </summary>
    /// <remarks>
    /// Registers a compression algorithm implementation. Algorithm ID 0 is reserved for uncompressed payloads and cannot be registered.
    /// </remarks>
    /// <param name="algorithm">要注册的压缩算法实现 / The compression algorithm implementation to register</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="algorithm"/> 为 null 时抛出 / Thrown when <paramref name="algorithm"/> is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="algorithm"/> 的 AlgorithmId 为 0 时抛出 / Thrown when <paramref name="algorithm"/> has AlgorithmId of 0</exception>
    public void Register(IMessageCompressionAlgorithm algorithm)
    {
        ArgumentNullException.ThrowIfNull(algorithm);
        if (algorithm.AlgorithmId == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(algorithm), "AlgorithmId 0 is reserved for uncompressed payload.");
        }

        _algorithms[algorithm.AlgorithmId] = algorithm;
    }

    /// <summary>
    /// 根据算法 ID 查找已注册的压缩算法实现。
    /// </summary>
    /// <remarks>
    /// Looks up a registered compression algorithm implementation by its algorithm ID.
    /// </remarks>
    /// <param name="algorithmId">算法 ID / The algorithm ID</param>
    /// <param name="algorithm">找到的算法实例；未找到时为 null / The found algorithm instance; null if not found</param>
    /// <returns>如果找到匹配的算法则返回 <c>true</c>；否则返回 <c>false</c> / <c>true</c> if a matching algorithm is found; otherwise <c>false</c></returns>
    public bool TryGet(byte algorithmId, out IMessageCompressionAlgorithm algorithm)
    {
        return _algorithms.TryGetValue(algorithmId, out algorithm);
    }
}
