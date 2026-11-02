namespace GameFrameX.Hotfix.Logic.Player.Login;

[MessageMapping(typeof(ReqPlayerLogin))]
internal sealed class ReqPlayerLoginHandler : GlobalRpcComponentHandler<LoginComponentAgent, ReqPlayerLogin, RespPlayerLogin>
{
    protected override async Task ActionAsync(ReqPlayerLogin request, RespPlayerLogin response, CancellationToken cancellationToken)
    {
        await ComponentAgent.OnPlayerLoginAsync(NetworkChannel, request, response);
    }
}
