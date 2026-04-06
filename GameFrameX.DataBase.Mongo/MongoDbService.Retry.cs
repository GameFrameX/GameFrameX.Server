using GameFrameX.DataBase.Abstractions;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Foundation.Logger;
using GameFrameX.Localization;
using MongoDB.Driver;
using System.Diagnostics;

namespace GameFrameX.DataBase.Mongo;

public sealed partial class MongoDbService
{
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
        return await ExecuteReadWithRetryAsync(operation, cancellationToken, operationName, fallbackValueFactory: null).ConfigureAwait(false);
    }

    /// <summary>
    /// 执行带有自动重试机制的读取操作（支持降级返回）。
    /// </summary>
    /// <typeparam name="T">操作返回值类型。</typeparam>
    /// <param name="operation">读取操作。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <param name="operationName">操作名称。</param>
    /// <param name="fallbackValueFactory">降级返回值工厂。</param>
    /// <returns>操作结果。</returns>
    private async Task<T> ExecuteReadWithRetryAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken, string operationName, Func<T> fallbackValueFactory)
    {
        if (TryReturnDegradedReadFallback(operationName, fallbackValueFactory, out T degradedValue))
        {
            return degradedValue;
        }

        return await ExecuteWithRetryAsync(operation, cancellationToken, _readRetryDelaysMilliseconds, operationName, "read").ConfigureAwait(false);
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
        }, cancellationToken, _readRetryDelaysMilliseconds, operationName, "read").ConfigureAwait(false);
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
        if (!IsCoreWriteAllowed(operationName))
        {
            throw new DatabaseUnavailableException(LocalizationService.GetString(Keys.Database.MongoDbServiceUnavailable));
        }

        if (!isIdempotent)
        {
            return await ExecuteWithRetryAsync(operation, cancellationToken, Array.Empty<int>(), operationName, "write").ConfigureAwait(false);
        }

        return await ExecuteWithRetryAsync(operation, cancellationToken, _idempotentWriteRetryDelaysMilliseconds, operationName, "write").ConfigureAwait(false);
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
        if (!IsCoreWriteAllowed(operationName))
        {
            throw new DatabaseUnavailableException(LocalizationService.GetString(Keys.Database.MongoDbServiceUnavailable));
        }

        var retryDelays = isIdempotent ? _idempotentWriteRetryDelaysMilliseconds : Array.Empty<int>();
        await ExecuteWithRetryAsync(async token =>
        {
            await operation(token).ConfigureAwait(false);
            return true;
        }, cancellationToken, retryDelays, operationName, "write").ConfigureAwait(false);
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
    /// <param name="operationType">操作类型（read/write） / Operation type (read/write)</param>
    /// <returns>操作结果 / The operation result</returns>
    /// <exception cref="DatabaseUnavailableException">当所有重试都失败后抛出 / Thrown when all retry attempts fail</exception>
    private async Task<T> ExecuteWithRetryAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken, IReadOnlyList<int> retryDelaysMilliseconds, string operationName, string operationType)
    {
        ArgumentNullException.ThrowIfNull(operation, nameof(operation));
        cancellationToken.ThrowIfCancellationRequested();
        var operationStopwatch = Stopwatch.StartNew();
        Exception lastException = null;
        for (var attempt = 0; attempt <= retryDelaysMilliseconds.Count; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                if (string.Equals(operationType, "write", StringComparison.Ordinal) && !IsCoreWriteAllowed(operationName))
                {
                    throw new DatabaseUnavailableException(LocalizationService.GetString(Keys.Database.MongoDbServiceUnavailable));
                }

                var result = await operation(cancellationToken).ConfigureAwait(false);
                RecordOperationSuccess(operationName, operationType);
                DbOperationLatencyMilliseconds.Record(operationStopwatch.Elapsed.TotalMilliseconds, new TagList { { "op", operationType }, { "name", operationName }, { "success", true }, });
                return result;
            }
            catch (Exception exception)
            {
                if (!IsRetryableMongoException(exception))
                {
                    throw;
                }

                lastException = exception;
                RecordRetryableFailure(operationName, operationType, exception);
                if (attempt < retryDelaysMilliseconds.Count)
                {
                    var delay = retryDelaysMilliseconds[attempt] + Random.Shared.Next(0, 100);
                    DbOperationRetryTotal.Add(1, new TagList { { "op", operationType }, { "name", operationName }, { "reason", GetFailureReason(exception) }, });
                    LogHelper.Warning("MongoDbService.{operationName} transient error, retry {attempt}/{maxRetry}. error={error}", operationName, attempt + 1, retryDelaysMilliseconds.Count, exception.Message);
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                break;
            }
        }

        // Localization: Database.MongoDb.OperationRetryFailed - MongoDbService.{0}重试失败，未知异常
        var finalException = new DatabaseUnavailableException(LocalizationService.GetString(Keys.Database.MongoDbOperationRetryFailed, operationName), lastException ?? new InvalidOperationException(LocalizationService.GetString(Keys.Database.MongoDbOperationRetryFailed, operationName)));
        DbOperationFailTotal.Add(1, new TagList { { "op", operationType }, { "name", operationName }, { "reason", GetFailureReason(lastException) }, });
        DbOperationLatencyMilliseconds.Record(operationStopwatch.Elapsed.TotalMilliseconds, new TagList { { "op", operationType }, { "name", operationName }, { "success", false }, });
        throw finalException;
    }

    /// <summary>
    /// 获取失败原因标签。
    /// </summary>
    /// <remarks>
    /// Gets a normalized failure reason tag.
    /// </remarks>
    /// <param name="exception">异常对象 / Exception object</param>
    /// <returns>失败原因标签 / Failure reason tag</returns>
    private static string GetFailureReason(Exception exception)
    {
        return exception switch
        {
            null => "unknown",
            TimeoutException => "timeout",
            MongoConnectionException => "connection",
            MongoExecutionTimeoutException => "execution_timeout",
            MongoException mongoException when mongoException.HasErrorLabel("RetryableWriteError") => "retryable_write",
            MongoException mongoException when mongoException.HasErrorLabel("RetryableReadError") => "retryable_read",
            MongoException => "mongo_exception",
            DatabaseUnavailableException => "database_unavailable",
            _ => "unknown",
        };
    }
}
