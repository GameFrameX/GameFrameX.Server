using System.Collections.Concurrent;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.NetWork;

public sealed class RpcSession : IRpcSession
{
    private readonly ConcurrentQueue<RpcData> waitingObjects = new ConcurrentQueue<RpcData>();
    private readonly ConcurrentQueue<RpcData> handlingObjects = new ConcurrentQueue<RpcData>();

    public RpcData Handler()
    {
        if (waitingObjects.TryDequeue(out var message))
        {
            if (message.IsReply)
            {
                handlingObjects.Enqueue(message);
            }

            return message;
        }

        return null;
    }

    public void Call(RpcData message)
    {
        waitingObjects.Enqueue(message);
    }

    public void Reply(IActorResponseMessage message)
    {
        if (handlingObjects.TryDequeue(out var messageActorObject))
        {
            messageActorObject.Reply(message);
        }
    }

    public Task<IActorResponseMessage> Call(IActorRequestMessage message)
    {
        var defaultMessageActorObject = RpcData.Create(message);
        waitingObjects.Enqueue(defaultMessageActorObject);
        return defaultMessageActorObject.Task;
    }

    public void Send(IActorRequestMessage message)
    {
        var defaultMessageActorObject = RpcData.Create(message);
        waitingObjects.Enqueue(defaultMessageActorObject);
    }

    public void Add(RpcData rpcData)
    {
        waitingObjects.Enqueue(rpcData);
    }
}