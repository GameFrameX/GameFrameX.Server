// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

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