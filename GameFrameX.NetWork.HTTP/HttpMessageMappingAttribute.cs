using GameFrameX.Foundation.Extensions;

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// HTTP消息处理器属性，用于标记HTTP消息处理器类
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class HttpMessageMappingAttribute : Attribute
{
    /// <summary>
    /// 处理器命名前缀
    /// </summary>
    public const string HTTPprefix = "";

    /// <summary>
    /// 处理器命名后缀
    /// </summary>
    public const string HTTPsuffix = "HttpHandler";

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="classType">处理器类的类型</param>
    /// <exception cref="ArgumentNullException">当classType为null时抛出</exception>
    /// <exception cref="InvalidOperationException">当classType不是密封类或不以HTTPsuffix结尾时抛出</exception>
    public HttpMessageMappingAttribute(Type classType)
    {
        classType.CheckNotNull(nameof(classType));
        var className = classType.Name;
        if (!classType.IsSealed)
        {
            throw new InvalidOperationException($"{className} 必须是标记为sealed的类");
        }

        // if (!className.StartsWith(HTTPprefix, StringComparison.Ordinal))
        // {
        //     throw new InvalidOperationException($"{className} 必须以{HTTPprefix}开头");
        // }

        if (!className.EndsWith(HTTPsuffix, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"{className} 必须以{HTTPsuffix}结尾");
        }

        OriginalCmd = className.Substring(HTTPprefix.Length, className.Length - HTTPprefix.Length - HTTPsuffix.Length);
        StandardCmd = OriginalCmd.ConvertToSnakeCase();
    }

    /// <summary>
    /// 原始命令名称
    /// </summary>
    public string OriginalCmd { get; }

    /// <summary>
    /// 标准化后的命令名称
    /// </summary>
    public string StandardCmd { get; }
}