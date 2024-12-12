using System.Reflection;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Attribute;
using GameFrameX.Core.Actors;
using GameFrameX.Core.Hotfix;
using GameFrameX.Core.Utility;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.Setting;

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

        assembly.CheckNotNull(nameof(assembly));
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
                var compTypes = ActorComponentDic.GetOrAdd(actorType);
                compTypes.Add(type);

                ComponentActorDic[type] = actorType;

                if (actorType < GlobalConst.ActorTypeSeparator)
                {
                    if (type.GetCustomAttribute(typeof(FuncAttribute)) is FuncAttribute funcAttr)
                    {
                        var set = FuncComponentDic.GetOrAdd(funcAttr.Func);
                        set.Add(type);
                        ComponentFuncDic[type] = funcAttr.Func;
                    }
                }
            }
            else
            {
                throw new Exception($"comp:{type.FullName}未绑定actor类型");
            }
        }

        LogHelper.Info($"初始化组件注册完成");
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
                        LogHelper.Warn($"{compType}未实现Agent");
                    }

                    /*if (actorType > ActorType.Separator)
                    {
                        LogHelper.Info($"激活全局组件：{actorType} {compType}");
                        await ActorManager.GetComponentAgent(agentType, actorType);
                    }*/
                }

                if (actorType > GlobalConst.ActorTypeSeparator)
                {
                    LogHelper.Info($"激活全局Actor: {actorType}");
                    await ActorManager.GetOrNew(ActorIdGenerator.GetActorId(actorType));
                }
            }

            LogHelper.Info($"激活全局组件并检测组件是否都包含Agent实现完成");
        }
        catch (Exception)
        {
            LogHelper.Error($"激活全局组件并检测组件是否都包含Agent实现失败");
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
                        LogHelper.Info(e);
                        // throw;
                    }
                }
            }
        }
        else
        {
            LogHelper.Fatal($"获取不属于此actor：{actor.Type}的组件");
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
            throw new Exception($"获取不属于此actor：{actor.Type}的Component:{compType.FullName}");
        }

        if (!compTypes.Contains(compType))
        {
            throw new Exception($"获取不属于此actor：{actor.Type}的Component:{compType.FullName}");
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