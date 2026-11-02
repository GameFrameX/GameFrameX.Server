namespace GameFrameX.Hotfix.Logic.Player.Mail;

[MessageMapping(typeof(ReqMailList))]
internal sealed class ReqMailListHandler : PlayerRpcComponentHandler<MailComponentAgent, ReqMailList, RespMailList>
{
    protected override async Task ActionAsync(ReqMailList request, RespMailList response, CancellationToken cancellationToken)
    {
        try
        {
            await ComponentAgent.OnReqMailListAsync(NetworkChannel, request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
