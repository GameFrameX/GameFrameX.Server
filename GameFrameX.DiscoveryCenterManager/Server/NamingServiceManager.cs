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
using GameFrameX.Foundation.Json;
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
    private Action<IServiceInfo> _onAdd;

    /// <summary>
    /// 服务器移除的时候触发的回调
    /// </summary>
    private Action<IServiceInfo> _onRemove;

    /// <summary>
    /// 服务器节点的id 为自身的serverId
    /// </summary>
    private readonly ConcurrentDictionary<long, List<IServiceInfo>> _serverMap = new();

    /// <summary>
    /// 设置服务器变更的回调
    /// </summary>
    /// <param name="onAdd"></param>
    /// <param name="onRemove"></param>
    public void SetServerChangeCallback(Action<IServiceInfo> onAdd, Action<IServiceInfo> onRemove)
    {
        _onAdd = onAdd;
        _onRemove = onRemove;
    }

    /// <summary>
    /// 自身服务器信息
    /// </summary>
    public IServiceInfo SelfServiceInfo { get; private set; }

    /// <summary>
    /// 根据服务器实例ID从服务器列表中删除对应的服务节点
    /// </summary>
    /// <param name="serverInstanceId">服务器实例ID</param>
    /// <returns>删除成功返回true，否则返回false</returns>
    public bool TryRemoveByInstanceId(long serverInstanceId)
    {
        if (serverInstanceId <= 0)
        {
            return false;
        }

        bool removed = false;
        var keysToRemove = new List<long>();

        foreach (var keyValuePair in _serverMap)
        {
            var serviceToRemove = keyValuePair.Value.FirstOrDefault(m => m.ServerInstanceId == serverInstanceId);
            if (serviceToRemove != null)
            {
                // 从列表中移除找到的服务节点
                keyValuePair.Value.Remove(serviceToRemove);

                // 触发移除回调
                _onRemove?.Invoke(serviceToRemove);
                removed = true;

                // 如果列表为空，标记该键需要被删除
                if (keyValuePair.Value.Count == 0)
                {
                    keysToRemove.Add(keyValuePair.Key);
                }

                // 找到并删除后跳出循环，因为ServerInstanceId应该是唯一的
                break;
            }
        }

        // 清空空的字典键
        foreach (var key in keysToRemove)
        {
            _serverMap.TryRemove(key, out _);
        }

        return removed;
    }

    /// <summary>
    /// 根据会话ID移除对应的服务节点
    /// </summary>
    /// <param name="sessionId">会话ID</param>
    /// <returns>成功移除返回true，否则返回false</returns>
    public bool TryRemoveBySessionId(string sessionId)
    {
        IServiceInfo serverInfo = default;
        foreach (var keyValuePair in _serverMap)
        {
            foreach (var serviceInfo in keyValuePair.Value)
            {
                if (serviceInfo.SessionId == sessionId)
                {
                    serverInfo = serviceInfo;
                    break;
                }
            }

            if (serverInfo != null)
            {
                break;
            }
        }

        if (serverInfo != null)
        {
            return TryRemoveByInstanceId(serverInfo.ServerInstanceId);
        }

        return false;
    }

    /// <summary>
    /// 根据会话ID获取对应的服务节点
    /// </summary>
    /// <param name="sessionId">会话ID</param>
    /// <returns>如果找到对应的服务节点，则返回该节点；否则返回null</returns>
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

        return default;
    }

    /// <summary>
    /// 根据节点数据从服务器列表中删除
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public bool TryRemoveByServiceInfo(IServiceInfo info)
    {
        if (info == null)
        {
            return true;
        }

        return TryRemoveByInstanceId(info.ServerInstanceId);
    }

    /// <summary>
    /// 根据服务器ID获取对应的服务节点列表
    /// </summary>
    /// <param name="serverId">服务器ID</param>
    /// <returns>如果找到对应的服务节点列表，则返回该列表；否则返回null</returns>
    public List<IServiceInfo> GetNodesByServerId(long serverId)
    {
        _serverMap.TryGetValue(serverId, out var value);
        return value;
    }

    /// <summary>
    /// 获取所有服务节点列表
    /// </summary>
    /// <returns>所有服务节点的列表，每个键值对表示一个服务器类型及其对应的服务节点列表</returns>
    public List<List<IServiceInfo>> GetAllNodes()
    {
        return _serverMap.Values.ToList();
    }

    /// <summary>
    /// 获取所有外部服务节点列表
    /// </summary>
    /// <returns>所有外部服务节点的列表，每个节点包含会话ID非空的服务节点</returns>
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
    /// 获取所有服务节点的总数量
    /// </summary>
    /// <returns>所有服务节点的总数量</returns>
    public int GetNodeCount()
    {
        return _serverMap.Values.Sum(list => list.Count);
    }

    /// <summary>
    /// 添加自身服务节点
    /// </summary>
    /// <param name="setting">自身服务节点的配置信息</param>
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
    /// 添加服务节点
    /// </summary>
    /// <param name="node">服务节点信息</param>
    public void Add(IServiceInfo node)
    {
        if (node == null)
        {
            return;
        }

        if (node.Type == SelfServiceInfo.Type)
        {
            LogHelper.Error($"Cannot add {SelfServiceInfo.Type.ToString()} node...{node}");
            return;
        }

        if (!_serverMap.TryGetValue(node.ServerId, out var list))
        {
            list = new List<IServiceInfo>();
            _serverMap[node.ServerId] = list;
        }

        if (list.Contains(node))
        {
            LogHelper.Warning("Duplicate node addition...Ignored " + node);
            return;
        }

        list.Add(node);
        _onAdd?.Invoke(node);
        LogHelper.Info($"Current total network node count: {GetNodeCount()} New node info: {JsonHelper.Serialize(node)}");
    }

    /// <summary>
    /// 根据服务器类型获取所有服务节点列表
    /// </summary>
    /// <param name="type">服务器类型</param>
    /// <returns>所有指定类型的服务节点列表</returns>
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
}