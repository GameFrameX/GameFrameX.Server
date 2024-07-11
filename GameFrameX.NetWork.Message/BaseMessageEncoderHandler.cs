using System.Buffers;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.SuperSocket.ProtoBase;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 基础消息编码处理器
/// </summary>
public abstract class BaseMessageEncoderHandler : IMessageEncoderHandler, IPackageEncoder<INetworkMessage>
{
    /// <summary>
    /// 获取消息业务类型
    /// </summary>
    /// <param name="messageType">消息对象类型</param>
    /// <returns></returns>
    public virtual MessageOperationType GetMessageOperationType(Type messageType)
    {
        var isHeartbeat = MessageProtoHelper.IsHeartbeat(messageType);
        return isHeartbeat ? MessageOperationType.HeartBeat : MessageOperationType.Game;
    }

    /// <summary>
    /// 和客户端之间的消息 数据长度(2)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public virtual byte[] Handler(INetworkMessage message)
    {
        if (message is MessageObject messageObject)
        {
            var messageType = message.GetType();

            var messageId = MessageProtoHelper.GetMessageIdByType(messageType);
            messageObject.SetMessageId(messageId);
            var  uniqueId = messageObject.UniqueId;
            var  bytes    = ProtoBufSerializerHelper.Serialize(messageObject);
            byte zipFlag  = 0;
            if (CompressHandler != null)
            {
                zipFlag = 1;
                // 压缩
                bytes = CompressHandler.Handler(bytes);
            }

            var len    = (ushort)(PackageLength + bytes.Length);
            var span   = new byte[len];
            int offset = 0;
            span.WriteUShort(len, ref offset);
            span.WriteByte((byte)GetMessageOperationType(messageType), ref offset);
            span.WriteByte(zipFlag, ref offset);
            span.WriteInt(uniqueId, ref offset);
            span.WriteInt(messageId, ref offset);
            span.WriteBytesWithoutLength(bytes, ref offset);
            return span;
        }

        LogHelper.Error("消息对象为空，编码异常");
        return null;
    }

    /// <summary>
    /// 压缩消息处理器
    /// </summary>
    protected IMessageCompressHandler CompressHandler { get; private set; }

    /// <summary>
    /// 设置压缩消息处理器
    /// </summary>
    /// <param name="compressHandler">压缩消息处理器</param>
    public void SetCompressionHandler(IMessageCompressHandler compressHandler = null)
    {
        CompressHandler = compressHandler;
    }

    /// <summary>
    /// len +cmdType + zipFlag+uniqueId + msgId + bytes.length
    /// </summary>
    public virtual ushort PackageLength { get; } = 2 + 1 + 1 + 4 + 4;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="pack"></param>
    /// <returns></returns>
    public int Encode(IBufferWriter<byte> writer, INetworkMessage pack)
    {
        var bytes = Handler(pack);
        writer.Write(bytes);
        return bytes.Length;
    }
}