// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//
//  使用本项目须严格遵守相应法律法规与开源许可证之规定。
//
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please see the LICENSE file at the root directory of the source code for the full license text.
//
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  侵犯公民合法权益等法律法规所禁止的行为！
//  Any disputes and liabilities arising from secondary development of this project
//  shall be borne by the developer; the project organization and contributors assume no responsibility.
//
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository:  https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
// ==========================================================================================

using System.Threading.Tasks;
using GameFrameX.Core.BaseHandler.RPC;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Foundation.Logger;
using GameFrameX.Proto.Proto;

namespace GameFrameX.Hotfix.Logic.ServerRole.SessionResume;

/// <summary>
/// 会话恢复处理器（220-18）：新物理连接携带登录时下发的会话凭据，恢复身份绑定与
/// Online 对局（快照 / 增量）；失败一律 Online 显式段负值（≤ -1001），绝不静默回落。
/// </summary>
[MessageMapping(typeof(ReqSessionResume))]
internal sealed class ReqSessionResumeHandler : BaseRpcMessageHandler<ReqSessionResume, RespSessionResume>
{
    protected override async Task ActionAsync(ReqSessionResume request, RespSessionResume response, CancellationToken cancellationToken)
    {
        // Localization: Hotfix.SessionResume.ResumeRequested - 会话恢复请求, 会话ID: {0}
        LogHelper.Info(LocalizationService.GetString(Localization.Keys.Hotfix.SessionResume.ResumeRequested, NetworkChannel.GameAppSession?.SessionId));

        var outcome = await SessionResumeDomainBridge.ResumeAsync(NetworkChannel, request.Token, request.LastAcknowledgedSequence);
        if (!outcome.Succeeded)
        {
            // Localization: Hotfix.SessionResume.ResumeFailed - 会话恢复失败, Code: {0}, Message: {1}, 会话ID: {2}
            LogHelper.Error(LocalizationService.GetString(Localization.Keys.Hotfix.SessionResume.ResumeFailed, outcome.FailureCode, outcome.FailureMessage, NetworkChannel.GameAppSession?.SessionId));
            response.ErrorCode = -(int)outcome.FailureCode;
            return;
        }

        response.PlayerId = outcome.PlayerId;
        response.NewSessionToken = outcome.NewToken ?? string.Empty;

        var matchResume = outcome.MatchResume;
        if (matchResume != null && matchResume.Resumed)
        {
            response.Recovered = true;
            response.MatchId = matchResume.MatchId;
            response.MatchState = matchResume.MatchState;
            response.ServerSequence = matchResume.ServerSequence;
            response.DeltaAvailable = matchResume.DeltaAvailable;
            response.Instruction = matchResume.Instruction ?? string.Empty;
            response.GameState = matchResume.GameState;
            if (matchResume.Members != null)
            {
                foreach (var member in matchResume.Members)
                {
                    if (member == null)
                    {
                        continue;
                    }

                    response.Members.Add(new SessionResumeMemberInfo { PlayerId = member.PlayerId, State = member.State });
                }
            }
        }
        else if (outcome.MatchFailureCode != GameFrameX.Capability.Online.Abstractions.Contracts.OnlineErrorCode.None)
        {
            // 身份恢复成功、对局恢复显式失败（超窗 / 已结束 / 已离局等）：会话绑定保持，
            // 对局失败码经 MatchResultCode 回显——ErrorCode 保持 0，客户端始终能读到新凭据（GFX-1077）。
            response.Recovered = false;
            response.MatchResultCode = -(int)outcome.MatchFailureCode;
        }

        // Localization: Hotfix.SessionResume.ResumeSucceeded - 会话恢复成功, PlayerId: {0}, 新会话ID: {1}, 对局恢复: {2}, MatchId: {3}, ServerSequence: {4}
        LogHelper.Info(LocalizationService.GetString(Localization.Keys.Hotfix.SessionResume.ResumeSucceeded, outcome.PlayerId, NetworkChannel.GameAppSession?.SessionId, response.Recovered, response.MatchId, response.ServerSequence));
    }
}
