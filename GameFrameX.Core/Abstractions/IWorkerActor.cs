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
/// 工作Actor接口定义
/// 用于处理异步任务队列的Actor接口，继承自IWorker接口
/// </summary>
public interface IWorkerActor : IWorker
{
    /// <summary>
    /// 判断是否需要入队
    /// 检查当前任务是否需要进入队列进行处理
    /// </summary>
    /// <returns>返回一个元组，包含两个值：needEnqueue(bool类型，表示是否需要入队)和chainId(long类型，表示调用链ID)</returns>
    (bool needEnqueue, long chainId) IsNeedEnqueue();

    /// <summary>
    /// 将无返回值的委托入队
    /// 将Action类型的委托添加到任务队列中进行异步处理
    /// </summary>
    /// <param name="work">要执行的无返回值委托方法</param>
    /// <param name="callChainId">用于跟踪调用链的唯一标识符</param>
    /// <param name="discard">当为true时，表示强制将任务入队，即使队列已满也会尝试入队</param>
    /// <param name="timeOut">任务执行的超时时间，默认为int.MaxValue</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    /// <returns>表示异步操作的Task对象</returns>
    Task Enqueue(Action work, long callChainId, bool discard = false, int timeOut = int.MaxValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// 将有返回值的委托入队
    /// 将Func<T/>类型的委托添加到任务队列中进行异步处理
    /// </summary>
    /// <param name="work">要执行的有返回值委托方法</param>
    /// <param name="callChainId">用于跟踪调用链的唯一标识符</param>
    /// <param name="discard">当为true时，表示强制将任务入队，即使队列已满也会尝试入队</param>
    /// <param name="timeOut">任务执行的超时时间，默认为int.MaxValue</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    /// <typeparam name="T">委托返回值的类型</typeparam>
    /// <returns>表示异步操作的Task<T/>对象，其结果为委托的返回值</returns>
    Task<T> Enqueue<T>(Func<T> work, long callChainId, bool discard = false, int timeOut = int.MaxValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// 将返回Task的委托入队
    /// 将Func<Task/>类型的委托添加到任务队列中进行异步处理
    /// </summary>
    /// <param name="work">要执行的返回Task的异步委托方法</param>
    /// <param name="callChainId">用于跟踪调用链的唯一标识符</param>
    /// <param name="discard">当为true时，表示强制将任务入队，即使队列已满也会尝试入队</param>
    /// <param name="timeOut">任务执行的超时时间，默认为int.MaxValue</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    /// <returns>表示异步操作的Task对象</returns>
    Task Enqueue(Func<Task> work, long callChainId, bool discard = false, int timeOut = int.MaxValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// 将返回Task<T/>的委托入队
    /// 将Func<Task/><T/>>类型的委托添加到任务队列中进行异步处理
    /// </summary>
    /// <param name="work">要执行的返回Task<T/>的异步委托方法</param>
    /// <param name="callChainId">用于跟踪调用链的唯一标识符</param>
    /// <param name="discard">当为true时，表示强制将任务入队，即使队列已满也会尝试入队</param>
    /// <param name="timeOut">任务执行的超时时间，默认为int.MaxValue</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    /// <typeparam name="T">异步操作返回值的类型</typeparam>
    /// <returns>表示异步操作的Task<T/>对象，其结果为委托的返回值</returns>
    Task<T> Enqueue<T>(Func<Task<T>> work, long callChainId, bool discard = false, int timeOut = int.MaxValue, CancellationToken cancellationToken = default);
}