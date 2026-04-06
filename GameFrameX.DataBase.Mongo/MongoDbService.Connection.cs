using GameFrameX.DataBase.Abstractions;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Foundation.Logger;
using GameFrameX.Localization;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;

namespace GameFrameX.DataBase.Mongo;

public sealed partial class MongoDbService
{
    /// <summary>
    /// 连接数据库。
    /// </summary>
    /// <remarks>
    /// Connects to the database.
    /// </remarks>
    /// <param name="dbOptions">数据库配置选项 / Database configuration options</param>
    /// <returns>返回数据库是否初始化成功 / Returns whether the database was initialized successfully</returns>
    public async Task<bool> Open(DbOptions dbOptions)
    {
        ArgumentNullException.ThrowIfNull(dbOptions, nameof(dbOptions));
        ArgumentNullException.ThrowIfNull(dbOptions.ConnectionString, nameof(dbOptions.ConnectionString));
        ArgumentNullException.ThrowIfNull(dbOptions.Name, nameof(dbOptions.Name));
        var connectionTarget = BuildConnectionTargetTag(dbOptions.ConnectionString, dbOptions.Name);
        var openStopwatch = Stopwatch.StartNew();

        if (_mongoClient != null && CurrentDatabase != null && _mongoDbContext != null)
        {
            var isSameTarget = string.Equals(Options?.ConnectionString, dbOptions.ConnectionString, StringComparison.Ordinal) &&
                               string.Equals(Options?.Name, dbOptions.Name, StringComparison.Ordinal);
            if (isSameTarget)
            {
                try
                {
                    await CurrentDatabase.RunCommandAsync((Command<BsonDocument>)"{ping:1}").ConfigureAwait(false);
                    Options = dbOptions;
                    return true;
                }
                catch
                {
                    ResetConnectionState();
                }
            }
            else
            {
                ResetConnectionState();
            }
        }

        Options = dbOptions;
        Exception lastException = null;
        var retryDelays = new[] { 300, 700, 1500 };
        for (var attempt = 0; attempt < retryDelays.Length; attempt++)
        {
            try
            {
                var settings = MongoClientSettings.FromConnectionString(Options.ConnectionString);
                settings.ServerSelectionTimeout = ServerSelectionTimeout;
                settings.ConnectTimeout = ConnectTimeout;
                settings.SocketTimeout = SocketTimeout;
                settings.RetryReads = true;
                settings.RetryWrites = true;

                _mongoClient = new MongoClient(settings);
                CurrentDatabase = _mongoClient.GetDatabase(Options.Name);
                _mongoDbContext = new MongoDbContext(CurrentDatabase);
                await CurrentDatabase.RunCommandAsync((Command<BsonDocument>)"{ping:1}").ConfigureAwait(false);
                lock (_availabilityLock)
                {
                    _consecutiveFailures = 0;
                    _consecutiveSuccesses = 0;
                    ChangeAvailabilityState(DatabaseAvailabilityState.Healthy, "open_success");
                }

                DbOperationLatencyMilliseconds.Record(openStopwatch.Elapsed.TotalMilliseconds, new TagList { { "op", "open" }, { "name", nameof(Open) }, { "success", true }, });
                LogHelper.Info("MongoDbService.Open {dbName} {target} {mongoDbInitializedSuccessfully}", dbOptions.Name, connectionTarget, LocalizationService.GetString(Localization.Keys.Database.MongoDbInitializedSuccessfully, connectionTarget, dbOptions.Name));
                return true;
            }
            catch (Exception exception)
            {
                lastException = exception;
                ResetConnectionState();
                if (attempt < retryDelays.Length - 1)
                {
                    DbOpenRetryTotal.Add(1, new TagList { { "db.target", connectionTarget }, });
                    LogHelper.Warning("MongoDbService.Open Retry {attempt}/{maxRetry} {dbName} {target} {exception}", attempt + 1, retryDelays.Length, dbOptions.Name, connectionTarget, exception.Message);
                    var delay = retryDelays[attempt] + Random.Shared.Next(0, 200);
                    await Task.Delay(delay).ConfigureAwait(false);
                }
            }
        }

        DbOperationFailTotal.Add(1, new TagList { { "op", "open" }, { "name", nameof(Open) }, { "reason", GetFailureReason(lastException) }, });
        DbOperationLatencyMilliseconds.Record(openStopwatch.Elapsed.TotalMilliseconds, new TagList { { "op", "open" }, { "name", nameof(Open) }, { "success", false }, });
        LogHelper.Fatal("MongoDbService.Open Exception {dbName} {target} {exception}", dbOptions.Name, connectionTarget, lastException);
        var message = LocalizationService.GetString(Localization.Keys.Database.MongoDbInitializationFailed, connectionTarget, dbOptions.Name);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
        LogHelper.Error("MongoDbService.Open Exception {dbName} {target} {message}", dbOptions.Name, connectionTarget, message);
        lock (_availabilityLock)
        {
            ChangeAvailabilityState(DatabaseAvailabilityState.Unhealthy, "open_failed");
            EnsureRecoveryTaskStarted();
        }

        return false;
    }

    /// <summary>
    /// 关闭MongoDB连接。
    /// </summary>
    /// <remarks>
    /// Closes the MongoDB connection.
    /// </remarks>
    public async Task Close()
    {
        await StopRecoveryTaskAsync().ConfigureAwait(false);
        ResetConnectionState();
        lock (_availabilityLock)
        {
            _consecutiveFailures = 0;
            _consecutiveSuccesses = 0;
            _recoveringProbeSuccessCount = 0;
            ChangeAvailabilityState(DatabaseAvailabilityState.Healthy, "close_reset");
        }
    }

    /// <summary>
    /// 重置连接状态并释放相关资源。
    /// </summary>
    /// <remarks>
    /// Resets the connection state and releases related resources.
    /// </remarks>
    private void ResetConnectionState()
    {
        _mongoClient?.Dispose();
        _mongoClient = null;
        _mongoDbContext = null;
        CurrentDatabase = null;
    }

    /// <summary>
    /// 确保数据库服务已初始化。
    /// </summary>
    /// <remarks>
    /// Ensures that the database service has been initialized.
    /// </remarks>
    /// <exception cref="DatabaseUnavailableException">当服务未初始化时抛出 / Thrown when the service is not initialized</exception>
    private void EnsureInitialized()
    {
        if (_mongoDbContext == null)
        {
            // Localization: Database.MongoDb.ServiceUnavailable - MongoDbService不可用，Open()尚未成功完成
            throw new DatabaseUnavailableException(LocalizationService.GetString(Keys.Database.MongoDbServiceUnavailable));
        }
    }
}
