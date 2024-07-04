using GameFrameX.Setting;

namespace GameFrameX.ServerManager;

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