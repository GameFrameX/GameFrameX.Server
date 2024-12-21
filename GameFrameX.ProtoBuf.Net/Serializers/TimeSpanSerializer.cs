#if !NO_RUNTIME
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class TimeSpanSerializer : IProtoSerializer
{
    private static readonly Type expectedType = typeof(TimeSpan);
    private readonly bool wellKnown;

    public TimeSpanSerializer(DataFormat dataFormat, TypeModel model)
    {
        wellKnown = dataFormat == DataFormat.WellKnown;
    }

    public Type ExpectedType
    {
        get { return expectedType; }
    }

    bool IProtoSerializer.RequiresOldValue
    {
        get { return false; }
    }

    bool IProtoSerializer.ReturnsValue
    {
        get { return true; }
    }

    public object Read(object value, ProtoReader source)
    {
        if (wellKnown)
        {
            return BclHelpers.ReadDuration(source);
        }

        Helpers.DebugAssert(value == null); // since replaces
        return BclHelpers.ReadTimeSpan(source);
    }

    public void Write(object value, ProtoWriter dest)
    {
        if (wellKnown)
        {
            BclHelpers.WriteDuration((TimeSpan)value, dest);
        }
        else
        {
            BclHelpers.WriteTimeSpan((TimeSpan)value, dest);
        }
    }

#if FEAT_COMPILER
        void IProtoSerializer.EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitWrite(ctx.MapType(typeof(BclHelpers)),
                wellKnown ? nameof(BclHelpers.WriteDuration) : nameof(BclHelpers.WriteTimeSpan), valueFrom);
        }
        void IProtoSerializer.EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            if (wellKnown) ctx.LoadValue(valueFrom);
            ctx.EmitBasicRead(ctx.MapType(typeof(BclHelpers)),
                wellKnown ? nameof(BclHelpers.ReadDuration) : nameof(BclHelpers.ReadTimeSpan),
                ExpectedType);
        }
#endif
}
#endif