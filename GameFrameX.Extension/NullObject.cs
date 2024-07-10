namespace GameFrameX.Extension;

/// <summary>
/// 可空对象
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly record struct NullObject<T> : IComparable, IComparable<T>, IEquatable<NullObject<T>>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    public NullObject(T item)
    {
        Item = item;
    }

    /// <summary>
    /// 
    /// </summary>
    public static NullObject<T> Null => new();

    /// <summary>
    /// 
    /// </summary>
    public T Item { get; }


    /// <summary>
    /// 隐式转换
    /// </summary>
    /// <param name="nullObject"></param>
    public static implicit operator T(NullObject<T> nullObject)
    {
        return nullObject.Item;
    }

    /// <summary>
    /// 隐式转换
    /// </summary>
    /// <param name="item"></param>
    public static implicit operator NullObject<T>(T item)
    {
        return new NullObject<T>(item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return (Item != null) ? Item.ToString() : "NULL";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int CompareTo(object value)
    {
        if (value is NullObject<T> nullObject)
        {
            if (nullObject.Item is IComparable c)
            {
                return ((IComparable)Item).CompareTo(c);
            }

            return Item.ToString().CompareTo(nullObject.Item.ToString());
        }

        return 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(T other)
    {
        if (other is IComparable c)
        {
            return ((IComparable)Item).CompareTo(c);
        }

        return Item.ToString().CompareTo(other.ToString());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Item);
    }

    /// <summary>指示当前对象是否等于同一类型的另一个对象。</summary>
    /// <param name="other">一个与此对象进行比较的对象。</param>
    /// <returns>如果当前对象等于 <paramref name="other" /> 参数，则为 true；否则为 false。</returns>
    public bool Equals(NullObject<T> other)
    {
        return EqualityComparer<T>.Default.Equals(Item, other.Item);
    }
}