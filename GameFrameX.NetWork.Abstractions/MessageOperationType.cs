namespace GameFrameX.NetWork.Abstractions;

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

    /// <summary>
    /// 游戏管理
    /// </summary>
    GameManager,

    /// <summary>
    /// 禁止
    /// </summary>
    Forbid,

    /// <summary>
    /// 重启
    /// </summary>
    Reboot,

    /// <summary>
    /// 重连
    /// </summary>
    Reconnect,

    /// <summary>
    /// 重载
    /// </summary>
    Reload,

    /// <summary>
    /// 退出
    /// </summary>
    Exit,

    /// <summary>
    /// 踢人
    /// </summary>
    Kick,

    /// <summary>
    /// 通知
    /// </summary>
    Notify,

    /// <summary>
    /// 转发
    /// </summary>
    Forward,

    /// <summary>
    /// 注册
    /// </summary>
    Register,

    /// <summary>
    /// 请求链接服务器信息
    /// </summary>
    RequestConnectServer,
}