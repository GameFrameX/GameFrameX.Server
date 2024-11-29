using GameFrameX.Log;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;
using Microsoft.Extensions.ObjectPool;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 基础消息编码处理器
/// </summary>
public sealed class DefaultMessageEncoderHandler : BaseMessageEncoderHandler
{
    private readonly ObjectPool<MessageObjectHeader> _messageObjectHeaderObjectPool;

    /// <summary>
    /// 
    /// </summary>
    public DefaultMessageEncoderHandler()
    {
        _messageObjectHeaderObjectPool = new DefaultObjectPoolProvider().Create<MessageObjectHeader>();
    }

    /// <summary>
    /// 和客户端之间的消息 数据长度(2)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public override byte[] Handler(IMessage message)
    {
        if (message is MessageObject messageObject)
        {
            MessageProtoHelper.SetMessageIdAndOperationType(messageObject);
            var messageBodyData = ProtoBufSerializerHelper.Serialize(messageObject);
            byte zipFlag = 0;
            BytesCompressHandler(ref messageBodyData, ref zipFlag);
            var messageObjectHeader = _messageObjectHeaderObjectPool.Get();
            messageObjectHeader.OperationType = messageObject.OperationType;
            messageObjectHeader.UniqueId = messageObject.UniqueId;
            messageObjectHeader.MessageId = messageObject.MessageId;
            messageObjectHeader.ZipFlag = zipFlag;
            var messageHeaderData = ProtoBufSerializerHelper.Serialize(messageObjectHeader);
            _messageObjectHeaderObjectPool.Return(messageObjectHeader);
            return InnerBufferHandler(messageBodyData, ref messageHeaderData);
        }

        LogHelper.Error("消息对象为空，编码异常");
        return null;
    }
}