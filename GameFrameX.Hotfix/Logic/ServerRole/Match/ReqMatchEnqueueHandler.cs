// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and related international regulations.
//
//  使用本项目须严格遵守相应法律法规与开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录的 LICENSE 文件。
//  please refer to the LICENSE file at the source code root for the full license text.
//
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any disputes and liabilities arising from secondary development,
//  本组织不承担任何责任，开发者须自行承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository: https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using System.Threading.Tasks;
using GameFrameX.Core.BaseHandler.RPC;
using GameFrameX.Proto.Proto;

namespace GameFrameX.Hotfix.Logic.ServerRole.Match;

/// <summary>
/// 匹配协议族（域号 420）薄适配处理器（经匹配域桥转发 Online 契约面；
/// 旧 FIFO 内存池组件链随切换退役）。
/// <para>
/// 身份语义保持：请求主体 = 会话绑定玩家 id，未登录回 <see cref="MatchErrorCode.NotLoggedIn"/>；
/// 入队成功回执的 MatchId 为 0；客户端等待匹配通知或状态查询结果后入场，错误码映射由 <c>MatchDomainBridge</c> 统一承载。
/// </para>
/// </summary>
/// <remarks>
/// Enqueue success returns MatchId 0; clients wait for a match notification or status result before entering.
/// </remarks>
[MessageMapping(typeof(ReqMatchEnqueue))]
internal sealed class ReqMatchEnqueueHandler : BaseRpcMessageHandler<ReqMatchEnqueue, RespMatchEnqueue>
{
    protected override async Task ActionAsync(ReqMatchEnqueue request, RespMatchEnqueue response, CancellationToken cancellationToken)
    {
        var playerId = ServerRoleHandlerHelper.GetCurrentPlayerId(NetworkChannel);
        if (playerId == 0)
        {
            response.ErrorCode = (int)MatchErrorCode.NotLoggedIn;
            return;
        }

        await MatchDomainBridge.EnqueueAsync(playerId, request, response);
    }
}
