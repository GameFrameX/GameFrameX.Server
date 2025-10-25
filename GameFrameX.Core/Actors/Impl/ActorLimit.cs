// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using System.Collections.Concurrent;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Utility;
using GameFrameX.Foundation.Logger;
using GameFrameX.Utility;

namespace GameFrameX.Core.Actors.Impl;

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
        /// 禁止双向调用
        /// </summary>
        NoBidirectionCall,
    }


    private static IRule _rule;
    private static readonly Dictionary<ushort, int> LevelDic = new(128);

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="type"> 检查规则 </param>
    public static void Init(RuleType type)
    {
        switch (type)
        {
            /*case RuleType.ByLevel:
            {
                _rule = new ByLevelRule();
                try
                {
                    foreach (ActorTypeLevel foo in Enum.GetValues(typeof(ActorTypeLevel)))
                    {
                        var actorType = (ushort)foo;
                        LevelDic.Add(actorType, (int)foo);
                    }
                }
                catch (Exception exception)
                {
                    LogHelper.Fatal(exception);
                    throw;
                }
            }
                break;*/
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

    private interface IRule
    {
        bool AllowCall(long target);
    }


    #region ByLevelRule

    private class ByLevelRule : IRule
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
            var targetType = ActorIdGenerator.GetActorType(target);
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

    private class NoBidirectionCallRule : IRule
    {
        private readonly ConcurrentDictionary<long, ConcurrentDictionary<long, bool>> _crossDic = new();

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
            {
                return true;
            }

            // Actor会在入队成功之后进行设置，这种属于Actor入队
            return AllowCall(actorId, target);
        }

        private bool AllowCall(long self, long target)
        {
            // 自己入自己的队允许，会直接执行
            if (self == target)
            {
                return true;
            }

            if (_crossDic.TryGetValue(target, out var set) && set.ContainsKey(self))
            {
                LogHelper.Error($"发生交叉死锁，ActorId1:{self} ActorType1:{ActorIdGenerator.GetActorType(self)} ActorId2:{target} ActorType2:{ActorIdGenerator.GetActorType(target)}");
                return false;
            }

            var selfSet = _crossDic.GetOrAdd(self, k => new ConcurrentDictionary<long, bool>());
            selfSet.TryAdd(target, false);

            return true;
        }
    }

    #endregion
}