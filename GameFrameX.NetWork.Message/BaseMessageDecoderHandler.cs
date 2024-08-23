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
public class BaseMessageDecoderHandler : IMessageDecoderHandler, IPackageDecoder<INetworkMessage>
{
    /// <summary>
    /// 消息头长度
    /// </summary>
    public virtual int MessageHeaderLength { get; } = 12;

    /// <summary>
    /// 和客户端之间的消息 数据长度(2)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public virtual INetworkMessage Handler(byte[] data)
    {
        ReadOnlySequence<byte> sequence = new ReadOnlySequence<byte>(data);
        return Handler(ref sequence);
    }

    private bool DecodeNetworkMessage(byte zipFlag, byte[] messageData, int messageId, byte operationType, int uniqueId, out INetworkMessage networkMessage)
    {
        networkMessage = null;
        if (zipFlag > 0)
        {
            if (DecompressHandler == null)
            {
                LogHelper.Fatal("未设置解压消息处理器, 请先设置解压消息处理器");
                {
                    return true;
                }
            }

            messageData = DecompressHandler.Handler(messageData);
        }

        var messageType = MessageProtoHelper.GetMessageTypeById(messageId);
        if (messageType != null)
        {
            var message = ProtoBufSerializerHelper.Deserialize(messageData, messageType);
            if (message is MessageObject messageObject)
            {
                messageObject.SetMessageId(messageId);
                messageObject.SetOperationType((MessageOperationType)operationType);
                messageObject.SetUniqueId(uniqueId);
                {
                    networkMessage = messageObject;
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    public INetworkMessage Handler(ref ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        try
        {
            reader.TryReadBigEndian(out ushort length);
            // 消息类型
            reader.TryReadBigEndian(out byte operationType);
            // 压缩标记
            reader.TryReadBigEndian(out byte zipFlag);
            // 消息唯一ID
            reader.TryReadBigEndian(out int uniqueId);
            // 消息ID
            reader.TryReadBigEndian(out int messageId);

            reader.TryReadBytes(length - MessageHeaderLength, out var messageData);

            if (DecodeNetworkMessage(zipFlag, messageData, messageId, operationType, uniqueId, out var networkMessage))
            {
                return networkMessage;
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

    /// <summary>
    /// 解码
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public INetworkMessage Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        return Handler(ref buffer);
    }
}