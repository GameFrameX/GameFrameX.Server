namespace GameFrameX.Hotfix.Logic.Game.Room;

[MessageMapping(typeof(ReqJoinRoom))]
internal sealed class ReqJoinRoomHandler : GlobalRpcComponentHandler<RoomComponentAgent, ReqJoinRoom, RespJoinRoom>
{
    protected override async Task ActionAsync(ReqJoinRoom request, RespJoinRoom response, CancellationToken cancellationToken)
    {
        try
        {
            var playerId = RoomHandlerHelper.GetCurrentPlayerId(NetworkChannel);
            await ComponentAgent.OnJoinRoomAsync(playerId, request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
