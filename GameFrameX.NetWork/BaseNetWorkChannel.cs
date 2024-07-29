using System.Collections.Concurrent;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.Setting;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.WebSocket.Server;
using GameFrameX.Utility;

namespace GameFrameX.NetWork
{
    /// <summary>
    /// 基础网络通道
    /// </summary>
    public class BaseNetWorkChannel : INetWorkChannel
    {
        /// <summary>
        /// 关闭源
        /// </summary>
        protected readonly CancellationTokenSource CloseSrc = new CancellationTokenSource();

        /// <summary>
        /// 会话
        /// </summary>
        public IGameAppSession Session { get; }

        /// <summary>
        /// Rpc会话
        /// </summary>
        public IRpcSession RpcSession { get; }

        /// <summary>
        /// 消息编码器
        /// </summary>
        private readonly IMessageEncoderHandler _messageEncoder;

        /// <summary>
        /// 是否是WebSocket
        /// </summary>
        public bool IsWebSocket { get; }

        /// <summary>
        /// 设置
        /// </summary>
        public AppSetting Setting { get; }

        /// <summary>
        /// WebSocket会话
        /// </summary>
        private readonly WebSocketSession _webSocketSession;

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
            Session         = session;
            IsWebSocket     = isWebSocket;
            Setting         = setting;
            _messageEncoder = messageEncoder;
            RpcSession      = rpcSession;
            if (isWebSocket)
            {
                _webSocketSession = (WebSocketSession)session;
            }
        }

        /// <summary>
        /// 写入消息
        /// </summary>
        /// <param name="messageObject">消息对象</param>
        public virtual async void Write(IMessage messageObject)
        {
            await WriteAsync(messageObject);
        }

        /// <summary>
        /// 将消息对象异步写入网络通道。
        /// </summary>
        /// <param name="messageObject">消息对象。</param>
        /// <param name="uniId">唯一ID。</param>
        /// <param name="code">状态码。</param>
        /// <param name="desc">描述。</param>
        public virtual async Task WriteAsync(IMessage messageObject, int uniId = 0, int code = 0, string desc = "")
        {
            messageObject.CheckNotNull(nameof(messageObject));

            var messageData = _messageEncoder.Handler(messageObject);
            if (Setting.IsDebug && Setting.IsDebugSend)
            {
                LogHelper.Debug($"---发送{messageObject.ToFormatMessageString()}");
            }

            if (IsWebSocket)
            {
                await _webSocketSession.SendAsync(messageData);
            }
            else
            {
                await Session.SendAsync(messageData);
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void Close()
        {
            CloseSrc.Cancel();
        }

        /// <summary>
        /// 是否关闭
        /// </summary>
        /// <returns></returns>
        public virtual bool IsClose()
        {
            return CloseSrc.IsCancellationRequested;
        }

        #region Data

        private readonly ConcurrentDictionary<string, object> _userDataKv = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 获取消息自定义数据
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
}