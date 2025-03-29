// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！


using System.Diagnostics.Metrics;

namespace GameFrameX.Monitor.NetWork;

public static class MetricsNetWorkHelper
{
    private const string ModuleName = "network.";

    private static Counter<ulong> _totalBytesReceivedCounter;
    private static Counter<ulong> _totalBytesSentCounter;
    private static Counter<ulong> _totalPacketsReceivedCounter;
    private static Counter<ulong> _totalPacketsSentCounter;

    private static Counter<ulong> _connectionFailuresCounter;
    /*private static Counter<ulong> _connectionTimeoutsCounter;
    private static Counter<ulong> _packetErrorsCounter;
    private static Counter<ulong> _packetDroppedCounter;*/

    private static Counter<ulong> _totalConnectionsCounter;


    /// <summary>
    /// 总连接数
    /// </summary>
    public static Counter<ulong> TotalConnectionsCounter
    {
        get { return _totalConnectionsCounter ??= MetricsHelper.Meter.CreateCounter<ulong>($"{ModuleName}total_connections", "条", "总连接数"); }
    }

    private static Gauge<ulong> _averageTrafficGauge;

    /// <summary>
    /// 流量平均值(bytes/s)
    /// </summary>
    public static Gauge<ulong> AverageTrafficGauge
    {
        get { return _averageTrafficGauge ??= MetricsHelper.Meter.CreateGauge<ulong>($"{ModuleName}average_traffic", "Bytes", "流量平均值"); }
    }

    private static Gauge<ulong> _peakTrafficGauge;

    /// <summary>
    /// 流量峰值(bytes/s)
    /// </summary>
    public static Gauge<ulong> PeakTrafficGauge
    {
        get { return _peakTrafficGauge ??= MetricsHelper.Meter.CreateGauge<ulong>($"{ModuleName}peak_traffic", "Bytes", "流量峰值"); }
    }

    private static Counter<ulong> _totalTrafficCounter;

    /// <summary>
    /// 总流量(bytes)
    /// </summary>
    public static Counter<ulong> TotalTrafficCounter
    {
        get { return _totalTrafficCounter ??= MetricsHelper.Meter.CreateCounter<ulong>($"{ModuleName}total_traffic", "Bytes", "总流量"); }
    }

    /// <summary>
    /// 连接失败次数
    /// </summary>
    public static Counter<ulong> ConnectionFailuresCounter
    {
        get { return _connectionFailuresCounter ??= MetricsHelper.Meter.CreateCounter<ulong>($"{ModuleName}connection_failures", "次", "连接失败次数"); }
    }

    /*/// <summary>
    /// 连接超时次数
    /// </summary>
    public static Counter<ulong> ConnectionTimeoutsCounter
    {
        get { return _connectionTimeoutsCounter ??= MetricsHelper.Meter.CreateCounter<ulong>(ModuleName + "connection_timeouts", "连接超时次数"); }
    }

    /// <summary>
    /// 数据包错误数
    /// </summary>
    public static Counter<ulong> PacketErrorsCounter
    {
        get { return _packetErrorsCounter ??= MetricsHelper.Meter.CreateCounter<ulong>(ModuleName + "packet_errors", "数据包错误数"); }
    }

    /// <summary>
    /// 丢包数
    /// </summary>
    public static Counter<ulong> PacketDroppedCounter
    {
        get { return _packetDroppedCounter ??= MetricsHelper.Meter.CreateCounter<ulong>(ModuleName + "packet_dropped", "丢包数"); }
    }*/

    private static Gauge<float> _bandwidthUtilizationGauge;

    /// <summary>
    /// 带宽利用率(%)
    /// </summary>
    public static Gauge<float> BandwidthUtilizationGauge
    {
        get { return _bandwidthUtilizationGauge ??= MetricsHelper.Meter.CreateGauge<float>($"{ModuleName}bandwidth_utilization", "百分比", "带宽利用率"); }
    }

    private static Histogram<ulong> _packetSizeHistogram;

    /// <summary>
    /// 数据包大小分布
    /// </summary>
    public static Histogram<ulong> PacketSizeHistogram
    {
        get { return _packetSizeHistogram ??= MetricsHelper.Meter.CreateHistogram<ulong>($"{ModuleName}packet_size", "数据包大小分布"); }
    }

    /// <summary>
    /// 总接收数据包数量
    /// </summary>
    public static Counter<ulong> TotalPacketsReceivedCounter
    {
        get { return _totalPacketsReceivedCounter ??= MetricsHelper.Meter.CreateCounter<ulong>($"{ModuleName}total_packets_received", "个", "总接收数据包数量"); }
    }

    /// <summary>
    /// 总发送数据包数量
    /// </summary>
    public static Counter<ulong> TotalPacketsSentCounter
    {
        get { return _totalPacketsSentCounter ??= MetricsHelper.Meter.CreateCounter<ulong>($"{ModuleName}total_packets_sent", "个", "总发送数据包数量"); }
    }

    private static Gauge<ulong> _packetsReceivedPerMinuteGauge;

    /// <summary>
    /// 每分钟接收数据包数量
    /// </summary>
    public static Gauge<ulong> PacketsReceivedPerMinuteGauge
    {
        get { return _packetsReceivedPerMinuteGauge ??= MetricsHelper.Meter.CreateGauge<ulong>($"{ModuleName}packets_received_per_minute", "个", "每分钟接收数据包数量"); }
    }

    private static Gauge<ulong> _packetsSentPerMinuteGauge;

    /// <summary>
    /// 每分钟发送数据包数量
    /// </summary>
    public static Gauge<ulong> PacketsSentPerMinuteGauge
    {
        get { return _packetsSentPerMinuteGauge ??= MetricsHelper.Meter.CreateGauge<ulong>($"{ModuleName}packets_sent_per_minute", "个", "每分钟发送数据包数量"); }
    }

    /// <summary>
    /// 总接收字节数
    /// </summary>
    public static Counter<ulong> TotalBytesReceivedCounter
    {
        get { return _totalBytesReceivedCounter ??= MetricsHelper.Meter.CreateCounter<ulong>($"{ModuleName}total_bytes_received", "Byte", "总接收字节数"); }
    }

    /// <summary>
    /// 总发送字节数
    /// </summary>
    public static Counter<ulong> TotalBytesSentCounter
    {
        get { return _totalBytesSentCounter ??= MetricsHelper.Meter.CreateCounter<ulong>($"{ModuleName}total_bytes_sent", "Byte", "总发送字节数"); }
    }

    private static Gauge<ulong> _currentConnectionsGauge;

    /// <summary>
    /// 当前连接数
    /// </summary>
    public static Gauge<ulong> CurrentConnectionsGauge
    {
        get { return _currentConnectionsGauge ??= MetricsHelper.Meter.CreateGauge<ulong>($"{ModuleName}current_connections", "条", "当前连接数"); }
    }

    private static Gauge<ulong> _bytesReceivedPerSecondGauge;

    /// <summary>
    /// 每秒接收字节数
    /// </summary>
    public static Gauge<ulong> BytesReceivedPerSecondGauge
    {
        get { return _bytesReceivedPerSecondGauge ??= MetricsHelper.Meter.CreateGauge<ulong>($"{ModuleName}bytes_received_per_second", "Byte", "每秒接收字节数"); }
    }

    private static Gauge<ulong> _bytesSentPerSecondGauge;

    /// <summary>
    /// 每秒发送字节数
    /// </summary>
    public static Gauge<ulong> BytesSentPerSecondGauge
    {
        get { return _bytesSentPerSecondGauge ??= MetricsHelper.Meter.CreateGauge<ulong>($"{ModuleName}bytes_sent_per_second", "Byte", "每秒发送字节数"); }
    }
}