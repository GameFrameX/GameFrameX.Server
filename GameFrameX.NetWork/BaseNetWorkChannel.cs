using System.Collections.Concurrent;
using GameFrameX.NetWork.Messages;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.WebSocket.Server;
using GameFrameX.Utility;

namespace GameFrameX.NetWork
{
    public class BaseNetWorkChannel : INetWorkChannel
    {
        protected readonly CancellationTokenSource CloseSrc = new CancellationTokenSource();

        // public long NetId { get; set; } = 0;
        // public int TargetServerId { get; set; }
        public IGameAppSession AppSession { get; }
        public IRpcSession RpcSession { get; }

        private readonly IMessageEncoderHandler messageEncoder;
        public bool IsWebSocket { get; }

        public BaseNetWorkChannel(IGameAppSession session, IMessageEncoderHandler messageEncoder, IRpcSession rpcSession, bool isWebSocket)
        {
            AppSession = session;
            IsWebSocket = isWebSocket;
            this.messageEncoder = messageEncoder;
            RpcSession = rpcSession;
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
            Guard.NotNull(messageObject, nameof(messageObject));

            var messageData = messageEncoder.Handler(messageObject);

            if (IsWebSocket)
            {
                var webSocketSession = (WebSocketSession)AppSession;
                await webSocketSession.SendAsync(messageData);
            }
            else
            {
                await AppSession.SendAsync(messageData);
            }
        }

        public virtual void Close()
        {
            CloseSrc.Cancel();
        }

        public virtual bool IsClose()
        {
            return CloseSrc.IsCancellationRequested;
        }

        #region Data

        private readonly ConcurrentDictionary<string, object> userDataKv = new ConcurrentDictionary<string, object>();

        public T GetData<T>(string key)
        {
            if (userDataKv.TryGetValue(key, out var v))
            {
                return (T)v;
            }

            return default;
        }

        public void RemoveData(string key)
        {
            userDataKv.Remove(key, out _);
        }

        public void SetData(string key, object value)
        {
            userDataKv[key] = value;
        }

        #endregion

        #region MessageTime

        private long lastReceiveMessageTime;

        /// <summary>
        /// 更新接收消息的时间
        /// </summary>
        /// <param name="offsetTicks"></param>
        public void UpdateReceiveMessageTime(long offsetTicks = 0)
        {
            lastReceiveMessageTime = DateTime.UtcNow.Ticks + offsetTicks;
        }

        /// <summary>
        /// 获取最后接收消息到现在的时间。单位秒
        /// </summary>
        /// <param name="utcTime"></param>
        /// <returns></returns>
        public long GetLastMessageTimeSecond(in DateTime utcTime)
        {
            return (utcTime.Ticks - lastReceiveMessageTime) / 10000_000;
        }

        #endregion
    }
}