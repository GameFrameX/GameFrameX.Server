using ProtoBuf;
using ProtoBuf.Meta;

namespace GameFrameX.Bot;

/// <summary>
/// 序列化帮助类
/// </summary>
public static class SerializerHelper
{
    /// <summary>
    /// 序列化对象
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static byte[] Serialize<T>(T value)
    {
        using var memoryStream = new MemoryStream();
        Serializer.Serialize(memoryStream, value);
        return memoryStream.ToArray();
    }

    /// <summary>
    /// 序列化对象
    /// </summary>
    /// <param name="value"></param>
    /// <param name="prefixStyle"></param>
    /// <param name="fieldNumber"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static byte[] SerializeWithLengthPrefix<T>(T value, PrefixStyle prefixStyle, int fieldNumber)
    {
        using var memoryStream = new MemoryStream();
        Serializer.SerializeWithLengthPrefix(memoryStream, value, prefixStyle, fieldNumber);
        return memoryStream.ToArray();
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
    /// <typeparam name="T">类型</typeparam>
    /// <returns></returns>
    public static T Deserialize<T>(byte[] data)
    {
        using var memoryStream = new MemoryStream(data);
        return (T)Serializer.Deserialize(typeof(T), memoryStream);
    }

    /// <summary>
    /// 反序列化数据对象
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="type">类型</param>
    /// <returns></returns>
    public static object Deserialize(byte[] data, Type type)
    {
        using var memoryStream = new MemoryStream(data);
        return Serializer.Deserialize(type, memoryStream);
    }

    /// <summary>
    /// 反序列化数据对象
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="prefixStyle"></param>
    /// <param name="fieldNumber"></param>
    /// <typeparam name="T">类型</typeparam>
    /// <returns></returns>
    public static T DeserializeWithLengthPrefix<T>(byte[] data, PrefixStyle prefixStyle, int fieldNumber)
    {
        using var memoryStream = new MemoryStream(data);
        return Serializer.DeserializeWithLengthPrefix<T>(memoryStream, prefixStyle, fieldNumber);
    }
}