using System;
using GameFrameX.Apps.Player.Mail;
using GameFrameX.Hotfix.Logic.Player.Mail;

namespace GameFrameX.Hotfix.Logic.Http.Mail
{
    /// <summary>
    /// Admin 撤回运营邮件 Campaign。
    /// 路由：POST /api/mailCampaign/revoke。
    /// 仅置 <see cref="MailCampaignState.Status"/> = Revoked 与时间戳（B3：撤回不回滚已发放资产）。
    /// </summary>
    [HttpMessageMapping(typeof(RevokeMailCampaignHttpHandler), typeof(RevokeMailCampaignRequest), typeof(RevokeMailCampaignResponse))]
    [RequireHttpSignature]
    [Description("Admin 撤回运营邮件 Campaign")]
    public sealed class RevokeMailCampaignHttpHandler : BaseHttpHandler
    {
        /// <summary>
        /// 处理撤回运营邮件 Campaign 的 HTTP 请求。调用注册表撤回将状态置为 Revoked 并返回撤回时间（B3：已发放资产不回滚）；Campaign 不存在、已撤回不可重复撤回或其他失败时返回对应错误码的 JSON 响应。
        /// </summary>
        /// <remarks>
        /// Handles the HTTP request for revoking a mail campaign. Invokes the registry revoke to mark the campaign as Revoked and returns the revoked time (B3: granted assets are not rolled back); returns a JSON response with the corresponding error code when the campaign is missing, already revoked, or fails otherwise.
        /// </remarks>
        /// <param name="context">HTTP 处理管线统一请求载荷 / Unified request payload for the HTTP handling pipeline</param>
        /// <returns>处理结果的 JSON 字符串 / JSON string of the processing result</returns>
        public override async Task<string> ActionAsync(HttpActionContext context)
        {
            var revokeRequest = (RevokeMailCampaignRequest)context.Request;
            var response = new RevokeMailCampaignResponse { CampaignId = revokeRequest.CampaignId };

            var code = MailCampaignRegistry.Revoke(revokeRequest.CampaignId, revokeRequest.Operator, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            response.Code = code;
            switch (code)
            {
                case MailCampaignErrorCode.Ok:
                    response.Message = "撤回成功（已发放资产不回滚 B3）";
                    if (MailCampaignRegistry.TryQuery(revokeRequest.CampaignId, out var campaign))
                    {
                        response.RevokedAt = campaign.RevokedAt;
                    }

                    break;
                case MailCampaignErrorCode.CampaignNotFound:
                    response.Message = "Campaign 不存在";
                    break;
                case MailCampaignErrorCode.CampaignAlreadyRevoked:
                    response.Message = "Campaign 已撤回，不可重复撤回";
                    break;
                default:
                    response.Message = "撤回失败";
                    break;
            }

            return HttpJsonResultData<string>.SuccessString(JsonHelper.Serialize(response));
        }
    }
}
