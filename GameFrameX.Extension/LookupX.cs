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
    private readonly IDictionary<TKey, List<TElement>> dictionary;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dic"></param>
    public LookupX(IDictionary<TKey, List<TElement>> dic)
    {
        dictionary = dic;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dic"></param>
    public LookupX(ConcurrentDictionary<TKey, List<TElement>> dic)
    {
        dictionary = dic;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator<List<TElement>> GetEnumerator()
    {
        return dictionary.Values.GetEnumerator();
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
        return dictionary.ContainsKey(key);
    }

    /// <summary>
    /// 
    /// </summary>
    public int Count => dictionary.Count;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    public List<TElement> this[TKey key]
    {
        get { return dictionary.TryGetValue(key, out var value) ? value : new List<TElement>(); }
    }
}