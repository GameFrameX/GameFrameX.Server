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
/// RPC会话数据接口。
/// </summary>
/// <remarks>
/// RPC session data interface for managing session state.
/// </remarks>
public interface IRpcSessionData
{
    /// <summary>
    /// 获取任务。
    /// </summary>
    /// <remarks>
    /// Gets the task for the RPC call.
    /// </remarks>
    /// <value>任务 / The task</value>
    Task<IRpcResult> Task { get; }

    /// <summary>
    /// 获取是否回复消息。
    /// </summary>
    /// <remarks>
    /// Gets whether the message has been replied.
    /// </remarks>
    /// <value>是否回复消息 / Whether the message has been replied</value>
    bool IsReply { get; }

    /// <summary>
    /// 获取唯一ID，用于标识RPC会话。
    /// </summary>
    /// <remarks>
    /// Gets the unique identifier for the RPC session.
    /// </remarks>
    /// <value>唯一ID / Unique identifier</value>
    long UniqueId { get; }

    /// <summary>
    /// 获取请求消息。
    /// </summary>
    /// <remarks>
    /// Gets the request message.
    /// </remarks>
    /// <value>请求消息 / Request message</value>
    INetworkMessage RequestMessage { get; }

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
    /// 增加时间。如果超时返回 <c>true</c>。
    /// </summary>
    /// <remarks>
    /// Increments the elapsed time. Returns <c>true</c> if timeout occurs.
    /// </remarks>
    /// <param name="millisecondsTime">流逝时间，单位毫秒 / Elapsed time in milliseconds</param>
    /// <returns>是否超时 / <c>true</c> if timeout; otherwise <c>false</c></returns>
    bool IncrementalElapseTime(long millisecondsTime);
}