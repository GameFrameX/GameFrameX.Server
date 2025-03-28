﻿using System.Collections.Concurrent;
using System.Reflection;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Core.BaseHandler;
using GameFrameX.Core.Components;
using GameFrameX.NetWork.HTTP;
using GameFrameX.Utility.Extensions;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Core.Hotfix;

/// <summary>
/// 热更新管理器
/// </summary>
public static class HotfixManager
{
    /// <summary>
    /// 标识是否正在进行热更新操作
    /// </summary>
    internal static volatile bool DoingHotfix;

    /// <summary>
    /// 当前使用的热更新模块
    /// </summary>
    private static volatile HotfixModule _module;

    /// <summary>
    /// 基础配置信息
    /// </summary>
    private static AppSetting _baseSetting;

    /// <summary>
    /// 存储旧的热更新模块的映射表，用于处理热更新过渡期间的请求
    /// </summary>
    private static readonly ConcurrentDictionary<int, HotfixModule> OldModuleMap = new();

    /// <summary>
    /// 空的事件监听器列表，用于在找不到监听器时返回
    /// </summary>
    private static readonly List<IEventListener> EmptyListenerList = new();

    /// <summary>
    /// 热更新程序集
    /// </summary>
    public static Assembly HotfixAssembly
    {
        get { return _module?.HotfixAssembly; }
    }

    /// <summary>
    /// 最近一次热更新重载的时间
    /// </summary>
    public static DateTime ReloadTime { get; private set; }

    /// <summary>
    /// 加载热更新模块
    /// </summary>
    /// <param name="setting">应用程序配置</param>
    /// <param name="dllPath">热更新程序集路径，默认为hotfix</param>
    /// <param name="hotfixDllName">热更新程序集名称</param>
    /// <param name="dllVersion">Dll版本.当不为空的时候会优先加载指定的Dll.替换 dllPath 参数</param>
    /// <returns>返回是否加载成功</returns>
    public static async Task<bool> LoadHotfixModule(AppSetting setting, string dllVersion = "", string dllPath = "hotfix", string hotfixDllName = "GameFrameX.Hotfix.dll")
    {
        dllPath.CheckNotNullOrEmptyOrWhiteSpace(nameof(dllPath));
        hotfixDllName.CheckNotNullOrEmptyOrWhiteSpace(nameof(hotfixDllName));
        if (setting != null)
        {
            _baseSetting = setting;
        }

        var path = Path.Combine(Environment.CurrentDirectory, string.IsNullOrEmpty(dllVersion) ? dllPath : $"{dllVersion}", hotfixDllName);
        var hotfixModule = new HotfixModule(path);
        var reload = _module != null;
        // 起服时失败会有异常抛出
        var success = hotfixModule.Init(reload);
        if (!success)
        {
            return false;
        }

        return await Load(hotfixModule, _baseSetting, reload);
    }

    /// <summary>
    /// 加载新的热更新模块
    /// </summary>
    /// <param name="newModule">新的热更新模块</param>
    /// <param name="setting">应用程序配置</param>
    /// <param name="reload">是否为重新加载</param>
    /// <returns>返回加载是否成功</returns>
    private static async Task<bool> Load(HotfixModule newModule, AppSetting setting, bool reload)
    {
        ReloadTime = DateTime.Now;
        if (reload)
        {
            var oldModule = _module;
            DoingHotfix = true;
            var oldModuleHash = oldModule.GetHashCode();
            OldModuleMap.TryAdd(oldModuleHash, oldModule);
            // 延迟3分钟后清理旧模块
            _ = Task.Run(async () =>
            {
                await Task.Delay(1000 * 60 * 3);
                OldModuleMap.TryRemove(oldModuleHash, out _);
                oldModule.Unload();
                DoingHotfix = !OldModuleMap.IsEmpty;
            });
        }

        _module = newModule;
        if (_module.HotfixBridge != null)
        {
            return await _module.HotfixBridge.OnLoadSuccess(setting, reload);
        }

        return true;
    }

    /// <summary>
    /// 停止热更新模块
    /// </summary>
    /// <param name="message">停止原因</param>
    /// <returns></returns>
    public static Task Stop(string message = "")
    {
        return _module?.HotfixBridge?.Stop(message) ?? Task.CompletedTask;
    }

    /// <summary>
    /// 获取组件对应的代理类型
    /// </summary>
    internal static Type GetAgentType(Type compType)
    {
        return _module.GetAgentType(compType);
    }

    /// <summary>
    /// 获取代理对应的组件类型
    /// </summary>
    internal static Type GetCompType(Type agentType)
    {
        return _module.GetComponentType(agentType);
    }

    /// <summary>
    /// 获取组件的代理实例
    /// </summary>
    /// <param name="component">组件实例</param>
    /// <param name="refAssemblyType">引用程序集类型</param>
    /// <typeparam name="T">代理类型</typeparam>
    /// <returns>返回代理实例</returns>
    public static T GetAgent<T>(BaseComponent component, Type refAssemblyType) where T : IComponentAgent
    {
        if (!OldModuleMap.IsEmpty)
        {
            var asb = typeof(T).Assembly;
            var asb2 = refAssemblyType?.Assembly;
            foreach (var kv in OldModuleMap)
            {
                var old = kv.Value;
                if (asb == old.HotfixAssembly || asb2 == old.HotfixAssembly)
                {
                    return old.GetAgent<T>(component);
                }
            }
        }

        return _module.GetAgent<T>(component);
    }

    /// <summary>
    /// 获取TCP消息处理器
    /// </summary>
    /// <param name="msgId">消息ID</param>
    /// <returns>返回对应的消息处理器</returns>
    public static BaseMessageHandler GetTcpHandler(int msgId)
    {
        return _module.GetTcpHandler(msgId);
    }

    /// <summary>
    /// 获取HTTP消息处理器
    /// </summary>
    /// <param name="cmd">HTTP命令</param>
    /// <returns>返回对应的HTTP处理器</returns>
    public static BaseHttpHandler GetHttpHandler(string cmd)
    {
        return _module.GetHttpHandler(cmd);
    }

    /// <summary>
    /// 获取所有HTTP消息处理器列表
    /// </summary>
    /// <returns>返回HTTP处理器列表</returns>
    public static List<BaseHttpHandler> GetListHttpHandler()
    {
        return _module.GetListHttpHandler();
    }

    /// <summary>
    /// 获取指定Actor类型和事件ID的事件监听器列表
    /// </summary>
    /// <param name="actorType">Actor类型</param>
    /// <param name="eventId">事件ID</param>
    /// <returns>返回监听器列表，如果没有则返回空列表</returns>
    public static List<IEventListener> FindListeners(ushort actorType, int eventId)
    {
        return _module.FindListeners(actorType, eventId) ?? EmptyListenerList;
    }

    /// <summary>
    /// 获取指定事件ID的事件监听器列表
    /// </summary>
    /// <param name="eventId">事件ID</param>
    /// <returns>返回监听器列表，如果没有则返回空列表</returns>
    public static List<IEventListener> FindListeners(int eventId)
    {
        return _module.FindListeners(eventId) ?? EmptyListenerList;
    }

    /// <summary>
    /// 获取指定类型的实例
    /// 主要用于获取Event,Timer, Schedule的Handler实例
    /// </summary>
    /// <typeparam name="T">实例类型</typeparam>
    /// <param name="typeName">类型名称</param>
    /// <param name="refAssemblyType">引用程序集类型</param>
    /// <returns>返回指定类型的实例，如果类型名称为空则返回默认值</returns>
    public static T GetInstance<T>(string typeName, Type refAssemblyType = null)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return default;
        }

        if (OldModuleMap.Count > 0)
        {
            var asb = refAssemblyType?.Assembly;
            foreach (var kv in OldModuleMap)
            {
                var old = kv.Value;
                if (asb == old.HotfixAssembly)
                {
                    return old.GetInstance<T>(typeName);
                }
            }
        }

        return _module.GetInstance<T>(typeName);
    }
}