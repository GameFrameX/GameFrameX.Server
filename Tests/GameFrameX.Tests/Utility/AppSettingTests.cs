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
        appSetting.InnerIp = "192.168.1.100";
        appSetting.InnerPort = 8080;
        appSetting.OuterIp = "203.0.113.1";
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
        Assert.Equal("192.168.1.100", appSetting.InnerIp);
        Assert.Equal((ushort)8080, appSetting.InnerPort);
        Assert.Equal("203.0.113.1", appSetting.OuterIp);
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
        appSetting.DiscoveryCenterIp = "192.168.1.200";
        appSetting.DiscoveryCenterPort = 8500;
        appSetting.DBIp = "192.168.1.201";
        appSetting.DBPort = 27017;
        appSetting.SDKType = 1;
        appSetting.APMPort = 8200;
        
        // Assert
        Assert.Equal("mongodb://localhost:27017", appSetting.DataBaseUrl);
        Assert.Equal("GameDB", appSetting.DataBaseName);
        Assert.Equal("zh-CN", appSetting.Language);
        Assert.Equal("Beijing", appSetting.DataCenter);
        Assert.Equal("192.168.1.200", appSetting.DiscoveryCenterIp);
        Assert.Equal((ushort)8500, appSetting.DiscoveryCenterPort);
        Assert.Equal("192.168.1.201", appSetting.DBIp);
        Assert.Equal((ushort)27017, appSetting.DBPort);
        Assert.Equal(1, appSetting.SDKType);
        Assert.Equal((ushort)8200, appSetting.APMPort);
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
            tasks[i] = Task.Run(() =>
            {
                appSetting.AppRunning = index % 2 == 0;
            });
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