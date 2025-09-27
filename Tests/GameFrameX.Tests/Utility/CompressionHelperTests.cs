// ==========================================================================================
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

using System;
using System.Linq;
using System.Text;
using Xunit;
using GameFrameX.Utility;

namespace GameFrameX.Tests.Utility
{
    /// <summary>
    /// CompressionHelper 类的单元测试
    /// </summary>
    public class CompressionHelperTests
    {
        #region Compress 方法测试

        /// <summary>
        /// 测试压缩空字节数组
        /// </summary>
        [Fact]
        public void Compress_WithEmptyArray_ShouldReturnEmptyArray()
        {
            // Arrange
            var emptyBytes = Array.Empty<byte>();

            // Act
            var result = CompressionHelper.Compress(emptyBytes);

            // Assert
            Assert.Same(emptyBytes, result);
        }

        /// <summary>
        /// 测试压缩null参数应抛出异常
        /// </summary>
        [Fact]
        public void Compress_WithNullInput_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => CompressionHelper.Compress(null));
        }

        /// <summary>
        /// 测试压缩有效数据
        /// </summary>
        [Fact]
        public void Compress_WithValidData_ShouldReturnCompressedData()
        {
            // Arrange
            var originalText = "This is a test string for compression. It should be compressed to a smaller size when using deflate algorithm.";
            var originalBytes = Encoding.UTF8.GetBytes(originalText);

            // Act
            var compressedBytes = CompressionHelper.Compress(originalBytes);

            // Assert
            Assert.NotNull(compressedBytes);
            Assert.True(compressedBytes.Length < originalBytes.Length, "压缩后的数据应该比原始数据小");
        }

        /// <summary>
        /// 测试压缩单字节数据
        /// </summary>
        [Fact]
        public void Compress_WithSingleByte_ShouldReturnValidResult()
        {
            // Arrange
            var singleByte = new byte[] { 42 };

            // Act
            var result = CompressionHelper.Compress(singleByte);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }

        /// <summary>
        /// 测试压缩重复数据
        /// </summary>
        [Fact]
        public void Compress_WithRepeatedData_ShouldAchieveGoodCompression()
        {
            // Arrange
            var repeatedData = new byte[1000];
            Array.Fill(repeatedData, (byte)65); // 填充相同的字节

            // Act
            var compressedData = CompressionHelper.Compress(repeatedData);

            // Assert
            Assert.NotNull(compressedData);
            Assert.True(compressedData.Length < repeatedData.Length / 10, "重复数据应该有很好的压缩率");
        }

        #endregion

        #region Decompress 方法测试

        /// <summary>
        /// 测试解压缩空字节数组
        /// </summary>
        [Fact]
        public void Decompress_WithEmptyArray_ShouldReturnEmptyArray()
        {
            // Arrange
            var emptyBytes = Array.Empty<byte>();

            // Act
            var result = CompressionHelper.Decompress(emptyBytes);

            // Assert
            Assert.Same(emptyBytes, result);
        }

        /// <summary>
        /// 测试解压缩null参数应抛出异常
        /// </summary>
        [Fact]
        public void Decompress_WithNullInput_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => CompressionHelper.Decompress(null));
        }

        /// <summary>
        /// 测试解压缩无效数据应返回原始数据
        /// </summary>
        [Fact]
        public void Decompress_WithInvalidData_ShouldReturnOriginalData()
        {
            // Arrange
            var invalidData = new byte[] { 1, 2, 3, 4, 5 }; // 无效的压缩数据

            // Act
            var result = CompressionHelper.Decompress(invalidData);

            // Assert
            Assert.Equal(invalidData, result);
        }

        #endregion

        #region 压缩和解压缩往返测试

        /// <summary>
        /// 测试压缩和解压缩的往返操作
        /// </summary>
        [Fact]
        public void CompressAndDecompress_RoundTrip_ShouldRestoreOriginalData()
        {
            // Arrange
            var originalText = "Hello, World! This is a test string for compression and decompression round trip test.";
            var originalBytes = Encoding.UTF8.GetBytes(originalText);

            // Act
            var compressedBytes = CompressionHelper.Compress(originalBytes);
            var decompressedBytes = CompressionHelper.Decompress(compressedBytes);
            var restoredText = Encoding.UTF8.GetString(decompressedBytes);

            // Assert
            Assert.Equal(originalBytes, decompressedBytes);
            Assert.Equal(originalText, restoredText);
        }

        /// <summary>
        /// 测试二进制数据的压缩和解压缩往返
        /// </summary>
        [Fact]
        public void CompressAndDecompress_WithBinaryData_ShouldRestoreOriginalData()
        {
            // Arrange
            var originalData = new byte[256];
            for (int i = 0; i < originalData.Length; i++)
            {
                originalData[i] = (byte)(i % 256);
            }

            // Act
            var compressedData = CompressionHelper.Compress(originalData);
            var decompressedData = CompressionHelper.Decompress(compressedData);

            // Assert
            Assert.Equal(originalData, decompressedData);
        }

        /// <summary>
        /// 测试大数据的压缩和解压缩往返
        /// </summary>
        [Fact]
        public void CompressAndDecompress_WithLargeData_ShouldRestoreOriginalData()
        {
            // Arrange
            var largeText = string.Join("", Enumerable.Repeat("This is a large text for testing compression. ", 1000));
            var originalBytes = Encoding.UTF8.GetBytes(largeText);

            // Act
            var compressedBytes = CompressionHelper.Compress(originalBytes);
            var decompressedBytes = CompressionHelper.Decompress(compressedBytes);
            var restoredText = Encoding.UTF8.GetString(decompressedBytes);

            // Assert
            Assert.Equal(originalBytes, decompressedBytes);
            Assert.Equal(largeText, restoredText);
            Assert.True(compressedBytes.Length < originalBytes.Length, "大数据应该被有效压缩");
        }

        /// <summary>
        /// 测试空字符串的压缩和解压缩往返
        /// </summary>
        [Fact]
        public void CompressAndDecompress_WithEmptyString_ShouldRestoreOriginalData()
        {
            // Arrange
            var emptyString = string.Empty;
            var originalBytes = Encoding.UTF8.GetBytes(emptyString);

            // Act
            var compressedBytes = CompressionHelper.Compress(originalBytes);
            var decompressedBytes = CompressionHelper.Decompress(compressedBytes);
            var restoredText = Encoding.UTF8.GetString(decompressedBytes);

            // Assert
            Assert.Equal(originalBytes, decompressedBytes);
            Assert.Equal(emptyString, restoredText);
        }

        /// <summary>
        /// 测试Unicode字符的压缩和解压缩往返
        /// </summary>
        [Fact]
        public void CompressAndDecompress_WithUnicodeData_ShouldRestoreOriginalData()
        {
            // Arrange
            var unicodeText = "你好世界！🌍 Hello World! Привет мир! こんにちは世界！";
            var originalBytes = Encoding.UTF8.GetBytes(unicodeText);

            // Act
            var compressedBytes = CompressionHelper.Compress(originalBytes);
            var decompressedBytes = CompressionHelper.Decompress(compressedBytes);
            var restoredText = Encoding.UTF8.GetString(decompressedBytes);

            // Assert
            Assert.Equal(originalBytes, decompressedBytes);
            Assert.Equal(unicodeText, restoredText);
        }

        #endregion

        #region 性能和边界测试

        /// <summary>
        /// 测试压缩效率
        /// </summary>
        [Fact]
        public void Compress_ShouldAchieveReasonableCompressionRatio()
        {
            // Arrange
            var repetitiveText = string.Join("", Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 100));
            var originalBytes = Encoding.UTF8.GetBytes(repetitiveText);

            // Act
            var compressedBytes = CompressionHelper.Compress(originalBytes);

            // Assert
            var compressionRatio = (double)compressedBytes.Length / originalBytes.Length;
            Assert.True(compressionRatio < 0.5, $"压缩率应该小于50%，实际压缩率: {compressionRatio:P2}");
        }

        /// <summary>
        /// 测试随机数据的压缩（通常压缩效果不佳）
        /// </summary>
        [Fact]
        public void Compress_WithRandomData_ShouldStillWork()
        {
            // Arrange
            var random = new Random(42); // 使用固定种子确保测试可重复
            var randomData = new byte[1000];
            random.NextBytes(randomData);

            // Act
            var compressedData = CompressionHelper.Compress(randomData);
            var decompressedData = CompressionHelper.Decompress(compressedData);

            // Assert
            Assert.NotNull(compressedData);
            Assert.Equal(randomData, decompressedData);
        }

        #endregion
    }
}