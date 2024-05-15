namespace GameFrameX.Apps.Player.Player.Entity;

public class PlayerState : CacheState
{
    public long RoleId => Id;

    /// <summary>
    /// 账号ID
    /// </summary>
    public long AccountId { get; set; }

    /// <summary>
    /// 玩家名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 玩家等级
    /// </summary>
    public uint Level { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    public uint Avatar { get; set; }

    /// <summary>
    /// 玩家状态
    /// </summary>
    public int State { get; set; }

    /// <summary>
    /// 登录时间
    /// </summary>
    public DateTime LoginTime { get; set; }

    /// <summary>
    /// 离线时间
    /// </summary>
    public DateTime OfflineTime { get; set; }
}