using System.Collections.Concurrent;
using GameFrameX.Log;
using GameFrameX.Monitor.Discovery;
using GameFrameX.Setting;
using GameFrameX.Utility;

namespace GameFrameX.ServerManager
{
    /// <summary>
    /// 服务器管理
    /// </summary>
    public sealed class NamingServiceManager : Singleton<NamingServiceManager>
    {
        /// <summary>
        /// 服务器节点的id 为自身的serverId
        /// </summary>
        private readonly ConcurrentDictionary<long, ServerInfo> _serverMap = new ConcurrentDictionary<long, ServerInfo>();

        public Action<ServerInfo> OnServerAdd;
        public Action<ServerInfo> OnServerRemove;

        /// <summary>
        /// 根据节点数据从服务器列表中删除
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public bool TryRemove(long serverId)
        {
            MetricsDiscoveryRegister.ServiceCounterOptions.Dec(-1);
            var result = _serverMap.TryRemove(serverId, out var value);
            OnServerRemove?.Invoke(value);
            return result;
        }

        /// <summary>
        /// 根据节点数据从服务器列表中删除
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public bool TrySessionRemove(string sessionId)
        {
            long serverId = 0;
            foreach (var keyValuePair in _serverMap)
            {
                if (keyValuePair.Value.SessionId == sessionId)
                {
                    serverId = keyValuePair.Key;
                    break;
                }
            }

            return TryRemove(serverId);
        }

        /// <summary>
        /// 根据节点数据从服务器列表中删除
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool TryRemove(ServerInfo info)
        {
            if (info == null)
            {
                return true;
            }

            return TryRemove(info.ServerId);
        }

        /// <summary>
        /// 根据id获取节点数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServerInfo TryGet(long id)
        {
            _serverMap.TryGetValue(id, out var v);
            return v;
        }

        /// <summary>
        /// 获取节点列表
        /// </summary>
        /// <returns></returns>
        public List<ServerInfo> GetAllNodes()
        {
            return _serverMap.Values.ToList();
        }

        /// <summary>
        /// 获取外部节点
        /// </summary>
        /// <returns></returns>
        public List<ServerInfo> GetOuterNodes()
        {
            return _serverMap.Values.Where(m => !string.IsNullOrEmpty(m.SessionId)).ToList();
        }

        /// <summary>
        /// 获取节点数量
        /// </summary>
        /// <returns></returns>
        public int GetNodeCount()
        {
            return _serverMap.Count;
        }

        private ServerInfo serverInfo;

        /// <summary>
        /// 添加自身
        /// </summary>
        public void AddSelf(BaseSetting setting)
        {
            serverInfo = new ServerInfo(setting.ServerType, null, string.Empty, setting.ServerName, setting.ServerId, setting.InnerIp, setting.InnerPort, setting.OuterIp, setting.OuterPort);
            _serverMap[serverInfo.ServerId] = serverInfo;
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node">节点信息</param>
        public void Add(ServerInfo node)
        {
            Guard.NotNull(node, nameof(node));
            if (node.Type == serverInfo.Type)
            {
                LogHelper.Error($"不能添加discovery节点...{node}");
                return;
            }

            if (_serverMap.ContainsKey(node.ServerId))
            {
                LogHelper.Error($"重复添加节点:[{node}]");
                return;
            }

            MetricsDiscoveryRegister.ServiceCounterOptions.Inc();
            _serverMap.TryAdd(node.ServerId, node);
            OnServerAdd?.Invoke(node);
            LogHelper.Info($"新的网络节点:[{node}]   总数：{GetNodeCount()}");
        }

        /// <summary>
        /// 根据服务器类型获取节点列表
        /// </summary>
        /// <param name="type">服务器类型</param>
        /// <returns></returns>
        public List<ServerInfo> GetNodesByType(ServerType type)
        {
            var list = new List<ServerInfo>();
            foreach (var node in _serverMap)
            {
                if (node.Value.Type == type)
                {
                    list.Add(node.Value);
                }
            }

            return list;
        }

        /*
        /// <summary>
        /// 设置节点的状态信息
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <param name="statusInfo">节点状态信息</param>
        public void SetNodeState(long nodeId, ServerStatusInfo statusInfo)
        {
            //Log.Debug($"设置节点{nodeId}状态");
            if (_serverMap.TryGetValue(nodeId, out var node))
            {
                node.StatusInfo = statusInfo;
            }
        }

        /// <summary>
        /// 设置节点的状态
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <param name="state">节点状态</param>
        public void SetNodeState(long nodeId, ServerStatus state)
        {
            //Log.Debug($"设置节点{nodeId}状态");
            if (_serverMap.TryGetValue(nodeId, out var node))
            {
                node.StatusInfo.Status = state;
            }
        }*/
    }
}