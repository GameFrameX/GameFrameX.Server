// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


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
