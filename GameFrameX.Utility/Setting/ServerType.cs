namespace GameFrameX.Utility.Setting;

/// <summary>
/// 服务器类型
/// </summary>
[Flags]
public enum ServerType
{
    /// <summary>
    /// 空值
    /// </summary>
    None = -1,

    /// <summary>
    /// 客户端
    /// </summary>
    Client = 0,

    #region 基础服务

    /// <summary>
    /// 日志服
    /// </summary>
    Log = 1 << 1,

    /// <summary>
    /// 数据库
    /// </summary>
    DataBase = Log << 1,

    /// <summary>
    /// 缓存服
    /// </summary>
    Cache = DataBase << 1,

    /// <summary>
    /// 网关服
    /// </summary>
    Gateway = Cache << 1,

    /// <summary>
    /// 账号服
    /// </summary>
    Account = Gateway << 1,

    /// <summary>
    /// 路由
    /// </summary>
    Router = Account << 1,

    /// <summary>
    /// 服务发现中心服，用于发现其他服务器。
    /// </summary>
    DiscoveryCenter = Router << 1,

    /// <summary>
    /// 远程备份
    /// </summary>
    Backup = DiscoveryCenter << 1,

    #endregion

    /// <summary>
    /// 登录服务器
    /// </summary>
    Login = Backup << 1,

    /// <summary>
    /// 游戏服
    /// </summary>
    Game = Login << 1,

    /// <summary>
    /// 匹配服
    /// </summary>
    Match = Game << 1,

    /// <summary>
    /// 充值服
    /// </summary>
    Recharge = Match << 1,

    /// <summary>
    /// 逻辑服
    /// </summary>
    Logic = Recharge << 1,

    /// <summary>
    /// 聊天服
    /// </summary>
    Chat = Logic << 1,

    /// <summary>
    /// 邮件服
    /// </summary>
    Mail = Chat << 1,

    /// <summary>
    /// 公会服
    /// </summary>
    Guild = Mail << 1,

    /// <summary>
    /// 房间服
    /// </summary>
    Room = Guild << 1,

    /// <summary>
    /// 全部
    /// </summary>
    All = Room | Game | Logic | Recharge | Chat | Mail | Guild | Backup | Account | DiscoveryCenter | Gateway | Cache | DataBase | Log,
}