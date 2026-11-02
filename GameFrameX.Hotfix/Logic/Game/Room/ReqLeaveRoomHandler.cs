namespace GameFrameX.Hotfix.Logic.Game.Room;

[MessageMapping(typeof(ReqLeaveRoom))]
internal sealed class ReqLeaveRoomHandler : GlobalRpcComponentHandler<RoomComponentAgent, ReqLeaveRoom, RespLeaveRoom>
{
    protected override async Task ActionAsync(ReqLeaveRoom request, RespLeaveRoom response, CancellationToken cancellationToken)
    {
        try
        {
            var playerId = RoomHandlerHelper.GetCurrentPlayerId(NetworkChannel);
            await ComponentAgent.OnLeaveRoomAsync(playerId, request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
