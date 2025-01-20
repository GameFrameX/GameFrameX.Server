using GameFrameX.DataBase.Mongo;

namespace GameFrameX.Apps.Account.Login.Entity;

public class LoginState : CacheState
{
    /// <summary>
    /// 昵称
    /// </summary>
    public string NickName { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 用户状态
    /// </summary>
    public int State { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    public uint Level { get; set; }

    /// <summary>
    /// //是否是重连
    /// </summary>
    public bool isReconnect { get; set; }

    public int serverId { get; set; }

    public int uniId { get; set; }

    public int sdkType { get; set; }

    public string sdkChannel { get; set; }
    
    /// <summary>
    /// 玩家在不同平台上的登录信息
    /// </summary>
    public List<PlatformLoginInfo> PlatformList { get; set; } = [];
}
public class PlatformLoginInfo
{
    /// <summary>
    /// 平台名称（例如 "Facebook", "Google", "QQ" 等）
    /// </summary>
    public string Platform { get; set; }

    /// <summary>
    /// 平台账号
    /// </summary>
    public string PlatformAccount { get; set; }

    /// <summary>
    /// 平台密码
    /// </summary>
    public string PlatformPassword { get; set; }

    /// <summary>
    /// 平台账号的唯一标识（例如 Facebook 的用户 ID，QQ 的QQ号等）
    /// </summary>
    public string PlatformOAuth { get; set; }

    /// <summary>
    /// 登录方式（如 密码登录，OAuth 等）
    /// </summary>
    public string PlatformLoginMethod { get; set; }

    /// <summary>
    /// 平台用户的昵称（可选，某些平台可能有）
    /// </summary>
    public string PlatformNickName { get; set; }
}