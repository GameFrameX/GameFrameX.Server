using System.Text;
using Xunit;
using GameFrameX.Utility;

namespace GameFrameX.Tests.Utility;

/// <summary>
/// ConsoleHelper 类的单元测试
/// </summary>
public class ConsoleHelperTests
{
    /// <summary>
    /// 测试 ConsoleLogo 方法 - 验证输出内容
    /// </summary>
    [Fact]
    public void ConsoleLogo_ShouldOutputCorrectContent()
    {
        // Arrange
        var originalOut = Console.Out;
        var stringBuilder = new StringBuilder();
        var stringWriter = new StringWriter(stringBuilder);
        
        try
        {
            Console.SetOut(stringWriter);

            // Act
            ConsoleHelper.ConsoleLogo();

            // Assert
            var output = stringBuilder.ToString();
            
            // 验证输出包含预期的内容
            Assert.Contains("GameFrameX", output);
            Assert.Contains("https://github.com/GameFrameX/GameFrameX", output);
            Assert.Contains("https://gameframex.doc.alianblank.com", output);
            Assert.Contains("项目主页", output);
            Assert.Contains("在线文档", output);
            
            // 验证输出包含ASCII艺术字符
            Assert.Contains("_____", output);
            Assert.Contains("|  __ \\", output);
            Assert.Contains("|  \\/", output);
            
            // 验证输出不为空
            Assert.False(string.IsNullOrWhiteSpace(output));
        }
        finally
        {
            // 恢复原始输出流
            Console.SetOut(originalOut);
            stringWriter.Dispose();
        }
    }

    /// <summary>
    /// 测试 ConsoleLogo 方法 - 验证输出行数
    /// </summary>
    [Fact]
    public void ConsoleLogo_ShouldOutputCorrectNumberOfLines()
    {
        // Arrange
        var originalOut = Console.Out;
        var stringBuilder = new StringBuilder();
        var stringWriter = new StringWriter(stringBuilder);
        
        try
        {
            Console.SetOut(stringWriter);

            // Act
            ConsoleHelper.ConsoleLogo();

            // Assert
            var output = stringBuilder.ToString();
            var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            
            // 验证输出行数（包括空行）
            // 根据源代码，应该有11行输出
            Assert.True(lines.Length >= 11, $"Expected at least 11 lines, but got {lines.Length}");
        }
        finally
        {
            // 恢复原始输出流
            Console.SetOut(originalOut);
            stringWriter.Dispose();
        }
    }

    /// <summary>
    /// 测试 ConsoleLogo 方法 - 验证不会抛出异常
    /// </summary>
    [Fact]
    public void ConsoleLogo_ShouldNotThrowException()
    {
        // Arrange
        var originalOut = Console.Out;
        var stringBuilder = new StringBuilder();
        var stringWriter = new StringWriter(stringBuilder);
        
        try
        {
            Console.SetOut(stringWriter);

            // Act & Assert
            var exception = Record.Exception(() => ConsoleHelper.ConsoleLogo());
            Assert.Null(exception);
        }
        finally
        {
            // 恢复原始输出流
            Console.SetOut(originalOut);
            stringWriter.Dispose();
        }
    }

    /// <summary>
    /// 测试 ConsoleLogo 方法 - 验证输出格式一致性
    /// </summary>
    [Fact]
    public void ConsoleLogo_ShouldProduceConsistentOutput()
    {
        // Arrange
        var originalOut = Console.Out;
        var outputs = new List<string>();
        
        try
        {
            // 多次调用并收集输出
            for (int i = 0; i < 3; i++)
            {
                var stringBuilder = new StringBuilder();
                var stringWriter = new StringWriter(stringBuilder);
                Console.SetOut(stringWriter);
                
                ConsoleHelper.ConsoleLogo();
                
                outputs.Add(stringBuilder.ToString());
                stringWriter.Dispose();
            }

            // Assert
            // 验证所有输出都相同
            for (int i = 1; i < outputs.Count; i++)
            {
                Assert.Equal(outputs[0], outputs[i]);
            }
        }
        finally
        {
            // 恢复原始输出流
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    /// 测试 ConsoleLogo 方法 - 验证输出包含正确的URL格式
    /// </summary>
    [Fact]
    public void ConsoleLogo_ShouldContainValidUrls()
    {
        // Arrange
        var originalOut = Console.Out;
        var stringBuilder = new StringBuilder();
        var stringWriter = new StringWriter(stringBuilder);
        
        try
        {
            Console.SetOut(stringWriter);

            // Act
            ConsoleHelper.ConsoleLogo();

            // Assert
            var output = stringBuilder.ToString();
            
            // 验证包含有效的URL格式
            Assert.Contains("https://", output);
            Assert.Contains(".com", output);
            Assert.Contains("github.com", output);
            Assert.Contains("alianblank.com", output);
        }
        finally
        {
            // 恢复原始输出流
            Console.SetOut(originalOut);
            stringWriter.Dispose();
        }
    }
}