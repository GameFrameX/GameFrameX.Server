namespace GameFrameX.ServerManager;

/// <summary>
/// 服务器状态
/// </summary>
public enum ServiceStatus
{
    /// <summary>
    /// 未知
    /// </summary>
    Unknown,

    /// <summary>
    /// 在线
    /// </summary>
    Online,

    /// <summary>
    /// 重连中
    /// </summary>
    Reconnecting,

    /// <summary>
    /// 维护
    /// </summary>
    Maintenance,

    /// <summary>
    /// 待命
    /// </summary>
    Standby,

    /// <summary>
    /// 重启
    /// </summary>
    Reboot,

    /// <summary>
    /// 备用
    /// </summary>
    Backup,

    /// <summary>
    /// 离线
    /// </summary>
    Offline,
}