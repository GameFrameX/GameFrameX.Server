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

using GameFrameX.Utility.Setting;

namespace GameFrameX.StartUp.Abstractions;

/// <summary>
/// Application startup interface definition
/// </summary>
/// <remarks>
/// 程序启动器基类接口定义，定义了应用程序启动、停止和配置管理的基本契约
/// </remarks>
public interface IAppStartUp
{
    /// <summary>
    /// Gets the application exit token
    /// </summary>
    /// <value>A task that represents the application exit token</value>
    /// <remarks>
    /// 应用退出令牌，用于监控应用程序的退出状态
    /// </remarks>
    Task<string> AppExitToken { get; }

    /// <summary>
    /// Gets the server type
    /// </summary>
    /// <value>The type identifier of the server</value>
    /// <remarks>
    /// 服务器类型标识符，用于区分不同类型的服务器实例
    /// </remarks>
    string ServerType { get; }

    /// <summary>
    /// Gets the application configuration settings
    /// </summary>
    /// <value>The application settings instance</value>
    /// <remarks>
    /// 配置信息，包含应用程序运行所需的各种配置参数
    /// </remarks>
    AppSetting Setting { get; }

    /// <summary>
    /// Initializes the application startup with specified parameters
    /// </summary>
    /// <param name="serverType">The type of server to initialize / 服务器类型</param>
    /// <param name="setting">The application settings / 启动设置</param>
    /// <param name="args">The startup arguments / 启动参数</param>
    /// <returns>True if initialization is successful; otherwise, false / 初始化成功返回true，否则返回false</returns>
    /// <remarks>
    /// 初始化应用程序启动器，设置服务器类型、配置信息和启动参数
    /// </remarks>
    bool Init(string serverType, AppSetting setting, string[] args);

    /// <summary>
    /// Starts the application asynchronously
    /// </summary>
    /// <returns>A task that represents the asynchronous start operation / 表示异步启动操作的任务</returns>
    /// <remarks>
    /// 异步启动应用程序，执行所有必要的初始化和服务启动流程
    /// </remarks>
    Task StartAsync();

    /// <summary>
    /// Stops the server asynchronously with an optional message
    /// </summary>
    /// <param name="message">The reason for stopping the server / 终止原因</param>
    /// <returns>A task that represents the asynchronous stop operation / 表示异步停止操作的任务</returns>
    /// <remarks>
    /// 异步终止服务器，执行清理和资源释放操作
    /// </remarks>
    Task StopAsync(string message = "");
}