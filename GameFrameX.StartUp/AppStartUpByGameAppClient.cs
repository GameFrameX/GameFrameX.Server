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

using GameFrameX.DiscoveryCenterManager.ServiceClient;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.Utility;
using ErrorEventArgs = GameFrameX.SuperSocket.ClientEngine.ErrorEventArgs;

namespace GameFrameX.StartUp;

/// <summary>
/// 程序启动器基类 - 提供服务器的基础功能实现。
/// </summary>
/// <remarks>
/// Application startup base class - provides server basic functionality implementation.
/// This partial class specifically handles communication functionality with the discovery center (DiscoveryCenter).
/// </remarks>
public abstract partial class AppStartUpBase
{
    /// <summary>
    /// 游戏应用服务客户端。
    /// </summary>
    /// <remarks>
    /// Game application service client.
    /// Used to establish connection with the discovery center and exchange messages.
    /// </remarks>
    /// <value>与发现中心通信的客户端 / Client for communicating with the discovery center</value>
    private GameAppServiceClient _gameAppServiceClient;

    /// <summary>
    /// 启动与发现中心（DiscoveryCenter）通信的客户端。
    /// </summary>
    /// <remarks>
    /// Start the client for communicating with the discovery center (DiscoveryCenter).
    /// Used to register the current server instance and receive messages pushed from the discovery center.
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
            ConnectDelay = Setting.GameAppClientConnectDelay,
            RetryDelay = Setting.GameAppClientRetryDelay,
            HeartBeatInterval = Setting.GameAppClientHeartBeatInterval,
            MaxRetryCount = Setting.GameAppClientMaxRetryCount,
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

        var connectEndPoint = NetHelper.ParseEndPoint(Setting.DiscoveryCenterHost, Setting.DiscoveryCenterPort);

        if (connectEndPoint == default)
        {
            LogHelper.Error($"DiscoveryCenterHost: {Setting.DiscoveryCenterHost} is not a valid IP address; unable to start connection to DiscoveryCenter. Please check the configuration item DiscoveryCenterHost");
            return;
        }

        // 根据配置创建发现中心终结点并初始化客户端
        _gameAppServiceClient = new GameAppServiceClient(connectEndPoint, gameAppServiceConfiguration);
    }

    /// <summary>
    /// 心跳回调：当需要向发现中心发送心跳消息时触发。
    /// </summary>
    /// <remarks>
    /// Heartbeat callback: triggered when a heartbeat message needs to be sent to the discovery center.
    /// Can construct and return heartbeat data here to maintain connection with the discovery center.
    /// </remarks>
    /// <returns>返回要发送的心跳消息对象；如无需发送心跳，返回 null / Returns the heartbeat message object to be sent; returns null if no heartbeat is needed</returns>
    protected virtual MessageObject GameAppClientOnHeartBeat(string s)
    {
        return default;
    }

    /// <summary>
    /// 调用发现中心（DiscoveryCenter）的 RPC 方法。
    /// </summary>
    /// <remarks>
    /// Call RPC method of the discovery center (DiscoveryCenter).
    /// Used to initiate an RPC call to the discovery center and wait for a response.
    /// </remarks>
    /// <typeparam name="T">要返回的响应消息类型 / The response message type to return</typeparam>
    /// <param name="messageObject">要调用的 RPC 方法参数 / RPC method parameters to call</param>
    /// <param name="timeOut">调用超时时间（毫秒）/ Call timeout (milliseconds)</param>
    /// <returns>表示异步操作的任务，任务结果为 IRpcResult 对象 / A task representing the asynchronous operation, with the task result being an IRpcResult object</returns>
    public Task<IRpcResult> Call<T>(MessageObject messageObject, int timeOut = 10000) where T : IResponseMessage, new()
    {
        return _gameAppServiceClient.Call<T>(messageObject, timeOut);
    }

    /// <summary>
    /// 向发现中心（DiscoveryCenter）发送消息。
    /// </summary>
    /// <remarks>
    /// Send message to the discovery center (DiscoveryCenter).
    /// If the client is already initialized, then call its method to send the message to the discovery center server.
    /// </remarks>
    /// <param name="message">待发送的消息对象 / Message object to be sent</param>
    public void Send(MessageObject message)
    {
        // If the client is already initialized, then call its method to send the message to the discovery center server
        _gameAppServiceClient?.Send(message);
    }

    /// <summary>
    /// 与发现中心通信发生错误时的回调。
    /// </summary>
    /// <remarks>
    /// Callback when communication error occurs with the discovery center.
    /// Handles errors that occur during communication with the discovery center.
    /// </remarks>
    /// <param name="id">GameAppServiceClient 类当前实例的唯一标识符 / The unique identifier of the current instance of the GameAppServiceClient class</param>
    /// <param name="obj">包含异常信息的错误事件参数 / Error event arguments containing exception information</param>
    protected virtual void GameAppClientOnError(string id, ErrorEventArgs obj)
    {
        LogHelper.Error(LocalizationService.GetString(Localization.Keys.StartUp.DiscoveryCenterCommunicationError, Setting.ServerType, id, obj.Exception?.ToString() ?? "Unknown exception"));
    }

    /// <summary>
    /// 收到发现中心推送消息时的回调。
    /// </summary>
    /// <remarks>
    /// Callback when receiving messages pushed by the discovery center.
    /// Handles various messages received from the discovery center.
    /// </remarks>
    /// <param name="id">GameAppServiceClient 类当前实例的唯一标识符 / The unique identifier of the current instance of the GameAppServiceClient class</param>
    /// <param name="message">发现中心下发的消息对象 / Message object sent by the discovery center</param>
    protected virtual void GameAppClientOnMessage(string id, MessageObject message)
    {
        LogHelper.Debug(LocalizationService.GetString(Localization.Keys.StartUp.DiscoveryCenterMessageReceived, Setting.ServerType, id, message.ToFormatMessageString()));
    }

    /// <summary>
    /// 与发现中心连接断开时的回调。
    /// </summary>
    /// <remarks>
    /// Callback when connection to the discovery center is disconnected.
    /// Handles the situation when connection to the discovery center is disconnected.
    /// </remarks>
    /// <param name="id">GameAppServiceClient 类当前实例的唯一标识符 / The unique identifier of the current instance of the GameAppServiceClient class</param>
    protected virtual void GameAppClientOnClosed(string id)
    {
        LogHelper.Debug(LocalizationService.GetString(Localization.Keys.StartUp.DiscoveryCenterDisconnected, Setting.ServerType, id));
    }

    /// <summary>
    /// 与发现中心连接建立成功时的回调。
    /// </summary>
    /// <remarks>
    /// Callback when connection to the discovery center is successfully established.
    /// Handles the situation when connection to the discovery center is successfully established.
    /// </remarks>
    /// <param name="id">GameAppServiceClient 类当前实例的唯一标识符 / The unique identifier of the current instance of the GameAppServiceClient class</param>
    protected virtual void GameAppClientOnConnected(string id)
    {
        LogHelper.Debug(LocalizationService.GetString(Localization.Keys.StartUp.DiscoveryCenterConnected, Setting.ServerType, id));
    }
}
