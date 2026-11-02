
namespace GameFrameX.Hotfix.Logic.Game.RockPaperScissors;
/// <summary>
/// 对局桥观察视图后的房间状态推进动作（notes §1.3 Room ↔ Match 映射的决策结果）。
/// </summary>
public enum RockPaperScissorsMatchRoomAction
{
    /// <summary>不推进（组合不满足映射条件）。</summary>
    None = 0,

    /// <summary>Playing → Settling（桥视图 Settling 驱动）。</summary>
    EnterSettling = 1,

    /// <summary>Playing/Settling → Settled（桥视图终态 Completed/SettlementFailed/Terminated 驱动）。</summary>
    MarkSettled = 2,
}