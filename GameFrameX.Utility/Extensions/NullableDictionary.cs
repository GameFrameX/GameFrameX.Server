using System.Collections.Concurrent;

namespace GameFrameX.Utility.Extensions;

/// <summary>
/// 支持null-key和value的字典类型
/// </summary>
/// <typeparam name="TKey">键的类型</typeparam>
/// <typeparam name="TValue">值的类型</typeparam>
public class NullableDictionary<TKey, TValue> : Dictionary<NullObject<TKey>, TValue>
{
    /// <summary>
    /// 初始化一个空的 NullableDictionary 实例
    /// </summary>
    public NullableDictionary()
    {
    }

    /// <summary>
    /// 使用指定的默认值初始化 NullableDictionary 实例
    /// </summary>
    /// <param name="fallbackValue">当键不存在时返回的默认值</param>
    public NullableDictionary(TValue fallbackValue)
    {
        FallbackValue = fallbackValue;
    }

    /// <summary>
    /// 使用指定的初始容量初始化 NullableDictionary 实例
    /// </summary>
    /// <param name="capacity">字典的初始容量</param>
    public NullableDictionary(int capacity) : base(capacity)
    {
    }

    /// <summary>
    /// 使用指定的比较器初始化 NullableDictionary 实例
    /// </summary>
    /// <param name="comparer">用于键的比较器</param>
    public NullableDictionary(IEqualityComparer<NullObject<TKey>> comparer) : base(comparer)
    {
    }

    /// <summary>
    /// 使用指定的初始容量和比较器初始化 NullableDictionary 实例
    /// </summary>
    /// <param name="capacity">字典的初始容量</param>
    /// <param name="comparer">用于键的比较器</param>
    public NullableDictionary(int capacity, IEqualityComparer<NullObject<TKey>> comparer) : base(capacity, comparer)
    {
    }

    /// <summary>
    /// 使用指定的字典初始化 NullableDictionary 实例
    /// </summary>
    /// <param name="dictionary">用于初始化字典的键值对集合</param>
    public NullableDictionary(IDictionary<NullObject<TKey>, TValue> dictionary) : base(dictionary)
    {
    }

    /// <summary>
    /// 使用指定的字典和比较器初始化 NullableDictionary 实例
    /// </summary>
    /// <param name="dictionary">用于初始化字典的键值对集合</param>
    /// <param name="comparer">用于键的比较器</param>
    public NullableDictionary(IDictionary<NullObject<TKey>, TValue> dictionary, IEqualityComparer<NullObject<TKey>> comparer) : base(dictionary, comparer)
    {
    }

    internal TValue FallbackValue { get; set; }

    /// <summary>
    /// 获取或设置指定键的值
    /// </summary>
    /// <param name="key">键</param>
    public new TValue this[NullObject<TKey> key]
    {
        get { return TryGetValue(key, out var value) ? value : FallbackValue; }
        set { base[key] = value; }
    }

    /// <summary>
    /// 根据条件获取或设置第一个匹配的值
    /// </summary>
    /// <param name="condition">条件谓词</param>
    public TValue this[Func<KeyValuePair<TKey, TValue>, bool> condition]
    {
        get
        {
            foreach (var pair in this.Where(pair => condition(new KeyValuePair<TKey, TValue>(pair.Key.Item, pair.Value))))
            {
                return pair.Value;
            }

            return FallbackValue;
        }
        set
        {
            foreach (var pair in this.Where(pair => condition(new KeyValuePair<TKey, TValue>(pair.Key.Item, pair.Value))))
            {
                this[pair.Key] = value;
            }
        }
    }

    /// <summary>
    /// 根据条件获取或设置第一个匹配的值
    /// </summary>
    /// <param name="condition">条件谓词</param>
    public TValue this[Func<TKey, TValue, bool> condition]
    {
        get
        {
            foreach (var pair in this.Where(pair => condition(pair.Key.Item, pair.Value)))
            {
                return pair.Value;
            }

            return FallbackValue;
        }
        set
        {
            foreach (var pair in this.Where(pair => condition(pair.Key.Item, pair.Value)))
            {
                this[pair.Key] = value;
            }
        }
    }

    /// <summary>
    /// 根据条件获取或设置第一个匹配的值
    /// </summary>
    /// <param name="condition">条件谓词</param>
    public TValue this[Func<TKey, bool> condition]
    {
        get
        {
            foreach (var pair in this.Where(pair => condition(pair.Key.Item)))
            {
                return pair.Value;
            }

            return FallbackValue;
        }
        set
        {
            foreach (var pair in this.Where(pair => condition(pair.Key.Item)))
            {
                this[pair.Key] = value;
            }
        }
    }

    /// <summary>
    /// 根据条件获取或设置第一个匹配的值
    /// </summary>
    /// <param name="condition">条件谓词</param>
    public TValue this[Func<TValue, bool> condition]
    {
        get
        {
            foreach (var pair in this.Where(pair => condition(pair.Value)))
            {
                return pair.Value;
            }

            return FallbackValue;
        }
        set
        {
            foreach (var pair in this.Where(pair => condition(pair.Value)))
            {
                this[pair.Key] = value;
            }
        }
    }

    /// <summary>
    /// 获取或设置指定键的值
    /// </summary>
    /// <param name="key">键</param>
    public TValue this[TKey key]
    {
        get { return TryGetValue(new NullObject<TKey>(key), out var value) ? value : FallbackValue; }
        set { base[new NullObject<TKey>(key)] = value; }
    }

    /// <summary>
    /// 判断字典是否包含指定的键
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>如果包含则返回 true，否则返回 false</returns>
    public bool ContainsKey(TKey key)
    {
        return base.ContainsKey(new NullObject<TKey>(key));
    }

    /// <summary>
    /// 向字典中添加键值对
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public void Add(TKey key, TValue value)
    {
        base.Add(new NullObject<TKey>(key), value);
    }

    /// <summary>
    /// 从字典中移除指定的键
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>如果成功移除则返回 true，否则返回 false</returns>
    public bool Remove(TKey key)
    {
        return base.Remove(new NullObject<TKey>(key));
    }

    /// <summary>
    /// 尝试获取指定键的值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">输出参数，存储找到的值</param>
    /// <returns>如果找到则返回 true，否则返回 false</returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
        return base.TryGetValue(new NullObject<TKey>(key), out value);
    }

    /// <summary>
    /// 从 Dictionary&lt;TKey, TValue&gt; 隐式转换为 NullableDictionary&lt;TKey, TValue&gt;
    /// </summary>
    /// <param name="dic">源字典</param>
    /// <returns>转换后的 NullableDictionary 实例</returns>
    public static implicit operator NullableDictionary<TKey, TValue>(Dictionary<TKey, TValue> dic)
    {
        var nullableDictionary = new NullableDictionary<TKey, TValue>();
        foreach (var p in dic)
        {
            nullableDictionary[p.Key] = p.Value;
        }

        return nullableDictionary;
    }

    /// <summary>
    /// 从 ConcurrentDictionary&lt;TKey, TValue&gt; 隐式转换为 NullableDictionary&lt;TKey, TValue&gt;
    /// </summary>
    /// <param name="dic">源字典</param>
    public static implicit operator NullableDictionary<TKey, TValue>(ConcurrentDictionary<TKey, TValue> dic)
    {
        var nullableDictionary = new NullableDictionary<TKey, TValue>();
        foreach (var p in dic)
        {
            nullableDictionary[p.Key] = p.Value;
        }

        return nullableDictionary;
    }

    /// <summary>
    /// 从 NullableDictionary{TKey, TValue} 隐式转换为 Dictionary{TKey, TValue}
    /// </summary>
    /// <param name="dic">源字典</param>
    public static implicit operator Dictionary<TKey, TValue>(NullableDictionary<TKey, TValue> dic)
    {
        var dictionary = new Dictionary<TKey, TValue>();
        foreach (var p in dic)
        {
            dictionary[p.Key] = p.Value;
        }

        return dictionary;
    }
}