using GameFrameX.Apps.Account.Login.Entity;
using GameFrameX.Apps.Player.Player.Entity;

namespace GameFrameX.Apps.Account.Login.Component;

[ComponentType(GlobalConst.ActorTypeServer)]
public sealed class LoginComponent : StateComponent<LoginState>
{
    protected override bool IsCreateDefaultState { get; set; } = false;

    public async Task<PlayerState> OnPlayerLoginAsync(ReqPlayerLogin reqLogin)
    {
        AppMetrics.PlayerLogin.Add(1);
        return await GameDb.FindAsync<PlayerState>(m => m.Id == reqLogin.Id, false);
    }
}
