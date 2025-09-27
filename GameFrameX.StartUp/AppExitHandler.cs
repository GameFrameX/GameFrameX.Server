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
using GameFrameX.StartUp.Abstractions;
using GameFrameX.Utility;
using GameFrameX.Foundation.Logger;
using GameFrameX.Utility.Setting;

namespace GameFrameX.StartUp;

/// <summary>
/// </summary>
internal static class AppExitHandler
{
    private static Action<string> _existCallBack;
    private static AppSetting _setting;
    private static PosixSignalRegistration _exitSignalRegistration;
    private static bool _isKill;
    private static readonly List<IFetalExceptionExitHandler> FetalExceptionExitHandlers = new();

    /// <summary>
    /// </summary>
    /// <param name="existCallBack">退出回调</param>
    /// <param name="setting">启动设置</param>
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

    private static void ExitSignalRegistrationHandler(PosixSignalContext posixSignalContext)
    {
        LogHelper.Info("PosixSignalRegistration SIGTERM....");
        _existCallBack?.Invoke("SIGTERM exit");
    }

    private static void DefaultOnUnloading(AssemblyLoadContext obj)
    {
        HandleFetalException("AssemblyLoadContext.Default.Unloading", obj.ToString());
    }

    /// <summary>
    /// 关闭程序
    /// </summary>
    public static void Kill()
    {
        _isKill = true;
    }

    /// <summary>
    /// 程序发生内部异常导致程序终止
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="e"></param>
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
        LogHelper.Error("get unhandled exception Tag:" + tag);
        if (e is IEnumerable arr)
        {
            var sb = new StringBuilder();
            foreach (var ex in arr)
            {
                sb.Append(ex);
            }

            LogHelper.Error($"Unhandled Exception:{sb}");
            _existCallBack?.Invoke("all Unhandled Exception:" + sb);
        }
        else
        {
            LogHelper.Error($"Unhandled Exception:{e}");
            _existCallBack?.Invoke($"Unhandled Exception:{e}");
        }
    }
}