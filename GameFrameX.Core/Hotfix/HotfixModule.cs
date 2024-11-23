using System.Collections.Concurrent;
using System.Reflection;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Core.BaseHandler;
using GameFrameX.Core.Components;
using GameFrameX.Core.Events;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Messages;
using GameFrameX.Setting;

namespace GameFrameX.Core.Hotfix
{
    internal class HotfixModule
    {
        private DllLoader _dllLoader = null;
        readonly string _dllPath;

        internal IHotfixBridge HotfixBridge { get; private set; }

        internal Assembly HotfixAssembly = null;

        /// <summary>
        /// comp -> compAgent
        /// </summary>
        private readonly Dictionary<Type, Type> _agentCompMap = new Dictionary<Type, Type>(512);

        readonly Dictionary<Type, Type> _compAgentMap = new Dictionary<Type, Type>(512);

        private readonly Dictionary<Type, Type> _agentAgentWrapperMap = new Dictionary<Type, Type>(512);

        /// <summary>
        /// cmd -> handler
        /// </summary>
        private readonly Dictionary<string, BaseHttpHandler> _httpHandlerMap = new Dictionary<string, BaseHttpHandler>(512);

        /// <summary>
        /// msgId -> handler
        /// </summary>
        private readonly Dictionary<int, Type> _tcpHandlerMap = new Dictionary<int, Type>(512);

        private readonly Dictionary<Type, Type> _rpcHandlerMap = new Dictionary<Type, Type>();
        private readonly ConcurrentDictionary<string, object> _typeCacheMap = new();

        /// <summary>
        /// actorType -> evtId -> listeners
        /// </summary>
        private readonly Dictionary<ActorType, Dictionary<int, List<IEventListener>>> _actorEvtListeners = new Dictionary<ActorType, Dictionary<int, List<IEventListener>>>(512);

        readonly bool _useAgentWrapper = true;

        internal HotfixModule(string dllPath)
        {
            _dllPath = dllPath;
        }

        internal HotfixModule()
        {
            HotfixAssembly = Assembly.GetEntryAssembly();

            ParseDll();
        }

        internal bool Init(bool reload)
        {
            bool success = false;
            try
            {
                _dllLoader = new DllLoader(_dllPath);
                HotfixAssembly = _dllLoader.HotfixDll;
                if (!reload)
                {
                    // 启动服务器时加载关联的dll
                    LoadRefAssemblies();
                }

                ParseDll();

                File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "dllPath.txt"), _dllPath);

                LogHelper.Info($"hotfix dll init success: {_dllPath}");
                success = true;
            }
            catch (Exception e)
            {
                LogHelper.Error($"hotfix dll init failed...\n{e}");
                if (!reload)
                {
                    throw;
                }
            }

            return success;
        }

        public void Unload()
        {
            if (_dllLoader != null)
            {
                var weak = _dllLoader.Unload();
                if (GlobalSettings.IsDebug)
                {
                    //检查hotfix dll是否已经释放
                    Task.Run(async () =>
                    {
                        int tryCount = 0;
                        while (weak.IsAlive && tryCount++ < 10)
                        {
                            await Task.Delay(100);
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }

                        LogHelper.Warn($"hotfix dll unloaded {(weak.IsAlive ? "failed" : "success")}");
                    });
                }
            }
        }

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
                    if ((HotfixBridge.IsNull() && type.GetInterface(fullName) != null))
                    {
                        var bridge = (IHotfixBridge)Activator.CreateInstance(type);

                        HotfixBridge = bridge;
                    }
                }

                AddRpcHandler(type);
            }
        }

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
                throw new Exception($"http handler cmd重复注册，cmd:{attr.OriginalCmd}");
            }

            // 注册标准化的命名
            if (!_httpHandlerMap.TryAdd(attr.StandardCmd, handler))
            {
                throw new Exception($"http handler cmd重复注册，cmd:{attr.OriginalCmd}");
            }

            return true;
        }

        private bool AddRpcHandler(Type type)
        {
            var attribute = (MessageRpcMappingAttribute)type.GetCustomAttribute(typeof(MessageRpcMappingAttribute), true);
            if (attribute.IsNull())
            {
                return false;
            }

            bool isHas = _rpcHandlerMap.TryGetValue(attribute.RequestMessage.GetType(), out var requestHandler);
            if (isHas && requestHandler?.GetType() == attribute.ResponseMessage.GetType())
            {
                LogHelper.Error($"重复注册消息rpc handler:[{attribute.RequestMessage}] msg:[{attribute.ResponseMessage}]");
                return false;
            }

            _rpcHandlerMap.Add(attribute.RequestMessage.GetType(), attribute.ResponseMessage.GetType());

            return true;
        }

        private bool AddTcpHandler(Type type)
        {
            var attribute = (MessageMappingAttribute)type.GetCustomAttribute(typeof(MessageMappingAttribute), true);
            if (attribute.IsNull())
            {
                return false;
            }

            var msgIdField = (MessageTypeHandlerAttribute)attribute.MessageType.GetCustomAttribute(typeof(MessageTypeHandlerAttribute), true);
            if (msgIdField.IsNull())
            {
                return false;
            }

            int msgId = msgIdField.MessageId;
            if (!_tcpHandlerMap.TryAdd(msgId, type))
            {
                LogHelper.Error("重复注册消息tcp handler:[{}] msg:[{}]", msgId, type);
            }

            return true;
        }

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

            bool find = false;
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
                throw new Exception($"组件agent必须以{GlobalConst.ComponentAgentNameSuffix}结尾，{fullName}");
            }

            var compType = type.BaseType.GetGenericArguments()[0];
            if (!_compAgentMap.TryAdd(compType, type))
            {
                throw new Exception($"comp:{compType.FullName}有多个agent");
            }

            _agentCompMap[type] = compType;
            return true;
        }

        internal BaseMessageHandler GetTcpHandler(int msgId)
        {
            if (_tcpHandlerMap.TryGetValue(msgId, out var handlerType))
            {
                var instance = Activator.CreateInstance(handlerType);
                if (instance is BaseMessageHandler handler)
                {
                    return handler;
                }

                throw new Exception($"错误的tcp handler类型，{instance.GetType().FullName}");
            }

            return null;
            //throw new HandlerNotFoundException($"消息id：{msgId}");
        }

        internal BaseHttpHandler GetHttpHandler(string cmd)
        {
            if (_httpHandlerMap.TryGetValue(cmd, out var handler))
            {
                return handler;
            }

            return null;
            // throw new HttpHandlerNotFoundException($"未注册的http命令:{cmd}");
        }

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

                agent.Owner = component;
                return agent;
            }

            throw new KeyNotFoundException(nameof(_compAgentMap) + " ===>" + nameof(type));
        }

        internal List<IEventListener> FindListeners(ActorType actorType, int evtId)
        {
            if (_actorEvtListeners.TryGetValue(actorType, out var evtListeners)
                && evtListeners.TryGetValue(evtId, out var listeners))
            {
                return listeners;
            }

            return null;
        }


        /// <summary>
        /// 获取实例(主要用于获取Event,Timer, Schedule,的Handler实例)
        /// </summary>
        /// <param name="typeName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal T GetInstance<T>(string typeName)
        {
            return (T)_typeCacheMap.GetOrAdd(typeName, k => HotfixAssembly.CreateInstance(k));
        }

        internal Type GetAgentType(Type compType)
        {
            _compAgentMap.TryGetValue(compType, out var agentType);
            return agentType;
        }

        internal Type GetCompType(Type agentType)
        {
            _agentCompMap.TryGetValue(agentType, out var compType);
            return compType;
        }
    }
}