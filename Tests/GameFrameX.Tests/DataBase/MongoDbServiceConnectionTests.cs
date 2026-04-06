using GameFrameX.DataBase.Abstractions;
using GameFrameX.DataBase.Mongo;
using GameFrameX.DataBase.Mongo.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using System.Reflection;
using Xunit;

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
