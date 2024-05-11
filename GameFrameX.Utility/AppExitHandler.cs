using System.Collections;
using System.Text;
using GameFrameX.Log;

namespace GameFrameX.Utility
{
    public static class AppExitHandler
    {
        private static Action<string> _existCallBack;

        public static void Init(Action<string> existCallBack)
        {
            _existCallBack = existCallBack;
            //退出监听
            AppDomain.CurrentDomain.ProcessExit += (s, e) => { _existCallBack?.Invoke("process exit"); };
            //Fetal异常监听
            AppDomain.CurrentDomain.UnhandledException += (s, e) => { HandleFetalException(e.ExceptionObject); };
            //ctrl+c
            Console.CancelKeyPress += (s, e) => { _existCallBack?.Invoke("ctrl+c exit"); };
        }

        /// <summary>
        /// 程序发生内部异常导致程序终止
        /// </summary>
        /// <param name="e"></param>
        private static void HandleFetalException(object e)
        {
            //这里可以发送短信或者钉钉消息通知到运维
            LogHelper.Error("get unhandled exception");
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