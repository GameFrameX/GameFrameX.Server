using GameFrameX.NetWork.Messages;
using GameFrameX.Utility;

namespace GameFrameX.NetWork;

public class RpcData
{
    /// <summary>
    /// 消息的唯一ID
    /// </summary>
    public long UniqueId { get; }

    /// <summary>
    /// 是否需要回复
    /// </summary>
    public bool IsReply { get; }

    /// <summary>
    /// 请求消息
    /// </summary>
    public IRequestMessage RequestMessage { get; protected set; }

    /// <summary>
    /// 响应消息
    /// </summary>
    public IResponseMessage ResponseMessage { get; protected set; }

    public void Reply(IActorResponseMessage actorResponseMessage)
    {
        ResponseMessage = actorResponseMessage;
        tcs.SetResult(actorResponseMessage);
    }

    public static RpcData Create(IActorRequestMessage actorRequestMessage, bool isReply = true)
    {
        var defaultMessageActorObject = new RpcData(actorRequestMessage, isReply);

        return defaultMessageActorObject;
    }

    public RpcData(IActorRequestMessage requestMessage, bool isReply = true)
    {
        RequestMessage = requestMessage;
        IsReply = isReply;
        UniqueId = UtilityIdGenerator.GetNextUniqueId();
        tcs = new TaskCompletionSource<IActorResponseMessage>();
    }

    private readonly TaskCompletionSource<IActorResponseMessage> tcs;
    public Task<IActorResponseMessage> Task => tcs.Task;
}