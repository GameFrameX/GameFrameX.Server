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

    /// <summary>
    /// 检测网络连通性
    /// </summary>
    /// <param name="host">目标主机地址</param>
    /// <param name="timeout">超时时间（毫秒），默认5000毫秒</param>
    /// <returns>如果网络连通，返回true；否则返回false</returns>
    /// <exception cref="ArgumentException">当host为null或空字符串时抛出此异常</exception>
    /// <exception cref="ArgumentOutOfRangeException">当timeout小于0时抛出此异常</exception>
    public static bool IsNetworkReachable(string host, int timeout = 5000)
    {
        ArgumentException.ThrowIfNullOrEmpty(host, nameof(host));
        ArgumentOutOfRangeException.ThrowIfNegative(timeout, nameof(timeout));

        try
        {
            using var ping = new Ping();
            var reply = ping.Send(host, timeout);
            return reply.Status == IPStatus.Success;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 获取本机MAC地址列表
    /// </summary>
    /// <returns>MAC地址字符串列表</returns>
    public static List<string> GetMacAddresses()
    {
        var macAddresses = new List<string>();
        try
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var network in networkInterfaces)
            {
                // 排除环回接口和非活动接口
                if (network.NetworkInterfaceType == NetworkInterfaceType.Loopback || 
                    network.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                var macAddress = network.GetPhysicalAddress().ToString();
                if (!string.IsNullOrEmpty(macAddress))
                {
                    // 格式化MAC地址为标准格式 (XX:XX:XX:XX:XX:XX)
                    var formattedMac = string.Join(":", 
                        Enumerable.Range(0, macAddress.Length / 2)
                        .Select(i => macAddress.Substring(i * 2, 2)));
                    macAddresses.Add(formattedMac);
                }
            }
        }
        catch
        {
            // 发生异常时返回空列表
        }

        return macAddresses;
    }

    /// <summary>
    /// 验证IP地址是否在指定的子网内
    /// </summary>
    /// <param name="ipAddress">要验证的IP地址</param>
    /// <param name="networkAddress">网络地址</param>
    /// <param name="subnetMask">子网掩码</param>
    /// <returns>如果IP地址在子网内，返回true；否则返回false</returns>
    /// <exception cref="ArgumentException">当IP地址格式无效时抛出此异常</exception>
    public static bool IsIpInSubnet(string ipAddress, string networkAddress, string subnetMask)
    {
        ArgumentException.ThrowIfNullOrEmpty(ipAddress, nameof(ipAddress));
        ArgumentException.ThrowIfNullOrEmpty(networkAddress, nameof(networkAddress));
        ArgumentException.ThrowIfNullOrEmpty(subnetMask, nameof(subnetMask));

        if (!IPAddress.TryParse(ipAddress, out var ip) ||
            !IPAddress.TryParse(networkAddress, out var network) ||
            !IPAddress.TryParse(subnetMask, out var mask))
        {
            throw new ArgumentException("无效的IP地址格式");
        }

        var ipBytes = ip.GetAddressBytes();
        var networkBytes = network.GetAddressBytes();
        var maskBytes = mask.GetAddressBytes();

        if (ipBytes.Length != networkBytes.Length || ipBytes.Length != maskBytes.Length)
        {
            return false;
        }

        for (int i = 0; i < ipBytes.Length; i++)
        {
            if ((ipBytes[i] & maskBytes[i]) != (networkBytes[i] & maskBytes[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 获取指定范围内的可用端口列表
    /// </summary>
    /// <param name="startPort">起始端口号</param>
    /// <param name="endPort">结束端口号</param>
    /// <param name="maxCount">最大返回数量，默认为10</param>
    /// <returns>可用端口号列表</returns>
    /// <exception cref="ArgumentOutOfRangeException">当端口范围无效时抛出此异常</exception>
    public static List<int> GetAvailablePorts(int startPort, int endPort, int maxCount = 10)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(startPort, 1, nameof(startPort));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startPort, 65535, nameof(startPort));
        ArgumentOutOfRangeException.ThrowIfLessThan(endPort, 1, nameof(endPort));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(endPort, 65535, nameof(endPort));
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(startPort, endPort, nameof(startPort));
        ArgumentOutOfRangeException.ThrowIfLessThan(maxCount, 1, nameof(maxCount));

        var availablePorts = new List<int>();
        var usedPorts = PortIsUsed().ToHashSet();

        for (int port = startPort; port < endPort && availablePorts.Count < maxCount; port++)
        {
            if (!usedPorts.Contains(port))
            {
                availablePorts.Add(port);
            }
        }

        return availablePorts;
    }

    /// <summary>
    /// 获取本机的公网IP地址
    /// </summary>
    /// <param name="timeout">超时时间（毫秒），默认10000毫秒</param>
    /// <returns>公网IP地址字符串，获取失败返回null</returns>
    /// <exception cref="ArgumentOutOfRangeException">当timeout小于0时抛出此异常</exception>
    public static async Task<string?> GetPublicIpAddressAsync(int timeout = 10000)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(timeout, nameof(timeout));

        var services = new[]
        {
            "https://api.ipify.org",
            "https://icanhazip.com",
            "https://ipinfo.io/ip"
        };

        using var httpClient = new HttpClient { Timeout = TimeSpan.FromMilliseconds(timeout) };
        
        foreach (var service in services)
        {
            try
            {
                var response = await httpClient.GetStringAsync(service);
                var ip = response.Trim();
                if (IsValidIpAddress(ip, out _))
                {
                    return ip;
                }
            }
            catch
            {
                // 尝试下一个服务
                continue;
            }
        }

        return null;
    }

    /// <summary>
    /// 判断IP地址是否为私有地址
    /// </summary>
    /// <param name="ipAddress">IP地址字符串</param>
    /// <returns>如果是私有地址，返回true；否则返回false</returns>
    /// <exception cref="ArgumentException">当IP地址格式无效时抛出此异常</exception>
    public static bool IsPrivateIpAddress(string ipAddress)
    {
        ArgumentException.ThrowIfNullOrEmpty(ipAddress, nameof(ipAddress));

        if (!IPAddress.TryParse(ipAddress, out var ip))
        {
            throw new ArgumentException("无效的IP地址格式", nameof(ipAddress));
        }

        var bytes = ip.GetAddressBytes();
        
        // IPv4私有地址范围
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            // 10.0.0.0/8
            if (bytes[0] == 10)
            {
                return true;
            }

            // 172.16.0.0/12
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
            {
                return true;
            }

            // 192.168.0.0/16
            if (bytes[0] == 192 && bytes[1] == 168)
            {
                return true;
            }

            // 127.0.0.0/8 (环回地址)
            if (bytes[0] == 127)
            {
                return true;
            }
        }
        
        return false;
    }
}