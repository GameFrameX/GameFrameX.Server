using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using GameFrameX.Log;
using GameFrameX.Setting;
using GameFrameX.StartUp.Abstractions;
using GameFrameX.Utility;

namespace GameFrameX.StartUp
{
    /// <summary>
    /// 
    /// </summary>
    internal static class AppExitHandler
    {
        private static Action<string> _existCallBack;
        private static AppSetting _setting;
        private static PosixSignalRegistration _exitSignalRegistration;
        private static bool _isKill = false;
        private static readonly List<IFetalExceptionExitHandler> FetalExceptionExitHandlers = new List<IFetalExceptionExitHandler>();

        /// <summary>
        /// 
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
                int line = 0;
                foreach (var ex in arr)
                {
                    sb.AppendLine($"Unhandled Exception:{line++}:{ex}");
                    LogHelper.Error($"Unhandled Exception:{ex}");
                }

                _existCallBack?.Invoke("all Unhandled Exception:" + sb);
            }
            else
            {
                LogHelper.Error($"Unhandled Exception:{e}");
                _existCallBack?.Invoke($"Unhandled Exception:{e}");
            }
        }
    }
}