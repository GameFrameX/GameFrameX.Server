// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   均受中华人民共和国及相关国际法律法规保护。
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   侵犯他人合法权益等法律法规所禁止的行为！
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   本项目组织与贡献者概不承担。
//   GitHub 仓库：https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System.Collections.Concurrent;
using GameFrameX.NetWork.RemoteMessaging.Abstractions;

namespace GameFrameX.NetWork.RemoteMessaging.Internal;

/// <summary>
/// 请求-响应匹配器。基于 UniqueId 管理请求表，支持超时清理和完成回调。
/// </summary>
internal sealed class RequestResponseMatcher : IRequestResponseMatcher
{
    private int _uniqueId;
    private readonly ConcurrentDictionary<int, PendingEntry> _pendingRequests = new();

    /// <inheritdoc />
    public int RegisterPendingRequest(int timeoutMs)
    {
        var uniqueId = Interlocked.Increment(ref _uniqueId);
        var entry = new PendingEntry
        {
            ExpiresAt = Environment.TickCount + timeoutMs,
        };
        _pendingRequests[uniqueId] = entry;
        return uniqueId;
    }

    /// <inheritdoc />
    public Task<MessageObject> WaitResponseAsync(int uniqueId, CancellationToken cancellationToken)
    {
        if (_pendingRequests.TryGetValue(uniqueId, out var entry))
        {
            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() => entry.TaskCompletionSource.TrySetCanceled(cancellationToken));
            }

            return entry.TaskCompletionSource.Task;
        }

        return Task.FromResult<MessageObject>(null);
    }

    /// <inheritdoc />
    public bool TryComplete(int uniqueId, MessageObject response)
    {
        if (!_pendingRequests.TryRemove(uniqueId, out var entry))
        {
            return false;
        }

        entry.TaskCompletionSource.TrySetResult(response);
        return true;
    }

    /// <inheritdoc />
    public void CleanupExpired()
    {
        var now = Environment.TickCount;
        foreach (var kvp in _pendingRequests)
        {
            if (now > kvp.Value.ExpiresAt)
            {
                if (_pendingRequests.TryRemove(kvp.Key, out var entry))
                {
                    entry.TaskCompletionSource.TrySetCanceled();
                }
            }
        }
    }

    private sealed class PendingEntry
    {
        public TaskCompletionSource<MessageObject> TaskCompletionSource { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public int ExpiresAt { get; set; }
    }
}
