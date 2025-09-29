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
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.SuperSocket.ClientEngine;
using GameFrameX.Utility;

namespace GameFrameX.StartUp.DiscoverCenter;

/// <summary>
/// 游戏程序TCP客户端类,用于处理与服务器的TCP连接和消息收发
/// </summary>
internal sealed class GameAppClient
{
    /// <summary>
    /// 连接延迟时间,单位毫秒
    /// </summary>
    public int ConnectDelay { get; }

    /// <summary>
    /// 内部TCP会话实例，负责底层网络通信
    /// </summary>
    private readonly AsyncTcpSession _mTcpClient;

    /// <summary>
    /// 当前重连次数计数器
    /// </summary>
    public int RetryCount { get; private set; }

    /// <summary>
    /// 客户端事件回调集合，用于通知外部连接、断开、消息等事件
    /// </summary>
    private readonly GameAppClientEvent _mGameAppClientEvent;

    /// <summary>
    /// 服务器终结点（IP与端口）
    /// </summary>
    private readonly EndPoint _serverHost;

    /// <summary>
    /// 心跳包发送间隔，单位毫秒
    /// </summary>
    private readonly int _heartBeatInterval;

    /// <summary>
    /// 每次重连之间的延迟时间，单位毫秒
    /// </summary>
    private readonly int _retryDelay;

    /// <summary>
    /// 最大重连次数，-1表示无限重试
    /// </summary>
    public int MaxRetryCount { get; }

    /// <summary>
    /// 标记当前实例是否已被释放，防止重复释放或空操作
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// 初始化游戏TCP客户端
    /// </summary>
    /// <param name="clientEvent">客户端事件回调结构体，包含连接、断开、消息等事件的处理委托</param>
    /// <param name="endPoint">服务器端点信息（IP和端口）</param>
    /// <param name="option">配置信息</param>
    public GameAppClient(GameAppClientEvent clientEvent, EndPoint endPoint, GameAppClientOption option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));
        ConnectDelay = option.ConnectDelay;
        _mGameAppClientEvent = clientEvent;
        _serverHost = endPoint;
        _heartBeatInterval = option.HeartBeatInterval;
        _retryDelay = option.RetryDelay;
        MaxRetryCount = option.MaxRetryCount;
        _mTcpClient = new AsyncTcpSession();
        _mTcpClient.Connected += OnClientOnConnected;
        _mTcpClient.Closed += OnClientOnClosed;
        _mTcpClient.DataReceived += OnClientOnDataReceived;
        _mTcpClient.Error += OnClientOnError;
    }

    /// <summary>
    /// 停止客户端连接，关闭底层TCP会话
    /// </summary>
    public void Stop()
    {
        _isDisposed = true;
        _mTcpClient.Close();
    }

    /// <summary>
    /// 启动客户端并尝试连接服务器，处理消息编解码和压缩解压缩处理器的初始化。
    /// 内部采用无限循环，在连接成功前持续重试，并在连接成功后周期性发送心跳。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    public async Task EntryAsync()
    {
        // 主循环：负责连接、重连与心跳
        while (true)
        {
            if (_isDisposed)
            {
                break;
            }

            // 如果未连接且未处于连接中，则尝试连接
            if (!_mTcpClient.IsConnected && !_mTcpClient.IsInConnecting)
            {
                LogHelper.Debug("尝试连接到发现中心服务器...");
                _mTcpClient.Connect(_serverHost);
                await Task.Delay(ConnectDelay);

                // 若连接成功或正在连接，则跳过本次循环
                if (_mTcpClient.IsConnected || _mTcpClient.IsInConnecting)
                {
                    continue;
                }

                // 未达到最大重连次数（或无限重试）则进行重连
                if (RetryCount < MaxRetryCount || MaxRetryCount < 0)
                {
                    LogHelper.Info($"未连接到发现中心服务器, 尝试重连 (尝试次数: {RetryCount + 1}/{(MaxRetryCount < 0 ? "∞" : MaxRetryCount.ToString())})...");
                    _mTcpClient.Connect(_serverHost);
                    RetryCount++;
                    await Task.Delay(_retryDelay);
                }
                else
                {
                    LogHelper.Info("重连次数已达到上限，停止尝试。");
                    break;
                }
            }
            else
            {
                // 连接成功，重置重连计数
                RetryCount = 0;
                // 发送心跳
                SendHeartBeat();
                // 等待下一次心跳间隔
                await Task.Delay(_heartBeatInterval);
            }
        }
    }

    /// <summary>
    /// 发送心跳包到服务器
    /// 更新心跳时间戳后通过SendToServer发送
    /// </summary>
    private void SendHeartBeat()
    {
        var messageObject = _mGameAppClientEvent.OnHeartBeat?.Invoke();
        if (messageObject != null)
        {
            SendToServer(messageObject);
        }
    }

    /// <summary>
    /// 发送消息到服务器
    /// 内部使用MessageHelper编码后通过TcpClient发送
    /// </summary>
    /// <param name="messageObject">要发送的消息对象</param>
    public void SendToServer(MessageObject messageObject)
    {
        var buffer = MessageHelper.EncoderHandler.Handler(messageObject);
        if (buffer != null)
        {
            _mTcpClient.Send(buffer);
        }
    }

    /// <summary>
    /// 处理客户端错误事件
    /// 将错误信息通过GameAppClientEvent回调给上层
    /// </summary>
    /// <param name="client">触发事件的TcpSession对象</param>
    /// <param name="e">包含异常信息的错误事件参数</param>
    private void OnClientOnError(object client, SuperSocket.ClientEngine.ErrorEventArgs e)
    {
        _mGameAppClientEvent.OnError?.Invoke(e);
    }

    /// <summary>
    /// 处理客户端连接关闭事件
    /// 记录日志并通过GameAppClientEvent通知上层连接已断开
    /// </summary>
    /// <param name="client">触发事件的TcpSession对象</param>
    /// <param name="e">事件参数</param>
    private void OnClientOnClosed(object client, EventArgs e)
    {
        LogHelper.Info("客户端断开连接");
        _mGameAppClientEvent.OnClosed?.Invoke();
    }

    /// <summary>
    /// 处理客户端连接成功事件
    /// 记录日志并通过GameAppClientEvent通知上层连接已建立
    /// </summary>
    /// <param name="client">触发事件的TcpSession对象</param>
    /// <param name="e">事件参数</param>
    private void OnClientOnConnected(object client, EventArgs e)
    {
        LogHelper.Info("客户端成功连接到服务器");
        _mGameAppClientEvent.OnConnected?.Invoke();
    }

    /// <summary>
    /// 处理接收到数据事件
    /// 将接收到的二进制数据解码为消息对象，若为内部网络消息则反序列化后通过回调通知上层
    /// </summary>
    /// <param name="client">触发事件的TcpSession对象</param>
    /// <param name="e">包含接收数据的数据事件参数</param>
    private void OnClientOnDataReceived(object client, DataEventArgs e)
    {
        var message = MessageHelper.DecoderHandler.Handler(e.Data.ReadBytesValue(e.Offset, e.Length));

        if (message is InnerNetworkMessage innerNetworkMessage)
        {
            _mGameAppClientEvent.OnMessage?.Invoke((MessageObject)innerNetworkMessage.DeserializeMessageObject());
        }
    }
}