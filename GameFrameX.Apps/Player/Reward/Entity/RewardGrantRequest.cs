namespace GameFrameX.Apps.Player.Reward.Entity;

/// <summary>
/// 统一奖励发放接口的请求。
/// </summary>
/// <remarks>
/// 调用方必须保证 <see cref="PlayerId"/> 命中目标玩家、<see cref="SourceType"/>/<see cref="SourceId"/>/<see cref="TraceId"/> 三者共同唯一。
/// 幂等键 = <c>RoleId:SourceType:SourceId:TraceId</c>（B6），重复提交不重复发奖、短路返回上次结果。键格式为持久化契约，勿随命名统一改动。
/// </remarks>
public sealed class RewardGrantRequest
{
    /// <summary>
    /// 目标玩家 ID（玩家 Actor 的 <c>ActorId</c>，等同玩家 <c>PlayerState.Id</c>）。
    /// </summary>
    public long PlayerId { get; set; }

    /// <summary>
    /// 奖励来源类型。禁止使用 <see cref="RewardSourceType.None"/> 拼接幂等键。
    /// </summary>
    public RewardSourceType SourceType { get; set; }

    /// <summary>
    /// 来源 ID。建议形如 <c>mail:&lt;MailId&gt;:attachment:&lt;AttachmentId&gt;</c>，与调用方业务一一对应。
    /// </summary>
    public string SourceId { get; set; }

    /// <summary>
    /// 调用方追踪 ID。与 <see cref="SourceType"/>/<see cref="SourceId"/> 组成幂等键的一部分。
    /// </summary>
    public string TraceId { get; set; }

    /// <summary>
    /// 待发放奖励列表。逐项独立判定成功 / 失败。
    /// </summary>
    public List<RewardItem> Rewards { get; set; } = new List<RewardItem>();
}
