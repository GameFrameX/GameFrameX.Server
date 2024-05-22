using SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.PipelineFilter;

public class MessageObjectPipelineFilter : PipelineFilterBase<IMessage>
{
    public override IMessage Filter(ref SequenceReader<byte> reader)
    {
        var pack = reader.Sequence;
        reader.TryReadBigEndian(out int totalLength);
        if (totalLength <= 0)
        {
            reader.AdvanceToEnd();
            return null;
        }

        var readBuffer = pack.Slice(pack.Start, totalLength);
        if (reader.Remaining < totalLength)
        {
            reader.AdvanceToEnd();
        }
        else
        {
            // 前面读了一个长度，所以要减回去
            reader.Advance(totalLength - 4);
        }

        return this.Decoder.Decode(ref readBuffer, this.Context);
    }
}