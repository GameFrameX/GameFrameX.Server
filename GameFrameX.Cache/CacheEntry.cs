using GameFrameX.DBServer.State;

namespace GameFrameX.Cache;

/// <summary>
/// 缓存实体
/// </summary>
public class CacheEntry
{
    /// <summary>
    /// Key
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Value
    /// </summary>
    public ICacheState Value { get; set; }
    // 可以添加更多属性，例如过期时间等
}