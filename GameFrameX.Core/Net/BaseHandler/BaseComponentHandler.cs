using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Actors;
using GameFrameX.Core.Hotfix.Agent;

namespace GameFrameX.Core.Net.BaseHandler
{
    /// <summary>
    /// 基础组件处理器基类
    /// </summary>
    public abstract class BaseComponentHandler : BaseMessageHandler
    {
        /// <summary>
        /// 组件代理ID
        /// </summary>
        protected long ActorId { get; set; }

        /// <summary>
        /// 组件代理类型
        /// </summary>
        protected abstract Type ComponentAgentType { get; }

        /// <summary>
        /// 缓存组件代理对象
        /// </summary>
        public IComponentAgent CacheComponent { get; protected set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        protected abstract Task InitActor();

        /// <summary>
        /// 初始化
        /// </summary>
        public override async Task Init()
        {
            await InitActor();
            if (CacheComponent == null)
            {
                CacheComponent = await ActorManager.GetComponentAgent(ActorId, ComponentAgentType);
                // LogHelper.Info(CacheComp);
            }
        }

        /// <summary>
        /// 内部执行
        /// </summary>
        /// <returns></returns>
        public override Task InnerAction()
        {
            CacheComponent.Tell(ActionAsync);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 根据组件类型获取对应的IComponentAgent
        /// </summary>
        /// <typeparam name="TOtherAgent"></typeparam>
        /// <returns></returns>
        protected Task<TOtherAgent> GetComponentAgent<TOtherAgent>() where TOtherAgent : IComponentAgent
        {
            return CacheComponent.GetComponentAgent<TOtherAgent>();
        }
    }
}