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


using System.Net;
using GameFrameX.SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork.Kcp;

/// <summary>
/// KCP game app session wrapper / KCP 游戏应用会话包装器
/// Implements IGameAppSession interface for compatibility with the existing framework
/// </summary>
public sealed class KcpGameAppSession : IGameAppSession
{
    private bool _disposed;

    /// <summary>
    /// Creates a new KCP game app session / 创建新的 KCP 游戏应用会话
    /// </summary>
    /// <param name="kcpSession">KCP session / KCP 会话</param>
    public KcpGameAppSession(IKcpSession kcpSession)
    {
        KcpSession = kcpSession ?? throw new ArgumentNullException(nameof(kcpSession));
    }

    /// <summary>
    /// Remote endpoint / 远程端点
    /// </summary>
    public EndPoint RemoteEndPoint
    {
        get { return KcpSession.RemoteEndPoint; }
    }

    /// <summary>
    /// KCP session / KCP 会话
    /// </summary>
    public IKcpSession KcpSession { get; }

    /// <summary>
    /// Session unique ID / 会话唯一 ID
    /// </summary>
    public string SessionID
    {
        get { return KcpSession.ConversationId.ToString(); }
    }

    /// <summary>
    /// Session is connected / 会话是否已连接
    /// </summary>
    public bool IsConnected
    {
        get { return KcpSession.IsConnected && !_disposed; }
    }

    /// <summary>
    /// Send data to client / 发送数据到客户端
    /// </summary>
    /// <param name="data">Data to send / 要发送的数据</param>
    /// <param name="cancellationToken">Cancellation token / 取消令牌</param>
    public async ValueTask SendAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        if (_disposed || !KcpSession.IsConnected)
        {
            return;
        }

        await KcpSession.SendAsync(data, cancellationToken);
    }

    /// <summary>
    /// Send data to client / 发送数据到客户端
    /// </summary>
    /// <param name="data">Data to send / 要发送的数据</param>
    /// <param name="cancellationToken">Cancellation token / 取消令牌</param>
    public async ValueTask SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        if (_disposed || !KcpSession.IsConnected)
        {
            return;
        }

        await KcpSession.SendAsync(data, cancellationToken);
    }

    /// <summary>
    /// Close the session / 关闭会话
    /// </summary>
    public void Close()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        KcpSession.Close();
    }

    /// <summary>
    /// Dispose resources / 释放资源
    /// </summary>
    public void Dispose()
    {
        Close();
    }
}