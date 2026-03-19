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

using GameFrameX.StartUp.Abstractions;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Utility.Setting;

namespace GameFrameX.StartUp;

/// <summary>
/// 应用程序入口。
/// </summary>
/// <remarks>
/// App entry point.
/// Provides a unified entry and exit handling mechanism for the application, responsible for managing the application lifecycle.
/// </remarks>
internal static class AppEnter
{
    /// <summary>
    /// 指示是否已调用退出方法。
    /// </summary>
    /// <remarks>
    /// Indicates whether the exit method has been called.
    /// </remarks>
    /// <value>如果已调用退出则为 <c>true</c>；否则为 <c>false</c> / <c>true</c> if exit has been called; otherwise <c>false</c></value>
    private static volatile bool _exitCalled;

    /// <summary>
    /// 主游戏循环任务。
    /// </summary>
    /// <remarks>
    /// The main game loop task.
    /// </remarks>
    /// <value>表示游戏循环执行的任务 / The task representing the game loop execution</value>
    private static volatile Task _gameLoopTask;

    /// <summary>
    /// 应用程序启动实例。
    /// </summary>
    /// <remarks>
    /// The application startup instance.
    /// </remarks>
    /// <value>用于管理应用程序生命周期的启动实例 / The startup instance used to manage application lifecycle</value>
    private static volatile IAppStartUp _appStartUp;

    /// <summary>
    /// 应用程序启动入口点。
    /// </summary>
    /// <remarks>
    /// Application startup entry point.
    /// Initializes the application, sets up exit handlers, and starts the main game loop.
    /// </remarks>
    /// <param name="appStartUp">应用程序启动实例 / Application startup instance</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="appStartUp"/> 为 null 时抛出 / Thrown when <paramref name="appStartUp"/> is null</exception>
    internal static async Task Entry(IAppStartUp appStartUp)
    {
        ArgumentNullException.ThrowIfNull(appStartUp, nameof(appStartUp));

        try
        {
            _appStartUp = appStartUp;
            AppExitHandler.Init(HandleExit, appStartUp.Setting);
            _gameLoopTask = appStartUp.StartAsync();
            await _gameLoopTask;
        }
        catch (Exception e)
        {
            string error;
            if (GlobalSettings.IsAppRunning)
            {
                error = $"abnormal server runtime:{e}";
            }
            else
            {
                error = $"failed to start the server:{e}";
            }

            LogHelper.Error(error);
        }
    }

    /// <summary>
    /// 处理应用程序退出。
    /// </summary>
    /// <remarks>
    /// Handles application exit.
    /// Ensures the application only exits once and performs necessary cleanup operations.
    /// </remarks>
    /// <param name="message">退出消息 / Exit message</param>
    private static void HandleExit(string message)
    {
        if (_exitCalled)
        {
            return;
        }

        _exitCalled = true;
        LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.Application.ListeningExitMessage));
        GlobalSettings.IsAppRunning = false;
        _appStartUp.StopAsync(message).Wait();
        AppExitHandler.Kill();
        LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.Application.ExecutingExitProcedure));
        _gameLoopTask?.Wait();
        LogHelper.FlushAndSave();
    }
}