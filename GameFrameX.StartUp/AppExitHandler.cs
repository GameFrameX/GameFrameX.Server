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
/// Application exit handler / 应用程序退出处理器
/// </summary>
/// <remarks>
/// 负责处理应用程序的各种退出信号和异常情况，确保程序能够优雅地关闭
/// </remarks>
internal static class AppExitHandler
{
    /// <summary>
    /// Exit callback action / 退出回调动作
    /// </summary>
    /// <value>
    /// The action to be called when application exits / 应用程序退出时要调用的动作
    /// </value>
    private static Action<string> _existCallBack;

    /// <summary>
    /// Application settings / 应用程序设置
    /// </summary>
    /// <value>
    /// The current application settings / 当前应用程序设置
    /// </value>
    private static AppSetting _setting;

    /// <summary>
    /// POSIX signal registration for handling SIGTERM / 用于处理SIGTERM的POSIX信号注册
    /// </summary>
    /// <value>
    /// The registration for POSIX signal handling / POSIX信号处理的注册
    /// </value>
    private static PosixSignalRegistration _exitSignalRegistration;

    /// <summary>
    /// Indicates whether the application is being killed / 指示应用程序是否正在被终止
    /// </summary>
    /// <value>
    /// true if the application is being killed; otherwise, false / 如果应用程序正在被终止则为true，否则为false
    /// </value>
    private static bool _isKill;

    /// <summary>
    /// List of fatal exception exit handlers / 致命异常退出处理器列表
    /// </summary>
    /// <value>
    /// Collection of handlers for fatal exceptions / 致命异常处理器集合
    /// </value>
    private static readonly List<IFetalExceptionExitHandler> FetalExceptionExitHandlers = new();

    /// <summary>
    /// Initialize the exit handler / 初始化退出处理器
    /// </summary>
    /// <param name="existCallBack">Exit callback action / 退出回调动作</param>
    /// <param name="setting">Application settings / 应用程序设置</param>
    /// <remarks>
    /// 设置各种退出信号监听器和异常处理器，确保应用程序能够响应各种退出条件
    /// </remarks>
    public static void Init(Action<string> existCallBack, AppSetting setting)
    {
        _isKill = false;
        _setting = setting;
        _existCallBack = existCallBack;
        var fetalExceptionExitHandlers = AssemblyHelper.GetRuntimeImplementTypeNames<IFetalExceptionExitHandler>();
        foreach (var exceptionExitHandler in fetalExceptionExitHandlers)
        {
            var handler = (IFetalExceptionExitHandler)Activator.CreateInstance(exceptionExitHandler);
            FetalExceptionExitHandlers.Add(handler);
        }

        _exitSignalRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, ExitSignalRegistrationHandler);
        //退出监听
        AppDomain.CurrentDomain.ProcessExit += (s, e) => { _existCallBack?.Invoke("process exit"); };
        //卸载监听
        AssemblyLoadContext.Default.Unloading += DefaultOnUnloading;
        //Fetal异常监听
        AppDomain.CurrentDomain.UnhandledException += (s, e) => { HandleFetalException("AppDomain.CurrentDomain.UnhandledException", e.ExceptionObject); };
        //Task异常监听
        TaskScheduler.UnobservedTaskException += (s, e) => { HandleFetalException("TaskScheduler.UnobservedTaskException", e.Exception); };
        //ctrl+c
        Console.CancelKeyPress += (s, e) => { _existCallBack?.Invoke("ctrl+c exit"); };
    }

    /// <summary>
    /// Handles POSIX signal registration / 处理POSIX信号注册
    /// </summary>
    /// <param name="posixSignalContext">POSIX signal context / POSIX信号上下文</param>
    /// <remarks>
    /// 响应SIGTERM信号，触发应用程序退出流程
    /// </remarks>
    private static void ExitSignalRegistrationHandler(PosixSignalContext posixSignalContext)
    {
        LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.Application.SigtermSignalReceived));
        _existCallBack?.Invoke("SIGTERM exit");
    }

    /// <summary>
    /// Handles assembly load context unloading / 处理程序集加载上下文卸载
    /// </summary>
    /// <param name="obj">Assembly load context / 程序集加载上下文</param>
    /// <remarks>
    /// 当程序集加载上下文卸载时触发，作为致命异常处理
    /// </remarks>
    private static void DefaultOnUnloading(AssemblyLoadContext obj)
    {
        HandleFetalException("AssemblyLoadContext.Default.Unloading", obj.ToString());
    }

    /// <summary>
    /// Kill the application / 终止应用程序
    /// </summary>
    /// <remarks>
    /// 设置终止标志，防止进一步的异常处理
    /// </remarks>
    public static void Kill()
    {
        _isKill = true;
    }

    /// <summary>
    /// Handle fatal exceptions that cause application termination / 处理导致应用程序终止的致命异常
    /// </summary>
    /// <param name="tag">Exception tag / 异常标签</param>
    /// <param name="e">Exception object / 异常对象</param>
    /// <remarks>
    /// 处理未处理的异常，记录日志并通知相关处理器，然后触发应用程序退出
    /// </remarks>
    private static void HandleFetalException(string tag, object e)
    {
        if (_isKill)
        {
            return;
        }

        if (FetalExceptionExitHandlers?.Count > 0)
        {
            foreach (var fetalExceptionExitHandler in FetalExceptionExitHandlers)
            {
                fetalExceptionExitHandler.Run(tag, _setting, e?.ToString());
            }
        }

        //这里可以发送短信或者钉钉消息通知到运维
        LogHelper.Error(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.Application.GetUnhandledException, tag));
        if (e is IEnumerable arr)
        {
            var sb = new StringBuilder();
            foreach (var ex in arr)
            {
                sb.Append(ex);
            }

            LogHelper.Error(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.Application.AllUnhandledExceptions, sb.ToString()));
            _existCallBack?.Invoke(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.Application.AllUnhandledExceptions, sb.ToString()));
        }
        else
        {
            LogHelper.Error(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.Application.UnhandledException, e?.ToString() ?? "Unknown exception"));
            _existCallBack?.Invoke(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.Application.UnhandledExceptionCallback, e?.ToString() ?? "Unknown exception"));
        }
    }
}