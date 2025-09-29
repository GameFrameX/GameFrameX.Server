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

using System;
using System.Threading.Tasks;
using GameFrameX.Utility.Setting;
using Xunit;

namespace GameFrameX.Tests.Utility;

/// <summary>
/// AppSetting类的单元测试
/// </summary>
public class AppSettingTests
{
    /// <summary>
    /// 测试AppSetting构造函数
    /// </summary>
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange & Act
        var appSetting = new AppSetting();

        // Assert
        Assert.NotNull(appSetting.AppExitSource);
        Assert.NotNull(appSetting.AppExitToken);
        Assert.False(appSetting.AppRunning);
        Assert.True(appSetting.LaunchTime <= DateTime.UtcNow);
        Assert.True(appSetting.LaunchTime > DateTime.UtcNow.AddMinutes(-1));

#if DEBUG
        Assert.True(appSetting.IsDebug);
        Assert.True(appSetting.IsDebugReceive);
        Assert.True(appSetting.IsDebugSend);
        Assert.True(appSetting.IsDebugSendHeartBeat);
        Assert.True(appSetting.IsDebugReceiveHeartBeat);
#else
        Assert.False(appSetting.IsDebug);
        Assert.False(appSetting.IsDebugReceive);
        Assert.False(appSetting.IsDebugSend);
        Assert.False(appSetting.IsDebugSendHeartBeat);
        Assert.False(appSetting.IsDebugReceiveHeartBeat);
#endif
    }

    /// <summary>
    /// 测试AppExitToken属性
    /// </summary>
    [Fact]
    public void AppExitToken_ShouldReturnTaskFromAppExitSource()
    {
        // Arrange
        var appSetting = new AppSetting();

        // Act
        var token = appSetting.AppExitToken;

        // Assert
        Assert.NotNull(token);
        Assert.Same(appSetting.AppExitSource.Task, token);
        Assert.False(token.IsCompleted);
    }

    /// <summary>
    /// 测试AppRunning属性设置为true
    /// </summary>
    [Fact]
    public void AppRunning_SetToTrue_ShouldUpdateValue()
    {
        // Arrange
        var appSetting = new AppSetting();

        // Act
        appSetting.AppRunning = true;

        // Assert
        Assert.True(appSetting.AppRunning);
        Assert.False(appSetting.AppExitToken.IsCompleted);
    }

    /// <summary>
    /// 测试AppRunning属性设置为false
    /// </summary>
    [Fact]
    public void AppRunning_SetToFalse_ShouldCancelAppExitToken()
    {
        // Arrange
        var appSetting = new AppSetting();
        appSetting.AppRunning = true;

        // Act
        appSetting.AppRunning = false;

        // Assert
        Assert.False(appSetting.AppRunning);
        Assert.True(appSetting.AppExitToken.IsCanceled);
    }

    /// <summary>
    /// 测试AppRunning在已取消状态下设置为true
    /// </summary>
    [Fact]
    public void AppRunning_SetToTrueAfterCanceled_ShouldRemainFalse()
    {
        // Arrange
        var appSetting = new AppSetting();
        appSetting.AppRunning = true;
        appSetting.AppRunning = false; // 取消

        // Act
        appSetting.AppRunning = true; // 尝试重新设置为true

        // Assert
        Assert.False(appSetting.AppRunning);
        Assert.True(appSetting.AppExitToken.IsCanceled);
    }

    /// <summary>
    /// 测试ServerType属性设置
    /// </summary>
    [Fact]
    public void ServerType_SetValue_ShouldUpdateServerTypeAndServerName()
    {
        // Arrange
        var appSetting = new AppSetting();

        // Act
        appSetting.ServerType = ServerType.Game;

        // Assert
        Assert.Equal(ServerType.Game, appSetting.ServerType);
        Assert.Equal("Game", appSetting.ServerName);
    }

    /// <summary>
    /// 测试ServerType属性设置为不同值
    /// </summary>
    [Theory]
    [InlineData(ServerType.Gateway, "Gateway")]
    [InlineData(ServerType.Account, "Account")]
    [InlineData(ServerType.Login, "Login")]
    [InlineData(ServerType.Chat, "Chat")]
    [InlineData(ServerType.DataBase, "DataBase")]
    public void ServerType_SetDifferentValues_ShouldUpdateCorrectly(ServerType serverType, string expectedName)
    {
        // Arrange
        var appSetting = new AppSetting();

        // Act
        appSetting.ServerType = serverType;

        // Assert
        Assert.Equal(serverType, appSetting.ServerType);
        Assert.Equal(expectedName, appSetting.ServerName);
    }

    /// <summary>
    /// 测试IsLocal方法
    /// </summary>
    [Fact]
    public void IsLocal_WithMatchingServerId_ShouldReturnTrue()
    {
        // Arrange
        var appSetting = new AppSetting { ServerId = 123 };

        // Act
        var result = appSetting.IsLocal(123);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试IsLocal方法与不匹配的服务器ID
    /// </summary>
    [Fact]
    public void IsLocal_WithNonMatchingServerId_ShouldReturnFalse()
    {
        // Arrange
        var appSetting = new AppSetting { ServerId = 123 };

        // Act
        var result = appSetting.IsLocal(456);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试ToString方法
    /// </summary>
    [Fact]
    public void ToString_ShouldReturnJsonString()
    {
        // Arrange
        var appSetting = new AppSetting
        {
            ServerId = 123,
            ServerType = ServerType.Game,
            IsDebug = true
        };

        // Act
        var result = appSetting.ToString();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("123", result); // ServerId
        Assert.Contains("Game", result); // ServerName
    }

    /// <summary>
    /// 测试ToFormatString方法
    /// </summary>
    [Fact]
    public void ToFormatString_ShouldReturnFormattedJsonString()
    {
        // Arrange
        var appSetting = new AppSetting
        {
            ServerId = 123,
            ServerType = ServerType.Game,
            IsDebug = true
        };

        // Act
        var result = appSetting.ToFormatString();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("123", result); // ServerId
        Assert.Contains("Game", result); // ServerName
        // 格式化的JSON应该包含换行符或缩进
        Assert.True(result.Contains("\n") || result.Contains("  "));
    }

    /// <summary>
    /// 测试LaunchTime属性
    /// </summary>
    [Fact]
    public void LaunchTime_ShouldBeSetToCurrentUtcTime()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var appSetting = new AppSetting();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(appSetting.LaunchTime >= beforeCreation);
        Assert.True(appSetting.LaunchTime <= afterCreation);
    }

    /// <summary>
    /// 测试LaunchTime属性可以被设置
    /// </summary>
    [Fact]
    public void LaunchTime_CanBeSet_ShouldUpdateValue()
    {
        // Arrange
        var appSetting = new AppSetting();
        var newTime = DateTime.UtcNow.AddHours(-1);

        // Act
        appSetting.LaunchTime = newTime;

        // Assert
        Assert.Equal(newTime, appSetting.LaunchTime);
    }

    /// <summary>
    /// 测试默认配置属性值
    /// </summary>
    [Fact]
    public void DefaultConfigurationProperties_ShouldHaveCorrectValues()
    {
        // Arrange & Act
        var appSetting = new AppSetting();

        // Assert
        Assert.False(appSetting.IsOpenTelemetryMetrics);
        Assert.False(appSetting.IsOpenTelemetryTracing);
        Assert.False(appSetting.IsOpenTelemetry);
        Assert.False(appSetting.IsMonitorTimeOut);
        Assert.Equal(0, appSetting.MonitorTimeOutSeconds);
        Assert.Equal(0, appSetting.NetWorkSendTimeOutSeconds);
        Assert.Equal(300_000, appSetting.SaveDataInterval);
        Assert.Equal(500, appSetting.SaveDataBatchCount);
        Assert.Equal(30_000, appSetting.SaveDataBatchTimeOut);
        Assert.Equal(30_000, appSetting.ActorTimeOut);
        Assert.Equal(15, appSetting.ActorRecycleTime);
        Assert.Equal(30_000, appSetting.ActorQueueTimeOut);
        Assert.Equal(3000, appSetting.MaxClientCount);
    }

    /// <summary>
    /// 测试配置属性可以被设置
    /// </summary>
    [Fact]
    public void ConfigurationProperties_CanBeSet_ShouldUpdateValues()
    {
        // Arrange
        var appSetting = new AppSetting();

        // Act
        appSetting.IsOpenTelemetryMetrics = true;
        appSetting.IsOpenTelemetryTracing = true;
        appSetting.IsOpenTelemetry = true;
        appSetting.IsMonitorTimeOut = true;
        appSetting.MonitorTimeOutSeconds = 5;
        appSetting.NetWorkSendTimeOutSeconds = 10;
        appSetting.SaveDataInterval = 600_000;
        appSetting.SaveDataBatchCount = 1000;
        appSetting.SaveDataBatchTimeOut = 60_000;
        appSetting.ActorTimeOut = 60_000;
        appSetting.ActorRecycleTime = 30;
        appSetting.ActorQueueTimeOut = 60_000;
        appSetting.MaxClientCount = 5000;

        // Assert
        Assert.True(appSetting.IsOpenTelemetryMetrics);
        Assert.True(appSetting.IsOpenTelemetryTracing);
        Assert.True(appSetting.IsOpenTelemetry);
        Assert.True(appSetting.IsMonitorTimeOut);
        Assert.Equal(5, appSetting.MonitorTimeOutSeconds);
        Assert.Equal(10, appSetting.NetWorkSendTimeOutSeconds);
        Assert.Equal(600_000, appSetting.SaveDataInterval);
        Assert.Equal(1000, appSetting.SaveDataBatchCount);
        Assert.Equal(60_000, appSetting.SaveDataBatchTimeOut);
        Assert.Equal(60_000, appSetting.ActorTimeOut);
        Assert.Equal(30, appSetting.ActorRecycleTime);
        Assert.Equal(60_000, appSetting.ActorQueueTimeOut);
        Assert.Equal(5000, appSetting.MaxClientCount);
    }

    /// <summary>
    /// 测试网络和端口相关属性
    /// </summary>
    [Fact]
    public void NetworkAndPortProperties_CanBeSet_ShouldUpdateValues()
    {
        // Arrange
        var appSetting = new AppSetting();

        // Act
        appSetting.InnerHost = "192.168.1.100";
        appSetting.InnerPort = 8080;
        appSetting.OuterHost = "203.0.113.1";
        appSetting.OuterPort = 9090;
        appSetting.HttpUrl = "http://localhost:8080";
        appSetting.HttpIsDevelopment = true;
        appSetting.HttpPort = 80;
        appSetting.HttpsPort = 443;
        appSetting.MetricsPort = 9091;
        appSetting.WsPort = 8081;
        appSetting.WssPort = 8443;
        appSetting.WssCertFilePath = "/path/to/cert.pem";

        // Assert
        Assert.Equal("192.168.1.100", appSetting.InnerHost);
        Assert.Equal((ushort)8080, appSetting.InnerPort);
        Assert.Equal("203.0.113.1", appSetting.OuterHost);
        Assert.Equal((ushort)9090, appSetting.OuterPort);
        Assert.Equal("http://localhost:8080", appSetting.HttpUrl);
        Assert.True(appSetting.HttpIsDevelopment);
        Assert.Equal((ushort)80, appSetting.HttpPort);
        Assert.Equal((ushort)443, appSetting.HttpsPort);
        Assert.Equal((ushort)9091, appSetting.MetricsPort);
        Assert.Equal((ushort)8081, appSetting.WsPort);
        Assert.Equal((ushort)8443, appSetting.WssPort);
        Assert.Equal("/path/to/cert.pem", appSetting.WssCertFilePath);
    }

    /// <summary>
    /// 测试数据库和服务相关属性
    /// </summary>
    [Fact]
    public void DatabaseAndServiceProperties_CanBeSet_ShouldUpdateValues()
    {
        // Arrange
        var appSetting = new AppSetting();

        // Act
        appSetting.DataBaseUrl = "mongodb://localhost:27017";
        appSetting.DataBaseName = "GameDB";
        appSetting.Language = "zh-CN";
        appSetting.DataCenter = "Beijing";
        appSetting.DiscoveryCenterHost = "192.168.1.200";
        appSetting.DiscoveryCenterPort = 8500;


        // Assert
        Assert.Equal("mongodb://localhost:27017", appSetting.DataBaseUrl);
        Assert.Equal("GameDB", appSetting.DataBaseName);
        Assert.Equal("zh-CN", appSetting.Language);
        Assert.Equal("Beijing", appSetting.DataCenter);
        Assert.Equal("192.168.1.200", appSetting.DiscoveryCenterHost);
        Assert.Equal((ushort)8500, appSetting.DiscoveryCenterPort);
    }

    /// <summary>
    /// 测试游戏逻辑相关属性
    /// </summary>
    [Fact]
    public void GameLogicProperties_CanBeSet_ShouldUpdateValues()
    {
        // Arrange
        var appSetting = new AppSetting();

        // Act
        appSetting.WorkerId = 1;
        appSetting.MinModuleId = 100;
        appSetting.MaxModuleId = 200;
        appSetting.TagName = "Production";
        appSetting.Description = "Game Server";
        appSetting.Note = "Main game logic server";
        appSetting.Label = "game,logic";

        // Assert
        Assert.Equal((ushort)1, appSetting.WorkerId);
        Assert.Equal((short)100, appSetting.MinModuleId);
        Assert.Equal((short)200, appSetting.MaxModuleId);
        Assert.Equal("Production", appSetting.TagName);
        Assert.Equal("Game Server", appSetting.Description);
        Assert.Equal("Main game logic server", appSetting.Note);
        Assert.Equal("game,logic", appSetting.Label);
    }

    /// <summary>
    /// 测试多线程环境下AppRunning属性的线程安全性
    /// </summary>
    [Fact]
    public async Task AppRunning_MultipleThreads_ShouldBeThreadSafe()
    {
        // Arrange
        var appSetting = new AppSetting();
        var tasks = new Task[10];

        // Act
        for (int i = 0; i < tasks.Length; i++)
        {
            int index = i;
            tasks[i] = Task.Run(() => { appSetting.AppRunning = index % 2 == 0; });
        }

        await Task.WhenAll(tasks);

        // Assert
        // 验证最终状态是一致的
        Assert.True(appSetting.AppExitToken.IsCanceled || !appSetting.AppExitToken.IsCompleted);
    }

    /// <summary>
    /// 测试ServerInstanceId属性
    /// </summary>
    [Fact]
    public void ServerInstanceId_CanBeSet_ShouldUpdateValue()
    {
        // Arrange
        var appSetting = new AppSetting();
        var instanceId = 123456789L;

        // Act
        appSetting.ServerInstanceId = instanceId;

        // Assert
        Assert.Equal(instanceId, appSetting.ServerInstanceId);
    }
}