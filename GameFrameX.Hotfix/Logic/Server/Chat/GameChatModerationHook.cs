// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and related international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 Apache License 2.0 单协议分发，
//   This project is licensed solely under the Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   Any legal disputes and liabilities arising from secondary development based on this project
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository: https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool
//   CNB Repository: https://cnb.cool
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


using System.Threading;
using GameFrameX.Capability.Online.Abstractions;
using System.Threading.Tasks;

namespace GameFrameX.Hotfix.Logic.Server.Chat;
/// <summary>
/// 聊天内容审核钩子示例实现（C294 热更侧钩子接线样例）：对消息内容做本地长度与空白校验后放行——
/// 演示「热更装载完成挂载、退役释放」的钩子生命周期，不承载真实第三方审核。
/// <para>
/// 维护约束：本实现按 <see cref="IOnlineChatModerationHook"/> 契约的红线书写——纯本地同步裁决
///（无外部 IO、无超时风险）；拒绝理由只供审计不透出；真实审核服务接入时以新热更版本替换本挂载
///（<see cref="GameFrameX.Capability.Hosting.Abstractions.CapabilityHook{TContract}.Attach"/> 原子替换，旧租约自动失效）。
/// </para>
/// </summary>
/// <remarks>
/// The sample chat-moderation hook implementation (the C294 hotfix-side hook wiring sample): applies a
/// local length / blank check to the message content and allows everything else — it demonstrates the
/// hook lifecycle (attach on hotfix load completion, release on retirement) and carries no real
/// third-party moderation. The implementation honours the hook contract red lines — a pure local
/// synchronous verdict (no external IO, no timeout risk), reasons for audit only; a real moderation
/// service replaces this mount with a new hotfix version (the atomic Attach replaces the old lease).
/// </remarks>
public sealed class GameChatModerationHook : IOnlineChatModerationHook
{
    /// <summary>
    /// 消息内容最大长度（超出即拒绝；样例阈值，演示本地裁决形态）。
    /// </summary>
    /// <remarks>
    /// The maximum message content length (rejections beyond it; a sample threshold demonstrating the
    /// local-verdict shape).
    /// </remarks>
    public const int MaximumContentLength = 500;

    /// <summary>
    /// 本地同步裁决聊天消息：内容为空白或超过 <see cref="MaximumContentLength"/> 即拒绝并附原因，其余一律放行；不访问外部服务。
    /// </summary>
    /// <remarks>
    /// Synchronously judges a chat message locally: blank content or content longer than <see cref="MaximumContentLength"/>
    /// is rejected with a reason; everything else is allowed. No external IO is performed and the cancellation token is not consulted.
    /// </remarks>
    /// <param name="message">待审核的聊天消息 / Chat message to moderate</param>
    /// <param name="cancellationToken">取消令牌（本实现为同步裁决，未使用） / Cancellation token (unused by this synchronous verdict)</param>
    /// <returns>已完成的裁决任务：拒绝（空白或超长）或放行 / Completed verdict task: reject (blank or too long) or allow</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> 为 null / <paramref name="message"/> is null</exception>
    public Task<OnlineChatModerationVerdict> CheckAsync(OnlineChatMessage message, CancellationToken cancellationToken = default)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (string.IsNullOrWhiteSpace(message.Content))
        {
            return Task.FromResult(OnlineChatModerationVerdict.Reject("空白消息"));
        }

        if (message.Content.Length > MaximumContentLength)
        {
            return Task.FromResult(OnlineChatModerationVerdict.Reject("消息超过最大长度 " + MaximumContentLength));
        }

        return Task.FromResult(OnlineChatModerationVerdict.Allow());
    }
}
