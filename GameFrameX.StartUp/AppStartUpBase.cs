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
using GameFrameX.StartUp.Abstractions;
using GameFrameX.Utility.Setting;

namespace GameFrameX.StartUp;

/// <summary>
/// Application startup base class / 应用程序启动基类
/// </summary>
/// <remarks>
/// 提供应用程序启动的基础功能，包括初始化、启动和停止等核心操作
/// </remarks>
public abstract partial class AppStartUpBase : IAppStartUp
{
    /// <summary>
    /// Application exit task completion source / 应用程序退出任务完成源
    /// </summary>
    /// <value>
    /// Task completion source for handling application exit / 用于处理应用程序退出的任务完成源
    /// </value>
    /// <remarks>
    /// 用于异步等待应用程序退出信号
    /// </remarks>
    protected readonly TaskCompletionSource<string> AppExitSource = new();

    /// <summary>
    /// Server type identifier / 服务器类型标识符
    /// </summary>
    /// <value>
    /// The type of the server / 服务器的类型
    /// </value>
    /// <remarks>
    /// 用于标识当前服务器实例的类型，如游戏服务器、登录服务器等
    /// </remarks>
    public string ServerType { get; private set; }

    /// <summary>
    /// Application configuration settings / 应用程序配置设置
    /// </summary>
    /// <value>
    /// The current application settings / 当前应用程序设置
    /// </value>
    /// <remarks>
    /// 包含服务器运行所需的所有配置信息
    /// </remarks>
    public AppSetting Setting { get; protected set; }

    /// <summary>
    /// Application exit token / 应用程序退出令牌
    /// </summary>
    /// <value>
    /// Task that completes when the application should exit / 当应用程序应该退出时完成的任务
    /// </value>
    /// <remarks>
    /// 可用于异步等待应用程序退出信号
    /// </remarks>
    public Task<string> AppExitToken
    {
        get { return AppExitSource.Task; }
    }

    /// <summary>
    /// Initialize the application startup / 初始化应用程序启动
    /// </summary>
    /// <param name="serverType">Server type identifier / 服务器类型标识符</param>
    /// <param name="setting">Application configuration settings / 应用程序配置设置</param>
    /// <param name="args">Command line arguments / 命令行参数</param>
    /// <returns>true if initialization succeeded; otherwise, false / 如果初始化成功则返回true，否则返回false</returns>
    /// <exception cref="ArgumentNullException">Thrown when setting is null / 当setting为null时抛出</exception>
    /// <remarks>
    /// 设置服务器类型和配置信息，并调用虚拟的Init方法进行子类特定的初始化
    /// </remarks>
    public bool Init(string serverType, AppSetting setting, string[] args = null)
    {
        ServerType = serverType;
        Setting = setting;
        Init();
        ArgumentNullException.ThrowIfNull(Setting, nameof(Setting));
        GlobalSettings.SetCurrentSetting(Setting);
        return true;
    }

    /// <summary>
    /// Start the application asynchronously / 异步启动应用程序
    /// </summary>
    /// <returns>A task representing the asynchronous start operation / 表示异步启动操作的任务</returns>
    /// <remarks>
    /// 子类必须实现此方法以定义具体的启动逻辑
    /// </remarks>
    public abstract Task StartAsync();

    /// <summary>
    /// Stop the server / 停止服务器
    /// </summary>
    /// <param name="message">Termination reason / 终止原因</param>
    /// <returns>A task representing the asynchronous stop operation / 表示异步停止操作的任务</returns>
    /// <remarks>
    /// 执行服务器停止流程，包括设置全局状态、记录日志、停止服务器和刷新日志
    /// </remarks>
    public virtual async Task StopAsync(string message = "")
    {
        GlobalSettings.IsAppRunning = false;
        LogHelper.Error($"服务器类型:{Setting.ServerType} 停止! 终止原因：{message}  配置信息: {Setting.ToFormatString()}");
        await StopServerAsync();
        AppExitSource?.TrySetResult(message);
        LogHelper.FlushAndSave();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Initialize the application / 初始化应用程序
    /// </summary>
    /// <remarks>
    /// 虚拟方法，子类可以重写以实现特定的初始化逻辑
    /// </remarks>
    protected virtual void Init()
    {
    }
}