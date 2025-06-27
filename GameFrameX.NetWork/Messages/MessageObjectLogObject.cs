// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 消息对象日志
/// </summary>
public sealed class MessageObjectLogObject
{
    /// <summary>
    /// 消息日志对象
    /// </summary>
    /// <param name="name"></param>
    /// <param name="messageId"></param>
    /// <param name="operationType"></param>
    /// <param name="uniqueId"></param>
    /// <param name="messageObject"></param>
    public MessageObjectLogObject(string name, int messageId, MessageOperationType operationType, int uniqueId, INetworkMessage messageObject)
    {
        MessageType = name;
        MessageId = messageId;
        OpType = operationType;
        UniqueId = uniqueId;
        Data = messageObject;
    }

    /// <summary>
    /// 消息类型
    /// </summary>
    public string MessageType { get; set; }

    /// <summary>
    /// 消息ID
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    public MessageOperationType OpType { get; set; }

    /// <summary>
    /// 唯一ID
    /// </summary>
    public int UniqueId { get; set; }

    /// <summary>
    /// 消息对象
    /// </summary>
    public object Data { get; set; }
}