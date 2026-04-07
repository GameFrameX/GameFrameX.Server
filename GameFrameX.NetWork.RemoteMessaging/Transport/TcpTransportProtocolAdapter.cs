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
/// 默认 TCP 协议适配器。负责服务端点解析与 TCP 连接建立，
/// 对上层屏蔽具体网络细节。
/// </summary>
/// <remarks>
/// Default TCP protocol adapter. Responsible for service endpoint resolution and TCP connection setup,
/// while hiding concrete networking details from upper layers.
/// </remarks>
internal sealed class TcpTransportProtocolAdapter : ITransportProtocolAdapter
{
    private readonly IConnectionProvider _connectionProvider;
    private readonly IServiceEndpointResolver _endpointResolver;

    /// <summary>
    /// 初始化 TCP 协议适配器。
    /// </summary>
    /// <remarks>
    /// Initializes the TCP protocol adapter.
    /// </remarks>
    /// <param name="endpointResolver">服务发现解析器 / Service endpoint resolver</param>
    /// <param name="connectionProvider">连接提供器 / Connection provider</param>
    public TcpTransportProtocolAdapter(IServiceEndpointResolver endpointResolver, IConnectionProvider connectionProvider)
    {
        _endpointResolver = endpointResolver;
        _connectionProvider = connectionProvider;
    }

    /// <inheritdoc />
    public string ProtocolName => "TCP";

    /// <inheritdoc />
    public async Task<Stream> GetOrCreateStreamAsync(string serviceName, CancellationToken cancellationToken = default)
    {
        var endpoint = _endpointResolver.ResolveTcpEndpoint(serviceName);
        if (!TryParseTcpEndpoint(endpoint, out var host, out var port))
        {
            throw new RemoteEndpointNotFoundException(serviceName, endpoint);
        }

        return await _connectionProvider.GetOrCreateStreamAsync(host, port, cancellationToken);
    }

    /// <inheritdoc />
    public bool IsServiceAvailable(string serviceName)
    {
        var endpoint = _endpointResolver.ResolveTcpEndpoint(serviceName);
        return TryParseTcpEndpoint(endpoint, out _, out _);
    }

    /// <inheritdoc />
    public void Invalidate()
    {
        _connectionProvider.Invalidate();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _connectionProvider.Dispose();
    }

    private static bool TryParseTcpEndpoint(string endpoint, out string host, out int port)
    {
        host = string.Empty;
        port = 0;
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return false;
        }

        var normalized = endpoint.Trim();
        if (!normalized.Contains("://", StringComparison.Ordinal))
        {
            normalized = $"tcp://{normalized}";
        }

        if (Uri.TryCreate(normalized, UriKind.Absolute, out var uri))
        {
            host = uri.Host;
            port = uri.Port;
            return !string.IsNullOrWhiteSpace(host) && port > 0;
        }

        var lastSeparatorIndex = endpoint.LastIndexOf(':');
        if (lastSeparatorIndex <= 0 || lastSeparatorIndex >= endpoint.Length - 1)
        {
            return false;
        }

        host = endpoint[..lastSeparatorIndex];
        return int.TryParse(endpoint[(lastSeparatorIndex + 1)..], out port) && port > 0;
    }
}
