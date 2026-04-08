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

using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.NetWork.RemoteMessaging.Unified;

/// <summary>
/// 玩家本服发送器接口。负责在本服直接通过 Session 投递消息。
/// </summary>
/// <remarks>
/// Player local sender interface. Responsible for delivering messages directly via Session on the local server.
/// </remarks>
public interface IPlayerLocalSender
{
    /// <summary>
    /// 检查玩家是否在本服在线。
    /// </summary>
    /// <remarks>
    /// Checks whether the player is online on the local server.
    /// </remarks>
    /// <param name="playerId">玩家ID / Player ID</param>
    /// <returns>是否在线 / Whether online</returns>
    bool IsPlayerOnline(long playerId);

    /// <summary>
    /// 直接发送消息给本服在线玩家。
    /// </summary>
    /// <remarks>
    /// Sends a message directly to an online player on the local server.
    /// </remarks>
    /// <param name="playerId">玩家ID / Player ID</param>
    /// <param name="message">消息对象 / Message object</param>
    /// <returns>是否发送成功 / Whether the send was successful</returns>
    Task<bool> SendToLocalPlayerAsync(long playerId, MessageObject message);
}
