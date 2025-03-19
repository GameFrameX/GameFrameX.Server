using System.Net;
using System.Net.NetworkInformation;

namespace GameFrameX.Utility;

/// <summary>
/// 网络端口帮助类
/// </summary>
public static class Net
{
    /// <summary>
    /// 判断IP地址是否合法
    /// </summary>
    /// <param name="ipAddress">IP地址字符串</param>
    /// <param name="value">解析成功的IPAddress对象</param>
    /// <returns>如果IP地址合法，返回true；否则返回false</returns>
    public static bool IsValidIpAddress(string ipAddress, out IPAddress value)
    {
        return IPAddress.TryParse(ipAddress, out value);
    }

    /// <summary>
    /// 获取第一个可用的端口号
    /// </summary>
    /// <param name="startPort">起始端口号，默认为667</param>
    /// <param name="maxPort">结束端口号，默认为65535</param>
    /// <returns>第一个可用的端口号，如果没有可用端口号则返回-1</returns>
    public static int GetFirstAvailablePort(int startPort = 667, int maxPort = 65535)
    {
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
    public static bool PortIsAvailable(int port)
    {
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
}