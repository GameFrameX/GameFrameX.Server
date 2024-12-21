using GameFrameX.NetWork.Abstractions;
using GameFrameX.Utility;

namespace GameFrameX.NetWork;

/// <summary>
/// RPC 数据
/// </summary>
public sealed class RpcData : IDisposable
{
    private readonly TaskCompletionSource<IRpcResult> _tcs;

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="requestMessage">请求消息</param>
    /// <param name="isReply">是否需要回复</param>
    /// <param name="timeout">超时时间,单位毫秒,默认10秒</param>
    private RpcData(IRequestMessage requestMessage, bool isReply = true, int timeout = 10000)
    {
        CreatedTime = TimeHelper.UnixTimeMilliseconds();
        RequestMessage = requestMessage;
        IsReply = isReply;
        Timeout = timeout;
        _tcs = new TaskCompletionSource<IRpcResult>();
    }

    /// <summary>
    /// 消息的唯一ID
    /// 从RequestMessage中获得
    /// </summary>
    public long UniqueId
    {
        get { return RequestMessage.UniqueId; }
    }

    /// <summary>
    /// 是否需要回复
    /// </summary>
    public bool IsReply { get; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public long CreatedTime { get; }

    /// <summary>
    /// 计时器消耗的时间
    /// </summary>
    private long ElapseTime { get; set; }

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
    /// RPC 耗时时间.单位毫秒
    /// 从创建到回复的时间差
    /// </summary>
    public long Time { get; private set; }

    /// <summary>
    /// RPC 回复任务
    /// </summary>
    public Task<IRpcResult> Task
    {
        get { return _tcs.Task; }
    }

    /// <summary>
    /// </summary>
    public void Dispose()
    {
        ElapseTime = 0;
        RequestMessage = null;
        ResponseMessage = null;
        Time = 0;
        _tcs?.TrySetCanceled();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// RPC 回复
    /// </summary>
    /// <param name="responseMessage"></param>
    public void Reply(IResponseMessage responseMessage)
    {
        ResponseMessage = responseMessage;
        Time = TimeHelper.UnixTimeMilliseconds() - CreatedTime;
        var result = new RpcResult(responseMessage);
        _tcs.SetResult(result);
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
    /// 增加时间。如果超时返回true
    /// </summary>
    /// <param name="millisecondsTime">流逝时间.单位毫秒</param>
    /// <returns></returns>
    internal bool IncrementalElapseTime(long millisecondsTime)
    {
        ElapseTime += millisecondsTime;
        if (ElapseTime >= Timeout)
        {
            var error = "Rpc call timeout! Message is :" + RequestMessage;
            _tcs.TrySetResult(new RpcResult(error));
            return true;
        }

        return false;
    }

    /// <summary>
    /// 析构函数
    /// </summary>
    ~RpcData()
    {
        Dispose();
    }
}