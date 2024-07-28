namespace GameFrameX.Core.Abstractions
{
    /// <summary>
    /// 每个服存在多个实例的（如玩家和公会）需要小于Separator
    /// 最大id应当小于4096
    /// Id一旦定义了不应该修改
    /// </summary>
    public enum ActorType
    {
        /// <summary>
        /// ID全服唯一类型
        /// </summary>
        None,

        /// <summary>
        /// 账号
        /// </summary>
        Account,

        /// <summary>
        /// 角色
        /// </summary>
        Player,

        /// <summary>
        /// 公会
        /// </summary>
        Guild,

        /// <summary>
        /// 网关
        /// </summary>
        Gate,

        /// <summary>
        /// 网关
        /// </summary>
        Chat,

        /// <summary>
        /// 世界
        /// </summary>
        World,

        /// <summary>
        /// 任务
        /// </summary>
        Task,

        /// <summary>
        /// 逻辑
        /// </summary>
        Logic,

        /// <summary>
        /// 全局
        /// </summary>
        Global,

        /// <summary>
        /// 数据库
        /// </summary>
        DataBase,

        /// <summary>
        /// 缓存
        /// </summary>
        Cache,

        /// <summary>
        /// 分割线(勿调整,勿用于业务逻辑)
        /// </summary>
        Separator = 128,

        /// <summary>
        /// 固定ID类型Actor
        /// </summary>
        Server = 129,

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
}