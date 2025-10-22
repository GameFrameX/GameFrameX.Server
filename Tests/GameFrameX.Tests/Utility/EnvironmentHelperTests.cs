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

using GameFrameX.Foundation.Utility;
using Microsoft.Extensions.Hosting;
using Xunit;
using GameFrameX.Utility;

namespace GameFrameX.Tests.Utility;

/// <summary>
/// EnvironmentHelper 类的单元测试
/// </summary>
public class EnvironmentHelperTests : IDisposable
{
    private readonly Dictionary<string, string> _originalEnvironmentVariables;

    public EnvironmentHelperTests()
    {
        // 保存原始环境变量
        _originalEnvironmentVariables = new Dictionary<string, string>
        {
            { "ASPNETCORE_ENVIRONMENT", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") },
            { "DOTNET_ENVIRONMENT", Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") },
            { "DOTNET_RUNNING_IN_CONTAINER", Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") },
            { "KUBERNETES_SERVICE_HOST", Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST") }
        };
    }

    public void Dispose()
    {
        // 恢复原始环境变量
        foreach (var kvp in _originalEnvironmentVariables)
        {
            if (kvp.Value == null)
            {
                Environment.SetEnvironmentVariable(kvp.Key, null);
            }
            else
            {
                Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
            }
        }
    }

    /// <summary>
    /// 测试 IsDevelopment 方法 - ASPNETCORE_ENVIRONMENT 设置为 Development
    /// </summary>
    [Fact]
    public void IsDevelopment_WithAspNetCoreEnvironmentDevelopment_ShouldReturnTrue()
    {
        // Arrange
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environments.Development);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);

        // Act
        var result = EnvironmentHelper.IsDevelopment();

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试 IsDevelopment 方法 - DOTNET_ENVIRONMENT 设置为 Development
    /// </summary>
    [Fact]
    public void IsDevelopment_WithDotNetEnvironmentDevelopment_ShouldReturnTrue()
    {
        // Arrange
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", Environments.Development);

        // Act
        var result = EnvironmentHelper.IsDevelopment();

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试 IsDevelopment 方法 - 环境变量设置为 Production
    /// </summary>
    [Fact]
    public void IsDevelopment_WithProductionEnvironment_ShouldReturnFalse()
    {
        // Arrange
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environments.Production);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);

        // Act
        var result = EnvironmentHelper.IsDevelopment();

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试 IsProduction 方法 - ASPNETCORE_ENVIRONMENT 设置为 Production
    /// </summary>
    [Fact]
    public void IsProduction_WithAspNetCoreEnvironmentProduction_ShouldReturnTrue()
    {
        // Arrange
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environments.Production);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);

        // Act
        var result = EnvironmentHelper.IsProduction();

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试 IsProduction 方法 - DOTNET_ENVIRONMENT 设置为 Production
    /// </summary>
    [Fact]
    public void IsProduction_WithDotNetEnvironmentProduction_ShouldReturnTrue()
    {
        // Arrange
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", Environments.Production);

        // Act
        var result = EnvironmentHelper.IsProduction();

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试 IsStaging 方法 - ASPNETCORE_ENVIRONMENT 设置为 Staging
    /// </summary>
    [Fact]
    public void IsStaging_WithAspNetCoreEnvironmentStaging_ShouldReturnTrue()
    {
        // Arrange
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environments.Staging);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);

        // Act
        var result = EnvironmentHelper.IsStaging();

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试 IsEnvironment 方法 - 自定义环境名称
    /// </summary>
    [Fact]
    public void IsEnvironment_WithCustomEnvironmentName_ShouldReturnTrue()
    {
        // Arrange
        const string customEnvironment = "Testing";
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", customEnvironment);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);

        // Act
        var result = EnvironmentHelper.IsEnvironment(customEnvironment);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试 IsEnvironment 方法 - 大小写不敏感
    /// </summary>
    [Fact]
    public void IsEnvironment_WithDifferentCase_ShouldReturnTrue()
    {
        // Arrange
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "DEVELOPMENT");
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);

        // Act
        var result = EnvironmentHelper.IsEnvironment("development");

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试 IsEnvironment 方法 - 环境名称不匹配
    /// </summary>
    [Fact]
    public void IsEnvironment_WithNonMatchingEnvironment_ShouldReturnFalse()
    {
        // Arrange
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environments.Development);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);

        // Act
        var result = EnvironmentHelper.IsEnvironment(Environments.Production);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试 IsDocker 方法 - 设置了 DOTNET_RUNNING_IN_CONTAINER 环境变量
    /// </summary>
    [Fact]
    public void IsDocker_WithContainerEnvironmentVariable_ShouldReturnTrue()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER", "true");

        // Act
        var result = EnvironmentHelper.IsDocker();

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试 IsDocker 方法 - 未设置 DOTNET_RUNNING_IN_CONTAINER 环境变量
    /// </summary>
    [Fact]
    public void IsDocker_WithoutContainerEnvironmentVariable_ShouldReturnFalse()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER", null);

        // Act
        var result = EnvironmentHelper.IsDocker();

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试 IsKubernetes 方法 - 设置了 KUBERNETES_SERVICE_HOST 环境变量
    /// </summary>
    [Fact]
    public void IsKubernetes_WithKubernetesEnvironmentVariable_ShouldReturnTrue()
    {
        // Arrange
        Environment.SetEnvironmentVariable("KUBERNETES_SERVICE_HOST", "10.0.0.1");

        // Act
        var result = EnvironmentHelper.IsKubernetes();

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试 IsKubernetes 方法 - 未设置 KUBERNETES_SERVICE_HOST 环境变量
    /// </summary>
    [Fact]
    public void IsKubernetes_WithoutKubernetesEnvironmentVariable_ShouldReturnFalse()
    {
        // Arrange
        Environment.SetEnvironmentVariable("KUBERNETES_SERVICE_HOST", null);

        // Act
        var result = EnvironmentHelper.IsKubernetes();

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试 GetEnvironmentName 方法 - 返回 ASPNETCORE_ENVIRONMENT 的值
    /// </summary>
    [Fact]
    public void GetEnvironmentName_WithAspNetCoreEnvironment_ShouldReturnCorrectValue()
    {
        // Arrange
        const string expectedEnvironment = "CustomEnvironment";
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", expectedEnvironment);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);

        // Act
        var result = EnvironmentHelper.GetEnvironmentName();

        // Assert
        Assert.Equal(expectedEnvironment, result);
    }

    /// <summary>
    /// 测试 GetEnvironmentName 方法 - 返回 DOTNET_ENVIRONMENT 的值
    /// </summary>
    [Fact]
    public void GetEnvironmentName_WithDotNetEnvironment_ShouldReturnCorrectValue()
    {
        // Arrange
        const string expectedEnvironment = "TestEnvironment";
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", expectedEnvironment);

        // Act
        var result = EnvironmentHelper.GetEnvironmentName();

        // Assert
        Assert.Equal(expectedEnvironment, result);
    }

    /// <summary>
    /// 测试 GetEnvironmentName 方法 - 未设置环境变量时返回 null
    /// </summary>
    [Fact]
    public void GetEnvironmentName_WithoutEnvironmentVariables_ShouldReturnNull()
    {
        // Arrange
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);

        // Act
        var result = EnvironmentHelper.GetEnvironmentName();

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// 测试 ASPNETCORE_ENVIRONMENT 优先级高于 DOTNET_ENVIRONMENT
    /// </summary>
    [Fact]
    public void EnvironmentMethods_AspNetCoreEnvironmentTakesPrecedence_ShouldUseAspNetCoreValue()
    {
        // Arrange
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environments.Development);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", Environments.Production);

        // Act
        var isDevelopment = EnvironmentHelper.IsDevelopment();
        var isProduction = EnvironmentHelper.IsProduction();
        var environmentName = EnvironmentHelper.GetEnvironmentName();

        // Assert
        Assert.True(isDevelopment);
        Assert.False(isProduction);
        Assert.Equal(Environments.Development, environmentName);
    }
}