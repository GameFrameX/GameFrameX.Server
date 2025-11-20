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