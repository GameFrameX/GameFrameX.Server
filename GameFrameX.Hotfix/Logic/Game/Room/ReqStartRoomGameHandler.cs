namespace GameFrameX.Hotfix.Logic.Game.Room;

[MessageMapping(typeof(ReqStartRoomGame))]
internal sealed class ReqStartRoomGameHandler : GlobalRpcComponentHandler<RoomComponentAgent, ReqStartRoomGame, RespStartRoomGame>
{
    protected override async Task ActionAsync(ReqStartRoomGame request, RespStartRoomGame response, CancellationToken cancellationToken)
    {
        try
        {
            var playerId = RoomHandlerHelper.GetCurrentPlayerId(NetworkChannel);
            await ComponentAgent.OnStartRoomGameAsync(playerId, request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
