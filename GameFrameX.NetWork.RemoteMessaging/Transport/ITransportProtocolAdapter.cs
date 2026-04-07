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

namespace GameFrameX.NetWork.RemoteMessaging.Transport;

/// <summary>
/// 可插拔传输协议适配器。屏蔽服务发现、地址解析与连接建立细节，
/// 让上层调用逻辑不直接依赖具体协议（TCP/KCP/QUIC）。
/// </summary>
/// <remarks>
/// Pluggable transport protocol adapter. Hides service discovery, endpoint parsing, and connection setup details,
/// so upper-layer call logic does not directly depend on a concrete protocol (TCP/KCP/QUIC).
/// </remarks>
public interface ITransportProtocolAdapter : IDisposable
{
    /// <summary>
    /// 获取协议名称（如 TCP/KCP/QUIC）。
    /// </summary>
    /// <remarks>
    /// Gets the protocol name (e.g., TCP/KCP/QUIC).
    /// </remarks>
    string ProtocolName { get; }

    /// <summary>
    /// 获取或创建目标服务对应的可用流。
    /// </summary>
    /// <remarks>
    /// Gets an existing or creates a new usable stream for the target service.
    /// </remarks>
    /// <param name="serviceName">目标服务名 / Target service name</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>可用流 / Usable stream</returns>
    Task<Stream> GetOrCreateStreamAsync(string serviceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查目标服务在当前协议下是否可用（至少可解析到合法端点）。
    /// </summary>
    /// <remarks>
    /// Checks whether the target service is available under current protocol (at least resolves to a valid endpoint).
    /// </remarks>
    /// <param name="serviceName">目标服务名 / Target service name</param>
    /// <returns>服务是否可用 / Whether service is available</returns>
    bool IsServiceAvailable(string serviceName);

    /// <summary>
    /// 标记连接失效，促使下次请求重建连接。
    /// </summary>
    /// <remarks>
    /// Invalidates current connection so next request can recreate it.
    /// </remarks>
    void Invalidate();
}
