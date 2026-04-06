using GameFrameX.DataBase.Abstractions;
using GameFrameX.DataBase.Mongo;
using GameFrameX.DataBase.Mongo.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using System.Reflection;

namespace GameFrameX.Tests.DataBase;

/// <summary>
/// MongoDbService 连接与健康检查相关测试。
/// </summary>
public sealed class MongoDbServiceConnectionTests
{
    /// <summary>
    /// 测试在相同配置下重复打开连接时复用已有数据库对象。
    /// </summary>
    [Fact]
    public async Task Open_WithSameOptions_ShouldReuseCurrentDatabaseInstance()
    {
        var connectionString = Environment.GetEnvironmentVariable("GAMEFRAMEX_TEST_MONGODB_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return;
        }

        var dbName = $"gameframex_test_conn_reuse_{Guid.NewGuid():N}";
        var service = new MongoDbService();
        var options = new DbOptions
        {
            Type = "MongoDb",
            ConnectionString = connectionString,
            Name = dbName,
        };

        try
        {
            var firstOpen = await service.Open(options);
            Assert.True(firstOpen);
            var firstDb = service.CurrentDatabase;

            var secondOpen = await service.Open(options);
            Assert.True(secondOpen);
            Assert.Same(firstDb, service.CurrentDatabase);
        }
        finally
        {
            await service.Close();
            await DropDatabaseQuietlyAsync(connectionString, dbName);
        }
    }

    /// <summary>
    /// 测试连续失败时健康检查会从降级升级为不健康状态。
    /// </summary>
    [Fact]
    public async Task HealthCheck_WhenConsecutiveFailuresReachThreshold_ShouldBecomeUnhealthy()
    {
        var settings = new MongoClientSettings
        {
            Server = new MongoServerAddress("127.0.0.1", 1),
            ServerSelectionTimeout = TimeSpan.FromMilliseconds(120),
            ConnectTimeout = TimeSpan.FromMilliseconds(120),
            SocketTimeout = TimeSpan.FromMilliseconds(120),
        };
        var client = new MongoClient(settings);
        var healthCheck = new MongoDbHealthCheck(client);
        var context = new HealthCheckContext();

        var firstStatus = HealthStatus.Unhealthy;
        var hasFirstStatus = false;
        var lastStatus = HealthStatus.Unhealthy;
        for (var i = 0; i < 5; i++)
        {
            var result = await healthCheck.CheckHealthAsync(context);
            if (!hasFirstStatus)
            {
                firstStatus = result.Status;
                hasFirstStatus = true;
            }

            lastStatus = result.Status;
        }

        Assert.True(hasFirstStatus);
        Assert.Equal(HealthStatus.Degraded, firstStatus);
        Assert.Equal(HealthStatus.Unhealthy, lastStatus);
    }

    /// <summary>
    /// 测试重试判定方法对超时异常返回可重试。
    /// </summary>
    [Fact]
    public void RetryClassifier_WhenTimeoutException_ShouldBeRetryable()
    {
        var method = typeof(MongoDbService).GetMethod("IsRetryableMongoException", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);
        var result = method.Invoke(null, new object[] { new TimeoutException("timeout"), });
        Assert.IsType<bool>(result);
        Assert.True((bool)result);
    }

    /// <summary>
    /// 测试重试判定方法对带重试标签的异常返回可重试。
    /// </summary>
    [Fact]
    public void RetryClassifier_WhenMongoExceptionHasRetryableReadLabel_ShouldBeRetryable()
    {
        var method = typeof(MongoDbService).GetMethod("IsRetryableMongoException", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);
        var exception = new MongoException("retryable");
        exception.AddErrorLabel("RetryableReadError");
        var result = method.Invoke(null, new object[] { exception, });
        Assert.IsType<bool>(result);
        Assert.True((bool)result);
    }

    /// <summary>
    /// 测试事务重试判定方法识别瞬态事务错误标签。
    /// </summary>
    [Fact]
    public void TransactionRetryClassifier_WhenTransientTransactionLabel_ShouldBeRetryable()
    {
        var method = typeof(MongoDbService).GetMethod("ShouldRetryTransactionException", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);
        var exception = new MongoException("tx transient");
        exception.AddErrorLabel("TransientTransactionError");
        var result = method.Invoke(null, new object[] { exception, });
        Assert.IsType<bool>(result);
        Assert.True((bool)result);
    }

    /// <summary>
    /// 测试提交重试判定方法识别未知提交结果标签。
    /// </summary>
    [Fact]
    public void CommitRetryClassifier_WhenUnknownCommitResultLabel_ShouldBeRetryable()
    {
        var method = typeof(MongoDbService).GetMethod("ShouldRetryTransactionCommit", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);
        var exception = new MongoException("commit transient");
        exception.AddErrorLabel("UnknownTransactionCommitResult");
        var result = method.Invoke(null, new object[] { exception, });
        Assert.IsType<bool>(result);
        Assert.True((bool)result);
    }

    /// <summary>
    /// 测试重试耗尽后抛出数据库不可用异常。
    /// </summary>
    [Fact]
    public async Task ExecuteWithRetryAsync_WhenRetryExhausted_ShouldThrowDatabaseUnavailableException()
    {
        var service = new MongoDbService();
        var method = typeof(MongoDbService).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                                           .FirstOrDefault(m => m.Name == "ExecuteWithRetryAsync" && m.IsGenericMethodDefinition && m.GetParameters().Length == 5);
        Assert.NotNull(method);
        var genericMethod = method.MakeGenericMethod(typeof(bool));
        Func<CancellationToken, Task<bool>> operation = _ => throw new TimeoutException("test timeout");
        var taskObject = genericMethod.Invoke(service, new object[] { operation, CancellationToken.None, Array.Empty<int>(), "UnitTestOperation", "read", });
        Assert.NotNull(taskObject);
        var task = Assert.IsType<Task<bool>>(taskObject);
        await Assert.ThrowsAsync<DatabaseUnavailableException>(async () => await task);
    }

    /// <summary>
    /// 测试连接目标标识不会泄露用户名和密码。
    /// </summary>
    [Fact]
    public void BuildConnectionTargetTag_ShouldNotContainCredentials()
    {
        var method = typeof(MongoDbService).GetMethod("BuildConnectionTargetTag", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);
        var connectionString = "mongodb://user_name:pass_word@127.0.0.1:27017/admin";
        var target = method.Invoke(null, new object[] { connectionString, "gameframex", }) as string;
        Assert.False(string.IsNullOrWhiteSpace(target));
        Assert.DoesNotContain("user_name", target, StringComparison.Ordinal);
        Assert.DoesNotContain("pass_word", target, StringComparison.Ordinal);
        Assert.Contains("host=127.0.0.1", target, StringComparison.Ordinal);
    }

    /// <summary>
    /// 测试状态机在连续可重试失败后从 Healthy 进入 Degraded 再进入 Unhealthy。
    /// </summary>
    [Fact]
    public void AvailabilityState_WhenConsecutiveRetryableFailures_ShouldTransitToUnhealthy()
    {
        var service = new MongoDbService();
        var recordFailureMethod = typeof(MongoDbService).GetMethod("RecordRetryableFailure", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(recordFailureMethod);

        for (var i = 0; i < 5; i++)
        {
            recordFailureMethod.Invoke(service, new object[] { "UnitTestRead", "read", new TimeoutException("state-machine-test"), });
        }

        Assert.Equal(DatabaseAvailabilityState.Unhealthy, service.AvailabilityState);
    }

    /// <summary>
    /// 测试 Recovering 状态下连续成功达到阈值后回切 Healthy。
    /// </summary>
    [Fact]
    public void AvailabilityState_WhenRecoveringGetsConsecutiveSuccesses_ShouldTransitToHealthy()
    {
        var service = new MongoDbService();
        var availabilityStateField = typeof(MongoDbService).GetField("_availabilityState", BindingFlags.NonPublic | BindingFlags.Instance);
        var recordSuccessMethod = typeof(MongoDbService).GetMethod("RecordOperationSuccess", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(availabilityStateField);
        Assert.NotNull(recordSuccessMethod);

        availabilityStateField.SetValue(service, (int)DatabaseAvailabilityState.Recovering);
        for (var i = 0; i < 3; i++)
        {
            recordSuccessMethod.Invoke(service, new object[] { "UnitTestRead", "read", });
        }

        Assert.Equal(DatabaseAvailabilityState.Healthy, service.AvailabilityState);
    }

    /// <summary>
    /// 测试 Unhealthy 状态下白名单读接口会触发降级返回。
    /// </summary>
    [Fact]
    public void DegradeReadFallback_WhenUnhealthyAndWhitelistedRead_ShouldReturnFallback()
    {
        var service = new MongoDbService();
        var availabilityStateField = typeof(MongoDbService).GetField("_availabilityState", BindingFlags.NonPublic | BindingFlags.Instance);
        var method = typeof(MongoDbService).GetMethod("TryReturnDegradedReadFallback", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(availabilityStateField);
        Assert.NotNull(method);

        availabilityStateField.SetValue(service, (int)DatabaseAvailabilityState.Unhealthy);
        var genericMethod = method.MakeGenericMethod(typeof(long));
        var args = new object[] { "CountAsync", new Func<long>(() => 42L), null, };
        var handled = genericMethod.Invoke(service, args);

        Assert.IsType<bool>(handled);
        Assert.True((bool)handled);
        Assert.Equal(42L, Assert.IsType<long>(args[2]));
    }

    /// <summary>
    /// 测试 Unhealthy 状态下核心写接口会被快速失败策略拒绝。
    /// </summary>
    [Fact]
    public void CoreWriteAllowed_WhenUnhealthyAndCoreWrite_ShouldReturnFalse()
    {
        var service = new MongoDbService();
        var availabilityStateField = typeof(MongoDbService).GetField("_availabilityState", BindingFlags.NonPublic | BindingFlags.Instance);
        var method = typeof(MongoDbService).GetMethod("IsCoreWriteAllowed", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(availabilityStateField);
        Assert.NotNull(method);

        availabilityStateField.SetValue(service, (int)DatabaseAvailabilityState.Unhealthy);
        var allowed = method.Invoke(service, new object[] { "AddAsync", });

        Assert.IsType<bool>(allowed);
        Assert.False((bool)allowed);
    }

    /// <summary>
    /// 异步清理测试数据库。
    /// </summary>
    /// <param name="connectionString">MongoDB 连接字符串。</param>
    /// <param name="dbName">测试数据库名称。</param>
    private static async Task DropDatabaseQuietlyAsync(string connectionString, string dbName)
    {
        try
        {
            var client = new MongoClient(connectionString);
            await client.DropDatabaseAsync(dbName);
        }
        catch
        {
        }
    }
}
