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
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.Utility.Setting;

namespace GameFrameX.NetWork.Kcp;

/// <summary>
/// KCP network channel / KCP 网络通道
/// Implements INetWorkChannel interface
/// </summary>
public sealed class KcpNetWorkChannel : INetWorkChannel
{
    private readonly IKcpSession _kcpSession;
    private readonly KcpGameAppSession _gameAppSession;
    private readonly ConcurrentDictionary<string, object> _userDataKv = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private long _lastReceiveMessageTime;
    private bool _disposed;

    /// <summary>
    /// Gets the sending bytes length / 获取发送字节长度
    /// </summary>
    public ulong SendBytesLength { get; private set; }

    /// <summary>
    /// Gets the sending packet length / 获取发送数据包长度
    /// </summary>
    public ulong SendPacketLength { get; private set; }

    /// <summary>
    /// Gets the receiving bytes length / 获取接收字节长度
    /// </summary>
    public ulong ReceiveBytesLength { get; private set; }

    /// <summary>
    /// Gets the receiving packet length / 获取接收数据包长度
    /// </summary>
    public ulong ReceivePacketLength { get; private set; }

    /// <summary>
    /// Gets the game app session / 获取游戏应用会话
    /// </summary>
    public IGameAppSession GameAppSession
    {
        get { return _gameAppSession; }
    }

    /// <summary>
    /// Gets the KCP session / 获取 KCP 会话
    /// </summary>
    public IKcpSession KcpSession
    {
        get { return _kcpSession; }
    }

    /// <summary>
    /// Gets the application settings / 获取应用配置
    /// </summary>
    public AppSetting Setting { get; }

    /// <summary>
    /// Creates a new KCP network channel / 创建新的 KCP 网络通道
    /// </summary>
    /// <param name="kcpSession">KCP session / KCP 会话</param>
    /// <param name="setting">Application settings / 应用配置</param>
    public KcpNetWorkChannel(IKcpSession kcpSession, AppSetting setting)
    {
        _kcpSession = kcpSession ?? throw new ArgumentNullException(nameof(kcpSession));
        _gameAppSession = new KcpGameAppSession(kcpSession);
        Setting = setting ?? throw new ArgumentNullException(nameof(setting));
        _lastReceiveMessageTime = DateTime.UtcNow.Ticks;
    }

    /// <summary>
    /// Update the receiving packet bytes length / 更新接收数据包字节长度
    /// </summary>
    /// <param name="bufferLength">Buffer length / 缓冲区长度</param>
    public void UpdateReceivePacketBytesLength(ulong bufferLength)
    {
        ReceivePacketLength++;
        ReceiveBytesLength += bufferLength;
    }

    /// <summary>
    /// Asynchronously write a message / 异步写入消息
    /// </summary>
    /// <param name="msg">Network message / 网络消息</param>
    /// <param name="errorCode">Error code / 错误码</param>
    public async Task WriteAsync(INetworkMessage msg, int errorCode = 0)
    {
        if (_disposed)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(msg, nameof(msg));

        if (msg is IResponseMessage respMsg)
        {
            if (respMsg.ErrorCode == 0 && errorCode != 0)
            {
                respMsg.ErrorCode = errorCode;
            }
        }

        var actorId = GetData<long>(GlobalConst.ActorIdKey);
        var messageData = MessageHelper.EncoderHandler.Handler(msg);

        if (Setting.IsDebug && Setting.IsDebugSend)
        {
            if (msg is IHeartBeatMessage)
            {
                if (Setting.IsDebugSendHeartBeat)
                {
                    LogHelper.Debug("Send HeartBeat Message:{actorId} {message}", actorId, LocalizationService.GetString(Localization.Keys.NetWork.MessageSent, msg.ToFormatMessageString(actorId)));
                }
            }
            else
            {
                var responseErrorCode = msg is IResponseMessage respMsg2 ? respMsg2.ErrorCode : 0;
                LogHelper.Debug("Send Message:{actorId} {errorCode} {message}", actorId, responseErrorCode, LocalizationService.GetString(Localization.Keys.NetWork.MessageSent, msg.ToFormatMessageString(actorId)));
            }
        }

        if (!_kcpSession.IsConnected)
        {
            return;
        }

        SendBytesLength += (ulong)messageData.Length;
        SendPacketLength++;

        using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(Setting.NetWorkSendTimeOutSeconds)))
        {
            try
            {
                await _kcpSession.SendAsync(messageData, cts.Token);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Send Message Error:{actorId} {message}", actorId, ex.Message);
            }
        }
    }

    /// <summary>
    /// Close the channel / 关闭通道
    /// </summary>
    public void Close()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _cancellationTokenSource.Cancel();
        _kcpSession.Close();
        ClearData();
    }

    /// <summary>
    /// Check if channel is closed / 检查通道是否已关闭
    /// </summary>
    /// <returns>True if closed / 如果已关闭则返回 true</returns>
    public bool IsClosed()
    {
        return _disposed || _cancellationTokenSource.IsCancellationRequested;
    }

    /// <summary>
    /// Get user data / 获取用户数据
    /// </summary>
    /// <typeparam name="T">Data type / 数据类型</typeparam>
    /// <param name="key">Data key / 数据键</param>
    /// <returns>User data / 用户数据</returns>
    public T GetData<T>(string key)
    {
        if (_userDataKv.TryGetValue(key, out var v))
        {
            return (T)v;
        }

        return default;
    }

    /// <summary>
    /// Clear all custom data / 清除所有自定义数据
    /// </summary>
    public void ClearData()
    {
        _userDataKv.Clear();
        SendPacketLength = default;
        SendBytesLength = default;
        ReceivePacketLength = default;
        ReceiveBytesLength = default;
    }

    /// <summary>
    /// Remove user data / 移除用户数据
    /// </summary>
    /// <param name="key">Data key / 数据键</param>
    public void RemoveData(string key)
    {
        _userDataKv.TryRemove(key, out _);
    }

    /// <summary>
    /// Set user data / 设置用户数据
    /// </summary>
    /// <param name="key">Data key / 数据键</param>
    /// <param name="value">Data value / 数据值</param>
    public void SetData(string key, object value)
    {
        _userDataKv[key] = value;
    }

    /// <summary>
    /// Update the last received message time / 更新最后接收消息时间
    /// </summary>
    /// <param name="offsetTicks">Offset ticks / 偏移 ticks</param>
    public void UpdateReceiveMessageTime(long offsetTicks = 0)
    {
        _lastReceiveMessageTime = DateTime.UtcNow.Ticks + offsetTicks;
    }

    /// <summary>
    /// Get the elapsed time since last message in seconds / 获取距离上次消息的秒数
    /// </summary>
    /// <param name="utcTime">Current UTC time / 当前 UTC 时间</param>
    /// <returns>Elapsed time in seconds / 距离上次消息的秒数</returns>
    public long GetLastMessageTimeSecond(in DateTime utcTime)
    {
        return (utcTime.Ticks - _lastReceiveMessageTime) / 10000_000;
    }
}
