// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   均受中华人民共和国及相关国际法律法规保护。
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   侵犯他人合法权益等法律法规所禁止的行为！
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   本项目组织与贡献者概不承担。
//   GitHub 仓库：https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System.Buffers;
using System.Buffers.Binary;
using GameFrameX.ProtoBuf.Net;

namespace GameFrameX.NetWork.RemoteMessaging.Transport;

/// <summary>
/// 默认消息编解码器。复用自定义包头结构：4字节总长 + 1字节操作类型 + 1字节算法ID + 4字节唯一ID + 4字节消息ID + ProtoBuf 载荷。
/// </summary>
/// <remarks>
/// Default message codec. Reuses the custom packet header structure: 4-byte total length + 1-byte operation type + 1-byte algorithm ID + 4-byte unique ID + 4-byte message ID + ProtoBuf payload.
/// </remarks>
internal sealed class DefaultMessageCodec : IMessageCodec
{
    /// <summary>
    /// 包头长度（不含自身4字节）：1(operationType) + 1(algorithmId) + 4(uniqueId) + 4(messageId) = 10
    /// 总包头含长度字段：4 + 10 = 14
    /// </summary>
    private const int InnerPackageHeaderLength = 14;
    private readonly int _compressThreshold;
    private readonly byte _defaultCompressionAlgorithmId;
    private readonly IMessageCompressionRegistry _compressionRegistry;

    /// <summary>
    /// 初始化默认消息编解码器（使用默认压缩注册表、Deflate 算法和 512 字节压缩阈值）。
    /// </summary>
    /// <remarks>
    /// Initializes the default message codec (using the default compression registry, Deflate algorithm, and 512-byte compression threshold).
    /// </remarks>
    public DefaultMessageCodec()
        : this(new DefaultMessageCompressionRegistry(), DeflateMessageCompressionAlgorithm.Id, 512)
    {
    }

    /// <summary>
    /// 初始化默认消息编解码器，指定压缩注册表、默认算法 ID 和压缩阈值。
    /// </summary>
    /// <remarks>
    /// Initializes the default message codec with the specified compression registry, default algorithm ID, and compression threshold.
    /// </remarks>
    /// <param name="compressionRegistry">压缩算法注册表 / The compression algorithm registry</param>
    /// <param name="defaultCompressionAlgorithmId">默认压缩算法 ID（0 表示不压缩） / Default compression algorithm ID (0 means no compression)</param>
    /// <param name="compressThreshold">压缩阈值（字节），超过此值才压缩 / Compression threshold in bytes; compression is only attempted above this value</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="compressionRegistry"/> 为 null 时抛出 / Thrown when <paramref name="compressionRegistry"/> is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="compressThreshold"/> 为负数或 <paramref name="defaultCompressionAlgorithmId"/> 未注册时抛出 / Thrown when <paramref name="compressThreshold"/> is negative or <paramref name="defaultCompressionAlgorithmId"/> is not registered</exception>
    public DefaultMessageCodec(
        IMessageCompressionRegistry compressionRegistry,
        byte defaultCompressionAlgorithmId,
        int compressThreshold = 512)
    {
        ArgumentNullException.ThrowIfNull(compressionRegistry);
        if (compressThreshold < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(compressThreshold), "Compression threshold must be greater than or equal to 0.");
        }

        if (defaultCompressionAlgorithmId > 0 && !compressionRegistry.TryGet(defaultCompressionAlgorithmId, out _))
        {
            throw new ArgumentException($"Compression algorithm '{defaultCompressionAlgorithmId}' is not registered.", nameof(defaultCompressionAlgorithmId));
        }

        _compressionRegistry = compressionRegistry;
        _defaultCompressionAlgorithmId = defaultCompressionAlgorithmId;
        _compressThreshold = compressThreshold;
    }

    /// <summary>
    /// 将消息对象编码为二进制包。
    /// </summary>
    /// <remarks>
    /// Encodes a message object into a binary packet.
    /// </remarks>
    /// <param name="message">消息对象 / The message object to encode</param>
    /// <returns>编码后的二进制数据 / The encoded binary data</returns>
    public PooledBuffer Encode(MessageObject message)
    {
        var messageData = ProtoBufSerializerHelper.Serialize(message);
        var algorithmId = (byte)0;
        if (_defaultCompressionAlgorithmId > 0 &&
            messageData.Length > _compressThreshold &&
            _compressionRegistry.TryGet(_defaultCompressionAlgorithmId, out var compressionAlgorithm))
        {
            var compressedData = compressionAlgorithm.Compress(messageData);
            if (compressedData.Length < messageData.Length)
            {
                messageData = compressedData;
                algorithmId = _defaultCompressionAlgorithmId;
            }
        }

        var totalLength = messageData.Length + InnerPackageHeaderLength;
        var buffer = ArrayPool<byte>.Shared.Rent(totalLength);
        BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan(0, 4), totalLength);
        buffer[4] = message.OperationType;
        buffer[5] = algorithmId;
        BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan(6, 4), message.UniqueId);
        BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan(10, 4), message.MessageId);
        Buffer.BlockCopy(messageData, 0, buffer, InnerPackageHeaderLength, messageData.Length);
        return new PooledBuffer(buffer, totalLength);
    }

    /// <summary>
    /// 从网络流中读取并解码一条消息。
    /// </summary>
    /// <remarks>
    /// Reads and decodes a message from the network stream.
    /// </remarks>
    /// <param name="stream">网络流 / The network stream to read from</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>解码后的消息对象；连接关闭时返回 null / The decoded message object, or null if the connection was closed</returns>
    public async Task<MessageObject> DecodeAsync(Stream stream, CancellationToken cancellationToken)
    {
        var lengthBuffer = ArrayPool<byte>.Shared.Rent(4);
        try
        {
            await ReadExactAsync(stream, lengthBuffer.AsMemory(0, 4), cancellationToken);
            var totalLength = BinaryPrimitives.ReadInt32BigEndian(lengthBuffer.AsSpan(0, 4));
            if (totalLength < InnerPackageHeaderLength)
            {
                return null;
            }

            var bodyLength = totalLength - 4;
            var bodyBuffer = ArrayPool<byte>.Shared.Rent(bodyLength);
            try
            {
                await ReadExactAsync(stream, bodyBuffer.AsMemory(0, bodyLength), cancellationToken);
                var operationType = bodyBuffer[0];
                var algorithmId = bodyBuffer[1];
                var uniqueId = BinaryPrimitives.ReadInt32BigEndian(bodyBuffer.AsSpan(2, 4));
                var messageId = BinaryPrimitives.ReadInt32BigEndian(bodyBuffer.AsSpan(6, 4));
                var payloadLength = totalLength - InnerPackageHeaderLength;
                var messageData = new byte[payloadLength];
                Buffer.BlockCopy(bodyBuffer, 10, messageData, 0, payloadLength);
                if (algorithmId > 0)
                {
                    if (!_compressionRegistry.TryGet(algorithmId, out var compressionAlgorithm))
                    {
                        throw new NotSupportedException($"Compression algorithm '{algorithmId}' is not registered.");
                    }

                    messageData = compressionAlgorithm.Decompress(messageData);
                }

                var messageType = MessageProtoHelper.GetMessageTypeById(messageId);
                if (messageType == null)
                {
                    return null;
                }

                var message = (MessageObject)ProtoBufSerializerHelper.Deserialize(messageData, messageType);
                message.SetOperationType(operationType);
                message.SetUniqueId(uniqueId);
                message.SetMessageId(messageId);
                return message;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bodyBuffer);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(lengthBuffer);
        }
    }

    /// <summary>
    /// 精确读取指定长度的数据。
    /// </summary>
    /// <remarks>
    /// Reads exactly the specified number of bytes from the stream.
    /// </remarks>
    private static async Task ReadExactAsync(Stream stream, Memory<byte> buffer, CancellationToken cancellationToken)
    {
        var offset = 0;
        while (offset < buffer.Length)
        {
            var readLength = await stream.ReadAsync(buffer[offset..], cancellationToken);
            if (readLength == 0)
            {
                throw new IOException("Remote connection closed.");
            }

            offset += readLength;
        }
    }
}
