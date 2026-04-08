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

using GameFrameX.Apps.Common.Session;
using GameFrameX.NetWork.RemoteMessaging.Unified;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Hotfix.Logic.Server.Unified;

/// <summary>
/// 玩家路由解析器默认实现。
/// 当前基于本地 SessionManager 判断在线状态，结合服务配置判断归属。
/// 后续可对接 Redis / DB / PlayerCenter 服务。
/// </summary>
/// <remarks>
/// Default implementation of player route resolver.
/// Currently uses local SessionManager for online status and service configuration for routing.
/// Can be integrated with Redis / DB / PlayerCenter service in the future.
/// </remarks>
public sealed class DefaultPlayerRouteResolver : IPlayerRouteResolver
{
    /// <inheritdoc />
    public Task<PlayerRouteInfo> ResolveAsync(long playerId)
    {
        // 检查本服是否有该玩家的 session
        var session = SessionManager.GetByRoleId(playerId);
        if (session != null)
        {
            var serverType = GlobalSettings.CurrentSetting?.ServerType ?? GameServerConst.Game.Name;
            var serverId = GlobalSettings.CurrentSetting?.ServerId ?? GameServerConst.Game.Id;

            return Task.FromResult(PlayerRouteInfo.Online(serverType, serverId));
        }

        // 回退到 SessionManager 内存路由（仅内存，不依赖外部存储）
        if (SessionManager.TryGetPlayerRoute(playerId, out var snapshot))
        {
            if (snapshot.IsOnline)
            {
                return Task.FromResult(PlayerRouteInfo.Online(snapshot.ServerType, snapshot.ServerId, snapshot.Version));
            }

            return Task.FromResult(PlayerRouteInfo.Offline());
        }

        // 当前未接入分布式路由源，默认离线
        return Task.FromResult(PlayerRouteInfo.Offline());
    }
}
