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

namespace GameFrameX.Core.Timer.Handler;

/// <summary>
/// 热更新程序集的计时器处理器
/// </summary>
/// <typeparam name="TAgent">组件类型，必须实现 <see cref="IComponentAgent" /> 接口</typeparam>
public abstract class TimerHandler<TAgent> : ITimerHandler where TAgent : IComponentAgent
{
    /// <summary>
    /// 内部计时器处理器调用。由 Quartz 调用
    /// </summary>
    /// <param name="agent">组件代理对象，用于与系统其他部分交互</param>
    /// <param name="gameEventArgs">传递给处理器的参数</param>
    /// <returns>一个任务，表示异步操作的结果</returns>
    public Task InnerHandleTimer(IComponentAgent agent, GameEventArgs gameEventArgs)
    {
        return HandleTimer((TAgent)agent, gameEventArgs);
    }

    /// <summary>
    /// 计时器调用
    /// </summary>
    /// <param name="agent">调用对象，具体类型的组件代理对象</param>
    /// <param name="gameEventArgs">参数对象，传递给处理器的参数</param>
    /// <returns>一个任务，表示异步操作的结果</returns>
    protected abstract Task HandleTimer(TAgent agent, GameEventArgs gameEventArgs);
}