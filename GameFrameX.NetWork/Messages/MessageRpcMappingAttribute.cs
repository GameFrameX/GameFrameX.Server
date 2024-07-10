using GameFrameX.Extension;

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestMessage"></param>
    /// <param name="responseMessage"></param>
    public MessageRpcMappingAttribute(IRequestMessage requestMessage, IResponseMessage responseMessage)
    {
        requestMessage.CheckNotNull(nameof(requestMessage));
        responseMessage.CheckNotNull(nameof(responseMessage));
        RequestMessage  = requestMessage;
        ResponseMessage = responseMessage;
    }
}