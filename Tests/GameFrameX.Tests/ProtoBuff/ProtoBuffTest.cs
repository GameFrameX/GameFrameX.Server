using GameFrameX.ProtoBuf.Net;
using ProtoBuf;
using Xunit;

namespace GameFrameX.Tests.ProtoBuff;

public class ProtoBuffTest
{
    [ProtoContract]
    public class PbTest
    {
        [ProtoMember(1, IsRequired = false)] public List<string> Test { get; set; } = new List<string>();
    }

    public ProtoBuffTest()
    {
        // xUnit uses constructor for setup
    }

    [Fact]
    public void TestSerialize()
    {
        PbTest pbTest = new PbTest();
        pbTest.Test.Add("1");
        pbTest.Test.Add(null);
        pbTest.Test.Add("3");
        pbTest.Test.Add("4");
        pbTest.Test.Add("5");
        pbTest.Test.Add("6");
        pbTest.Test.Add("7");
        pbTest.Test.Add("8");
        pbTest.Test.Add("9");

        try
        {
            var xx = ProtoBufSerializerHelper.Serialize(pbTest);
            Console.WriteLine(xx);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}