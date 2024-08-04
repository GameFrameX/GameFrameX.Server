namespace GameFrameX.Setting;

/// <summary>
/// 需要小于1000，因为1000以上作为服务器id了
/// </summary>
public enum IdModule
{
    /// <summary>
    /// 最小值
    /// </summary>
    Min = 0,

    /// <summary>
    /// 账号
    /// </summary>
    Account,

    /// <summary>
    /// 角色
    /// </summary>
    Player,

    /// <summary>
    /// 单服/玩家不同即可
    /// </summary>
    Pet = 101,

    /// <summary>
    /// 
    /// </summary>
    Equip = 102,

    /// <summary>
    /// 
    /// </summary>
    WorkerActor = 103,

    /// <summary>
    /// 最大值
    /// </summary>
    Max = 999
}