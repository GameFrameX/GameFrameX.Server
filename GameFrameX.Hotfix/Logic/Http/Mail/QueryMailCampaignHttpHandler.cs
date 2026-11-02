using System.Collections.Generic;
using GameFrameX.Apps.Player.Mail;
using GameFrameX.Apps.Player.Mail.Entity;
using GameFrameX.Hotfix.Logic.Player.Mail;

namespace GameFrameX.Hotfix.Logic.Http.Mail
{
    /// <summary>
    /// Admin 查询运营邮件 Campaign 发布状态。
    /// 路由：GET /api/mailCampaign/query。
    /// <see cref="QueryMailCampaignRequest.CampaignId"/> 大于 0 时按 ID 精确查询单条；否则按过滤条件列表查询。
    /// </summary>
    [HttpMessageMapping(typeof(QueryMailCampaignHttpHandler), typeof(QueryMailCampaignRequest), typeof(QueryMailCampaignResponse))]
    [RequireHttpSignature]
    [Description("Admin 查询运营邮件 Campaign 发布状态")]
    public sealed class QueryMailCampaignHttpHandler : BaseHttpHandler
    {
        /// <summary>
        /// 处理查询运营邮件 Campaign 发布状态的 HTTP 请求。CampaignId 大于 0 时按 ID 精确查询单条，不存在返回 CampaignNotFound；否则按状态、邮件类型、服务器、渠道、等级与创建时间等 AND 过滤条件列表查询并返回 Campaign 列表与总数。
        /// </summary>
        /// <remarks>
        /// Handles the HTTP request for querying mail campaign publish status. When CampaignId is greater than 0, queries a single campaign by id and returns CampaignNotFound when missing; otherwise lists campaigns with AND-combined filters (status, mail type, server, channel, level, creation time) and returns the campaign list with the total count.
        /// </remarks>
        /// <param name="context">HTTP 处理管线统一请求载荷 / Unified request payload for the HTTP handling pipeline</param>
        /// <returns>处理结果的 JSON 字符串 / JSON string of the processing result</returns>
        public override async Task<string> ActionAsync(HttpActionContext context)
        {
            var queryRequest = (QueryMailCampaignRequest)context.Request;
            var response = new QueryMailCampaignResponse { Code = MailCampaignErrorCode.Ok };

            if (queryRequest.CampaignId > 0)
            {
                if (MailCampaignRegistry.TryQuery(queryRequest.CampaignId, out var single))
                {
                    response.Campaigns.Add(single);
                    response.Total = 1;
                }
                else
                {
                    response.Code = MailCampaignErrorCode.CampaignNotFound;
                    response.Message = "Campaign 不存在";
                }

                return HttpJsonResultData<string>.SuccessString(JsonHelper.Serialize(response));
            }

            var list = MailCampaignRegistry.QueryAll(new MailCampaignQuery
            {
                Status = queryRequest.Status,
                MailType = queryRequest.MailType,
                ServerId = queryRequest.ServerId,
                ChannelId = queryRequest.ChannelId,
                MinLevel = queryRequest.MinLevel,
                CreatedFromUnixSeconds = queryRequest.CreatedFrom,
                CreatedToUnixSeconds = queryRequest.CreatedTo,
                Limit = queryRequest.Limit,
            });

            response.Campaigns = list;
            response.Total = list.Count;
            response.Message = "查询成功";
            return HttpJsonResultData<string>.SuccessString(JsonHelper.Serialize(response));
        }
    }
}
