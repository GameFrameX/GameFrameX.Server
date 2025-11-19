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
/// App入口
/// </summary>
/// <remarks>
/// 提供应用程序的统一入口和退出处理机制，负责管理应用程序的生命周期
/// </remarks>
internal static class AppEnter
{
    /// <summary>
    /// Indicates whether the exit method has been called / 指示是否已调用退出方法
    /// </summary>
    /// <value>
    /// true if exit has been called; otherwise, false / 如果已调用退出则为true，否则为false
    /// </value>
    private static volatile bool _exitCalled;
    
    /// <summary>
    /// The main game loop task / 主游戏循环任务
    /// </summary>
    /// <value>
    /// The task representing the game loop execution / 表示游戏循环执行的任务
    /// </value>
    private static volatile Task _gameLoopTask;
    
    /// <summary>
    /// The application startup instance / 应用程序启动实例
    /// </summary>
    /// <value>
    /// The startup instance used to manage application lifecycle / 用于管理应用程序生命周期的启动实例
    /// </value>
    private static volatile IAppStartUp _appStartUp;

    /// <summary>
    /// Application startup entry point / 应用程序启动入口点
    /// </summary>
    /// <param name="appStartUp">Application startup instance / 应用程序启动实例</param>
    /// <returns>A task representing the asynchronous operation / 表示异步操作的任务</returns>
    /// <exception cref="ArgumentNullException">Thrown when appStartUp is null / 当appStartUp为null时抛出</exception>
    /// <remarks>
    /// 初始化应用程序，设置退出处理器，并启动主游戏循环
    /// </remarks>
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
    /// Handles application exit / 处理应用程序退出
    /// </summary>
    /// <param name="message">Exit message / 退出消息</param>
    /// <remarks>
    /// 确保应用程序只退出一次，并执行必要的清理操作
    /// </remarks>
    private static void HandleExit(string message)
    {
        if (_exitCalled)
        {
            return;
        }

        _exitCalled = true;
        LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.Application.ListeningExitMessage));
        GlobalSettings.IsAppRunning = false;
        _appStartUp.StopAsync(message).Wait();
        AppExitHandler.Kill();
        LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.Application.ExecutingExitProcedure));
        _gameLoopTask?.Wait();
        LogHelper.FlushAndSave();
    }
}