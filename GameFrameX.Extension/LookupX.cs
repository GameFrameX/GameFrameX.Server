using System.Collections;
using System.Collections.Concurrent;

namespace GameFrameX.Extension;

/// <summary>
/// 表示键和元素之间的多对多关系的集合。
/// </summary>
/// <typeparam name="TKey">键的类型。</typeparam>
/// <typeparam name="TElement">元素的类型。</typeparam>
public class LookupX<TKey, TElement> : IEnumerable<List<TElement>>
{
    private readonly IDictionary<TKey, List<TElement>> _dictionary;

    /// <summary>
    /// 使用指定的字典初始化一个新的 <see cref="LookupX{TKey, TElement}"/> 实例。
    /// </summary>
    /// <param name="dic">用于存储键和元素列表的字典。</param>
    public LookupX(IDictionary<TKey, List<TElement>> dic)
    {
        _dictionary = dic;
    }

    /// <summary>
    /// 使用指定的并发字典初始化一个新的 <see cref="LookupX{TKey, TElement}"/> 实例。
    /// </summary>
    /// <param name="dic">用于存储键和元素列表的并发字典。</param>
    public LookupX(ConcurrentDictionary<TKey, List<TElement>> dic)
    {
        _dictionary = dic;
    }

    /// <summary>
    /// 返回一个枚举器，该枚举器可以遍历集合中的每个元素列表。
    /// </summary>
    /// <returns>一个枚举器，该枚举器可以遍历集合中的每个元素列表。</returns>
    public IEnumerator<List<TElement>> GetEnumerator()
    {
        return _dictionary.Values.GetEnumerator();
    }

    /// <summary>
    /// 返回一个枚举器，该枚举器可以遍历集合中的每个元素列表。
    /// </summary>
    /// <returns>一个枚举器，该枚举器可以遍历集合中的每个元素列表。</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// 判断集合中是否包含指定的键。
    /// </summary>
    /// <param name="key">要检查的键。</param>
    /// <returns>如果集合中包含指定的键，则返回 true；否则返回 false。</returns>
    public bool Contains(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    /// <summary>
    /// 获取集合中的键值对数量。
    /// </summary>
    public int Count
    {
        get { return _dictionary.Count; }
    }

    /// <summary>
    /// 获取与指定键关联的元素列表。
    /// 如果键不存在，则返回一个空的元素列表。
    /// </summary>
    /// <param name="key">要查找的键。</param>
    /// <returns>与指定键关联的元素列表。</returns>
    public List<TElement> this[TKey key]
    {
        get { return _dictionary.TryGetValue(key, out var value) ? value : new List<TElement>(); }
    }
}
