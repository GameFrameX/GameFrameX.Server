namespace GameFrameX.Extension;

/// <summary>
/// 守护.用于判断参数
/// </summary>
public static class ObjectExtension
{
    /// <summary>
    /// 检查对象是否为null
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool IsNull(this object self)
    {
        return self == null;
    }

    /// <summary>
    /// 检查对象是否不为null
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool IsNotNull(this object self)
    {
        return !self.IsNull();
    }

    /// <summary>
    /// 判断参数是否为null
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="name">参数名称</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentNullException">当参数为null,引发参数为空异常</exception>
    public static void CheckNotNull<T>(this T value, string name) where T : class
    {
        if (value.IsNull())
        {
            throw new ArgumentNullException(name, " can not be null.");
        }
    }

    /// <summary>
    /// 判断参数是否为null
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="name">参数名称</param>
    /// <param name="message">错误消息</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentNullException">当参数为null,引发参数为空异常</exception>
    public static void CheckNotNull<T>(this T value, string name, string message) where T : class
    {
        if (value.IsNull())
        {
            throw new ArgumentNullException(name, message);
        }
    }

    /// <summary>
    /// 判断参数是否在最大值和最小值之间
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="minValue">最小值</param>
    /// <param name="maxValue">最大值</param>
    /// <exception cref="ArgumentOutOfRangeException">当参数不在范围内时,引发参数为空异常</exception>
    public static void CheckRange(this int value, int minValue = 0, int maxValue = int.MaxValue)
    {
        if (value <= minValue || value >= maxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
    }

    /// <summary>
    /// 判断参数是否在最大值和最小值之间
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="minValue">最小值</param>
    /// <param name="maxValue">最大值</param>
    /// <returns>返回是否在范围内</returns>
    public static bool IsRange(this int value, int minValue = 0, int maxValue = int.MaxValue)
    {
        if (value <= minValue || value >= maxValue)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 判断参数是否在最大值和最小值之间
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="minValue">最小值</param>
    /// <param name="maxValue">最大值</param>
    /// <returns>返回是否在范围内</returns>
    public static bool IsRange(this uint value, uint minValue = 0, uint maxValue = uint.MaxValue)
    {
        if (value <= minValue || value >= maxValue)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 判断参数是否在最大值和最小值之间
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="minValue">最小值</param>
    /// <param name="maxValue">最大值</param>
    /// <returns>返回是否在范围内</returns>
    public static bool IsRange(this long value, long minValue = 0, long maxValue = long.MaxValue)
    {
        if (value <= minValue || value >= maxValue)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 判断参数是否在最大值和最小值之间
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="minValue">最小值</param>
    /// <param name="maxValue">最大值</param>
    /// <returns>返回是否在范围内</returns>
    public static bool IsRange(this ulong value, ulong minValue = 0, ulong maxValue = ulong.MaxValue)
    {
        if (value <= minValue || value >= maxValue)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 判断参数是否在最大值和最小值之间
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="minValue">最小值</param>
    /// <param name="maxValue">最大值</param>
    /// <returns>返回是否在范围内</returns>
    public static bool IsRange(this short value, short minValue = 0, short maxValue = short.MaxValue)
    {
        if (value <= minValue || value >= maxValue)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 判断参数是否在最大值和最小值之间
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="minValue">最小值</param>
    /// <param name="maxValue">最大值</param>
    /// <returns>返回是否在范围内</returns>
    public static bool IsRange(this ushort value, ushort minValue = 0, ushort maxValue = ushort.MaxValue)
    {
        if (value <= minValue || value >= maxValue)
        {
            return false;
        }

        return true;
    }
}