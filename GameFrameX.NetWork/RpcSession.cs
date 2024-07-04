using System.Collections.Concurrent;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.NetWork;

/// <summary>
/// RPC会话
/// </summary>
public sealed class RpcSession : IRpcSession
{
    /// <summary>
    /// 等待队列
    /// </summary>
    private readonly ConcurrentQueue<RpcData> _waitingObjects = new ConcurrentQueue<RpcData>();

    /// <summary>
    /// RPC处理队列
    /// </summary>
    private readonly ConcurrentDictionary<long, RpcData> _rpcHandlingObjects = new ConcurrentDictionary<long, RpcData>();

    /// <summary>
    /// 处理消息队列
    /// </summary>
    /// <returns></returns>
    public RpcData Handler()
    {
        if (_waitingObjects.TryDequeue(out var message))
        {
            if (message.IsReply)
            {
                _rpcHandlingObjects.TryAdd(message.UniqueId, message);
            }

            return message;
        }

        return null;
    }

    /// <summary>
    /// 调用
    /// </summary>
    /// <param name="message"></param>
    public void Call(RpcData message)
    {
        _waitingObjects.Enqueue(message);
    }

    /// <summary>
    /// 回复
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public bool Reply(IResponseMessage message)
    {
        if (_rpcHandlingObjects.TryRemove(message.UniqueId, out var rpcData))
        {
            rpcData.Reply(message);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 异步调用
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task<IResponseMessage> Call(IRequestMessage message)
    {
        var defaultMessageActorObject = RpcData.Create(message);
        _waitingObjects.Enqueue(defaultMessageActorObject);
        return defaultMessageActorObject.Task;
    }

    /// <summary>
    /// 发送
    /// </summary>
    /// <param name="message"></param>
    public void Send(IRequestMessage message)
    {
        var defaultMessageActorObject = RpcData.Create(message);
        _waitingObjects.Enqueue(defaultMessageActorObject);
    }

    /// <summary>
    /// 添加
    /// </summary>
    /// <param name="rpcData"></param>
    public void Add(RpcData rpcData)
    {
        _waitingObjects.Enqueue(rpcData);
    }
}