using System.Text;

namespace GameFrameX.Tests;

public class CompressTest
{
    private byte[] bytes;
    private byte[] comData;

    [SetUp]
    public void Setup()
    {
        bytes = Encoding.UTF8.GetBytes("qwertyuiopasdfghjklzxcvbnmuiopasddfghjklzdfghjklzfuiopasdfdfghjklzdfghjklzuidfghjklzdfghjklzopasdfuidfghjklzdfghjklzodfghjklzdfghjklzpasdfuiopasdfuidfghjklzdfghjklzopasdfuiopasdfuiopasdfuiopasdf");
    }

    [Test]
    public void compress()
    {
        var comData = GameFrameX.Utility.Compression.Compress(bytes);
        Console.WriteLine(comData);
    }

    [Test]
    public void decompress()
    {
        comData = GameFrameX.Utility.Compression.Compress(bytes);
        var data = GameFrameX.Utility.Compression.Decompress(comData);
        var x = Encoding.UTF8.GetString(data);
        Console.WriteLine(x);
    }
}