using System.Diagnostics;
using System.Text;
using GameFrameX.StartUp.Abstractions;
using GameFrameX.Utility.Extensions;
using GameFrameX.Utility.Log;
using GameFrameX.Utility.Setting;

namespace GameFrameX.StartUp;

/// <summary>
/// App入口
/// </summary>
internal static class AppEnter
{
    private static volatile bool _exitCalled;
    private static volatile Task _gameLoopTask;
    private static volatile Task _shutDownTask;
    private static volatile IAppStartUp _appStartUp;

    /// <summary>
    /// 启动
    /// </summary>
    /// <param name="appStartUp">启动对象</param>
    internal static async Task Entry(IAppStartUp appStartUp)
    {
        appStartUp.CheckNotNull(nameof(appStartUp));

        try
        {
            _appStartUp = appStartUp;
            AppExitHandler.Init(HandleExit, appStartUp.Setting);
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
                LogHelper.InfoConsole(error);
            }
            else
            {
                error = $"启动服务器失败 e:{e}";
                LogHelper.InfoConsole(error);
            }

            await File.WriteAllTextAsync($"server_error_{appStartUp.ServerType}.txt", $"{appStartUp.Setting.ToFormatString()}\n\n{error}", Encoding.UTF8);
        }
    }

    private static void HandleExit(string message)
    {
        if (_exitCalled)
        {
            return;
        }

        _exitCalled = true;
        LogHelper.InfoConsole("监听到退出程序消息");

        void Function()
        {
            GlobalSettings.IsAppRunning = false;
            _appStartUp.StopAsync(message);
            AppExitHandler.Kill();
            LogHelper.InfoConsole("退出程序");
            _gameLoopTask?.Wait();
            Process.GetCurrentProcess().Kill();
        }

        _shutDownTask = Task.Run(Function);
        _shutDownTask.Wait();
    }
}