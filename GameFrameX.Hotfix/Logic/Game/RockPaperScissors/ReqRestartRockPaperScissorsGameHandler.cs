namespace GameFrameX.Hotfix.Logic.Game.RockPaperScissors;

[MessageMapping(typeof(ReqRestartRockPaperScissorsGame))]
internal sealed class ReqRestartRockPaperScissorsGameHandler : GlobalRpcComponentHandler<RockPaperScissorsGameComponentAgent, ReqRestartRockPaperScissorsGame, RespRestartRockPaperScissorsGame>
{
    protected override async Task ActionAsync(ReqRestartRockPaperScissorsGame request, RespRestartRockPaperScissorsGame response, CancellationToken cancellationToken)
    {
        try
        {
            var playerId = RockPaperScissorsHandlerHelper.GetCurrentPlayerId(NetworkChannel);
            await ComponentAgent.OnRestartGameAsync(playerId, request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
