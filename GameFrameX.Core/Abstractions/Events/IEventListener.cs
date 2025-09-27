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

namespace GameFrameX.Core.Abstractions.Events;

/// <summary>
/// 事件监听器接口
/// 每个实例其实都是单例的，用于处理特定类型的事件
/// 实现此接口的类需要保证线程安全
/// </summary>
public interface IEventListener
{
    /// <summary>
    /// 获取事件代理类型
    /// 用于标识此监听器可以处理哪种类型的事件代理
    /// </summary>
    /// <returns>事件代理的类型</returns>
    Type AgentType { get; }

    /// <summary>
    /// 事件处理函数
    /// 处理带有组件代理的事件，可以访问和操作组件的状态
    /// </summary>
    /// <param name="agent">组件代理，提供对组件的访问能力</param>
    /// <param name="gameEventArgs">需要处理的事件对象</param>
    /// <returns>一个表示异步操作的任务，当事件处理完成时完成</returns>
    Task HandleEvent(IComponentAgent agent, GameEventArgs gameEventArgs);

    /// <summary>
    /// 事件处理函数
    /// 处理不需要组件代理的独立事件
    /// </summary>
    /// <param name="gameEventArgs">需要处理的事件对象</param>
    /// <returns>一个表示异步操作的任务，当事件处理完成时完成</returns>
    Task HandleEvent(GameEventArgs gameEventArgs);
}