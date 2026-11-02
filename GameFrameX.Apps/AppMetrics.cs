// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 Apache License 2.0 单协议分发，
//   This project is licensed solely under the Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace GameFrameX.Apps;

/// <summary>
/// Apps 程序集业务指标统一门面（System.Diagnostics.Metrics）。
/// Meter 名须与 ServiceDefaults 的 AddMeter 白名单条目保持一致，指标经 OTel Prometheus 导出器由 /metrics 端点对外暴露。
/// </summary>
/// <remarks>
/// Unified business metrics facade for the Apps assembly (System.Diagnostics.Metrics).
/// The meter name must match the AddMeter whitelist entry in ServiceDefaults; instruments are exported
/// by the OTel Prometheus exporter through the /metrics endpoint.
/// </remarks>
public static class AppMetrics
{
    /// <summary>Apps 业务指标 Meter 名称 / Apps business metrics meter name.</summary>
    public const string MeterName = "GameFrameX.Apps";

    private static readonly Meter Meter = new(MeterName, "1.0.0");

    /// <summary>
    /// 玩家角色登录次数：玩家角色通过角色 ID 登录成功路径（OnPlayerLogin）计数。
    /// </summary>
    /// <remarks>
    /// Player role login count: recorded when a player role completes the role-id login path (OnPlayerLogin).
    /// </remarks>
    public static readonly Counter<long> PlayerLogin = Meter.CreateCounter<long>("player_login", description: "玩家角色登录 / Player role logins");

    /// <summary>
    /// 账号登录次数：账号密码校验请求进入登录路径时计数（含登录失败后转注册的请求）。
    /// </summary>
    /// <remarks>
    /// Account login count: recorded when a login request enters the credential check path,
    /// including requests that fall through to registration.
    /// </remarks>
    public static readonly Counter<long> AccountLogin = Meter.CreateCounter<long>("account_login", description: "账号登录次数 / Account logins");

    /// <summary>
    /// 账号注册次数：登录时账号不存在而创建新账号（LoginState 落库）时计数。
    /// </summary>
    /// <remarks>
    /// Account registration count: recorded when a login creates a new account (LoginState persisted)
    /// because the credentials did not exist.
    /// </remarks>
    public static readonly Counter<long> AccountRegister = Meter.CreateCounter<long>("account_register", description: "账号注册次数 / Account registrations");

    /// <summary>
    /// 玩家角色创建次数：新玩家角色（PlayerState）保存入库前计数。
    /// </summary>
    /// <remarks>
    /// Player role creation count: recorded before a newly created player role (PlayerState) is persisted.
    /// </remarks>
    public static readonly Counter<long> PlayerCreate = Meter.CreateCounter<long>("player_create", description: "玩家角色创建数量 / Player roles created");

    /// <summary>
    /// 玩家角色列表查询次数：按账号 ID 查询角色列表的数据库查询发起时计数。
    /// </summary>
    /// <remarks>
    /// Player role list query count: recorded when the role-list query for an account id is issued.
    /// </remarks>
    public static readonly Counter<long> PlayerListQuery = Meter.CreateCounter<long>("player_list_query", description: "获取玩家列表 / Player role list queries");
}
