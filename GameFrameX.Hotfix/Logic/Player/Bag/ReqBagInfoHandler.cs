namespace GameFrameX.Hotfix.Logic.Player.Bag;

[MessageMapping(typeof(ReqBagInfo))]
internal sealed class ReqBagInfoHandler : PlayerRpcComponentHandler<BagComponentAgent, ReqBagInfo, RespBagInfo>
{
    protected override async Task ActionAsync(ReqBagInfo request, RespBagInfo response, CancellationToken cancellationToken)
    {
        try
        {
            await ComponentAgent.OnReqBagInfoAsync(NetworkChannel, request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
