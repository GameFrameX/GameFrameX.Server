namespace GameFrameX.Launcher.Transfer;

/// <summary>
/// 转发器的标记
/// </summary>
public class TransferAttribute : Attribute
{
    /// <summary>
    /// 源服务器
    /// </summary>
    public ServerType Source { get; }

    /// <summary>
    /// 目标服务器
    /// </summary>
    public ServerType Destination { get; }

    public TransferAttribute(ServerType sourceType, ServerType destinationType)
    {
        Source = sourceType;
        Destination = destinationType;
    }
}