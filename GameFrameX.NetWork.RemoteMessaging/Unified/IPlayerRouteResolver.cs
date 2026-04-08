// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   均受中华人民共和国及相关国际法律法规保护。
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   侵犯他人合法权益等法律法规所禁止的行为！
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   本项目组织与贡献者概不承担。
//   GitHub 仓库：https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//  ==========================================================================================

namespace GameFrameX.NetWork.RemoteMessaging.Unified;

/// <summary>
/// 玩家路由解析结果。
/// </summary>
/// <remarks>
/// Player route resolution result.
/// </remarks>
public sealed class PlayerRouteInfo
{
    /// <summary>
    /// 玩家所在的服务器类型（如 "Game"、"Social"）
    /// </summary>
    /// <remarks>
    /// The server type where the player is located (e.g. "Game", "Social").
    /// </remarks>
    public string ServerType { get; init; }

    /// <summary>
    /// 玩家所在的服务器ID
    /// </summary>
    /// <remarks>
    /// The server ID where the player is located.
    /// </remarks>
    public int ServerId { get; init; }

    /// <summary>
    /// 玩家是否在线
    /// </summary>
    /// <remarks>
    /// Whether the player is online.
    /// </remarks>
    public bool IsOnline { get; init; }

    /// <summary>
    /// 路由版本号（用于过期校验）
    /// </summary>
    /// <remarks>
    /// Route version number (used for expiration checking).
    /// </remarks>
    public long Version { get; init; }

    /// <summary>
    /// 创建在线路由信息
    /// </summary>
    /// <remarks>
    /// Creates online route information.
    /// </remarks>
    /// <param name="serverType">服务器类型 / Server type</param>
    /// <param name="serverId">服务器ID / Server ID</param>
    /// <param name="version">路由版本号 / Route version</param>
    /// <returns>在线路由信息 / Online route information</returns>
    public static PlayerRouteInfo Online(string serverType, int serverId, long version = 0)
    {
        return new PlayerRouteInfo
        {
            ServerType = serverType,
            ServerId = serverId,
            IsOnline = true,
            Version = version,
        };
    }

    /// <summary>
    /// 创建离线路由信息
    /// </summary>
    /// <remarks>
    /// Creates offline route information.
    /// </remarks>
    /// <returns>离线路由信息 / Offline route information</returns>
    public static PlayerRouteInfo Offline()
    {
        return new PlayerRouteInfo
        {
            IsOnline = false,
        };
    }
}

/// <summary>
/// 玩家路由解析器接口。负责查询玩家归属（ServerType/ServerId/在线状态）。
/// </summary>
/// <remarks>
/// Player route resolver interface. Responsible for querying player location (ServerType/ServerId/online status).
/// </remarks>
public interface IPlayerRouteResolver
{
    /// <summary>
    /// 解析玩家路由信息。
    /// </summary>
    /// <remarks>
    /// Resolves player route information.
    /// </remarks>
    /// <param name="playerId">玩家ID / Player ID</param>
    /// <returns>路由信息，null 表示路由缺失 / Route information, null indicates route missing</returns>
    Task<PlayerRouteInfo> ResolveAsync(long playerId);
}
