using GameFrameX.Setting;
using GameFrameX.Utility;
using Newtonsoft.Json;

namespace GameFrameX.ServerManager;

/// <summary>
/// 游戏服务器信息
/// </summary>
public sealed class GameServiceInfo : IServiceInfo
{
    public GameServiceInfo(ServerType type, object session, string sessionId, string serverName, long serverId, ushort minModuleMessageId, ushort maxModuleMessageId)
    {
        Type = type;
        Session = session;
        ServerName = serverName;
        ServerId = serverId;
        MinModuleMessageId = minModuleMessageId;
        MaxModuleMessageId = maxModuleMessageId;
        SessionId = sessionId;
        StatusInfo = new ServiceStatusInfo();
    }

    public ushort MaxModuleMessageId { get; set; }

    public ushort MinModuleMessageId { get; }
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
    /// 服务器状态
    /// </summary>
    public ServiceStatusInfo StatusInfo { get; set; }

    public override string ToString()
    {
        return JsonHelper.Serialize(this);
    }
}