#if !NO_RUNTIME
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class SingleSerializer : IProtoSerializer
{
    private static readonly Type expectedType = typeof(float);

    public Type ExpectedType
    {
        get { return expectedType; }
    }

    public SingleSerializer(TypeModel model)
    {
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
        return source.ReadSingle();
    }

    public void Write(object value, ProtoWriter dest)
    {
        ProtoWriter.WriteSingle((float)value, dest);
    }


#if FEAT_COMPILER
        void IProtoSerializer.EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicWrite("WriteSingle", valueFrom);
        }
        void IProtoSerializer.EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicRead("ReadSingle", ExpectedType);
        }
#endif
}
#endif