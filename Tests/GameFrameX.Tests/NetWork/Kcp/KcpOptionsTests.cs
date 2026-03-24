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

using GameFrameX.NetWork.Kcp;
using Xunit;

namespace GameFrameX.Tests.NetWork.Kcp;

/// <summary>
/// KcpOptions 单元测试 / KcpOptions unit tests
/// </summary>
public class KcpOptionsTests
{
    #region 默认值测试 / Default Value Tests

    /// <summary>
    /// 测试默认配置值 / Test default configuration values
    /// </summary>
    [Fact]
    public void KcpOptions_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var options = new KcpOptions();

        // Assert
        Assert.False(options.Enable);
        Assert.True(options.NoDelay);
        Assert.Equal(10, options.Interval);
        Assert.Equal(2, options.Resend);
        Assert.False(options.EnableFlowControl);
        Assert.Equal(128, options.SendWindow);
        Assert.Equal(128, options.ReceiveWindow);
        Assert.Equal(1400, options.Mtu);
        Assert.Equal(60000, options.ConnectionTimeout);
        Assert.Equal(10000, options.KeepAliveInterval);
        Assert.Equal(5, options.UpdatePeriod);
        Assert.Equal(120, options.SessionTimeout);
    }

    #endregion

    #region 属性设置测试 / Property Setting Tests

    /// <summary>
    /// 测试设置 Enable 属性 / Test setting Enable property
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void KcpOptions_Enable_ShouldBeSettable(bool value)
    {
        // Arrange
        var options = new KcpOptions();

        // Act
        options.Enable = value;

        // Assert
        Assert.Equal(value, options.Enable);
    }

    /// <summary>
    /// 测试设置 NoDelay 属性 / Test setting NoDelay property
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void KcpOptions_NoDelay_ShouldBeSettable(bool value)
    {
        // Arrange
        var options = new KcpOptions();

        // Act
        options.NoDelay = value;

        // Assert
        Assert.Equal(value, options.NoDelay);
    }

    /// <summary>
    /// 测试设置 Interval 属性 / Test setting Interval property
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public void KcpOptions_Interval_ShouldBeSettable(int value)
    {
        // Arrange
        var options = new KcpOptions();

        // Act
        options.Interval = value;

        // Assert
        Assert.Equal(value, options.Interval);
    }

    /// <summary>
    /// 测试设置 Resend 属性 / Test setting Resend property
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    public void KcpOptions_Resend_ShouldBeSettable(int value)
    {
        // Arrange
        var options = new KcpOptions();

        // Act
        options.Resend = value;

        // Assert
        Assert.Equal(value, options.Resend);
    }

    /// <summary>
    /// 测试设置窗口大小属性 / Test setting window size properties
    /// </summary>
    [Theory]
    [InlineData(32, 32)]
    [InlineData(64, 64)]
    [InlineData(128, 128)]
    [InlineData(256, 256)]
    [InlineData(512, 512)]
    public void KcpOptions_WindowSizes_ShouldBeSettable(int sendWindow, int receiveWindow)
    {
        // Arrange
        var options = new KcpOptions();

        // Act
        options.SendWindow = sendWindow;
        options.ReceiveWindow = receiveWindow;

        // Assert
        Assert.Equal(sendWindow, options.SendWindow);
        Assert.Equal(receiveWindow, options.ReceiveWindow);
    }

    /// <summary>
    /// 测试设置 MTU 属性 / Test setting MTU property
    /// </summary>
    [Theory]
    [InlineData(576)]
    [InlineData(1400)]
    [InlineData(1500)]
    public void KcpOptions_Mtu_ShouldBeSettable(int value)
    {
        // Arrange
        var options = new KcpOptions();

        // Act
        options.Mtu = value;

        // Assert
        Assert.Equal(value, options.Mtu);
    }

    /// <summary>
    /// 测试设置超时属性 / Test setting timeout properties
    /// </summary>
    [Theory]
    [InlineData(30000, 60)]
    [InlineData(60000, 120)]
    [InlineData(120000, 300)]
    public void KcpOptions_Timeouts_ShouldBeSettable(int connectionTimeout, int sessionTimeout)
    {
        // Arrange
        var options = new KcpOptions();

        // Act
        options.ConnectionTimeout = connectionTimeout;
        options.SessionTimeout = sessionTimeout;

        // Assert
        Assert.Equal(connectionTimeout, options.ConnectionTimeout);
        Assert.Equal(sessionTimeout, options.SessionTimeout);
    }

    #endregion

    #region 完整配置测试 / Full Configuration Tests

    /// <summary>
    /// 测试完整配置 / Test full configuration
    /// </summary>
    [Fact]
    public void KcpOptions_FullConfiguration_ShouldBeSetCorrectly()
    {
        // Arrange & Act
        var options = new KcpOptions
        {
            Enable = true,
            NoDelay = true,
            Interval = 20,
            Resend = 3,
            EnableFlowControl = true,
            SendWindow = 256,
            ReceiveWindow = 256,
            Mtu = 1200,
            ConnectionTimeout = 30000,
            KeepAliveInterval = 5000,
            UpdatePeriod = 10,
            SessionTimeout = 60
        };

        // Assert
        Assert.True(options.Enable);
        Assert.True(options.NoDelay);
        Assert.Equal(20, options.Interval);
        Assert.Equal(3, options.Resend);
        Assert.True(options.EnableFlowControl);
        Assert.Equal(256, options.SendWindow);
        Assert.Equal(256, options.ReceiveWindow);
        Assert.Equal(1200, options.Mtu);
        Assert.Equal(30000, options.ConnectionTimeout);
        Assert.Equal(5000, options.KeepAliveInterval);
        Assert.Equal(10, options.UpdatePeriod);
        Assert.Equal(60, options.SessionTimeout);
    }

    /// <summary>
    /// 测试低延迟配置 / Test low latency configuration
    /// </summary>
    [Fact]
    public void KcpOptions_LowLatencyConfiguration_ShouldBeOptimizedForSpeed()
    {
        // Arrange & Act - 低延迟配置 / Low latency configuration
        var options = new KcpOptions
        {
            Enable = true,
            NoDelay = true,
            Interval = 10,
            Resend = 2,
            EnableFlowControl = false,
            SendWindow = 128,
            ReceiveWindow = 128
        };

        // Assert
        Assert.True(options.NoDelay);
        Assert.Equal(10, options.Interval);
        Assert.Equal(2, options.Resend);
        Assert.False(options.EnableFlowControl);
    }

    /// <summary>
    /// 测试高吞吐量配置 / Test high throughput configuration
    /// </summary>
    [Fact]
    public void KcpOptions_HighThroughputConfiguration_ShouldBeOptimizedForBandwidth()
    {
        // Arrange & Act - 高吞吐量配置 / High throughput configuration
        var options = new KcpOptions
        {
            Enable = true,
            NoDelay = false,
            Interval = 40,
            Resend = 0,
            EnableFlowControl = true,
            SendWindow = 1024,
            ReceiveWindow = 1024
        };

        // Assert
        Assert.False(options.NoDelay);
        Assert.Equal(40, options.Interval);
        Assert.Equal(0, options.Resend);
        Assert.True(options.EnableFlowControl);
        Assert.Equal(1024, options.SendWindow);
        Assert.Equal(1024, options.ReceiveWindow);
    }

    #endregion
}
