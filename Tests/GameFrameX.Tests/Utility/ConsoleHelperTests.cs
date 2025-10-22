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

using System.Text;
using GameFrameX.Foundation.Utility;
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