// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

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