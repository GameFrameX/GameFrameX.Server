namespace GameFrameX.Extension;

/// <summary>
/// 值可被Dispose的字典类型
/// </summary>
/// <typeparam name="TKey">键的类型。</typeparam>
/// <typeparam name="TValue">值的类型，必须实现IDisposable接口。</typeparam>
public class DisposableDictionary<TKey, TValue> : NullableDictionary<TKey, TValue>, IDisposable where TValue : IDisposable
{
    private bool _isDisposed;

    /// <summary>
    /// 初始化一个新的 <see cref="DisposableDictionary{TKey, TValue}" /> 实例。
    /// </summary>
    public DisposableDictionary()
    {
    }

    /// <summary>
    /// 使用指定的默认值初始化一个新的 <see cref="DisposableDictionary{TKey, TValue}" /> 实例。
    /// </summary>
    /// <param name="fallbackValue">当键不存在时返回的默认值。</param>
    public DisposableDictionary(TValue fallbackValue)
    {
        FallbackValue = fallbackValue;
    }

    /// <summary>
    /// 使用指定的初始容量初始化一个新的 <see cref="DisposableDictionary{TKey, TValue}" /> 实例。
    /// </summary>
    /// <param name="capacity">字典的初始容量。</param>
    public DisposableDictionary(int capacity) : base(capacity)
    {
    }

    /// <summary>
    /// 使用指定的字典初始化一个新的 <see cref="DisposableDictionary{TKey, TValue}" /> 实例。
    /// </summary>
    /// <param name="dictionary">用于初始化的字典。</param>
    public DisposableDictionary(IDictionary<NullObject<TKey>, TValue> dictionary) : base(dictionary)
    {
    }

    /// <summary>
    /// 释放资源。
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        Dispose(true);
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 终结器，确保未释放的资源在对象被垃圾回收时被释放。
    /// </summary>
    ~DisposableDictionary()
    {
        Dispose(false);
    }

    /// <summary>
    /// 释放资源。
    /// </summary>
    /// <param name="disposing">指示是否应释放托管资源。</param>
    protected virtual void Dispose(bool disposing)
    {
        foreach (var s in Values.Where(v => v != null))
        {
            s.Dispose();
        }
    }
}