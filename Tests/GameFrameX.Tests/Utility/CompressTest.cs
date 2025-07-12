using System.IO.Compression;
using System.Text;
using GameFrameX.Utility;
using Xunit;

namespace GameFrameX.Tests;

public class CompressTest
{
    private byte[] bytes;
    private byte[] comData;

    public CompressTest()
    {
        var str = "Hello World!";
        bytes = Encoding.UTF8.GetBytes(str);
        comData = CompressionHelper.Compress(bytes);
    }

    [Fact]
    public void TestCompress()
    {
        var str = "Hello World!";
        var bytes = Encoding.UTF8.GetBytes(str);
        var comData = CompressionHelper.Compress(bytes);
        Assert.True(comData.Length > 0);
    }

    [Fact]
    public void TestDecompress()
    {
        var decompressedData = CompressionHelper.Decompress(comData);
        var decompressedStr = Encoding.UTF8.GetString(decompressedData);
        Assert.Equal("Hello World!", decompressedStr);
    }

    [Fact]
    public void TestCompressAndDecompress()
    {
        var originalStr = "This is a test string for compression and decompression.";
        var originalBytes = Encoding.UTF8.GetBytes(originalStr);
        
        var compressedBytes = CompressionHelper.Compress(originalBytes);
        var decompressedBytes = CompressionHelper.Decompress(compressedBytes);
        var decompressedStr = Encoding.UTF8.GetString(decompressedBytes);
        
        Assert.Equal(originalStr, decompressedStr);
    }

    [Fact]
    public void TestCompressNull()
    {
        Assert.Throws<ArgumentNullException>(() => CompressionHelper.Compress(null));
    }

    [Fact]
    public void TestDecompressNull()
    {
        Assert.Throws<ArgumentNullException>(() => CompressionHelper.Decompress(null));
    }

    [Fact]
    public void TestCompressEmptyArray()
    {
        var emptyArray = new byte[0];
        var result = CompressionHelper.Compress(emptyArray);
        Assert.Same(emptyArray, result);
    }

    [Fact]
    public void TestDecompressEmptyArray()
    {
        var emptyArray = new byte[0];
        var result = CompressionHelper.Decompress(emptyArray);
        Assert.Same(emptyArray, result);
    }
}