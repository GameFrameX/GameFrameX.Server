using System.Threading.Tasks;
using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.Core.Abstractions.Attribute;
using GameFrameX.Core.Components;
using GameFrameX.Monitor.Player;
using GameFrameX.Setting;

namespace GameFrameX.Apps.Player.Player.Component;

[ComponentType(GlobalConst.ActorTypePlayer)]
public sealed class PlayerComponent : StateComponent<PlayerState>
{
    public async Task<PlayerState> OnPlayerLogin(ReqPlayerLogin reqLogin)
    {
        MetricsPlayerRegister.LoginCounterOptions.Inc();
        return await GameDb.FindAsync<PlayerState>(m => m.Id == reqLogin.Id);
    }
}