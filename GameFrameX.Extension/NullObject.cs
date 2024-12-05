namespace GameFrameX.Extension;

/// <summary>
/// 表示可为空的对象。
/// </summary>
/// <typeparam name="T">对象的类型。</typeparam>
public readonly record struct NullObject<T> : IComparable, IComparable<T>, IEquatable<NullObject<T>>
{
    /// <summary>
    /// 初始化一个新的 <see cref="NullObject{T}"/> 实例。
    /// </summary>
    /// <param name="item">对象的值。</param>
    public NullObject(T item)
    {
        Item = item;
    }

    /// <summary>
    /// 获取一个表示空值的 <see cref="NullObject{T}"/> 实例。
    /// </summary>
    public static NullObject<T> Null
    {
        get { return new NullObject<T>(); }
    }

    /// <summary>
    /// 获取对象的值。
    /// </summary>
    public T Item { get; }

    /// <summary>
    /// 将 <see cref="NullObject{T}"/> 隐式转换为类型 <typeparamref name="T"/>。
    /// </summary>
    /// <param name="nullObject">要转换的 <see cref="NullObject{T}"/> 实例。</param>
    public static implicit operator T(NullObject<T> nullObject)
    {
        return nullObject.Item;
    }

    /// <summary>
    /// 将类型 <typeparamref name="T"/> 隐式转换为 <see cref="NullObject{T}"/>。
    /// </summary>
    /// <param name="item">要转换的值。</param>
    public static implicit operator NullObject<T>(T item)
    {
        return new NullObject<T>(item);
    }

    /// <summary>
    /// 返回对象的字符串表示形式。
    /// </summary>
    /// <returns>对象的字符串表示形式，如果对象为 null，则返回 "NULL"。</returns>
    public override string ToString()
    {
        return (Item != null) ? Item.ToString() : "NULL";
    }

    /// <summary>
    /// 比较当前对象与另一个对象。
    /// </summary>
    /// <param name="value">要比较的对象。</param>
    /// <returns>一个整数，指示当前对象与 <paramref name="value"/> 的相对顺序。</returns>
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
    /// 比较当前对象与同一类型的另一个对象。
    /// </summary>
    /// <param name="other">要比较的对象。</param>
    /// <returns>一个整数，指示当前对象与 <paramref name="other"/> 的相对顺序。</returns>
    public int CompareTo(T other)
    {
        if (other is IComparable c)
        {
            return ((IComparable)Item).CompareTo(c);
        }

        return Item.ToString().CompareTo(other.ToString());
    }

    /// <summary>
    /// 返回当前对象的哈希代码。
    /// </summary>
    /// <returns>当前对象的哈希代码。</returns>
    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Item);
    }

    /// <summary>
    /// 指示当前对象是否等于同一类型的另一个对象。
    /// </summary>
    /// <param name="other">一个与此对象进行比较的对象。</param>
    /// <returns>如果当前对象等于 <paramref name="other"/> 参数，则为 true；否则为 false。</returns>
    public bool Equals(NullObject<T> other)
    {
        return EqualityComparer<T>.Default.Equals(Item, other.Item);
    }
}