namespace GameFrameX.Hotfix.Logic.Account.Login;

using PlayerLoginComponentAgent = GameFrameX.Hotfix.Logic.Player.Login.LoginComponentAgent;

[MessageMapping(typeof(ReqPlayerList))]
internal sealed class ReqPlayerListHandler : GlobalRpcComponentHandler<PlayerLoginComponentAgent, ReqPlayerList, RespPlayerList>
{
    protected override async Task ActionAsync(ReqPlayerList request, RespPlayerList response, CancellationToken cancellationToken)
    {
        await ComponentAgent.OnGetPlayerListAsync(request, response);
    }
}
