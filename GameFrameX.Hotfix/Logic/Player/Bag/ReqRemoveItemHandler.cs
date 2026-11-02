namespace GameFrameX.Hotfix.Logic.Player.Bag;

/// <summary>
/// 减少背包物品
/// </summary>
[MessageMapping(typeof(ReqRemoveItem))]
internal sealed class ReqRemoveItemHandler : PlayerRpcComponentHandler<BagComponentAgent, ReqRemoveItem, RespRemoveItem>
{
    protected override async Task ActionAsync(ReqRemoveItem request, RespRemoveItem rsponse, CancellationToken cancellationToken)
    {
        try
        {
            await ComponentAgent.OnRemoveBagItemAsync(NetworkChannel, request, rsponse);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            rsponse.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
