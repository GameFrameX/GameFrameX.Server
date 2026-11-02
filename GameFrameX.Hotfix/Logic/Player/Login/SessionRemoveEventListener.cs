using GameFrameX.Core.Session;
using GameFrameX.Core.Abstractions.Events;

namespace GameFrameX.Hotfix.Logic.Player.Login;

[Event(typeof(SessionRemovedEventArgs))]
internal sealed class SessionRemoveEventListener : EventListener<PlayerComponentAgent>
{
    protected override Task HandleEventAsync(PlayerComponentAgent agent, GameEventArgs gameEventArgs)
    {
        return agent.OnLogoutAsync();
    }
}
