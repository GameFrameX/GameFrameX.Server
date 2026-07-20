// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using System;
using System.Collections.Generic;
using GameFrameX.Apps.Player.Mail;
using GameFrameX.Apps.Player.Mail.Entity;
using GameFrameX.Hotfix.Logic.Player.Mail;
using GameFrameX.Proto.Proto;

namespace GameFrameX.Apps.SelfCheck
{
    /// <summary>
    /// Admin 邮件发布 HTTP API 的最小自检：覆盖 <see cref="MailCampaignRegistry"/> 的校验、发布、撤回、查询路径，
    /// 以及 B1（发布后不可修改）/ B3（撤回不回滚已发放资产，仅置状态）/ B5（撤回后同 CampaignId 拒绝再发布，Campaign 侧兜底）/
    /// 端到端 smoke（发布 → 查询 → 撤回 → 查询）/ 幂等错误码。
    /// </summary>
    public static class MailCampaignSelfCheck
    {
        /// <summary>
        /// 执行自检，失败时抛出异常。
        /// </summary>
        public static void Run()
        {
            MailCampaignRegistry.ResetForTest();

            AssertValidate();
            AssertPublishAndB1Immutability();
            AssertRevokeAndB3NoRollback();
            AssertRevokeBlocksRepublish();
            AssertEndToEndFlow();
            AssertQuery();

            MailCampaignRegistry.ResetForTest();
        }

        private static void AssertValidate()
        {
            AssertEqual(OperationStatusCode.InvalidCampaignParameter, MailCampaignRegistry.Validate(null), "Validate(null)");

            var noTitle = new MailCampaignState { MailType = MailType.Operation };
            AssertEqual(OperationStatusCode.InvalidCampaignParameter, MailCampaignRegistry.Validate(noTitle), "Validate(no-title)");

            var blankTitle = new MailCampaignState
            {
                MailType = MailType.Operation,
                Titles = new List<MailLocalizedContent> { new MailLocalizedContent { Language = "", Text = "" } },
            };
            AssertEqual(OperationStatusCode.InvalidCampaignParameter, MailCampaignRegistry.Validate(blankTitle), "Validate(blank-title)");

            var badAttachment = new MailCampaignState
            {
                MailType = MailType.Operation,
                Titles = new List<MailLocalizedContent> { new MailLocalizedContent { Language = "zh-cn", Text = "title" } },
                Attachments = new List<MailAttachmentState> { new MailAttachmentState { SlotId = 1, Amount = 0 } },
            };
            AssertEqual(OperationStatusCode.InvalidReward, MailCampaignRegistry.Validate(badAttachment), "Validate(amount<=0)");

            var dupSlot = new MailCampaignState
            {
                MailType = MailType.Operation,
                Titles = new List<MailLocalizedContent> { new MailLocalizedContent { Language = "zh-cn", Text = "title" } },
                Attachments = new List<MailAttachmentState>
                {
                    new MailAttachmentState { SlotId = 1, Amount = 10 },
                    new MailAttachmentState { SlotId = 1, Amount = 20 },
                },
            };
            AssertEqual(OperationStatusCode.InvalidCampaignParameter, MailCampaignRegistry.Validate(dupSlot), "Validate(dup-slot)");

            var levelRangeBad = MakeValidCampaign();
            levelRangeBad.MinLevel = 50;
            levelRangeBad.MaxLevel = 10;
            AssertEqual(OperationStatusCode.InvalidCampaignParameter, MailCampaignRegistry.Validate(levelRangeBad), "Validate(min>max)");

            AssertEqual(OperationStatusCode.Ok, MailCampaignRegistry.Validate(MakeValidCampaign()), "Validate(valid)");
        }

        private static void AssertPublishAndB1Immutability()
        {
            var campaign = MakeValidCampaign();
            var published = MailCampaignRegistry.PublishOrUpdate(campaign, "admin-test", 1_700_000_000L);

            if (published.CampaignId <= 0)
            {
                throw new InvalidOperationException("PublishOrUpdate 应分配 CampaignId。");
            }

            AssertEqual(1, published.PublishVersion, "PublishOrUpdate.PublishVersion");
            AssertEqual(MailCampaignStatus.Published, published.Status, "PublishOrUpdate.Status");
            AssertEqual(1_700_000_000L, published.PublishedAt, "PublishOrUpdate.PublishedAt");
            AssertEqual("admin-test", published.PublishOperator, "PublishOrUpdate.PublishOperator");

            // B1：已发布的 Campaign 主体字段不可修改，重复提交同 CampaignId 应抛 InvalidOperationException。
            var republish = MakeValidCampaign();
            republish.CampaignId = published.CampaignId;
            try
            {
                MailCampaignRegistry.PublishOrUpdate(republish, "admin-test", 1_700_000_100L);
                throw new InvalidOperationException("B1 自检失败：重复发布已发布 Campaign 应抛异常。");
            }
            catch (InvalidOperationException)
            {
                // 预期
            }
        }

        private static void AssertRevokeAndB3NoRollback()
        {
            var campaign = MakeValidCampaign();
            var published = MailCampaignRegistry.PublishOrUpdate(campaign, "admin-test", 1_700_000_200L);

            AssertEqual(OperationStatusCode.CampaignNotFound, MailCampaignRegistry.Revoke(999_999L, "admin-test", 1_700_000_300L), "Revoke(unknown)");

            var code = MailCampaignRegistry.Revoke(published.CampaignId, "admin-test", 1_700_000_300L);
            AssertEqual(OperationStatusCode.Ok, code, "Revoke(ok)");

            if (!MailCampaignRegistry.TryQuery(published.CampaignId, out var revoked))
            {
                throw new InvalidOperationException("Revoke 后 TryQuery 失败。");
            }

            AssertEqual(MailCampaignStatus.Revoked, revoked.Status, "Revoke.Status");
            AssertEqual(1_700_000_300L, revoked.RevokedAt, "Revoke.RevokedAt");
            AssertEqual("admin-test", revoked.RevokeOperator, "Revoke.RevokeOperator");

            // B3：已发放资产不回滚——此处只校验状态字段，资产回滚由发放接口幂等账本保证（不在本 change 范围）。
            AssertEqual(OperationStatusCode.CampaignAlreadyRevoked, MailCampaignRegistry.Revoke(published.CampaignId, "admin-test", 1_700_000_400L), "Revoke(duplicate)");
        }

        private static void AssertQuery()
        {
            var c1 = MakeValidCampaign();
            c1.MailType = MailType.Operation;
            c1.MinLevel = 10;
            c1.MaxLevel = 50;
            c1.ServerIds = new List<int> { 100 };
            var p1 = MailCampaignRegistry.PublishOrUpdate(c1, "admin-test", 1_700_000_500L);

            var c2 = MakeValidCampaign();
            c2.MailType = MailType.Compensation;
            var p2 = MailCampaignRegistry.PublishOrUpdate(c2, "admin-test", 1_700_000_600L);

            var all = MailCampaignRegistry.QueryAll();
            if (all.Count < 2)
            {
                throw new InvalidOperationException("QueryAll 应返回至少 2 条，实际=" + all.Count + "。");
            }

            var filtered = MailCampaignRegistry.QueryAll(mailType: MailType.Operation);
            if (filtered.Count == 0 || filtered[0].MailType != MailType.Operation)
            {
                throw new InvalidOperationException("QueryAll(mailType) 过滤失败。");
            }

            var byServer = MailCampaignRegistry.QueryAll(serverId: 100);
            if (byServer.Count == 0 || byServer.Exists(c => !c.ServerIds.Contains(100)))
            {
                throw new InvalidOperationException("QueryAll(serverId) 过滤失败。");
            }

            if (!MailCampaignRegistry.TryQuery(p1.CampaignId, out _))
            {
                throw new InvalidOperationException("TryQuery 已发布 Campaign 失败。");
            }

            if (MailCampaignRegistry.TryQuery(p2.CampaignId + 10_000L, out _))
            {
                throw new InvalidOperationException("TryQuery 不存在的 CampaignId 应返回 false。");
            }
        }

        // B5（Campaign 侧兜底）：撤回后的 Campaign，再次以同 CampaignId 调用 PublishOrUpdate 应抛 InvalidOperationException，
        // 呼应 U1 §4.5「撤回 Campaign 防重实例化」与 §4.6「未实例化的 Campaign：撤回仅置 Campaign 状态，懒创建算法的防重实例化分支兜底」。
        private static void AssertRevokeBlocksRepublish()
        {
            var campaign = MakeValidCampaign();
            var published = MailCampaignRegistry.PublishOrUpdate(campaign, "admin-test", 1_700_010_000L);

            AssertEqual(OperationStatusCode.Ok, MailCampaignRegistry.Revoke(published.CampaignId, "admin-test", 1_700_010_100L), "Revoke(before-republish)");

            // 撤回后同 CampaignId 再发布：Registry 拒绝（已撤回视为终态，主体字段不可再修改）。
            var republish = MakeValidCampaign();
            republish.CampaignId = published.CampaignId;
            try
            {
                MailCampaignRegistry.PublishOrUpdate(republish, "admin-test", 1_700_010_200L);
                throw new InvalidOperationException("B5 自检失败：撤回后同 CampaignId 再次发布应抛异常。");
            }
            catch (InvalidOperationException)
            {
                // 预期：撤回后同 CampaignId 不可再发布，避免撤回 Campaign 被重实例化。
            }

            // 撤回状态应保持不变（不被再发布覆盖）。
            if (!MailCampaignRegistry.TryQuery(published.CampaignId, out var afterAttempt))
            {
                throw new InvalidOperationException("B5 自检失败：再发布尝试后 TryQuery 失败。");
            }
            AssertEqual(MailCampaignStatus.Revoked, afterAttempt.Status, "Revoke-blocks-republish.Status");
            AssertEqual(1_700_010_100L, afterAttempt.RevokedAt, "Revoke-blocks-republish.RevokedAt");
        }

        // 端到端 smoke（Campaign 侧）：Admin 发布 → Server 保存发布记录 → TryQuery 命中 Published →
        // 撤回 → TryQuery 命中 Revoked（含 RevokedAt / RevokeOperator）→ QueryAll(status=Revoked) 过滤命中。
        // 玩家懒创建 / 附件领取 / B2 / B6 由 #74 / C10 落地后补，本 smoke 只覆盖 Campaign 侧子流程。
        private static void AssertEndToEndFlow()
        {
            var campaign = MakeValidCampaign();
            campaign.MailType = MailType.Operation;
            campaign.ServerIds = new List<int> { 200 };
            campaign.MinLevel = 20;
            campaign.MaxLevel = 80;

            // 1. 发布 → Server 保存不可变快照（B1）。
            var published = MailCampaignRegistry.PublishOrUpdate(campaign, "admin-flow", 1_700_020_000L);
            if (published.CampaignId <= 0)
            {
                throw new InvalidOperationException("E2E 自检失败：PublishOrUpdate 应分配 CampaignId。");
            }
            AssertEqual(MailCampaignStatus.Published, published.Status, "E2E.Publish.Status");
            AssertEqual("admin-flow", published.PublishOperator, "E2E.Publish.Operator");

            // 2. TryQuery 命中 Published。
            if (!MailCampaignRegistry.TryQuery(published.CampaignId, out var q1))
            {
                throw new InvalidOperationException("E2E 自检失败：发布后 TryQuery 失败。");
            }
            AssertEqual(MailCampaignStatus.Published, q1.Status, "E2E.QueryPublished.Status");
            AssertEqual(0L, q1.RevokedAt, "E2E.QueryPublished.RevokedAt");

            // 3. 撤回（B3：仅置状态字段，不回滚已发放资产——资产回滚由统一发放接口幂等账本保证）。
            AssertEqual(OperationStatusCode.Ok, MailCampaignRegistry.Revoke(published.CampaignId, "admin-revoke", 1_700_020_500L), "E2E.Revoke");

            // 4. TryQuery 命中 Revoked + RevokedAt + RevokeOperator。
            if (!MailCampaignRegistry.TryQuery(published.CampaignId, out var q2))
            {
                throw new InvalidOperationException("E2E 自检失败：撤回后 TryQuery 失败。");
            }
            AssertEqual(MailCampaignStatus.Revoked, q2.Status, "E2E.QueryRevoked.Status");
            AssertEqual(1_700_020_500L, q2.RevokedAt, "E2E.QueryRevoked.RevokedAt");
            AssertEqual("admin-revoke", q2.RevokeOperator, "E2E.QueryRevoked.RevokeOperator");

            // 5. QueryAll(status=Revoked) 过滤命中；重复撤回返回 CampaignAlreadyRevoked。
            var revokedList = MailCampaignRegistry.QueryAll(status: MailCampaignStatus.Revoked);
            if (revokedList.Find(c => c.CampaignId == published.CampaignId) == null)
            {
                throw new InvalidOperationException("E2E 自检失败：QueryAll(status=Revoked) 未命中已撤回 Campaign。");
            }
            AssertEqual(OperationStatusCode.CampaignAlreadyRevoked, MailCampaignRegistry.Revoke(published.CampaignId, "admin-revoke", 1_700_020_600L), "E2E.Revoke(duplicate)");
        }

        private static MailCampaignState MakeValidCampaign()
        {
            return new MailCampaignState
            {
                MailType = MailType.Operation,
                Titles = new List<MailLocalizedContent>
                {
                    new MailLocalizedContent { Language = "zh-cn", Text = "测试标题" },
                    new MailLocalizedContent { Language = "en-us", Text = "Test Title" },
                },
                Contents = new List<MailLocalizedContent>
                {
                    new MailLocalizedContent { Language = "zh-cn", Text = "测试正文" },
                },
                Attachments = new List<MailAttachmentState>
                {
                    new MailAttachmentState { SlotId = 1, RewardType = 0, ItemId = 1001, Amount = 5 },
                },
                ExpireAttachmentPolicy = ExpireAttachmentPolicy.DiscardUnclaimed,
                ExpireAt = 1_700_999_999L,
            };
        }

        private static void AssertEqual<T>(T expected, T actual, string name)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new InvalidOperationException(name + " 自检失败，expected=" + expected + ", actual=" + actual + "。");
            }
        }
    }
}
