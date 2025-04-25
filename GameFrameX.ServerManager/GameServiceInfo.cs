using System.Text.Json.Serialization;
using GameFrameX.Foundation.Json;
using GameFrameX.Utility.Setting;

namespace GameFrameX.ServerManager;

/// <summary>
/// 游戏服务器信息
/// </summary>
public sealed class GameServiceInfo : IServiceInfo
{
    /// <summary>
    /// 构造游戏服务器信息
    /// </summary>
    /// <param name="type"></param>
    /// <param name="session"></param>
    /// <param name="sessionId"></param>
    /// <param name="serverName"></param>
    /// <param name="serverId"></param>
    /// <param name="minModuleMessageId"></param>
    /// <param name="maxModuleMessageId"></param>
    public GameServiceInfo(ServerType type, object session, string sessionId, string serverName, long serverId, short minModuleMessageId, short maxModuleMessageId)
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

    /// <summary>
    /// 最大模块消息ID
    /// </summary>
    public short MaxModuleMessageId { get; set; }

    /// <summary>
    /// 最小模块消息ID
    /// </summary>
    public short MinModuleMessageId { get; }

    /// <summary>
    /// 会话
    /// </summary>
    [JsonIgnore]
    public object Session { get; }

    /// <summary>
    /// 服务器状态
    /// </summary>
    public ServiceStatusInfo StatusInfo { get; set; }

    /// <summary>
    /// 会话ID
    /// </summary>
    public string SessionId { get; }

    /// <summary>
    /// 服务器类型
    /// </summary>
    public ServerType Type { get; }

    /// <summary>
    /// 服务器名称
    /// </summary>
    public string ServerName { get; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    public long ServerId { get; }

    /// <summary>
    /// 转换为字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonHelper.SerializeFormat(this);
    }
}