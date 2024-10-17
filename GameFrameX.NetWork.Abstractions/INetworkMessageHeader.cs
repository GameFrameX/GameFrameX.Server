namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 网络消息头
/// </summary>
public interface INetworkMessageHeader
{
    /// <summary>
    /// 消息ID
    /// </summary>
    int MessageId { get; set; }

    /// <summary>
    /// 唯一消息序列ID
    /// </summary>
    int UniqueId { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    byte OperationType { get; set; }

    /// <summary>
    /// 压缩标记
    /// </summary>
    byte ZipFlag { get; set; }
}