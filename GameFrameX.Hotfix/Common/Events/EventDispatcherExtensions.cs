using GameFrameX.Apps.Common.Event;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Hotfix.Logic.Server.Server;

namespace GameFrameX.Hotfix.Common.Events
{
    public static class EventDispatcherExtensions
    {
        public static void Dispatch(this IComponentAgent agent, int evtId, Param args = null)
        {
            var evt = new Event
            {
                EventId = evtId,
                Data = args
            };

            // 自己处理
            SelfHandle(agent, evtId, evt);

            if ((EventId)evtId > EventId.RoleSeparator && agent.OwnerType > GlobalConst.ActorTypeSeparator)
            {
                // 全局非玩家事件，抛给所有玩家
                agent.Tell(()
                               =>
                           {
                               return ServerComponentAgent.OnlineRoleForeach(role
                                                                                 =>
                                                                             {
                                                                                 role.Dispatch(evtId, args);
                                                                             });
                           });
            }
        }

        static void SelfHandle(IComponentAgent agent, int evtId, Event evt)
        {
            agent.Tell(async () =>
            {
                // 事件需要在本actor内执行，不可多线程执行，所以不能使用Task.WhenAll来处理
                var listeners = HotfixManager.FindListeners(agent.OwnerType, evtId);
                if (listeners.IsNullOrEmpty())
                {
                    // Log.Warn($"事件：{(EventID)evtId} 没有找到任何监听者");
                    return;
                }

                foreach (var listener in listeners)
                {
                    var comp = await agent.GetComponentAgent(listener.AgentType);
                    try
                    {
                        await listener.HandleEvent(comp, evt);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception);
                    }
                }
            });
        }

        public static void Dispatch(this IComponentAgent agent, EventId evtId, Param args = null)
        {
            Dispatch(agent, (int)evtId, args);
        }
    }
}