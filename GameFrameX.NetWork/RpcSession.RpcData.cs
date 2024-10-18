using GameFrameX.NetWork.Abstractions;
using GameFrameX.Utility;

namespace GameFrameX.NetWork;

/// <summary>
/// RPC 数据
/// </summary>
public sealed class RpcData : IDisposable
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
    /// 创建时间
    /// </summary>
    public long CreatedTime { get; }

    /// <summary>
    /// 消耗的时间
    /// </summary>
    public long ElapseTime { get; private set; }

    /// <summary>
    /// 超时时间。单位毫秒
    /// </summary>
    public int Timeout { get; }

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
    /// <param name="requestMessage">请求消息</param>
    /// <param name="isReply">是否需要回复</param>
    /// <param name="timeout">超时时间,单位毫秒</param>
    /// <returns></returns>
    public static RpcData Create(IRequestMessage requestMessage, bool isReply = true, int timeout = 10000)
    {
        var rpcData = new RpcData(requestMessage, isReply, timeout);
        return rpcData;
    }

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="requestMessage">请求消息</param>
    /// <param name="isReply">是否需要回复</param>
    /// <param name="timeout">超时时间,单位毫秒</param>
    private RpcData(IRequestMessage requestMessage, bool isReply = true, int timeout = 10000) : this()
    {
        RequestMessage = requestMessage;
        IsReply = isReply;
        UniqueId = requestMessage.UniqueId;
        Timeout = timeout;
        _tcs = new TaskCompletionSource<IResponseMessage>();
    }

    /// <summary>
    /// 增加时间。如果超时返回true
    /// </summary>
    /// <param name="millisecondsTime">流逝时间.单位毫秒</param>
    /// <returns></returns>
    internal bool IncrementalElapseTime(long millisecondsTime)
    {
        ElapseTime += millisecondsTime;
        if (ElapseTime >= Timeout)
        {
            _tcs.TrySetException(new TimeoutException("Rpc call timeout! Message is :" + RequestMessage));
            return true;
        }

        return false;
    }

    private readonly TaskCompletionSource<IResponseMessage> _tcs;

    /// <summary>
    /// RPC 回复任务
    /// </summary>
    public Task<IResponseMessage> Task
    {
        get { return _tcs.Task; }
    }

    /// <summary>
    /// 
    /// </summary>
    private RpcData()
    {
        CreatedTime = TimeHelper.UnixTimeMilliseconds();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        _tcs?.TrySetCanceled();
        GC.SuppressFinalize(this);
    }
}