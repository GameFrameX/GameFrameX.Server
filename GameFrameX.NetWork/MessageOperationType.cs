namespace GameFrameX.NetWork;

/// <summary>
/// 消息操作业务类型
/// </summary>
public enum MessageOperationType : byte
{
    /// <summary>
    /// 空类型
    /// </summary>
    None,

    /// <summary>
    /// 心跳
    /// </summary>
    HeartBeat,

    /// <summary>
    /// 缓存查询
    /// </summary>
    Cache,

    /// <summary>
    /// 数据库查询
    /// </summary>
    Database,

    /// <summary>
    /// 游戏
    /// </summary>
    Game,
}