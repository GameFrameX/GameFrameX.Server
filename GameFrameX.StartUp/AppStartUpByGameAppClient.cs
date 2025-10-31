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
/// 程序启动器基类 - 提供TCP和WebSocket服务器的基础功能实现
/// </summary>
public abstract partial class AppStartUpBase
{
    private GameAppServiceClient _gameAppClient;

    /// <summary>
    /// 启动与发现中心（DiscoveryCenter）通信的客户端，用于注册当前服务器实例并接收发现中心推送的消息
    /// </summary>
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
        _gameAppClient = new GameAppServiceClient(endPoint, gameAppServiceConfiguration);
    }

    /// <summary>
    /// 心跳回调：当需要向发现中心发送心跳消息时触发，可在此构造并返回心跳数据
    /// </summary>
    /// <returns>返回要发送的心跳消息对象；如无需发送心跳，返回 null</returns>
    protected virtual MessageObject GameAppClientOnHeartBeat()
    {
        return default;
    }

    /// <summary>
    /// 调用发现中心（DiscoveryCenter）的RPC方法
    /// </summary>
    /// <typeparam name="T">要返回的响应消息类型</typeparam>
    /// <param name="messageObject">要调用的RPC方法参数</param>
    /// <param name="timeOut">调用超时时间（毫秒）</param>
    /// <returns>表示异步操作的任务，任务结果为IRpcResult对象</returns>
    public Task<IRpcResult> Call<T>(MessageObject messageObject, int timeOut = 10000) where T : IResponseMessage, new()
    {
        return _gameAppClient.Call<T>(messageObject, timeOut);
    }

    /// <summary>
    /// 向发现中心（DiscoveryCenter）发送消息
    /// </summary>
    /// <param name="message">待发送的消息对象</param>
    public void Send(MessageObject message)
    {
        // 如果客户端已初始化，则调用其方法将消息发送至发现中心服务器
        _gameAppClient?.Send(message);
    }

    /// <summary>
    /// 与发现中心通信发生错误时的回调
    /// </summary>
    /// <param name="obj">包含异常信息的错误事件参数</param>
    protected virtual void GameAppClientOnError(ErrorEventArgs obj)
    {
        LogHelper.Error($"服务器{Setting.ServerType}与发现中心通信发生错误，e:{obj.Exception}");
    }

    /// <summary>
    /// 收到发现中心推送消息时的回调
    /// </summary>
    /// <param name="message">发现中心下发的消息对象</param>
    protected virtual void GameAppClientOnMessage(MessageObject message)
    {
        LogHelper.Debug($"服务器{Setting.ServerType}接收到发现中心消息,{message.ToFormatMessageString()}");
    }

    /// <summary>
    /// 与发现中心连接断开时的回调
    /// </summary>
    protected virtual void GameAppClientOnClosed()
    {
        LogHelper.Debug($"服务器{Setting.ServerType}与发现中心断开连接...");
    }

    /// <summary>
    /// 与发现中心连接建立成功时的回调
    /// </summary>
    protected virtual void GameAppClientOnConnected()
    {
        LogHelper.Debug($"服务器{Setting.ServerType}连接到发现中心成功...");
    }
}