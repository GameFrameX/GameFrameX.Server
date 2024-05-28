using GameFrameX.Serialize.Serialize;
using SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.StartUp;

/// <summary>
/// 消息结构 4 + 4 + 4 + bytes.length
/// </summary>
public class BaseMessageDecoderHandler : IMessageDecoderHandler, IPackageDecoder<IMessage>
{
    /// <summary>
    /// 和客户端之间的消息 数据长度(4)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public virtual IMessage? Handler(byte[] data)
    {
        try
        {
            int readOffset = 0;
            var length = data.ReadInt(ref readOffset);
            var uniqueId = data.ReadLong(ref readOffset);
            var messageId = data.ReadInt(ref readOffset);
            var messageData = data.ReadBytes(ref readOffset);
            var messageType = ProtoMessageIdHandler.GetRequestMessageTypeById(messageId);
            if (messageType != null)
            {
                var messageObject = SerializerHelper.Deserialize(messageData, messageType);
                if (messageObject is MessageObject messageObject1)
                {
                    messageObject1.MessageId = messageId;
                    messageObject1.UniqueId = uniqueId;
                    return messageObject1;
                }

                if (messageObject is MessageActorObject messageActorObject)
                {
                    messageActorObject.MessageId = messageId;
                    messageActorObject.UniqueId = uniqueId;
                    return messageActorObject;
                }
            }

            LogHelper.Fatal("未知消息类型");
            return null;
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            return null;
        }
    }

    /// <summary>
    /// 服务器之间的消息 数据长度(4)+消息唯一ID(8)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public virtual IMessage? RpcHandler(byte[] data)
    {
        try
        {
            int readOffset = 0;
            var length = data.ReadInt(ref readOffset);
            var uniqueId = data.ReadLong(ref readOffset);
            var messageId = data.ReadInt(ref readOffset);
            var messageData = data.ReadBytes(ref readOffset);
            var messageType = ProtoMessageIdHandler.GetResponseMessageTypeById(messageId);
            if (messageType != null)
            {
                var message = SerializerHelper.Deserialize(messageData, messageType);
                if (message is MessageObject messageObject)
                {
                    messageObject.MessageId = messageId;
                    messageObject.UniqueId = uniqueId;
                    return messageObject;
                }

                if (message is MessageActorObject messageActorObject)
                {
                    messageActorObject.MessageId = messageId;
                    messageActorObject.UniqueId = uniqueId;
                    return messageActorObject;
                }
            }

            LogHelper.Fatal("未知消息类型");
            return null;
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            return null;
        }
    }

    public IMessage Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        return Handler(buffer.ToArray());
    }
}