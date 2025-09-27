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
using GameFrameX.Core.Abstractions.Events;

namespace GameFrameX.Core.Events;

/// <summary>
/// 事件监听器基类，用于处理特定组件代理类型的事件
/// </summary>
/// <typeparam name="T">组件代理类型，必须实现IComponentAgent接口</typeparam>
public abstract class EventListener<T> : IEventListener where T : class, IComponentAgent
{
    /// <summary>
    /// 实现IEventListener接口的事件处理函数
    /// </summary>
    /// <param name="agent">组件代理实例，类型为IComponentAgent</param>
    /// <param name="gameEventArgs">需要处理的事件对象</param>
    /// <returns>异步任务，表示事件处理的完成状态</returns>
    /// <remarks>
    /// 该方法会将传入的IComponentAgent类型转换为泛型类型T后调用保护方法HandleEvent
    /// </remarks>
    public Task HandleEvent(IComponentAgent agent, GameEventArgs gameEventArgs)
    {
        return HandleEvent(agent as T, gameEventArgs);
    }

    /// <summary>
    /// 无代理对象的事件处理函数重载
    /// </summary>
    /// <param name="gameEventArgs">需要处理的事件对象</param>
    /// <returns>异步任务，表示事件处理的完成状态</returns>
    /// <remarks>
    /// 该方法会以null作为agent参数调用另一个HandleEvent重载
    /// </remarks>
    public Task HandleEvent(GameEventArgs gameEventArgs)
    {
        return HandleEvent(null, gameEventArgs);
    }

    /// <summary>
    /// 获取事件代理类型属性
    /// </summary>
    /// <remarks>
    /// 返回泛型参数T的实际类型，用于运行时类型识别
    /// </remarks>
    public Type AgentType { get; } = typeof(T);

    /// <summary>
    /// 具体的事件处理实现方法
    /// </summary>
    /// <param name="agent">泛型类型的组件代理实例</param>
    /// <param name="gameEventArgs">需要处理的事件对象</param>
    /// <returns>异步任务，表示事件处理的完成状态</returns>
    /// <remarks>
    /// 派生类必须实现该抽象方法来提供具体的事件处理逻辑
    /// </remarks>
    protected abstract Task HandleEvent(T agent, GameEventArgs gameEventArgs);
}