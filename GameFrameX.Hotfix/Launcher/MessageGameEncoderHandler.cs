using System.Buffers;
using GameFrameX.Core.Net.BaseHandler;
using GameFrameX.Launcher.StartUp;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Messages;
using GameFrameX.Serialize.Serialize;
using SuperSocket.ProtoBase;

namespace GameFrameX.Hotfix.Launcher;

public class MessageGameEncoderHandler : BaseMessageEncoderHandler
{
    protected override int GetActorMessageId(Type messageType)
    {
        return ProtoMessageIdHandler.GetRequestActorMessageIdByType(messageType);
    }

    protected override int GetMessageId(Type messageType)
    {
        return ProtoMessageIdHandler.GetRequestMessageIdByType(messageType);
    }

    protected override int GetRpcMessageId(Type messageType)
    {
        return ProtoMessageIdHandler.GetRequestMessageIdByType(messageType);
    }
}