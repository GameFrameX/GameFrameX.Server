using FreeRedis;
using GameFrameX.Extension;

namespace GameFrameX.DataBase.NoSql.Redis;

/// <summary>
/// Redis帮助类
/// </summary>
public partial class RedisHelper : INoSqlHelper
{
    private RedisClient _client;


    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="connectionStrings">链接字符串</param>
    public void Init(params string[] connectionStrings)
    {
        ConnectionStringBuilder[] connectionStringBuilders = new ConnectionStringBuilder[connectionStrings.Length];
        for (var index = 0; index < connectionStrings.Length; index++)
        {
            var connectionString = connectionStrings[index];
            connectionStringBuilders[index] = connectionString;
        }

        _client = new RedisClient(connectionStringBuilders);
    }

    /// <summary>
    /// 从NoSql中删除指定的keys，如果不存在则忽略
    /// </summary>
    /// <param name="keys">Keys列表</param>
    /// <returns></returns>
    public long Delete(params string[] keys)
    {
        return _client.Del(keys);
    }

    /// <summary>
    /// 从NoSql中删除指定的keys，如果不存在则忽略
    /// </summary>
    /// <param name="keys">Keys列表</param>
    /// <returns></returns>
    public Task<long> DeleteAsync(params string[] keys)
    {
        return _client.DelAsync(keys);
    }

    /// <summary>
    /// 判断指定的key是否存在
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public long Exists(params string[] keys)
    {
        _client.CheckNotNull(nameof(_client));
        return _client.Exists(keys);
    }

    /// <summary>
    /// 异步判断指定的key是否存在
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public Task<long> ExistsAsync(params string[] keys)
    {
        _client.CheckNotNull(nameof(_client));
        return _client.ExistsAsync(keys);
    }

    /// <summary>
    /// 判断指定的key是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Exists(string key)
    {
        NullGuard(key);
        return _client.Exists(key);
    }

    /// <summary>
    /// 异步判断指定的key是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Task<bool> ExistsAsync(string key)
    {
        NullGuard(key);
        return _client.ExistsAsync(key);
    }

    /// <summary>
    /// 从NoSql中设置指定Key的过期时间
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="seconds">过期时间</param>
    /// <returns></returns>
    public bool Expire(string key, int seconds)
    {
        NullGuard(key);
        return _client.Expire(key, seconds);
    }

    /// <summary>
    /// 异步从NoSql中设置指定Key的过期时间
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="seconds">过期时间</param>
    /// <returns></returns>
    public Task<bool> ExpireAsync(string key, int seconds)
    {
        NullGuard(key);
        return _client.ExpireAsync(key, seconds);
    }

    /// <summary>
    /// 从NoSql中设置指定Key的过期时间
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="expireTime">过期时间</param>
    /// <returns></returns>
    public bool Expire(string key, TimeSpan expireTime)
    {
        NullGuard(key);
        return _client.Expire(key, expireTime);
    }

    /// <summary>
    /// 异步从NoSql中设置指定Key的过期时间
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="expireTime">过期时间</param>
    /// <returns></returns>
    public Task<bool> ExpireAsync(string key, TimeSpan expireTime)
    {
        NullGuard(key);
        return _client.ExpireAsync(key, expireTime);
    }

    /// <summary>
    /// 从NoSql中删除指定Key的过期时间
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns></returns>
    public bool RemoveExpireTime(string key)
    {
        NullGuard(key);
        return _client.Persist(key);
    }

    /// <summary>
    /// 异步从NoSql中删除指定Key的过期时间
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns></returns>
    public Task<bool> RemoveExpireTimeAsync(string key)
    {
        NullGuard(key);
        return _client.PersistAsync(key);
    }

    private void NullGuard(string key)
    {
        _client.CheckNotNull(nameof(_client));
        key.CheckNotNullOrEmpty(nameof(key));
    }

    private void NullGuard<T>(string key, T value) where T : class
    {
        NullGuard(key);
        value.CheckNotNull(nameof(value));
    }
}