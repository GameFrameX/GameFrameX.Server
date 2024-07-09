using System.Buffers;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.SuperSocket.ProtoBase;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 基础消息解码处理器
/// </summary>
public class BaseMessageDecoderHandler : IMessageDecoderHandler, IPackageDecoder<IMessage>
{
    /// <summary>
    /// 和客户端之间的消息 数据长度(2)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public virtual IMessage Handler(byte[] data)
    {
        try
        {
            int readOffset = 0;
            var length = data.ReadUShort(ref readOffset);
            // 消息类型
            var operationType = data.ReadByte(ref readOffset);
            // 压缩标记
            var zipFlag = data.ReadByte(ref readOffset);
            var uniqueId = data.ReadInt(ref readOffset);
            var messageId = data.ReadInt(ref readOffset);
            var messageData = data.ReadBytes(readOffset, length - readOffset);

            if (zipFlag > 0)
            {
                if (DecompressHandler == null)
                {
                    LogHelper.Fatal("未设置解压消息处理器, 请先设置解压消息处理器");
                    return null;
                }

                messageData = DecompressHandler.Handler(messageData);
            }

            var messageType = MessageProtoHelper.GetMessageTypeById(messageId);
            if (messageType != null)
            {
                var message = ProtoBufSerializerHelper.Deserialize(messageData, messageType);
                if (message is MessageObject messageObject)
                {
                    messageObject.MessageId = messageId;
                    messageObject.SetMessageOperationType(operationType);
                    messageObject.SetUniqueId(uniqueId);
                    return messageObject;
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
    /// 解压消息处理器
    /// </summary>
    protected IMessageDecompressHandler DecompressHandler { get; private set; }

    /// <summary>
    /// 设置解压消息处理器
    /// </summary>
    /// <param name="decompressHandler">解压消息处理器</param>
    public void SetDecompressionHandler(IMessageDecompressHandler decompressHandler = null)
    {
        DecompressHandler = decompressHandler;
    }

    public IMessage Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        return Handler(buffer.ToArray());
    }
}