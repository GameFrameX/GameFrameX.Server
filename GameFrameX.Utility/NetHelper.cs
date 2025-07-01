using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace GameFrameX.Utility;

/// <summary>
/// 网络帮助类
/// 提供网络相关的实用工具方法，包括IP地址验证、端口可用性检查、本地IP获取等功能
/// </summary>
/// <remarks>
/// 核心功能：
/// - IP地址格式验证和解析
/// - 端口可用性检测和管理
/// - 本地网络接口信息获取
/// - 网络连接状态查询
/// 
/// 适用场景：
/// - 网络服务器端口分配
/// - 客户端连接配置验证
/// - 网络诊断和监控
/// - 分布式系统节点发现
/// </remarks>
public static class NetHelper
{
    /// <summary>
    /// 判断IP地址是否合法
    /// </summary>
    /// <param name="ipAddress">IP地址字符串</param>
    /// <param name="value">解析成功的IPAddress对象</param>
    /// <returns>如果IP地址合法，返回true；否则返回false</returns>
    /// <exception cref="ArgumentNullException">当ipAddress为null时抛出此异常</exception>
    public static bool IsValidIpAddress(string ipAddress, out IPAddress value)
    {
        ArgumentNullException.ThrowIfNull(ipAddress, nameof(ipAddress));

        return IPAddress.TryParse(ipAddress, out value);
    }

    /// <summary>
    /// 获取第一个可用的端口号
    /// </summary>
    /// <param name="startPort">起始端口号，默认为667</param>
    /// <param name="maxPort">结束端口号，默认为65535</param>
    /// <returns>第一个可用的端口号，如果没有可用端口号则返回-1</returns>
    /// <exception cref="ArgumentOutOfRangeException">当startPort小于1或大于65535时抛出此异常</exception>
    /// <exception cref="ArgumentOutOfRangeException">当maxPort小于1或大于65535时抛出此异常</exception>
    /// <exception cref="ArgumentException">当startPort大于等于maxPort时抛出此异常</exception>
    public static int GetFirstAvailablePort(int startPort = 667, int maxPort = 65535)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(startPort, 1, nameof(startPort));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startPort, 65535, nameof(startPort));
        ArgumentOutOfRangeException.ThrowIfLessThan(maxPort, 1, nameof(maxPort));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(maxPort, 65535, nameof(maxPort));
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(startPort, maxPort, nameof(startPort));

        for (var i = startPort; i < maxPort; i++)
        {
            if (PortIsAvailable(i))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// 获取操作系统已用的端口号
    /// </summary>
    /// <returns>包含已用端口号的列表</returns>
    public static List<int> PortIsUsed()
    {
        // 获取本地计算机的网络连接和通信统计数据的信息
        var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

        // 返回本地计算机上的所有TCP监听程序
        var ipsTcp = ipGlobalProperties.GetActiveTcpListeners();

        // 返回本地计算机上的所有UDP监听程序
        var ipsUdp = ipGlobalProperties.GetActiveUdpListeners();

        // 返回本地计算机上的Internet协议版本4 (IPV4) 传输控制协议 (TCP) 连接的信息
        var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

        var allPorts = new List<int>();
        foreach (var ep in ipsTcp)
        {
            allPorts.Add(ep.Port);
        }

        foreach (var ep in ipsUdp)
        {
            allPorts.Add(ep.Port);
        }

        foreach (var conn in tcpConnInfoArray)
        {
            allPorts.Add(conn.LocalEndPoint.Port);
        }

        return allPorts;
    }

    /// <summary>
    /// 检查指定端口是否可用
    /// </summary>
    /// <param name="port">要检查的端口号</param>
    /// <returns>如果端口未被使用，返回true；否则返回false</returns>
    /// <exception cref="ArgumentOutOfRangeException">当port小于1或大于65535时抛出此异常</exception>
    public static bool PortIsAvailable(int port)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(port, 1, nameof(port));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(port, 65535, nameof(port));

        var isAvailable = true;

        var portUsed = PortIsUsed();

        foreach (var p in portUsed)
        {
            if (p != port)
            {
                continue;
            }

            isAvailable = false;
            break;
        }

        return isAvailable;
    }

    /// <summary>
    /// 获取本地IP地址列表
    /// </summary>
    /// <param name="addressFamily">IP地址类型,默认为IPv4</param>
    /// <returns>本地IP地址列表</returns>
    public static List<string> GetLocalIpList(AddressFamily addressFamily = AddressFamily.InterNetwork)
    {
        var ipList = new List<string>();
        try
        {
            // 获取本地计算机的网络接口信息
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var network in networkInterfaces)
            {
                // 排除非活动状态和环回接口
                if (network.OperationalStatus != OperationalStatus.Up || network.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }


                // 获取网络接口的IP属性
                var properties = network.GetIPProperties();
                // 跳过没有网关的地址
                var gateways = properties.GatewayAddresses;
                if (gateways == null || gateways.Count == 0)
                {
                    continue;
                }

                foreach (var address in properties.UnicastAddresses)
                {
                    // 根据指定的地址类型筛选IP
                    if (address.Address.AddressFamily == addressFamily)
                    {
                        ipList.Add(address.Address.ToString());
                    }
                }
            }
        }
        catch (Exception)
        {
            // 发生异常时返回空列表
            return new List<string>();
        }

        return ipList;
    }
}