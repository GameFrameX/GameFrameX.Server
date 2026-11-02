// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   均受中华人民共和国及相关国际法律法规保护。
//   使用本项目须严格遵守相应法律法规与开源许可证之规定。
//   本项目采用 Apache License 2.0 单协议分发，
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   侵犯他人合法权益等法律法规所禁止的行为！
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   本组织与贡献者概不承担。
//   GitHub 仓库：https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//  ==========================================================================================


using GameFrameX.Network.RemoteMessaging.Unified;

namespace GameFrameX.Hotfix.Logic.Server.Unified;

/// <summary>
/// 统一消息发送器的玩家路由装配选择。
/// </summary>
/// <remarks>
/// The resolver-selection wiring for <see cref="UnifiedMessageSenderHolder"/>.
/// The launch flow activates the Mongo discovery layer — and with it the
/// control-database player-route read side — strictly before the hotfix module loads,
/// so by the time <c>OnLoadSuccess</c> runs,
/// <c>MongoPlayerRouteResolverBootstrap.Resolver</c> is non-null whenever discovery
/// was activated. The selection therefore prefers the Mongo resolver (the
/// cross-process read side finally consuming the <c>player_route</c> collection) and
/// falls back to <see cref="DefaultPlayerRouteResolver"/> for shapes that never
/// activated discovery — preserving the legacy behavior exactly (the fallback branch is zero-change).
/// </remarks>
public static class PlayerRouteWiring
{
    /// <summary>
    /// 按发现层可用性选择玩家路由解析器：发现层已装配则用 Mongo 解析器（经适配器），否则回落缺省实现。
    /// </summary>
    /// <remarks>
    /// Selects the player-route resolver: the discovery-provided resolver wrapped in
    /// <see cref="RoutingPlayerRouteResolverAdapter"/> when present, otherwise the
    /// Hotfix <see cref="DefaultPlayerRouteResolver"/>. Pure function of the argument
    /// so the selection is unit-testable without a control database; the production
    /// caller passes <c>MongoPlayerRouteResolverBootstrap.Resolver</c>.
    /// </remarks>
    /// <param name="discoveredResolver">发现层暴露的解析器（未激活发现层时为 null）/ The discovery-provided resolver (null when discovery never activated)</param>
    /// <returns>装入 UnifiedMessageSenderHolder 的解析器 / The resolver to install into UnifiedMessageSenderHolder</returns>
    public static IPlayerRouteResolver SelectRouteResolver(GameFrameX.Discovery.Routing.IPlayerRouteResolver discoveredResolver)
    {
        if (discoveredResolver == null)
        {
            return new DefaultPlayerRouteResolver();
        }

        return new RoutingPlayerRouteResolverAdapter(discoveredResolver);
    }
}
