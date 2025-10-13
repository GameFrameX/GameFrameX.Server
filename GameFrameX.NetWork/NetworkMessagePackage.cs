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


using System.Text.Json.Serialization;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;

namespace GameFrameX.NetWork;

/// <summary>
/// Defines the interface for a network message package, including message data, header, and related operations.
/// </summary>
/// <remarks>
/// 表示网络消息包的接口，包含消息数据、消息头及相关操作方法。
/// </remarks>
public sealed class NetworkMessagePackage : INetworkMessagePackage
{
    /// <summary>
    /// The type of the message contained in the package.
    /// </summary>
    /// <remarks>
    /// 包含在消息包中的消息类型。
    /// </remarks>
    [JsonIgnore]
    public Type MessageType { get; private set; }

    /// <summary>
    /// The message data, usually serialized as a byte array.
    /// </summary>
    /// <remarks>
    /// 消息数据，通常为序列化后的二进制内容。
    /// </remarks>
    public byte[] MessageData { get; private set; }

    /// <summary>
    /// The message header object, containing metadata such as message ID, length, and type.
    /// </summary>
    /// <remarks>
    /// 消息头对象，包含消息标识、长度、类型等元数据。
    /// </remarks>
    public INetworkMessageHeader Header { get; private set; }

    /// <summary>
    /// Deserializes the message data into a message object.
    /// </summary>
    /// <returns>The deserialized message object. / 反序列化后的消息对象。</returns>
    /// <remarks>
    /// 将消息数据反序列化为消息对象。
    /// </remarks>
    public INetworkMessage DeserializeMessageObject()
    {
        var message = (INetworkMessage)ProtoBufSerializerHelper.Deserialize(MessageData, MessageType);
        message.SetUniqueId(Header.UniqueId);
        MessageProtoHelper.SetMessageId(message);
        if (message is MessageObject messageObject)
        {
            messageObject.SetOperationType(Header.OperationType);
        }

        return message;
    }

    /// <summary>
    /// Sets the message data, typically used when receiving or constructing a message package.
    /// </summary>
    /// <remarks>
    /// 设置消息数据，通常用于接收或构建消息包时赋值。
    /// </remarks>
    /// <param name="messageData">The byte array of message data to set./ 要设置的消息数据字节数组。</param>
    public void SetMessageData(byte[] messageData)
    {
        MessageData = messageData;
    }

    /// <summary>
    /// 获取格式化后的消息字符串
    /// </summary>
    /// <param name="actorId">ActorId，用于标识消息所属的角色或实体，默认值为0</param>
    /// <returns>格式化后的消息字符串，包含消息ID、操作类型、唯一ID、消息对象及ActorId</returns>
    public string ToFormatMessageString(long actorId = default)
    {
        return MessageObjectLoggerHelper.FormatMessage(Header.MessageId, Header.OperationType, Header.UniqueId, DeserializeMessageObject(), actorId);
    }

    /// <summary>
    /// 清除消息内容
    /// </summary>
    public void Clear()
    {
        MessageType = default;
        MessageData = default;
        Header = default;
    }

    /// <summary>
    /// 设置消息头
    /// </summary>
    /// <param name="header">要设置的网络消息头对象，不能为空</param>
    private void SetMessageHeader(INetworkMessageHeader header)
    {
        Header = header;
    }

    /// <summary>
    /// 设置消息类型
    /// </summary>
    /// <param name="messageType">要设置的消息类型，不能为空</param>
    private void SetMessageType(Type messageType)
    {
        MessageType = messageType;
    }

    /// <summary>
    /// 根据消息对象和消息头创建网络消息包
    /// </summary>
    /// <param name="message">要封装的网络消息对象，不能为空</param>
    /// <param name="messageObjectHeader">消息头对象，不能为空</param>
    /// <returns>新创建并初始化完成的 NetworkMessagePackage 实例</returns>
    public static NetworkMessagePackage Create(INetworkMessage message, INetworkMessageHeader messageObjectHeader)
    {
        ArgumentNullException.ThrowIfNull(messageObjectHeader, nameof(messageObjectHeader));
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        var networkMessage = new NetworkMessagePackage();
        messageObjectHeader.OperationType = MessageProtoHelper.GetMessageOperationType(message);
        messageObjectHeader.MessageId = MessageProtoHelper.GetMessageIdByType(message);
        messageObjectHeader.UniqueId = message.UniqueId;
        networkMessage.SetMessageType(message.GetType());
        try
        {
            var buffer = ProtoBufSerializerHelper.Serialize(message);
            networkMessage.SetMessageData(buffer);
            networkMessage.SetMessageHeader(messageObjectHeader);
            return networkMessage;
        }
        catch (Exception e)
        {
            LogHelper.Error("If the message object is encoded abnormally, check the error log, exception:" + e);
            throw;
        }
    }

    /// <summary>
    /// 根据消息头、消息数据及消息类型创建网络消息包
    /// </summary>
    /// <param name="messageObjectHeader">消息头对象，不能为空</param>
    /// <param name="messageData">已序列化的消息数据字节数组，不能为空</param>
    /// <param name="messageType">消息数据对应的类型，不能为空</param>
    /// <returns>新创建并初始化完成的 NetworkMessagePackage 实例</returns>
    public static NetworkMessagePackage Create(INetworkMessageHeader messageObjectHeader, byte[] messageData, Type messageType)
    {
        ArgumentNullException.ThrowIfNull(messageObjectHeader, nameof(messageObjectHeader));
        ArgumentNullException.ThrowIfNull(messageData, nameof(messageData));
        ArgumentNullException.ThrowIfNull(messageType, nameof(messageType));
        var networkMessagePackage = new NetworkMessagePackage();
        networkMessagePackage.SetMessageHeader(messageObjectHeader);
        networkMessagePackage.SetMessageData(messageData);
        networkMessagePackage.SetMessageType(messageType);
        return networkMessagePackage;
    }
}