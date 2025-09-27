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
using System.Collections.Generic;
using System.Linq;
using Xunit;
using GameFrameX.Utility;

namespace GameFrameX.Tests.Utility
{
    /// <summary>
    /// IllegalWordDetection 类的单元测试
    /// </summary>
    public class IllegalWordDetectionTests
    {
        private readonly string[] _testBadWords = new[]
        {
            "敏感词",
            "违禁词",
            "测试词",
            "fuck",
            "shit",
            "damn",
            "badword",
            "test"
        };

        /// <summary>
        /// 初始化测试数据
        /// </summary>
        private void InitializeTestData()
        {
            IllegalWordDetection.Init(_testBadWords, false);
            // 等待初始化完成
            System.Threading.Thread.Sleep(100);
        }

        #region Init 方法测试

        /// <summary>
        /// 测试使用字符串数组初始化敏感词
        /// </summary>
        [Fact]
        public void Init_WithStringArray_ShouldInitializeSuccessfully()
        {
            // Arrange
            var badWords = new[] { "test1", "test2", "test3" };

            // Act & Assert
            // 不应该抛出异常
            IllegalWordDetection.Init(badWords, false);
        }

        /// <summary>
        /// 测试使用空数组初始化
        /// </summary>
        [Fact]
        public void Init_WithEmptyArray_ShouldNotThrow()
        {
            // Arrange
            var emptyArray = Array.Empty<string>();

            // Act & Assert
            IllegalWordDetection.Init(emptyArray, false);
        }

        /// <summary>
        /// 测试使用null数组初始化
        /// </summary>
        [Fact]
        public void Init_WithNullArray_ShouldNotThrow()
        {
            // Act & Assert
            IllegalWordDetection.Init((string[])null, false);
        }

        /// <summary>
        /// 测试后台线程初始化
        /// </summary>
        [Fact]
        public void Init_WithBackgroundThread_ShouldNotBlock()
        {
            // Arrange
            var badWords = new[] { "background1", "background2" };
            var startTime = DateTime.Now;

            // Act
            IllegalWordDetection.Init(badWords, true);
            var endTime = DateTime.Now;

            // Assert
            // 后台初始化应该很快返回
            Assert.True((endTime - startTime).TotalMilliseconds < 100);
        }

        #endregion

        #region HasBlockWords 方法测试

        /// <summary>
        /// 测试检测包含敏感词的文本
        /// </summary>
        [Fact]
        public void HasBlockWords_WithSensitiveWords_ShouldReturnTrue()
        {
            // Arrange
            InitializeTestData();
            var textWithBadWords = "这是一个包含敏感词的文本";

            // Act
            var result = IllegalWordDetection.HasBlockWords(textWithBadWords);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// 测试检测不包含敏感词的文本
        /// </summary>
        [Fact]
        public void HasBlockWords_WithoutSensitiveWords_ShouldReturnFalse()
        {
            // Arrange
            InitializeTestData();
            var cleanText = "这是一个干净的文本内容";

            // Act
            var result = IllegalWordDetection.HasBlockWords(cleanText);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// 测试检测空字符串
        /// </summary>
        [Fact]
        public void HasBlockWords_WithEmptyString_ShouldReturnFalse()
        {
            // Arrange
            InitializeTestData();

            // Act
            var result = IllegalWordDetection.HasBlockWords(string.Empty);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// 测试检测null字符串
        /// </summary>
        [Fact]
        public void HasBlockWords_WithNullString_ShouldReturnFalse()
        {
            // Arrange
            InitializeTestData();

            // Act
            var result = IllegalWordDetection.HasBlockWords(null);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// 测试检测英文敏感词
        /// </summary>
        [Fact]
        public void HasBlockWords_WithEnglishBadWords_ShouldReturnTrue()
        {
            // Arrange
            InitializeTestData();
            var textWithEnglishBadWords = "This is a fuck text";

            // Act
            var result = IllegalWordDetection.HasBlockWords(textWithEnglishBadWords);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// 测试大小写不敏感
        /// </summary>
        [Fact]
        public void HasBlockWords_CaseInsensitive_ShouldReturnTrue()
        {
            // Arrange
            InitializeTestData();
            var textWithUpperCase = "This is a FUCK text";
            var textWithMixedCase = "This is a FuCk text";

            // Act
            var result1 = IllegalWordDetection.HasBlockWords(textWithUpperCase);
            var result2 = IllegalWordDetection.HasBlockWords(textWithMixedCase);

            // Assert
            Assert.True(result1);
            Assert.True(result2);
        }

        #endregion

        #region Filter 方法测试

        /// <summary>
        /// 测试过滤包含敏感词的文本
        /// </summary>
        [Fact]
        public void Filter_WithSensitiveWords_ShouldReplaceWithMask()
        {
            // Arrange
            InitializeTestData();
            var textWithBadWords = "这是一个包含敏感词的文本";

            // Act
            var filteredText = IllegalWordDetection.Filter(textWithBadWords);

            // Assert
            Assert.NotEqual(textWithBadWords, filteredText);
            Assert.Contains("***", filteredText);
        }

        /// <summary>
        /// 测试使用自定义掩码字符过滤
        /// </summary>
        [Fact]
        public void Filter_WithCustomMask_ShouldReplaceWithCustomChar()
        {
            // Arrange
            InitializeTestData();
            var textWithBadWords = "这是一个包含敏感词的文本";
            var customMask = '#';

            // Act
            var filteredText = IllegalWordDetection.Filter(textWithBadWords, customMask);

            // Assert
            Assert.NotEqual(textWithBadWords, filteredText);
            Assert.Contains("###", filteredText);
        }

        /// <summary>
        /// 测试过滤不包含敏感词的文本
        /// </summary>
        [Fact]
        public void Filter_WithoutSensitiveWords_ShouldReturnOriginal()
        {
            // Arrange
            InitializeTestData();
            var cleanText = "这是一个干净的文本内容";

            // Act
            var filteredText = IllegalWordDetection.Filter(cleanText);

            // Assert
            Assert.Equal(cleanText, filteredText);
        }

        /// <summary>
        /// 测试过滤空字符串
        /// </summary>
        [Fact]
        public void Filter_WithEmptyString_ShouldReturnEmpty()
        {
            // Arrange
            InitializeTestData();

            // Act
            var filteredText = IllegalWordDetection.Filter(string.Empty);

            // Assert
            Assert.Equal(string.Empty, filteredText);
        }

        /// <summary>
        /// 测试过滤英文敏感词
        /// </summary>
        [Fact]
        public void Filter_WithEnglishBadWords_ShouldReplaceCorrectly()
        {
            // Arrange
            InitializeTestData();
            var textWithEnglishBadWords = "This is a fuck text";

            // Act
            var filteredText = IllegalWordDetection.Filter(textWithEnglishBadWords);

            // Assert
            Assert.NotEqual(textWithEnglishBadWords, filteredText);
            Assert.Contains("****", filteredText);
        }

        #endregion

        #region DetectIllegalWords 方法测试

        /// <summary>
        /// 测试检测敏感词并返回位置信息
        /// </summary>
        [Fact]
        public void DetectIllegalWords_WithSensitiveWords_ShouldReturnPositions()
        {
            // Arrange
            InitializeTestData();
            var textWithBadWords = "这是敏感词测试";

            // Act
            var hasIllegalWords = IllegalWordDetection.DetectIllegalWords(textWithBadWords, false, out var positions);

            // Assert
            Assert.True(hasIllegalWords);
            Assert.True(positions.Count > 0);
        }

        /// <summary>
        /// 测试检测多个敏感词
        /// </summary>
        [Fact]
        public void DetectIllegalWords_WithMultipleSensitiveWords_ShouldReturnAllPositions()
        {
            // Arrange
            InitializeTestData();
            var textWithMultipleBadWords = "敏感词和违禁词都在这里";

            // Act
            var hasIllegalWords = IllegalWordDetection.DetectIllegalWords(textWithMultipleBadWords, false, out var positions);

            // Assert
            Assert.True(hasIllegalWords);
            Assert.True(positions.Count >= 2);
        }

        /// <summary>
        /// 测试只返回第一个敏感词
        /// </summary>
        [Fact]
        public void DetectIllegalWords_ReturnFirstOnly_ShouldReturnOnlyFirst()
        {
            // Arrange
            InitializeTestData();
            var textWithMultipleBadWords = "敏感词和违禁词都在这里";

            // Act
            var hasIllegalWords = IllegalWordDetection.DetectIllegalWords(textWithMultipleBadWords, true, out var positions);

            // Assert
            Assert.True(hasIllegalWords);
            Assert.True(positions.Count <= 1);
        }

        /// <summary>
        /// 测试检测不包含敏感词的文本
        /// </summary>
        [Fact]
        public void DetectIllegalWords_WithoutSensitiveWords_ShouldReturnEmptyPositions()
        {
            // Arrange
            InitializeTestData();
            var cleanText = "这是一个干净的文本内容";

            // Act
            var hasIllegalWords = IllegalWordDetection.DetectIllegalWords(cleanText, false, out var positions);

            // Assert
            Assert.False(hasIllegalWords);
            Assert.Empty(positions);
        }

        #endregion

        #region 边界和性能测试

        /// <summary>
        /// 测试长文本的处理
        /// </summary>
        [Fact]
        public void IllegalWordDetection_WithLongText_ShouldHandleCorrectly()
        {
            // Arrange
            InitializeTestData();
            var longText = string.Join("", Enumerable.Repeat("这是一段很长的文本内容，用来测试性能。", 100)) + "敏感词";

            // Act
            var startTime = DateTime.Now;
            var hasIllegalWords = IllegalWordDetection.HasBlockWords(longText);
            var endTime = DateTime.Now;

            // Assert
            Assert.True(hasIllegalWords);
            Assert.True((endTime - startTime).TotalMilliseconds < 1000, "长文本处理应该在合理时间内完成");
        }

        /// <summary>
        /// 测试包含特殊字符的文本
        /// </summary>
        [Fact]
        public void IllegalWordDetection_WithSpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            InitializeTestData();
            var textWithSpecialChars = "这是@#$%敏感词^&*()测试";

            // Act
            var hasIllegalWords = IllegalWordDetection.HasBlockWords(textWithSpecialChars);
            var filteredText = IllegalWordDetection.Filter(textWithSpecialChars);

            // Assert
            Assert.True(hasIllegalWords);
            Assert.NotEqual(textWithSpecialChars, filteredText);
        }

        /// <summary>
        /// 测试Unicode字符的处理
        /// </summary>
        [Fact]
        public void IllegalWordDetection_WithUnicodeCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            InitializeTestData();
            var textWithUnicode = "这是🌍敏感词🚀测试";

            // Act
            var hasIllegalWords = IllegalWordDetection.HasBlockWords(textWithUnicode);

            // Assert
            Assert.True(hasIllegalWords);
        }

        /// <summary>
        /// 测试性能基准
        /// </summary>
        [Fact]
        public void IllegalWordDetection_PerformanceBenchmark_ShouldBeReasonable()
        {
            // Arrange
            InitializeTestData();
            var testText = "这是一个包含敏感词的测试文本，用来测试性能表现";
            const int iterations = 1000;

            // Act
            var startTime = DateTime.Now;
            for (int i = 0; i < iterations; i++)
            {
                IllegalWordDetection.HasBlockWords(testText);
            }
            var endTime = DateTime.Now;

            // Assert
            var totalTime = (endTime - startTime).TotalMilliseconds;
            var averageTime = totalTime / iterations;
            Assert.True(averageTime < 10, $"平均检测时间应该小于10ms，实际: {averageTime:F2}ms");
        }

        #endregion
    }
}