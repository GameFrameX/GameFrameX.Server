using System.Collections.Concurrent;
using System.Reflection;
using GameFrameX.Core.Actors;
using GameFrameX.Core.Comps;
using GameFrameX.Core.Events;
using GameFrameX.Core.Hotfix.Agent;
using GameFrameX.Core.Net.BaseHandler;
using GameFrameX.NetWork.HTTP;
using GameFrameX.Setting;

namespace GameFrameX.Core.Hotfix
{
    public static class HotfixManager
    {
        internal static volatile bool DoingHotfix = false;

        private static volatile HotfixModule _module = null;

        private static BaseSetting _baseSetting;
        public static Assembly HotfixAssembly => _module?.HotfixAssembly;

        private static readonly ConcurrentDictionary<int, HotfixModule> oldModuleMap = new();

        public static DateTime ReloadTime { get; private set; }

        public static async Task<bool> LoadHotfixModule(BaseSetting setting, string dllVersion = "")
        {
            if (setting != null)
            {
                _baseSetting = setting;
            }

            var dllPath = Path.Combine(Environment.CurrentDirectory, string.IsNullOrEmpty(dllVersion) ? "hotfix/GameFrameX.Hotfix.dll" : $"{dllVersion}/GameFrameX.Hotfix.dll");
            var hotfixModule = new HotfixModule(dllPath);
            bool reload = _module != null;
            // 起服时失败会有异常抛出
            var success = hotfixModule.Init(reload);
            if (!success)
            {
                return false;
            }

            return await Load(hotfixModule, _baseSetting, reload);
        }

        private static async Task<bool> Load(HotfixModule newModule, BaseSetting setting, bool reload)
        {
            ReloadTime = DateTime.Now;
            if (reload)
            {
                var oldModule = _module;
                DoingHotfix = true;
                int oldModuleHash = oldModule.GetHashCode();
                oldModuleMap.TryAdd(oldModuleHash, oldModule);
                _ = Task.Run(async () =>
                {
                    await Task.Delay(1000 * 60 * 3);
                    oldModuleMap.TryRemove(oldModuleHash, out _);
                    oldModule.Unload();
                    DoingHotfix = !oldModuleMap.IsEmpty;
                });
            }

            _module = newModule;
            if (_module.HotfixBridge != null)
                return await _module.HotfixBridge.OnLoadSuccess(setting, reload);
            return true;
        }

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
            return _module.GetCompType(agentType);
        }

        public static T GetAgent<T>(BaseComponent component, Type refAssemblyType) where T : IComponentAgent
        {
            if (!oldModuleMap.IsEmpty)
            {
                var asb = typeof(T).Assembly;
                var asb2 = refAssemblyType?.Assembly;
                foreach (var kv in oldModuleMap)
                {
                    var old = kv.Value;
                    if (asb == old.HotfixAssembly || asb2 == old.HotfixAssembly)
                        return old.GetAgent<T>(component);
                }
            }

            return _module.GetAgent<T>(component);
        }

        public static BaseMessageHandler GetTcpHandler(int msgId)
        {
            return _module.GetTcpHandler(msgId);
        }

        public static BaseHttpHandler GetHttpHandler(string cmd)
        {
            return _module.GetHttpHandler(cmd);
        }

        public static List<IEventListener> FindListeners(ActorType actorType, int evtId)
        {
            return _module.FindListeners(actorType, evtId) ?? EMPTY_LISTENER_LIST;
        }

        private static readonly List<IEventListener> EMPTY_LISTENER_LIST = new();

        /// <summary>
        /// 获取实例
        /// 主要用于获取Event,Timer, Schedule,的Handler实例
        /// </summary>
        public static T GetInstance<T>(string typeName, Type refAssemblyType = null)
        {
            if (string.IsNullOrEmpty(typeName))
                return default;
            if (oldModuleMap.Count > 0)
            {
                var asb = refAssemblyType?.Assembly;
                foreach (var kv in oldModuleMap)
                {
                    var old = kv.Value;
                    if (asb == old.HotfixAssembly)
                        return old.GetInstance<T>(typeName);
                }
            }

            return _module.GetInstance<T>(typeName);
        }
    }
}