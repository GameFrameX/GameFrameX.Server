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

using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.StartUp.Abstractions;
using GameFrameX.Utility;
using GameFrameX.Foundation.Logger;
using GameFrameX.Utility.Setting;

namespace GameFrameX.StartUp;

/// <summary>
/// 应用程序退出处理器。
/// </summary>
/// <remarks>
/// Application exit handler.
/// Responsible for handling various exit signals and exceptions of the application, ensuring the program can shut down gracefully.
/// </remarks>
internal static class AppExitHandler
{
    /// <summary>
    /// 退出回调动作。
    /// </summary>
    /// <remarks>
    /// Exit callback action.
    /// </remarks>
    /// <value>应用程序退出时要调用的动作 / The action to be called when application exits</value>
    private static Action<string> _exitCallBack;

    /// <summary>
    /// 应用程序设置。
    /// </summary>
    /// <remarks>
    /// Application settings.
    /// </remarks>
    /// <value>当前应用程序设置 / The current application settings</value>
    private static AppSetting _setting;

    /// <summary>
    /// 用于处理 SIGTERM 的 POSIX 信号注册。
    /// </summary>
    /// <remarks>
    /// POSIX signal registration for handling SIGTERM.
    /// </remarks>
    /// <value>POSIX 信号处理的注册 / The registration for POSIX signal handling</value>
    private static PosixSignalRegistration _exitSignalRegistration;

    /// <summary>
    /// 指示应用程序是否正在被终止。
    /// </summary>
    /// <remarks>
    /// Indicates whether the application is being killed.
    /// </remarks>
    /// <value>如果应用程序正在被终止则为 <c>true</c>；否则为 <c>false</c> / <c>true</c> if the application is being killed; otherwise <c>false</c></value>
    private static bool _isKill;

    /// <summary>
    /// 致命异常退出处理器列表。
    /// </summary>
    /// <remarks>
    /// List of fatal exception exit handlers.
    /// </remarks>
    /// <value>致命异常处理器集合 / Collection of handlers for fatal exceptions</value>
    private static readonly List<IFatalExceptionExitHandler> FatalExceptionExitHandlers = new();

    /// <summary>
    /// 初始化退出处理器。
    /// </summary>
    /// <remarks>
    /// Initialize the exit handler.
    /// Sets up various exit signal listeners and exception handlers to ensure the application can respond to various exit conditions.
    /// </remarks>
    /// <param name="exitCallBack">退出回调动作 / Exit callback action</param>
    /// <param name="setting">应用程序设置 / Application settings</param>
    public static void Init(Action<string> exitCallBack, AppSetting setting)
    {
        _isKill = false;
        _setting = setting;
        _exitCallBack = exitCallBack;
        var fatalExceptionExitHandlers = AssemblyHelper.GetRuntimeImplementTypeNames<IFatalExceptionExitHandler>();
        foreach (var exceptionExitHandler in fatalExceptionExitHandlers)
        {
            var handler = (IFatalExceptionExitHandler)Activator.CreateInstance(exceptionExitHandler);
            FatalExceptionExitHandlers.Add(handler);
        }

        _exitSignalRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, ExitSignalRegistrationHandler);
        //退出监听
        AppDomain.CurrentDomain.ProcessExit += (s, e) => { _exitCallBack?.Invoke("process exit"); };
        //卸载监听
        AssemblyLoadContext.Default.Unloading += DefaultOnUnloading;
        //Fatal异常监听
        AppDomain.CurrentDomain.UnhandledException += (s, e) => { HandleFatalException("AppDomain.CurrentDomain.UnhandledException", e.ExceptionObject); };
        //Task异常监听
        TaskScheduler.UnobservedTaskException += (s, e) => { HandleFatalException("TaskScheduler.UnobservedTaskException", e.Exception); };
        //ctrl+c
        Console.CancelKeyPress += (s, e) => { _exitCallBack?.Invoke("ctrl+c exit"); };
    }

    /// <summary>
    /// 处理 POSIX 信号注册。
    /// </summary>
    /// <remarks>
    /// Handles POSIX signal registration.
    /// Responds to SIGTERM signal and triggers the application exit flow.
    /// </remarks>
    /// <param name="posixSignalContext">POSIX 信号上下文 / POSIX signal context</param>
    private static void ExitSignalRegistrationHandler(PosixSignalContext posixSignalContext)
    {
        LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.Application.SigtermSignalReceived));
        _exitCallBack?.Invoke("SIGTERM exit");
    }

    /// <summary>
    /// 处理程序集加载上下文卸载。
    /// </summary>
    /// <remarks>
    /// Handles assembly load context unloading.
    /// Triggered when the assembly load context is unloaded, treated as a fatal exception.
    /// </remarks>
    /// <param name="obj">程序集加载上下文 / Assembly load context</param>
    private static void DefaultOnUnloading(AssemblyLoadContext obj)
    {
        HandleFatalException("AssemblyLoadContext.Default.Unloading", obj.ToString());
    }

    /// <summary>
    /// 终止应用程序。
    /// </summary>
    /// <remarks>
    /// Kill the application.
    /// Sets the termination flag to prevent further exception handling.
    /// </remarks>
    public static void Kill()
    {
        _isKill = true;
    }

    /// <summary>
    /// 处理导致应用程序终止的致命异常。
    /// </summary>
    /// <remarks>
    /// Handle fatal exceptions that cause application termination.
    /// Handles unhandled exceptions, logs them, notifies relevant handlers, and triggers application exit.
    /// </remarks>
    /// <param name="tag">异常标签 / Exception tag</param>
    /// <param name="e">异常对象 / Exception object</param>
    private static void HandleFatalException(string tag, object e)
    {
        if (_isKill)
        {
            return;
        }

        if (FatalExceptionExitHandlers?.Count > 0)
        {
            foreach (var fatalExceptionExitHandler in FatalExceptionExitHandlers)
            {
                fatalExceptionExitHandler.Run(tag, _setting, e?.ToString());
            }
        }

        //这里可以发送短信或者钉钉消息通知到运维
        LogHelper.Error(LocalizationService.GetString(Localization.Keys.StartUp.Application.GetUnhandledException, tag));
        if (e is IEnumerable arr)
        {
            var sb = new StringBuilder();
            foreach (var ex in arr)
            {
                sb.Append(ex);
            }

            LogHelper.Error(LocalizationService.GetString(Localization.Keys.StartUp.Application.AllUnhandledExceptions, sb.ToString()));
            _exitCallBack?.Invoke(LocalizationService.GetString(Localization.Keys.StartUp.Application.AllUnhandledExceptions, sb.ToString()));
        }
        else
        {
            LogHelper.Error(LocalizationService.GetString(Localization.Keys.StartUp.Application.UnhandledException, e?.ToString() ?? "Unknown exception"));
            _exitCallBack?.Invoke(LocalizationService.GetString(Localization.Keys.StartUp.Application.UnhandledExceptionCallback, e?.ToString() ?? "Unknown exception"));
        }
    }
}