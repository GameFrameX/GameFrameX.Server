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

using System.Collections.Concurrent;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Logger;
using GameFrameX.Utility;
using GameFrameX.Utility.Setting;

namespace GameFrameX.DiscoveryCenterManager.Server;

/// <summary>
/// 服务器管理
/// </summary>
public sealed class NamingServiceManager : Singleton<NamingServiceManager>
{
    /// <summary>
    /// 服务器添加的时候触发的回调
    /// </summary>
    private Action<IServiceInfo> _onServerAdd;

    /// <summary>
    /// 服务器移除的时候触发的回调
    /// </summary>
    private Action<IServiceInfo> _onServerRemove;

    /// <summary>
    /// 服务器节点的id 为自身的serverId
    /// </summary>
    private readonly ConcurrentDictionary<long, List<IServiceInfo>> _serverMap = new();

    /// <summary>
    /// 设置服务器变更的回调
    /// </summary>
    /// <param name="onServerAdd"></param>
    /// <param name="onServerRemove"></param>
    public void SetServerChangeCallback(Action<IServiceInfo> onServerAdd, Action<IServiceInfo> onServerRemove)
    {
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

        var result = _serverMap.TryRemove(serverId, out var value);
        if (value != null)
        {
            foreach (var serviceInfo in value)
            {
                _onServerRemove?.Invoke(serviceInfo);
            }
        }

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
            foreach (var serviceInfo in keyValuePair.Value)
            {
                if (serviceInfo.SessionId == sessionId)
                {
                    serverId = keyValuePair.Key;
                    break;
                }
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
            foreach (var serviceInfo in keyValuePair.Value)
            {
                if (serviceInfo.SessionId == sessionId)
                {
                    return serviceInfo;
                }
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
    public List<IServiceInfo> TryGet(long id)
    {
        _serverMap.TryGetValue(id, out var value);
        return value;
    }

    /// <summary>
    /// 获取节点列表
    /// </summary>
    /// <returns></returns>
    public List<List<IServiceInfo>> GetAllNodes()
    {
        return _serverMap.Values.ToList();
    }

    /// <summary>
    /// 获取外部节点
    /// </summary>
    /// <returns></returns>
    public List<IServiceInfo> GetOuterNodes()
    {
        List<IServiceInfo> result = new();
        foreach (var keyValuePair in _serverMap)
        {
            foreach (var serviceInfo in keyValuePair.Value)
            {
                if (serviceInfo.SessionId.IsNotNullOrEmpty())
                {
                    result.Add(serviceInfo);
                }
            }
        }

        return result;
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

        SelfServiceInfo = new ServiceInfo(setting.ServerType, null, string.Empty, setting.ServerName, setting.ServerId, setting.ServerInstanceId, setting.InnerIp, setting.InnerPort, setting.OuterIp, setting.OuterPort);
        _serverMap[SelfServiceInfo.ServerId] = [SelfServiceInfo,];
    }

    /// <summary>
    /// 添加节点
    /// </summary>
    /// <param name="node">节点信息</param>
    public void Add(IServiceInfo node)
    {
        ArgumentNullException.ThrowIfNull(node, nameof(node));
        if (node.Type == SelfServiceInfo.Type)
        {
            LogHelper.Error($"不能添加{SelfServiceInfo.Type.ToString()}节点...{node}");
            return;
        }

        if (!_serverMap.TryGetValue(node.ServerId, out var list))
        {
            list = new List<IServiceInfo>();
            _serverMap[node.ServerId] = list;
        }

        if (list.Contains(node))
        {
            LogHelper.Warning("重复添加节点...忽略处理" + node);
            return;
        }

        list.Add(node);
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
            foreach (var serviceInfo in node.Value)
            {
                if (serviceInfo.Type == type)
                {
                    list.Add(serviceInfo);
                }
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