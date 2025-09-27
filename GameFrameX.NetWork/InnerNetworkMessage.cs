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



using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;

namespace GameFrameX.NetWork;

/// <summary>
/// 内部消息
/// </summary>
public sealed class InnerNetworkMessage : IInnerNetworkMessage
{
    private readonly ConcurrentDictionary<string, object> _data = new ConcurrentDictionary<string, object>();

    /// <summary>
    /// 消息类型
    /// </summary>
    [JsonIgnore]
    public Type MessageType { get; private set; }

    /// <summary>
    /// 消息数据
    /// </summary>
    public byte[] MessageData { get; private set; }

    /// <summary>
    /// 消息头对象
    /// </summary>
    public INetworkMessageHeader Header { get; private set; }

    /// <summary>
    /// 转换消息数据为消息对象
    /// </summary>
    /// <returns></returns>
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
    /// 设置消息数据
    /// </summary>
    /// <param name="messageData"></param>
    public void SetMessageData(byte[] messageData)
    {
        MessageData = messageData;
    }

    /// <summary>
    /// 获取格式化后的消息字符串
    /// </summary>
    /// <param name="actorId">ActorId</param>
    /// <returns>格式化后的消息字符串</returns>
    public string ToFormatMessageString(long actorId = default)
    {
        return MessageObjectLoggerHelper.FormatMessage(Header.MessageId, Header.OperationType, Header.UniqueId, DeserializeMessageObject(), actorId);
    }

    /// <summary>
    /// 设置自定义数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetData(string key, object value)
    {
        _data[key] = value;
    }

    /// <summary>
    /// 获取自定义数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object GetData(string key)
    {
        _data.TryGetValue(key, out var value);
        return value;
    }

    /// <summary>
    /// 清除自定义数据
    /// </summary>
    public void ClearData()
    {
        _data.Clear();
    }

    /// <summary>
    /// 设置消息头
    /// </summary>
    /// <param name="header"></param>
    public void SetMessageHeader(INetworkMessageHeader header)
    {
        Header = header;
    }

    /// <summary>
    /// 设置消息类型
    /// </summary>
    /// <param name="messageType"></param>
    public void SetMessageType(Type messageType)
    {
        MessageType = messageType;
    }

    /// <summary>
    /// 创建内部消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="messageObjectHeader"></param>
    /// <returns></returns>
    public static IInnerNetworkMessage Create(INetworkMessage message, INetworkMessageHeader messageObjectHeader)
    {
        var networkMessage = new InnerNetworkMessage();
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
            LogHelper.Error("消息对象编码异常,请检查错误日志");
            LogHelper.Error(e);
            throw;
        }
    }

    /// <summary>
    /// 获取自定义数据
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> GetData()
    {
        return _data.ToDictionary();
    }

    /// <summary>
    /// 删除自定义数据
    /// </summary>
    /// <param name="key"></param>
    public bool RemoveData(string key)
    {
        return _data.Remove(key, out _);
    }

    /// <summary>
    /// 创建内部消息
    /// </summary>
    /// <param name="messageObjectHeader">消息头</param>
    /// <param name="messageData">消息体</param>
    /// <param name="messageType">消息体的类型</param>
    /// <returns></returns>
    public static InnerNetworkMessage Create(INetworkMessageHeader messageObjectHeader, byte[] messageData, Type messageType)
    {
        var innerMessage = new InnerNetworkMessage();
        innerMessage.SetMessageHeader(messageObjectHeader);
        innerMessage.SetMessageData(messageData);
        innerMessage.SetMessageType(messageType);
        return innerMessage;
    }
}