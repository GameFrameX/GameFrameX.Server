/*using GameFrameX.DataBase.State;
using GameFrameX.Serialize.Serialize;
using SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.StartUp.DataBase;

internal class MessageActorDataBaseDecoderHandler : IPackageDecoder<IMessage>
{
    public IMessage Handler(Span<byte> data)
    {
        int readOffset = 0;
        var messageTypeId = data.ReadLong(ref readOffset);
        var messageData = data.ReadBytes(ref readOffset);
        var messageTypeType = CacheStateTypeManager.GetType(messageTypeId);
        var messageObject = (IMessage)SerializerHelper.Deserialize(messageData, messageTypeType);
        return messageObject;
    }

    public IMessage Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        var data = buffer.ToArray();
        var messageObject = Handler(data);
        return messageObject;
    }
}*/