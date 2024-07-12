using GameFrameX.Core.Actors;

namespace GameFrameX.Core.Abstractions.Agent
{
    /// <summary>
    /// 组件代理接口
    /// </summary>
    public interface IComponentAgent : IWorker
    {
        /// <summary>
        /// ActorId
        /// </summary>
        long ActorId { get; }

        /// <summary>
        /// 拥有者
        /// </summary>
        IComponent Owner { get; set; }

        /// <summary>
        /// 激活
        /// </summary>
        void Active();

        /// <summary>
        /// 反激活
        /// </summary>
        /// <returns></returns>
        Task Inactive();

        /// <summary>
        /// 拥有者类型
        /// </summary>
        ActorType OwnerType { get; }

        /// <summary>
        /// 根据代理类型获取代理组件
        /// </summary>
        /// <param name="agentType">代理类型</param>
        /// <returns></returns>
        public Task<IComponentAgent> GetComponentAgent(Type agentType);

        /// <summary>
        /// 根据代理类型获取代理组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<T> GetComponentAgent<T>() where T : IComponentAgent;
    }
}