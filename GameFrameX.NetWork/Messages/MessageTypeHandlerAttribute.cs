namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 消息类型处理器标记
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class MessageTypeHandlerAttribute : Attribute
{
    /// <summary>
    /// 消息ID
    /// </summary>
    public int MessageId { get; }

    /// <summary>
    /// 主消息ID
    /// </summary>
    public int MainId { get; }

    /// <summary>
    /// 子消息ID
    /// </summary>
    public int SubId { get; }

    /// <summary>
    /// 构造消息类型处理器
    /// </summary>
    /// <param name="messageId">消息ID</param>
    public MessageTypeHandlerAttribute(int messageId)
    {
        MessageId = messageId;
    }
}