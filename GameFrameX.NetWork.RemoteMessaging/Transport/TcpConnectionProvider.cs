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

using System.Net.Sockets;

namespace GameFrameX.NetWork.RemoteMessaging.Transport;

/// <summary>
/// TCP 长连接提供器。管理单条 TCP 连接的创建、复用和释放。
/// </summary>
/// <remarks>
/// TCP persistent connection provider. Manages the creation, reuse, and disposal of a single TCP connection.
/// </remarks>
internal sealed class TcpConnectionProvider : IConnectionProvider
{
    private const int DefaultTimeoutMs = 5000;

    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private string _connectedEndpoint = string.Empty;
    private bool _disposed;
    private NetworkStream _networkStream;
    private TcpClient _tcpClient;

    /// <summary>
    /// 获取或创建到指定目标的可用网络流。
    /// </summary>
    /// <remarks>
    /// Gets an existing or creates a new usable network stream to the specified target.
    /// </remarks>
    /// <param name="host">目标主机 / Target host</param>
    /// <param name="port">目标端口 / Target port</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>可用的网络流 / An available network stream</returns>
    public async Task<Stream> GetOrCreateStreamAsync(string host, int port, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            var endpoint = $"{host}:{port}";
            if (_tcpClient != null && _tcpClient.Connected && _networkStream != null && _connectedEndpoint == endpoint)
            {
                return _networkStream;
            }

            DisposeCore();
            var client = new TcpClient
            {
                NoDelay = true,
                ReceiveTimeout = DefaultTimeoutMs,
                SendTimeout = DefaultTimeoutMs,
            };
            await client.ConnectAsync(host, port, cancellationToken);
            _tcpClient = client;
            _networkStream = client.GetStream();
            _connectedEndpoint = endpoint;
            return _networkStream;
        }
        catch
        {
            DisposeCore();
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// 标记当前连接为失效，下次调用时重建。
    /// </summary>
    /// <remarks>
    /// Marks the current connection as invalid so it will be re-established on the next call.
    /// </remarks>
    public void Invalidate()
    {
        DisposeCore();
    }

    /// <summary>
    /// 释放 TCP 连接和关联资源。
    /// </summary>
    /// <remarks>
    /// Releases the TCP connection and associated resources.
    /// </remarks>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        DisposeCore();
        _semaphore.Dispose();
    }

    private void DisposeCore()
    {
        _networkStream?.Dispose();
        _networkStream = null;
        _tcpClient?.Dispose();
        _tcpClient = null;
        _connectedEndpoint = string.Empty;
    }
}