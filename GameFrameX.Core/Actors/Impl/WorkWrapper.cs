using GameFrameX.Core.Abstractions;

namespace GameFrameX.Core.Actors.Impl
{
    /// <summary>
    /// 工作包装器
    /// </summary>
    public abstract class WorkWrapper
    {
        /// <summary>
        /// 工作对象
        /// </summary>
        public WorkerActor Owner { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public abstract Task DoTask();

        /// <summary>
        /// 获取调用链
        /// </summary>
        /// <returns></returns>
        public abstract string GetTrace();

        /// <summary>
        /// 强制设置结果
        /// </summary>
        public abstract void ForceSetResult();

        /// <summary>
        /// 调用链ID
        /// </summary>
        public long CallChainId { get; set; }

        /// <summary>
        /// 设置上下文
        /// </summary>
        protected void SetContext()
        {
            RuntimeContext.SetContext(CallChainId, Owner.Id);
            Owner.CurrentChainId = CallChainId;
        }

        /// <summary>
        /// 重置上下文
        /// </summary>
        public void ResetContext()
        {
            Owner.CurrentChainId = 0;
        }
    }
}