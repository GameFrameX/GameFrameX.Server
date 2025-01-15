using GameFrameX.Utility.Extensions;
using Newtonsoft.Json;

namespace GameFrameX.Utility;

/// <summary>
/// Json 帮助类
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// 序列化对象
    /// </summary>
    /// <param name="obj">需要序列化的对象</param>
    /// <returns>序列化后的 JSON 字符串</returns>
    public static string Serialize(object obj)
    {
        obj.CheckNotNull(nameof(obj));
        var json = JsonConvert.SerializeObject(obj);
        return json;
    }

    /// <summary>
    /// 序列化对象且格式化JSON
    /// </summary>
    /// <param name="obj">需要序列化的对象</param>
    /// <returns>序列化后的 JSON 字符串</returns>
    public static string SerializeFormat(object obj)
    {
        obj.CheckNotNull(nameof(obj));
        var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
        return json;
    }

    /// <summary>
    /// 反序列化对象
    /// </summary>
    /// <param name="json">需要反序列化的 JSON 字符串</param>
    /// <typeparam name="T">反序列化后的对象类型</typeparam>
    /// <returns>反序列化后的对象</returns>
    public static T Deserialize<T>(string json) where T : class, new()
    {
        json.CheckNotNullOrEmpty(nameof(json));
        return JsonConvert.DeserializeObject<T>(json);
    }

    /// <summary>
    /// 反序列化对象
    /// </summary>
    /// <param name="json">需要反序列化的 JSON 字符串</param>
    /// <param name="type">反序列化后的对象类型</param>
    /// <returns>反序列化后的对象</returns>
    public static object Deserialize(string json, Type type)
    {
        json.CheckNotNullOrEmpty(nameof(json));
        type.CheckNotNull(nameof(type));
        return JsonConvert.DeserializeObject(json, type);
    }
}