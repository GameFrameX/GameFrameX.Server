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


﻿// ==========================================================================================
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


using GameFrameX.Foundation.Utility;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.Foundation.Localization.Core;

namespace GameFrameX.NetWork;

/// <summary>
/// RPC会话数据。
/// </summary>
/// <remarks>
/// Represents the data associated with an RPC session, including request/response handling and timeout management.
/// </remarks>
public sealed class RpcSessionData : IRpcSessionData, IDisposable
{
    private readonly TaskCompletionSource<IRpcResult> _tcs;

    /// <summary>
    /// 创建RPC会话数据。
    /// </summary>
    /// <remarks>
    /// Initializes a new RPC session data instance.
    /// </remarks>
    /// <param name="requestMessage">请求消息 / The request message</param>
    /// <param name="isReply">是否需要回复 / Whether a reply is expected</param>
    /// <param name="timeout">超时时间，单位毫秒，默认10秒 / Timeout in milliseconds, defaults to 10 seconds</param>
    private RpcSessionData(IRequestMessage requestMessage, bool isReply = true, int timeout = 10000)
    {
        CreatedTime = TimerHelper.UnixTimeMilliseconds();
        RequestMessage = requestMessage;
        IsReply = isReply;
        Timeout = timeout;
        _tcs = new TaskCompletionSource<IRpcResult>();
    }

    /// <summary>
    /// 获取消息的唯一ID，从RequestMessage中获得。
    /// </summary>
    /// <remarks>
    /// Gets the unique identifier from the request message.
    /// </remarks>
    /// <value>消息的唯一ID / The unique identifier</value>
    public long UniqueId
    {
        get { return RequestMessage.UniqueId; }
    }

    /// <summary>
    /// 获取是否需要回复。
    /// </summary>
    /// <remarks>
    /// Gets whether a reply is expected for this RPC call.
    /// </remarks>
    /// <value>是否需要回复 / Whether a reply is expected</value>
    public bool IsReply { get; }

    /// <summary>
    /// 获取创建时间。
    /// </summary>
    /// <remarks>
    /// Gets the creation timestamp in Unix milliseconds.
    /// </remarks>
    /// <value>创建时间 / The creation timestamp</value>
    public long CreatedTime { get; }

    /// <summary>
    /// 获取或设置计时器消耗的时间。
    /// </summary>
    /// <remarks>
    /// Gets or sets the elapsed time for timeout tracking.
    /// </remarks>
    private long ElapseTime { get; set; }

    /// <summary>
    /// 获取超时时间，单位毫秒。
    /// </summary>
    /// <remarks>
    /// Gets the timeout duration in milliseconds.
    /// </remarks>
    /// <value>超时时间 / The timeout duration</value>
    public int Timeout { get; }

    /// <summary>
    /// 获取请求消息。
    /// </summary>
    /// <remarks>
    /// Gets the request message.
    /// </remarks>
    /// <value>请求消息 / The request message</value>
    public INetworkMessage RequestMessage { get; private set; }

    /// <summary>
    /// 获取响应消息。
    /// </summary>
    /// <remarks>
    /// Gets the response message.
    /// </remarks>
    /// <value>响应消息 / The response message</value>
    public INetworkMessage ResponseMessage { get; private set; }

    /// <summary>
    /// 获取RPC耗时时间，单位毫秒，从创建到回复的时间差。
    /// </summary>
    /// <remarks>
    /// Gets the time elapsed from creation to response in milliseconds.
    /// </remarks>
    /// <value>RPC耗时时间 / The RPC duration</value>
    public long Time { get; private set; }

    /// <summary>
    /// 获取RPC回复任务。
    /// </summary>
    /// <remarks>
    /// Gets the task that will complete when the RPC response is received.
    /// </remarks>
    /// <value>RPC回复任务 / The RPC response task</value>
    public Task<IRpcResult> Task
    {
        get { return _tcs.Task; }
    }

    /// <summary>
    /// 释放资源。
    /// </summary>
    /// <remarks>
    /// Releases all resources used by this instance.
    /// </remarks>
    public void Dispose()
    {
        ElapseTime = 0;
        RequestMessage = null;
        ResponseMessage = null;
        Time = 0;
        _tcs?.TrySetCanceled();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// RPC回复。
    /// </summary>
    /// <remarks>
    /// Completes the RPC call with the specified response message.
    /// </remarks>
    /// <param name="responseMessage">响应消息 / The response message</param>
    /// <returns>是否成功回复 / <c>true</c> if the reply was successful; otherwise <c>false</c></returns>
    public bool Reply(IResponseMessage responseMessage)
    {
        ResponseMessage = responseMessage;
        Time = TimerHelper.UnixTimeMilliseconds() - CreatedTime;
        var result = new RpcResult(responseMessage);
        _tcs.SetResult(result);
        return true;
    }

    /// <summary>
    /// 创建RPC会话数据。
    /// </summary>
    /// <remarks>
    /// Creates a new RPC session data instance.
    /// </remarks>
    /// <param name="requestMessage">请求消息 / The request message</param>
    /// <param name="isReply">是否需要回复 / Whether a reply is expected</param>
    /// <param name="timeout">超时时间，单位毫秒 / Timeout in milliseconds</param>
    /// <returns>RPC会话数据 / The RPC session data instance</returns>
    public static IRpcSessionData Create(IRequestMessage requestMessage, bool isReply = true, int timeout = 10000)
    {
        var rpcData = new RpcSessionData(requestMessage, isReply, timeout);
        return rpcData;
    }

    /// <summary>
    /// 增加时间，如果超时返回true。
    /// </summary>
    /// <remarks>
    /// Increments the elapsed time and checks for timeout.
    /// </remarks>
    /// <param name="millisecondsTime">流逝时间，单位毫秒 / Elapsed time in milliseconds</param>
    /// <returns>如果超时则为 <c>true</c>；否则为 <c>false</c> / <c>true</c> if timed out; otherwise <c>false</c></returns>
    public bool IncrementalElapseTime(long millisecondsTime)
    {
        ElapseTime += millisecondsTime;
        if (ElapseTime >= Timeout)
        {
            var error = LocalizationService.GetString(
                Localization.Keys.NetWork.RpcCallTimeout,
                RequestMessage?.ToString() ?? "null");
            _tcs.TrySetResult(new RpcResult(error));
            return true;
        }

        return false;
    }

    /// <summary>
    /// 析构函数。
    /// </summary>
    /// <remarks>
    /// Finalizer to ensure resources are released.
    /// </remarks>
    ~RpcSessionData()
    {
        Dispose();
    }
}