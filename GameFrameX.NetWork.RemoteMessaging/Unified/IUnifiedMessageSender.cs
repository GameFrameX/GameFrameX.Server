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
/// 统一消息发送接口。提供两套核心发送能力：
/// <para>SendToPlayerAsync — 发送给目标玩家（内部自动处理本服、跨服、离线）。</para>
/// <para>SendToServerAsync — 发送给目标服务（内部自动处理服务发现、实例选择、一致性路由）。</para>
/// 业务层不感知底层协议（TCP/KCP/QUIC），只感知"发送目标与语义"。
/// </summary>
/// <remarks>
/// Unified message sending interface. Provides two core sending capabilities:
/// SendToPlayerAsync — Send to target player (automatically handles local, cross-server, and offline delivery).
/// SendToServerAsync — Send to target service (automatically handles service discovery, instance selection, consistent routing).
/// The business layer is unaware of the underlying protocol (TCP/KCP/QUIC), only aware of "target and semantics".
/// </remarks>
public interface IUnifiedMessageSender
{
    // ─────────────────────────────────────────────────────────────
    //  SendToPlayerAsync — 玩家消息
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// 发送消息给目标玩家。内部自动处理本服直发、跨服转发、离线处理。
    /// </summary>
    /// <remarks>
    /// Sends a message to the target player. Automatically handles local delivery, cross-server forwarding, and offline processing.
    /// </remarks>
    /// <param name="playerId">目标玩家ID / Target player ID</param>
    /// <param name="message">消息对象 / Message object</param>
    /// <param name="options">发送选项 / Send options</param>
    /// <param name="ct">取消令牌 / Cancellation token</param>
    /// <returns>发送结果 / Send result</returns>
    Task<PlayerSendResult> SendToPlayerAsync(long playerId, MessageObject message, PlayerSendOptions options = null, CancellationToken ct = default);

    // ─────────────────────────────────────────────────────────────
    //  SendToServerAsync — 系统消息（请求-响应）
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// 发送请求-响应消息给目标服务。内部自动处理服务发现、实例选择、一致性路由。
    /// </summary>
    /// <remarks>
    /// Sends a request-response message to the target service. Automatically handles service discovery, instance selection, and consistent routing.
    /// </remarks>
    /// <typeparam name="TResp">响应消息类型 / Response message type</typeparam>
    /// <param name="serviceName">目标服务名 / Target service name</param>
    /// <param name="message">请求消息对象 / Request message object</param>
    /// <param name="options">发送选项 / Send options</param>
    /// <param name="ct">取消令牌 / Cancellation token</param>
    /// <returns>结构化调用结果 / Structured call result</returns>
    Task<ServerSendResult<TResp>> SendToServerAsync<TResp>(string serviceName, MessageObject message, ServerSendOptions options = null, CancellationToken ct = default)
        where TResp : class, IResponseMessage;

    // ─────────────────────────────────────────────────────────────
    //  SendToServerOneWayAsync — 系统消息（单向）
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// 单向发送消息给目标服务，不等待响应。
    /// </summary>
    /// <remarks>
    /// Sends a one-way message to the target service without waiting for a response.
    /// </remarks>
    /// <param name="serviceName">目标服务名 / Target service name</param>
    /// <param name="message">消息对象 / Message object</param>
    /// <param name="options">发送选项 / Send options</param>
    /// <param name="ct">取消令牌 / Cancellation token</param>
    /// <returns>发送结果 / Send result</returns>
    Task<ServerSendResult> SendToServerOneWayAsync(string serviceName, MessageObject message, ServerSendOptions options = null, CancellationToken ct = default);
}
