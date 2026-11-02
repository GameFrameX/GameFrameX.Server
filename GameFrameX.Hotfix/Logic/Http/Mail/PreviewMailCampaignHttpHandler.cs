using System;
using System.Collections.Generic;
using GameFrameX.Apps.Player.Mail;
using GameFrameX.Apps.Player.Mail.Entity;
using GameFrameX.Hotfix.Logic.Player.Mail;

namespace GameFrameX.Hotfix.Logic.Http.Mail
{
    /// <summary>
    /// Admin 预览 / 校验 Campaign 参数（不落库）。
    /// 路由：POST /api/mailCampaign/preview。
    /// 用于发布前确认多语言文案、附件、过滤条件是否合法，并返回预估命中规模。
    /// </summary>
    [HttpMessageMapping(typeof(PreviewMailCampaignHttpHandler), typeof(PreviewMailCampaignRequest), typeof(PreviewMailCampaignResponse))]
    [RequireHttpSignature]
    [Description("Admin 预览 / 校验运营邮件 Campaign")]
    public sealed class PreviewMailCampaignHttpHandler : BaseHttpHandler
    {
        /// <summary>
        /// 处理预览 / 校验运营邮件 Campaign 参数的 HTTP 请求（不落库）。构造 Campaign 状态并校验，校验通过后执行预览并返回预估命中规模；校验失败或参数非法（ArgumentException）时返回对应错误码的 JSON 响应。
        /// </summary>
        /// <remarks>
        /// Handles the HTTP request for previewing / validating a mail campaign (without persistence). Builds the campaign state and validates it, then runs the preview and returns the estimated hit count; returns a JSON response with the corresponding error code when validation fails or a parameter is invalid (ArgumentException).
        /// </remarks>
        /// <param name="context">HTTP 处理管线统一请求载荷 / Unified request payload for the HTTP handling pipeline</param>
        /// <returns>处理结果的 JSON 字符串 / JSON string of the processing result</returns>
        public override async Task<string> ActionAsync(HttpActionContext context)
        {
            var previewRequest = (PreviewMailCampaignRequest)context.Request;
            var response = new PreviewMailCampaignResponse();

            var state = new MailCampaignState
            {
                MailType = previewRequest.MailType,
                MinLevel = previewRequest.MinLevel,
                MaxLevel = previewRequest.MaxLevel,
                Titles = previewRequest.Titles ?? new List<MailLocalizedContent>(),
                Contents = previewRequest.Contents ?? new List<MailLocalizedContent>(),
                Attachments = previewRequest.Attachments ?? new List<MailAttachmentState>(),
                ServerIds = previewRequest.ServerIds ?? new List<int>(),
                ChannelIds = previewRequest.ChannelIds ?? new List<int>(),
                ExpireAttachmentPolicy = previewRequest.ExpireAttachmentPolicy,
                ExpireAt = previewRequest.ExpireAt,
            };

            var code = MailCampaignRegistry.Validate(state);
            if (code != MailCampaignErrorCode.Ok)
            {
                response.Code = code;
                response.Message = "参数校验失败";
                return HttpJsonResultData<string>.SuccessString(JsonHelper.Serialize(response));
            }

            try
            {
                MailCampaignRegistry.Preview(state, out var estimatedHitCount);
                response.Code = MailCampaignErrorCode.Ok;
                response.EstimatedHitCount = estimatedHitCount;
                response.Message = "参数校验通过";
                return HttpJsonResultData<string>.SuccessString(JsonHelper.Serialize(response));
            }
            catch (ArgumentException ex)
            {
                response.Code = MailCampaignErrorCode.InvalidCampaignParameter;
                response.Message = ex.Message;
                return HttpJsonResultData<string>.SuccessString(JsonHelper.Serialize(response));
            }
        }
    }
}
