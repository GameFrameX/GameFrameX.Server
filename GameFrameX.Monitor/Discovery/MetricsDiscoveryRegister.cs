using Prometheus;

namespace GameFrameX.Monitor.Discovery;

public static class MetricsDiscoveryRegister
{
    private static Counter _serviceCounterOptions;

    public static Counter ServiceCounterOptions
    {
        get { return _serviceCounterOptions ??= Prometheus.Metrics.CreateCounter("service_count", "注册到发现中心的服务数量"); }
    }
}