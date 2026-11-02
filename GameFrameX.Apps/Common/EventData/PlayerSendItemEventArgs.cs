using GameFrameX.Core.Abstractions.Events;

namespace GameFrameX.Apps.Common.EventData;

/// <summary>
/// 发送道具给玩家
/// </summary>
public sealed class PlayerSendItemEventArgs : GameEventArgs
{
    /// <summary>
    /// 玩家ID
    /// </summary>
    public long PlayerId { get; }

    /// <summary>
    /// 道具列表，键为道具ID，值为道具数量
    /// </summary>
    public Dictionary<int, long> Items { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="playerId">玩家ID</param>
    /// <param name="items">道具列表</param>
    public PlayerSendItemEventArgs(long playerId, Dictionary<int, long> items)
    {
        PlayerId = playerId;
        Items = items;
    }
}
