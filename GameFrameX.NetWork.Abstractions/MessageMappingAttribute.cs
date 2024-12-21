namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 消息类型对应的消息映射处理器
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class MessageMappingAttribute : Attribute
{
    /// <summary>
    /// 构造消息类型对应的消息映射处理器
    /// </summary>
    /// <param name="messageType">消息处理类型</param>
    public MessageMappingAttribute(Type messageType)
    {
        MessageType = messageType;
    }

    /// <summary>
    /// 处理类型
    /// </summary>
    public Type MessageType { get; }
}