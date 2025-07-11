// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// HTTP响应消息特性，用于标记HTTP响应消息类型
/// 此特性用于标记HTTP处理器的响应消息类型，确保响应消息类型继承自HttpMessageResponseBase
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class HttpMessageResponseAttribute : Attribute
{
    /// <summary>
    /// 获取响应消息的类型
    /// 该属性存储HTTP响应消息的具体类型，用于运行时的消息处理和序列化
    /// </summary>
    public Type MessageType { get; }

    /// <summary>
    /// 初始化 <see cref="HttpMessageResponseAttribute"/> 的新实例
    /// 构造函数会验证传入的类型是否为有效的HTTP响应消息类型
    /// </summary>
    /// <param name="classType">响应消息的类型，必须继承自HttpMessageResponseBase</param>
    /// <exception cref="ArgumentNullException">当 classType 为 null 时抛出此异常</exception>
    /// <exception cref="InvalidCastException">当 classType 未继承自HttpMessageResponseBase时抛出此异常</exception>
    public HttpMessageResponseAttribute(Type classType)
    {
        ArgumentNullException.ThrowIfNull(classType, nameof(classType));
        var isRequest = classType.IsSubclassOf(typeof(HttpMessageResponseBase));
        if (isRequest == false)
        {
            throw new InvalidCastException($"消息类型 {classType.Name} 必须继承自 {nameof(HttpMessageResponseBase)}");
        }

        MessageType = classType;
    }
}