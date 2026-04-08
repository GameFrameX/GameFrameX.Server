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
using GameFrameX.Core.BaseHandler;
using GameFrameX.Hotfix.Logic.Server;
using GameFrameX.NetWork.RemoteMessaging.Unified;

namespace GameFrameX.Hotfix.Logic.Server.Unified;

/// <summary>
/// 跨服转发玩家消息的内部处理器。
/// 在目标服接收 ReqSendToPlayerInner 请求，通过本地 Session 投递消息。
/// 绑定到 ServerComponentAgent 以复用服务器级 Actor 基础设施。
/// </summary>
/// <remarks>
/// Internal handler for cross-server player message forwarding.
/// Receives ReqSendToPlayerInner requests on the target server and delivers messages via local Session.
/// Binds to ServerComponentAgent to reuse server-level Actor infrastructure.
/// </remarks>
[MessageMapping(typeof(ReqSendToPlayerInner))]
internal sealed class SendToPlayerInnerHandler : PlayerRpcComponentHandler<ServerComponentAgent, ReqSendToPlayerInner, RespSendToPlayerInner>
{
    protected override async Task ActionAsync(ReqSendToPlayerInner request, RespSendToPlayerInner response)
    {
        if (request.TargetPlayerId <= 0)
        {
            response.Success = false;
            response.ErrorCode = -1;
            return;
        }

        var session = SessionManager.GetByRoleId(request.TargetPlayerId);
        if (session == null)
        {
            response.Success = false;
            response.PlayerOffline = true;
            response.ErrorCode = -2;
            return;
        }

        if (request.InnerMessage == null)
        {
            response.Success = false;
            response.ErrorCode = -3;
            return;
        }

        try
        {
            await session.WriteAsync(request.InnerMessage);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorCode = -4;
            LogHelper.Error(ex, "SendToPlayerInnerHandler delivery failed, PlayerId:{PlayerId}", request.TargetPlayerId);
        }
    }
}
