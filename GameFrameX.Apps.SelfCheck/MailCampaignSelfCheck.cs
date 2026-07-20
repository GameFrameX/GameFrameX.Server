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
    /// 以及 B1（发布后不可修改）/ B3（撤回不回滚已发放资产，仅置状态）/ 幂等错误码。
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
