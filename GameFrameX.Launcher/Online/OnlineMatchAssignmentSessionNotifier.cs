//  ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and related international regulations.
//   使用本项目须严格遵守相应法律法规与开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 Apache License 2.0 单协议分发，
//   This project is licensed solely under the Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please see the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes or liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository: https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository: https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


using System.Threading;
using System.Threading.Tasks;
using GameFrameX.Capability.Online.Match;
using GameFrameX.Core.Session;
using GameFrameX.Proto.Proto;

namespace GameFrameX.Launcher.Online;

/// <summary>
/// 撮合成立通知的宿主投递实现（生产实现）：按旧链会话簿记定位成员连接并 best-effort 写出
/// <see cref="NotifyMatchMade"/> 协议消息——会话通道与协议产物只存在于宿主侧，能力包经出站端口解耦。
/// <para>
/// 维护约束（红线）：逐成员 best-effort——离线（无会话）成员静默跳过，等待方经状态轮询兜底；
/// 写出异常按调用方既有批次隔离路径外抛（单条失败不吞没，交调度器日志出口观测）。
/// </para>
/// </summary>
public sealed class OnlineMatchAssignmentSessionNotifier : IOnlineMatchAssignmentNotifier
{
    /// <summary>
    /// 向通知载荷内的全部成员投递撮合成立通知：逐成员解析在线会话，离线跳过，在线写出协议消息。
    /// </summary>
    /// <param name="notification">撮合成立通知载荷（协议无关）。</param>
    /// <param name="cancellationToken">停止取消令牌。</param>
    /// <returns>投递任务。</returns>
    public async Task NotifyAssignmentCreatedAsync(OnlineMatchAssignmentNotification notification, CancellationToken cancellationToken)
    {
        var notify = new NotifyMatchMade { MatchId = notification.MatchId, Mode = notification.Mode };
        foreach (var playerId in notification.PlayerIds)
        {
            var session = PlayerSessionManager.Instance.GetByPlayerId(playerId);
            if (session == null)
            {
                // 离线方静默跳过：best-effort 推送，等待方经状态轮询兜底。
                continue;
            }

            await session.WriteAsync(notify).ConfigureAwait(false);
        }
    }
}
