using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.NetWork;

/// <summary>
/// RPC请求结果
/// </summary>
public sealed class RpcResult : IRpcResult, IDisposable
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess
    {
        get { return Error == string.Empty; }
    }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string Error { get; private set; }

    /// <summary>
    /// 返回消息
    /// </summary>
    public IResponseMessage Message { get; private set; }


    /// <summary>
    /// 创建消息结果对象
    /// </summary>
    /// <param name="message"></param>
    internal RpcResult(IResponseMessage message)
    {
        Message = message;
        Error = string.Empty;
    }

    /// <summary>
    /// 创建消息结果对象
    /// </summary>
    /// <param name="error">错误信息</param>
    internal RpcResult(string error)
    {
        Error = error;
    }

    /// <summary>
    /// 析构函数
    /// </summary>
    ~RpcResult()
    {
        Dispose();
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        if (Message != null)
        {
            GC.SuppressFinalize(Message);
        }

        Error = string.Empty;
        Message = null;
        GC.SuppressFinalize(this);
    }
}