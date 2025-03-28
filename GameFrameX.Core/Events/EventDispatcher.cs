using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Core.Actors;
using GameFrameX.Core.Hotfix;
using GameFrameX.Utility;
using GameFrameX.Utility.Extensions;
using GameFrameX.Foundation.Logger;

namespace GameFrameX.Core.Events;

/// <summary>
/// 事件分发类 - 负责处理游戏中所有事件的分发和处理
/// </summary>
public static class EventDispatcher
{
    /// <summary>
    /// 分发事件到指定的Actor或全局监听器
    /// </summary>
    /// <param name="actorId">目标Actor的唯一标识符，如果为无效值则分发到全局监听器</param>
    /// <param name="eventId">要分发的事件ID</param>
    /// <param name="eventArgs">事件携带的参数数据，可以为null</param>
    public static void Dispatch(long actorId, int eventId, Param eventArgs = null)
    {
        // 构造事件参数对象
        var gameEventArgs = new GameEventArgs
        {
            EventId = eventId,
            Data = eventArgs,
        };

        // 尝试获取目标Actor
        var actor = ActorManager.GetActor(actorId);
        if (actor != null)
        {
            // 定义在Actor上下文中执行的异步任务
            async Task Work()
            {
                // 事件需要在本actor内执行，不可多线程执行，所以不能使用Task.WhenAll来处理
                // 获取该Actor类型下注册的所有事件监听器
                var listeners = HotfixManager.FindListeners(actor.Type, eventId);
                if (listeners.IsNullOrEmpty())
                {
                    LogHelper.Warn($"事件：{eventId} 没有找到任何监听者");
                    return;
                }

                // 遍历所有监听器并执行事件处理
                foreach (var listener in listeners)
                {
                    // 获取监听器对应的组件代理
                    var comp = await actor.GetComponentAgent(listener.AgentType);
                    try
                    {
                        // 调用监听器的事件处理方法
                        await listener.HandleEvent(comp, gameEventArgs);
                    }
                    catch (Exception exception)
                    {
                        // 捕获并记录事件处理过程中的异常
                        LogHelper.Error(exception);
                    }
                }
            }

            // 将工作任务提交到Actor的消息队列中
            actor.Tell(Work);
        }
        else
        {
            // 当Actor不存在时，定义在全局上下文中执行的异步任务
            async Task Work()
            {
                // 事件需要在本actor内执行，不可多线程执行，所以不能使用Task.WhenAll来处理
                // 获取全局注册的事件监听器
                var listeners = HotfixManager.FindListeners(eventId);
                if (listeners.IsNullOrEmpty())
                {
                    LogHelper.Warn($"事件：{eventId} 没有找到任何监听者");
                    return;
                }

                // 遍历所有监听器并执行事件处理
                foreach (var listener in listeners)
                {
                    try
                    {
                        // 调用监听器的事件处理方法
                        await listener.HandleEvent(gameEventArgs);
                    }
                    catch (Exception exception)
                    {
                        // 捕获并记录事件处理过程中的异常
                        LogHelper.Error(exception);
                    }
                }
            }

            // 在新的线程中执行全局事件处理
            Task.Run(Work);
        }
    }
}