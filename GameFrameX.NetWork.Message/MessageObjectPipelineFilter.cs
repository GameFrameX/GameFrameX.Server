using System.Buffers;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.SuperSocket.ProtoBase;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 消息对象流水线过滤处理器
/// </summary>
public class MessageObjectPipelineFilter : PipelineFilterBase<INetworkMessage>
{
    public override INetworkMessage Filter(ref SequenceReader<byte> reader)
    {
        var pack = reader.Sequence;
        reader.TryReadBigEndian(out ushort totalLength);
        if (totalLength <= 0)
        {
            reader.AdvanceToEnd();
            return default;
        }

        var readBuffer = pack.Slice(pack.Start, totalLength);
        if (reader.Remaining < totalLength)
        {
            reader.AdvanceToEnd();
        }
        else
        {
            // 前面读了一个长度，所以要减回去
            reader.Advance(totalLength - 2);
        }

        return this.Decoder.Decode(ref readBuffer, this.Context);
    }
}