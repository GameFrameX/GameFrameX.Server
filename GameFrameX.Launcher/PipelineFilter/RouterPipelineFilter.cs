using SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.PipelineFilter;

public class RouterPipelineFilter : IPipelineFilter<IMessage>
{
    public void Reset()
    {
    }

    public object Context { get; set; }
    public IPackageDecoder<IMessage> Decoder { get; set; }

    public IMessage Filter(ref SequenceReader<byte> reader)
    {
        return null;
    }

    public IPipelineFilter<IMessage> NextFilter { get; }
}