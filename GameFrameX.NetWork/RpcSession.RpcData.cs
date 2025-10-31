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


using GameFrameX.Foundation.Utility;
using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.NetWork;

/// <summary>
/// RPC 数据
/// </summary>
public sealed class RpcSessionData : IRpcSessionData, IDisposable
{
    private readonly TaskCompletionSource<IRpcResult> _tcs;

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="requestMessage">请求消息</param>
    /// <param name="isReply">是否需要回复</param>
    /// <param name="timeout">超时时间,单位毫秒,默认10秒</param>
    private RpcSessionData(IRequestMessage requestMessage, bool isReply = true, int timeout = 10000)
    {
        CreatedTime = TimerHelper.UnixTimeMilliseconds();
        RequestMessage = requestMessage;
        IsReply = isReply;
        Timeout = timeout;
        _tcs = new TaskCompletionSource<IRpcResult>();
    }

    /// <summary>
    /// 消息的唯一ID
    /// 从RequestMessage中获得
    /// </summary>
    public long UniqueId
    {
        get { return RequestMessage.UniqueId; }
    }

    /// <summary>
    /// 是否需要回复
    /// </summary>
    public bool IsReply { get; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public long CreatedTime { get; }

    /// <summary>
    /// 计时器消耗的时间
    /// </summary>
    private long ElapseTime { get; set; }

    /// <summary>
    /// 超时时间。单位毫秒
    /// </summary>
    public int Timeout { get; }

    /// <summary>
    /// 请求消息
    /// </summary>
    public INetworkMessage RequestMessage { get; private set; }

    /// <summary>
    /// 响应消息
    /// </summary>
    public INetworkMessage ResponseMessage { get; private set; }

    /// <summary>
    /// RPC 耗时时间.单位毫秒
    /// 从创建到回复的时间差
    /// </summary>
    public long Time { get; private set; }

    /// <summary>
    /// RPC 回复任务
    /// </summary>
    public Task<IRpcResult> Task
    {
        get { return _tcs.Task; }
    }

    /// <summary>
    /// </summary>
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
    /// RPC 回复
    /// </summary>
    /// <param name="responseMessage"></param>
    public bool Reply(IResponseMessage responseMessage)
    {
        ResponseMessage = responseMessage;
        Time = TimerHelper.UnixTimeMilliseconds() - CreatedTime;
        var result = new RpcResult(responseMessage);
        _tcs.SetResult(result);
        return true;
    }

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="requestMessage">请求消息</param>
    /// <param name="isReply">是否需要回复</param>
    /// <param name="timeout">超时时间,单位毫秒</param>
    /// <returns></returns>
    public static IRpcSessionData Create(IRequestMessage requestMessage, bool isReply = true, int timeout = 10000)
    {
        var rpcData = new RpcSessionData(requestMessage, isReply, timeout);
        return rpcData;
    }

    /// <summary>
    /// 增加时间。如果超时返回true
    /// </summary>
    /// <param name="millisecondsTime">流逝时间.单位毫秒</param>
    /// <returns></returns>
    public bool IncrementalElapseTime(long millisecondsTime)
    {
        ElapseTime += millisecondsTime;
        if (ElapseTime >= Timeout)
        {
            var error = "Rpc call timeout! Message is :" + RequestMessage;
            _tcs.TrySetResult(new RpcResult(error));
            return true;
        }

        return false;
    }

    /// <summary>
    /// 析构函数
    /// </summary>
    ~RpcSessionData()
    {
        Dispose();
    }
}