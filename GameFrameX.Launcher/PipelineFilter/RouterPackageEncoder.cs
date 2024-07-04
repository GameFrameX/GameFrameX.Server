using GameFrameX.SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.PipelineFilter;

public class RouterPackageEncoder : IPackageEncoder<RouterPackageEncoder>
{
    public int Encode(IBufferWriter<byte> writer, RouterPackageEncoder pack)
    {
        return 0;
    }
}