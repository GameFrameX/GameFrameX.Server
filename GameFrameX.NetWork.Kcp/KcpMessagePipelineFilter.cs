using System.Buffers;
using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.NetWork.Kcp;

/// <summary>
/// KCP message pipeline filter / KCP 消息管道过滤器
/// </summary>
public sealed class KcpMessagePipelineFilter
{
    /// <summary>
    /// Parse messages from KCP session / 从 KCP 会话解析消息
    /// </summary>
    /// <param name="session">KCP session / KCP 会话</param>
    /// <returns>List of parsed messages / 解析的消息列表</returns>
    public List<IMessage> Filter(IKcpSession session)
    {
        var messages = new List<IMessage>();

        while (session.IsConnected)
        {
            var peekSize = session.PeekSize();
            if (peekSize <= 0)
            {
                break;
            }

            var buffer = ArrayPool<byte>.Shared.Rent(peekSize);
            try
            {
                var bytesRead = session.Recv(buffer.AsSpan(0, peekSize));
                if (bytesRead <= 0)
                {
                    break;
                }

                var message = ParseMessage(buffer.AsSpan(0, bytesRead));
                if (message != null)
                {
                    messages.Add(message);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        return messages;
    }

    /// <summary>
    /// Parse a single message from buffer / 从缓冲区解析单条消息
    /// </summary>
    /// <param name="buffer">Message buffer / 消息缓冲区</param>
    /// <returns>Parsed message or null / 解析的消息或 null</returns>
    public IMessage ParseMessage(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < sizeof(int))
        {
            return null;
        }

        // Read total length (big-endian)
        var totalLength = ReadInt32BigEndian(buffer);
        if (buffer.Length < totalLength)
        {
            return null;
        }

        // Convert to Sequence for decoding
        var messageData = buffer.Slice(0, totalLength).ToArray();
        var sequence = new ReadOnlySequence<byte>(messageData);

        return MessageHelper.DecoderHandler.Handler(ref sequence);
    }

    /// <summary>
    /// Try parse message header / 尝试解析消息头
    /// </summary>
    /// <param name="buffer">Message buffer / 消息缓冲区</param>
    /// <param name="totalLength">Total message length / 消息总长度</param>
    /// <returns>True if header was parsed / 如果头部解析成功则返回 true</returns>
    public bool TryParseHeader(ReadOnlySpan<byte> buffer, out int totalLength)
    {
        totalLength = 0;

        if (buffer.Length < sizeof(int))
        {
            return false;
        }

        totalLength = ReadInt32BigEndian(buffer);
        return totalLength > 0;
    }

    /// <summary>
    /// Read int32 in big-endian format / 以大端序读取 int32
    /// </summary>
    private static int ReadInt32BigEndian(ReadOnlySpan<byte> buffer)
    {
        var value = BitConverter.ToInt32(buffer.Slice(0, 4));
        if (BitConverter.IsLittleEndian)
        {
            // Convert from little-endian to big-endian
            value = (int)(((uint)value >> 24) | (((uint)value >> 8) & 0xFF00) | (((uint)value << 8) & 0xFF0000) | ((uint)value << 24));
        }

        return value;
    }
}