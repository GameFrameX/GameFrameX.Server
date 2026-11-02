namespace GameFrameX.Hotfix.Logic.Player.Mail;

[MessageMapping(typeof(ReqMailRead))]
internal sealed class ReqMailReadHandler : PlayerRpcComponentHandler<MailComponentAgent, ReqMailRead, RespMailRead>
{
    protected override async Task ActionAsync(ReqMailRead request, RespMailRead response, CancellationToken cancellationToken)
    {
        try
        {
            await ComponentAgent.OnReqMailReadAsync(NetworkChannel, request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
