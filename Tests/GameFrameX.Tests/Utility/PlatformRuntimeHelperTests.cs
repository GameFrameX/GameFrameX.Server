using System.Runtime.InteropServices;
using Xunit;
using GameFrameX.Utility;

namespace GameFrameX.Tests.Utility;

/// <summary>
/// PlatformRuntimeHelper 类的单元测试
/// </summary>
public class PlatformRuntimeHelperTests
{
    /// <summary>
    /// 测试 IsLinux 属性
    /// </summary>
    [Fact]
    public void IsLinux_ShouldReturnCorrectValue()
    {
        // Arrange & Act
        var isLinux = PlatformRuntimeHelper.IsLinux;
        var expectedIsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        // Assert
        Assert.Equal(expectedIsLinux, isLinux);
    }

    /// <summary>
    /// 测试 IsOsx 属性
    /// </summary>
    [Fact]
    public void IsOsx_ShouldReturnCorrectValue()
    {
        // Arrange & Act
        var isOsx = PlatformRuntimeHelper.IsOsx;
        var expectedIsOsx = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        // Assert
        Assert.Equal(expectedIsOsx, isOsx);
    }

    /// <summary>
    /// 测试 IsWindows 属性
    /// </summary>
    [Fact]
    public void IsWindows_ShouldReturnCorrectValue()
    {
        // Arrange & Act
        var isWindows = PlatformRuntimeHelper.IsWindows;
        var expectedIsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        // Assert
        Assert.Equal(expectedIsWindows, isWindows);
    }

    /// <summary>
    /// 测试 IsFreeBsd 属性
    /// </summary>
    [Fact]
    public void IsFreeBsd_ShouldReturnCorrectValue()
    {
        // Arrange & Act
        var isFreeBsd = PlatformRuntimeHelper.IsFreeBsd;
        var expectedIsFreeBsd = RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);

        // Assert
        Assert.Equal(expectedIsFreeBsd, isFreeBsd);
    }

    /// <summary>
    /// 测试在任何时候只有一个平台为真
    /// </summary>
    [Fact]
    public void OnlyOnePlatformShouldBeTrue()
    {
        // Arrange & Act
        var platforms = new[]
        {
            PlatformRuntimeHelper.IsLinux,
            PlatformRuntimeHelper.IsOsx,
            PlatformRuntimeHelper.IsWindows,
            PlatformRuntimeHelper.IsFreeBsd
        };

        var trueCount = 0;
        foreach (var platform in platforms)
        {
            if (platform)
            {
                trueCount++;
            }
        }

        // Assert
        Assert.True(trueCount >= 1, "至少应该有一个平台为真");
        Assert.True(trueCount <= 1, "最多只能有一个平台为真");
    }

    /// <summary>
    /// 测试属性的一致性 - 多次调用应该返回相同的值
    /// </summary>
    [Fact]
    public void PlatformProperties_ShouldBeConsistent()
    {
        // Arrange & Act
        var isLinux1 = PlatformRuntimeHelper.IsLinux;
        var isLinux2 = PlatformRuntimeHelper.IsLinux;
        
        var isOsx1 = PlatformRuntimeHelper.IsOsx;
        var isOsx2 = PlatformRuntimeHelper.IsOsx;
        
        var isWindows1 = PlatformRuntimeHelper.IsWindows;
        var isWindows2 = PlatformRuntimeHelper.IsWindows;
        
        var isFreeBsd1 = PlatformRuntimeHelper.IsFreeBsd;
        var isFreeBsd2 = PlatformRuntimeHelper.IsFreeBsd;

        // Assert
        Assert.Equal(isLinux1, isLinux2);
        Assert.Equal(isOsx1, isOsx2);
        Assert.Equal(isWindows1, isWindows2);
        Assert.Equal(isFreeBsd1, isFreeBsd2);
    }

    /// <summary>
    /// 测试当前运行平台的检测
    /// </summary>
    [Fact]
    public void CurrentPlatform_ShouldBeDetectedCorrectly()
    {
        // Arrange & Act
        var currentPlatform = "Unknown";
        
        if (PlatformRuntimeHelper.IsWindows)
        {
            currentPlatform = "Windows";
        }
        else if (PlatformRuntimeHelper.IsLinux)
        {
            currentPlatform = "Linux";
        }
        else if (PlatformRuntimeHelper.IsOsx)
        {
            currentPlatform = "OSX";
        }
        else if (PlatformRuntimeHelper.IsFreeBsd)
        {
            currentPlatform = "FreeBSD";
        }

        // Assert
        Assert.NotEqual("Unknown", currentPlatform);
    }
}