namespace GameFrameX.NetWork.Messages;

public interface IActorResponseMessage : IResponseMessage
{
    public long ActorId { get; set; }
    public long ActorInstanceId { get; set; }
}