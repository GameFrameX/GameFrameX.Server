#if !NO_RUNTIME
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class DecimalSerializer : IProtoSerializer
{
    private static readonly Type expectedType = typeof(decimal);

    public DecimalSerializer(TypeModel model)
    {
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
        Helpers.DebugAssert(value == null); // since replaces
        return BclHelpers.ReadDecimal(source);
    }

    public void Write(object value, ProtoWriter dest)
    {
        BclHelpers.WriteDecimal((decimal)value, dest);
    }

#if FEAT_COMPILER
        void IProtoSerializer.EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitWrite(ctx.MapType(typeof(BclHelpers)), "WriteDecimal", valueFrom);
        }
        void IProtoSerializer.EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicRead(ctx.MapType(typeof(BclHelpers)), "ReadDecimal", ExpectedType);
        }
#endif
}
#endif