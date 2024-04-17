using System.Text;
using GameFrameX.Extension;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Messages;
using GameFrameX.Serialize.Serialize;
using GameFrameX.Utility;
using SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.Message;

class MessageRouterEncoderHandler : IMessageEncoderHandler, IPackageEncoder<IMessage>
{
    public byte[] Handler(IMessage message)
    {
        var bytes = SerializerHelper.Serialize(message);
        // len +timestamp + msgId + bytes.length
        int len = 4 + 8 + 4 + bytes.Length;
        var span = ArrayPool<byte>.Shared.Rent(len);
        int offset = 0;
        span.WriteInt(len, ref offset);
        span.WriteLong(TimeHelper.UnixTimeSeconds(), ref offset);
        var messageType = message.GetType();
        var msgId = ProtoMessageIdHandler.GetRespMessageIdByType(messageType);
        span.WriteInt(msgId, ref offset);
        span.WriteBytes(bytes, ref offset);
        ArrayPool<byte>.Shared.Return(span);
        LogHelper.Debug($"---发送消息:[{msgId},{message.GetType().Name}] 消息内容:[{message}]");
        return span;
    }

    public int Encode(IBufferWriter<byte> writer, IMessage pack)
    {
        var bytes = Handler(pack);
        writer.Write(bytes);
        ArrayPool<byte>.Shared.Return(bytes);
        return bytes.Length;
    }
}