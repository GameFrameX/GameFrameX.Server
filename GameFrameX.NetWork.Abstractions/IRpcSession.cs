// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// RPC会话接口。
/// </summary>
/// <remarks>
/// RPC session interface for managing remote procedure calls.
/// </remarks>
public interface IRpcSession
{
    /// <summary>
    /// 异步调用，且等待返回。
    /// </summary>
    /// <remarks>
    /// Asynchronously calls and waits for the response.
    /// </remarks>
    /// <typeparam name="T">返回消息对象类型 / Response message type</typeparam>
    /// <param name="message">调用消息对象 / Request message object</param>
    /// <param name="timeOutMillisecond">调用超时，单位毫秒，默认10秒 / Call timeout in milliseconds, default is 10 seconds</param>
    /// <returns>返回消息对象 / Response message object</returns>
    Task<IRpcResult> Call<T>(IRequestMessage message, int timeOutMillisecond = 10000) where T : IResponseMessage, new();

    /// <summary>
    /// 异步发送，不等待结果。
    /// </summary>
    /// <remarks>
    /// Asynchronously sends without waiting for the result.
    /// </remarks>
    /// <param name="message">调用消息对象 / Request message object</param>
    void Send(IRequestMessage message);

    /// <summary>
    /// 处理消息队列。
    /// </summary>
    /// <remarks>
    /// Processes the message queue.
    /// </remarks>
    /// <returns>处理结果 / Processing result</returns>
    IRpcSessionData Handler();

    /// <summary>
    /// 回复消息。
    /// </summary>
    /// <remarks>
    /// Replies with a message.
    /// </remarks>
    /// <param name="message">回复消息对象 / Response message object</param>
    /// <returns>是否成功回复 / <c>true</c> if reply is successful; otherwise <c>false</c></returns>
    bool Reply(IResponseMessage message);

    /// <summary>
    /// 处理消息队列。
    /// </summary>
    /// <remarks>
    /// Processes the message queue with elapsed time.
    /// </remarks>
    /// <param name="elapseMillisecondsTime">时间间隔，单位毫秒 / Time interval in milliseconds</param>
    void Tick(int elapseMillisecondsTime);

    /// <summary>
    /// 停止会话。
    /// </summary>
    /// <remarks>
    /// Stops the session.
    /// </remarks>
    void Stop();
}