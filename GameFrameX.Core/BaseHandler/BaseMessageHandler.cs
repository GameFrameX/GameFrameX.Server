using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Core.BaseHandler
{
    /// <summary>
    /// 基础消息处理器
    /// </summary>
    public abstract class BaseMessageHandler : IMessageHandler
    {
        /// <summary>
        /// 网络频道
        /// </summary>
        public INetWorkChannel NetWorkChannel { get; private set; }

        /// <summary>
        /// 消息对象
        /// </summary>
        public INetworkMessage Message { get; private set; }

        /// <summary>
        /// 初始化
        /// 子类实现必须调用
        /// </summary>
        /// <param name="message">消息对象</param>
        /// <param name="netWorkChannel">网络渠道</param>
        /// <returns></returns>
        public virtual Task Init(INetworkMessage message, INetWorkChannel netWorkChannel)
        {
            Message = message;
            NetWorkChannel = netWorkChannel;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 动作异步
        /// </summary>
        /// <returns></returns>
        protected abstract Task ActionAsync();

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public virtual Task InnerAction()
        {
            return ActionAsync();
        }
    }
}