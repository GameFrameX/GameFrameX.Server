using System.Diagnostics.Metrics;

namespace GameFrameX.Monitor.Account;

/// <summary>
/// 账号监控帮助类
/// </summary>
public static class MetricsAccountHelper
{
    private static Counter<ulong> _loginCounterOptions;

    private static Counter<ulong> _registerCounterOptions;
    private const string ModuleName = "account.";

    /// <summary>
    /// 账号登录次数
    /// </summary>
    public static Counter<ulong> LoginCounterOptions
    {
        get { return _loginCounterOptions ??= MetricsHelper.Meter.CreateCounter<ulong>($"{ModuleName}login", "次", "账号登录次数"); }
    }

    /// <summary>
    /// 账号注册次数
    /// </summary>
    public static Counter<ulong> RegisterCounterOptions
    {
        get { return _registerCounterOptions ??= MetricsHelper.Meter.CreateCounter<ulong>($"{ModuleName}register", "次", "账号注册次数"); }
    }
}