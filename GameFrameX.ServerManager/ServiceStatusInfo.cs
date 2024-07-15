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

    /// <summary>
    /// 构造
    /// </summary>
    public ServiceStatusInfo()
    {
        Status      = ServiceStatus.Online;
        MaxLoad     = 5000;
        CurrentLoad = 0;
    }

    /// <summary>
    /// 转换为字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"MaxLoad:{MaxLoad},CurrentLoad:{CurrentLoad},Status:{Status}";
    }
}