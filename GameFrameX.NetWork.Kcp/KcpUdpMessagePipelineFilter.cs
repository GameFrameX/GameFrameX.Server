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

using System.Buffers;
using System.Collections.Concurrent;
using System.Net;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Message;
using GameFrameX.SuperSocket.ProtoBase;
using GameFrameX.SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork.Kcp;

/// <summary>
/// UDP filter that routes datagrams through KCP and outputs IMessage / 将 UDP 数据经 KCP 解析并输出 IMessage
/// </summary>
public sealed class KcpUdpMessagePipelineFilter : PipelineFilterBase<IMessage>
{
    private readonly MessageObjectPipelineFilter _fallbackFilter = new();
    private readonly KcpUdpMessagePipelineOptions _options;
    private SessionContext _sessionContext;

    public KcpUdpMessagePipelineFilter(KcpUdpMessagePipelineOptions options)
    {
        _options = options ?? new KcpUdpMessagePipelineOptions();
    }

    public override IMessage Filter(ref SequenceReader<byte> reader)
    {
        _fallbackFilter.Context = Context;
        _fallbackFilter.Decoder = Decoder;

        if (!_options.EnableKcp)
        {
            return _fallbackFilter.Filter(ref reader);
        }

        var sessionContext = EnsureSessionContext();
        if (sessionContext == null)
        {
            return _fallbackFilter.Filter(ref reader);
        }

        if (sessionContext.PendingMessages.TryDequeue(out var cachedMessage))
        {
            return cachedMessage;
        }

        var packetLength = (int)reader.Remaining;
        if (packetLength <= 0)
        {
            return null;
        }

        var packetData = reader.Sequence.Slice(reader.Position, reader.Remaining).ToArray();
        reader.Advance(reader.Remaining);

        var kcpSession = sessionContext.SessionManager.GetOrCreateSession(sessionContext.RemoteEndPoint);
        kcpSession.Input(packetData);

        var parsedMessages = sessionContext.KcpFilter.Filter(kcpSession);
        if (parsedMessages.Count > 0)
        {
            var gameSession = new KcpGameAppSession(kcpSession);
            foreach (var message in parsedMessages)
            {
                sessionContext.PendingMessages.Enqueue(message);
                NotifyPackageReceived(gameSession, message);
            }
        }

        if (sessionContext.PendingMessages.TryDequeue(out var package))
        {
            return package;
        }

        return null;
    }

    public override void Reset()
    {
        _sessionContext?.Dispose();
        _sessionContext = null;
        _fallbackFilter.Reset();
        base.Reset();
    }

    private SessionContext EnsureSessionContext()
    {
        if (_sessionContext != null)
        {
            return _sessionContext;
        }

        if (Context is not IAppSession appSession || appSession.RemoteEndPoint == null)
        {
            return null;
        }

        var sessionManager = new KcpSessionManager(
            _options.CreateKcpOptions(),
            (outputData, remoteEndPoint) =>
            {
                var sendTask = appSession.SendAsync(outputData);
            },
            _options.OnKcpConnected,
            _options.OnKcpDisconnected);

        _sessionContext = new SessionContext(appSession.RemoteEndPoint, sessionManager, new KcpMessagePipelineFilter());
        return _sessionContext;
    }

    private void NotifyPackageReceived(IGameAppSession session, IMessage message)
    {
        try
        {
            _options.OnKcpPackage?.Invoke(session, message);
        }
        catch
        {
            // Ignore callback failures to avoid impacting network pipeline.
        }
    }

    private sealed class SessionContext : IDisposable
    {
        public SessionContext(EndPoint remoteEndPoint, KcpSessionManager sessionManager, KcpMessagePipelineFilter kcpFilter)
        {
            RemoteEndPoint = remoteEndPoint;
            SessionManager = sessionManager;
            KcpFilter = kcpFilter;
        }

        public EndPoint RemoteEndPoint { get; }

        public KcpSessionManager SessionManager { get; }

        public KcpMessagePipelineFilter KcpFilter { get; }

        public ConcurrentQueue<IMessage> PendingMessages { get; } = new();

        public void Dispose()
        {
            SessionManager.Dispose();
        }
    }
}
