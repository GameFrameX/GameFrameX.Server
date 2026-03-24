using GameFrameX.Core.Abstractions.Events;

namespace GameFrameX.Apps.Common.EventData;

public sealed class ServiceOfflineEventArgs : GameEventArgs
{
    public string ServiceName { get; }
    public long InstanceId { get; }
    public string Reason { get; }
    public DateTime Timestamp { get; }

    public ServiceOfflineEventArgs(string serviceName, long instanceId, string reason, DateTime timestamp)
    {
        ServiceName = serviceName;
        InstanceId = instanceId;
        Reason = reason;
        Timestamp = timestamp;
    }
}
