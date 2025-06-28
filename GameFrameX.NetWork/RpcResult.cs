// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.NetWork;

/// <summary>
/// RPC请求结果
/// </summary>
public sealed class RpcResult : IRpcResult, IDisposable
{
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
    /// 析构函数
    /// </summary>
    ~RpcResult()
    {
        Dispose();
    }
}