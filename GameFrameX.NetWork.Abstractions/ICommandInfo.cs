namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 消息指令信息
/// </summary>
public interface ICommandInfo
{
    /// <summary>
    /// 合并后的消息ID
    /// </summary>
    int MessageId { get; }

    /// <summary>
    /// 主消息ID
    /// </summary>
    int MainId { get; }

    /// <summary>
    /// 次消息ID
    /// </summary>
    int SubId { get; }
}