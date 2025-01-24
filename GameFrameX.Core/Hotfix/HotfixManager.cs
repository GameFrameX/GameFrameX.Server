using System.Collections.Concurrent;
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
    internal static volatile bool DoingHotfix;

    private static volatile HotfixModule _module;

    private static AppSetting _baseSetting;

    private static readonly ConcurrentDictionary<int, HotfixModule> OldModuleMap = new();

    private static readonly List<IEventListener> EmptyListenerList = new();

    /// <summary>
    /// 热更新程序集
    /// </summary>
    public static Assembly HotfixAssembly
    {
        get { return _module?.HotfixAssembly; }
    }

    /// <summary>
    /// 重载时间
    /// </summary>
    public static DateTime ReloadTime { get; private set; }

    /// <summary>
    /// 加载热更新模块
    /// </summary>
    /// <param name="setting"></param>
    /// <param name="dllPath">热更新程序集路径，默认为hotfix</param>
    /// <param name="hotfixDllName">热更新程序集名称</param>
    /// <param name="dllVersion">Dll版本.当不为空的时候会优先加载指定的Dll.替换 dllPath 参数</param>
    /// <returns>返回是否成功</returns>
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

    private static async Task<bool> Load(HotfixModule newModule, AppSetting setting, bool reload)
    {
        ReloadTime = DateTime.Now;
        if (reload)
        {
            var oldModule = _module;
            DoingHotfix = true;
            var oldModuleHash = oldModule.GetHashCode();
            OldModuleMap.TryAdd(oldModuleHash, oldModule);
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
    /// 停止
    /// </summary>
    /// <returns></returns>
    public static Task Stop()
    {
        return _module?.HotfixBridge?.Stop() ?? Task.CompletedTask;
    }

    internal static Type GetAgentType(Type compType)
    {
        return _module.GetAgentType(compType);
    }

    internal static Type GetCompType(Type agentType)
    {
        return _module.GetComponentType(agentType);
    }

    /// <summary>
    /// 获取代理
    /// </summary>
    /// <param name="component"></param>
    /// <param name="refAssemblyType"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
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
    /// <param name="msgId"></param>
    /// <returns></returns>
    public static BaseMessageHandler GetTcpHandler(int msgId)
    {
        return _module.GetTcpHandler(msgId);
    }

    /// <summary>
    /// 获取HTTP消息处理器
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public static BaseHttpHandler GetHttpHandler(string cmd)
    {
        return _module.GetHttpHandler(cmd);
    }

    /// <summary>
    /// 获取HTTP消息处理器
    /// </summary>
    /// <returns></returns>
    public static List<BaseHttpHandler> GetListHttpHandler()
    {
        return _module.GetListHttpHandler();
    }

    /// <summary>
    /// 获取事件监听器列表
    /// </summary>
    /// <param name="actorType"></param>
    /// <param name="evtId"></param>
    /// <returns></returns>
    public static List<IEventListener> FindListeners(ushort actorType, int evtId)
    {
        return _module.FindListeners(actorType, evtId) ?? EmptyListenerList;
    }

    /// <summary>
    /// 获取实例
    /// 主要用于获取Event,Timer, Schedule,的Handler实例
    /// </summary>
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