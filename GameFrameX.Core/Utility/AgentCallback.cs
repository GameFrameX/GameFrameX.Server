using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Utility;

namespace GameFrameX.Core.Utility
{
    /// <summary>
    /// 代理调用回调
    /// </summary>
    public interface IAgentCallback
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="agent">组件代理</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        Task<bool> Invoke(IComponentAgent agent, Param param = null);

        /// <summary>
        /// 组件代理类型
        /// </summary>
        /// <returns></returns>
        Type CompAgentType();
    }

    /// <summary>
    /// 代理调用回调
    /// </summary>
    /// <typeparam name="TAgent"></typeparam>
    public abstract class AgentCallback<TAgent> : IAgentCallback where TAgent : IComponentAgent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Type CompAgentType()
        {
            return typeof(TAgent);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<bool> Invoke(IComponentAgent agent, Param param = null)
        {
            return OnCall((TAgent)agent, param);
        }

        /// <summary>
        /// 回调
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected abstract Task<bool> OnCall(TAgent comp, Param param);
    }
}