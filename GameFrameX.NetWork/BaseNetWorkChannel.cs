// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


using System.Collections.Concurrent;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.WebSocket.Server;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Logger;
using GameFrameX.Utility.Setting;
using GameFrameX.Foundation.Localization.Core;

namespace GameFrameX.NetWork;

/// <summary>
/// 基础网络通道。
/// </summary>
/// <remarks>
/// Base class for network channels, providing common functionality for client-server communication.
/// </remarks>
public abstract class BaseNetWorkChannel : INetWorkChannel
{
    /// <summary>
    /// WebSocket会话。
    /// </summary>
    /// <remarks>
    /// The WebSocket session for WebSocket connections.
    /// </remarks>
    private readonly WebSocketSession _webSocketSession;

    /// <summary>
    /// 关闭源。
    /// </summary>
    /// <remarks>
    /// Cancellation token source for managing connection closure.
    /// </remarks>
    protected readonly CancellationTokenSource CancellationTokenSource = new();

    /// <summary>
    /// 网络发送超时时间，单位秒。
    /// </summary>
    /// <remarks>
    /// Timeout duration for network send operations.
    /// </remarks>
    protected readonly TimeSpan NetWorkSendTimeOutSecondsTimeSpan;

    /// <summary>
    /// 初始化基础网络通道。
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the base network channel.
    /// </remarks>
    /// <param name="session">游戏应用会话对象 / The game application session object</param>
    /// <param name="setting">应用配置 / The application settings</param>
    /// <param name="rpcSession">RPC会话对象 / The RPC session object</param>
    /// <param name="isWebSocket">是否为WebSocket连接 / Whether the connection is WebSocket</param>
    public BaseNetWorkChannel(IGameAppSession session, AppSetting setting, IRpcSession rpcSession, bool isWebSocket)
    {
        ArgumentNullException.ThrowIfNull(setting, nameof(setting));
        GameAppSession = session;
        IsWebSocket = isWebSocket;
        Setting = setting;
        RpcSession = rpcSession;
        SendPacketLength = default;
        SendBytesLength = default;
        ReceivePacketLength = default;
        ReceiveBytesLength = default;
        NetWorkSendTimeOutSecondsTimeSpan = TimeSpan.FromSeconds(Setting.NetWorkSendTimeOutSeconds);
        if (isWebSocket)
        {
            _webSocketSession = (WebSocketSession)session;
        }
    }

    /// <summary>
    /// 获取是否为WebSocket连接。
    /// </summary>
    /// <remarks>
    /// Gets whether this channel uses WebSocket protocol.
    /// </remarks>
    /// <value>如果是WebSocket连接则为 <c>true</c>；否则为 <c>false</c> / <c>true</c> if WebSocket; otherwise <c>false</c></value>
    public bool IsWebSocket { get; }

    /// <summary>
    /// 获取应用配置。
    /// </summary>
    /// <remarks>
    /// Gets the application settings for this channel.
    /// </remarks>
    /// <value>应用配置 / The application settings</value>
    public AppSetting Setting { get; }

    /// <summary>
    /// 获取发送字节长度，记录通过此通道发送的总字节数。
    /// </summary>
    /// <remarks>
    /// Gets the total number of bytes sent through this channel.
    /// </remarks>
    /// <value>发送字节长度 / Total bytes sent</value>
    public ulong SendBytesLength { get; private set; }

    /// <summary>
    /// 获取发送数据包长度，记录通过此通道发送的数据包总数。
    /// </summary>
    /// <remarks>
    /// Gets the total number of packets sent through this channel.
    /// </remarks>
    /// <value>发送数据包总数 / Total packets sent</value>
    public ulong SendPacketLength { get; private set; }

    /// <summary>
    /// 获取接收字节长度，记录通过此通道接收的总字节数。
    /// </summary>
    /// <remarks>
    /// Gets the total number of bytes received through this channel.
    /// </remarks>
    /// <value>接收字节长度 / Total bytes received</value>
    public ulong ReceiveBytesLength { get; private set; }

    /// <summary>
    /// 获取接收数据包长度，记录通过此通道接收的数据包总数。
    /// </summary>
    /// <remarks>
    /// Gets the total number of packets received through this channel.
    /// </remarks>
    /// <value>接收数据包总数 / Total packets received</value>
    public ulong ReceivePacketLength { get; private set; }

    /// <summary>
    /// 更新接收数据包字节长度。
    /// </summary>
    /// <remarks>
    /// Updates the received packet and byte counters.
    /// </remarks>
    /// <param name="bufferLength">接收数据包字节长度 / The length of the received buffer in bytes</param>
    public void UpdateReceivePacketBytesLength(ulong bufferLength)
    {
        ReceivePacketLength++;
        ReceiveBytesLength += bufferLength;
    }

    /// <summary>
    /// 获取游戏应用会话。
    /// </summary>
    /// <remarks>
    /// Gets the game application session associated with this channel.
    /// </remarks>
    /// <value>游戏应用会话 / The game application session</value>
    public IGameAppSession GameAppSession { get; }

    /// <summary>
    /// 获取RPC会话。
    /// </summary>
    /// <remarks>
    /// Gets the RPC session for managing request-response communication.
    /// </remarks>
    /// <value>RPC会话 / The RPC session</value>
    public IRpcSession RpcSession { get; }

    /// <summary>
    /// 异步写入消息。
    /// </summary>
    /// <remarks>
    /// Asynchronously writes a network message to the channel.
    /// </remarks>
    /// <param name="messageObject">消息对象 / The network message object to send</param>
    /// <param name="errorCode">错误码 / The error code to include in response messages</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    public virtual async Task WriteAsync(INetworkMessage messageObject, int errorCode = 0)
    {
        ArgumentNullException.ThrowIfNull(messageObject, nameof(messageObject));
        var responseErrorCode = 0;
        if (messageObject is IResponseMessage responseMessage)
        {
            if (responseMessage.ErrorCode == 0 && errorCode != 0)
            {
                responseMessage.ErrorCode = errorCode;
                responseErrorCode = errorCode;
            }
            else
            {
                responseErrorCode = responseMessage.ErrorCode;
            }
        }

        var actorId = GetData<long>(GlobalConst.ActorIdKey);
        var messageData = MessageHelper.EncoderHandler.Handler(messageObject);
        if (Setting.IsDebug && Setting.IsDebugSend)
        {
            // 判断是否是心跳消息
            if (messageObject is IHeartBeatMessage)
            {
                // 判断是否打印心跳消息的发送
                if (Setting.IsDebugSendHeartBeat)
                {
                    LogHelper.Debug("Send HeartBeat Message:{actorId} {message}", actorId, LocalizationService.GetString(Localization.Keys.NetWork.MessageSent, messageObject.ToFormatMessageString(actorId)));
                }
            }
            else
            {
                LogHelper.Debug("Send Message:{actorId} {errorCode} {message}", actorId, responseErrorCode, LocalizationService.GetString(Localization.Keys.NetWork.MessageSent, messageObject.ToFormatMessageString(actorId)));
            }
        }

        if (!GameAppSession.IsConnected)
        {
            return;
        }

        SendBytesLength += (ulong)messageData.Length;
        SendPacketLength++;
        using (var cancellationTokenSource = new CancellationTokenSource(NetWorkSendTimeOutSecondsTimeSpan))
        {
            try
            {
                if (IsWebSocket)
                {
                    await _webSocketSession.SendAsync(messageData, cancellationTokenSource.Token);
                }
                else
                {
                    await GameAppSession.SendAsync(messageData, cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException exception)
            {
                LogHelper.Error("Send Message Timeout:{actorId} {message}", actorId, LocalizationService.GetString(Localization.Keys.NetWork.MessageSendTimeout, exception.Message));
            }
            catch (Exception e)
            {
                LogHelper.Error("Send Message Error:{actorId} {message}", actorId, e.Message);
            }
        }
    }

    /// <summary>
    /// 关闭通道。
    /// </summary>
    /// <remarks>
    /// Closes the network channel and releases associated resources.
    /// </remarks>
    public virtual void Close()
    {
        ClearData();
        CancellationTokenSource.Cancel();
    }

    /// <summary>
    /// 检查通道是否已关闭。
    /// </summary>
    /// <remarks>
    /// Checks whether the channel has been closed.
    /// </remarks>
    /// <returns>如果已关闭则为 <c>true</c>；否则为 <c>false</c> / <c>true</c> if closed; otherwise <c>false</c></returns>
    public virtual bool IsClosed()
    {
        return CancellationTokenSource.IsCancellationRequested;
    }

    #region Data

    private readonly ConcurrentDictionary<string, object> _userDataKv = new();

    /// <summary>
    /// 获取用户数据对象。
    /// </summary>
    /// <remarks>
    /// Gets user data by key. May throw conversion exceptions. Returns default value if key does not exist.
    /// </remarks>
    /// <typeparam name="T">将要获取的数据类型 / The type of data to retrieve</typeparam>
    /// <param name="key">数据键 / The data key</param>
    /// <returns>用户数据对象 / The user data object, or default value if not found</returns>
    public T GetData<T>(string key)
    {
        if (_userDataKv.TryGetValue(key, out var v))
        {
            return (T)v;
        }

        return default;
    }

    /// <summary>
    /// 清除自定义数据。
    /// </summary>
    /// <remarks>
    /// Clears all custom data and resets packet/byte counters.
    /// </remarks>
    public void ClearData()
    {
        _userDataKv.Clear();
        SendPacketLength = default;
        SendBytesLength = default;
        ReceivePacketLength = default;
        ReceiveBytesLength = default;
    }

    /// <summary>
    /// 删除自定义数据。
    /// </summary>
    /// <remarks>
    /// Removes custom data by key.
    /// </remarks>
    /// <param name="key">数据键 / The data key to remove</param>
    public void RemoveData(string key)
    {
        _userDataKv.Remove(key, out _);
    }

    /// <summary>
    /// 设置自定义数据。
    /// </summary>
    /// <remarks>
    /// Sets custom data by key.
    /// </remarks>
    /// <param name="key">数据键 / The data key</param>
    /// <param name="value">数据值 / The data value</param>
    public void SetData(string key, object value)
    {
        _userDataKv[key] = value;
    }

    #endregion

    #region MessageTime

    private long _lastReceiveMessageTime;

    /// <summary>
    /// 更新接收消息的时间。
    /// </summary>
    /// <remarks>
    /// Updates the timestamp of the last received message.
    /// </remarks>
    /// <param name="offsetTicks">时间偏移量（ticks） / Time offset in ticks</param>
    public void UpdateReceiveMessageTime(long offsetTicks = 0)
    {
        _lastReceiveMessageTime = DateTime.UtcNow.Ticks + offsetTicks;
    }

    /// <summary>
    /// 获取最后接收消息到现在的时间，单位秒。
    /// </summary>
    /// <remarks>
    /// Gets the elapsed time since the last received message in seconds.
    /// </remarks>
    /// <param name="utcTime">当前UTC时间 / The current UTC time</param>
    /// <returns>距离上次接收消息的秒数 / The number of seconds since the last received message</returns>
    public long GetLastMessageTimeSecond(in DateTime utcTime)
    {
        return (utcTime.Ticks - _lastReceiveMessageTime) / 10000_000;
    }

    #endregion
}