using GameFrameX.Setting;
using GameFrameX.Utility;
using Newtonsoft.Json;

namespace GameFrameX.ServerManager;

/// <summary>
/// 服务器信息
/// </summary>
public sealed class ServerInfo
{
    public ServerInfo(ServerType type, object session, string sessionId, string serverName, long serverId, string innerIp, ushort innerPort, string outerIp, ushort outerPort)
    {
        Type = type;
        Session = session;
        ServerName = serverName;
        ServerId = serverId;
        InnerIp = innerIp;
        InnerPort = innerPort;
        OuterIp = outerIp;
        OuterPort = outerPort;
        SessionId = sessionId;
        StatusInfo = new ServerStatusInfo();
    }

    public string SessionId { get; }

    /// <summary>
    /// 服务器类型
    /// </summary>
    public ServerType Type { get; }

    [JsonIgnore] public object Session { get; }

    /// <summary>
    /// 服务器名称
    /// </summary>
    public string ServerName { get; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    public long ServerId { get; }

    /// <summary>
    /// 内部IP
    /// </summary>
    public string InnerIp { get; }

    /// <summary>
    /// 内部端口
    /// </summary>
    public ushort InnerPort { get; }

    /// <summary>
    /// 外部IP
    /// </summary>
    public string OuterIp { get; }

    /// <summary>
    /// 外部端口
    /// </summary>
    public ushort OuterPort { get; }

    /// <summary>
    /// 服务器状态
    /// </summary>
    public ServerStatusInfo StatusInfo { get; set; }

    public override string ToString()
    {
        return JsonHelper.Serialize(this);
    }
}