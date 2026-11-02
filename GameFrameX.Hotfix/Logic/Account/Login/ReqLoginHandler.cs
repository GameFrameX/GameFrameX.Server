namespace GameFrameX.Hotfix.Logic.Account.Login;

using PlayerLoginComponentAgent = GameFrameX.Hotfix.Logic.Player.Login.LoginComponentAgent;

[MessageMapping(typeof(ReqLogin))]
internal sealed class ReqLoginHandler : GlobalRpcComponentHandler<PlayerLoginComponentAgent, ReqLogin, RespLogin>
{
    protected override async Task ActionAsync(ReqLogin request, RespLogin response, CancellationToken cancellationToken)
    {
        await ComponentAgent.OnLoginAsync(request, response);
    }
}
