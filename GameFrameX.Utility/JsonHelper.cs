using GameFrameX.Extension;

namespace GameFrameX.Utility;

/// <summary>
/// Json 帮助类
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// 序列化对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string Serialize(object obj)
    {
        obj.CheckNotNull(nameof(obj));
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        return json;
    }

    /// <summary>
    /// 序列化对象且格式化JSON
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string SerializeFormat(object obj)
    {
        obj.CheckNotNull(nameof(obj));
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
        return json;
    }

    /// <summary>
    /// 反序列化对象
    /// </summary>
    /// <param name="json"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Deserialize<T>(string json) where T : class, new()
    {
        json.CheckNotNullOrEmpty(nameof(json));
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
    }

    /// <summary>
    /// 反序列化对象
    /// </summary>
    /// <param name="json"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object Deserialize(string json, Type type)
    {
        json.CheckNotNullOrEmpty(nameof(json));
        type.CheckNotNull(nameof(type));
        return Newtonsoft.Json.JsonConvert.DeserializeObject(json, type);
    }
}