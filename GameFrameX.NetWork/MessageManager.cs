namespace GameFrameX.NetWork;

/// <summary>
/// 消息ID管理器
/// </summary>
public static class MessageManager
{
    /// <summary>
    /// 获取主消息ID
    /// </summary>
    /// <param name="messageId"></param>
    /// <returns></returns>
    public static int GetMainId(int messageId)
    {
        return (messageId >> 16) & 0xFFFF;
    }

    /// <summary>
    /// 获取子消息ID
    /// </summary>
    /// <param name="messageId"></param>
    /// <returns></returns>
    public static int GetSubId(int messageId)
    {
        return messageId & 0xFFFF;
    }

    /// <summary>
    /// 获取消息ID
    /// </summary>
    /// <param name="mainId"></param>
    /// <param name="subId"></param>
    /// <returns></returns>
    public static int GetMessageId(int mainId, int subId)
    {
        return (mainId << 16) + subId;
    }
}