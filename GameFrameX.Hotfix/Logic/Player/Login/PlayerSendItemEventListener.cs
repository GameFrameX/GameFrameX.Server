using GameFrameX.Apps.Common.EventData;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Foundation.Localization.Core;

namespace GameFrameX.Hotfix.Logic.Player.Login;

[Event(typeof(PlayerSendItemEventArgs))]
internal sealed class PlayerSendItemEventListener : EventListener<PlayerComponentAgent>
{
    protected override Task HandleEventAsync(PlayerComponentAgent agent, GameEventArgs gameEventArgs)
    {
        if (agent == null)
        {
            // Localization: Hotfix.Player.AgentIsNull - 代理对象为空
            LogHelper.Error(LocalizationService.GetString(Localization.Keys.Hotfix.Player.AgentIsNull));
            return Task.CompletedTask;
        }

        return agent.OnLogoutAsync();
    }
}
