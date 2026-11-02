namespace GameFrameX.Hotfix.Logic.Player.Bag;

/// <summary>
/// 请求使用物品
/// </summary>
[MessageMapping(typeof(ReqUseItem))]
internal sealed class ReqUseItemHandler : PlayerRpcComponentHandler<BagComponentAgent, ReqUseItem, RespUseItem>
{
    protected override async Task ActionAsync(ReqUseItem request, RespUseItem response, CancellationToken cancellationToken)
    {
        try
        {
            await ComponentAgent.OnUseBagItemAsync(NetworkChannel, request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
