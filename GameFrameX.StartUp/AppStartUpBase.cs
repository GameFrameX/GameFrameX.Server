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

using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.StartUp.Abstractions;
using GameFrameX.Utility.Setting;

namespace GameFrameX.StartUp;

/// <summary>
/// 应用程序启动基类。
/// </summary>
/// <remarks>
/// Application startup base class.
/// Provides basic functionality for application startup, including initialization, start, and stop core operations.
/// </remarks>
public abstract partial class AppStartUpBase : IAppStartUp
{
    /// <summary>
    /// 应用程序退出任务完成源。
    /// </summary>
    /// <remarks>
    /// Application exit task completion source.
    /// Used to asynchronously wait for application exit signals.
    /// </remarks>
    protected readonly TaskCompletionSource<string> AppExitSource = new();

    /// <summary>
    /// 获取服务器类型标识符。
    /// </summary>
    /// <remarks>
    /// Gets the server type identifier.
    /// Used to identify the type of the current server instance, such as game server, login server, etc.
    /// </remarks>
    /// <value>服务器的类型 / The type of the server</value>
    public string ServerType { get; private set; }

    /// <summary>
    /// 获取应用程序配置设置。
    /// </summary>
    /// <remarks>
    /// Gets the application configuration settings.
    /// Contains all configuration information required for server operation.
    /// </remarks>
    /// <value>当前应用程序设置 / The current application settings</value>
    public AppSetting Setting { get; protected set; }

    /// <summary>
    /// 获取应用程序退出令牌。
    /// </summary>
    /// <remarks>
    /// Gets the application exit token.
    /// Can be used to asynchronously wait for application exit signals.
    /// </remarks>
    /// <value>当应用程序应该退出时完成的任务 / Task that completes when the application should exit</value>
    public Task<string> AppExitToken
    {
        get { return AppExitSource.Task; }
    }

    /// <summary>
    /// 初始化应用程序启动。
    /// </summary>
    /// <remarks>
    /// Initialize the application startup.
    /// Sets the server type and configuration information, and calls the virtual Init method for subclass-specific initialization.
    /// </remarks>
    /// <param name="serverType">服务器类型标识符 / Server type identifier</param>
    /// <param name="setting">应用程序配置设置 / Application configuration settings</param>
    /// <param name="args">命令行参数 / Command line arguments</param>
    /// <returns>如果初始化成功则返回 <c>true</c>；否则返回 <c>false</c> / <c>true</c> if initialization succeeded; otherwise <c>false</c></returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="setting"/> 为 null 时抛出 / Thrown when <paramref name="setting"/> is null</exception>
    public bool Init(string serverType, AppSetting setting, string[] args = null)
    {
        ArgumentNullException.ThrowIfNull(setting, nameof(setting));
        ServerType = serverType;
        Setting = setting;
        Init();
        GlobalSettings.SetCurrentSetting(Setting);
        return true;
    }

    /// <summary>
    /// 异步启动应用程序。
    /// </summary>
    /// <remarks>
    /// Start the application asynchronously.
    /// Subclasses must implement this method to define specific startup logic.
    /// </remarks>
    /// <returns>表示异步启动操作的任务 / A task representing the asynchronous start operation</returns>
    public abstract Task StartAsync();

    /// <summary>
    /// 停止服务器。
    /// </summary>
    /// <remarks>
    /// Stop the server.
    /// Executes the server stop flow, including setting global state, logging, stopping the server, and flushing logs.
    /// </remarks>
    /// <param name="message">终止原因 / Termination reason</param>
    /// <returns>表示异步停止操作的任务 / A task representing the asynchronous stop operation</returns>
    public virtual async Task StopAsync(string message = "")
    {
        GlobalSettings.IsAppRunning = false;
        LogHelper.Error(LocalizationService.GetString(Localization.Keys.StartUp.ServerStopped, Setting.ServerType, message, Setting.ToFormatString()));
        await StopServerAsync();
        AppExitSource?.TrySetResult(message);
        LogHelper.FlushAndSave();
    }

    /// <summary>
    /// 初始化应用程序。
    /// </summary>
    /// <remarks>
    /// Initialize the application.
    /// Virtual method that subclasses can override to implement specific initialization logic.
    /// </remarks>
    protected virtual void Init()
    {
    }
}