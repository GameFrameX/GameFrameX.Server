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


using GameFrameX.Utility.Setting;

namespace GameFrameX.StartUp.Abstractions;

/// <summary>
/// 应用程序启动器接口定义。
/// </summary>
/// <remarks>
/// Application startup interface definition.
/// Defines the basic contract for application startup, stop, and configuration management.
/// </remarks>
public interface IAppStartUp
{
    /// <summary>
    /// 获取应用程序退出令牌。
    /// </summary>
    /// <remarks>
    /// Gets the application exit token.
    /// Used to monitor the application's exit status.
    /// </remarks>
    /// <value>表示应用程序退出令牌的任务 / A task that represents the application exit token</value>
    Task<string> AppExitToken { get; }

    /// <summary>
    /// 获取服务器类型。
    /// </summary>
    /// <remarks>
    /// Gets the server type.
    /// Server type identifier used to distinguish different types of server instances.
    /// </remarks>
    /// <value>服务器的类型标识符 / The type identifier of the server</value>
    string ServerType { get; }

    /// <summary>
    /// 获取应用程序配置设置。
    /// </summary>
    /// <remarks>
    /// Gets the application configuration settings.
    /// Configuration information containing various configuration parameters required for application operation.
    /// </remarks>
    /// <value>应用程序设置实例 / The application settings instance</value>
    AppSetting Setting { get; }

    /// <summary>
    /// 使用指定参数初始化应用程序启动器。
    /// </summary>
    /// <remarks>
    /// Initializes the application startup with specified parameters.
    /// Initializes the application startup, sets the server type, configuration information, and startup parameters.
    /// </remarks>
    /// <param name="serverType">服务器类型 / The type of server to initialize</param>
    /// <param name="setting">应用程序设置 / The application settings</param>
    /// <param name="args">启动参数 / The startup arguments</param>
    /// <returns>如果初始化成功则返回 <c>true</c>；否则返回 <c>false</c> / <c>true</c> if initialization is successful; otherwise <c>false</c></returns>
    bool Init(string serverType, AppSetting setting, string[] args);

    /// <summary>
    /// 异步启动应用程序。
    /// </summary>
    /// <remarks>
    /// Starts the application asynchronously.
    /// Starts the application asynchronously, performing all necessary initialization and service startup flows.
    /// </remarks>
    /// <returns>表示异步启动操作的任务 / A task that represents the asynchronous start operation</returns>
    Task StartAsync();

    /// <summary>
    /// 使用可选消息异步停止服务器。
    /// </summary>
    /// <remarks>
    /// Stops the server asynchronously with an optional message.
    /// Stops the server asynchronously, performing cleanup and resource release operations.
    /// </remarks>
    /// <param name="message">终止原因 / The reason for stopping the server</param>
    /// <returns>表示异步停止操作的任务 / A task that represents the asynchronous stop operation</returns>
    Task StopAsync(string message = "");
}
