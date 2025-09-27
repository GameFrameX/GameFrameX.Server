// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using System.Buffers;
using GameFrameX.Foundation.Extensions;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
// using GameFrameX.SuperSocket.ProtoBase;
using GameFrameX.Foundation.Logger;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 对外部客户端的消息解码处理器
/// </summary>
[Obsolete("建议使用 DefaultMessageDecoderHandler 替代")]
public sealed class ClientMessageDecoderHandler : DefaultMessageDecoderHandler
{
    /// <summary>
    /// 消息头长度
    /// </summary>
    public override ushort PackageHeaderLength { get; } = sizeof(uint) + sizeof(byte) + sizeof(byte) + sizeof(int) + sizeof(int);

    /// <summary>
    /// 消息解码
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    public override IMessage Handler(ref ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        try
        {
            // 消息总长度
            reader.TryReadBigEndianValue(out uint totalLength);
            // 消息对象头
            var messageObjectHeader = new MessageObjectHeader();
            reader.TryReadBigEndianValue(out byte operationType);
            reader.TryReadBigEndianValue(out byte zipFlag);
            reader.TryReadBigEndianValue(out int uniqueId);
            reader.TryReadBigEndianValue(out int messageId);
            messageObjectHeader.OperationType = (MessageOperationType)operationType;
            messageObjectHeader.ZipFlag = zipFlag;
            messageObjectHeader.UniqueId = uniqueId;
            messageObjectHeader.MessageId = messageId;
            // 消息内容
            reader.TryReadBytesValue((int)(totalLength - PackageHeaderLength), out var messageData);
            if (messageObjectHeader.ZipFlag > 0)
            {
                ArgumentNullException.ThrowIfNull(DecompressHandler, nameof(DecompressHandler));
                messageData = DecompressHandler.Handler(messageData);
            }

            var messageType = MessageProtoHelper.GetMessageTypeById(messageObjectHeader.MessageId);

            if (messageObjectHeader.MessageId >= 0)
            {
                // 外部消息
                return OuterNetworkMessage.Create(messageObjectHeader, messageData, messageType);
            }

            throw new Exception("不支持的消息类型,消息ID:" + messageObjectHeader.MessageId);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            return null;
        }
    }
}