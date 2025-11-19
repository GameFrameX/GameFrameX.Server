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
/// 本地化键常量定义 - Logs 分部类
/// </summary>
public static partial class Keys
{
    /// <summary>
    /// 日志消息资源键
    /// </summary>
    public static class Logs
    {
        /// <summary>
        /// 服务器启动时找到配置文件的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Server.Start.FindConfig
        /// 用途: 服务器启动过程中成功找到配置文件时记录
        /// 参数: {0} - 配置文件路径
        /// </remarks>
        public const string Server_Start_Find_Config = "Logs.Server.Start.FindConfig";

        /// <summary>
        /// 服务器启动使用默认配置的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Server.Start.DefaultConfig
        /// 用途: 服务器启动时未找到配置文件，使用默认配置时记录
        /// </remarks>
        public const string Server_Start_Default_Config = "Logs.Server.Start.DefaultConfig";

        /// <summary>
        /// 服务器正在启动的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Server.Starting
        /// 用途: 服务器启动开始时记录
        /// 参数: {0} - 服务器名称, {1} - 版本号
        /// </remarks>
        public const string Server_Starting = "Logs.Server.Starting";

        /// <summary>
        /// 服务器停止时无正在运行任务的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Server.Stopped.NoTasks
        /// 用途: 服务器停止时确认没有正在运行的任务时记录
        /// </remarks>
        public const string Server_Stopped_No_Tasks = "Logs.Server.Stopped.NoTasks";

        /// <summary>
        /// 游戏服务器启动的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Game.Server.Start
        /// 用途: 游戏服务器启动开始时记录
        /// </remarks>
        public const string Game_Server_Start = "Logs.Game.Server.Start";

        /// <summary>
        /// 游戏服务器配置Actor完成的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Game.Server.ConfigActor.End
        /// 用途: 游戏服务器Actor系统配置完成时记录
        /// </remarks>
        public const string Game_Server_Config_Actor_End = "Logs.Game.Server.ConfigActor.End";

        /// <summary>
        /// 游戏服务器数据库配置完成的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Game.Server.Database.End
        /// 用途: 游戏服务器数据库配置完成时记录
        /// </remarks>
        public const string Game_Server_Database_End = "Logs.Game.Server.Database.End";

        /// <summary>
        /// 游戏服务器注册组件开始的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Game.Server.RegisterComponents.Start
        /// 用途: 游戏服务器开始注册组件时记录
        /// </remarks>
        public const string Game_Server_Register_Components_Start = "Logs.Game.Server.RegisterComponents.Start";

        /// <summary>
        /// 游戏服务器热更新完成的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Game.Server.Hotfix.End
        /// 用途: 游戏服务器热更新加载完成时记录
        /// </remarks>
        public const string Game_Server_Hotfix_End = "Logs.Game.Server.Hotfix.End";

        /// <summary>
        /// 游戏服务器主循环启动的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Game.Server.MainLoop
        /// 用途: 游戏服务器主循环开始运行时记录
        /// </remarks>
        public const string Game_Server_Main_Loop = "Logs.Game.Server.MainLoop";

        /// <summary>
        /// 游戏服务器启动完成的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Game.Server.Start.Complete
        /// 用途: 游戏服务器完全启动并准备好接收连接时记录
        /// </remarks>
        public const string Game_Server_Start_Complete = "Logs.Game.Server.Start.Complete";

        /// <summary>
        /// 游戏服务器开始退出的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Game.Server.Exit.Start
        /// 用途: 游戏服务器开始关闭流程时记录
        /// </remarks>
        public const string Game_Server_Exit_Start = "Logs.Game.Server.Exit.Start";

        /// <summary>
        /// 游戏服务器成功退出的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Game.Server.Exit.Success
        /// 用途: 游戏服务器完全关闭时记录
        /// </remarks>
        public const string Game_Server_Exit_Success = "Logs.Game.Server.Exit.Success";

        /// <summary>
        /// 数据库初始化成功的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Database.Init.Success
        /// 用途: 数据库连接和初始化成功时记录
        /// 参数: {0} - 数据库连接字符串, {1} - 数据库名称
        /// </remarks>
        public const string Database_Init_Success = "Logs.Database.Init.Success";

        /// <summary>
        /// 客户端正在连接的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Client.Connecting
        /// 用途: 客户端开始连接到服务器时记录
        /// 参数: {0} - 客户端ID, {1} - 服务器地址
        /// </remarks>
        public const string Client_Connecting = "Logs.Client.Connecting";

        /// <summary>
        /// 客户端重试连接的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Client.RetryConnect
        /// 用途: 客户端连接失败后准备重试时记录
        /// 参数: {0} - 客户端ID, {1} - 重试次数, {2} - 最大重试次数
        /// </remarks>
        public const string Client_Retry_Connect = "Logs.Client.RetryConnect";

        /// <summary>
        /// 客户端达到最大重试次数的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Client.MaxRetryReached
        /// 用途: 客户端连接重试次数达到上限时记录
        /// 参数: {0} - 客户端ID, {1} - 最大重试次数
        /// </remarks>
        public const string Client_Max_Retry_Reached = "Logs.Client.MaxRetryReached";

        /// <summary>
        /// 客户端发生错误的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Client.ErrorOccurred
        /// 用途: 客户端操作过程中发生错误时记录
        /// 参数: {0} - 客户端ID, {1} - 错误信息
        /// </remarks>
        public const string Client_Error_Occurred = "Logs.Client.ErrorOccurred";

        /// <summary>
        /// 客户端断开连接的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Client.Disconnected
        /// 用途: 客户端与服务器断开连接时记录
        /// 参数: {0} - 客户端ID, {1} - 断开原因
        /// </remarks>
        public const string Client_Disconnected = "Logs.Client.Disconnected";

        /// <summary>
        /// 客户端连接成功的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Client.Connected.Success
        /// 用途: 客户端成功连接到服务器时记录
        /// 参数: {0} - 客户端ID, {1} - 服务器地址
        /// </remarks>
        public const string Client_Connected_Success = "Logs.Client.Connected.Success";

        /// <summary>
        /// 网络客户端断开连接的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Network.Client.Disconnect
        /// 用途: 网络层客户端断开连接时记录
        /// 参数: {0} - 连接ID
        /// </remarks>
        public const string Network_Client_Disconnect = "Logs.Network.Client.Disconnect";

        /// <summary>
        /// 网络客户端连接的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Network.Client.Connect
        /// 用途: 网络层客户端建立连接时记录
        /// 参数: {0} - 连接ID, {1} - 远程地址
        /// </remarks>
        public const string Network_Client_Connect = "Logs.Network.Client.Connect";

        /// <summary>
        /// 发现中心服务器异常的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Discovery.Server.Exception
        /// 用途: 发现中心服务器发生异常时记录
        /// 参数: {0} - 异常信息
        /// </remarks>
        public const string Discovery_Server_Exception = "Logs.Discovery.Server.Exception";

        /// <summary>
        /// 玩家在发现中心注册成功的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Discovery.Player.Register.Success
        /// 用途: 玩家成功注册到发现中心时记录
        /// 参数: {0} - 玩家ID, {1} - 服务类型
        /// </remarks>
        public const string Discovery_Player_Register_Success = "Logs.Discovery.Player.Register.Success";

        /// <summary>
        /// 玩家在发现中心注销成功的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Discovery.Player.Unregister.Success
        /// 用途: 玩家成功从发现中心注销时记录
        /// 参数: {0} - 玩家ID
        /// </remarks>
        public const string Discovery_Player_Unregister_Success = "Logs.Discovery.Player.Unregister.Success";

        /// <summary>
        /// 服务在发现中心注册成功的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Discovery.Service.Register.Success
        /// 用途: 服务成功注册到发现中心时记录
        /// 参数: {0} - 服务ID, {1} - 服务类型
        /// </remarks>
        public const string Discovery_Service_Register_Success = "Logs.Discovery.Service.Register.Success";

        /// <summary>
        /// 服务在发现中心注销成功的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Discovery.Service.Unregister.Success
        /// 用途: 服务成功从发现中心注销时记录
        /// 参数: {0} - 服务ID
        /// </remarks>
        public const string Discovery_Service_Unregister_Success = "Logs.Discovery.Service.Unregister.Success";

        /// <summary>
        /// 连接外部服务的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Discovery.ExternalService.Connect
        /// 用途: 成功连接到外部服务时记录
        /// 参数: {0} - 服务地址, {1} - 服务类型
        /// </remarks>
        public const string Discovery_External_Service_Connect = "Logs.Discovery.ExternalService.Connect";

        /// <summary>
        /// 断开外部服务连接的消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Discovery.ExternalService.Disconnect
        /// 用途: 与外部服务断开连接时记录
        /// 参数: {0} - 服务地址, {1} - 断开原因
        /// </remarks>
        public const string Discovery_External_Service_Disconnect = "Logs.Discovery.ExternalService.Disconnect";

        /// <summary>
        /// 控制台警告消息
        /// </summary>
        /// <remarks>
        /// 键名: Logs.Warning.ConsoleMessage
        /// 用途: 向控制台输出警告信息时使用
        /// 参数: {0} - 警告消息内容
        /// </remarks>
        public const string Warning_Console_Message = "Logs.Warning.ConsoleMessage";
    }
}