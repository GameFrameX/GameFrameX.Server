using Prometheus;

namespace GameFrameX.Monitor.Account;

public static class MetricsAccountRegister
{
    static Counter _loginCounterOptions;

    /// <summary>
    /// 账号登录次数
    /// </summary>
    public static Counter LoginCounterOptions
    {
        get { return _loginCounterOptions ??= Prometheus.Metrics.CreateCounter("account_login", "账号登录次数"); }
    }

    static Counter _registerCounterOptions;

    /// <summary>
    /// 账号注册次数
    /// </summary>
    public static Counter RegisterCounterOptions
    {
        get { return _registerCounterOptions ??= Prometheus.Metrics.CreateCounter("account_register", "账号注册次数"); }
    }
}