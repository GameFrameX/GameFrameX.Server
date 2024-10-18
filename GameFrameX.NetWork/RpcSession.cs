using System.Collections.Concurrent;
using GameFrameX.NetWork.Abstractions;
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
    /// 异步调用,且等待返回
    /// </summary>
    /// <param name="message">调用消息对象</param>
    /// <param name="timeOutMillisecond">调用超时,单位毫秒,默认10秒</param>
    /// <returns>返回消息对象</returns>
    public Task<IResponseMessage> Call(IRequestMessage message, int timeOutMillisecond = 10000)
    {
        var rpcData = RpcData.Create(message);
        _waitingObjects.Enqueue(rpcData);
        return rpcData.Task;
    }


    /// <summary>
    /// 异步发送,不等待结果
    /// </summary>
    /// <param name="message">调用消息对象</param>
    public void Send(IRequestMessage message)
    {
        var actorObject = RpcData.Create(message);
        _waitingObjects.Enqueue(actorObject);
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