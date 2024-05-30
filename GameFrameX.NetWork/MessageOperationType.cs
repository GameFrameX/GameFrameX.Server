namespace GameFrameX.NetWork;

/// <summary>
/// 消息操作业务类型
/// </summary>
public enum MessageOperationType : byte
{
    /// <summary>
    /// 心跳
    /// </summary>
    HeartBeat = 1,

    /// <summary>
    /// 游戏
    /// </summary>
    Game = 2
}