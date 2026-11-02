using GameFrameX.Apps.Player.Mail.Entity;

namespace GameFrameX.Hotfix.Logic.Player.Mail;

/// <summary>
/// 邮件 Campaign 命中筛选（U1 §4.5 <c>MatchFilter</c>）。
/// </summary>
/// <remarks>
/// 命中规则：<c>ServerIds</c> 空 or 含当前服 <c>Setting.ServerId</c>；<c>MinLevel/MaxLevel</c> 为 0 时表示不限。
/// 渠道（<c>ChannelIds</c>）命中由调用方在调用本筛选前单独判定（未补齐前不实例化也不记 key）。
/// </remarks>
internal static class MailCampaignFilter
{
    /// <summary>
    /// 判断 Campaign 是否命中当前玩家。任一非空条件不满足即返回 false。
    /// </summary>
    /// <param name="campaign">Campaign 不可变快照。</param>
    /// <param name="serverId">当前服 <c>Setting.ServerId</c>（业务 ServerInstanceId）。</param>
    /// <param name="playerLevel">玩家等级。</param>
    /// <param name="playerCreatedTime">玩家创建时间（unix 秒，<c>CacheState.CreatedTime</c>）。</param>
    /// <returns>命中返回 true；否则 false。</returns>
    public static bool Match(MailCampaignState campaign, int serverId, long playerLevel, long playerCreatedTime)
    {
        if (campaign.ServerIds != null && campaign.ServerIds.Count > 0 && !campaign.ServerIds.Contains(serverId))
        {
            return false;
        }

        if (campaign.MinLevel > 0 && playerLevel < campaign.MinLevel)
        {
            return false;
        }

        if (campaign.MaxLevel > 0 && playerLevel > campaign.MaxLevel)
        {
            return false;
        }

        return true;
    }
}
