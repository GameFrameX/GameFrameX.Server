namespace GameFrameX.Cache;

/// <summary>
/// 缓存配置
/// </summary>
public class CacheConfig
{
    /// <summary>
    /// 缓存大小
    /// </summary>
    public int CacheSize { get; set; }

    /// <summary>
    /// 默认过期时间
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; }

    /// <summary>
    /// Redis连接字符串
    /// </summary>
    public string RedisConnectionString { get; set; }
}