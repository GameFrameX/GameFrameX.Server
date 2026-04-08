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
using GameFrameX.NetWork.Messages;
using ProtoBuf;

namespace GameFrameX.NetWork.RemoteMessaging.Unified;

/// <summary>
/// 跨服转发玩家消息的内部请求协议。
/// 当玩家不在本服时，通过此消息将原始消息转发到目标服。
/// </summary>
/// <remarks>
/// Internal request protocol for cross-server player message forwarding.
/// When the player is not on the local server, the original message is forwarded to the target server via this message.
/// </remarks>
[ProtoContract]
public sealed class ReqSendToPlayerInner : MessageObject, IRequestMessage
{
    /// <summary>
    /// 目标玩家ID
    /// </summary>
    /// <remarks>
    /// Target player ID.
    /// </remarks>
    [ProtoMember(1)]
    public long TargetPlayerId { get; set; }

    /// <summary>
    /// 需要投递给玩家的原始消息
    /// </summary>
    /// <remarks>
    /// The original message to be delivered to the player.
    /// </remarks>
    [ProtoMember(2)]
    public MessageObject InnerMessage { get; set; }

    /// <inheritdoc />
    public override void Clear()
    {
        TargetPlayerId = 0;
        InnerMessage = null;
    }
}

/// <summary>
/// 跨服转发玩家消息的内部响应协议。
/// </summary>
/// <remarks>
/// Internal response protocol for cross-server player message forwarding.
/// </remarks>
[ProtoContract]
public sealed class RespSendToPlayerInner : MessageObject, IResponseMessage
{
    /// <summary>
    /// 是否投递成功
    /// </summary>
    /// <remarks>
    /// Whether delivery was successful.
    /// </remarks>
    [ProtoMember(1)]
    public bool Success { get; set; }

    /// <summary>
    /// 目标玩家是否在线
    /// </summary>
    /// <remarks>
    /// Whether the target player is online.
    /// </remarks>
    [ProtoMember(2)]
    public bool PlayerOffline { get; set; }

    /// <inheritdoc />
    [ProtoMember(3)]
    public int ErrorCode { get; set; }

    /// <inheritdoc />
    public override void Clear()
    {
        Success = false;
        PlayerOffline = false;
        ErrorCode = 0;
    }
}
