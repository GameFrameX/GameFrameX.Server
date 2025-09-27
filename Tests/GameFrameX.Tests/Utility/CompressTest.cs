﻿// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

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