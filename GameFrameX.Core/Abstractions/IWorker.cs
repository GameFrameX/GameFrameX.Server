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

namespace GameFrameX.Core.Abstractions;

/// <summary>
/// 工作者接口定义
/// 用于处理异步和同步的工作任务，支持超时和取消操作
/// </summary>
public interface IWorker
{
    /// <summary>
    /// 发送无返回值的工作指令
    /// 同步执行指定的工作内容
    /// </summary>
    /// <param name="work">需要执行的工作内容委托</param>
    /// <param name="timeOut">执行超时时间（毫秒）,默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    void Tell(Action work, int timeOut = -1, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送有返回值的工作指令
    /// 同步执行指定的异步工作内容
    /// </summary>
    /// <param name="work">需要执行的异步工作内容委托</param>
    /// <param name="timeOut">执行超时时间（毫秒）,默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    void Tell(Func<Task> work, int timeOut = -1, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步发送无返回值的工作指令
    /// 异步执行指定的工作内容
    /// </summary>
    /// <param name="work">需要执行的工作内容委托</param>
    /// <param name="timeOut">执行超时时间（毫秒）,默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    /// <returns>表示异步操作的任务对象</returns>
    Task SendAsync(Action work, int timeOut = -1, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步发送有返回值的工作指令
    /// 异步执行指定的同步工作内容并返回结果
    /// </summary>
    /// <param name="work">需要执行的工作内容委托</param>
    /// <param name="timeOut">执行超时时间（毫秒）,默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    /// <typeparam name="T">返回结果的类型</typeparam>
    /// <returns>包含执行结果的异步任务对象</returns>
    Task<T> SendAsync<T>(Func<T> work, int timeOut = -1, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步发送有返回值的工作指令
    /// 异步执行指定的异步工作内容
    /// </summary>
    /// <param name="work">需要执行的异步工作内容委托</param>
    /// <param name="timeOut">执行超时时间（毫秒）,默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="checkLock">是否在执行前检查锁定状态</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    /// <returns>表示嵌套异步操作的任务对象</returns>
    Task SendAsync(Func<Task> work, int timeOut = -1, bool checkLock = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步发送有返回值的工作指令
    /// 异步执行指定的异步工作内容并返回结果
    /// </summary>
    /// <param name="work">需要执行的异步工作内容委托</param>
    /// <param name="timeOut">执行超时时间（毫秒）,默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    /// <typeparam name="T">返回结果的类型</typeparam>
    /// <returns>包含执行结果的异步任务对象</returns>
    Task<T> SendAsync<T>(Func<Task<T>> work, int timeOut = -1, CancellationToken cancellationToken = default);
}