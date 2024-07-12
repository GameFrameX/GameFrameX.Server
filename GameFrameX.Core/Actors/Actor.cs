using System.Collections.Concurrent;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Timer;
using GameFrameX.Core.Actors.Impl;
using GameFrameX.Core.Comps;
using GameFrameX.Log;

namespace GameFrameX.Core.Actors
{
    /// <summary>
    /// Actor
    /// </summary>
    public sealed class Actor : IActor
    {
        /// <summary>
        /// 
        /// </summary>
        public Actor()
        {
            ScheduleIdSet = new HashSet<long>();
        }

        private readonly ConcurrentDictionary<Type, BaseComponent> _componentsMap = new ConcurrentDictionary<Type, BaseComponent>();

        /// <summary>
        /// IActor唯一标识
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 订阅哈希列表
        /// </summary>
        public HashSet<long> ScheduleIdSet { get; }

        /// <summary>
        /// Actor类型
        /// </summary>
        public ActorType Type { get; set; }

        /// <summary>
        /// 工作Actor
        /// </summary>
        public IWorkerActor WorkerActor { get; init; }

        /// <summary>
        /// 是否自动回收
        /// </summary>
        public bool AutoRecycle { get; private set; } = false;


        /// <summary>
        /// 设置自动回收标记
        /// </summary>
        /// <param name="autoRecycle">是否自动回收</param>
        public void SetAutoRecycle(bool autoRecycle)
        {
            Tell(() => { AutoRecycle = autoRecycle; });
        }

        /// <summary>
        /// 根据组件类型获取对应的IComponentAgent
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns></returns>
        public async Task<T> GetComponentAgent<T>() where T : IComponentAgent
        {
            return (T)await GetComponentAgent(typeof(T));
        }

        /// <summary>
        /// 根据组件类型获取对应的IComponentAgent
        /// </summary>
        /// <param name="agentType">代理类型</param>
        /// <returns></returns>
        public async Task<IComponentAgent> GetComponentAgent(Type agentType)
        {
            var compType = agentType.BaseType.GetGenericArguments()[0];
            var comp     = _componentsMap.GetOrAdd(compType, GetOrAddFactory);
            var agent    = comp.GetAgent(agentType);
            if (!comp.IsActive)
            {
                async Task Worker()
                {
                    await comp.Active();
                    agent.Active();
                }

                await SendAsyncWithoutCheck(Worker);
            }

            return agent;
        }

        /// <summary>
        /// 获取或添加组件类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private BaseComponent GetOrAddFactory(Type type)
        {
            return ComponentRegister.NewComp(this, type);
        }

        /// <summary>
        /// 默认超时时长
        /// </summary>
        public const int TIME_OUT = int.MaxValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public Actor(long id, ActorType type)
        {
            Id          = id;
            Type        = type;
            WorkerActor = new WorkerActor(id);

            if (type == ActorType.Player)
            {
                Tell(() => SetAutoRecycle(true));
            }
            else
            {
                Tell(() => ComponentRegister.ActiveComps(this));
            }
        }

        /// <summary>
        /// 跨天
        /// </summary>
        /// <param name="openServerDay">开服天数</param>
        public async Task CrossDay(int openServerDay)
        {
            LogHelper.Debug($"actor跨天 id:{Id} type:{Type}");
            foreach (var comp in _componentsMap.Values)
            {
                var agent = comp.GetAgent();
                if (agent is ICrossDay crossDay)
                {
                    // 使用try-catch缩小异常影响范围
                    try
                    {
                        await crossDay.OnCrossDay(openServerDay);
                    }
                    catch (Exception e)
                    {
                        LogHelper.Error($"{agent.GetType().FullName}跨天失败 actorId:{Id} actorType:{Type} 异常：\n{e}");
                    }
                }
            }
        }

        internal bool ReadyToDeActive
        {
            get { return _componentsMap.Values.All(item => item.ReadyToInactive); }
        }

        /// <summary>
        /// 保存全部数据
        /// </summary>
        internal async Task SaveAllState()
        {
            foreach (var item in _componentsMap)
            {
                await item.Value.SaveState();
            }
        }

        /// <summary>
        /// 反激活所有组件
        /// </summary>
        public async Task Inactive()
        {
            foreach (var item in _componentsMap.Values)
            {
                await item.Inactive();
            }
        }

        #region actor 入队

        /// <summary>
        /// 发送无返回值的工作指令
        /// </summary>
        /// <param name="work">工作内容</param>
        /// <param name="timeOut">超时,默认为int.MaxValue</param>
        public void Tell(Action work, int timeOut = TIME_OUT)
        {
            WorkerActor.Tell(work, timeOut);
        }

        /// <summary>
        /// 发送有返回值的工作指令
        /// </summary>
        /// <param name="work">工作内容</param>
        /// <param name="timeOut">超时,默认为int.MaxValue</param>
        public void Tell(Func<Task> work, int timeOut = TIME_OUT)
        {
            WorkerActor.Tell(work, timeOut);
        }

        /// <summary>
        /// 发送无返回值工作指令,超时,默认为int.MaxValue
        /// </summary>
        /// <param name="work">工作内容</param>
        /// <returns></returns>
        public Task SendAsync(Action work)
        {
            return WorkerActor.SendAsync(work, int.MaxValue);
        }

        /// <summary>
        /// 发送异步消息
        /// </summary>
        /// <param name="work"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task SendAsync(Action work, int timeout)
        {
            return WorkerActor.SendAsync(work, timeout);
        }

        /// <summary>
        /// 发送异步消息
        /// </summary>
        /// <param name="work"></param>
        /// <param name="timeout"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<T> SendAsync<T>(Func<T> work, int timeout = TIME_OUT)
        {
            return WorkerActor.SendAsync(work, timeout);
        }


        /// <summary>
        /// 发送异步消息
        /// </summary>
        /// <param name="work"></param>
        /// <param name="timeout"></param>
        /// <param name="checkLock">是否检查锁</param>
        /// <returns></returns>
        public Task SendAsync(Func<Task> work, int timeout = TIME_OUT, bool checkLock = true)
        {
            return WorkerActor.SendAsync(work, timeout, checkLock);
        }

        /// <summary>
        /// 发送异步消息
        /// </summary>
        /// <param name="work"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task SendAsyncWithoutCheck(Func<Task> work, int timeout = TIME_OUT)
        {
            return WorkerActor.SendAsync(work, timeout, false);
        }

        /// <summary>
        /// 发送异步消息
        /// </summary>
        /// <param name="work">工作对象</param>
        /// <param name="timeout">超时时间</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<T> SendAsync<T>(Func<Task<T>> work, int timeout = TIME_OUT)
        {
            return WorkerActor.SendAsync(work, timeout);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{base.ToString()}_{Type}_{Id}";
        }

        /// <summary>
        /// 清理全部代理
        /// </summary>
        public void ClearAgent()
        {
            foreach (var comp in _componentsMap.Values)
            {
                comp.ClearCacheAgent();
            }
        }
    }
}