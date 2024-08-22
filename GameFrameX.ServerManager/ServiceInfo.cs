using GameFrameX.Setting;
using GameFrameX.Utility;
using Newtonsoft.Json;

namespace GameFrameX.ServerManager;

/// <summary>
/// 服务器信息
/// </summary>
public sealed class ServiceInfo : IServiceInfo
{
    /// <summary>
    /// 构造服务器信息
    /// </summary>
    /// <param name="type"></param>
    /// <param name="session"></param>
    /// <param name="sessionId"></param>
    /// <param name="serverName"></param>
    /// <param name="serverId"></param>
    /// <param name="innerIp"></param>
    /// <param name="innerPort"></param>
    /// <param name="outerIp"></param>
    /// <param name="outerPort"></param>
    public ServiceInfo(ServerType type, object session, string sessionId, string serverName, long serverId, string innerIp, ushort innerPort, string outerIp, ushort outerPort)
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
        StatusInfo = new ServiceStatusInfo();
    }

    /// <summary>
    /// 会话ID
    /// </summary>
    public string SessionId { get; }

    /// <summary>
    /// 服务器类型
    /// </summary>
    public ServerType Type { get; }

    /// <summary>
    /// 会话
    /// </summary>
    [JsonIgnore]
    public object Session { get; }

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
    public ServiceStatusInfo StatusInfo { get; set; }

    /// <summary>
    /// 转换为字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonHelper.SerializeFormat(this);
    }
}