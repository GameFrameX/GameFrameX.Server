using System.Collections.Generic;

namespace GameFrameX.Apps.Player.Mail.Entity
{
    /// <summary>
    /// 运营邮件附件元信息（Campaign 侧）。描述一个附件槽位的类型、物品、数量，
    /// 实际发放走统一奖励发放接口（<c>RewardGrantComponentAgent.GrantAsync</c>），
    /// 玩家领取时再生成具体 <c>RewardItem</c> 并写入幂等账本。
    /// </summary>
    public sealed class MailAttachmentState
    {
        /// <summary>
        /// 附件槽位 ID，Campaign 内唯一。用于领取幂等与展示排序。
        /// </summary>
        public int SlotId { get; set; }

        /// <summary>
        /// 奖励类型（普通道具 / 隐藏道具 / 月卡 / VIP 点 等）。一期仅 <c>NormalItem</c> 可发放，其它返回 <c>Unsupported</c>。
        /// </summary>
        public int RewardType { get; set; }

        /// <summary>
        /// 物品 ID（普通道具对应 <c>TbItemConfig</c> 配置 ID）。
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// 数量。必须大于 0，否则发布校验返回 <c>InvalidReward</c>。
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        /// 展示用图标 ID（可选，缺省时由客户端按 ItemId 兜底）。
        /// </summary>
        public int IconId { get; set; }
    }
}
