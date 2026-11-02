namespace GameFrameX.Hotfix.Logic.Player.Mail;

[MessageMapping(typeof(ReqMailDelete))]
internal sealed class ReqMailDeleteHandler : PlayerRpcComponentHandler<MailComponentAgent, ReqMailDelete, RespMailDelete>
{
    protected override async Task ActionAsync(ReqMailDelete request, RespMailDelete response, CancellationToken cancellationToken)
    {
        try
        {
            await ComponentAgent.OnReqMailDeleteAsync(NetworkChannel, request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
