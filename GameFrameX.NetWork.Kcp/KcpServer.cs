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


using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Foundation.Logger;
using GameFrameX.Localization;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.Utility.Setting;

namespace GameFrameX.NetWork.Kcp;

/// <summary>
/// KCP server / KCP 服务器
/// </summary>
public sealed class KcpServer : IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ConcurrentDictionary<uint, KcpNetWorkChannel> _channels = new();
    private readonly KcpMessagePipelineFilter _messageFilter;
    private readonly Func<EndPoint, ValueTask> _onConnected;
    private readonly Func<EndPoint, ValueTask> _onDisconnected;
    private readonly KcpOptions _options;
    private readonly Action<IGameAppSession, IMessage> _packageHandler;
    private readonly byte[] _receiveBuffer;
    private readonly AppSetting _setting;
    private readonly Socket _udpSocket;
    private bool _disposed;

    /// <summary>
    /// Creates a new KCP server / 创建新的 KCP 服务器
    /// </summary>
    /// <param name="port">Listening port / 监听端口</param>
    /// <param name="options">KCP options / KCP 配置选项</param>
    /// <param name="setting">Application settings / 应用配置</param>
    /// <param name="packageHandler">Message handler / 消息处理器</param>
    /// <param name="onConnected">Connected callback / 连接回调</param>
    /// <param name="onDisconnected">Disconnected callback / 断开回调</param>
    public KcpServer(
        int port,
        KcpOptions options,
        AppSetting setting,
        Action<IGameAppSession, IMessage> packageHandler,
        Func<EndPoint, ValueTask> onConnected,
        Func<EndPoint, ValueTask> onDisconnected)
    {
        Port = port;
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
        _packageHandler = packageHandler ?? throw new ArgumentNullException(nameof(packageHandler));
        _onConnected = onConnected;
        _onDisconnected = onDisconnected;
        _receiveBuffer = new byte[options.Mtu * 2];
        _messageFilter = new KcpMessagePipelineFilter();

        _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            ReceiveBufferSize = 1024 * 1024,
            SendBufferSize = 1024 * 1024,
        };

        SessionManager = new KcpSessionManager(_options, SendOutput);
    }

    /// <summary>
    /// Gets the session manager / 获取会话管理器
    /// </summary>
    public KcpSessionManager SessionManager { get; }

    /// <summary>
    /// Gets whether the server is running / 获取服务器是否正在运行
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Gets the listening port / 获取监听端口
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// Dispose resources / 释放资源
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        Stop();
        _cancellationTokenSource.Dispose();
    }

    /// <summary>
    /// Start the KCP server / 启动 KCP 服务器
    /// </summary>
    public async Task StartAsync()
    {
        if (IsRunning)
        {
            return;
        }

        _udpSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
        IsRunning = true;

        LogHelper.Info(LocalizationService.GetString(Keys.StartUp.KcpServer.StartupComplete, "KCP", "0.0.0.0", Port));

        _ = Task.Run(ReceiveLoop, _cancellationTokenSource.Token);
    }

    /// <summary>
    /// Stop the KCP server / 停止 KCP 服务器
    /// </summary>
    public void Stop()
    {
        if (!IsRunning)
        {
            return;
        }

        IsRunning = false;
        _cancellationTokenSource.Cancel();
        _udpSocket.Close();

        foreach (var channel in _channels.Values)
        {
            channel.Close();
        }

        _channels.Clear();
        SessionManager.Dispose();

        LogHelper.Info(LocalizationService.GetString(Keys.StartUp.KcpServer.ClientDisconnected, "Server stopped"));
    }

    private async Task ReceiveLoop()
    {
        var endpoint = new IPEndPoint(IPAddress.Any, 0) as EndPoint;

        while (!_cancellationTokenSource.Token.IsCancellationRequested && IsRunning)
        {
            try
            {
                var result = await _udpSocket.ReceiveFromAsync(_receiveBuffer, endpoint);
                if (result.ReceivedBytes > 0)
                {
                    ProcessReceivedData(result.RemoteEndPoint, _receiveBuffer.AsSpan(0, result.ReceivedBytes));
                }
            }
            catch (SocketException)
            {
                // Socket closed or error, exit loop
                break;
            }
            catch (ObjectDisposedException)
            {
                // Socket disposed, exit loop
                break;
            }
            catch (Exception ex)
            {
                LogHelper.Error<string>("KCP receive error: {message}", ex.Message);
            }
        }
    }

    private void ProcessReceivedData(EndPoint remoteEndPoint, ReadOnlySpan<byte> data)
    {
        if (data.Length < 4)
        {
            return;
        }

        // Try to get conversation ID from the first 4 bytes (KCP header)
        var conversationId = BitConverter.ToUInt32(data.Slice(0, 4));
        if (BitConverter.IsLittleEndian)
        {
            conversationId = (conversationId >> 24) | ((conversationId >> 8) & 0xFF00) | ((conversationId << 8) & 0xFF0000) | (conversationId << 24);
        }

        IKcpSession session;
        var isNewSession = false;
        if (SessionManager.TryGetSession(conversationId, out var existingSession))
        {
            session = existingSession;
        }
        else
        {
            session = SessionManager.GetOrCreateSession(remoteEndPoint, conversationId);
            isNewSession = true;
        }

        // Input data to KCP session
        session.Input(data);

        // Process received messages
        ProcessSessionMessages(session, isNewSession, remoteEndPoint);
    }

    private void ProcessSessionMessages(IKcpSession session, bool isNewSession, EndPoint remoteEndPoint)
    {
        if (isNewSession)
        {
            _ = _onConnected?.Invoke(remoteEndPoint);
        }

        var messages = _messageFilter.Filter(session);
        foreach (var message in messages)
        {
            if (!_channels.TryGetValue(session.ConversationId, out var channel))
            {
                channel = new KcpNetWorkChannel(session, _setting);
                _channels[session.ConversationId] = channel;
            }

            _packageHandler(channel.GameAppSession, message);
        }
    }

    private void SendOutput(ReadOnlyMemory<byte> data, EndPoint remoteEndPoint)
    {
        if (!IsRunning || _disposed)
        {
            return;
        }

        try
        {
            _udpSocket.SendTo(data.Span, SocketFlags.None, remoteEndPoint);
        }
        catch (Exception ex)
        {
            LogHelper.Error<string>("KCP send error: {message}", ex.Message);
        }
    }

    /// <summary>
    /// Send data to a specific session / 发送数据到指定会话
    /// </summary>
    /// <param name="conversationId">Conversation ID / 会话 ID</param>
    /// <param name="data">Data to send / 要发送的数据</param>
    public async ValueTask SendAsync(uint conversationId, ReadOnlyMemory<byte> data)
    {
        if (SessionManager.TryGetSession(conversationId, out var session))
        {
            await session.SendAsync(data, CancellationToken.None);
        }
    }

    /// <summary>
    /// Send data to a specific endpoint / 发送数据到指定端点
    /// </summary>
    /// <param name="endPoint">Remote endpoint / 远程端点</param>
    /// <param name="data">Data to send / 要发送的数据</param>
    public async ValueTask SendAsync(EndPoint endPoint, ReadOnlyMemory<byte> data)
    {
        if (SessionManager.TryGetSession(endPoint, out var session))
        {
            await session.SendAsync(data, CancellationToken.None);
        }
    }

    /// <summary>
    /// Get channel by conversation ID / 根据会话 ID 获取通道
    /// </summary>
    /// <param name="conversationId">Conversation ID / 会话 ID</param>
    /// <param name="channel">Network channel / 网络通道</param>
    /// <returns>True if channel exists / 如果通道存在则返回 true</returns>
    public bool TryGetChannel(uint conversationId, out KcpNetWorkChannel channel)
    {
        return _channels.TryGetValue(conversationId, out channel);
    }
}