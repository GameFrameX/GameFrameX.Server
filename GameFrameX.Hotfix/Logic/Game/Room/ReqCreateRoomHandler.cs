namespace GameFrameX.Hotfix.Logic.Game.Room;

[MessageMapping(typeof(ReqCreateRoom))]
internal sealed class ReqCreateRoomHandler : GlobalRpcComponentHandler<RoomComponentAgent, ReqCreateRoom, RespCreateRoom>
{
    protected override async Task ActionAsync(ReqCreateRoom request, RespCreateRoom response, CancellationToken cancellationToken)
    {
        try
        {
            var playerId = RoomHandlerHelper.GetCurrentPlayerId(NetworkChannel);
            await ComponentAgent.OnCreateRoomAsync(playerId, request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
