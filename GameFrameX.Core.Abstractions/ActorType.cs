namespace GameFrameX.Core.Abstractions;

/// <summary>
/// 每个服存在多个实例的（如玩家和公会）需要小于Separator
/// 最大id应当小于999
/// Id一旦定义了不应该修改
/// </summary>
public enum ActorType
{
    /// <summary>
    /// 空将会被判断为无效值
    /// </summary>
    None,

    /// <summary>
    /// 角色
    /// 管理玩家角色 
    /// </summary>
    Player,

    /*
    /// <summary>
    /// 路由
    /// 提供对外的网络通信服务
    /// </summary>
    Router,

    /// <summary>
    /// 服务发现中心
    /// 用于发现其他服务器
    /// </summary>
    DiscoveryCenter,
    */

    /// <summary>
    /// 网关
    /// 作为客户端和游戏服务器之间的中介，处理网络通信。
    /// </summary>
    Gate,

    /// <summary>
    /// 聊天
    /// 管理玩家间的聊天和社交功能。
    /// </summary>
    Chat,

    /// <summary>
    /// 世界
    /// 管理游戏世界的状态，包括地图信息、NPC、环境等。
    /// </summary>
    World,

    /// <summary>
    /// 任务
    /// 管理游戏内任务和任务进度。
    /// </summary>
    Task,

    /// <summary>
    /// 逻辑
    /// 负责处理游戏规则、玩家行为、AI等逻辑。
    /// </summary>
    Logic,

    /// <summary>
    /// 全局
    /// </summary>
    Global,

    /// <summary>
    /// 数据库
    /// 存储和管理游戏数据，如玩家信息、物品数据等。
    /// </summary>
    DataBase,

    /// <summary>
    /// 缓存
    /// 提供数据缓存服务，提高访问速度和减轻数据库压力。
    /// </summary>
    Cache,

    /// <summary>
    /// 匹配
    /// 负责玩家匹配，组队和创建比赛。
    /// </summary>
    Match,

    /// <summary>
    /// 场景
    /// 负责具体的游戏场景，处理场景内的玩家和对象。
    /// </summary>
    Scene,

    /// <summary>
    /// 交易
    /// 处理游戏内交易。
    /// </summary>
    Trade,

    /// <summary>
    /// 支付
    /// 处理游戏内购买和交易。
    /// </summary>
    Payment,

    /// <summary>
    /// 备份
    /// 负责数据的定期备份和灾难恢复。
    /// </summary>
    Backup,

    /// <summary>
    /// 资源
    /// 管理和分发游戏资源，如地图、纹理、音效等。
    /// </summary>
    Resource,

    /// <summary>
    /// 监控
    /// 监控游戏服务器的状态和性能。
    /// </summary>
    Monitoring,

    /// <summary>
    /// 更新
    /// 负责游戏内容的更新和补丁分发。
    /// </summary>
    Update,

    /// <summary>
    /// 认证
    /// 负责用户身份验证和授权。
    /// </summary>
    Authentication,

    /// <summary>
    /// 排行榜
    /// 管理玩家的排名和排行榜数据。
    /// </summary>
    Ranking,

    /// <summary>
    /// 团队
    /// 管理玩家的队伍信息。
    /// </summary>
    Team,

    /// <summary>
    /// 语音
    /// 处理游戏内语音聊天。
    /// </summary>
    Voice,

    /// <summary>
    /// 社交
    /// 管理玩家的社交网络和好友系统。
    /// </summary>
    Social,

    /// <summary>
    /// 成就
    /// </summary>
    Achievement,

    /// <summary>
    /// 邮件
    /// 处理游戏内邮件系统，发送通知和奖励。
    /// </summary>
    Email,

    /// <summary>
    /// 广告
    /// 管理游戏内广告展示和广告投放和验证
    /// </summary>
    Advertisement,

    /// <summary>
    /// 分析
    /// 收集和分析玩家行为数据，用于优化游戏。
    /// </summary>
    Analytics,

    /// <summary>
    /// 推荐
    /// 根据玩家行为推荐游戏内容或商品。
    /// </summary>
    Recommendation,

    /// <summary>
    /// 日志
    /// 收集和存储游戏日志，用于问题诊断和数据分析。
    /// </summary>
    Log,

    /// <summary>
    /// 配置
    /// 管理游戏配置和设置，允许动态调整。
    /// </summary>
    Configuration,

    /// <summary>
    /// 安全
    /// 检测和防御游戏中的安全漏洞
    /// 负责游戏安全，如反作弊和防止非法行为。
    /// </summary>
    Security,

    /// <summary>
    /// 测试
    /// 用于游戏测试和调试，可能包含新功能或修复。
    /// </summary>
    Test,

    /// <summary>
    /// 存档
    /// 存储旧版本数据或历史数据，用于数据恢复。
    /// </summary>
    Archive,

    /// <summary>
    /// 市场
    /// 管理游戏内市场和虚拟商品的销售。
    /// </summary>
    Marketplace,

    /// <summary>
    /// 用户生成内容
    /// 管理用户生成的内容，如自定义地图、关卡等。
    /// </summary>
    UserGeneratedContent,

    /// <summary>
    /// 环境
    /// 管理游戏世界的环境因素，如天气、时间等。
    /// </summary>
    Environment,

    /// <summary>
    /// 拍卖行
    /// 管理拍卖行的物品列表和交易。
    /// </summary>
    AuctionHouse,

    /// <summary>
    /// 竞技场
    /// 管理玩家在竞技场的对战和排名。
    /// </summary>
    Arena,

    /// <summary>
    /// 公会
    /// 管理公会的创建、管理和活动。
    /// </summary>
    Guild,

    /// <summary>
    /// 房间
    /// 管理房间的创建、管理和活动。
    /// </summary>
    Room,

    /// <summary>
    /// 战斗
    /// 专门处理游戏中的战斗逻辑和同步。
    /// </summary>
    Combat,

    /// <summary>
    /// 技能
    /// 管理玩家技能和技能树。
    /// </summary>
    Skill,

    /// <summary>
    /// 装备
    /// 管理玩家装备和装备属性。
    /// </summary>
    Equipment,

    /// <summary>
    /// 属性
    /// 管理角色或对象的属性，如力量、智力等。
    /// </summary>
    Attribute,

    /// <summary>
    /// NPC行为
    /// 控制NPC的行为和交互。
    /// </summary>
    NpcBehavior,

    /// <summary>
    /// 脚本
    /// 执行和管理游戏脚本和自定义逻辑。
    /// </summary>
    Script,

    /// <summary>
    /// 用户反馈
    /// 收集和处理用户反馈和报告。
    /// </summary>
    Feedback,

    /// <summary>
    /// 玩家反馈响应
    /// 提供玩家反馈响应和处理
    /// </summary>
    PlayerFeedbackResponse,

    /// <summary>
    /// 客户支持
    /// 提供客户支持和帮助请求的处理。
    /// </summary>
    Support,

    /// <summary>
    /// 数据挖掘
    /// 进行数据挖掘以发现玩家行为模式和趋势。
    /// </summary>
    DataMining,

    /// <summary>
    /// 虚拟货币
    /// 管理游戏中的虚拟货币系统.
    /// </summary>
    VirtualCurrency,

    /// <summary>
    /// 游戏教程
    /// 提供新玩家教程和游戏指导。
    /// </summary>
    Tutorial,

    /// <summary>
    /// 玩家行为预测
    /// 预测玩家行为以提供个性化的游戏体验。
    /// </summary>
    PlayerBehaviorPrediction,

    /// <summary>
    /// 玩家行为识别
    /// 识别和奖励积极的玩家行为或处理不当行为。
    /// </summary>
    PlayerBehaviorRecognition,

    /// <summary>
    /// 分割线(勿调整,勿用于业务逻辑)
    /// </summary>
    Separator = 128,

    /// <summary>
    /// 固定ID类型Actor
    /// </summary>
    Server = 129,

    /// <summary>
    /// 账号
    /// 管理玩家账号信息，如注册、密码找回等。
    /// </summary>
    Account,

    /// <summary>
    /// 最大值
    /// </summary>
    Max = 999,
}

/// <summary>
/// 供ActorLimit检测调用关系
/// </summary>
public enum ActorTypeLevel
{
    /// <summary>
    /// 角色
    /// </summary>
    Role = 1,

    /// <summary>
    /// 公会
    /// </summary>
    Guild,

    /// <summary>
    /// 系统服务
    /// </summary>
    Server,
}