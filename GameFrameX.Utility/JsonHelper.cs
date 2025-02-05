using System.Text.Json;
using System.Text.Json.Serialization;
using GameFrameX.Utility.Extensions;

namespace GameFrameX.Utility;

/// <summary>
/// Json 帮助类
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// 默认序列化配置
    /// </summary>
    public readonly static JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
    {
        // 忽略 null 值
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        // 忽略循环引用
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        // 大小写判断
        PropertyNameCaseInsensitive = true,
        // 添加自定义转换器，类似于 StringEnumConverter
        Converters =
        {
            new JsonStringEnumConverter() // 处理枚举为字符串
        },
    };

    /// <summary>
    /// 格式化序列化配置
    /// </summary>
    public readonly static JsonSerializerOptions FormatOptions = new JsonSerializerOptions
    {
        // 忽略 null 值
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        // 忽略循环引用
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        // 大小写判断
        PropertyNameCaseInsensitive = true,
        // 格式化 JSON
        WriteIndented = true,
        // 添加自定义转换器，类似于 StringEnumConverter
        Converters =
        {
            new JsonStringEnumConverter() // 处理枚举为字符串
        },
    };


    /// <summary>
    /// 序列化对象
    /// </summary>
    /// <param name="obj">需要序列化的对象</param>
    /// <returns>序列化后的 JSON 字符串</returns>
    public static string Serialize(object obj)
    {
        obj.CheckNotNull(nameof(obj));
        var json = JsonSerializer.Serialize(obj, DefaultOptions);
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
        var json = JsonSerializer.Serialize(obj, FormatOptions);
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
        return JsonSerializer.Deserialize<T>(json, DefaultOptions);
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
        return JsonSerializer.Deserialize(json, type, DefaultOptions);
    }
}