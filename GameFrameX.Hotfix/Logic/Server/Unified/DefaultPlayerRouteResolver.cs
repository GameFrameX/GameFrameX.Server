// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 Apache License 2.0 单协议分发，
//   This project is licensed solely under the Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using GameFrameX.Core.Session;
using GameFrameX.Network.RemoteMessaging.Unified;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Hotfix.Logic.Server.Unified;

/// <summary>
/// 玩家路由解析器默认实现。
/// 当前基于本地 PlayerSessionManager 判断在线状态，结合服务配置判断归属。
/// 后续可对接 Redis / DB / PlayerCenter 服务。
/// </summary>
/// <remarks>
/// Default implementation of player route resolver.
/// Currently uses local PlayerSessionManager for online status and service configuration for routing.
/// Can be integrated with Redis / DB / PlayerCenter service in the future.
/// </remarks>
public sealed class DefaultPlayerRouteResolver : IPlayerRouteResolver
{
    /// <summary>
    /// 解析玩家路由信息。优先按本服 PlayerSessionManager 会话命中，服务类型与 ID 取自全局配置；未命中时回退到 PlayerSessionManager 内存路由表，按快照返回在线或离线；两者均未命中时默认返回离线。
    /// </summary>
    /// <remarks>
    /// Resolves player route information. First checks the local PlayerSessionManager session with server type and id taken from global settings; when missing, falls back to the PlayerSessionManager in-memory route table and returns online or offline based on the snapshot; returns offline by default when neither hits.
    /// </remarks>
    /// <param name="playerId">玩家ID / Player ID</param>
    /// <returns>玩家路由信息，本实现始终返回非 null，未命中时为离线信息 / Player route info; this implementation never returns null and yields offline info when missing</returns>
    public Task<PlayerRouteInfo> ResolveAsync(long playerId)
    {
        // 检查本服是否有该玩家的 session
        var session = PlayerSessionManager.Instance.GetByPlayerId(playerId);
        if (session != null)
        {
            var serverType = GlobalSettings.CurrentSetting?.ServerType ?? GameServerConst.Game.Name;
            var serverId = GlobalSettings.CurrentSetting?.ServerId ?? GameServerConst.Game.Id;

            return Task.FromResult(PlayerRouteInfo.Online(serverType, serverId, PlayerSessionManager.Instance.LocalDiscoveryInstanceId));
        }

        // 回退到 PlayerSessionManager 内存路由（仅内存，不依赖外部存储）
        if (PlayerSessionManager.Instance.TryGetPlayerRoute(playerId, out var snapshot))
        {
            if (snapshot.IsOnline)
            {
                return Task.FromResult(PlayerRouteInfo.Online(snapshot.ServerType, snapshot.ServerId, snapshot.InstanceId, snapshot.Version));
            }

            return Task.FromResult(PlayerRouteInfo.Offline());
        }

        // 当前未接入分布式路由源，默认离线
        return Task.FromResult(PlayerRouteInfo.Offline());
    }
}
