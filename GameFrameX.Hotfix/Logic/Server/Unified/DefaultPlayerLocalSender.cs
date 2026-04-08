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
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.RemoteMessaging.Unified;

namespace GameFrameX.Hotfix.Logic.Server.Unified;

/// <summary>
/// 玩家本服发送器默认实现。通过 SessionManager 投递消息给本服在线玩家。
/// </summary>
/// <remarks>
/// Default implementation of player local sender. Delivers messages to online players via SessionManager.
/// </remarks>
public sealed class DefaultPlayerLocalSender : IPlayerLocalSender
{
    /// <inheritdoc />
    public bool IsPlayerOnline(long playerId)
    {
        var session = SessionManager.GetByRoleId(playerId);
        return session != null && session.WorkChannel != null;
    }

    /// <inheritdoc />
    public async Task<bool> SendToLocalPlayerAsync(long playerId, MessageObject message)
    {
        var session = SessionManager.GetByRoleId(playerId);
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
