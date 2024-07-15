using GameFrameX.DBServer.State;

namespace GameFrameX.Cache;

/// <summary>
/// 缓存接口定义
/// </summary>
public interface ICache
{
    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    void Set(string key, CacheState value);

    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    object Get(string key);

    /// <summary>
    /// 是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGet(string key, out CacheState value);

    /// <summary>
    /// 移除
    /// </summary>
    /// <param name="key"></param>
    void Remove(string key);

    /// <summary>
    /// 是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Contains(string key);

    /// <summary>
    /// 清除
    /// </summary>
    void Flush(); // 清除所有缓存

    /// <summary>
    /// 刷新
    /// </summary>
    /// <param name="key"></param>
    void Refresh(string key); // 刷新过期策略，而不移除缓存


    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Task SetAsync(long key, CacheState value);

    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Task SetAsync(string key, CacheState value);

    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<CacheState> GetAsync(string key);

    /// <summary>
    /// 尝试获取
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Task<bool> TryGetAsync(string key, out CacheState value);

    /// <summary>
    /// 移除
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task RemoveAsync(string key);

    /// <summary>
    /// 是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<bool> ContainsAsync(string key);

    /// <summary>
    /// 清除
    /// </summary>
    /// <returns></returns>
    Task FlushAsync();

    /// <summary>
    /// 刷新
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task RefreshAsync(string key);

    /// <summary>
    /// 获取第一个
    /// </summary>
    /// <returns></returns>
    Task<CacheState> GetFirstAsync();

    /// <summary>
    /// 移除
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Remove(CacheState value);
}