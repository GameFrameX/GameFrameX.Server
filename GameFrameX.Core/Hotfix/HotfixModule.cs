using System.Collections.Concurrent;
using System.Reflection;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Core.BaseHandler;
using GameFrameX.Core.Components;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.HTTP;
using GameFrameX.Utility.Extensions;
using GameFrameX.Foundation.Logger;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Core.Hotfix;

/// <summary>
/// 热更模块，负责热更新相关的初始化、卸载、解析DLL等工作。
/// </summary>
internal sealed class HotfixModule
{
    /// <summary>
    /// 角色类型到事件ID到监听者的映射。
    /// </summary>
    private readonly Dictionary<ushort, Dictionary<int, List<IEventListener>>> _actorEvtListeners = new Dictionary<ushort, Dictionary<int, List<IEventListener>>>(512);

    /// <summary>
    /// 代理类型到代理包装类型的映射。
    /// </summary>
    private readonly Dictionary<Type, Type> _agentAgentWrapperMap = new Dictionary<Type, Type>(512);

    /// <summary>
    /// 组件类型到代理类型的映射。
    /// </summary>
    private readonly Dictionary<Type, Type> _agentCompMap = new Dictionary<Type, Type>(512);

    /// <summary>
    /// 组件类型到代理类型的映射。
    /// </summary>
    private readonly Dictionary<Type, Type> _compAgentMap = new Dictionary<Type, Type>(512);

    /// <summary>
    /// DLL路径。
    /// </summary>
    private readonly string _dllPath;

    /// <summary>
    /// HTTP命令到处理器的映射。
    /// </summary>
    private readonly Dictionary<string, BaseHttpHandler> _httpHandlerMap = new Dictionary<string, BaseHttpHandler>(512);

    /// <summary>
    /// RPC请求类型到响应类型的映射。
    /// </summary>
    private readonly Dictionary<Type, Type> _rpcHandlerMap = new Dictionary<Type, Type>(512);

    /// <summary>
    /// 消息ID到处理器类型的映射。
    /// </summary>
    private readonly Dictionary<int, Type> _tcpHandlerMap = new Dictionary<int, Type>(512);

    /// <summary>
    /// 消息处理类型列表
    /// </summary>
    private readonly List<Type> _tcpHandlerTypes = new List<Type>(512);

    /// <summary>
    /// 类型缓存。
    /// </summary>
    private readonly ConcurrentDictionary<string, object> _typeCacheMap = new ConcurrentDictionary<string, object>();

    /// <summary>
    /// 是否使用代理包装。
    /// </summary>
    private readonly bool _useAgentWrapper = true;

    /// <summary>
    /// DLL加载器。
    /// </summary>
    private DllLoader _dllLoader;

    /// <summary>
    /// 热更程序集。
    /// </summary>
    internal Assembly HotfixAssembly;

    /// <summary>
    /// 构造函数，接受DLL路径。
    /// </summary>
    /// <param name="dllPath">DLL路径。</param>
    internal HotfixModule(string dllPath)
    {
        _dllPath = dllPath;
    }

    /// <summary>
    /// 默认构造函数，初始化热更程序集并解析DLL。
    /// </summary>
    internal HotfixModule()
    {
        HotfixAssembly = Assembly.GetEntryAssembly();
        ParseDll();
    }

    /// <summary>
    /// 热更桥接接口。
    /// </summary>
    internal IHotfixBridge HotfixBridge { get; private set; }

    /// <summary>
    /// 初始化热更模块。
    /// </summary>
    /// <param name="reload">是否重新加载。</param>
    /// <returns>初始化是否成功。</returns>
    internal bool Init(bool reload)
    {
        var success = false;
        try
        {
            _dllLoader = new DllLoader(_dllPath);
            HotfixAssembly = _dllLoader.HotfixDll;
            if (!reload)
            {
                // 启动服务器时加载关联的DLL
                LoadRefAssemblies();
            }

            ParseDll();

            LogHelper.Info($"热更DLL初始化成功: {_dllPath}");
            success = true;
        }
        catch (Exception e)
        {
            LogHelper.Error($"热更DLL初始化失败...\n{e}");
            if (!reload)
            {
                throw;
            }
        }

        return success;
    }

    /// <summary>
    /// 卸载热更模块。
    /// </summary>
    public void Unload()
    {
        if (_dllLoader != null)
        {
            var weak = _dllLoader.Unload();
            if (GlobalSettings.IsDebug)
            {
                // 检查热更DLL是否已经释放
                Task.Run(async () =>
                {
                    var tryCount = 0;
                    while (weak.IsAlive && tryCount++ < 10)
                    {
                        await Task.Delay(100);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                    LogHelper.Warn($"热更DLL卸载{(weak.IsAlive ? "失败" : "成功")}");
                });
            }
        }
    }

    /// <summary>
    /// 加载引用的程序集。
    /// </summary>
    private void LoadRefAssemblies()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var nameSet = new HashSet<string>(assemblies.Select(t => t.GetName().Name));
        var hotfixRefAssemblies = HotfixAssembly.GetReferencedAssemblies();
        foreach (var refAssembly in hotfixRefAssemblies)
        {
            if (nameSet.Contains(refAssembly.Name))
            {
                continue;
            }

            var refPath = $"{Environment.CurrentDirectory}/{refAssembly.Name}.dll";
            if (File.Exists(refPath))
            {
                Assembly.LoadFrom(refPath);
            }
        }
    }

    /// <summary>
    /// 解析DLL中的类型并进行注册。
    /// </summary>
    private void ParseDll()
    {
        var fullName = typeof(IHotfixBridge).FullName;
        var types = HotfixAssembly.GetTypes();
        foreach (var type in types)
        {
            if (!AddAgent(type)
                && !AddEvent(type)
                && !AddTcpHandler(type)
                && !AddHttpHandler(type))
            {
                if (HotfixBridge.IsNull() && type.GetInterface(fullName) != null)
                {
                    var bridge = (IHotfixBridge)Activator.CreateInstance(type);

                    HotfixBridge = bridge;
                }
            }

            AddRpcHandler(type);
        }
    }

    /// <summary>
    /// 添加HTTP处理器。
    /// </summary>
    /// <param name="type">处理器类型。</param>
    /// <returns>是否添加成功。</returns>
    private bool AddHttpHandler(Type type)
    {
        if (!type.IsSubclassOf(typeof(BaseHttpHandler)))
        {
            return false;
        }

        var attr = (HttpMessageMappingAttribute)type.GetCustomAttribute(typeof(HttpMessageMappingAttribute));
        if (attr.IsNull())
        {
            // 不是最终实现类
            return true;
        }

        var handler = (BaseHttpHandler)Activator.CreateInstance(type);
        // 注册原始命令
        if (!_httpHandlerMap.TryAdd(attr.OriginalCmd, handler))
        {
            throw new Exception($"HTTP处理器命令重复注册，命令:{attr.OriginalCmd}");
        }

        // 注册标准化的命名
        if (!_httpHandlerMap.TryAdd(attr.StandardCmd, handler))
        {
            throw new Exception($"HTTP处理器命令重复注册，命令:{attr.OriginalCmd}");
        }

        return true;
    }

    /// <summary>
    /// 添加RPC处理器。
    /// </summary>
    /// <param name="type">处理器类型。</param>
    /// <returns>是否添加成功。</returns>
    private bool AddRpcHandler(Type type)
    {
        var attribute = (MessageRpcMappingAttribute)type.GetCustomAttribute(typeof(MessageRpcMappingAttribute), true);
        if (attribute.IsNull())
        {
            return false;
        }

        var isHas = _rpcHandlerMap.TryGetValue(attribute.RequestMessage.GetType(), out var requestHandler);
        if (isHas && requestHandler?.GetType() == attribute.ResponseMessage.GetType())
        {
            LogHelper.Error($"重复注册消息RPC处理器:[{attribute.RequestMessage}] 消息:[{attribute.ResponseMessage}]");
            return false;
        }

        _rpcHandlerMap.Add(attribute.RequestMessage.GetType(), attribute.ResponseMessage.GetType());

        return true;
    }

    /// <summary>
    /// 添加TCP处理器。
    /// </summary>
    /// <param name="type">处理器类型。</param>
    /// <returns>是否添加成功。</returns>
    private bool AddTcpHandler(Type type)
    {
        var attribute = (MessageMappingAttribute)type.GetCustomAttribute(typeof(MessageMappingAttribute), true);
        if (attribute.IsNull())
        {
            return false;
        }

        var classFullName = type.FullName;
        if (classFullName == null)
        {
            return false;
        }

        if (!type.IsSealed)
        {
            throw new InvalidOperationException($"{classFullName} 必须是标记为sealed的类");
        }

        if (!classFullName.EndsWith(GlobalConst.ComponentHandlerNameSuffix))
        {
            throw new Exception($"组件代理必须以[{GlobalConst.ComponentHandlerNameSuffix}]结尾，{classFullName}");
        }

        if (_tcpHandlerTypes.Contains(attribute.MessageType))
        {
            LogHelper.Error($"重复注册消息TCP处理器 类型:[{type.FullName}]");
            return false;
        }

        var msgIdField = (MessageTypeHandlerAttribute)attribute.MessageType.GetCustomAttribute(typeof(MessageTypeHandlerAttribute), true);
        if (msgIdField.IsNull())
        {
            return false;
        }

        var msgId = msgIdField.MessageId;
        if (!_tcpHandlerMap.TryAdd(msgId, type))
        {
            LogHelper.Error($"重复注册消息TCP处理器:[{msgId}] 消息:[{type}]");
        }

        _tcpHandlerTypes.Add(attribute.MessageType);
        return true;
    }

    /// <summary>
    /// 添加事件监听者。
    /// </summary>
    /// <param name="type">监听者类型。</param>
    /// <returns>是否添加成功。</returns>
    private bool AddEvent(Type type)
    {
        if (!type.IsImplWithInterface(typeof(IEventListener)))
        {
            return false;
        }

        var compAgentType = type.BaseType.GetGenericArguments()[0];
        var compType = compAgentType.BaseType.GetGenericArguments()[0];
        var actorType = ComponentRegister.ComponentActorDic[compType];
        var evtListenersDic = _actorEvtListeners.GetOrAdd(actorType);

        var find = false;
        foreach (var attr in type.GetCustomAttributes())
        {
            if (attr is EventInfoAttribute evt)
            {
                find = true;

                var evtId = evt.EventId;
                var listeners = evtListenersDic.GetOrAdd(evtId);
                listeners.Add((IEventListener)Activator.CreateInstance(type));
            }
        }

        if (!find)
        {
            throw new Exception($"IEventListener:{type.FullName}没有指定监听的事件");
        }

        return true;
    }

    /// <summary>
    /// 添加组件代理。
    /// </summary>
    /// <param name="type">代理类型。</param>
    /// <returns>是否添加成功。</returns>
    private bool AddAgent(Type type)
    {
        type.CheckNotNull(nameof(type));
        if (!type.IsImplWithInterface(typeof(IComponentAgent)))
        {
            return false;
        }

        var fullName = type.FullName;
        if (fullName == "GameFrameX.Launcher.Logic.Server.ServerComp")
        {
            return false;
        }

        // 这里处理SourceGeneratedCode 生成的代码的 Warp 对象,注意命名空间的识别
        if (fullName.StartsWith(GlobalConst.HotfixNameSpaceNamePrefix) && fullName.EndsWith(GlobalConst.ComponentAgentWrapperNameSuffix))
        {
            _agentAgentWrapperMap[type.BaseType] = type;
            return true;
        }

        if (!fullName.EndsWith(GlobalConst.ComponentAgentNameSuffix))
        {
            throw new Exception($"组件代理必须以{GlobalConst.ComponentAgentNameSuffix}结尾，{fullName}");
        }

        var compType = type.BaseType.GetGenericArguments()[0];
        if (!_compAgentMap.TryAdd(compType, type))
        {
            throw new Exception($"组件:{compType.FullName}有多个代理");
        }

        _agentCompMap[type] = compType;
        return true;
    }

    /// <summary>
    /// 获取TCP处理器。
    /// </summary>
    /// <param name="msgId">消息ID。</param>
    /// <returns>TCP处理器实例。</returns>
    internal BaseMessageHandler GetTcpHandler(int msgId)
    {
        if (_tcpHandlerMap.TryGetValue(msgId, out var handlerType))
        {
            var instance = Activator.CreateInstance(handlerType);
            if (instance is BaseMessageHandler handler)
            {
                return handler;
            }

            throw new Exception($"错误的TCP处理器类型，{instance.GetType().FullName}");
        }

        return null;
        //throw new HandlerNotFoundException($"消息ID：{msgId}");
    }

    /// <summary>
    /// 获取HTTP处理器。
    /// </summary>
    /// <param name="cmd">命令。</param>
    /// <returns>HTTP处理器实例。</returns>
    internal BaseHttpHandler GetHttpHandler(string cmd)
    {
        if (_httpHandlerMap.TryGetValue(cmd, out var handler))
        {
            return handler;
        }

        return null;
        // throw new HttpHandlerNotFoundException($"未注册的HTTP命令:{cmd}");
    }

    /// <summary>
    /// 获取HTTP处理器列表。
    /// </summary>
    /// <returns>HTTP处理器列表。</returns>
    internal List<BaseHttpHandler> GetListHttpHandler()
    {
        var values = _httpHandlerMap.Values;
        List<BaseHttpHandler> list = new List<BaseHttpHandler>(values.Count / 2);
        foreach (var handler in values)
        {
            if (list.Contains(handler))
            {
                continue;
            }

            list.Add(handler);
        }

        return list;
    }

    /// <summary>
    /// 获取组件代理。
    /// </summary>
    /// <param name="component">组件实例。</param>
    /// <typeparam name="T">代理类型。</typeparam>
    /// <returns>代理实例。</returns>
    internal T GetAgent<T>(BaseComponent component) where T : IComponentAgent
    {
        var type = component.GetType();
        if (_compAgentMap.TryGetValue(type, out var agentType))
        {
            T agent = default;
            if (_useAgentWrapper)
            {
                if (_agentAgentWrapperMap.TryGetValue(agentType, out var warpType))
                {
                    agent = (T)Activator.CreateInstance(warpType);
                }
            }

            if (agent.IsNull())
            {
                agent = (T)Activator.CreateInstance(agentType);
            }

            if (agent.IsNull())
            {
                throw new ArgumentNullException(nameof(agent));
            }

            agent.SetOwner(component);
            return agent;
        }

        throw new KeyNotFoundException(nameof(_compAgentMap) + " ===>" + nameof(type));
    }

    /// <summary>
    /// 查找事件监听者。
    /// </summary>
    /// <param name="actorType">角色类型。</param>
    /// <param name="evtId">事件ID。</param>
    /// <returns>事件监听者列表。</returns>
    internal List<IEventListener> FindListeners(ushort actorType, int evtId)
    {
        if (_actorEvtListeners.TryGetValue(actorType, out var evtListeners)
            && evtListeners.TryGetValue(evtId, out var listeners))
        {
            return listeners;
        }

        return null;
    }

    /// <summary>
    /// 查找事件监听者。
    /// </summary>
    /// <param name="eventId">事件ID。</param>
    /// <returns>事件监听者列表。</returns>
    internal List<IEventListener> FindListeners(int eventId)
    {
        var listenerList = new List<IEventListener>(32);
        foreach (var actorEvtListener in _actorEvtListeners)
        {
            if (actorEvtListener.Value.TryGetValue(eventId, out var listeners))
            {
                listenerList.AddRange(listeners);
            }
        }

        return listenerList;
    }

    /// <summary>
    /// 获取实例（主要用于获取Event, Timer, Schedule的处理器实例）。
    /// </summary>
    /// <param name="typeName">类型名称。</param>
    /// <typeparam name="T">实例类型。</typeparam>
    /// <returns>实例对象。</returns>
    internal T GetInstance<T>(string typeName)
    {
        return (T)_typeCacheMap.GetOrAdd(typeName, k => HotfixAssembly.CreateInstance(k));
    }

    /// <summary>
    /// 获取代理类型。
    /// </summary>
    /// <param name="compType">组件类型。</param>
    /// <returns>代理类型。</returns>
    internal Type GetAgentType(Type compType)
    {
        _compAgentMap.TryGetValue(compType, out var agentType);
        return agentType;
    }

    /// <summary>
    /// 获取组件类型。
    /// </summary>
    /// <param name="agentType">代理类型。</param>
    /// <returns>组件类型。</returns>
    internal Type GetComponentType(Type agentType)
    {
        _agentCompMap.TryGetValue(agentType, out var compType);
        return compType;
    }
}