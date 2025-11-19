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

namespace GameFrameX.Localization;

/// <summary>
/// 本地化键常量定义 - Core 分部类
/// </summary>
public static partial class Keys
{
    /// <summary>
    /// Core模块相关日志和错误消息资源键
    /// </summary>
    public static class Core
    {
        /// <summary>
        /// Actor相关消息
        /// </summary>
        public static class Actor
        {
            /// <summary>
            /// actor跨天 id:{0} type:{1}
            /// </summary>
            /// <remarks>
            /// 键名: Core.Actor.CrossDay
            /// 用途: Actor跨天处理时记录
            /// 参数: {0} - Actor ID, {1} - Actor类型
            /// </remarks>
            public const string CrossDay = "Core.Actor.CrossDay";

            /// <summary>
            /// {0}跨天失败 actorId:{1} actorType:{2} 异常：\n{3}
            /// </summary>
            /// <remarks>
            /// 键名: Core.Actor.CrossDayFailed
            /// 用途: Actor跨天处理失败时记录
            /// 参数: {0} - Agent类型, {1} - Actor ID, {2} - Actor类型, {3} - 异常信息
            /// </remarks>
            public const string CrossDayFailed = "Core.Actor.CrossDayFailed";

            /// <summary>
            /// Actor回收回调执行异常 actorId:{0} actorType:{1} 异常：\n{2}
            /// </summary>
            /// <remarks>
            /// 键名: Core.Actor.RecycleCallbackFailed
            /// 用途: Actor回收回调执行异常时记录
            /// 参数: {0} - Actor ID, {1} - Actor类型, {2} - 异常信息
            /// </remarks>
            public const string RecycleCallbackFailed = "Core.Actor.RecycleCallbackFailed";

            /// <summary>
            /// Actor回收过程异常 actorId:{0} actorType:{1} 异常：\n{2}
            /// </summary>
            /// <remarks>
            /// 键名: Core.Actor.RecycleFailed
            /// 用途: Actor回收过程异常时记录
            /// 参数: {0} - Actor ID, {1} - Actor类型, {2} - 异常信息
            /// </remarks>
            public const string RecycleFailed = "Core.Actor.RecycleFailed";

            /// <summary>
            /// actor回收 id:{0} type:{1}
            /// </summary>
            /// <remarks>
            /// 键名: Core.Actor.Recycled
            /// 用途: Actor被回收时记录
            /// 参数: {0} - Actor ID, {1} - Actor类型
            /// </remarks>
            public const string Recycled = "Core.Actor.Recycled";
        }

        /// <summary>
        /// Actor限制相关消息
        /// </summary>
        public static class ActorLimit
        {
            /// <summary>
            /// 不支持的rule类型:{0}
            /// </summary>
            /// <remarks>
            /// 键名: Core.ActorLimit.UnsupportedRuleType
            /// 用途: 当遇到不支持的规则类型时记录
            /// 参数: {0} - 规则类型
            /// </remarks>
            public const string UnsupportedRuleType = "Core.ActorLimit.UnsupportedRuleType";

            /// <summary>
            /// 不合法的调用路径:{0}==>{1}
            /// </summary>
            /// <remarks>
            /// 键名: Core.ActorLimit.InvalidCallPath
            /// 用途: 当检测到不合法的调用路径时记录
            /// 参数: {0} - 当前类型, {1} - 目标类型
            /// </remarks>
            public const string InvalidCallPath = "Core.ActorLimit.InvalidCallPath";

            /// <summary>
            /// 发生交叉死锁，ActorId1:{0} ActorType1:{1} ActorId2:{2} ActorType2:{3}
            /// </summary>
            /// <remarks>
            /// 键名: Core.ActorLimit.CrossDeadlock
            /// 用途: 当检测到交叉死锁时记录
            /// 参数: {0} - Actor ID 1, {1} - Actor类型1, {2} - Actor ID 2, {3} - Actor类型2
            /// </remarks>
            public const string CrossDeadlock = "Core.ActorLimit.CrossDeadlock";
        }

        /// <summary>
        /// 定时器相关消息
        /// </summary>
        public static class Timer
        {
            /// <summary>
            /// 初始化全局定时开始...
            /// </summary>
            /// <remarks>
            /// 键名: Core.Timer.GlobalTimerInitializationStart
            /// 用途: 全局定时器初始化开始时记录
            /// </remarks>
            public const string GlobalTimerInitializationStart = "Core.Timer.GlobalTimerInitializationStart";

            /// <summary>
            /// 初始化全局定时完成...
            /// </summary>
            /// <remarks>
            /// 键名: Core.Timer.GlobalTimerInitializationComplete
            /// 用途: 全局定时器初始化完成时记录
            /// </remarks>
            public const string GlobalTimerInitializationComplete = "Core.Timer.GlobalTimerInitializationComplete";

            /// <summary>
            /// 下次定时回存时间 {0}
            /// </summary>
            /// <remarks>
            /// 键名: Core.Timer.NextSaveTime
            /// 用途: 记录下次定时回存时间
            /// 参数: {0} - 下次回存时间
            /// </remarks>
            public const string NextSaveTime = "Core.Timer.NextSaveTime";

            /// <summary>
            /// 开始定时回存 时间:{0}
            /// </summary>
            /// <remarks>
            /// 键名: Core.Timer.SaveStart
            /// 用途: 定时回存开始时记录
            /// 参数: {0} - 开始时间
            /// </remarks>
            public const string SaveStart = "Core.Timer.SaveStart";

            /// <summary>
            /// 结束定时回存 时间:{0} 耗时: {1}ms
            /// </summary>
            /// <remarks>
            /// 键名: Core.Timer.SaveEnd
            /// 用途: 定时回存结束时记录
            /// 参数: {0} - 结束时间, {1} - 耗时(毫秒)
            /// </remarks>
            public const string SaveEnd = "Core.Timer.SaveEnd";

            /// <summary>
            /// 开始回收空闲Actor 时间:{0}
            /// </summary>
            /// <remarks>
            /// 键名: Core.Timer.ActorRecycleStart
            /// 用途: 开始回收空闲Actor时记录
            /// 参数: {0} - 开始时间
            /// </remarks>
            public const string ActorRecycleStart = "Core.Timer.ActorRecycleStart";

            /// <summary>
            /// 结束回收空闲Actor 时间:{0}
            /// </summary>
            /// <remarks>
            /// 键名: Core.Timer.ActorRecycleEnd
            /// 用途: 结束回收空闲Actor时记录
            /// 参数: {0} - 结束时间
            /// </remarks>
            public const string ActorRecycleEnd = "Core.Timer.ActorRecycleEnd";

            /// <summary>
            /// 停止全局定时开始...
            /// </summary>
            /// <remarks>
            /// 键名: Core.Timer.GlobalTimerStopStart
            /// 用途: 停止全局定时开始时记录
            /// </remarks>
            public const string GlobalTimerStopStart = "Core.Timer.GlobalTimerStopStart";

            /// <summary>
            /// 停止全局定时完成...
            /// </summary>
            /// <remarks>
            /// 键名: Core.Timer.GlobalTimerStopComplete
            /// 用途: 停止全局定时完成时记录
            /// </remarks>
            public const string GlobalTimerStopComplete = "Core.Timer.GlobalTimerStopComplete";

            /// <summary>
            /// 错误的ITimerHandler类型，回调失败 type:{0}
            /// </summary>
            /// <remarks>
            /// 键名: Core.Timer.InvalidHandlerType
            /// 用途: 当ITimerHandler类型错误时记录
            /// 参数: {0} - 处理器类型
            /// </remarks>
            public const string InvalidHandlerType = "Core.Timer.InvalidHandlerType";
        }

        /// <summary>
        /// Actor管理器相关消息
        /// </summary>
        public static class ActorManager
        {
            /// <summary>
            /// save all state, use: {0}ms
            /// </summary>
            /// <remarks>
            /// 键名: Core.ActorManager.SaveAllStateTime
            /// 用途: 保存所有状态时记录耗时
            /// 参数: {0} - 耗时(毫秒)
            /// </remarks>
            public const string SaveAllStateTime = "Core.ActorManager.SaveAllStateTime";

            /// <summary>
            /// save all state error
            /// {0}
            /// </summary>
            /// <remarks>
            /// 键名: Core.ActorManager.SaveAllStateError
            /// 用途: 保存所有状态出错时记录
            /// 参数: {0} - 异常信息
            /// </remarks>
            public const string SaveAllStateError = "Core.ActorManager.SaveAllStateError";

            /// <summary>
            /// timer save state error
            /// </summary>
            /// <remarks>
            /// 键名: Core.ActorManager.TimerSaveStateError
            /// 用途: 定时器保存状态出错时记录
            /// </remarks>
            public const string TimerSaveStateError = "Core.ActorManager.TimerSaveStateError";

            /// <summary>
            /// 全局Actor：{0}执行跨天
            /// </summary>
            /// <remarks>
            /// 键名: Core.ActorManager.GlobalActorCrossDay
            /// 用途: 全局Actor执行跨天时记录
            /// 参数: {0} - Actor类型
            /// </remarks>
            public const string GlobalActorCrossDay = "Core.ActorManager.GlobalActorCrossDay";

            /// <summary>
            /// 全局comp跨天耗时过久，不阻止其他comp跨天，当前已过{0}秒
            /// </summary>
            /// <remarks>
            /// 键名: Core.ActorManager.GlobalCompCrossDayTimeout
            /// 用途: 全局组件跨天超时时记录
            /// 参数: {0} - 超时秒数
            /// </remarks>
            public const string GlobalCompCrossDayTimeout = "Core.ActorManager.GlobalCompCrossDayTimeout";

            /// <summary>
            /// 全局comp跨天完成 耗时：{0}ms
            /// </summary>
            /// <remarks>
            /// 键名: Core.ActorManager.GlobalCompCrossDayComplete
            /// 用途: 全局组件跨天完成时记录
            /// 参数: {0} - 耗时(毫秒)
            /// </remarks>
            public const string GlobalCompCrossDayComplete = "Core.ActorManager.GlobalCompCrossDayComplete";

            /// <summary>
            /// 非玩家comp跨天耗时过久，不阻止玩家comp跨天，当前已过{0}秒
            /// </summary>
            /// <remarks>
            /// 键名: Core.ActorManager.NonPlayerCompCrossDayTimeout
            /// 用途: 非玩家组件跨天超时时记录
            /// 参数: {0} - 超时秒数
            /// </remarks>
            public const string NonPlayerCompCrossDayTimeout = "Core.ActorManager.NonPlayerCompCrossDayTimeout";

            /// <summary>
            /// 非玩家comp跨天完成 耗时：{0}ms
            /// </summary>
            /// <remarks>
            /// 键名: Core.ActorManager.NonPlayerCompCrossDayComplete
            /// 用途: 非玩家组件跨天完成时记录
            /// 参数: {0} - 耗时(毫秒)
            /// </remarks>
            public const string NonPlayerCompCrossDayComplete = "Core.ActorManager.NonPlayerCompCrossDayComplete";
        }

        /// <summary>
        /// 组件注册相关消息
        /// </summary>
        public static class ComponentRegister
        {
            /// <summary>
            /// initialize component registration complete
            /// </summary>
            /// <remarks>
            /// 键名: Core.ComponentRegister.InitializationComplete
            /// 用途: 组件注册初始化完成时记录
            /// </remarks>
            public const string InitializationComplete = "Core.ComponentRegister.InitializationComplete";

            /// <summary>
            /// {0}未实现Agent,请检查业务代码是否正确
            /// </summary>
            /// <remarks>
            /// 键名: Core.ComponentRegister.AgentNotImplemented
            /// 用途: 组件未实现Agent时记录
            /// 参数: {0} - 组件类型
            /// </remarks>
            public const string AgentNotImplemented = "Core.ComponentRegister.AgentNotImplemented";

            /// <summary>
            /// 激活全局组件：{0} {1}
            /// </summary>
            /// <remarks>
            /// 键名: Core.ComponentRegister.ActivateGlobalComponent
            /// 用途: 激活全局组件时记录
            /// 参数: {0} - Actor类型, {1} - 组件类型
            /// </remarks>
            public const string ActivateGlobalComponent = "Core.ComponentRegister.ActivateGlobalComponent";

            /// <summary>
            /// activate the global actor: {0}
            /// </summary>
            /// <remarks>
            /// 键名: Core.ComponentRegister.ActivateGlobalActor
            /// 用途: 激活全局Actor时记录
            /// 参数: {0} - Actor类型
            /// </remarks>
            public const string ActivateGlobalActor = "Core.ComponentRegister.ActivateGlobalActor";

            /// <summary>
            /// Activate the global component and check if the components all contain the Agent implementation completion
            /// </summary>
            /// <remarks>
            /// 键名: Core.ComponentRegister.ActivateGlobalComponentCheckComplete
            /// 用途: 全局组件激活检查完成时记录
            /// </remarks>
            public const string ActivateGlobalComponentCheckComplete = "Core.ComponentRegister.ActivateGlobalComponentCheckComplete";

            /// <summary>
            /// Activate the global component and detect if the components all contain the agent implementation failed
            /// </summary>
            /// <remarks>
            /// 键名: Core.ComponentRegister.ActivateGlobalComponentCheckFailed
            /// 用途: 全局组件激活检查失败时记录
            /// </remarks>
            public const string ActivateGlobalComponentCheckFailed = "Core.ComponentRegister.ActivateGlobalComponentCheckFailed";

            /// <summary>
            /// get an actor that doesn't belong to this actor: [{0}] components
            /// </summary>
            /// <remarks>
            /// 键名: Core.ComponentRegister.ActorNotBelongToThis
            /// 用途: 当尝试获取不属于当前Actor的组件时记录
            /// 参数: {0} - Actor类型
            /// </remarks>
            public const string ActorNotBelongToThis = "Core.ComponentRegister.ActorNotBelongToThis";
        }

        /// <summary>
        /// 状态组件相关消息
        /// </summary>
        public static class StateComponent
        {
            /// <summary>
            /// save all state, use: {0}ms
            /// </summary>
            /// <remarks>
            /// 键名: Core.StateComponent.SaveAllStateTime
            /// 用途: 状态组件保存所有状态时记录耗时
            /// 参数: {0} - 耗时(毫秒)
            /// </remarks>
            public const string SaveAllStateTime = "Core.StateComponent.SaveAllStateTime";

            /// <summary>
            /// save all state error
            /// {0}
            /// </summary>
            /// <remarks>
            /// 键名: Core.StateComponent.SaveAllStateError
            /// 用途: 状态组件保存所有状态出错时记录
            /// 参数: {0} - 异常信息
            /// </remarks>
            public const string SaveAllStateError = "Core.StateComponent.SaveAllStateError";

            /// <summary>
            /// timer save state error
            /// </summary>
            /// <remarks>
            /// 键名: Core.StateComponent.TimerSaveStateError
            /// 用途: 状态组件定时器保存状态出错时记录
            /// </remarks>
            public const string TimerSaveStateError = "Core.StateComponent.TimerSaveStateError";

            /// <summary>
            /// StateComp.SaveState.Failed.StateId:{0},{1}
            /// </summary>
            /// <remarks>
            /// 键名: Core.StateComponent.SaveStateFailed
            /// 用途: 状态组件保存状态失败时记录
            /// 参数: {0} - 状态ID, {1} - 异常信息
            /// </remarks>
            public const string SaveStateFailed = "Core.StateComponent.SaveStateFailed";

            /// <summary>
            /// [StateComp] 状态回存 {0} count:{1}
            /// </summary>
            /// <remarks>
            /// 键名: Core.StateComponent.StateSaveBack
            /// 用途: 状态回存时记录
            /// 参数: {0} - 状态名称, {1} - 数量
            /// </remarks>
            public const string StateSaveBack = "Core.StateComponent.StateSaveBack";

            /// <summary>
            /// 保存数据失败，类型:{0}
            /// </summary>
            /// <remarks>
            /// 键名: Core.StateComponent.SaveDataFailed
            /// 用途: 保存数据失败时记录
            /// 参数: {0} - 数据类型
            /// </remarks>
            public const string SaveDataFailed = "Core.StateComponent.SaveDataFailed";

            /// <summary>
            /// 保存数据异常，类型:{0}，{1}
            /// </summary>
            /// <remarks>
            /// 键名: Core.StateComponent.SaveDataException
            /// 用途: 保存数据异常时记录
            /// 参数: {0} - 数据类型, {1} - 异常信息
            /// </remarks>
            public const string SaveDataException = "Core.StateComponent.SaveDataException";
        }

        /// <summary>
        /// 消息处理器相关消息
        /// </summary>
        public static class MessageHandler
        {
            /// <summary>
            /// 执行超时:{0}
            /// </summary>
            /// <remarks>
            /// 键名: Core.MessageHandler.ExecutionTimeout
            /// 用途: 消息处理器执行超时时记录
            /// 参数: {0} - 超时消息
            /// </remarks>
            public const string ExecutionTimeout = "Core.MessageHandler.ExecutionTimeout";

            /// <summary>
            /// 消息处理器：{0},UniqueId：{1} 执行耗时：{2} ms
            /// </summary>
            /// <remarks>
            /// 键名: Core.MessageHandler.ExecutionTimeWarning
            /// 用途: 消息处理器执行耗时过长时记录
            /// 参数: {0} - 处理器名称, {1} - 消息唯一ID, {2} - 耗时(毫秒)
            /// </remarks>
            public const string ExecutionTimeWarning = "Core.MessageHandler.ExecutionTimeWarning";

            /// <summary>
            /// 消息处理器：{0},UniqueId：{1} 执行耗时：{2} ms
            /// </summary>
            /// <remarks>
            /// 键名: Core.MessageHandler.ExecutionTimeDebug
            /// 用途: 调试模式下记录消息处理器执行耗时
            /// 参数: {0} - 处理器名称, {1} - 消息唯一ID, {2} - 耗时(毫秒)
            /// </remarks>
            public const string ExecutionTimeDebug = "Core.MessageHandler.ExecutionTimeDebug";

            /// <summary>
            /// ActorId is 0, can not get component，{0}, close channel
            /// </summary>
            /// <remarks>
            /// 键名: Core.MessageHandler.ActorIdIsZero
            /// 用途: 当ActorId为0无法获取组件时记录
            /// 参数: {0} - 消息类型
            /// </remarks>
            public const string ActorIdIsZero = "Core.MessageHandler.ActorIdIsZero";

            /// <summary>
            /// CacheComponent is null, can not get component, close channel
            /// </summary>
            /// <remarks>
            /// 键名: Core.MessageHandler.CacheComponentIsNull
            /// 用途: 当缓存组件为空时记录
            /// </remarks>
            public const string CacheComponentIsNull = "Core.MessageHandler.CacheComponentIsNull";

            /// <summary>
            /// InnerAction error: {0} {1}
            /// </summary>
            /// <remarks>
            /// 键名: Core.MessageHandler.InnerActionError
            /// 用途: 内部动作执行错误时记录
            /// 参数: {0} - 请求消息类型, {1} - 请求消息
            /// </remarks>
            public const string InnerActionError = "Core.MessageHandler.InnerActionError";
        }

        /// <summary>
        /// 热更新相关消息
        /// </summary>
        public static class Hotfix
        {
            /// <summary>
            /// the hot change dll initialization succeeds: {0}
            /// </summary>
            /// <remarks>
            /// 键名: Core.Hotfix.DllInitializationSuccess
            /// 用途: 热更新DLL初始化成功时记录
            /// 参数: {0} - DLL路径
            /// </remarks>
            public const string DllInitializationSuccess = "Core.Hotfix.DllInitializationSuccess";

            /// <summary>
            /// the hot change dll initialization failed...
            /// {0}
            /// </summary>
            /// <remarks>
            /// 键名: Core.Hotfix.DllInitializationFailed
            /// 用途: 热更新DLL初始化失败时记录
            /// 参数: {0} - 异常信息
            /// </remarks>
            public const string DllInitializationFailed = "Core.Hotfix.DllInitializationFailed";

            /// <summary>
            /// hot dll uninstall{0}
            /// </summary>
            /// <remarks>
            /// 键名: Core.Hotfix.DllUninstall
            /// 用途: 热更新DLL卸载时记录
            /// 参数: {0} - 成功或失败状态
            /// </remarks>
            public const string DllUninstall = "Core.Hotfix.DllUninstall";
        }
    }

    /// <summary>
    /// Core模块异常相关资源键
    /// </summary>
    public static class CoreExceptions
    {
        /// <summary>
        /// 定时器相关异常
        /// </summary>
        public static class Timer
        {
            /// <summary>
            /// 定时器参数错误 TimerHandler:{0} {1}:{2} {3}:{4}
            /// </summary>
            /// <remarks>
            /// 键名: CoreExceptions.Timer.InvalidParameters
            /// 用途: 定时器参数错误时抛出异常
            /// 参数: {0} - 处理器类型, {1} - 参数名1, {2} - 参数值1, {3} - 参数名2, {4} - 参数值2
            /// </remarks>
            public const string InvalidParameters = "CoreExceptions.Timer.InvalidParameters";

            /// <summary>
            /// 定时每周执行 参数为空：{0} TimerHandler:{1} actorId:{2} actorType:{3}
            /// </summary>
            /// <remarks>
            /// 键名: CoreExceptions.Timer.DayOfWeeksParameterNull
            /// 用途: 定时器每周执行参数为空时抛出异常
            /// 参数: {0} - 参数名, {1} - 处理器类型, {2} - Actor ID, {3} - Actor类型
            /// </remarks>
            public const string DayOfWeeksParameterNull = "CoreExceptions.Timer.DayOfWeeksParameterNull";

            /// <summary>
            /// 定时器代码需要在热更项目里
            /// </summary>
            /// <remarks>
            /// 键名: CoreExceptions.Timer.CodeMustBeInHotfix
            /// 用途: 定时器代码不在热更项目时抛出异常
            /// </remarks>
            public const string CodeMustBeInHotfix = "CoreExceptions.Timer.CodeMustBeInHotfix";
        }

        /// <summary>
        /// 组件相关异常
        /// </summary>
        public static class Component
        {
            /// <summary>
            /// component:[{0}] the actor type is not bound
            /// </summary>
            /// <remarks>
            /// 键名: CoreExceptions.Component.ActorTypeNotBound
            /// 用途: 组件Actor类型未绑定时抛出异常
            /// 参数: {0} - 组件类型
            /// </remarks>
            public const string ActorTypeNotBound = "CoreExceptions.Component.ActorTypeNotBound";

            /// <summary>
            /// get an actor that doesn't belong to this actor: [{0}] component:[{1}]
            /// </summary>
            /// <remarks>
            /// 键名: CoreExceptions.Component.ActorNotBelongToThis
            /// 用途: 获取不属于当前Actor的组件时抛出异常
            /// 参数: {0} - Actor类型, {1} - 组件类型
            /// </remarks>
            public const string ActorNotBelongToThis = "CoreExceptions.Component.ActorNotBelongToThis";
        }

        /// <summary>
        /// 消息相关异常
        /// </summary>
        public static class Message
        {
            /// <summary>
            /// 消息类型错误, {0} to: {1}
            /// </summary>
            /// <remarks>
            /// 键名: CoreExceptions.Message.TypeCastError
            /// 用途: 消息类型转换错误时抛出异常
            /// 参数: {0} - 源类型, {1} - 目标类型
            /// </remarks>
            public const string TypeCastError = "CoreExceptions.Message.TypeCastError";

            /// <summary>
            /// 消息处理器未初始化,请调用先Init方法，如果已经子类实现了Init方法，请调用在子类Init中调用父类Init方法
            /// </summary>
            /// <remarks>
            /// 键名: CoreExceptions.Message.HandlerNotInitialized
            /// 用途: 消息处理器未初始化时抛出异常
            /// </remarks>
            public const string HandlerNotInitialized = "CoreExceptions.Message.HandlerNotInitialized";
        }

        /// <summary>
        /// 热更新相关异常
        /// </summary>
        public static class Hotfix
        {
            /// <summary>
            /// HTTP processor command repeatedly registers, command:{0}
            /// </summary>
            /// <remarks>
            /// 键名: CoreExceptions.Hotfix.HttpProcessorRepeatedlyRegistered
            /// 用途: HTTP处理器重复注册时抛出异常
            /// 参数: {0} - 命令
            /// </remarks>
            public const string HttpProcessorRepeatedlyRegistered = "CoreExceptions.Hotfix.HttpProcessorRepeatedlyRegistered";

            /// <summary>
            /// wrong tcp processor type:{0}
            /// </summary>
            /// <remarks>
            /// 键名: CoreExceptions.Hotfix.WrongTcpProcessorType
            /// 用途: TCP处理器类型错误时抛出异常
            /// 参数: {0} - 处理器类型
            /// </remarks>
            public const string WrongTcpProcessorType = "CoreExceptions.Hotfix.WrongTcpProcessorType";

            /// <summary>
            /// {0} must be a class marked as sealed
            /// </summary>
            /// <remarks>
            /// 键名: CoreExceptions.Hotfix.ClassMustBeSealed
            /// 用途: 类必须标记为sealed时抛出异常
            /// 参数: {0} - 类名
            /// </remarks>
            public const string ClassMustBeSealed = "CoreExceptions.Hotfix.ClassMustBeSealed";

            /// <summary>
            /// the message processor must be in the[{0}]ending，{1}
            /// </summary>
            /// <remarks>
            /// 键名: CoreExceptions.Hotfix.MessageProcessorWrongSuffix
            /// 用途: 消息处理器后缀错误时抛出异常
            /// 参数: {0} - 正确后缀, {1} - 类名
            /// </remarks>
            public const string MessageProcessorWrongSuffix = "CoreExceptions.Hotfix.MessageProcessorWrongSuffix";

            /// <summary>
            /// the event handler must be based on [{0}] ending，{1}
            /// </summary>
            /// <remarks>
            /// 键名: CoreExceptions.Hotfix.EventHandlerWrongSuffix
            /// 用途: 事件处理器后缀错误时抛出异常
            /// 参数: {0} - 正确后缀, {1} - 类名
            /// </remarks>
            public const string EventHandlerWrongSuffix = "CoreExceptions.Hotfix.EventHandlerWrongSuffix";

            /// <summary>
            /// IEventListener:{0} There are no events that are specified to listen to
            /// </summary>
            /// <remarks>
            /// 键名: CoreExceptions.Hotfix.NoEventsToListen
            /// 用途: 事件监听器没有指定要监听的事件时抛出异常
            /// 参数: {0} - 监听器类型
            /// </remarks>
            public const string NoEventsToListen = "CoreExceptions.Hotfix.NoEventsToListen";

            /// <summary>
            /// the component agent must be based on [{0}] ending，{1}
            /// </summary>
            /// <remarks>
            /// 键名: CoreExceptions.Hotfix.ComponentAgentWrongSuffix
            /// 用途: 组件代理后缀错误时抛出异常
            /// 参数: {0} - 正确后缀, {1} - 类名
            /// </remarks>
            public const string ComponentAgentWrongSuffix = "CoreExceptions.Hotfix.ComponentAgentWrongSuffix";

            /// <summary>
            /// component:[{0}] there are multiple agents
            /// </summary>
            /// <remarks>
            /// 键名: CoreExceptions.Hotfix.MultipleAgents
            /// 用途: 组件有多个代理时抛出异常
            /// 参数: {0} - 组件类型
            /// </remarks>
            public const string MultipleAgents = "CoreExceptions.Hotfix.MultipleAgents";
        }

        /// <summary>
        /// 属性相关异常
        /// </summary>
        public static class Attribute
        {
            /// <summary>
            /// 无效的组件类型 {0},值应大于{1}且小于{2}和不为{3}
            /// </summary>
            /// <remarks>
            /// 键名: CoreExceptions.Attribute.InvalidComponentType
            /// 用途: 组件类型无效时抛出异常
            /// 参数: {0} - 类型值, {1} - 最小值, {2} - 最大值, {3} - 分隔符值
            /// </remarks>
            public const string InvalidComponentType = "CoreExceptions.Attribute.InvalidComponentType";
        }
    }
}