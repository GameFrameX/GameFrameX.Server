using Prometheus;

namespace GameFrameX.Monitor.Discovery;

public static class MetricsDiscoveryRegister
{
    private static Counter _serviceCounterOptions;

    /// <summary>
    /// 注册到发现中心的服务数量
    /// </summary>
    public static Counter ServiceCounterOptions
    {
        get { return _serviceCounterOptions ??= Prometheus.Metrics.CreateCounter("service_count", "注册到发现中心的服务数量"); }
    }
}