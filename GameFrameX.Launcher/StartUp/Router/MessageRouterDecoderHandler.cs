using GameFrameX.Extension;
using GameFrameX.NetWork;
using GameFrameX.Serialize.Serialize;
using SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.StartUp.Router;

/// <summary>
/// 消息结构 4 + 4 + 4 + bytes.length
/// </summary>
public class MessageRouterDecoderHandler : IMessageDecoderHandler, IPackageDecoder<IMessage>
{
    /// <summary>
    /// 和客户端之间的消息 数据长度(4)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public IMessage Handler(byte[] data)
    {
        try
        {
            int readOffset = 0;
            var length = data.ReadInt(ref readOffset);
            var uniqueId = data.ReadInt(ref readOffset);
            var messageId = data.ReadInt(ref readOffset);
            var messageData = data.ReadBytes(ref readOffset);
            var messageType = ProtoMessageIdHandler.GetReqTypeById(messageId);
            if (messageType != null)
            {
                var messageObject = (MessageObject)SerializerHelper.Deserialize(messageData, messageType);
                messageObject.MessageId = messageId;
                messageObject.UniqueId = uniqueId;
                return messageObject;
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
    public IMessage RpcHandler(byte[] data)
    {
        try
        {
            int readOffset = 0;
            var length = data.ReadInt(ref readOffset);
            var uniqueId = data.ReadLong(ref readOffset);
            var messageId = data.ReadInt(ref readOffset);
            var messageData = data.ReadBytes(ref readOffset);
            var messageType = ProtoMessageIdHandler.GetResponseActorTypeById(messageId);
            if (messageType != null)
            {
                var messageObject = (MessageActorObject)SerializerHelper.Deserialize(messageData, messageType);
                messageObject.MessageId = messageId;
                messageObject.UniqueId = uniqueId;
                return messageObject;
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