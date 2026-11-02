namespace GameFrameX.Hotfix.Logic.Player.Mail;

[MessageMapping(typeof(ReqMailClaimAttachment))]
internal sealed class ReqMailClaimAttachmentHandler : PlayerRpcComponentHandler<MailComponentAgent, ReqMailClaimAttachment, RespMailClaimAttachment>
{
    protected override async Task ActionAsync(ReqMailClaimAttachment request, RespMailClaimAttachment response, CancellationToken cancellationToken)
    {
        try
        {
            await ComponentAgent.OnReqMailClaimAttachmentAsync(NetworkChannel, request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
