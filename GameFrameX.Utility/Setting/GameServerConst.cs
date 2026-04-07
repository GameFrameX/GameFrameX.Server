// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
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

namespace GameFrameX.Utility.Setting;

/// <summary>
/// 游戏服务器常量定义（名称 + 服务器ID）。
/// </summary>
/// <remarks>
/// Centralized service constants binding Name and Id together.
/// </remarks>
public static class GameServerConst
{
    /// <summary>
    /// 玩家主服务
    /// </summary>
    public static class Game
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Game";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 1000;
    }

    /// <summary>
    /// 网关服务
    /// </summary>
    public static class Gateway
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Gateway";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5000;
    }

    /// <summary>
    /// 即时通信服务
    /// </summary>
    public static class Im
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Im";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5500;
    }

    /// <summary>
    /// 好友服务
    /// </summary>
    public static class Friend
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Friend";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5540;
    }

    /// <summary>
    /// 鉴权服务
    /// </summary>
    public static class Auth
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Auth";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5040;
    }

    /// <summary>
    /// 社交服务
    /// </summary>
    public static class Social
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Social";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5530;
    }

    /// <summary>
    /// 房间服务
    /// </summary>
    public static class Room
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Room";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 7010;
    }

    /// <summary>
    /// 登录服务
    /// </summary>
    public static class Login
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Login";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5030;
    }

    /// <summary>
    /// 聊天服务
    /// </summary>
    public static class Chat
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Chat";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5510;
    }

    /// <summary>
    /// 公会服务
    /// </summary>
    public static class Guild
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Guild";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5570;
    }

    /// <summary>
    /// 邮件服务
    /// </summary>
    public static class Mail
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Mail";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5590;
    }

    /// <summary>
    /// 排行榜服务
    /// </summary>
    public static class Rank
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Rank";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 6050;
    }

    /// <summary>
    /// 匹配服务
    /// </summary>
    public static class Match
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Match";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 7000;
    }

    /// <summary>
    /// 场景服务
    /// </summary>
    public static class Scene
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Scene";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 7020;
    }

    /// <summary>
    /// 世界服务
    /// </summary>
    public static class World
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "World";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 7030;
    }

    /// <summary>
    /// 战斗服务
    /// </summary>
    public static class Battle
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Battle";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 7040;
    }

    /// <summary>
    /// 组队服务
    /// </summary>
    public static class Team
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Team";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5550;
    }

    /// <summary>
    /// 队伍服务
    /// </summary>
    public static class Party
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Party";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5560;
    }

    /// <summary>
    /// 联赛服务
    /// </summary>
    public static class League
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "League";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 6060;
    }

    /// <summary>
    /// 战盟服务
    /// </summary>
    public static class Clan
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Clan";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5580;
    }

    /// <summary>
    /// 交易服务
    /// </summary>
    public static class Trade
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Trade";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 7610;
    }

    /// <summary>
    /// 拍卖服务
    /// </summary>
    public static class Auction
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Auction";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 7620;
    }

    /// <summary>
    /// 背包服务
    /// </summary>
    public static class Inventory
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Inventory";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 7600;
    }

    /// <summary>
    /// 任务服务
    /// </summary>
    public static class Quest
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Quest";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 6010;
    }

    /// <summary>
    /// 成就服务
    /// </summary>
    public static class Achievement
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Achievement";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 6020;
    }

    /// <summary>
    /// 活动服务
    /// </summary>
    public static class Activity
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Activity";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 6030;
    }

    /// <summary>
    /// 运营事件服务
    /// </summary>
    public static class Event
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Event";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 6040;
    }

    /// <summary>
    /// 支付服务
    /// </summary>
    public static class Payment
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Payment";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 6530;
    }

    /// <summary>
    /// 订单服务
    /// </summary>
    public static class Order
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Order";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 6520;
    }

    /// <summary>
    /// 充值服务
    /// </summary>
    public static class Recharge
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Recharge";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 6540;
    }

    /// <summary>
    /// 反作弊服务
    /// </summary>
    public static class AntiCheat
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "AntiCheat";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 8030;
    }

    /// <summary>
    /// 内容审核服务
    /// </summary>
    public static class Moderation
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Moderation";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 8020;
    }

    /// <summary>
    /// GM 服务
    /// </summary>
    public static class GM
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "GM";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 8000;
    }

    /// <summary>
    /// 公告服务
    /// </summary>
    public static class Notice
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Notice";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5600;
    }

    /// <summary>
    /// 推送服务
    /// </summary>
    public static class Push
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Push";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5610;
    }

    /// <summary>
    /// 分析服务
    /// </summary>
    public static class Analytics
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Analytics";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 8500;
    }

    /// <summary>
    /// 数仓服务
    /// </summary>
    public static class DataWarehouse
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "DataWarehouse";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 8510;
    }

    /// <summary>
    /// 日志服务
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Log";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 8520;
    }

    /// <summary>
    /// 监控服务
    /// </summary>
    public static class Monitor
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Monitor";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 8530;
    }

    /// <summary>
    /// 指标服务
    /// </summary>
    public static class Metrics
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Metrics";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 8540;
    }

    /// <summary>
    /// 配置服务
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Config";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5070;
    }

    /// <summary>
    /// 开关服务
    /// </summary>
    public static class FeatureFlag
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "FeatureFlag";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5080;
    }

    /// <summary>
    /// AI 网关服务
    /// </summary>
    public static class AIGateway
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "AIGateway";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 7090;
    }

    /// <summary>
    /// AI 匹配服务
    /// </summary>
    public static class AIMatch
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "AIMatch";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 7100;
    }

    /// <summary>
    /// 回放服务
    /// </summary>
    public static class Replay
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Replay";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 7060;
    }

    /// <summary>
    /// 观战服务
    /// </summary>
    public static class Spectator
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Spectator";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 7050;
    }

    /// <summary>
    /// 锦标赛服务
    /// </summary>
    public static class Tournament
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Tournament";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 7070;
    }

    /// <summary>
    /// 跨服协调服务
    /// </summary>
    public static class CrossServer
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "CrossServer";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 7080;
    }

    /// <summary>
    /// 代理服务
    /// </summary>
    public static class Proxy
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Proxy";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5010;
    }

    /// <summary>
    /// 中继服务
    /// </summary>
    public static class Relay
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Relay";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5020;
    }

    /// <summary>
    /// 语音服务
    /// </summary>
    public static class Voice
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Voice";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5520;
    }

    /// <summary>
    /// 运营工具服务
    /// </summary>
    public static class LiveOps
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "LiveOps";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 8010;
    }

    /// <summary>
    /// 热更补丁服务
    /// </summary>
    public static class Patch
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Patch";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 6560;
    }

    /// <summary>
    /// 资源分发服务
    /// </summary>
    public static class Resource
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Resource";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 6550;
    }

    /// <summary>
    /// 本地化服务
    /// </summary>
    public static class Localization
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Localization";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 8550;
    }

    /// <summary>
    /// SDK 聚合服务
    /// </summary>
    public static class Sdk
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Sdk";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5060;
    }

    /// <summary>
    /// 账号服务
    /// </summary>
    public static class Account
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Account";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5050;
    }

    /// <summary>
    /// 角色服务
    /// </summary>
    public static class Character
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Name = "Character";

        /// <summary>
        /// 服务ID
        /// </summary>
        public const int Id = 5620;
    }
}
