using ProtoBuf;
using ProtoBuf.Meta;

namespace GameFrameX.ProtoBuf.Net;

/// <summary>
/// 消息序列化帮助类
/// </summary>
public static class ProtoBufSerializerHelper
{
    /// <summary>
    /// 序列化对象
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static byte[] Serialize<T>(T value)
    {
        using (var memoryStream = new MemoryStream())
        {
            Serializer.Serialize(memoryStream, value);
            return memoryStream.ToArray();
        }
    }

    /// <summary>
    /// 注册类型
    /// </summary>
    /// <param name="type"></param>
    public static void Register(Type type)
    {
        RuntimeTypeModel.Default.Add(type, false);
    }

    /// <summary>
    /// 反序列化数据对象
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="type">对象类型</param>
    /// <returns></returns>
    public static object Deserialize(byte[] data, Type type)
    {
        using (var memoryStream = new MemoryStream(data))
        {
            return Serializer.Deserialize(type, memoryStream);
        }
    }

    /// <summary>
    /// 将字节数组反序列化并合并到已有实例（合并模式）
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="data">待反序列化的字节数组</param>
    /// <param name="instance">目标对象实例，反序列化数据将合并到该实例中。若实例为 null，将抛出 ArgumentNullException。</param>
    /// <returns>合并完成后的实例（与参数 instance 为同一对象）。若输入字节数组为空，则直接返回原实例。</returns>
    /// <exception cref="ArgumentNullException">当 instance 为 null 时抛出。</exception>
    /// <exception cref="ArgumentException">当 data 为 null 或长度为零时抛出。</exception>
    /// <remarks>
    /// 该方法使用 ProtoBuf 的合并模式，将字节流中的数据字段合并到已有对象中。
    /// 适用于需要增量更新或复用对象的场景，可避免频繁创建新实例带来的性能开销。
    /// 注意：合并过程中，现有字段值可能被覆盖，具体行为取决于 ProtoBuf 字段规则。
    /// </remarks>
    public static T Deserialize<T>(byte[] data, T instance)
    {
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        ArgumentNullException.ThrowIfNull(instance, nameof(instance));

        if (data.Length == 0)
        {
            return instance;
        }

        using (var memoryStream = new MemoryStream(data))
        {
            return Serializer.Merge(memoryStream, instance);
        }
    }

    /// <summary>
    /// 反序列化数据对象
    /// </summary>
    /// <param name="data">数据内容</param>
    /// <typeparam name="T">目标类型</typeparam>
    /// <returns></returns>
    public static T Deserialize<T>(byte[] data) where T : class, new()
    {
        using (var memoryStream = new MemoryStream(data))
        {
            return Serializer.Deserialize(typeof(T), memoryStream) as T;
        }
    }
}