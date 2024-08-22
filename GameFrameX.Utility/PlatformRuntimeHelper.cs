using System.Runtime.InteropServices;

namespace GameFrameX.Utility;

/// <summary>
/// 平台运行时帮助类
/// </summary>
public static class PlatformRuntimeHelper
{
    /// <summary>
    /// 是否是Linux
    /// </summary>
    public static bool IsLinux
    {
        get { return RuntimeInformation.IsOSPlatform(OSPlatform.Linux); }
    }

    /// <summary>
    /// 是否是Mac
    /// </summary>
    public static bool IsOsx
    {
        get { return RuntimeInformation.IsOSPlatform(OSPlatform.OSX); }
    }

    /// <summary>
    /// 是否是Windows
    /// </summary>
    public static bool IsWindows
    {
        get { return RuntimeInformation.IsOSPlatform(OSPlatform.Windows); }
    }

    /// <summary>
    /// 是否是FreeBSD
    /// </summary>
    public static bool IsFreeBsd
    {
        get { return RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD); }
    }
}