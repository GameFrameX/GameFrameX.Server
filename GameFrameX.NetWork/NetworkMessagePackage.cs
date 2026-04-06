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


using System.Text.Json.Serialization;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.Foundation.Localization.Core;

namespace GameFrameX.NetWork;

/// <summary>
/// 网络消息包。
/// </summary>
/// <remarks>
/// Represents a network message package, including message data, header, and related operations.
/// </remarks>
public sealed class NetworkMessagePackage : INetworkMessagePackage
{
    /// <summary>
    /// 获取包含在消息包中的消息类型。
    /// </summary>
    /// <remarks>
    /// The type of the message contained in the package.
    /// </remarks>
    [JsonIgnore]
    public Type MessageType { get; private set; }

    /// <summary>
    /// 获取消息数据，通常为序列化后的二进制内容。
    /// </summary>
    /// <remarks>
    /// The message data, usually serialized as a byte array.
    /// </remarks>
    public byte[] MessageData { get; private set; }

    /// <summary>
    /// 获取消息头对象，包含消息标识、长度、类型等元数据。
    /// </summary>
    /// <remarks>
    /// The message header object, containing metadata such as message ID, length, and type.
    /// </remarks>
    public INetworkMessageHeader Header { get; private set; }

    /// <summary>
    /// 将消息数据反序列化为消息对象。
    /// </summary>
    /// <remarks>
    /// Deserializes the message data into a message object.
    /// </remarks>
    /// <returns>反序列化后的消息对象 / The deserialized message object</returns>
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
    /// 设置消息数据，通常用于接收或构建消息包时赋值。
    /// </summary>
    /// <remarks>
    /// Sets the message data, typically used when receiving or constructing a message package.
    /// </remarks>
    /// <param name="messageData">要设置的消息数据字节数组 / The byte array of message data to set</param>
    public void SetMessageData(byte[] messageData)
    {
        MessageData = messageData;
    }

    /// <summary>
    /// 获取格式化后的消息字符串。
    /// </summary>
    /// <remarks>
    /// Gets a formatted string representation of the message for logging purposes.
    /// </remarks>
    /// <param name="actorId">ActorId，用于标识消息所属的角色或实体，默认值为0 / The actor ID, defaults to 0</param>
    /// <returns>格式化后的消息字符串，包含消息ID、操作类型、唯一ID、消息对象及ActorId / The formatted message string</returns>
    public string ToFormatMessageString(long actorId = default)
    {
        return MessageObjectLoggerHelper.FormatMessage(Header.MessageId, Header.OperationType, Header.UniqueId, DeserializeMessageObject(), actorId);
    }

    /// <summary>
    /// 清除消息内容。
    /// </summary>
    /// <remarks>
    /// Clears all message content and resets properties to default values.
    /// </remarks>
    public void Clear()
    {
        MessageType = default;
        MessageData = default;
        Header = default;
    }

    /// <summary>
    /// 设置消息头。
    /// </summary>
    /// <remarks>
    /// Sets the message header.
    /// </remarks>
    /// <param name="header">要设置的网络消息头对象 / The network message header to set</param>
    private void SetMessageHeader(INetworkMessageHeader header)
    {
        Header = header;
    }

    /// <summary>
    /// 设置消息类型。
    /// </summary>
    /// <remarks>
    /// Sets the message type.
    /// </remarks>
    /// <param name="messageType">要设置的消息类型 / The message type to set</param>
    private void SetMessageType(Type messageType)
    {
        MessageType = messageType;
    }

    /// <summary>
    /// 根据消息对象和消息头创建网络消息包。
    /// </summary>
    /// <remarks>
    /// Creates a network message package from the specified message and header.
    /// </remarks>
    /// <param name="message">要封装的网络消息对象 / The network message to encapsulate</param>
    /// <param name="messageObjectHeader">消息头对象 / The message header object</param>
    /// <returns>新创建并初始化完成的 NetworkMessagePackage 实例 / The created network message package</returns>
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
            LogHelper.Error("Create NetworkMessagePackage Error {messageId} {operationType} {uniqueId} {exception}", messageObjectHeader.MessageId, messageObjectHeader.OperationType, messageObjectHeader.UniqueId, e.Message);
            throw;
        }
    }

    /// <summary>
    /// 根据消息头、消息数据及消息类型创建网络消息包。
    /// </summary>
    /// <remarks>
    /// Creates a network message package from the specified header, data, and type.
    /// </remarks>
    /// <param name="messageObjectHeader">消息头对象 / The message header object</param>
    /// <param name="messageData">已序列化的消息数据字节数组 / The serialized message data</param>
    /// <param name="messageType">消息数据对应的类型 / The type of the message data</param>
    /// <returns>新创建并初始化完成的 NetworkMessagePackage 实例 / The created network message package</returns>
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