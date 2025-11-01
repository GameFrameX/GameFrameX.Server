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

using System.Net;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.StartUp.ServiceClient;
using ErrorEventArgs = GameFrameX.SuperSocket.ClientEngine.ErrorEventArgs;

namespace GameFrameX.StartUp;

/// <summary>
/// Application startup base class - provides TCP and WebSocket server basic functionality implementation / 程序启动器基类 - 提供TCP和WebSocket服务器的基础功能实现
/// </summary>
/// <remarks>
/// 此部分类专门处理与发现中心（DiscoveryCenter）的通信功能
/// </remarks>
public abstract partial class AppStartUpBase
{
    /// <summary>
    /// Game application service client / 游戏应用服务客户端
    /// </summary>
    /// <value>
    /// Client for communicating with the discovery center / 与发现中心通信的客户端
    /// </value>
    /// <remarks>
    /// 用于与发现中心建立连接并进行消息交换
    /// </remarks>
    private GameAppServiceClient _gameAppServiceClient;

    /// <summary>
    /// Start the client for communicating with the discovery center (DiscoveryCenter) / 启动与发现中心（DiscoveryCenter）通信的客户端
    /// </summary>
    /// <remarks>
    /// 用于注册当前服务器实例并接收发现中心推送的消息
    /// </remarks>
    private void StartGameAppClient()
    {
        // 创建客户端事件回调对象
        var gameAppServiceConfiguration = new GameAppServiceConfiguration
        {
            OnConnected = GameAppClientOnConnected,
            OnClosed = GameAppClientOnClosed,
            OnMessage = GameAppClientOnMessage,
            OnError = GameAppClientOnError,
            OnHeartBeat = GameAppClientOnHeartBeat,
        };

        if (Setting.DiscoveryCenterHost.IsNullOrEmptyOrWhiteSpace())
        {
            LogHelper.Error("DiscoveryCenterHost is not configured; unable to start connection to DiscoveryCenter. Please check the configuration item DiscoveryCenterHost");
            return;
        }

        if (!Setting.DiscoveryCenterPort.IsRange())
        {
            LogHelper.Error("DiscoveryCenterPort is not configured; unable to start connection to DiscoveryCenter. Please check the configuration item DiscoveryCenterPort");
            return;
        }

        var endPoint = new DnsEndPoint(Setting.DiscoveryCenterHost, Setting.DiscoveryCenterPort);

        // 根据配置创建发现中心终结点并初始化客户端
        _gameAppServiceClient = new GameAppServiceClient(endPoint, gameAppServiceConfiguration);
    }

    /// <summary>
    /// Heartbeat callback: triggered when a heartbeat message needs to be sent to the discovery center / 心跳回调：当需要向发现中心发送心跳消息时触发
    /// </summary>
    /// <returns>Returns the heartbeat message object to be sent; returns null if no heartbeat is needed / 返回要发送的心跳消息对象；如无需发送心跳，返回 null</returns>
    /// <remarks>
    /// 可在此构造并返回心跳数据，用于维持与发现中心的连接
    /// </remarks>
    protected virtual MessageObject GameAppClientOnHeartBeat()
    {
        return default;
    }

    /// <summary>
    /// Call RPC method of the discovery center (DiscoveryCenter) / 调用发现中心（DiscoveryCenter）的RPC方法
    /// </summary>
    /// <typeparam name="T">The response message type to return / 要返回的响应消息类型</typeparam>
    /// <param name="messageObject">RPC method parameters to call / 要调用的RPC方法参数</param>
    /// <param name="timeOut">Call timeout (milliseconds) / 调用超时时间（毫秒）</param>
    /// <returns>A task representing the asynchronous operation, with the task result being an IRpcResult object / 表示异步操作的任务，任务结果为IRpcResult对象</returns>
    /// <remarks>
    /// 用于向发现中心发起RPC调用并等待响应
    /// </remarks>
    public Task<IRpcResult> Call<T>(MessageObject messageObject, int timeOut = 10000) where T : IResponseMessage, new()
    {
        return _gameAppServiceClient.Call<T>(messageObject, timeOut);
    }

    /// <summary>
    /// Send message to the discovery center (DiscoveryCenter) / 向发现中心（DiscoveryCenter）发送消息
    /// </summary>
    /// <param name="message">Message object to be sent / 待发送的消息对象</param>
    /// <remarks>
    /// If the client is already initialized, then call its method to send the message to the discovery center server
    /// </remarks>
    public void Send(MessageObject message)
    {
        // If the client is already initialized, then call its method to send the message to the discovery center server
        _gameAppServiceClient?.Send(message);
    }

    /// <summary>
    /// Callback when communication error occurs with the discovery center / 与发现中心通信发生错误时的回调
    /// </summary>
    /// <param name="obj">Error event arguments containing exception information / 包含异常信息的错误事件参数</param>
    /// <remarks>
    /// 处理与发现中心通信过程中发生的错误
    /// </remarks>
    protected virtual void GameAppClientOnError(ErrorEventArgs obj)
    {
        LogHelper.Error($"服务器{Setting.ServerType}与发现中心通信发生错误，e:{obj.Exception}");
    }

    /// <summary>
    /// Callback when receiving messages pushed by the discovery center / 收到发现中心推送消息时的回调
    /// </summary>
    /// <param name="message">Message object sent by the discovery center / 发现中心下发的消息对象</param>
    /// <remarks>
    /// 处理从发现中心接收到的各种消息
    /// </remarks>
    protected virtual void GameAppClientOnMessage(MessageObject message)
    {
        LogHelper.Debug($"服务器{Setting.ServerType}接收到发现中心消息,{message.ToFormatMessageString()}");
    }

    /// <summary>
    /// Callback when connection to the discovery center is disconnected / 与发现中心连接断开时的回调
    /// </summary>
    /// <remarks>
    /// 处理与发现中心连接断开的情况
    /// </remarks>
    protected virtual void GameAppClientOnClosed()
    {
        LogHelper.Debug($"服务器{Setting.ServerType}与发现中心断开连接...");
    }

    /// <summary>
    /// Callback when connection to the discovery center is successfully established / 与发现中心连接建立成功时的回调
    /// </summary>
    /// <remarks>
    /// 处理与发现中心成功建立连接的情况
    /// </remarks>
    protected virtual void GameAppClientOnConnected()
    {
        LogHelper.Debug($"服务器{Setting.ServerType}连接到发现中心成功...");
    }
}