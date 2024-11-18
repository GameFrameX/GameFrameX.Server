namespace GameFrameX.Extension
{
    /// <summary>
    /// 双向字典，实现键和值的双向映射。
    /// </summary>
    /// <typeparam name="TKey">键的类型。</typeparam>
    /// <typeparam name="TValue">值的类型。</typeparam>
    public class BidirectionalDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _forwardDictionary;
        private readonly Dictionary<TValue, TKey> _reverseDictionary;

        /// <summary>
        /// 初始化 <see cref="BidirectionalDictionary{TKey, TValue}"/> 类的新实例。
        /// </summary>
        public BidirectionalDictionary(int initialCapacity = 8)
        {
            _forwardDictionary = new Dictionary<TKey, TValue>(initialCapacity);
            _reverseDictionary = new Dictionary<TValue, TKey>(initialCapacity);
        }

        /// <summary>
        /// 尝试根据值获取对应的键。
        /// </summary>
        /// <param name="value">要查找的值。</param>
        /// <param name="key">查找到的键。</param>
        /// <returns>如果成功找到键，则为 true；否则为 false。</returns>
        public bool TryGetKey(TValue value, out TKey key)
        {
            return _reverseDictionary.TryGetValue(value, out key);
        }

        /// <summary>
        /// 尝试根据键获取对应的值。
        /// </summary>
        /// <param name="key">要查找的键。</param>
        /// <param name="value">查找到的值。</param>
        /// <returns>如果成功找到值，则为 true；否则为 false。</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _forwardDictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// 清空双向字典中的所有键值对。
        /// </summary>
        public void Clear()
        {
            _forwardDictionary.Clear();
            _reverseDictionary.Clear();
        }

        /// <summary>
        /// 尝试向双向字典中添加键值对。
        /// </summary>
        /// <param name="key">要添加的键。</param>
        /// <param name="value">要添加的值。</param>
        /// <returns>如果成功添加键值对，则为 true；否则为 false。</returns>
        public bool TryAdd(TKey key, TValue value)
        {
            if (_forwardDictionary.TryAdd(key, value))
            {
                _reverseDictionary.Add(value, key);
                return true;
            }

            return false;
        }
    }
}