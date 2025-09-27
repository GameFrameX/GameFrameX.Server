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

using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Core.Actors;
using GameFrameX.Core.Hotfix;
using GameFrameX.Foundation.Extensions;
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
    public static void Dispatch(long actorId, int eventId, GameEventArgs eventArgs = null)
    {
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
                    LogHelper.Warning($"事件：{eventId} 没有找到任何监听者");
                    return;
                }

                // 遍历所有监听器并执行事件处理
                foreach (var listener in listeners)
                {
                    // 获取监听器对应的组件代理
                    var componentAgent = await actor.GetComponentAgent(listener.AgentType, false);
                    try
                    {
                        // 调用监听器的事件处理方法
                        await listener.HandleEvent(componentAgent, eventArgs);
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
                    LogHelper.Warning($"事件：{eventId} 没有找到任何监听者");
                    return;
                }

                // 遍历所有监听器并执行事件处理
                foreach (var listener in listeners)
                {
                    try
                    {
                        // 调用监听器的事件处理方法
                        await listener.HandleEvent(eventArgs);
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