using Prometheus;

namespace GameFrameX.Monitor.Account;

public static class MetricsAccountRegister
{
    static Counter _loginCounterOptions;

    public static Counter LoginCounterOptions
    {
        get { return _loginCounterOptions ??= Prometheus.Metrics.CreateCounter("account_login", "Account"); }
    }

    static Counter _registerCounterOptions;

    public static Counter RegisterCounterOptions
    {
        get { return _registerCounterOptions ??= Prometheus.Metrics.CreateCounter("account_register", "Account"); }
    }
}