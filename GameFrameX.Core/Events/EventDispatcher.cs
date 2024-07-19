using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Core.Actors;
using GameFrameX.Core.Hotfix;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.Utility;

namespace GameFrameX.Core.Events
{
    /// <summary>
    /// 事件分发
    /// </summary>
    public static class EventDispatcher
    {
        /// <summary>
        /// 分发事件
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="eventId">事件ID</param>
        /// <param name="args">参数对象,可以为null</param>
        public static void Dispatch(long actorId, int eventId, Param args = null)
        {
            var actor = ActorManager.GetActor(actorId);
            if (actor != null)
            {
                var evt = new Event
                          {
                              EventId = eventId,
                              Data    = args
                          };

                actor.Tell(async () =>
                           {
                               // 事件需要在本actor内执行，不可多线程执行，所以不能使用Task.WhenAll来处理
                               var listeners = HotfixManager.FindListeners(actor.Type, eventId);
                               if (listeners.IsNullOrEmpty())
                               {
                                   LogHelper.Warn($"事件：{eventId} 没有找到任何监听者");
                                   return;
                               }

                               foreach (var listener in listeners)
                               {
                                   var comp = await actor.GetComponentAgent(listener.AgentType);
                                   await listener.HandleEvent(comp, evt);
                               }
                           });
            }
        }
    }
}