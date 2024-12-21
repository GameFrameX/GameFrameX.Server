using Microsoft.IO;
using ProtoBuf;
using ProtoBuf.Meta;

namespace GameFrameX.ProtoBuf.Net;

/// <summary>
/// 消息序列化帮助类
/// </summary>
public static class ProtoBufSerializerHelper
{
    private static readonly RecyclableMemoryStreamManager Manager = new();

    /// <summary>
    /// 序列化对象
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static byte[] Serialize<T>(T value)
    {
        using (var memoryStream = Manager.GetStream())
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
        using (var memoryStream = Manager.GetStream(data))
        {
            return Serializer.Deserialize(type, memoryStream);
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
        using (var memoryStream = Manager.GetStream(data))
        {
            return Serializer.Deserialize(typeof(T), memoryStream) as T;
        }
    }
}