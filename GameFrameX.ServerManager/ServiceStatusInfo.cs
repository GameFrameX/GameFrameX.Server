namespace GameFrameX.ServerManager;

/// <summary>
/// 服务器状态信息
/// 默认状态为在线
/// 默认承载为5000
/// </summary>
public sealed class ServiceStatusInfo
{
    /// <summary>
    /// 服务器状态
    /// </summary>
    public ServiceStatus Status { get; set; }

    /// <summary>
    /// 承载上限
    /// </summary>
    public int MaxLoad { get; set; }

    /// <summary>
    /// 当前承载
    /// </summary>
    public int CurrentLoad { get; set; } = 0;

    public ServiceStatusInfo()
    {
        Status = ServiceStatus.Online;
        MaxLoad = 5000;
        CurrentLoad = 0;
    }

    public override string ToString()
    {
        return $"MaxLoad:{MaxLoad},CurrentLoad:{CurrentLoad},Status:{Status}";
    }
}