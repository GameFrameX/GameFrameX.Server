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
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.WebSocket.Server;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Logger;
using GameFrameX.Utility.Setting;
using GameFrameX.Foundation.Localization.Core;

namespace GameFrameX.NetWork;

/// <summary>
/// 基础网络通道
/// </summary>
public abstract class BaseNetWorkChannel : INetWorkChannel
{
    /// <summary>
    /// WebSocket会话
    /// </summary>
    private readonly WebSocketSession _webSocketSession;

    /// <summary>
    /// 关闭源
    /// </summary>
    protected readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

    /// <summary>
    /// 网络发送超时时间,单位秒
    /// </summary>
    protected readonly TimeSpan NetWorkSendTimeOutSecondsTimeSpan;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="session"></param>
    /// <param name="setting"></param>
    /// <param name="rpcSession"></param>
    /// <param name="isWebSocket"></param>
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
    /// 是否是WebSocket
    /// </summary>
    public bool IsWebSocket { get; }

    /// <summary>
    /// 设置
    /// </summary>
    public AppSetting Setting { get; }

    /// <summary>
    /// 发送字节长度 - 记录通过此通道发送的总字节数
    /// </summary>
    public ulong SendBytesLength { get; private set; }

    /// <summary>
    /// 发送数据包长度 - 记录通过此通道发送的数据包总数
    /// </summary>
    public ulong SendPacketLength { get; private set; }

    /// <summary>
    /// 接收字节长度 - 记录通过此通道接收的总字节数
    /// </summary>
    public ulong ReceiveBytesLength { get; private set; }

    /// <summary>
    /// 接收数据包长度 - 记录通过此通道接收的数据包总数
    /// </summary>
    public ulong ReceivePacketLength { get; private set; }

    /// <summary>
    /// 更新接收数据包字节长度
    /// </summary>
    /// <param name="bufferLength">接收数据包字节长度</param>
    public void UpdateReceivePacketBytesLength(ulong bufferLength)
    {
        ReceivePacketLength++;
        ReceiveBytesLength += bufferLength;
    }

    /// <summary>
    /// 会话
    /// </summary>
    public IGameAppSession GameAppSession { get; }

    /// <summary>
    /// Rpc会话
    /// </summary>
    public IRpcSession RpcSession { get; }

    /// <summary>
    /// 异步写入消息
    /// </summary>
    /// <param name="messageObject">消息对象</param>
    /// <param name="errorCode">错误码</param>
    /// <returns></returns>
    public virtual async Task WriteAsync(INetworkMessage messageObject, int errorCode = 0)
    {
        ArgumentNullException.ThrowIfNull(messageObject, nameof(messageObject));

        if (messageObject is IResponseMessage responseMessage && responseMessage.ErrorCode == default && errorCode != default)
        {
            responseMessage.ErrorCode = errorCode;
        }

        var messageData = MessageHelper.EncoderHandler.Handler(messageObject);
        if (Setting.IsDebug && Setting.IsDebugSend)
        {
            var actorId = GetData<long>(GlobalConst.ActorIdKey);
            // 判断是否是心跳消息
            if (messageObject is IHeartBeatMessage)
            {
                // 判断是否打印心跳消息的发送
                if (Setting.IsDebugSendHeartBeat)
                {
                    LogHelper.Debug(LocalizationService.GetString(
                                        GameFrameX.Localization.Keys.NetWork.MessageSent,
                                        messageObject.ToFormatMessageString(actorId)));
                }
            }
            else
            {
                LogHelper.Debug(LocalizationService.GetString(
                                    GameFrameX.Localization.Keys.NetWork.MessageSent,
                                    messageObject.ToFormatMessageString(actorId)));
            }
        }

        if (!GameAppSession.IsConnected)
        {
            return;
        }

        SendBytesLength += ((ulong)messageData.Length);
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
                LogHelper.Error(LocalizationService.GetString(
                                    GameFrameX.Localization.Keys.NetWork.MessageSendTimeout,
                                    exception.Message));
            }
            catch (Exception e)
            {
                LogHelper.Error(e);
            }
        }
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public virtual void Close()
    {
        ClearData();
        CancellationTokenSource.Cancel();
    }

    /// <summary>
    /// 是否关闭
    /// </summary>
    /// <returns></returns>
    public virtual bool IsClosed()
    {
        return CancellationTokenSource.IsCancellationRequested;
    }

    #region Data

    private readonly ConcurrentDictionary<string, object> _userDataKv = new ConcurrentDictionary<string, object>();

    /// <summary>
    /// 获取用户数据对象.
    /// 可能会发生转换失败的异常。
    /// 如果数据不存在则返回null
    /// </summary>
    /// <param name="key">数据Key</param>
    /// <typeparam name="T">将要获取的数据类型。</typeparam>
    /// <returns>用户数据对象</returns>
    public T GetData<T>(string key)
    {
        if (_userDataKv.TryGetValue(key, out var v))
        {
            return (T)v;
        }

        return default;
    }

    /// <summary>
    /// 清除自定义数据
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
    /// 删除自定义数据
    /// </summary>
    /// <param name="key"></param>
    public void RemoveData(string key)
    {
        _userDataKv.Remove(key, out _);
    }

    /// <summary>
    /// 设置自定义数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetData(string key, object value)
    {
        _userDataKv[key] = value;
    }

    #endregion

    #region MessageTime

    private long _lastReceiveMessageTime;

    /// <summary>
    /// 更新接收消息的时间
    /// </summary>
    /// <param name="offsetTicks"></param>
    public void UpdateReceiveMessageTime(long offsetTicks = 0)
    {
        _lastReceiveMessageTime = DateTime.UtcNow.Ticks + offsetTicks;
    }

    /// <summary>
    /// 获取最后接收消息到现在的时间。单位秒
    /// </summary>
    /// <param name="utcTime"></param>
    /// <returns></returns>
    public long GetLastMessageTimeSecond(in DateTime utcTime)
    {
        return (utcTime.Ticks - _lastReceiveMessageTime) / 10000_000;
    }

    #endregion
}