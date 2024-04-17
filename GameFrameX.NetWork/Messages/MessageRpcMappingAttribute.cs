namespace GameFrameX.NetWork.Messages;

/// <summary>
/// RPC 消息
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class MessageRpcMappingAttribute : Attribute
{
    /// <summary>
    /// 请求消息
    /// </summary>
    public IRequestMessage RequestMessage { get; }

    /// <summary>
    /// 响应消息
    /// </summary>
    public IResponseMessage ResponseMessage { get; }

    public MessageRpcMappingAttribute(IRequestMessage requestMessage, IResponseMessage responseMessage)
    {
        Utility.Guard.NotNull(requestMessage, nameof(requestMessage));
        Utility.Guard.NotNull(responseMessage, nameof(responseMessage));
        RequestMessage = requestMessage;
        ResponseMessage = responseMessage;
    }
}