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


using GameFrameX.Apps.Server.Component;
using GameFrameX.Apps.Server.Entity;
using GameFrameX.Apps.Common.Event;
using GameFrameX.Apps.Common.EventData;
using GameFrameX.Core.Abstractions.Attribute;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Core.Events;
using GameFrameX.Core.Timer.Handler;
using GameFrameX.Foundation.Utility;
using GameFrameX.Hotfix.Logic.Player.Login;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace GameFrameX.Hotfix.Logic.Server;

public class ServerComponentAgent : StateComponentAgent<ServerComponent, ServerState>
{
    private readonly Dictionary<string, HashSet<long>> _topologySnapshot = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _offlineConfirmCounter = new(StringComparer.OrdinalIgnoreCase);
    private static readonly string[] TopologyServiceNames = { GameServerConst.Game.Name, GameServerConst.Social.Name, GameServerConst.Gateway.Name };

    public override async Task<bool> Active()
    {
        var isContinue = await base.Active();
        if (isContinue)
        {
            // 跨天定时器
            WithCronExpression<CrossDayTimeHandler>("0 0 0 * * ? *");
            Schedule<ServiceTopologyWatcherHandler>(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            if (State.FirstStartTime == default)
            {
                State.FirstStartTime = TimerHelper.UnixTimeSeconds();
                await OwnerComponent.WriteStateAsync();
            }
        }

        return isContinue;
    }

    private Task WatchServiceTopology()
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
                    EventDispatcher.Dispatch(ActorId, (int)EventId.ServiceOnline, new ServiceOnlineEventArgs(serviceName, instance.Key, DateTime.UtcNow, instance.Value));
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
                    EventDispatcher.Dispatch(ActorId, (int)EventId.ServiceOffline, new ServiceOfflineEventArgs(serviceName, instanceId, "Removed", DateTime.UtcNow));
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

            var key = rawKey.Replace("__", ":", StringComparison.Ordinal).Trim();
            if (!key.StartsWith("services:", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var segments = key.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length < 3)
            {
                continue;
            }

            var serviceToken = segments[1];
            var serviceName = TopologyServiceNames.FirstOrDefault(name => string.Equals(name, serviceToken, StringComparison.OrdinalIgnoreCase));
            if (serviceName == null)
            {
                continue;
            }

            if (GlobalSettings.CurrentSetting != null && string.Equals(serviceName, GlobalSettings.CurrentSetting.ServerType, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var instanceId = BuildInstanceId(serviceName, rawValue);
            if (!snapshot[serviceName].TryGetValue(instanceId, out var endpoints))
            {
                endpoints = Array.Empty<string>();
                snapshot[serviceName][instanceId] = endpoints;
            }

            snapshot[serviceName][instanceId] = endpoints.Concat(new[] { rawValue }).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        }

        return snapshot;
    }

    private static long BuildInstanceId(string serviceName, string endpoint)
    {
        var payload = $"{serviceName}|{endpoint}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        return Math.Abs(BitConverter.ToInt64(hash, 0));
    }

    [Service]
    [Discard]
    public virtual ValueTask AddOnlineRole(long roleId)
    {
        OwnerComponent.OnlineSet.Add(roleId);
        return ValueTask.CompletedTask;
    }

    [Service]
    [Discard]
    public virtual ValueTask RemoveOnlineRole(long roleId)
    {
        OwnerComponent.OnlineSet.Remove(roleId);
        return ValueTask.CompletedTask;
    }

    public static async Task OnlineRoleForeach(Action<PlayerComponentAgent> func)
    {
        var serverComp = await ActorManager.GetComponentAgent<ServerComponentAgent>();
        serverComp.Tell(async () =>
        {
            foreach (var roleId in serverComp.OwnerComponent.OnlineSet)
            {
                var roleComp = await ActorManager.GetComponentAgent<PlayerComponentAgent>(roleId);
                roleComp.Tell(() => func(roleComp));
            }
        });
    }


    private Task TestDelayTimer()
    {
        LogHelper.Debug("ServerCompAgent.TestDelayTimer.延时3秒执行.执行一次");
        return Task.CompletedTask;
    }

    private Task TestScheduleTimer()
    {
        LogHelper.Debug("ServerCompAgent.TestSchedueTimer.延时1秒执行.每隔10秒执行");
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
    public virtual Task<int> GetWorldLevel()
    {
        return Task.FromResult(State.WorldLevel);
    }

    [Service]
    public virtual Task<bool> IsOnline(long roleId)
    {
        foreach (var id in OwnerComponent.OnlineSet)
        {
            if (id == roleId)
            {
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
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
    protected virtual Task DoSomething1()
    {
        return Task.CompletedTask;
    }

    [ThreadSafe]
    protected void DoSomething2()
    {
    }

    [Discard]
    [TimeOut(12000)]
    protected virtual Task DoSomething3()
    {
        return Task.CompletedTask;
    }

    private class DelayTimer : TimerHandler<ServerComponentAgent>
    {
        protected override Task HandleTimer(ServerComponentAgent agent, GameEventArgs gameEventArgs)
        {
            return agent.TestDelayTimer();
        }
    }

    private class ScheduleTimer : TimerHandler<ServerComponentAgent>
    {
        protected override Task HandleTimer(ServerComponentAgent agent, GameEventArgs gameEventArgs)
        {
            return agent.TestScheduleTimer();
        }
    }

    /// <summary>
    /// 跨天定时器调用
    /// </summary>
    private class CrossDayTimeHandler : TimerHandler<ServerComponentAgent>
    {
        protected override async Task HandleTimer(ServerComponentAgent agent, GameEventArgs gameEventArgs)
        {
            LogHelper.Debug($"ServerCompAgent.CrossDayTimeHandler.跨天定时器执行{TimerHelper.CurrentDateTimeWithUtcFormat()}");
            await ActorManager.RoleCrossDay(1);
            await ActorManager.CrossDay(1, GlobalConst.ActorTypeServer);
        }
    }

    private class ServiceTopologyWatcherHandler : TimerHandler<ServerComponentAgent>
    {
        protected override Task HandleTimer(ServerComponentAgent agent, GameEventArgs gameEventArgs)
        {
            return agent.WatchServiceTopology();
        }
    }
    /*******************演示代码**************************/
}
