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
        if (value == null)
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
        if (value == null)
        {
            throw new ArgumentNullException(name, message);
        }
    }
}