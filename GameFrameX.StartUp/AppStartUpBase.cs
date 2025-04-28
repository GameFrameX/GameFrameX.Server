using System.Net;
using GameFrameX.Foundation.Logger;
using GameFrameX.StartUp.Abstractions;
using GameFrameX.SuperSocket.Server;
using GameFrameX.SuperSocket.Server.Abstractions;
using GameFrameX.Utility.Extensions;
using GameFrameX.Utility.Setting;

namespace GameFrameX.StartUp;

/// <summary>
/// 程序启动器基类
/// </summary>
public abstract partial class AppStartUpBase : IAppStartUp
{
    /// <summary>
    /// 应用退出
    /// </summary>
    protected readonly TaskCompletionSource<string> AppExitSource = new();

    /// <summary>
    /// 监听配置
    /// 当InnerIp为空时.将使用Any
    /// 当InnerPort小于1000时.将抛出异常
    /// 外部不要修改任何值
    /// </summary>
    public ListenOptions ListenOptions { get; protected set; }

    /// <summary>
    /// 服务器类型
    /// </summary>
    public ServerType ServerType { get; private set; }

    /// <summary>
    /// 配置信息
    /// </summary>
    public AppSetting Setting { get; protected set; }

    /// <summary>
    /// 应用退出
    /// </summary>
    public Task<string> AppExitToken
    {
        get { return AppExitSource.Task; }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="serverType">服务器类型</param>
    /// <param name="setting">配置信息对象</param>
    /// <param name="args">参数</param>
    /// <returns></returns>
    public bool Init(ServerType serverType, AppSetting setting, string[] args = null)
    {
        ServerType = serverType;
        Setting = setting;
        ListenOptions = new ListenOptions();
        Init();
        Setting.CheckNotNull(nameof(Setting));
        GlobalSettings.SaveIntervalInMilliSeconds = Setting.SaveDataInterval;
        GlobalSettings.ServerId = Setting.ServerId;
        GlobalSettings.IsDebug = Setting.IsDebug;
        return true;
    }

    /// <summary>
    /// 启动
    /// </summary>
    public abstract Task StartAsync();

    /// <summary>
    /// 终止服务器
    /// </summary>
    /// <param name="message">终止原因</param>
    public virtual async Task StopAsync(string message = "")
    {
        GlobalSettings.IsAppRunning = false;
        LogHelper.ErrorConsole($"服务器类型:{Setting.ServerType} 停止! 终止原因：{message}  配置信息: {Setting.ToFormatString()}");
        await StopServerAsync();
        AppExitSource?.TrySetResult(message);
        LogHelper.FlushAndSave();
        await Task.CompletedTask;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void Init()
    {
    }

    /// <summary>
    /// 配置启动,当InnerIP为空时.将使用Any
    /// </summary>
    /// <param name="options"></param>
    protected virtual void ConfigureSuperSocket(ServerOptions options)
    {
        if (Setting.InnerIp.IsNotNullOrWhiteSpace())
        {
            if (Setting.InnerPort <= 1000)
            {
                throw new ArgumentOutOfRangeException(nameof(Setting.InnerPort), $"InnerPort参数必须大于1000,当前值为{Setting.InnerPort}");
            }
        }

        ListenOptions.CheckNotNull(nameof(ListenOptions));
        ListenOptions.Ip = Setting.InnerIp.IsNullOrEmpty() ? IPAddress.Any.ToString() : Setting.InnerIp;
        ListenOptions.Port = Setting.InnerPort;
        options.AddListener(ListenOptions);
    }
}