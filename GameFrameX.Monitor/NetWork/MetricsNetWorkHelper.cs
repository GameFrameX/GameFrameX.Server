// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Prometheus;

namespace GameFrameX.Monitor.NetWork;

public static class MetricsNetWorkHelper
{
    private static Counter _totalBytesReceivedCounter;
    private static Counter _totalBytesSentCounter;
    private static Gauge _currentConnectionsGauge;
    private static Gauge _bytesReceivedPerSecondGauge;
    private static Gauge _bytesSentPerSecondGauge;
    private static Gauge _networkLatencyGauge;
    private static Counter _totalPacketsReceivedCounter;
    private static Counter _totalPacketsSentCounter;
    private static Gauge _packetsReceivedPerMinuteGauge;
    private static Gauge _packetsSentPerMinuteGauge;
    private static Counter _connectionFailuresCounter;
    private static Counter _connectionTimeoutsCounter;
    private static Counter _packetErrorsCounter;
    private static Counter _packetDroppedCounter;
    private static Gauge _connectionQualityGauge;
    private static Gauge _bandwidthUtilizationGauge;
    private static Histogram _packetSizeHistogram;
    private static Gauge _retransmissionRateGauge;
    private static Counter _tcpResetCounter;
    private static Counter _dnsFailuresCounter;
    private static Gauge _socketBacklogGauge;
    private static Counter _tlsHandshakeFailuresCounter;
    private static Gauge _ipv4ConnectionsGauge;
    private static Gauge _ipv6ConnectionsGauge;
    private static Counter _udpErrorsCounter;
    private static Counter _totalConnectionsCounter;
    private static Gauge _averageTrafficGauge;
    private static Gauge _peakTrafficGauge;
    private static Counter _totalTrafficCounter;

    /// <summary>
    /// 总连接数
    /// </summary>
    public static Counter TotalConnectionsCounter
    {
        get { return _totalConnectionsCounter ??= Metrics.CreateCounter("total_connections", "总连接数"); }
    }

    /// <summary>
    /// 流量平均值(bytes/s)
    /// </summary>
    public static Gauge AverageTrafficGauge
    {
        get { return _averageTrafficGauge ??= Metrics.CreateGauge("average_traffic", "流量平均值"); }
    }

    /// <summary>
    /// 流量峰值(bytes/s)
    /// </summary>
    public static Gauge PeakTrafficGauge
    {
        get { return _peakTrafficGauge ??= Metrics.CreateGauge("peak_traffic", "流量峰值"); }
    }

    /// <summary>
    /// 总流量(bytes)
    /// </summary>
    public static Counter TotalTrafficCounter
    {
        get { return _totalTrafficCounter ??= Metrics.CreateCounter("total_traffic", "总流量"); }
    }

    /// <summary>
    /// 连接失败次数
    /// </summary>
    public static Counter ConnectionFailuresCounter
    {
        get { return _connectionFailuresCounter ??= Metrics.CreateCounter("connection_failures", "连接失败次数"); }
    }

    /// <summary>
    /// 连接超时次数
    /// </summary>
    public static Counter ConnectionTimeoutsCounter
    {
        get { return _connectionTimeoutsCounter ??= Metrics.CreateCounter("connection_timeouts", "连接超时次数"); }
    }

    /// <summary>
    /// 数据包错误数
    /// </summary>
    public static Counter PacketErrorsCounter
    {
        get { return _packetErrorsCounter ??= Metrics.CreateCounter("packet_errors", "数据包错误数"); }
    }

    /// <summary>
    /// 丢包数
    /// </summary>
    public static Counter PacketDroppedCounter
    {
        get { return _packetDroppedCounter ??= Metrics.CreateCounter("packet_dropped", "丢包数"); }
    }

    /// <summary>
    /// 连接质量指标(0-100)
    /// </summary>
    public static Gauge ConnectionQualityGauge
    {
        get { return _connectionQualityGauge ??= Metrics.CreateGauge("connection_quality", "连接质量指标"); }
    }

    /// <summary>
    /// 带宽利用率(%)
    /// </summary>
    public static Gauge BandwidthUtilizationGauge
    {
        get { return _bandwidthUtilizationGauge ??= Metrics.CreateGauge("bandwidth_utilization", "带宽利用率"); }
    }

    /// <summary>
    /// 数据包大小分布
    /// </summary>
    public static Histogram PacketSizeHistogram
    {
        get { return _packetSizeHistogram ??= Metrics.CreateHistogram("packet_size", "数据包大小分布"); }
    }

    /// <summary>
    /// 重传率(%)
    /// </summary>
    public static Gauge RetransmissionRateGauge
    {
        get { return _retransmissionRateGauge ??= Metrics.CreateGauge("retransmission_rate", "重传率"); }
    }

    /// <summary>
    /// TCP重置次数
    /// </summary>
    public static Counter TcpResetCounter
    {
        get { return _tcpResetCounter ??= Metrics.CreateCounter("tcp_reset", "TCP重置次数"); }
    }

    /// <summary>
    /// DNS解析失败次数
    /// </summary>
    public static Counter DnsFailuresCounter
    {
        get { return _dnsFailuresCounter ??= Metrics.CreateCounter("dns_failures", "DNS解析失败次数"); }
    }

    /// <summary>
    /// Socket积压队列大小
    /// </summary>
    public static Gauge SocketBacklogGauge
    {
        get { return _socketBacklogGauge ??= Metrics.CreateGauge("socket_backlog", "Socket积压队列大小"); }
    }

    /// <summary>
    /// TLS握手失败次数
    /// </summary>
    public static Counter TlsHandshakeFailuresCounter
    {
        get { return _tlsHandshakeFailuresCounter ??= Metrics.CreateCounter("tls_handshake_failures", "TLS握手失败次数"); }
    }

    /// <summary>
    /// IPv4连接数
    /// </summary>
    public static Gauge Ipv4ConnectionsGauge
    {
        get { return _ipv4ConnectionsGauge ??= Metrics.CreateGauge("ipv4_connections", "IPv4连接数"); }
    }

    /// <summary>
    /// IPv6连接数
    /// </summary>
    public static Gauge Ipv6ConnectionsGauge
    {
        get { return _ipv6ConnectionsGauge ??= Metrics.CreateGauge("ipv6_connections", "IPv6连接数"); }
    }

    /// <summary>
    /// UDP错误数
    /// </summary>
    public static Counter UdpErrorsCounter
    {
        get { return _udpErrorsCounter ??= Metrics.CreateCounter("udp_errors", "UDP错误数"); }
    }

    /// <summary>
    /// 总接收数据包数量
    /// </summary>
    public static Counter TotalPacketsReceivedCounter
    {
        get { return _totalPacketsReceivedCounter ??= Metrics.CreateCounter("total_packets_received", "总接收数据包数量"); }
    }

    /// <summary>
    /// 总发送数据包数量
    /// </summary>
    public static Counter TotalPacketsSentCounter
    {
        get { return _totalPacketsSentCounter ??= Metrics.CreateCounter("total_packets_sent", "总发送数据包数量"); }
    }

    /// <summary>
    /// 每分钟接收数据包数量
    /// </summary>
    public static Gauge PacketsReceivedPerMinuteGauge
    {
        get { return _packetsReceivedPerMinuteGauge ??= Metrics.CreateGauge("packets_received_per_minute", "每分钟接收数据包数量"); }
    }

    /// <summary>
    /// 每分钟发送数据包数量
    /// </summary>
    public static Gauge PacketsSentPerMinuteGauge
    {
        get { return _packetsSentPerMinuteGauge ??= Metrics.CreateGauge("packets_sent_per_minute", "每分钟发送数据包数量"); }
    }

    /// <summary>
    /// 总接收字节数
    /// </summary>
    public static Counter TotalBytesReceivedCounter
    {
        get { return _totalBytesReceivedCounter ??= Metrics.CreateCounter("total_bytes_received", "总接收字节数"); }
    }

    /// <summary>
    /// 总发送字节数
    /// </summary>
    public static Counter TotalBytesSentCounter
    {
        get { return _totalBytesSentCounter ??= Metrics.CreateCounter("total_bytes_sent", "总发送字节数"); }
    }

    /// <summary>
    /// 当前连接数
    /// </summary>
    public static Gauge CurrentConnectionsGauge
    {
        get { return _currentConnectionsGauge ??= Metrics.CreateGauge("current_connections", "当前连接数"); }
    }

    /// <summary>
    /// 每秒接收字节数
    /// </summary>
    public static Gauge BytesReceivedPerSecondGauge
    {
        get { return _bytesReceivedPerSecondGauge ??= Metrics.CreateGauge("bytes_received_per_second", "每秒接收字节数"); }
    }

    /// <summary>
    /// 每秒发送字节数
    /// </summary>
    public static Gauge BytesSentPerSecondGauge
    {
        get { return _bytesSentPerSecondGauge ??= Metrics.CreateGauge("bytes_sent_per_second", "每秒发送字节数"); }
    }

    /// <summary>
    /// 网络延迟
    /// </summary>
    public static Gauge NetworkLatencyGauge
    {
        get { return _networkLatencyGauge ??= Metrics.CreateGauge("network_latency", "网络延迟(毫秒)"); }
    }
}