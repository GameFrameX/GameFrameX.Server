namespace GameFrameX.Core.Abstractions;

/// <summary>
/// 每个服存在多个实例的（如玩家和公会）需要小于Separator
/// 最大id应当小于999
/// Id一旦定义了不应该修改
/// </summary>
public enum ActorType : ushort
{
    /// <summary>
    /// 空将会被判断为无效值
    /// </summary>
    None,

    /// <summary>
    /// 角色
    /// 管理玩家角色 
    /// </summary>
    Player,

    /// <summary>
    /// 分割线(勿调整,勿用于业务逻辑)
    /// </summary>
    Separator = 128,

    /// <summary>
    /// 固定ID类型Actor
    /// </summary>
    Server = 129,

    /// <summary>
    /// 账号
    /// 管理玩家账号信息，如注册、密码找回等。
    /// </summary>
    Account,

    /// <summary>
    /// 最大值
    /// </summary>
    Max = 999,
}

/// <summary>
/// 供ActorLimit检测调用关系
/// </summary>
public enum ActorTypeLevel
{
    /// <summary>
    /// 角色
    /// </summary>
    Role = 1,

    /// <summary>
    /// 公会
    /// </summary>
    Guild,

    /// <summary>
    /// 系统服务
    /// </summary>
    Server,
}