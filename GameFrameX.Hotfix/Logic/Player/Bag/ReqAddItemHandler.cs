namespace GameFrameX.Hotfix.Logic.Player.Bag;

/// <summary>
/// 增加背包物品
/// </summary>
[MessageMapping(typeof(ReqAddItem))]
internal sealed class ReqAddItemHandler : PlayerRpcComponentHandler<BagComponentAgent, ReqAddItem, RespAddItem>
{
    protected override async Task ActionAsync(ReqAddItem request, RespAddItem response, CancellationToken cancellationToken)
    {
        try
        {
            await ComponentAgent.OnAddBagItemAsync(NetworkChannel, request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
