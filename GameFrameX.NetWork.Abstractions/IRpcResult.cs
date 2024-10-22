namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// RPC请求结果
/// </summary>
public interface IRpcResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// 错误信息
    /// </summary>
    string Error { get; }

    /// <summary>
    /// 返回消息
    /// </summary>
    IResponseMessage Message { get; }
}