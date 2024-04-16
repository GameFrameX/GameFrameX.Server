/*using GameFrameX.DBServer.State;
using GameFrameX.Extension;
using GameFrameX.Serialize.Serialize;
using SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.Message;

internal class MessageActorDataBaseDecoderHandler : IPackageDecoder<ICacheState>
{
    public ICacheState Handler(Span<byte> data)
    {
        int readOffset = 0;
        var messageTypeId = data.ReadLong(ref readOffset);
        var messageData = data.ReadBytes(ref readOffset);
        var messageTypeType = CacheStateTypeManager.GetType(messageTypeId);
        var messageObject = (ICacheState)SerializerHelper.Deserialize(messageData, messageTypeType);
        return messageObject;
    }

    public ICacheState Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        var data = buffer.ToArray();
        var messageObject = Handler(data);
        return messageObject;
    }
}*/