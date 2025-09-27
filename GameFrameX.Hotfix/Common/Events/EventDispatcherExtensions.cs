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

using GameFrameX.Apps.Common.Event;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Logger;
using GameFrameX.Hotfix.Logic.Server;
using GameFrameX.Utility.Setting;
using GameEventArgs = GameFrameX.Core.Abstractions.Events.GameEventArgs;

namespace GameFrameX.Hotfix.Common.Events;

public static class EventDispatcherExtensions
{
    /// <summary>
    /// 分发事件
    /// </summary>
    /// <param name="agent">代理对象</param>
    /// <param name="eventId">事件ID</param>
    /// <param name="gameEventArgs">事件参数,可以为null</param>
    public static void Dispatch(this IComponentAgent agent, int eventId, GameEventArgs gameEventArgs = null)
    {
        // 自己处理
        SelfHandle(agent, eventId, gameEventArgs);

        if ((EventId)eventId > EventId.RoleSeparator && agent.OwnerType > GlobalConst.ActorTypeSeparator)
        {
            // 全局非玩家事件，抛给所有玩家
            agent.Tell(()
                           =>
                       {
                           return ServerComponentAgent.OnlineRoleForeach(role
                                                                             =>
                                                                         {
                                                                             role.Dispatch(eventId, gameEventArgs);
                                                                         });
                       });
        }
    }

    private static void SelfHandle(IComponentAgent agent, int evtId, GameEventArgs gameEventArgs)
    {
        agent.Tell(async () =>
        {
            // 事件需要在本actor内执行，不可多线程执行，所以不能使用Task.WhenAll来处理
            var listeners = HotfixManager.FindListeners(agent.OwnerType, evtId);
            if (listeners.IsNullOrEmpty())
            {
                LogHelper.Warning($"事件ID：{evtId} 没有找到任何监听者");
                return;
            }

            foreach (var listener in listeners)
            {
                var componentAgent = await agent.GetComponentAgent(listener.AgentType, false);
                try
                {
                    await listener.HandleEvent(componentAgent, gameEventArgs);
                }
                catch (Exception exception)
                {
                    LogHelper.Error(exception);
                }
            }
        });
    }

    /// <summary>
    /// 分发事件
    /// </summary>
    /// <param name="agent">代理对象</param>
    /// <param name="eventId">事件ID</param>
    /// <param name="args">事件参数</param>
    public static void Dispatch(this IComponentAgent agent, EventId eventId, GameEventArgs args = null)
    {
        Dispatch(agent, (int)eventId, args);
    }
}