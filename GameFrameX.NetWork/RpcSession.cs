// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Collections.Concurrent;
using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.NetWork;

/// <summary>
/// RPC会话
/// </summary>
public sealed class RpcSession : IRpcSession, IDisposable
{
    /// <summary>
    /// 删除列表
    /// </summary>
    private readonly HashSet<long> _removeUniqueIds = new();

    /// <summary>
    /// RPC处理队列
    /// </summary>
    private readonly ConcurrentDictionary<long, RpcData> _rpcHandlingObjects = new();

    /// <summary>
    /// 等待队列
    /// </summary>
    private readonly ConcurrentQueue<RpcData> _waitingObjects = new();

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }


    /// <summary>
    /// 异步调用,且等待返回
    /// </summary>
    /// <param name="message">调用消息对象</param>
    /// <param name="timeOutMillisecond">调用超时,单位毫秒,默认10秒</param>
    /// <returns>返回消息对象</returns>
    public Task<IRpcResult> Call(IRequestMessage message, int timeOutMillisecond = 10000)
    {
        var rpcData = RpcData.Create(message, true, timeOutMillisecond);
        _waitingObjects.Enqueue(rpcData);
        return rpcData.Task;
    }

    /// <summary>
    /// 异步发送,不等待结果
    /// </summary>
    /// <param name="message">调用消息对象</param>
    public void Send(IRequestMessage message)
    {
        var actorObject = RpcData.Create(message, false);
        _waitingObjects.Enqueue(actorObject);
    }

    /// <summary>
    /// 处理消息队列
    /// </summary>
    /// <returns></returns>
    public RpcData TryPeek()
    {
        if (_waitingObjects.TryPeek(out var message))
        {
            return message;
        }

        return null;
    }

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
    /// 计时器
    /// </summary>
    /// <param name="elapseMillisecondsTime">流逝时间,单位毫秒</param>
    public void Tick(int elapseMillisecondsTime)
    {
        if (_rpcHandlingObjects.Count > 0)
        {
            var elapseSecondsTime = (long)elapseMillisecondsTime;
            _removeUniqueIds.Clear();
            foreach (var handlingObject in _rpcHandlingObjects)
            {
                var isTimeout = handlingObject.Value.IncrementalElapseTime(elapseSecondsTime);
                if (isTimeout)
                {
                    _removeUniqueIds.Add(handlingObject.Key);
                }
            }
        }

        if (_removeUniqueIds.Count > 0)
        {
            foreach (var uniqueId in _removeUniqueIds)
            {
                _rpcHandlingObjects.TryRemove(uniqueId, out _);
            }

            _removeUniqueIds.Clear();
        }
    }

    /// <summary>
    /// 停止
    /// </summary>
    public void Stop()
    {
        _removeUniqueIds?.Clear();
        _waitingObjects?.Clear();
        _rpcHandlingObjects?.Clear();
    }

    /// <summary>
    /// 析构函数
    /// </summary>
    ~RpcSession()
    {
        Dispose();
    }
}