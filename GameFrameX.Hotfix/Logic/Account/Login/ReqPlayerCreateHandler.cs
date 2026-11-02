namespace GameFrameX.Hotfix.Logic.Account.Login;

using PlayerLoginComponentAgent = GameFrameX.Hotfix.Logic.Player.Login.LoginComponentAgent;

[MessageMapping(typeof(ReqPlayerCreate))]
internal sealed class ReqPlayerCreateHandler : GlobalRpcComponentHandler<PlayerLoginComponentAgent, ReqPlayerCreate, RespPlayerCreate>
{
    protected override async Task ActionAsync(ReqPlayerCreate request, RespPlayerCreate response, CancellationToken cancellationToken)
    {
        await ComponentAgent.OnPlayerCreateAsync(request, response);
    }
}
