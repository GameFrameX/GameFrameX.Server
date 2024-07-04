using GameFrameX.NetWork.Messages;
using GameFrameX.Utility;

namespace GameFrameX.NetWork;

/// <summary>
/// RPC 数据
/// </summary>
public sealed class RpcData
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
    public IRequestMessage RequestMessage { get; private set; }

    /// <summary>
    /// 响应消息
    /// </summary>
    public IResponseMessage ResponseMessage { get; private set; }

    /// <summary>
    /// RPC 回复
    /// </summary>
    /// <param name="actorResponseMessage"></param>
    public void Reply(IResponseMessage actorResponseMessage)
    {
        ResponseMessage = actorResponseMessage;
        _tcs.SetResult(actorResponseMessage);
    }

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="actorRequestMessage"></param>
    /// <param name="isReply"></param>
    /// <returns></returns>
    public static RpcData Create(IRequestMessage actorRequestMessage, bool isReply = true)
    {
        var defaultMessageActorObject = new RpcData(actorRequestMessage, isReply);

        return defaultMessageActorObject;
    }

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="requestMessage"></param>
    /// <param name="isReply"></param>
    private RpcData(IRequestMessage requestMessage, bool isReply = true)
    {
        RequestMessage = requestMessage;
        IsReply = isReply;
        UniqueId = UtilityIdGenerator.GetNextUniqueId();
        _tcs = new TaskCompletionSource<IResponseMessage>();
    }

    private readonly TaskCompletionSource<IResponseMessage> _tcs;

    /// <summary>
    /// RPC 回复任务
    /// </summary>
    public Task<IResponseMessage> Task => _tcs.Task;
}