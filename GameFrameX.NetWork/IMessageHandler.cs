using GameFrameX.NetWork.Messages;

namespace GameFrameX.NetWork;

public interface IMessageHandler
{
    Task Init();
    Task InnerAction();
    MessageObject Message { get; set; }
    INetChannel Channel { get; set; }
}