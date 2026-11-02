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
using GameFrameX.Network.Abstractions;
using GameFrameX.Network.RemoteMessaging.Unified;

namespace GameFrameX.Hotfix.Logic.Server.Unified;

/// <summary>
/// 玩家本服发送器默认实现。通过 PlayerSessionManager 投递消息给本服在线玩家。
/// </summary>
/// <remarks>
/// Default implementation of player local sender. Delivers messages to online players via PlayerSessionManager.
/// </remarks>
public sealed class DefaultPlayerLocalSender : IPlayerLocalSender
{
    /// <summary>
    /// 检查玩家是否在本服在线。通过 PlayerSessionManager 查找玩家会话，仅当会话存在且工作通道不为空时视为在线。
    /// </summary>
    /// <remarks>
    /// Checks whether the player is online on the local server. Looks up the player session via PlayerSessionManager and treats the player as online only when the session exists and its work channel is not null.
    /// </remarks>
    /// <param name="playerId">玩家ID / Player ID</param>
    /// <returns>是否在线 / Whether online</returns>
    public bool IsPlayerOnline(long playerId)
    {
        var session = PlayerSessionManager.Instance.GetByPlayerId(playerId);
        return session != null && session.WorkChannel != null;
    }

    /// <summary>
    /// 直接发送消息给本服在线玩家。通过会话异步写入消息，会话不存在或写入抛出异常时返回 false，不向上传播异常。
    /// </summary>
    /// <remarks>
    /// Sends a message directly to an online player on the local server. Writes the message asynchronously through the session; returns false without propagating exceptions when the session is missing or the write throws.
    /// </remarks>
    /// <param name="playerId">玩家ID / Player ID</param>
    /// <param name="message">消息对象 / Message object</param>
    /// <returns>是否发送成功 / Whether the send was successful</returns>
    public async Task<bool> SendToLocalPlayerAsync(long playerId, MessageObject message)
    {
        var session = PlayerSessionManager.Instance.GetByPlayerId(playerId);
        if (session == null)
        {
            return false;
        }

        try
        {
            await session.WriteAsync(message);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
