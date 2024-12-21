#if !NO_RUNTIME
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class Int64Serializer : IProtoSerializer
{
    private static readonly Type expectedType = typeof(long);

    public Int64Serializer(TypeModel model)
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
        return source.ReadInt64();
    }

    public void Write(object value, ProtoWriter dest)
    {
        ProtoWriter.WriteInt64((long)value, dest);
    }

#if FEAT_COMPILER
        void IProtoSerializer.EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicWrite("WriteInt64", valueFrom);
        }
        void IProtoSerializer.EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicRead("ReadInt64", ExpectedType);
        }
#endif
}
#endif