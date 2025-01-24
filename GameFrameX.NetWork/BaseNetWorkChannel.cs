using System.Collections.Concurrent;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.WebSocket.Server;
using GameFrameX.Utility.Extensions;
using GameFrameX.Utility.Log;
using GameFrameX.Utility.Setting;

namespace GameFrameX.NetWork;

/// <summary>
/// 基础网络通道
/// </summary>
public class BaseNetWorkChannel : INetWorkChannel
{
    /// <summary>
    /// 消息编码器
    /// </summary>
    private readonly IMessageEncoderHandler _messageEncoder;

    /// <summary>
    /// WebSocket会话
    /// </summary>
    private readonly WebSocketSession _webSocketSession;

    /// <summary>
    /// 关闭源
    /// </summary>
    protected readonly CancellationTokenSource CancellationTokenSource = new();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="session"></param>
    /// <param name="setting"></param>
    /// <param name="messageEncoder"></param>
    /// <param name="rpcSession"></param>
    /// <param name="isWebSocket"></param>
    public BaseNetWorkChannel(IGameAppSession session, AppSetting setting, IMessageEncoderHandler messageEncoder, IRpcSession rpcSession, bool isWebSocket)
    {
        setting.CheckNotNull(nameof(setting));
        messageEncoder.CheckNotNull(nameof(messageEncoder));
        GameAppSession = session;
        IsWebSocket = isWebSocket;
        Setting = setting;
        _messageEncoder = messageEncoder;
        RpcSession = rpcSession;
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
        messageObject.CheckNotNull(nameof(messageObject));

        if (messageObject is IResponseMessage responseMessage)
        {
            responseMessage.ErrorCode = errorCode;
        }

        var messageData = _messageEncoder.Handler(messageObject);
        if (Setting.IsDebug && Setting.IsDebugSend)
        {
            LogHelper.Debug($"---发送{messageObject.ToFormatMessageString()}");
        }


        if (IsWebSocket)
        {
            if (_webSocketSession.State == SessionState.Connected)
            {
                try
                {
                    await _webSocketSession.SendAsync(messageData);
                }
                catch (Exception e)
                {
                    LogHelper.Error(e);
                }
            }
        }
        else
        {
            var appSession = (IAppSession)GameAppSession;
            if (appSession.Connection.IsClosed == false && appSession.State == SessionState.Connected)
            {
                try
                {
                    await GameAppSession.SendAsync(messageData);
                }
                catch (Exception e)
                {
                    LogHelper.Error(e);
                }
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

    private readonly ConcurrentDictionary<string, object> _userDataKv = new();


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