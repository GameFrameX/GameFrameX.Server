using GameFrameX.NetWork.Messages;

namespace GameFrameX.NetWork;

public interface IMessageDecoderHandler
{
    IMessage Handler(Span<byte> data);
}