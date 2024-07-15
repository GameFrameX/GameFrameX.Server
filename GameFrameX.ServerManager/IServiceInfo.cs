using GameFrameX.Setting;

namespace GameFrameX.ServerManager;

/// <summary>
/// 服务信息
/// </summary>
public interface IServiceInfo
{
    /// <summary>
    /// 会话ID
    /// </summary>
    string SessionId { get; }

    /// <summary>
    /// 服务器类型
    /// </summary>
    ServerType Type { get; }

    /// <summary>
    /// 服务器名称
    /// </summary>
    string ServerName { get; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    long ServerId { get; }
}