using System.Collections.Concurrent;

namespace GameFrameX.Utility.Extensions;

/// <summary>
/// 支持null键和值的并发字典类型
/// </summary>
/// <typeparam name="TKey">键的类型</typeparam>
/// <typeparam name="TValue">值的类型</typeparam>
public class NullableConcurrentDictionary<TKey, TValue> : ConcurrentDictionary<NullObject<TKey>, TValue>
{
    /// <summary>
    /// 初始化一个新的 <see cref="NullableConcurrentDictionary{TKey, TValue}" /> 实例。
    /// </summary>
    public NullableConcurrentDictionary()
    {
    }

    /// <summary>
    /// 使用指定的默认值初始化一个新的 <see cref="NullableConcurrentDictionary{TKey, TValue}" /> 实例。
    /// </summary>
    /// <param name="fallbackValue">当键不存在时返回的默认值。</param>
    public NullableConcurrentDictionary(TValue fallbackValue)
    {
        FallbackValue = fallbackValue;
    }

    /// <summary>
    /// 使用指定的并发级别和初始容量初始化一个新的 <see cref="NullableConcurrentDictionary{TKey, TValue}" /> 实例。
    /// </summary>
    /// <param name="concurrencyLevel">并发级别。</param>
    /// <param name="capacity">初始容量。</param>
    public NullableConcurrentDictionary(int concurrencyLevel, int capacity) : base(concurrencyLevel, capacity)
    {
    }

    /// <summary>
    /// 使用指定的比较器初始化一个新的 <see cref="NullableConcurrentDictionary{TKey, TValue}" /> 实例。
    /// </summary>
    /// <param name="comparer">用于比较键的比较器。</param>
    public NullableConcurrentDictionary(IEqualityComparer<NullObject<TKey>> comparer) : base(comparer)
    {
    }

    /// <summary>
    /// 获取或设置当键不存在时返回的默认值。
    /// </summary>
    internal TValue FallbackValue { get; set; }

    /// <summary>
    /// 获取或设置指定键的值。
    /// </summary>
    /// <param name="key">键。</param>
    public new TValue this[NullObject<TKey> key]
    {
        get { return base.TryGetValue(key, out var value) ? value : FallbackValue; }
        set { base[key] = value; }
    }

    /// <summary>
    /// 根据条件获取或设置第一个匹配的键值对的值。
    /// </summary>
    /// <param name="condition">用于筛选键值对的条件。</param>
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
    /// 根据条件获取或设置第一个匹配的键值对的值。
    /// </summary>
    /// <param name="condition">用于筛选键值对的条件。</param>
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
    /// 根据条件获取或设置第一个匹配的键值对的值。
    /// </summary>
    /// <param name="condition">用于筛选键的条件。</param>
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
    /// 根据条件获取或设置第一个匹配的键值对的值。
    /// </summary>
    /// <param name="condition">用于筛选值的条件。</param>
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
    /// 获取或设置指定键的值。
    /// </summary>
    /// <param name="key">键。</param>
    public TValue this[TKey key]
    {
        get { return base.TryGetValue(new NullObject<TKey>(key), out var value) ? value : FallbackValue; }
        set { base[new NullObject<TKey>(key)] = value; }
    }

    /// <summary>
    /// 判断字典中是否包含指定的键。
    /// </summary>
    /// <param name="key">键。</param>
    /// <returns>如果包含指定的键，则返回 true；否则返回 false。</returns>
    public bool ContainsKey(TKey key)
    {
        return base.ContainsKey(new NullObject<TKey>(key));
    }

    /// <summary>
    /// 尝试添加一个键值对。
    /// </summary>
    /// <param name="key">键。</param>
    /// <param name="value">值。</param>
    /// <returns>如果成功添加，则返回 true；否则返回 false。</returns>
    public bool TryAdd(TKey key, TValue value)
    {
        return base.TryAdd(new NullObject<TKey>(key), value);
    }

    /// <summary>
    /// 尝试移除一个键值对。
    /// </summary>
    /// <param name="key">键。</param>
    /// <param name="value">移除的值。</param>
    /// <returns>如果成功移除，则返回 true；否则返回 false。</returns>
    public bool TryRemove(TKey key, out TValue value)
    {
        return base.TryRemove(new NullObject<TKey>(key), out value);
    }

    /// <summary>
    /// 尝试更新一个键值对。
    /// </summary>
    /// <param name="key">键。</param>
    /// <param name="value">新的值。</param>
    /// <param name="comparisionValue">比较值。</param>
    /// <returns>如果成功更新，则返回 true；否则返回 false。</returns>
    public bool TryUpdate(TKey key, TValue value, TValue comparisionValue)
    {
        return base.TryUpdate(new NullObject<TKey>(key), value, comparisionValue);
    }

    /// <summary>
    /// 尝试获取指定键的值。
    /// </summary>
    /// <param name="key">键。</param>
    /// <param name="value">获取的值。</param>
    /// <returns>如果成功获取，则返回 true；否则返回 false。</returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
        return base.TryGetValue(new NullObject<TKey>(key), out value);
    }

    /// <summary>
    /// 从 <see cref="Dictionary{TKey, TValue}" /> 隐式转换为 <see cref="NullableConcurrentDictionary{TKey, TValue}" />。
    /// </summary>
    /// <param name="dic">要转换的字典。</param>
    public static implicit operator NullableConcurrentDictionary<TKey, TValue>(Dictionary<TKey, TValue> dic)
    {
        var nullableDictionary = new NullableConcurrentDictionary<TKey, TValue>();
        foreach (var p in dic)
        {
            nullableDictionary[p.Key] = p.Value;
        }

        return nullableDictionary;
    }

    /// <summary>
    /// 从 <see cref="ConcurrentDictionary{TKey, TValue}" /> 隐式转换为 <see cref="NullableConcurrentDictionary{TKey, TValue}" />。
    /// </summary>
    /// <param name="dic">要转换的并发字典。</param>
    public static implicit operator NullableConcurrentDictionary<TKey, TValue>(ConcurrentDictionary<TKey, TValue> dic)
    {
        var nullableDictionary = new NullableConcurrentDictionary<TKey, TValue>();
        foreach (var p in dic)
        {
            nullableDictionary[p.Key] = p.Value;
        }

        return nullableDictionary;
    }

    /// <summary>
    /// 从 <see cref="NullableConcurrentDictionary{TKey, TValue}" /> 隐式转换为 <see cref="ConcurrentDictionary{TKey, TValue}" />。
    /// </summary>
    /// <param name="dic">要转换的字典。</param>
    public static implicit operator ConcurrentDictionary<TKey, TValue>(NullableConcurrentDictionary<TKey, TValue> dic)
    {
        var concurrentDictionary = new ConcurrentDictionary<TKey, TValue>();
        foreach (var p in dic)
        {
            concurrentDictionary[p.Key] = p.Value;
        }

        return concurrentDictionary;
    }
}