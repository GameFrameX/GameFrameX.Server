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

using System.Reflection;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Attribute;
using GameFrameX.Core.Actors;
using GameFrameX.Core.Hotfix;
using GameFrameX.Core.Utility;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Logger;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Core.Components;

/// <summary>
/// 组件注册器
/// </summary>
public static class ComponentRegister
{
    /// <summary>
    /// ActorType 到 CompTypeList 的映射
    /// </summary>
    private static readonly Dictionary<ushort, HashSet<Type>> ActorComponentDic = new();

    /// <summary>
    /// CompType 到 ActorType 的映射
    /// </summary>
    internal static readonly Dictionary<Type, ushort> ComponentActorDic = new();

    /// <summary>
    /// 功能码到 CompTypes 的映射
    /// </summary>
    private static readonly Dictionary<int, HashSet<Type>> FuncComponentDic = new();

    /// <summary>
    /// CompType 到功能码的映射
    /// </summary>
    private static readonly Dictionary<Type, short> ComponentFuncDic = new();

    /// <summary>
    /// 根据 CompType 获取对应的 ActorType 类型
    /// </summary>
    /// <param name="componentType">组件类型</param>
    /// <returns>ActorType 类型</returns>
    public static ushort GetActorType(Type componentType)
    {
        ComponentActorDic.TryGetValue(componentType, out var actorType);
        return actorType;
    }

    /// <summary>
    /// 根据 ActorType 类型获取对应的 CompTypes 列表
    /// </summary>
    /// <param name="actorType">ActorType 类型</param>
    /// <returns>CompTypes 列表</returns>
    public static IEnumerable<Type> GetComponents(ushort actorType)
    {
        ActorComponentDic.TryGetValue(actorType, out var comps);
        return comps;
    }

    /// <summary>
    /// 初始化组件注册器
    /// </summary>
    /// <param name="assembly">目标程序集</param>
    /// <returns>初始化任务</returns>
    /// <exception cref="Exception">当程序集为 null 时抛出</exception>
    public static Task Init(Assembly assembly = null)
    {
        if (assembly == null)
        {
            assembly = Assembly.GetEntryAssembly();
        }

        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));
        var baseCompName = typeof(BaseComponent);
        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            if (type.IsAbstract || !type.IsSubclassOf(baseCompName))
            {
                continue;
            }

            if (type.GetCustomAttribute(typeof(ComponentTypeAttribute)) is ComponentTypeAttribute compAttr)
            {
                var actorType = compAttr.Type;
                var compTypes = ActorComponentDic.GetOrAddValue(actorType);
                compTypes.Add(type);

                ComponentActorDic[type] = actorType;

                if (actorType < GlobalConst.ActorTypeSeparator)
                {
                    if (type.GetCustomAttribute(typeof(FuncAttribute)) is FuncAttribute funcAttr)
                    {
                        var set = FuncComponentDic.GetOrAddValue(funcAttr.Func);
                        set.Add(type);
                        ComponentFuncDic[type] = funcAttr.Func;
                    }
                }
            }
            else
            {
                throw new Exception($"component:[{type.FullName}] the actor type is not bound");
            }
        }

        LogHelper.Info("initialize component registration complete");
        return Task.CompletedTask;
    }

    /// <summary>
    /// 激活全局组件
    /// </summary>
    /// <returns>激活任务</returns>
    public static async Task ActiveGlobalComponents()
    {
        try
        {
            foreach (var (actorType, value) in ActorComponentDic)
            {
                foreach (var compType in value)
                {
                    var agentType = HotfixManager.GetAgentType(compType);
                    if (agentType == null)
                    {
                        LogHelper.Warning($"{compType}未实现Agent,请检查业务代码是否正确");
                    }

                    /*if (actorType > ActorType.Separator)
                    {
                        LogHelper.Info($"激活全局组件：{actorType} {compType}");
                        await ActorManager.GetComponentAgent(agentType, actorType);
                    }*/
                }

                if (actorType > GlobalConst.ActorTypeSeparator)
                {
                    LogHelper.Debug($"activate the global actor: {actorType}");
                    await ActorManager.GetOrNew(ActorIdGenerator.GetActorId(actorType));
                }
            }

            LogHelper.Debug("Activate the global component and check if the components all contain the Agent implementation completion");
        }
        catch (Exception)
        {
            LogHelper.Error("Activate the global component and detect if the components all contain the agent implementation failed");
            throw;
        }
    }

    /// <summary>
    /// 激活角色组件
    /// </summary>
    /// <param name="componentAgent">组件代理</param>
    /// <param name="openFuncSet">开放的功能集合</param>
    /// <returns>激活任务</returns>
    public static Task ActiveRoleComponents(IComponentAgent componentAgent, HashSet<short> openFuncSet)
    {
        return ActiveComponents(componentAgent.Owner.Actor,
                                t => !ComponentFuncDic.TryGetValue(t, out var func)
                                     || openFuncSet.Contains(func));
        //foreach (var compType in GetComps(ActorType.Role))
        //{
        //    bool active;
        //    if (CompFuncDic.TryGetValue(compType, out var func))
        //    {
        //        active = openFuncSet.Contains(func);
        //    }
        //    else
        //    {
        //        active = true;
        //    }
        //    if (active)
        //    {
        //        var agentType = HotfixMgr.GetAgentType(compType);
        //        await compAgent.GetCompAgent(agentType);
        //    }
        //}
    }

    /// <summary>
    /// 激活指定条件下的组件
    /// </summary>
    /// <param name="actor">演员</param>
    /// <param name="predict">条件判断函数</param>
    /// <returns>激活任务</returns>
    internal static async Task ActiveComponents(IActor actor, Func<Type, bool> predict = null)
    {
        var compTypes = GetComponents(actor.Type);

        if (compTypes != null)
        {
            foreach (var compType in compTypes)
            {
                if (predict == null || predict(compType))
                {
                    var agentType = HotfixManager.GetAgentType(compType);
                    try
                    {
                        await actor.GetComponentAgent(agentType);
                    }
                    catch (Exception e)
                    {
                        LogHelper.Fatal(e);
                        // throw;
                    }
                }
            }
        }
        else
        {
            LogHelper.Fatal($"get an actor that doesn't belong to this actor: [{actor.Type}] components");
        }
    }

    /// <summary>
    /// 创建组件实例
    /// </summary>
    /// <param name="actor">演员</param>
    /// <param name="compType">组件类型</param>
    /// <returns>创建的组件实例</returns>
    internal static BaseComponent CreateComponent(Actor actor, Type compType)
    {
        if (!ActorComponentDic.TryGetValue(actor.Type, out var compTypes))
        {
            throw new Exception($"get an actor that doesn't belong to this actor: [{actor.Type}] component:[{compType.FullName}]");
        }

        if (!compTypes.Contains(compType))
        {
            throw new Exception($"get an actor that doesn't belong to this actor: [{actor.Type}] component:[{compType.FullName}]");
        }

        var comp = (BaseComponent)Activator.CreateInstance(compType);
        if (comp != null)
        {
            comp.Actor = actor;
            return comp;
        }

        return default;
    }
}