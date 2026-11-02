namespace GameFrameX.Hotfix.Logic.Game.RockPaperScissors;

[MessageMapping(typeof(ReqRockPaperScissorsGameInfo))]
internal sealed class ReqRockPaperScissorsGameInfoHandler : GlobalRpcComponentHandler<RockPaperScissorsGameComponentAgent, ReqRockPaperScissorsGameInfo, RespRockPaperScissorsGameInfo>
{
    protected override async Task ActionAsync(ReqRockPaperScissorsGameInfo request, RespRockPaperScissorsGameInfo response, CancellationToken cancellationToken)
    {
        try
        {
            var playerId = RockPaperScissorsHandlerHelper.GetCurrentPlayerId(NetworkChannel);
            await ComponentAgent.OnReqGameInfoAsync(playerId, request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
