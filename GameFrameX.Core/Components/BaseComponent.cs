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

using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Hotfix;

namespace GameFrameX.Core.Components;

/// <summary>
/// 基础组件基类
/// </summary>
public abstract class BaseComponent : IComponent, IState
{
    private readonly object _cacheAgentLock = new();
    private IComponentAgent _cacheAgent;

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
    public bool IsActive { get; private set; }

    /// <summary>
    /// 是否准备完毕
    /// </summary>
    /// <returns>是否准备完毕</returns>
    internal virtual bool ReadyToInactive
    {
        get { return true; }
    }

    /// <summary>
    /// Actor 对象
    /// </summary>
    public IActor Actor { get; set; }

    /// <summary>
    /// 根据组件类型获取对应的 IComponentAgent 数据
    /// </summary>
    /// <param name="refAssemblyType">引用程序集，如果为 null 则使用当前程序集引用</param>
    /// <returns>IComponentAgent 实例</returns>
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
    /// 激活组件
    /// </summary>
    /// <returns>激活任务</returns>
    public virtual Task Active()
    {
        if (IsActive)
        {
            return Task.CompletedTask;
        }

        IsActive = true;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 反激活组件
    /// </summary>
    /// <returns>反激活任务</returns>
    public virtual async Task Inactive()
    {
        var agent = GetAgent();
        if (agent != null)
        {
            await agent.BeforeInActivation();
            await agent.Inactive();
            await agent.AfterInActivation();
        }

        IsActive = false;
    }

    /// <summary>
    /// 读取状态
    /// </summary>
    /// <returns>一个表示异步操作的任务，该任务在状态读取完成时完成</returns>
    /// <remarks>
    /// 此方法用于异步读取对象的当前状态信息
    /// </remarks>
    public virtual Task ReadStateAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 更新状态
    /// </summary>
    /// <returns>一个表示异步操作的任务，该任务在状态更新完成时完成</returns>
    /// <remarks>
    /// 此方法用于异步更新对象的状态信息
    /// 在状态发生变化时应调用此方法以保持状态的同步
    /// ⚠️ 注意：这是一个高消耗的操作，不应随意调用。该方法应由系统自动调用，避免手动调用造成性能问题。
    /// </remarks>
    public virtual Task WriteStateAsync()
    {
        return Task.CompletedTask;
    }
}