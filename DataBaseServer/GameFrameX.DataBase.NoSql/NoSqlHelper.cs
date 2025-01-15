using GameFrameX.Utility.Extensions;

namespace GameFrameX.DataBase.NoSql;

/// <summary>
/// NoSql帮助类
/// </summary>
public static class NoSqlHelper
{
    private static INoSqlHelper _noSqlHelper;

    /// <summary>
    /// 设置NoSql
    /// </summary>
    /// <param name="helper"></param>
    public static void SetNoSqlHelper(INoSqlHelper helper)
    {
        helper.CheckNotNull(nameof(helper));
        _noSqlHelper = helper;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="connectionStrings">链接字符串</param>
    public static void Init(params string[] connectionStrings)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        _noSqlHelper.Init(connectionStrings);
    }

    /// <summary>
    /// 同步设置指定的Key的数据到NoSql中
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    public static void Set(string key, string value)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        _noSqlHelper.Set(key, value);
    }

    /// <summary>
    /// 同步设置指定的Key的数据到NoSql中
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    /// <typeparam name="T"></typeparam>
    public static void Set<T>(string key, T value) where T : class
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        _noSqlHelper.Set(key, value);
    }

    /// <summary>
    /// 同步设置指定的Key的数据到NoSql中
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    /// <param name="timeOut">过期时间</param>
    /// <typeparam name="T"></typeparam>
    public static void Set<T>(string key, T value, TimeSpan timeOut) where T : class
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        _noSqlHelper.Set(key, value, timeOut);
    }

    /// <summary>
    /// 同步设置指定的Key的数据到NoSql中
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    /// <param name="timeoutSeconds">过期时间</param>
    /// <typeparam name="T"></typeparam>
    public static void Set<T>(string key, T value, int timeoutSeconds) where T : class
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        _noSqlHelper.Set(key, value, timeoutSeconds);
    }

    /// <summary>
    /// 异步设置指定的Key的数据到NoSql中
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">值</param>
    /// <returns></returns>
    public static Task SetAsync(string key, string value)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));

        return _noSqlHelper.SetAsync(key, value);
    }

    /// <summary>
    /// 异步设置指定的Key的数据到NoSql中
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task SetAsync<T>(string key, T value) where T : class
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.SetAsync(key, value);
    }

    /// <summary>
    /// 异步设置指定的Key的数据到NoSql中，指定的过期时间
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">值</param>
    /// <param name="timeOut">过期时间</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task SetAsync<T>(string key, T value, TimeSpan timeOut) where T : class
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.SetAsync(key, value, timeOut);
    }

    /// <summary>
    /// 异步设置指定的Key的数据到NoSql中
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">值</param>
    /// <param name="timeoutSeconds">过期时间，单位秒</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task SetAsync<T>(string key, T value, int timeoutSeconds) where T : class
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.SetAsync(key, value, timeoutSeconds);
    }

    /// <summary>
    /// 从NoSql中获取指定的key的值
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns></returns>
    public static string GetString(string key)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.GetString(key);
    }

    /// <summary>
    /// 异步从NoSql中获取指定的key的值
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns></returns>
    public static Task<string> GetStringAsync(string key)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.GetStringAsync(key);
    }

    /// <summary>
    /// 从NoSql中获取指定的key的值，如果不存在则返回null
    /// </summary>
    /// <param name="key">Key</param>
    /// <typeparam name="T">类型</typeparam>
    /// <returns></returns>
    public static T Get<T>(string key) where T : class
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        key.CheckNotNullOrEmpty(nameof(key));
        return _noSqlHelper.Get<T>(key);
    }

    /// <summary>
    /// 异步从NoSql中获取指定的key的值，如果不存在则返回null
    /// </summary>
    /// <param name="key">Key</param>
    /// <typeparam name="T">类型</typeparam>
    /// <returns></returns>
    public static Task<T> GetAsync<T>(string key) where T : class
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        key.CheckNotNullOrEmpty(nameof(key));
        return _noSqlHelper.GetAsync<T>(key);
    }

    /// <summary>
    /// 从NoSql中删除指定的keys，如果不存在则忽略
    /// </summary>
    /// <param name="keys">Keys列表</param>
    /// <returns></returns>
    public static long Delete(params string[] keys)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.Delete(keys);
    }

    /// <summary>
    /// 从NoSql中删除指定的keys，如果不存在则忽略
    /// </summary>
    /// <param name="keys">Keys列表</param>
    /// <returns></returns>
    public static Task<long> DeleteAsync(params string[] keys)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.DeleteAsync(keys);
    }

    /// <summary>
    /// 判断指定的key是否存在
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public static long Exists(params string[] keys)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.Exists(keys);
    }

    /// <summary>
    /// 异步判断指定的key是否存在
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public static Task<long> ExistsAsync(params string[] keys)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.ExistsAsync(keys);
    }

    /// <summary>
    /// 判断指定的key是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool Exists(string key)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.Exists(key);
    }

    /// <summary>
    /// 异步判断指定的key是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static Task<bool> ExistsAsync(string key)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.ExistsAsync(key);
    }

    /// <summary>
    /// 从NoSql中设置指定Key的过期时间
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="seconds">过期时间</param>
    /// <returns></returns>
    public static bool Expire(string key, int seconds)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.Expire(key, seconds);
    }

    /// <summary>
    /// 异步从NoSql中设置指定Key的过期时间
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="seconds">过期时间</param>
    /// <returns></returns>
    public static Task<bool> ExpireAsync(string key, int seconds)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.ExpireAsync(key, seconds);
    }

    /// <summary>
    /// 从NoSql中设置指定Key的过期时间
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="expireTime">过期时间</param>
    /// <returns></returns>
    public static bool Expire(string key, TimeSpan expireTime)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.Expire(key, expireTime);
    }

    /// <summary>
    /// 异步从NoSql中设置指定Key的过期时间
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="expireTime">过期时间</param>
    /// <returns></returns>
    public static Task<bool> ExpireAsync(string key, TimeSpan expireTime)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.ExpireAsync(key, expireTime);
    }

    /// <summary>
    /// 从NoSql中删除指定Key的过期时间
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns></returns>
    public static bool RemoveExpireTime(string key)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.RemoveExpireTime(key);
    }

    /// <summary>
    /// 异步从NoSql中删除指定Key的过期时间
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns></returns>
    public static Task<bool> RemoveExpireTimeAsync(string key)
    {
        _noSqlHelper.CheckNotNull(nameof(_noSqlHelper));
        return _noSqlHelper.RemoveExpireTimeAsync(key);
    }
}