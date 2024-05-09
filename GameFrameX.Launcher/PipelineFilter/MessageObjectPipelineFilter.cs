using SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.PipelineFilter;

public class MessageObjectPipelineFilter : PipelineFilterBase<IMessage>
{
    const int HeaderSize = 4 + 4 + 4 + 4;

    public override IMessage Filter(ref SequenceReader<byte> reader)
    {
        var pack = reader.Sequence;
        reader.TryReadBigEndian(out int length);
        if (length <= 0)
        {
            reader.AdvanceToEnd();
            return null;
        }

        int totalLength = length + HeaderSize;
        var readBuffer = pack.Slice(pack.Start, totalLength);
        reader.Advance(totalLength);
        return this.Decoder.Decode(ref readBuffer, this.Context);
    }
}