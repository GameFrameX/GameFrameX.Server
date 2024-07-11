using System.Diagnostics;
using System.Text;
using GameFrameX.Log;
using GameFrameX.Setting;
using GameFrameX.StartUp.Abstractions;

namespace GameFrameX.StartUp
{
    /// <summary>
    /// App入口
    /// </summary>
    public static class AppEnter
    {
        private static volatile bool        _exitCalled   = false;
        private static volatile Task        _gameLoopTask = null;
        private static volatile Task        _shutDownTask = null;
        private static volatile IAppStartUp _appStartUp;

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="appStartUp">启动对象</param>
        public static async Task Entry(IAppStartUp appStartUp)
        {
            try
            {
                _appStartUp = appStartUp;
                AppExitHandler.Init(HandleExit);
                _gameLoopTask = appStartUp.StartAsync();
                await _gameLoopTask;
                if (_shutDownTask != null)
                {
                    await _shutDownTask;
                }
            }
            catch (Exception e)
            {
                string error;
                if (GlobalSettings.IsAppRunning)
                {
                    error = $"服务器运行时异常 e:{e}";
                    LogHelper.Info(error);
                }
                else
                {
                    error = $"启动服务器失败 e:{e}";
                    LogHelper.Info(error);
                }

                await File.WriteAllTextAsync("server_error.txt", $"{error}", Encoding.UTF8);
            }
        }

        private static void HandleExit(string message)
        {
            if (_exitCalled)
            {
                return;
            }

            _exitCalled = true;
            LogHelper.Info($"监听到退出程序消息");

            void Function()
            {
                GlobalSettings.IsAppRunning = false;
                _appStartUp.StopAsync(message);
                AppExitHandler.Kill();
                LogHelper.Info($"退出程序");
                _gameLoopTask?.Wait();
                Process.GetCurrentProcess().Kill();
            }

            _shutDownTask = Task.Run(Function);
            _shutDownTask.Wait();
        }
    }
}