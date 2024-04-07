namespace GameFrameX.NetWork.Messages;

public interface IActorRequestMessage : IRequestMessage
{
    public long ActorId { get; set; }
    public long ActorInstanceId { get; set; }
}