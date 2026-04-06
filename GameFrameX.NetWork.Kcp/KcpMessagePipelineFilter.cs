// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


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