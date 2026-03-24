using GameFrameX.Core.Abstractions.Events;

namespace GameFrameX.Apps.Common.EventData;

public sealed class ServiceOnlineEventArgs : GameEventArgs
{
    public string ServiceName { get; }
    public long InstanceId { get; }
    public IReadOnlyList<string> Endpoints { get; }
    public DateTime Timestamp { get; }

    public ServiceOnlineEventArgs(string serviceName, long instanceId, DateTime timestamp, IReadOnlyList<string> endpoints = null)
    {
        ServiceName = serviceName;
        InstanceId = instanceId;
        Timestamp = timestamp;
        Endpoints = endpoints ?? Array.Empty<string>();
    }
}
