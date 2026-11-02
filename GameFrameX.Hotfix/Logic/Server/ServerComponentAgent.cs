using GameFrameX.Apps.Server.Component;
using GameFrameX.Apps.Server.Entity;
using GameFrameX.Apps.Common.EventData;
using GameFrameX.Core.Abstractions.Attribute;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Core.Events;
using GameFrameX.Core.Timer.Handler;
using GameFrameX.Foundation.Utility;
using GameFrameX.Foundation.Localization.Core;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace GameFrameX.Hotfix.Logic.Server;

public class ServerComponentAgent : StateComponentAgent<ServerComponent, ServerState>
{
    private readonly Dictionary<string, HashSet<long>> _topologySnapshot = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _offlineConfirmCounter = new(StringComparer.OrdinalIgnoreCase);
    private static readonly string[] TopologyServiceNames = { GameServerConst.Game.Name, GameServerConst.Social.Name, GameServerConst.Gateway.Name };

    public override async Task<bool> ActiveAsync()
    {
        var isContinue = await base.ActiveAsync();
        if (isContinue)
        {
            // 跨天定时器
            await WithCronExpressionAsync<CrossDayTimeHandler>("0 0 0 * * ? *");
            await ScheduleAsync<ServiceTopologyWatcherHandler>(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            if (State.FirstStartTime == default)
            {
                State.FirstStartTime = TimerHelper.UnixTimeSeconds();
                await OwnerComponent.WriteStateAsync();
            }
        }

        return isContinue;
    }

    private Task WatchServiceTopologyAsync()
    {
        var currentSnapshot = BuildCurrentTopologySnapshot();
        foreach (var currentPair in currentSnapshot)
        {
            var serviceName = currentPair.Key;
            var currentInstances = currentPair.Value;
            _topologySnapshot.TryGetValue(serviceName, out var previousInstances);
            previousInstances ??= new HashSet<long>();
            foreach (var instance in currentInstances)
            {
                if (!previousInstances.Contains(instance.Key))
                {
                    EventDispatcher.Dispatch(ActorId, new ServiceOnlineEventArgs(serviceName, instance.Key, DateTime.UtcNow, instance.Value));
                }

                _offlineConfirmCounter.Remove(GetOfflineKey(serviceName, instance.Key));
            }

            foreach (var instanceId in previousInstances)
            {
                if (currentInstances.ContainsKey(instanceId))
                {
                    continue;
                }

                var counterKey = GetOfflineKey(serviceName, instanceId);
                _offlineConfirmCounter.TryGetValue(counterKey, out var counter);
                counter++;
                if (counter >= 2)
                {
                    EventDispatcher.Dispatch(ActorId, new ServiceOfflineEventArgs(serviceName, instanceId, "Removed", DateTime.UtcNow));
                    _offlineConfirmCounter.Remove(counterKey);
                    continue;
                }

                _offlineConfirmCounter[counterKey] = counter;
            }
        }

        _topologySnapshot.Clear();
        foreach (var pair in currentSnapshot)
        {
            _topologySnapshot[pair.Key] = pair.Value.Keys.ToHashSet();
        }

        return Task.CompletedTask;
    }

    private static string GetOfflineKey(string serviceName, long instanceId)
    {
        return $"{serviceName}:{instanceId}";
    }

    private static Dictionary<string, Dictionary<long, IReadOnlyList<string>>> BuildCurrentTopologySnapshot()
    {
        var snapshot = new Dictionary<string, Dictionary<long, IReadOnlyList<string>>>(StringComparer.OrdinalIgnoreCase);
        foreach (var serviceName in TopologyServiceNames)
        {
            snapshot[serviceName] = new Dictionary<long, IReadOnlyList<string>>();
        }

        var variables = Environment.GetEnvironmentVariables();
        foreach (DictionaryEntry variable in variables)
        {
            if (variable.Key is not string rawKey || variable.Value is not string rawValue || string.IsNullOrWhiteSpace(rawValue))
            {
                continue;
            }

            TryAddTopologyEntry(snapshot, rawKey, rawValue);
        }

        return snapshot;
    }

    private static void TryAddTopologyEntry(Dictionary<string, Dictionary<long, IReadOnlyList<string>>> snapshot, string rawKey, string rawValue)
    {
        var key = rawKey.Replace("__", ":", StringComparison.Ordinal).Trim();
        if (!key.StartsWith("services:", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var segments = key.Split(':', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length < 3)
        {
            return;
        }

        var serviceToken = segments[1];
        var serviceName = TopologyServiceNames.FirstOrDefault(name => string.Equals(name, serviceToken, StringComparison.OrdinalIgnoreCase));
        if (serviceName == null)
        {
            return;
        }

        if (GlobalSettings.CurrentSetting != null && string.Equals(serviceName, GlobalSettings.CurrentSetting.ServerType, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var instanceId = BuildInstanceId(serviceName, rawValue);
        if (!snapshot[serviceName].TryGetValue(instanceId, out var endpoints))
        {
            endpoints = Array.Empty<string>();
            snapshot[serviceName][instanceId] = endpoints;
        }

        snapshot[serviceName][instanceId] = endpoints.Concat(new[] { rawValue }).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
    }

    private static long BuildInstanceId(string serviceName, string endpoint)
    {
        var payload = $"{serviceName}|{endpoint}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        return Math.Abs(BitConverter.ToInt64(hash, 0));
    }

    private Task TestDelayTimerAsync()
    {
        // Localization: Hotfix.Server.TestDelayTimerExecuted - ServerCompAgent.TestDelayTimer.延时3秒执行.执行一次
        LogHelper.Debug(LocalizationService.GetString(Localization.Keys.Hotfix.Server.TestDelayTimerExecuted));
        return Task.CompletedTask;
    }

    private Task TestScheduleTimerAsync()
    {
        // Localization: Hotfix.Server.TestScheduleTimerExecuted - ServerCompAgent.TestSchedueTimer.延时1秒执行.每隔10秒执行
        LogHelper.Debug(LocalizationService.GetString(Localization.Keys.Hotfix.Server.TestScheduleTimerExecuted));
        //
        // var states = await GameDb.FindListAsync<LoginState>(m => m.Id != 0);
        // LOGGER.Debug(states);
        //
        // var s1 = await GameDb.CountAsync<LoginState>(m => m.Id == 563517512475926528);
        // LOGGER.Debug(s1);
        // var s = await GameDb.FindAsync<LoginState>(m => m.UserName != null);
        // LOGGER.Debug(s);
        // LoginState loginState = new LoginState()
        // {
        //     Id = TimeHelper.UnixTimeSeconds(), CreateId = TimeHelper.UnixTimeSeconds(),
        //     UserName = "Save"
        // };
        // loginState.AfterLoadFromDB(true);
        // var savedState = await GameDb.UpdateAsync<LoginState>(loginState);
        // LOGGER.Debug(savedState);
        // await Task.Delay(TimeSpan.FromSeconds(1));
        // var count = await GameDb.DeleteAsync(savedState);
        // LOGGER.Debug(count);
        return Task.CompletedTask;
    }


    /// <summary>
    /// 获取服务器开服启动时间
    /// </summary>
    /// <returns></returns>
    [Service]
    [ThreadSafe]
    public virtual long FirstStartTime()
    {
        return State.FirstStartTime;
    }

    [Service]
    [ThreadSafe]
    public virtual Task<int> GetWorldLevelAsync()
    {
        return Task.FromResult(State.WorldLevel);
    }

    /*******************演示代码**************************/
    [Service]
    [ThreadSafe]
    public virtual int DoSomething0()
    {
        return State.WorldLevel;
    }

    [Discard]
    [ThreadSafe]
    protected virtual Task DoSomething1Async()
    {
        return Task.CompletedTask;
    }

    [ThreadSafe]
    protected void DoSomething2()
    {
        // 演示占位方法，无具体业务逻辑；保留方法签名以供示例展示。
    }

    [Discard]
    [TimeOut(12000)]
    protected virtual Task DoSomething3Async()
    {
        return Task.CompletedTask;
    }

    private class DelayTimer : TimerHandler<ServerComponentAgent>
    {
        protected override Task HandleTimerAsync(ServerComponentAgent agent, GameEventArgs gameEventArgs)
        {
            return agent.TestDelayTimerAsync();
        }
    }

    private class ScheduleTimer : TimerHandler<ServerComponentAgent>
    {
        protected override Task HandleTimerAsync(ServerComponentAgent agent, GameEventArgs gameEventArgs)
        {
            return agent.TestScheduleTimerAsync();
        }
    }

    /// <summary>
    /// 跨天定时器调用
    /// </summary>
    private class CrossDayTimeHandler : TimerHandler<ServerComponentAgent>
    {
        protected override async Task HandleTimerAsync(ServerComponentAgent agent, GameEventArgs gameEventArgs)
        {
            // Localization: Hotfix.Server.CrossDayTimerExecuted - ServerCompAgent.CrossDayTimeHandler.跨天定时器执行{0}
            LogHelper.Debug(LocalizationService.GetString(Localization.Keys.Hotfix.Server.CrossDayTimerExecuted, TimerHelper.CurrentDateTimeWithUtcFormat()));
            var crossDays = TimerHelper.GetCrossDaysWithUtc(agent.State.FirstStartTime);

            await ActorManager.RoleCrossDayAsync(crossDays);
            await ActorManager.CrossDayAsync(crossDays, GlobalConst.ActorTypeServer);
        }
    }

    private class ServiceTopologyWatcherHandler : TimerHandler<ServerComponentAgent>
    {
        protected override Task HandleTimerAsync(ServerComponentAgent agent, GameEventArgs gameEventArgs)
        {
            return agent.WatchServiceTopologyAsync();
        }
    }
    /*******************演示代码**************************/
}
