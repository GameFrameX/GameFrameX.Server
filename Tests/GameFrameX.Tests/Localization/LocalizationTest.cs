using GameFrameX.Foundation.Localization.Core;

namespace GameFrameX.Tests.Localization;

/// <summary>
/// 本地化功能测试
/// </summary>
public class LocalizationTest
{
    [Fact]
    public void TestResourceManagerAvailability()
    {
        // Act
        var message = LocalizationService.GetString("Log_Server_Started");

        // Assert
        Assert.NotNull(message);
        Assert.NotEmpty(message);
    }
}