using System.Text;
using GameFrameX.Utility;

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
        var comData = Compression.Compress(bytes);
        Console.WriteLine(comData);
    }

    [Test]
    public void decompress()
    {
        comData = Compression.Compress(bytes);
        var data = Compression.Decompress(comData);
        var x = Encoding.UTF8.GetString(data);
        Console.WriteLine(x);
    }
}