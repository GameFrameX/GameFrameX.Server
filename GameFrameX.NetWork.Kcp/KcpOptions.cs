namespace GameFrameX.NetWork.Kcp;

/// <summary>
/// KCP configuration options / KCP 配置选项
/// </summary>
public sealed class KcpOptions
{
    /// <summary>
    /// Enable KCP server / 是否启用 KCP 服务器
    /// </summary>
    public bool Enable { get; set; } = false;

    /// <summary>
    /// Enable no-delay mode / 是否启用无延迟模式
    /// </summary>
    public bool NoDelay { get; set; } = true;

    /// <summary>
    /// Internal update interval in milliseconds / 内部更新间隔（毫秒）
    /// </summary>
    public int Interval { get; set; } = 10;

    /// <summary>
    /// Fast resend count (0=disable, 2=recommended) / 快速重传次数（0=禁用，2=推荐值）
    /// </summary>
    public int Resend { get; set; } = 2;

    /// <summary>
    /// Enable flow control / 是否启用流控
    /// </summary>
    public bool EnableFlowControl { get; set; } = false;

    /// <summary>
    /// Send window size / 发送窗口大小
    /// </summary>
    public int SendWindow { get; set; } = 128;

    /// <summary>
    /// Receive window size / 接收窗口大小
    /// </summary>
    public int ReceiveWindow { get; set; } = 128;

    /// <summary>
    /// Maximum transmission unit / 最大传输单元
    /// </summary>
    public int Mtu { get; set; } = 1400;

    /// <summary>
    /// Connection timeout in milliseconds / 连接超时时间（毫秒）
    /// </summary>
    public int ConnectionTimeout { get; set; } = 60000;

    /// <summary>
    /// Keep alive interval in milliseconds / 心跳间隔（毫秒）
    /// </summary>
    public int KeepAliveInterval { get; set; } = 10000;

    /// <summary>
    /// Update period in milliseconds / 更新周期（毫秒）
    /// </summary>
    public int UpdatePeriod { get; set; } = 5;

    /// <summary>
    /// Session timeout in seconds / 会话超时时间（秒）
    /// </summary>
    public int SessionTimeout { get; set; } = 120;
}