// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 指令信息
/// </summary>
public sealed class CommandInfo : ICommandInfo
{
    /// <summary>
    /// 创建指令对象
    /// </summary>
    /// <param name="messageId"></param>
    public CommandInfo(int messageId)
    {
        MessageId = messageId;
        MainId = MessageIdUtility.GetMainId(MessageId);
        SubId = MessageIdUtility.GetSubId(MessageId);
    }

    /// <summary>
    /// 创建指令对象
    /// </summary>
    /// <param name="mainId"></param>
    /// <param name="subId"></param>
    public CommandInfo(int mainId, int subId)
    {
        MessageId = MessageIdUtility.GetMessageId(mainId, subId);
        MainId = mainId;
        SubId = subId;
    }

    /// <summary>
    /// 合并后的消息ID
    /// </summary>
    public int MessageId { get; }

    /// <summary>
    /// 主消息ID
    /// </summary>
    public int MainId { get; }

    /// <summary>
    /// 次消息ID
    /// </summary>
    public int SubId { get; }

    /// <summary>
    /// 创建指令对象
    /// </summary>
    /// <param name="messageId"></param>
    /// <returns></returns>
    public static CommandInfo Create(int messageId)
    {
        return new CommandInfo(messageId);
    }

    /// <summary>
    /// 创建指令对象
    /// </summary>
    /// <param name="mainId"></param>
    /// <param name="subId"></param>
    /// <returns></returns>
    public static CommandInfo Create(int mainId, int subId)
    {
        return new CommandInfo(mainId, subId);
    }
}