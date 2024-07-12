using GameFrameX.Core.Actors;
using GameFrameX.Core.Hotfix;
using GameFrameX.Extension;
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
        /// <param name="id"></param>
        /// <param name="eventId"></param>
        /// <param name="args"></param>
        public static void Dispatch(long id, int eventId, Param args = null)
        {
            var actor = ActorManager.GetActor(id);
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
                                   // Log.Warn($"事件：{(EventID)evtId} 没有找到任何监听者");
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