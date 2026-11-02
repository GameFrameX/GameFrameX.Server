namespace GameFrameX.Hotfix.Logic.Game.RockPaperScissors;

[MessageMapping(typeof(ReqSubmitRockPaperScissorsGesture))]
internal sealed class ReqSubmitRockPaperScissorsGestureHandler : GlobalRpcComponentHandler<RockPaperScissorsGameComponentAgent, ReqSubmitRockPaperScissorsGesture, RespSubmitRockPaperScissorsGesture>
{
    protected override async Task ActionAsync(ReqSubmitRockPaperScissorsGesture request, RespSubmitRockPaperScissorsGesture response, CancellationToken cancellationToken)
    {
        try
        {
            var playerId = RockPaperScissorsHandlerHelper.GetCurrentPlayerId(NetworkChannel);
            await ComponentAgent.OnSubmitGestureAsync(playerId, request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
