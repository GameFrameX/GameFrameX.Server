using System.Text;

namespace GameFrameX.Core.Config;

/// <summary>
/// 提供字符串处理的静态方法。
/// </summary>
public static class StringUtil
{
    /// <summary>
    /// 将对象转换为字符串表示形式。
    /// </summary>
    /// <param name="o">要转换的对象。</param>
    /// <returns>对象的字符串表示形式。</returns>
    public static string ToStr(object o)
    {
        return ToStr(o, new StringBuilder());
    }

    /// <summary>
    /// 将对象转换为字符串表示形式，并使用提供的StringBuilder进行构建。
    /// </summary>
    /// <param name="o">要转换的对象。</param>
    /// <param name="sb">用于构建字符串的StringBuilder。</param>
    /// <returns>对象的字符串表示形式。</returns>
    public static string ToStr(object o, StringBuilder sb)
    {
        foreach (var p in o.GetType().GetFields())
        {
            sb.Append($"{p.Name} = {p.GetValue(o)},");
        }

        foreach (var p in o.GetType().GetProperties())
        {
            sb.Append($"{p.Name} = {p.GetValue(o)},");
        }

        return sb.ToString();
    }

    /// <summary>
    /// 将数组转换为字符串表示形式。
    /// </summary>
    /// <param name="arr">要转换的数组。</param>
    /// <typeparam name="T">数组元素的类型。</typeparam>
    /// <returns>数组的字符串表示形式。</returns>
    public static string ArrayToString<T>(T[] arr)
    {
        return "[" + string.Join(",", arr) + "]";
    }

    /// <summary>
    /// 将集合转换为字符串表示形式。
    /// </summary>
    /// <param name="arr">要转换的集合。</param>
    /// <typeparam name="T">集合元素的类型。</typeparam>
    /// <returns>集合的字符串表示形式。</returns>
    public static string CollectionToString<T>(IEnumerable<T> arr)
    {
        return "[" + string.Join(",", arr) + "]";
    }

    /// <summary>
    /// 将字典转换为字符串表示形式。
    /// </summary>
    /// <param name="dic">要转换的字典。</param>
    /// <typeparam name="TK">字典键的类型。</typeparam>
    /// <typeparam name="TV">字典值的类型。</typeparam>
    /// <returns>字典的字符串表示形式。</returns>
    public static string CollectionToString<TK, TV>(IDictionary<TK, TV> dic)
    {
        var sb = new StringBuilder('{');
        foreach (var e in dic)
        {
            sb.Append(e.Key).Append(':');
            sb.Append(e.Value).Append(',');
        }

        sb.Append('}');
        return sb.ToString();
    }
}