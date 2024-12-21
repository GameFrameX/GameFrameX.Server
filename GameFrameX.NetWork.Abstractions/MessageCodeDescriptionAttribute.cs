namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 消息错误码的描述
/// </summary>
[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property)]
public sealed class MessageCodeDescriptionAttribute : Attribute
{
    /// <summary>
    /// 构造消息错误码的描述
    /// </summary>
    /// <param name="description">错误码描述</param>
    public MessageCodeDescriptionAttribute(string description)
    {
        Description = description;
    }

    /// <summary>
    /// 消息错误码的描述
    /// </summary>
    public string Description { get; }
}