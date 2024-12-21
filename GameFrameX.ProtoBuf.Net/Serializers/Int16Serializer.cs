#if !NO_RUNTIME
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class Int16Serializer : IProtoSerializer
{
    private static readonly Type expectedType = typeof(short);

    public Int16Serializer(TypeModel model)
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
        return source.ReadInt16();
    }

    public void Write(object value, ProtoWriter dest)
    {
        ProtoWriter.WriteInt16((short)value, dest);
    }

#if FEAT_COMPILER
        void IProtoSerializer.EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicWrite("WriteInt16", valueFrom);
        }
        void IProtoSerializer.EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicRead("ReadInt16", ExpectedType);
        }
#endif
}
#endif