namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 消息ID工具类
/// </summary>
public static class MessageIdUtility
{
    /// <summary>
    /// 获取主消息ID
    /// </summary>
    /// <param name="messageId">消息码</param>
    /// <returns></returns>
    public static int GetMainId(int messageId)
    {
        return (messageId >> 16) & 0xFFFF;
    }

    /// <summary>
    /// 获取子消息ID
    /// </summary>
    /// <param name="messageId">消息码</param>
    /// <returns></returns>
    public static int GetSubId(int messageId)
    {
        return messageId & 0xFFFF;
    }

    /// <summary>
    /// 获取消息ID
    /// </summary>
    /// <param name="mainId">主消息码</param>
    /// <param name="subId">子消息码</param>
    /// <returns>返回组合之后的完整消息码</returns>
    public static int GetMessageId(int mainId, int subId)
    {
        return (mainId << 16) + subId;
    }
}