namespace GameFrameX.Utility;

public static class JsonHelper
{
    /// <summary>
    /// 序列化对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string Serialize(object obj)
    {
        Guard.NotNull(obj, nameof(obj));
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
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
        Guard.NotNull(json, nameof(json));
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
    }
}