namespace GameFrameX.Apps.Common.Session;

public sealed class Session
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="sessionId">连接会话ID</param>
    /// <param name="netWorkChannel">网络渠道对象</param>
    public Session(string sessionId, INetWorkChannel netWorkChannel)
    {
        WorkChannel = netWorkChannel;
        SessionId = sessionId;
        CreateTime = TimeHelper.UnixTimeSeconds();
    }

    /// <summary>
    /// 全局会话ID
    /// </summary>
    public string SessionId { get; }

    /// <summary>
    /// 玩家ID
    /// </summary>
    public long PlayerId { get; private set; }

    /// <summary>
    /// 连接时间
    /// </summary>
    public long CreateTime { get; }

    /// <summary>
    /// 连接上下文
    /// </summary>
    [JsonIgnore]
    public INetWorkChannel WorkChannel { get; }

    /// <summary>
    /// 连接标示，避免自己顶自己的号,客户端每次启动游戏生成一次/或者每个设备一个
    /// </summary>
    public string Sign { get; private set; }

    /// <summary>
    /// 设置玩家ID
    /// </summary>
    /// <param name="playerId">玩家ID</param>
    public void SetPlayerId(long playerId)
    {
        PlayerId = playerId;
    }

    /// <summary>
    /// 设置签名
    /// </summary>
    /// <param name="sign">签名</param>
    public void SetSign(string sign)
    {
        Sign = sign;
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="messageObject">消息对象</param>
    /// <param name="errorCode">消息错误码</param>
    public async Task WriteAsync(MessageObject messageObject, int errorCode = 0)
    {
        if (WorkChannel != null)
        {
            await WorkChannel.WriteAsync(messageObject, errorCode);
        }
    }
}