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

namespace GameFrameX.Utility.Setting;

/// <summary>
/// 全局常量类
/// </summary>
public static class GlobalConst
{
    /// <summary>
    /// SessionId Key
    /// </summary>
    public const string SessionIdKey = "SESSION_ID";

    /// <summary>
    /// ActorId Key
    /// </summary>
    public const string ActorIdKey = "ACTOR_ID";

    /// <summary>
    /// 唯一ID
    /// </summary>
    public const string UniqueIdIdKey = "UNIQUEID_ID";

    /// <summary>
    /// 组件代理名称后缀
    /// </summary>
    public const string ComponentAgentNameSuffix = "ComponentAgent";

    /// <summary>
    /// 组件处理器名称后缀
    /// </summary>
    public const string ComponentHandlerNameSuffix = "Handler";

    /// <summary>
    /// 事件处理器名称后缀
    /// </summary>
    public const string EventListenerNameSuffix = "EventListener";

    /// <summary>
    /// 组件代理包裹名称后缀
    /// </summary>
    public const string ComponentAgentWrapperNameSuffix = "ComponentAgentWrapper";

    /// <summary>
    /// 组件包裹名称后缀
    /// </summary>
    public const string WrapperNameSuffix = "Wrapper";

    /// <summary>
    /// 组件代理名称前缀
    /// </summary>
    public const string HotfixNameSpaceNamePrefix = "GameFrameX.Hotfix.";

    /// <summary>
    /// 秒标记
    /// </summary>
    public const int SecondMask = 0b111111111111111111111111111111;

    /// <summary>
    /// 最大全局ID
    /// </summary>
    public const int MaxGlobalId = MaxServerId * 1000;

    /// <summary>
    /// 最小服务器ID
    /// </summary>
    public const int MinServerId = 1000;

    /// <summary>
    /// 最大服务器ID
    /// </summary>
    public const int MaxServerId = 9999;

    /// <summary>
    /// 最大Actor增量
    /// </summary>
    public const int MaxActorIncrease = 4095; // 4095

    /// <summary>
    /// 最大唯一增量
    /// </summary>
    public const int MaxUniqueIncrease = 524287; //524287

    /// <summary>
    /// 服务器ID 长度标记位=>49 = 63-14
    /// </summary>
    public const int ServerIdOrModuleIdMask = 49; //49+14=63

    /// <summary>
    /// Actor类型标记
    /// </summary>
    public const int ActorTypeMask = 42; //42+7 = 49

    /// <summary>
    /// 时间戳标记
    /// </summary>
    public const int TimestampMask = 12; //12+30 =42

    /// <summary>
    /// 模块ID时间戳标记
    /// </summary>
    public const int ModuleIdTimestampMask = 19; //19+30 =42

    /// <summary>
    /// 模块ID最大值
    /// </summary>
    public const int IdModuleMax = 999;

    /// <summary>
    /// WorkerActor模块Id 值 
    /// </summary>
    public const int WorkerActorIdModuleValue = 500;

    #region ActorType

    /// <summary>
    /// 空将会被判断为无效值
    /// </summary>
    public const ushort ActorTypeNone = 0;

    /// <summary>
    /// 角色
    /// </summary>
    public const ushort ActorTypePlayer = 1;

    /// <summary>
    /// Actor类型-分割线(勿调整,勿用于业务逻辑)
    /// </summary>
    public const int ActorTypeSeparator = 128;

    /// <summary>
    /// 服务器系统的Actor类型
    /// </summary>
    public const int ActorTypeServer = 129;

    /// <summary>
    /// Actor类型的最大值
    /// </summary>
    public const int ActorTypeMax = 999;

    #endregion


    #region HTTP

    /// <summary>
    /// HTTP 请求的签名字段名称
    /// </summary>
    public const string HttpSignKey = "sign";

    /// <summary>
    /// HTTP 请求的时间戳字段名称
    /// </summary>
    public const string HttpTimestampKey = "timestamp";

    #endregion


    #region GlobalTimer 全局计时器

    /// <summary>
    /// 数据存储间隔 单位 毫秒
    /// </summary>
    internal const int SaveIntervalInMilliSeconds = 300_000; //300_000;

    /// <summary>
    /// </summary>
    public const int MAGIC = 60;

    #endregion
}