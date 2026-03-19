// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================


using System.Collections.Concurrent;
using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.NetWork;

/// <summary>
/// RPC会话。
/// </summary>
/// <remarks>
/// RPC session for managing request-response communication patterns.
/// </remarks>
public sealed class RpcSession : IRpcSession, IDisposable
{
    /// <summary>
    /// 删除列表。
    /// </summary>
    /// <remarks>
    /// List of unique IDs to be removed from the handling queue.
    /// </remarks>
    private readonly HashSet<long> _removeUniqueIds = new();

    /// <summary>
    /// RPC处理队列。
    /// </summary>
    /// <remarks>
    /// Dictionary containing RPC data currently being processed, keyed by unique ID.
    /// </remarks>
    private readonly ConcurrentDictionary<long, IRpcSessionData> _rpcHandlingObjects = new();

    /// <summary>
    /// 等待队列。
    /// </summary>
    /// <remarks>
    /// Queue of RPC data waiting to be processed.
    /// </remarks>
    private readonly ConcurrentQueue<IRpcSessionData> _waitingObjects = new();

    /// <summary>
    /// 释放资源。
    /// </summary>
    /// <remarks>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </remarks>
    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }


    /// <summary>
    /// 异步调用，并等待返回。
    /// </summary>
    /// <remarks>
    /// Asynchronously invokes the RPC call and waits for the response.
    /// </remarks>
    /// <typeparam name="T">响应消息类型 / The type of the response message</typeparam>
    /// <param name="message">调用消息对象 / The request message object</param>
    /// <param name="timeOutMillisecond">调用超时，单位毫秒，默认10秒 / Timeout in milliseconds, defaults to 10 seconds</param>
    /// <returns>返回消息对象 / The response message object</returns>
    public Task<IRpcResult> Call<T>(IRequestMessage message, int timeOutMillisecond = 10000) where T : IResponseMessage, new()
    {
        var rpcData = RpcSessionData.Create(message, true, timeOutMillisecond);
        _waitingObjects.Enqueue(rpcData);
        return rpcData.Task;
    }

    /// <summary>
    /// 异步发送，不等待结果。
    /// </summary>
    /// <remarks>
    /// Asynchronously sends the message without waiting for a response.
    /// </remarks>
    /// <param name="message">调用消息对象 / The request message object</param>
    public void Send(IRequestMessage message)
    {
        var actorObject = RpcSessionData.Create(message, false);
        _waitingObjects.Enqueue(actorObject);
    }

    /// <summary>
    /// 尝试查看消息队列头部元素但不移除。
    /// </summary>
    /// <remarks>
    /// Peeks at the first message in the queue without removing it.
    /// </remarks>
    /// <returns>等待处理的消息对象 / The message object waiting to be processed, or null if the queue is empty</returns>
    public IRpcSessionData TryPeek()
    {
        if (_waitingObjects.TryPeek(out var message))
        {
            return message;
        }

        return null;
    }

    /// <summary>
    /// 处理消息队列，取出并返回队列头部的消息。
    /// </summary>
    /// <remarks>
    /// Dequeues and returns the first message from the waiting queue.
    /// </remarks>
    /// <returns>处理的消息对象 / The message object to be processed, or null if the queue is empty</returns>
    public IRpcSessionData Handler()
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
    /// 回复消息。
    /// </summary>
    /// <remarks>
    /// Replies to a pending RPC request with the specified response message.
    /// </remarks>
    /// <param name="message">回复消息对象 / The response message object</param>
    /// <returns>是否成功回复 / <c>true</c> if the reply was successful; otherwise <c>false</c></returns>
    public bool Reply(IResponseMessage message)
    {
        if (_rpcHandlingObjects == default || message == default)
        {
            return false;
        }

        if (_rpcHandlingObjects.TryRemove(message.UniqueId, out var rpcData))
        {
            if (rpcData != default)
            {
                return rpcData.Reply(message);
            }
        }

        return false;
    }

    /// <summary>
    /// 计时器更新，检查超时的RPC请求。
    /// </summary>
    /// <remarks>
    /// Updates the timer and checks for timed-out RPC requests.
    /// </remarks>
    /// <param name="elapseMillisecondsTime">流逝时间，单位毫秒 / Elapsed time in milliseconds</param>
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
    /// 停止会话，清除所有队列。
    /// </summary>
    /// <remarks>
    /// Stops the session and clears all internal queues.
    /// </remarks>
    public void Stop()
    {
        _removeUniqueIds?.Clear();
        _waitingObjects?.Clear();
        _rpcHandlingObjects?.Clear();
    }

    /// <summary>
    /// 析构函数。
    /// </summary>
    /// <remarks>
    /// Finalizer to ensure resources are released.
    /// </remarks>
    ~RpcSession()
    {
        Dispose();
    }
}