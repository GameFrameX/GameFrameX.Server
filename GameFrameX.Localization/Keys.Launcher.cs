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
/// 本地化键常量定义 - Launcher 分部类
/// </summary>
public static partial class Keys
{
    /// <summary>
    /// Launcher模块相关日志和错误消息资源键
    /// </summary>
    public static class Launcher
    {
        /// <summary>
        /// 开始启动服务器{0}
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.ServerStartBegin
        /// 用途: 记录服务器启动开始的信息
        /// 参数: {0} - 服务器类型
        /// </remarks>
        public const string ServerStartBegin = "Launcher.ServerStartBegin";

        /// <summary>
        /// 开始配置Actor限制逻辑...
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.ActorLimitConfigBegin
        /// 用途: 记录开始配置Actor限制逻辑的日志
        /// </remarks>
        public const string ActorLimitConfigBegin = "Launcher.ActorLimitConfigBegin";

        /// <summary>
        /// 配置Actor限制逻辑结束...
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.ActorLimitConfigEnd
        /// 用途: 记录配置Actor限制逻辑结束的日志
        /// </remarks>
        public const string ActorLimitConfigEnd = "Launcher.ActorLimitConfigEnd";

        /// <summary>
        /// 开始启动数据库服务...
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.DatabaseServiceStartBegin
        /// 用途: 记录开始启动数据库服务的日志
        /// </remarks>
        public const string DatabaseServiceStartBegin = "Launcher.DatabaseServiceStartBegin";

        /// <summary>
        /// 数据库服务启动失败的异常消息
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.DatabaseServiceStartFailed
        /// 用途: 当数据库服务启动失败时抛出异常
        /// </remarks>
        public const string DatabaseServiceStartFailed = "Launcher.DatabaseServiceStartFailed";

        /// <summary>
        /// 启动数据库服务 结束...
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.DatabaseServiceStartEnd
        /// 用途: 记录数据库服务启动结束的控制台日志
        /// </remarks>
        public const string DatabaseServiceStartEnd = "Launcher.DatabaseServiceStartEnd";

        /// <summary>
        /// 注册组件开始...
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.ComponentRegisterBegin
        /// 用途: 记录组件注册开始的日志
        /// </remarks>
        public const string ComponentRegisterBegin = "Launcher.ComponentRegisterBegin";

        /// <summary>
        /// 注册组件结束...
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.ComponentRegisterEnd
        /// 用途: 记录组件注册结束的日志
        /// </remarks>
        public const string ComponentRegisterEnd = "Launcher.ComponentRegisterEnd";

        /// <summary>
        /// 开始加载热更新模块...
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.HotfixModuleLoadBegin
        /// 用途: 记录开始加载热更新模块的日志
        /// </remarks>
        public const string HotfixModuleLoadBegin = "Launcher.HotfixModuleLoadBegin";

        /// <summary>
        /// 加载热更新模块结束...
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.HotfixModuleLoadEnd
        /// 用途: 记录热更新模块加载结束的日志
        /// </remarks>
        public const string HotfixModuleLoadEnd = "Launcher.HotfixModuleLoadEnd";

        /// <summary>
        /// 进入游戏主循环...
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.EnterMainLoop
        /// 用途: 记录进入游戏主循环的日志
        /// </remarks>
        public const string EnterMainLoop = "Launcher.EnterMainLoop";

        /// <summary>
        /// 服务器{0}启动结束...
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.ServerStartEnd
        /// 用途: 记录服务器启动结束的信息
        /// 参数: {0} - 服务器类型
        /// </remarks>
        public const string ServerStartEnd = "Launcher.ServerStartEnd";

        /// <summary>
        /// 服务器执行异常，e:{0}
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.ServerExecutionException
        /// 用途: 记录服务器执行异常的信息
        /// 参数: {0} - 异常详情
        /// </remarks>
        public const string ServerExecutionException = "Launcher.ServerExecutionException";

        /// <summary>
        /// 退出服务器开始
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.ServerExitBegin
        /// 用途: 记录服务器开始退出的日志
        /// </remarks>
        public const string ServerExitBegin = "Launcher.ServerExitBegin";

        /// <summary>
        /// 退出服务器成功
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.ServerExitSuccess
        /// 用途: 记录服务器退出成功的日志
        /// </remarks>
        public const string ServerExitSuccess = "Launcher.ServerExitSuccess";

        /// <summary>
        /// ---发送[{0} To {1}]  {2}
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.SendMessage
        /// 用途: 记录发送消息的调试信息
        /// 参数: {0} - 发送方服务器类型, {1} - 接收方服务器类型, {2} - 消息内容
        /// </remarks>
        public const string SendMessage = "Launcher.SendMessage";

        /// <summary>
        /// ---收到[{0} To {1}]  {2}
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.ReceiveMessage
        /// 用途: 记录接收消息的调试信息
        /// 参数: {0} - 发送方服务器类型, {1} - 接收方服务器类型, {2} - 消息内容
        /// </remarks>
        public const string ReceiveMessage = "Launcher.ReceiveMessage";

        /// <summary>
        /// 注册玩家成功：{0}  {1}
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.PlayerRegisterSuccess
        /// 用途: 记录玩家注册成功的日志
        /// 参数: {0} - 玩家ID, {1} - 玩家信息
        /// </remarks>
        public const string PlayerRegisterSuccess = "Launcher.PlayerRegisterSuccess";

        /// <summary>
        /// 注销玩家成功：{0}  {1}
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.PlayerUnregisterSuccess
        /// 用途: 记录玩家注销成功的日志
        /// 参数: {0} - 玩家ID, {1} - 玩家信息
        /// </remarks>
        public const string PlayerUnregisterSuccess = "Launcher.PlayerUnregisterSuccess";

        /// <summary>
        /// 注册服务成功：{0}  {1}  {2}
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.ServiceRegisterSuccess
        /// 用途: 记录服务注册成功的日志
        /// 参数: {0} - 服务器类型, {1} - 服务器名称, {2} - 注册信息
        /// </remarks>
        public const string ServiceRegisterSuccess = "Launcher.ServiceRegisterSuccess";

        /// <summary>
        /// 注销服务成功：{0}  {1}
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.ServiceUnregisterSuccess
        /// 用途: 记录服务注销成功的日志
        /// 参数: {0} - 服务器ID, {1} - 服务器实例ID
        /// </remarks>
        public const string ServiceUnregisterSuccess = "Launcher.ServiceUnregisterSuccess";

        /// <summary>
        /// 有外部服务连接到中心服务器成功。链接信息：SessionID:{0} RemoteEndPoint:{1}
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.ExternalServiceConnected
        /// 用途: 记录外部服务连接到中心服务器成功的日志
        /// 参数: {0} - SessionID, {1} - RemoteEndPoint
        /// </remarks>
        public const string ExternalServiceConnected = "Launcher.ExternalServiceConnected";

        /// <summary>
        /// 有外部服务从中心服务器断开。链接信息：断开原因:{0}
        /// </summary>
        /// <remarks>
        /// 键名: Launcher.ExternalServiceDisconnected
        /// 用途: 记录外部服务从中心服务器断开的日志
        /// 参数: {0} - 断开原因
        /// </remarks>
        public const string ExternalServiceDisconnected = "Launcher.ExternalServiceDisconnected";
    }
}