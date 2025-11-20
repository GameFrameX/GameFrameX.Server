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
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.Utility;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Localization;
using Serilog;

namespace GameFrameX.DiscoveryCenterManager.Service;

/// <summary>
/// Service connection manager for Discovery Center communications
/// </summary>
/// <remarks>
/// 服务连接管理器，负责注册、注销以及通过连接发送消息与进行RPC调用。
/// 管理内部的连接字典，并在使用前确保连接已初始化。
/// 该类为单例，继承自 <see>
///     <cref>GameFrameX.Utility.Singleton{GameFrameX.DiscoveryCenterManager.Service.ServiceConnectManager}</cref>
/// </see>
/// 。
/// </remarks>
/// <seealso cref="GameFrameX.DiscoveryCenterManager.Service.ServiceConnect"/>
/// <seealso cref="GameFrameX.NetWork.Messages.MessageObject"/>
/// <seealso cref="GameFrameX.NetWork.Abstractions.IRpcResult"/>
public sealed class ServiceConnectManager : Singleton<ServiceConnectManager>
{
    /// <summary>
    /// Internal map of serverInstanceId to service connections
    /// </summary>
    /// <remarks>
    /// 内部维护服务器实例ID到服务连接的映射，用于快速检索并管理连接的生命周期。
    /// 使用线程安全的 <see>
    ///     <cref>System.Collections.Concurrent.ConcurrentDictionary{System.Int64, GameFrameX.DiscoveryCenterManager.Service.ServiceConnect}</cref>
    /// </see>
    /// 。
    /// </remarks>
    readonly ConcurrentDictionary<long, ServiceConnect> _serviceConnects = new();


    /// <summary>
    /// Registers a service connection into the manager
    /// </summary>
    /// <param name="name">The service name / 服务名称</param>
    /// <param name="serverId">The server id / 服务器ID</param>
    /// <param name="serverInstanceId">The server instance id / 服务器实例ID</param>
    /// <param name="host">The target host / 目标主机</param>
    /// <param name="port">The target port / 目标端口</param>
    /// <remarks>
    /// 当指定的 `serverInstanceId` 已注册时将记录警告并跳过。
    /// 若未注册则创建新的 <see cref="GameFrameX.DiscoveryCenterManager.Service.ServiceConnect"/> 并加入管理字典。
    /// </remarks>
    public void Register(string name, long serverId, long serverInstanceId, string host, int port)
    {
        if (_serviceConnects.TryGetValue(serverInstanceId, out var serviceConnect))
        {
            Log.Warning(LocalizationService.GetString(Keys.DiscoveryCenterManager.ServerInstanceAlreadyRegistered, serverInstanceId));
            return;
        }

        serviceConnect = new ServiceConnect(name, serverId, serverInstanceId, host, port);
        _serviceConnects.TryAdd(serverInstanceId, serviceConnect);
    }

    /// <summary>
    /// Unregisters a service connection and disposes its resources
    /// </summary>
    /// <param name="serverInstanceId">The server instance id to unregister / 要注销的服务器实例ID</param>
    /// <remarks>
    /// 若找到对应连接则进行移除并释放资源；否则记录错误日志提示未找到。
    /// </remarks>
    public void UnRegister(long serverInstanceId)
    {
        if (_serviceConnects.Remove(serverInstanceId, out var serviceConnect))
        {
            Log.Information(LocalizationService.GetString(Keys.DiscoveryCenterManager.ServerInstanceUnregistered, serverInstanceId));
            serviceConnect?.Dispose();
        }
        else
        {
            Log.Error(LocalizationService.GetString(Keys.DiscoveryCenterManager.ServerInstanceNotFound, serverInstanceId));
        }
    }

    /// <summary>
    /// Sends a message to the specified server instance via its connection
    /// </summary>
    /// <param name="serverInstanceId">The target server instance id / 目标服务器实例ID</param>
    /// <param name="message">The message to send / 要发送的消息</param>
    /// <returns>A task representing the asynchronous send operation / 表示异步发送操作的任务</returns>
    /// <remarks>
    /// 若连接未初始化则先调用初始化流程；当找不到连接时记录错误日志。
    /// </remarks>
    /// <seealso cref="GameFrameX.DiscoveryCenterManager.Service.ServiceConnect.InitAsync"/>
    public async Task Send(long serverInstanceId, MessageObject message)
    {
        if (_serviceConnects.TryGetValue(serverInstanceId, out var connect))
        {
            if (!connect.IsInit)
            {
                await connect.InitAsync();
            }

            connect.ServiceClient?.Send(message);
        }
        else
        {
            Log.Error(LocalizationService.GetString(Keys.DiscoveryCenterManager.SendServerInstanceNotFound, serverInstanceId));
        }
    }


    /// <summary>
    /// Performs an RPC call to the specified server instance
    /// </summary>
    /// <typeparam name="T">Response message type, must implement <see cref="GameFrameX.NetWork.Abstractions.IResponseMessage"/> and have a parameterless constructor / 响应消息类型，需实现IResponseMessage且具有无参构造</typeparam>
    /// <param name="serverInstanceId">The target server instance id / 目标服务器实例ID</param>
    /// <param name="request">The request message to call / 要调用的请求消息</param>
    /// <param name="timeout">The timeout in milliseconds / 超时时间（毫秒）</param>
    /// <returns>An RPC result representing the call outcome / 表示调用结果的RPC返回值</returns>
    /// <remarks>
    /// 若连接未初始化则先进行初始化；当找不到连接时返回默认值并记录错误。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 示例：向指定实例发起RPC调用
    /// var result = await ServiceConnectManager.Instance.Call<RespPing/>(instanceId, new ReqPing(), 5000);
    /// </code>
    /// </example>
    /// <seealso cref="GameFrameX.DiscoveryCenterManager.Service.ServiceConnect.ServiceClient"/>
    /// <seealso cref="GameFrameX.NetWork.Abstractions.IRpcResult"/>
    public async Task<IRpcResult> Call<T>(long serverInstanceId, MessageObject request, int timeout = 30000) where T : IResponseMessage, new()
    {
        if (_serviceConnects.TryGetValue(serverInstanceId, out var serviceConnect))
        {
            if (!serviceConnect.IsInit)
            {
                await serviceConnect.InitAsync();
            }

            return await serviceConnect.ServiceClient.Call<T>(request, timeout);
        }

        Log.Error(LocalizationService.GetString(Keys.DiscoveryCenterManager.CallServerInstanceNotFound, serverInstanceId));
        return default;
    }
}
