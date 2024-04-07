using GameFrameX.NetWork.Messages;
using SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.PipelineFilter;

public class MessageObjectPipelineFilter : IPipelineFilter<IMessage>
{
    public void Reset()
    {
    }

    public object? Context { get; set; }

    public IMessage Filter(ref SequenceReader<byte> reader)
    {
        ReadOnlySequence<byte> buffer = reader.Sequence;
        reader.Advance(buffer.Length);
        return this.Decoder.Decode(ref buffer, this.Context);
    }

    public IPackageDecoder<IMessage> Decoder { get; set; }
    public IPipelineFilter<IMessage> NextFilter { get; }
}