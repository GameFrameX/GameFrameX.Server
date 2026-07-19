# 玩家邮件系统编排

## 目的

`GFX-135` 是 `U1-player-mail-system` 的 Phase 0 umbrella 编排任务，只固定玩家邮件系统的跨仓库执行顺序和不可抢跑边界，不实现邮件状态、协议、HTTP API 或 Admin 页面。

权威设计入口是外部 gfx-kernel 文档：

- `gfx-doc/dev/apps/changes/U1-player-mail-system/change.md`
- `gfx-doc/dev/apps/changes/U1-player-mail-system/follow-up-reward-grant-contract.md`

当前仓库的 `gfx-doc` 目录被 `.gitignore` 忽略，当前 feature worktree 不提交该目录。本文档只作为 Server 代码仓内可提交的编排锚点。

## 源码锚点

- 业务 `ServerInstanceId` 对应当前启动参数 `--ServerId` / `Setting.ServerId`，后续 Server API 和 Admin 游戏服务器唯一 ID 必须绑定到同一语义。
- 当前已有后台直接给玩家发道具路径和背包状态能力，邮件附件领取不得直接写背包，必须经由统一奖励发放接口。
- 玩家邮件属于玩家维度业务状态，后续状态组件应延续 `StateComponent<TState>` / `StateComponentAgent<TComponent, TState>` 分层。

## 执行顺序

| Phase | Linear | Project | Status at 2026-07-19 | Boundary |
|---:|---|---|---|---|
| 0 | `GFX-135` | server | In Progress | 总览与跨仓库依赖编排，不写功能代码。 |
| 1 | `GFX-136` | server | Backlog | 复审邮件协议、状态机和懒创建边界，不绕过此结论实现后续代码。 |
| 2 | `GFX-137` | server | Backlog | 实现统一奖励发放接口一期，先支持普通道具并保留隐藏资产扩展位。 |
| 2 | `GFX-138` | server | Backlog | 实现玩家邮件核心状态与懒创建，依赖 Phase 1 结论。 |
| 2 | `GFX-140` | server | Backlog | 提供 Admin 邮件发布 HTTP API，依赖 Phase 1 结论。 |
| 3 | `GFX-139` | server | Backlog | 实现附件领取、幂等和删除保护，依赖奖励发放接口和邮件核心状态。 |
| 4 | `GFX-141` | admin-api | Todo | 实现邮件模板、发布记录和 Server 发布调用，必须遵守 Server API/Proto 契约。 |
| 4 | `GFX-142` | admin-vue | Todo | 实现模板编辑、筛选和发布确认，不自行发明后端字段。 |
| 5 | `GFX-143` | admin-vue | Backlog | 实现发布记录、撤回和状态查看，依赖 Admin API 发布记录契约。 |
| 6 | `GFX-144` | server | Backlog | 端到端联调：Admin 发布、Server 保存、玩家懒创建、附件领取、撤回和过期验证。 |

## 不可变约束

- 发布后不可修改；修正文案、附件或筛选条件必须创建新发布版本。
- 未领取或未领取完附件的邮件禁止删除。
- 撤回不回滚已领取资产；未实例化邮件不再创建，已实例化未领取邮件按规则作废。
- 过期未领取附件默认作废，且必须保留可配置策略。
- Server API/Proto 是跨仓库契约源，Admin API 和 Admin Vue 只能对接已落地契约。
- 涉及状态、筛选命中、领取、撤回和过期策略的 C# 类型、属性、函数必须补中文 XML 注释，解释业务语义和不可逆边界。

## 验收边界

`GFX-135` 的完成条件是依赖链、执行顺序和不可抢跑规则在 Server 仓内可追踪；后续每个实现 issue 必须各自完成构建、测试、代码审查和 PR 关联。

