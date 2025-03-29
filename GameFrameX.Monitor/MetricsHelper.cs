using System.Diagnostics.Metrics;

namespace GameFrameX.Monitor;

/// <summary>
/// 监控帮助类
/// </summary>
public static class MetricsHelper
{
    /// <summary>
    /// 监控
    /// </summary>
    public static Meter Meter { get; private set; }

    /// <summary>
    /// 是否已启动
    /// </summary>
    public static bool IsStarted
    {
        get { return Meter != null; }
    }

    /// <summary>
    /// 启动监控
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="version">版本</param>
    /// <param name="port">对外访问的端口</param>
    public static void Start(string name = "GameFrameX", string version = "1.0.0", int port = 0)
    {
        Meter = new Meter(name, version);
        Console.WriteLine($"Open http://localhost:{port}/metrics?accept=application/openmetrics-text in a web browser.");
    }
}