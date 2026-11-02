namespace GameFrameX.Hotfix.Logic.Player.Mail;

[MessageMapping(typeof(ReqMailClaimAllAttachment))]
internal sealed class ReqMailClaimAllAttachmentHandler : PlayerRpcComponentHandler<MailComponentAgent, ReqMailClaimAllAttachment, RespMailClaimAllAttachment>
{
    protected override async Task ActionAsync(ReqMailClaimAllAttachment request, RespMailClaimAllAttachment response, CancellationToken cancellationToken)
    {
        try
        {
            await ComponentAgent.OnReqMailClaimAllAttachmentAsync(NetworkChannel, request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
