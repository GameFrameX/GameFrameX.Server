using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Hotfix;

namespace GameFrameX.Core.Comps
{
    /// <summary>
    /// 基础组件基类
    /// </summary>
    public abstract class BaseComponent : IComponent
    {
        private          IComponentAgent _cacheAgent     = null;
        private readonly object          _cacheAgentLock = new();

        /// <summary>
        /// 根据组件类型获取对应的IComponentAgent数据
        /// </summary>
        /// <param name="refAssemblyType">引用程序集，如果为null则使用当前程序集引用</param>
        /// <returns></returns>
        public IComponentAgent GetAgent(Type refAssemblyType = null)
        {
            lock (_cacheAgentLock)
            {
                if (_cacheAgent != null && !HotfixManager.DoingHotfix)
                {
                    return _cacheAgent;
                }

                var agent = HotfixManager.GetAgent<IComponentAgent>(this, refAssemblyType);
                _cacheAgent = agent;
                return agent;
            }
        }

        /// <summary>
        /// 清理缓存代理
        /// </summary>
        public void ClearCacheAgent()
        {
            _cacheAgent = null;
        }

        /// <summary>
        /// Actor对象
        /// </summary>
        public IActor Actor { get; set; }

        /// <summary>
        /// ActorId
        /// </summary>
        internal long ActorId
        {
            get { return Actor.Id; }
        }

        /// <summary>
        /// 是否是激活状态
        /// </summary>
        public bool IsActive { get; private set; } = false;

        /// <summary>
        /// 激活组件
        /// </summary>
        /// <returns></returns>
        public virtual Task Active()
        {
            IsActive = true;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 反激活组件
        /// </summary>
        public virtual async Task Inactive()
        {
            var agent = GetAgent();
            if (agent != null)
                await agent.Inactive();
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <returns></returns>
        internal virtual Task SaveState()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 是否准备完毕
        /// </summary>
        /// <returns></returns>
        internal virtual bool ReadyToInactive
        {
            get { return true; }
        }
    }
}