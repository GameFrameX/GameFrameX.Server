using GameFrameX.Utility.Setting;

namespace GameFrameX.Launcher.StartUp;

public abstract class AppStartUpService : AppStartUpBase
{
    /// <summary>
    /// 启动服务器
    /// </summary>
    /// <returns></returns>
    public override Task StartAsync()
    {
        GlobalSettings.IsAppRunning = true;
        return Task.CompletedTask;
    }
}