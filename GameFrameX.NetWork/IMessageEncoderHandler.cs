using GameFrameX.NetWork.Messages;

namespace GameFrameX.NetWork;

public interface IMessageEncoderHandler
{
    byte[] Handler(IMessage message);
}