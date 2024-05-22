using SuperSocket.Server;
using SuperSocket.Server.Abstractions;
using Timer = System.Timers.Timer;

namespace GameFrameX.Launcher.StartUp;

public abstract class AppStartUpBase : IAppStartUp
{
    /// <summary>
    /// 服务器类型
    /// </summary>
    public ServerType ServerType { get; private set; }

    public AppSetting Setting { get; protected set; }

    /// <summary>
    /// 重连间隔时间。单位毫秒
    /// </summary>
    protected virtual int ReconnectInterval { get; } = 5000;

    /// <summary>
    /// 心跳间隔时间。单位毫秒
    /// </summary>
    protected virtual int HeartBeatInterval { get; } = 15000;


    protected readonly TaskCompletionSource<string> AppExitSource = new TaskCompletionSource<string>();

    /// <summary>
    /// 重连定时器
    /// </summary>
    protected Timer ReconnectionTimer;


    /// <summary>
    /// 心跳定时器
    /// </summary>
    protected Timer HeartBeatTimer { get; set; }

    public Task<string> AppExitToken => AppExitSource.Task;

    protected virtual void Init()
    {
    }

    public bool Init(ServerType serverType, BaseSetting setting, string[] args = null)
    {
        ServerType = serverType;
        Setting = (AppSetting)setting;
        Init();
        HeartBeatTimer = new Timer(HeartBeatInterval);
        HeartBeatTimer.Elapsed += HeartBeatTimerOnElapsed;

        ReconnectionTimer = new Timer(ReconnectInterval);
        ReconnectionTimer.Elapsed += ReconnectionTimerOnElapsed;
        return true;
    }

    /// <summary>
    /// 心跳定时器回调
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void HeartBeatTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
    }

    /// <summary>
    /// 重连定时器回调
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void ReconnectionTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
    }

    /// <summary>
    /// 配置启动
    /// </summary>
    /// <param name="options"></param>
    protected virtual void ConfigureSuperSocket(ServerOptions options)
    {
        options.AddListener(new ListenOptions { Ip = Setting.InnerIp.IsNullOrEmpty() ? IPAddress.Any.ToString() : Setting.InnerIp, Port = Setting.InnerPort });
    }

    public abstract Task EnterAsync();

    public virtual async Task Stop(string message = "")
    {
        LogHelper.Error(message);
        AppExitSource.TrySetResult(message);
        await Task.CompletedTask;
    }
}