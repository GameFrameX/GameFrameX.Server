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

using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.BaseHandler.Normal;
using GameFrameX.Core.Components;
using GameFrameX.Core.Utility;
using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.Core.BaseHandler;

/// <summary>
/// 全局组件处理器
/// </summary>
/// <typeparam name="TRequest">请求消息类型，必须实现 <see cref="IRequestMessage"/> 接口，且具备无参构造函数</typeparam>
/// <remarks>
/// 所有全局组件处理器均应继承自此抽象类，以实现统一的组件初始化、超时监控、性能统计与异常处理逻辑。
/// </remarks>
public abstract class GlobalComponentHandler<TRequest> : BaseComponentHandler<TRequest> where TRequest : class, IRequestMessage, new()
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
    protected override Task<bool> InitActor()
    {
        if (ActorId <= 0)
        {
            var compType = ComponentAgentType.BaseType.GetGenericArguments()[0];
            var actorType = ComponentRegister.GetActorType(compType);
            ActorId = ActorIdGenerator.GetActorId(actorType);
        }

        return Task.FromResult(true);
    }
}

/// <summary>
/// 全局组件处理器（泛型双参版）
/// </summary>
/// <typeparam name="T">组件代理类型，必须实现 <see cref="IComponentAgent"/> 接口</typeparam>
/// <typeparam name="TRequest">请求消息类型，必须实现 <see cref="IRequestMessage"/> 接口，且具备无参构造函数</typeparam>
/// <remarks>
/// 所有全局组件处理器均应继承自此抽象类，以实现统一的组件初始化、超时监控、性能统计与异常处理逻辑。
/// 本泛型版本在单参版的基础上，额外提供强类型的组件代理访问能力，方便业务层直接操作具体组件代理而无需强制转换。
/// </remarks>
public abstract class GlobalComponentHandler<T, TRequest> : GlobalComponentHandler<TRequest> where T : IComponentAgent where TRequest : class, IRequestMessage, new()
{
    /// <summary>
    /// 组件代理类型
    /// </summary>
    protected override Type ComponentAgentType
    {
        get { return typeof(T); }
    }

    /// <summary>
    /// 缓存组件代理对象
    /// </summary>
    protected T ComponentAgent
    {
        get { return (T)CacheComponent; }
    }
}