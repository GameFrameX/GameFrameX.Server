using Prometheus;

namespace GameFrameX.Monitor.Discovery;

public static class MetricsDiscoveryRegister
{
    private static Gauge _serviceCounterOptions;

    /// <summary>
    /// 注册到发现中心的服务数量
    /// </summary>
    public static Gauge ServiceCounterOptions
    {
        get { return _serviceCounterOptions ??= Prometheus.Metrics.CreateGauge("service_count", "注册到发现中心的服务数量"); }
    }
}