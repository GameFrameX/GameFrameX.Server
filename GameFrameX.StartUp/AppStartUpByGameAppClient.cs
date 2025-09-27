using System.Net;
using System.Reflection;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Message;
using GameFrameX.NetWork.Messages;
using GameFrameX.StartUp.DiscoverCenter;
using GameFrameX.StartUp.Extensions;
using GameFrameX.SuperSocket.Connection;
using GameFrameX.SuperSocket.Primitives;
using GameFrameX.SuperSocket.Server;
using GameFrameX.SuperSocket.Server.Abstractions;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.Server.Host;
using GameFrameX.SuperSocket.WebSocket;
using GameFrameX.SuperSocket.WebSocket.Server;
using GameFrameX.Utility;
using GameFrameX.Utility.Setting;
using Grafana.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Serilog;
using CloseReason = GameFrameX.SuperSocket.WebSocket.CloseReason;
using ErrorEventArgs = GameFrameX.SuperSocket.ClientEngine.ErrorEventArgs;

namespace GameFrameX.StartUp;

/// <summary>
/// 程序启动器基类 - 提供TCP和WebSocket服务器的基础功能实现
/// </summary>
public abstract partial class AppStartUpBase
{
    private GameAppClient _gameAppClient;

    /// <summary>
    /// 启动与发现中心（DiscoveryCenter）通信的客户端，用于注册当前服务器实例并接收发现中心推送的消息
    /// </summary>
    private async void StartGameAppClient()
    {
        // 创建客户端事件回调对象
        var gameAppClientEvent = new GameAppClientEvent
        {
            OnConnected = GameAppClientOnConnected,
            OnClosed = GameAppClientOnClosed,
            OnMessage = GameAppClientOnMessage,
            OnError = GameAppClientOnError,
        };

        // 根据配置创建发现中心终结点并初始化客户端
        _gameAppClient = new GameAppClient(gameAppClientEvent, new DnsEndPoint(Setting.DiscoveryCenterIp, Setting.DiscoveryCenterPort));

        // 异步启动客户端，开始与发现中心建立连接
        await _gameAppClient.EntryAsync();
    }

    /// <summary>
    /// 向发现中心（DiscoveryCenter）发送消息
    /// </summary>
    /// <param name="message">待发送的消息对象</param>
    public void Send(MessageObject message)
    {
        // 如果客户端已初始化，则调用其方法将消息发送至发现中心服务器
        _gameAppClient?.SendToServer(message);
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