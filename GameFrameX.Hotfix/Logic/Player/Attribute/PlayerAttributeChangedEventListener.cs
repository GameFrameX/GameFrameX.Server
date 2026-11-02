using GameFrameX.Apps.Common.EventData;
using GameFrameX.Core.Session;
using GameFrameX.Core.Abstractions.Events;

namespace GameFrameX.Hotfix.Logic.Player.Attribute;

/// <summary>
/// 监听玩家最终属性变化，向在线玩家 session 推送增量同步消息。
/// </summary>
[Event(typeof(AttributeChangedEventArgs))]
internal sealed class PlayerAttributeChangedEventListener : EventListener<PlayerAttributeComponentAgent>
{
    protected override async Task HandleEventAsync(PlayerAttributeComponentAgent agent, GameEventArgs gameEventArgs)
    {
        if (agent == null || !(gameEventArgs is AttributeChangedEventArgs args))
        {
            return;
        }

        var session = PlayerSessionManager.Instance.GetByPlayerId(args.PlayerId);
        if (session == null)
        {
            return;
        }

        await session.WriteAsync(PlayerAttributeSyncBuilder.BuildChanged(args.AttributeType, args.NewValue));
    }
}
