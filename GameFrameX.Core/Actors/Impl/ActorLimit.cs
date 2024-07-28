using System.Collections.Concurrent;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Utility;
using GameFrameX.Log;

namespace GameFrameX.Core.Actors.Impl
{
    /// <summary>
    /// 判断Actor交叉死锁
    /// </summary>
    public static class ActorLimit
    {
        /// <summary>
        /// 可以按需扩展检查规则
        /// </summary>
        public enum RuleType
        {
            /// <summary>
            /// 不检查
            /// </summary>
            None,

            /// <summary>
            /// 分等级(高等级不能【等待】调用低等级)
            /// </summary>
            ByLevel,

            /// <summary>
            /// 禁止双向调用
            /// </summary>
            NoBidirectionCall
        }

        interface IRule
        {
            bool AllowCall(long target);
        }


        private static          IRule                      _rule;
        private static readonly Dictionary<ActorType, int> LevelDic = new Dictionary<ActorType, int>(128);

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="type"> 检查规则 </param>
        public static void Init(RuleType type)
        {
            switch (type)
            {
                case RuleType.ByLevel:
                {
                    _rule = new ByLevelRule();
                    try
                    {
                        foreach (ActorTypeLevel foo in Enum.GetValues(typeof(ActorTypeLevel)))
                        {
                            ActorType actorType = (ActorType)Enum.Parse(typeof(ActorType), foo.ToString());
                            LevelDic.Add(actorType, (int)foo);
                        }
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Fatal(exception);
                        throw;
                    }
                }
                    break;
                case RuleType.NoBidirectionCall:
                {
                    _rule = new NoBidirectionCallRule();
                }
                    break;
                case RuleType.None:
                    break;
                default:
                    LogHelper.Error($"不支持的rule类型:{type}");
                    break;
            }
        }

        /// <summary>
        /// 是否允许调用
        /// </summary>
        /// <param name="target">目标</param>
        /// <returns>返回是否调用</returns>
        public static bool AllowCall(long target)
        {
            if (_rule != null)
            {
                return _rule.AllowCall(target);
            }

            return true;
        }


        #region ByLevelRule

        class ByLevelRule : IRule
        {
            /// <summary>
            /// 判断是否允许调用
            /// </summary>
            /// <param name="target">目标</param>
            /// <returns></returns>
            bool IRule.AllowCall(long target)
            {
                var actorId = RuntimeContext.CurrentActor;
                // 从其他线程抛到actor，不涉及入队行为
                if (actorId == 0)
                {
                    return true;
                }

                var currentType = ActorIdGenerator.GetActorType(actorId);
                var targetType  = ActorIdGenerator.GetActorType(target);
                if (LevelDic.TryGetValue(targetType, out var targetValue) && LevelDic.TryGetValue(currentType, out var currentValue))
                {
                    //等级高的不能【等待】调用等级低的
                    if (currentValue > targetValue)
                    {
                        LogHelper.Error($"不合法的调用路径:{currentType}==>{targetType}");
                        return false;
                    }
                }

                return true;
            }
        }

        #endregion


        #region NoBidirectionCallRule

        class NoBidirectionCallRule : IRule
        {
            internal readonly ConcurrentDictionary<long, ConcurrentDictionary<long, bool>> CrossDic = new();

            private bool AllowCall(long self, long target)
            {
                // 自己入自己的队允许，会直接执行
                if (self == target)
                {
                    return true;
                }

                if (CrossDic.TryGetValue(target, out var set) && set.ContainsKey(self))
                {
                    LogHelper.Error($"发生交叉死锁，ActorId1:{self} ActorType1:{ActorIdGenerator.GetActorType(self)} ActorId2:{target} ActorType2:{ActorIdGenerator.GetActorType(target)}");
                    return false;
                }

                var selfSet = CrossDic.GetOrAdd(self, k => new ConcurrentDictionary<long, bool>());
                selfSet.TryAdd(target, false);

                return true;
            }

            /// <summary>
            /// 是否允许调用
            /// </summary>
            /// <param name="target">目标</param>
            /// <returns>返回是否调用</returns>
            public bool AllowCall(long target)
            {
                var actorId = RuntimeContext.CurrentActor;
                // 从IO线程抛到actor，不涉及入队行为
                if (actorId == 0)
                    return true;
                // Actor会在入队成功之后进行设置，这种属于Actor入队
                return AllowCall(actorId, target);
            }
        }

        #endregion
    }
}