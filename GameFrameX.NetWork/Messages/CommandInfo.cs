using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 指令信息
/// </summary>
public sealed class CommandInfo : ICommandInfo
{
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
    public CommandInfo(int messageId)
    {
        MessageId = messageId;
        MainId = MessageManager.GetMainId(MessageId);
        SubId = MessageManager.GetSubId(MessageId);
    }

    /// <summary>
    /// 创建指令对象
    /// </summary>
    /// <param name="mainId"></param>
    /// <param name="subId"></param>
    public CommandInfo(int mainId, int subId)
    {
        MessageId = MessageManager.GetMessageId(mainId, subId);
        MainId = mainId;
        SubId = subId;
    }

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