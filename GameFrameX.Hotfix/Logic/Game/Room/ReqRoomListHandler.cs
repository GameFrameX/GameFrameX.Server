namespace GameFrameX.Hotfix.Logic.Game.Room;

[MessageMapping(typeof(ReqRoomList))]
internal sealed class ReqRoomListHandler : GlobalRpcComponentHandler<RoomComponentAgent, ReqRoomList, RespRoomList>
{
    protected override async Task ActionAsync(ReqRoomList request, RespRoomList response, CancellationToken cancellationToken)
    {
        try
        {
            await ComponentAgent.OnReqRoomListAsync(request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
