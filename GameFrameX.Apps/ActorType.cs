using GameFrameX.Setting;

namespace GameFrameX.Apps;

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
    /// 分割线(勿调整,勿用于业务逻辑)
    /// </summary>
    Separator = GlobalConst.ActorTypeSeparator,

    /// <summary>
    /// 账号
    /// 管理玩家账号信息，如注册、密码找回等。
    /// </summary>
    Account = 130,

    /// <summary>
    /// 最大值
    /// </summary>
    Max = GlobalConst.ActorTypeMax,
}