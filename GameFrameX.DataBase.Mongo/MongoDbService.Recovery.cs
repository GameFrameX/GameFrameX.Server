using GameFrameX.DataBase.Abstractions;
using GameFrameX.Foundation.Logger;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameFrameX.DataBase.Mongo;

public sealed partial class MongoDbService
{
    /// <summary>
    /// 启动后台恢复任务（单飞）。
    /// </summary>
    private void EnsureRecoveryTaskStarted()
    {
        if (Interlocked.CompareExchange(ref _recoveryTaskStartedFlag, 1, 0) != 0)
        {
            return;
        }

        _recoveryTaskCancellationTokenSource = new CancellationTokenSource();
        _recoveryTask = Task.Run(() => RunRecoveryLoopAsync(_recoveryTaskCancellationTokenSource.Token));
    }

    /// <summary>
    /// 停止后台恢复任务。
    /// </summary>
    private async Task StopRecoveryTaskAsync()
    {
        if (Interlocked.CompareExchange(ref _recoveryTaskStartedFlag, 0, 1) != 1)
        {
            return;
        }

        try
        {
            _recoveryTaskCancellationTokenSource?.Cancel();
            if (_recoveryTask != null)
            {
                await _recoveryTask.ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _recoveryTaskCancellationTokenSource?.Dispose();
            _recoveryTaskCancellationTokenSource = null;
            _recoveryTask = null;
        }
    }

    /// <summary>
    /// 后台恢复循环，负责 Unhealthy 状态下探活。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    private async Task RunRecoveryLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (AvailabilityState != DatabaseAvailabilityState.Unhealthy)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
                continue;
            }

            var jitterMilliseconds = Random.Shared.Next(0, (int)_recoveryProbeJitterDelay.TotalMilliseconds);
            await Task.Delay(_recoveryProbeBaseDelay + TimeSpan.FromMilliseconds(jitterMilliseconds), cancellationToken).ConfigureAwait(false);

            var pingSucceeded = await TryReconnectAndPingAsync(cancellationToken).ConfigureAwait(false);
            if (!pingSucceeded)
            {
                continue;
            }

            lock (_availabilityLock)
            {
                if (AvailabilityState == DatabaseAvailabilityState.Unhealthy)
                {
                    ChangeAvailabilityState(DatabaseAvailabilityState.Recovering, "background_probe_success");
                }
            }
        }
    }

    /// <summary>
    /// 尝试重建连接并执行探活。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>成功返回 true。</returns>
    private async Task<bool> TryReconnectAndPingAsync(CancellationToken cancellationToken)
    {
        var options = Options;
        if (options == null || string.IsNullOrWhiteSpace(options.ConnectionString) || string.IsNullOrWhiteSpace(options.Name))
        {
            return false;
        }

        try
        {
            var settings = MongoClientSettings.FromConnectionString(options.ConnectionString);
            settings.ServerSelectionTimeout = _serverSelectionTimeout;
            settings.ConnectTimeout = _connectTimeout;
            settings.SocketTimeout = _socketTimeout;
            settings.RetryReads = true;
            settings.RetryWrites = true;
            var newClient = new MongoClient(settings);
            var newDatabase = newClient.GetDatabase(options.Name);
            await newDatabase.RunCommandAsync((Command<BsonDocument>)"{ping:1}", cancellationToken: cancellationToken).ConfigureAwait(false);

            lock (_availabilityLock)
            {
                var oldClient = _mongoClient;
                _mongoClient = newClient;
                CurrentDatabase = newDatabase;
                _mongoDbContext = new MongoDbContext(newDatabase);
                oldClient?.Dispose();
            }

            return true;
        }
        catch (Exception exception)
        {
            LogHelper.Warning("MongoDbService.TryReconnectAndPingAsync failed. error={error}", exception.Message);
            return false;
        }
    }
}
