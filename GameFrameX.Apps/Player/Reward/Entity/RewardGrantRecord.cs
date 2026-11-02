namespace GameFrameX.Apps.Player.Reward.Entity;

/// <summary>
/// 统一奖励发放的一次幂等账本记录，存储在玩家 <see cref="RewardGrantRecordState"/> 中。
/// </summary>
/// <remarks>
/// 该记录是发放幂等（B6）的事实来源：同一 <see cref="TraceKey"/> 再次提交时，发放接口直接读取本记录返回上次结果，
/// 不重复写背包、不重复扣附件。记录一旦写入即不可变（不更新、不删除，一期无 TTL / 容量回收）。
/// </remarks>
public sealed class RewardGrantRecord
{
    /// <summary>
    /// 幂等键，格式 <c>RoleId:SourceType:SourceId:TraceId</c>。
    /// </summary>
    public string TraceKey { get; set; }

    /// <summary>
    /// 来源类型（冗余存储，便于审计）。
    /// </summary>
    public int SourceType { get; set; }

    /// <summary>
    /// 来源 ID（冗余存储，便于审计）。
    /// </summary>
    public string SourceId { get; set; }

    /// <summary>
    /// 调用方追踪 ID（冗余存储，便于审计）。
    /// </summary>
    public string TraceId { get; set; }

    /// <summary>
    /// 发放完成的 unix 秒。
    /// </summary>
    public long GrantedAt { get; set; }

    /// <summary>
    /// 是否全部成功。
    /// </summary>
    public bool AllSuccess { get; set; }

    /// <summary>
    /// 整体错误码（<c>OperationStatusCode</c> 整数值）。
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>
    /// 逐项发放结果快照。重复提交时据此重建返回结果。
    /// </summary>
    public List<RewardGrantItemResult> Items { get; set; } = new List<RewardGrantItemResult>();
}
