// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规与开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 Apache License 2.0 单协议分发，
//   This project is licensed solely under the Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes or liabilities arising from secondary development based on this project
//   本组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository:  https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:    https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using GameFrameX.Core.Session;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Hotfix.Logic.Game.Room;

namespace GameFrameX.Hotfix.Logic.ServerRole.SessionResume;

/// <summary>
/// 玩家物理断开业务监听器（宿主→热更事件桥接承接方）：房间断线标记 + 会话恢复两层协同通知。
/// </summary>
/// <remarks>
/// The player-disconnect business listener (the host-to-hotfix event-bridging sink): marks the
/// player disconnected in the room system and notifies the session-resume two-layer coordination.
/// 全局分发（actorId = 0）时监听器被投递到房间代理所属的全局 Actor 串行执行，并收到该 Actor 上的真实房间代理；
/// <see cref="SessionResumeDomainBridge.NotifyPlayerDisconnectedAsync"/> 自带旧回调迟到守卫，
/// 恢复先于旧断开回调到达时安全跳过。
/// Under the global dispatch (actorId = 0) the listener is posted to the global actor owning the
/// room agent for serialized execution and receives the real room agent on that actor;
/// <see cref="SessionResumeDomainBridge.NotifyPlayerDisconnectedAsync"/>
/// carries its own stale-callback guard — a resume that outruns the stale disconnect callback is
/// skipped safely.
/// </remarks>
[Event(typeof(PlayerDisconnectedEventArgs))]
internal sealed class PlayerDisconnectedEventListener : EventListener<RoomComponentAgent>
{
    /// <summary>
    /// 承接玩家物理断开业务：房间断线标记（等待重连或超时清理）+ 会话恢复两层协同通知
    ///（会话域开重连窗 + 对局域标记断线成员）。
    /// </summary>
    /// <param name="agent">分发投递的所属全局 Actor 上的房间代理 / The room agent on the owning global actor of the dispatch</param>
    /// <param name="gameEventArgs">断开事件参数 / The disconnect event args</param>
    /// <returns>异步任务 / The async task</returns>
    protected override async Task HandleEventAsync(RoomComponentAgent agent, GameEventArgs gameEventArgs)
    {
        var eventArgs = (PlayerDisconnectedEventArgs)gameEventArgs;

        // 房间系统：标记玩家断线，等待重连或超时清理（agent 为分发传入的所属 Actor 代理）
        await agent.MarkPlayerDisconnectedAsync(eventArgs.PlayerId);

        // 两层会话协同：Online 会话域开重连窗 + 对局域标记断线成员（含旧回调迟到守卫）。
        await SessionResumeDomainBridge.NotifyPlayerDisconnectedAsync(eventArgs.PlayerId, eventArgs.SessionId);
    }
}
