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

using GameFrameX.DataBase.Abstractions;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Foundation.Utility;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameFrameX.DataBase.Mongo;

/// <summary>
/// MongoDB服务连接类，实现了
/// <see>
///     <cref>IDatabaseService</cref>
/// </see>
/// 接口。
/// </summary>
/// <remarks>
/// MongoDB service connection class that implements the
/// <see>
///     <cref>IDatabaseService</cref>
/// </see>
/// interface.
/// </remarks>
public sealed partial class MongoDbService : IDatabaseService
{
    private static readonly TimeSpan ServerSelectionTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan SocketTimeout = TimeSpan.FromSeconds(10);
    private static readonly int[] ReadRetryDelaysMilliseconds = { 120, 300, 700 };
    private static readonly int[] IdempotentWriteRetryDelaysMilliseconds = { 150, 400, 900 };
    private static readonly int[] TransactionRetryDelaysMilliseconds = { 200, 500, 1000 };

    /// <summary>
    /// 获取或设置当前使用的MongoDB数据库。
    /// </summary>
    /// <remarks>
    /// Gets or sets the currently used MongoDB database.
    /// </remarks>
    public IMongoDatabase CurrentDatabase { get; private set; }

    /// <summary>
    /// 获取或设置当前使用的MongoDB数据库配置选项。
    /// </summary>
    /// <remarks>
    /// Gets or sets the currently used MongoDB database configuration options.
    /// </remarks>
    public DbOptions Options{ get; private set; }

    private MongoDbContext _mongoDbContext;
    private MongoClient _mongoClient;

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
            throw new DatabaseUnavailableException("MongoDbService is unavailable. Open() has not completed successfully.");
        }
    }

    /// <summary>
    /// 获取当前时间戳（毫秒）。
    /// </summary>
    /// <remarks>
    /// Gets the current timestamp in milliseconds.
    /// If <see cref="DbOptions.IsUseTimeZone"/> is <c>true</c>, returns the timestamp with time zone offset;
    /// otherwise, returns the standard UTC timestamp.
    /// </remarks>
    /// <returns>
    /// 返回当前时间的 Unix 时间戳（毫秒） / Returns the current Unix timestamp in milliseconds
    /// </returns>
    private long GetCurrentTimestamp()
    {
        if (Options.IsUseTimeZone)
        {
            return TimerHelper.UnixTimeMillisecondsWithTimeZoneOffset();
        }

        return TimerHelper.UnixTimeMilliseconds();
    }

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

                LogHelper.Info("MongoDbService.Open {dbName} {ConnectionString} {mongoDbInitializedSuccessfully}", dbOptions.Name, dbOptions.ConnectionString, LocalizationService.GetString(Localization.Keys.Database.MongoDbInitializedSuccessfully, dbOptions.ConnectionString, dbOptions.Name));
                return true;
            }
            catch (Exception exception)
            {
                lastException = exception;
                ResetConnectionState();
                if (attempt < retryDelays.Length - 1)
                {
                    LogHelper.Warning("MongoDbService.Open Retry {attempt}/{maxRetry} {dbName} {ConnectionString} {exception}", attempt + 1, retryDelays.Length, dbOptions.Name, dbOptions.ConnectionString, exception.Message);
                    var delay = retryDelays[attempt] + Random.Shared.Next(0, 200);
                    await Task.Delay(delay).ConfigureAwait(false);
                }
            }
        }

        LogHelper.Fatal("MongoDbService.Open Exception {dbName} {ConnectionString} {exception}", dbOptions.Name, dbOptions.ConnectionString, lastException);
        var message = LocalizationService.GetString(Localization.Keys.Database.MongoDbInitializationFailed, dbOptions.ConnectionString, dbOptions.Name);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
        LogHelper.Error("MongoDbService.Open Exception {dbName} {ConnectionString} {message}", dbOptions.Name, dbOptions.ConnectionString, message);
        return false;
    }

    /// <summary>
    /// 关闭MongoDB连接。
    /// </summary>
    /// <remarks>
    /// Closes the MongoDB connection.
    /// </remarks>
    public Task Close()
    {
        ResetConnectionState();
        return Task.CompletedTask;
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
    /// 执行带有自动重试机制的读取操作。
    /// </summary>
    /// <remarks>
    /// Executes a read operation with automatic retry mechanism on transient failures.
    /// </remarks>
    /// <typeparam name="T">操作返回值类型 / The type of the operation result</typeparam>
    /// <param name="operation">要执行的读取操作 / The read operation to execute</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <param name="operationName">操作名称，用于日志记录 / Operation name for logging</param>
    /// <returns>操作结果 / The operation result</returns>
    private async Task<T> ExecuteReadWithRetryAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken, string operationName)
    {
        return await ExecuteWithRetryAsync(operation, cancellationToken, ReadRetryDelaysMilliseconds, operationName).ConfigureAwait(false);
    }

    /// <summary>
    /// 执行带有自动重试机制的读取操作（无返回值）。
    /// </summary>
    /// <remarks>
    /// Executes a read operation with automatic retry mechanism on transient failures (no return value).
    /// </remarks>
    /// <param name="operation">要执行的读取操作 / The read operation to execute</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <param name="operationName">操作名称，用于日志记录 / Operation name for logging</param>
    private async Task ExecuteReadWithRetryAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken, string operationName)
    {
        await ExecuteWithRetryAsync(async token =>
        {
            await operation(token).ConfigureAwait(false);
            return true;
        }, cancellationToken, ReadRetryDelaysMilliseconds, operationName).ConfigureAwait(false);
    }

    /// <summary>
    /// 执行带有自动重试机制的写入操作。
    /// </summary>
    /// <remarks>
    /// Executes a write operation with automatic retry mechanism on transient failures.
    /// Only idempotent operations will be retried to avoid duplicate writes.
    /// </remarks>
    /// <typeparam name="T">操作返回值类型 / The type of the operation result</typeparam>
    /// <param name="operation">要执行的写入操作 / The write operation to execute</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <param name="operationName">操作名称，用于日志记录 / Operation name for logging</param>
    /// <param name="isIdempotent">操作是否为幂等操作 / Whether the operation is idempotent</param>
    /// <returns>操作结果 / The operation result</returns>
    private async Task<T> ExecuteWriteWithRetryAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken, string operationName, bool isIdempotent)
    {
        if (!isIdempotent)
        {
            return await operation(cancellationToken).ConfigureAwait(false);
        }

        return await ExecuteWithRetryAsync(operation, cancellationToken, IdempotentWriteRetryDelaysMilliseconds, operationName).ConfigureAwait(false);
    }

    /// <summary>
    /// 执行带有自动重试机制的写入操作（无返回值）。
    /// </summary>
    /// <remarks>
    /// Executes a write operation with automatic retry mechanism on transient failures (no return value).
    /// Only idempotent operations will be retried to avoid duplicate writes.
    /// </remarks>
    /// <param name="operation">要执行的写入操作 / The write operation to execute</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <param name="operationName">操作名称，用于日志记录 / Operation name for logging</param>
    /// <param name="isIdempotent">操作是否为幂等操作 / Whether the operation is idempotent</param>
    private async Task ExecuteWriteWithRetryAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken, string operationName, bool isIdempotent)
    {
        if (!isIdempotent)
        {
            await operation(cancellationToken).ConfigureAwait(false);
            return;
        }

        await ExecuteWithRetryAsync(async token =>
        {
            await operation(token).ConfigureAwait(false);
            return true;
        }, cancellationToken, IdempotentWriteRetryDelaysMilliseconds, operationName).ConfigureAwait(false);
    }

    /// <summary>
    /// 判断异常是否为可重试的事务异常。
    /// </summary>
    /// <remarks>
    /// Determines whether the exception is a transient transaction error that can be retried.
    /// </remarks>
    /// <param name="exception">要检查的异常 / The exception to check</param>
    /// <returns>如果是可重试的事务异常则返回 <c>true</c>；否则返回 <c>false</c> / <c>true</c> if the exception is retryable; otherwise <c>false</c></returns>
    private static bool ShouldRetryTransactionException(Exception exception)
    {
        return exception is MongoException mongoException && mongoException.HasErrorLabel("TransientTransactionError");
    }

    /// <summary>
    /// 判断异常是否为可重试的事务提交异常。
    /// </summary>
    /// <remarks>
    /// Determines whether the exception indicates an unknown transaction commit result that can be retried.
    /// </remarks>
    /// <param name="exception">要检查的异常 / The exception to check</param>
    /// <returns>如果是可重试的事务提交异常则返回 <c>true</c>；否则返回 <c>false</c> / <c>true</c> if the commit is retryable; otherwise <c>false</c></returns>
    private static bool ShouldRetryTransactionCommit(Exception exception)
    {
        return exception is MongoException mongoException && mongoException.HasErrorLabel("UnknownTransactionCommitResult");
    }

    /// <summary>
    /// 判断异常是否为可重试的 MongoDB 异常。
    /// </summary>
    /// <remarks>
    /// Determines whether the exception is a retryable MongoDB exception such as connection error, timeout, or transient error.
    /// </remarks>
    /// <param name="exception">要检查的异常 / The exception to check</param>
    /// <returns>如果是可重试的异常则返回 <c>true</c>；否则返回 <c>false</c> / <c>true</c> if the exception is retryable; otherwise <c>false</c></returns>
    private static bool IsRetryableMongoException(Exception exception)
    {
        if (exception is MongoConnectionException or TimeoutException or MongoExecutionTimeoutException)
        {
            return true;
        }

        if (exception is MongoException mongoException)
        {
            if (mongoException.HasErrorLabel("RetryableWriteError") || mongoException.HasErrorLabel("RetryableReadError"))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 执行带有自定义重试延迟的通用重试机制。
    /// </summary>
    /// <remarks>
    /// Executes an operation with custom retry delays and exponential backoff with jitter.
    /// </remarks>
    /// <typeparam name="T">操作返回值类型 / The type of the operation result</typeparam>
    /// <param name="operation">要执行的操作 / The operation to execute</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <param name="retryDelaysMilliseconds">每次重试的延迟毫秒数列表 / List of retry delays in milliseconds</param>
    /// <param name="operationName">操作名称，用于日志记录 / Operation name for logging</param>
    /// <returns>操作结果 / The operation result</returns>
    /// <exception cref="InvalidOperationException">当所有重试都失败后抛出 / Thrown when all retry attempts fail</exception>
    private async Task<T> ExecuteWithRetryAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken, IReadOnlyList<int> retryDelaysMilliseconds, string operationName)
    {
        ArgumentNullException.ThrowIfNull(operation, nameof(operation));
        cancellationToken.ThrowIfCancellationRequested();
        Exception lastException = null;
        for (var attempt = 0; attempt <= retryDelaysMilliseconds.Count; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return await operation(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (IsRetryableMongoException(exception) && attempt < retryDelaysMilliseconds.Count)
            {
                lastException = exception;
                var delay = retryDelaysMilliseconds[attempt] + Random.Shared.Next(0, 100);
                LogHelper.Warning("MongoDbService.{operationName} transient error, retry {attempt}/{maxRetry}. error={error}", operationName, attempt + 1, retryDelaysMilliseconds.Count, exception.Message);
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
        }

        throw lastException ?? new InvalidOperationException($"MongoDbService.{operationName} failed with unknown retry exception.");
    }

    /// <summary>
    /// 获取指定类型的MongoDB集合。
    /// </summary>
    /// <remarks>
    /// Gets the MongoDB collection of the specified type.
    /// </remarks>
    /// <typeparam name="TState">文档的类型 / The type of the document</typeparam>
    /// <param name="settings">集合的设置 / Collection settings</param>
    /// <returns>指定类型的MongoDB集合 / MongoDB collection of the specified type</returns>
    private IMongoCollection<TState> GetCollection<TState>(MongoCollectionSettings settings = null) where TState : class, ICacheState, new()
    {
        var collectionName = typeof(TState).Name;
        var collection = CurrentDatabase.GetCollection<TState>(collectionName, settings);
        CreateIndexes(collection);
        return collection;
    }

    /// <summary>
    /// 获取指定名称的MongoDB集合。
    /// </summary>
    /// <remarks>
    /// Gets the MongoDB collection with the specified name.
    /// </remarks>
    /// <param name="collectionName">集合名称 / Collection name</param>
    /// <param name="settings">集合的设置 / Collection settings</param>
    /// <returns>指定名称的MongoDB集合 / MongoDB collection with the specified name</returns>
    private IMongoCollection<BsonDocument> GetCollection(string collectionName, MongoCollectionSettings settings = null)
    {
        var collection = CurrentDatabase.GetCollection<BsonDocument>(collectionName, settings);
        return collection;
    }
}
