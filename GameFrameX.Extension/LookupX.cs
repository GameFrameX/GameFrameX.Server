using System.Collections;
using System.Collections.Concurrent;

namespace GameFrameX.Extension;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TElement"></typeparam>
public class LookupX<TKey, TElement> : IEnumerable<List<TElement>>
{
    private readonly IDictionary<TKey, List<TElement>> _dictionary;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dic"></param>
    public LookupX(IDictionary<TKey, List<TElement>> dic)
    {
        _dictionary = dic;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dic"></param>
    public LookupX(ConcurrentDictionary<TKey, List<TElement>> dic)
    {
        _dictionary = dic;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator<List<TElement>> GetEnumerator()
    {
        return _dictionary.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    /// <summary>
    /// 
    /// </summary>
    public int Count
    {
        get { return _dictionary.Count; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    public List<TElement> this[TKey key]
    {
        get { return _dictionary.TryGetValue(key, out var value) ? value : new List<TElement>(); }
    }
}