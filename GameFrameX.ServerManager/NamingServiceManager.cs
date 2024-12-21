using System.Collections.Concurrent;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.Monitor.Discovery;
using GameFrameX.Setting;

namespace GameFrameX.ServerManager;

/// <summary>
/// 服务器管理
/// </summary>
public sealed class NamingServiceManager
{
    /// <summary>
    /// 服务器添加的时候触发的回调
    /// </summary>
    private readonly Action<IServiceInfo> _onServerAdd;

    /// <summary>
    /// 服务器移除的时候触发的回调
    /// </summary>
    private readonly Action<IServiceInfo> _onServerRemove;

    /// <summary>
    /// 服务器节点的id 为自身的serverId
    /// </summary>
    private readonly ConcurrentDictionary<long, IServiceInfo> _serverMap;

    /// <summary>
    /// </summary>
    public NamingServiceManager(Action<IServiceInfo> onServerAdd, Action<IServiceInfo> onServerRemove)
    {
        _serverMap = new ConcurrentDictionary<long, IServiceInfo>();
        _onServerAdd = onServerAdd;
        _onServerRemove = onServerRemove;
    }

    /// <summary>
    /// 自身服务器信息
    /// </summary>
    public IServiceInfo SelfServiceInfo { get; private set; }

    /// <summary>
    /// 根据节点数据从服务器列表中删除
    /// </summary>
    /// <param name="serverId"></param>
    /// <returns></returns>
    public bool TryRemove(long serverId)
    {
        if (serverId <= 0)
        {
            return false;
        }

        MetricsDiscoveryRegister.ServiceCounterOptions.Dec(-1);
        var result = _serverMap.TryRemove(serverId, out var value);
        _onServerRemove?.Invoke(value);
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
    /// <param name="sessionId"></param>
    /// <returns></returns>
    public IServiceInfo GetNodeBySessionId(string sessionId)
    {
        foreach (var keyValuePair in _serverMap)
        {
            if (keyValuePair.Value.SessionId == sessionId)
            {
                return keyValuePair.Value;
            }
        }

        return null;
    }

    /// <summary>
    /// 根据节点数据从服务器列表中删除
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public bool TryRemove(IServiceInfo info)
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
    public IServiceInfo TryGet(long id)
    {
        _serverMap.TryGetValue(id, out var value);
        return value;
    }

    /// <summary>
    /// 获取节点列表
    /// </summary>
    /// <returns></returns>
    public List<IServiceInfo> GetAllNodes()
    {
        return _serverMap.Values.ToList();
    }

    /// <summary>
    /// 获取外部节点
    /// </summary>
    /// <returns></returns>
    public List<IServiceInfo> GetOuterNodes()
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

    /// <summary>
    /// 添加自身
    /// </summary>
    public void AddSelf(AppSetting setting)
    {
        if (SelfServiceInfo != null)
        {
            // 已经添加过了
            return;
        }

        SelfServiceInfo = new ServiceInfo(setting.ServerType, null, string.Empty, setting.ServerName, setting.ServerId, setting.InnerIp, setting.InnerPort, setting.OuterIp, setting.OuterPort);
        _serverMap[SelfServiceInfo.ServerId] = SelfServiceInfo;
    }

    /// <summary>
    /// 添加节点
    /// </summary>
    /// <param name="node">节点信息</param>
    public void Add(IServiceInfo node)
    {
        node.CheckNotNull(nameof(node));
        if (node.Type == SelfServiceInfo.Type)
        {
            LogHelper.Error($"不能添加{SelfServiceInfo.Type.ToString()}节点...{node}");
            return;
        }

        if (_serverMap.ContainsKey(node.ServerId))
        {
            LogHelper.Error($"重复添加节点:[{node}]");
            return;
        }

        MetricsDiscoveryRegister.ServiceCounterOptions.Inc();
        _serverMap.TryAdd(node.ServerId, node);
        _onServerAdd?.Invoke(node);
        LogHelper.Info($"新的网络节点总数：{GetNodeCount()} 新的节点信息:\n {node}");
    }

    /// <summary>
    /// 根据服务器类型获取节点列表
    /// </summary>
    /// <param name="type">服务器类型</param>
    /// <returns></returns>
    public List<IServiceInfo> GetNodesByType(ServerType type)
    {
        var list = new List<IServiceInfo>();
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