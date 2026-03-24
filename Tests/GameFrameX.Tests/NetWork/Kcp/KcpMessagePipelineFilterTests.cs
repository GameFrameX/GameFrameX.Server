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

using System.Net;
using GameFrameX.NetWork.Kcp;
using Xunit;

namespace GameFrameX.Tests.NetWork.Kcp;

/// <summary>
/// KcpMessagePipelineFilter 单元测试 / KcpMessagePipelineFilter unit tests
/// </summary>
public class KcpMessagePipelineFilterTests
{
    private readonly KcpMessagePipelineFilter _filter;

    public KcpMessagePipelineFilterTests()
    {
        _filter = new KcpMessagePipelineFilter();
    }

    #region TryParseHeader 测试 / TryParseHeader Tests

    /// <summary>
    /// 测试解析有效的消息头 / Test parsing valid message header
    /// </summary>
    [Theory]
    [InlineData(4, new byte[] { 0x00, 0x00, 0x00, 0x04 })]
    [InlineData(8, new byte[] { 0x00, 0x00, 0x00, 0x08 })]
    [InlineData(100, new byte[] { 0x00, 0x00, 0x00, 0x64 })]
    [InlineData(256, new byte[] { 0x00, 0x00, 0x01, 0x00 })]
    public void TryParseHeader_ValidBuffer_ShouldReturnTrue(int expectedLength, byte[] headerBytes)
    {
        // Act
        var result = _filter.TryParseHeader(headerBytes, out var totalLength);

        // Assert
        Assert.True(result);
        Assert.Equal(expectedLength, totalLength);
    }

    /// <summary>
    /// 测试解析过短的缓冲区 / Test parsing buffer that is too short
    /// </summary>
    [Theory]
    [InlineData(new byte[] { })]
    [InlineData(new byte[] { 0x00 })]
    [InlineData(new byte[] { 0x00, 0x00 })]
    [InlineData(new byte[] { 0x00, 0x00, 0x00 })]
    public void TryParseHeader_TooShortBuffer_ShouldReturnFalse(byte[] headerBytes)
    {
        // Act
        var result = _filter.TryParseHeader(headerBytes, out var totalLength);

        // Assert
        Assert.False(result);
        Assert.Equal(0, totalLength);
    }

    /// <summary>
    /// 测试解析长度为0的消息头 / Test parsing header with zero length
    /// </summary>
    [Fact]
    public void TryParseHeader_ZeroLength_ShouldReturnFalse()
    {
        // Arrange
        var headerBytes = new byte[] { 0x00, 0x00, 0x00, 0x00 };

        // Act
        var result = _filter.TryParseHeader(headerBytes, out var totalLength);

        // Assert
        Assert.False(result);
        Assert.Equal(0, totalLength);
    }

    #endregion

    #region ParseMessage 测试 / ParseMessage Tests

    /// <summary>
    /// 测试解析过短的消息 / Test parsing message that is too short
    /// </summary>
    [Fact]
    public void ParseMessage_TooShortBuffer_ShouldReturnNull()
    {
        // Arrange
        var buffer = new byte[] { 0x00, 0x00 };

        // Act
        var message = _filter.ParseMessage(buffer);

        // Assert
        Assert.Null(message);
    }

    /// <summary>
    /// 测试解析长度不匹配的消息 / Test parsing message with mismatched length
    /// </summary>
    [Fact]
    public void ParseMessage_MismatchedLength_ShouldReturnNull()
    {
        // Arrange - 头部声明长度为100，但实际只有8字节 / Header claims length 100, but only 8 bytes provided
        var buffer = new byte[] { 0x00, 0x00, 0x00, 0x64, 0x01, 0x02, 0x03, 0x04 };

        // Act
        var message = _filter.ParseMessage(buffer);

        // Assert
        Assert.Null(message);
    }

    #endregion

    #region Filter 测试 / Filter Tests

    /// <summary>
    /// 测试过滤空会话 / Test filtering empty session
    /// </summary>
    [Fact]
    public void Filter_EmptySession_ShouldReturnEmptyList()
    {
        // Arrange
        var options = new KcpOptions();
        using var manager = new KcpSessionManager(options, (data, ep) => { });
        var endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
        var session = manager.GetOrCreateSession(endpoint);

        // Act - 没有输入数据，应该返回空列表 / No data input, should return empty list
        var messages = _filter.Filter(session);

        // Assert
        Assert.Empty(messages);
    }

    #endregion

    #region 大端序测试 / Big-Endian Tests

    /// <summary>
    /// 测试大端序读取 / Test big-endian reading
    /// </summary>
    [Fact]
    public void ReadInt32BigEndian_ShouldReadCorrectly()
    {
        // Arrange - 大端序表示 305419896 (0x12345678)
        var buffer = new byte[] { 0x12, 0x34, 0x56, 0x78 };

        // Act
        var result = _filter.TryParseHeader(buffer, out var value);

        // Assert
        Assert.True(result);
        Assert.Equal(0x12345678, value);
    }

    /// <summary>
    /// 测试大端序读取最大值 / Test big-endian reading max value
    /// </summary>
    [Fact]
    public void ReadInt32BigEndian_MaxValue_ShouldReadCorrectly()
    {
        // Arrange - 大端序表示 2147483647 (0x7FFFFFFF)
        var buffer = new byte[] { 0x7F, 0xFF, 0xFF, 0xFF };

        // Act
        var result = _filter.TryParseHeader(buffer, out var value);

        // Assert
        Assert.True(result);
        Assert.Equal(2147483647, value);
    }

    #endregion
}
