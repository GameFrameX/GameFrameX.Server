using GameFrameX.NetWork;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Core.Net.BaseHandler
{
    /// <summary>
    /// 基础消息处理器
    /// </summary>
    public abstract class BaseMessageHandler : IMessageHandler
    {
        /// <summary>
        /// 网络频道
        /// </summary>
        public INetWorkChannel NetWorkChannel { get; set; }

        /// <summary>
        /// 消息对象
        /// </summary>
        public MessageObject Message { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public virtual Task Init()
        {
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