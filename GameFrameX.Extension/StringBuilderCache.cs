// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/*============================================================
**
** Class:  StringBuilderCache
**
** Purpose: provide a cached reusable instance of stringbuilder
**          per thread  it's an optimisation that reduces the
**          number of instances constructed and collected.
**
**  Acquire - is used to get a string builder to use of a
**            particular size.  It can be called any number of
**            times, if a stringbuilder is in the cache then
**            it will be returned and the cache emptied.
**            subsequent calls will return a new stringbuilder.
**
**            A StringBuilder instance is cached in
**            Thread Local Storage and so there is one per thread
**
**  Release - Place the specified builder in the cache if it is
**            not too big.
**            The stringbuilder should not be used after it has
**            been released.
**            Unbalanced Releases are perfectly acceptable.  It
**            will merely cause the runtime to create a new
**            stringbuilder next time Acquire is called.
**
**  GetStringAndRelease
**          - ToString() the stringbuilder, Release it to the
**            cache and return the resulting string
**
===========================================================*/

using System.Text;

namespace GameFrameX.Extension;

/// <summary>
/// 提供 StringBuilder 的缓存可重用实例
/// </summary>
public static class StringBuilderCache
{
    // The value 360 was chosen in discussion with performance experts as a compromise between using
    // as litle memory (per thread) as possible and still covering a large part of short-lived
    // StringBuilder creations on the startup path of VS designers.
    private const int MaxBuilderSize = 360;

    [ThreadStatic] private static StringBuilder _cachedInstance;

    /// <summary>
    /// 获取指定大小的 StringBuilder
    /// </summary>
    /// <param name="capacity">长度,默认为 16</param>
    /// <returns>StringBuilder 对象</returns>
    public static StringBuilder Acquire(int capacity = 16)
    {
        if (capacity <= MaxBuilderSize)
        {
            var sb = _cachedInstance;
            if (sb != null)
            {
                // Avoid stringbuilder block fragmentation by getting a new StringBuilder
                // when the requested size is larger than the current capacity
                if (capacity <= sb.Capacity)
                {
                    _cachedInstance = null;
                    sb.Clear();
                    return sb;
                }
            }
        }

        return new StringBuilder(capacity);
    }

    /// <summary>
    /// 如果指定的构建器不是太大，则将其放在缓存中
    /// </summary>
    /// <param name="sb">StringBuilder 对象</param>
    public static void Release(StringBuilder sb)
    {
        if (sb.Capacity <= MaxBuilderSize)
        {
            _cachedInstance = sb;
        }
    }

    /// <summary>
    /// ToString（） 字符串生成器，将其释放到缓存中并返回结果字符串
    /// </summary>
    /// <param name="sb">StringBuilder 对象</param>
    /// <returns>返回其生成的字符串</returns>
    public static string GetStringAndRelease(StringBuilder sb)
    {
        var result = sb.ToString();
        Release(sb);
        return result;
    }
}