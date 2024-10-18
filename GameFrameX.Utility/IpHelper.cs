using System.Net;

namespace GameFrameX.Utility;

/// <summary>
/// IP帮助类
/// </summary>
public static class IpHelper
{
    /// <summary>
    /// 判断IP是否合法
    /// </summary>
    /// <param name="ipAddress">Ip地址</param>
    /// <param name="value">解析成功的值</param>
    /// <returns></returns>
    public static bool IsValidIpAddress(string ipAddress, out IPAddress value)
    {
        return IPAddress.TryParse(ipAddress, out value);
    }
}