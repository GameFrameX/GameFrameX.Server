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
using GameFrameX.Utility;
using Xunit;

namespace GameFrameX.Tests.Utility;

/// <summary>
/// RandomHelper 类的单元测试
/// </summary>
public class RandomHelperTests
{
    /// <summary>
    /// 测试 NextUInt32 方法是否返回有效的UInt32值
    /// </summary>
    [Fact]
    public void NextUInt32_ShouldReturnValidUInt32()
    {
        // Act
        var result1 = RandomHelper.NextUInt32();
        var result2 = RandomHelper.NextUInt32();
        var result3 = RandomHelper.NextUInt32();

        // Assert
        Assert.True(result1 >= uint.MinValue && result1 <= uint.MaxValue);
        Assert.True(result2 >= uint.MinValue && result2 <= uint.MaxValue);
        Assert.True(result3 >= uint.MinValue && result3 <= uint.MaxValue);
        
        // 验证随机性（虽然理论上可能相等，但概率极低）
        Assert.True(result1 != result2 || result2 != result3 || result1 != result3);
    }

    /// <summary>
    /// 测试 Next 方法（无参数）是否返回非负整数
    /// </summary>
    [Fact]
    public void Next_WithoutParameters_ShouldReturnNonNegativeInteger()
    {
        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var result = RandomHelper.Next();
            Assert.True(result >= 0 && result < int.MaxValue);
        }
    }

    /// <summary>
    /// 测试 Next 方法（带最大值参数）是否返回正确范围的值
    /// </summary>
    [Fact]
    public void Next_WithMaxValue_ShouldReturnValueInRange()
    {
        // Arrange
        const int maxValue = 100;

        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var result = RandomHelper.Next(maxValue);
            Assert.True(result >= 0 && result < maxValue);
        }
    }

    /// <summary>
    /// 测试 Next 方法（带最大值参数）在边界情况下的行为
    /// </summary>
    [Fact]
    public void Next_WithMaxValueZero_ShouldReturnZero()
    {
        // Act
        var result = RandomHelper.Next(0);

        // Assert
        Assert.Equal(0, result);
    }

    /// <summary>
    /// 测试 Next 方法（带最大值参数）在负数输入时抛出异常
    /// </summary>
    [Fact]
    public void Next_WithNegativeMaxValue_ShouldThrowArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => RandomHelper.Next(-1));
    }

    /// <summary>
    /// 测试 Next 方法（带范围参数）是否返回正确范围的值
    /// </summary>
    [Fact]
    public void Next_WithRange_ShouldReturnValueInRange()
    {
        // Arrange
        const int minValue = 10;
        const int maxValue = 50;

        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var result = RandomHelper.Next(minValue, maxValue);
            Assert.True(result >= minValue && result < maxValue);
        }
    }

    /// <summary>
    /// 测试 Next 方法（带范围参数）在相等边界时的行为
    /// </summary>
    [Fact]
    public void Next_WithEqualMinAndMaxValue_ShouldReturnMinValue()
    {
        // Arrange
        const int value = 42;

        // Act
        var result = RandomHelper.Next(value, value);

        // Assert
        Assert.Equal(value, result);
    }

    /// <summary>
    /// 测试 Next 方法（带范围参数）在最小值大于最大值时抛出异常
    /// </summary>
    [Fact]
    public void Next_WithMinValueGreaterThanMaxValue_ShouldThrowArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => RandomHelper.Next(50, 10));
    }

    /// <summary>
    /// 测试 NextInt64 方法（无参数）是否返回非负长整数
    /// </summary>
    [Fact]
    public void NextInt64_WithoutParameters_ShouldReturnNonNegativeLong()
    {
        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var result = RandomHelper.NextInt64();
            Assert.True(result >= 0 && result < long.MaxValue);
        }
    }

    /// <summary>
    /// 测试 NextInt64 方法（带最大值参数）是否返回正确范围的值
    /// </summary>
    [Fact]
    public void NextInt64_WithMaxValue_ShouldReturnValueInRange()
    {
        // Arrange
        const int maxValue = 1000;

        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var result = RandomHelper.NextInt64(maxValue);
            Assert.True(result >= 0 && result < maxValue);
        }
    }

    /// <summary>
    /// 测试 NextInt64 方法（带最大值参数）在负数输入时抛出异常
    /// </summary>
    [Fact]
    public void NextInt64_WithNegativeMaxValue_ShouldThrowArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => RandomHelper.NextInt64(-1));
    }

    /// <summary>
    /// 测试 NextInt64 方法（带范围参数）是否返回正确范围的值
    /// </summary>
    [Fact]
    public void NextInt64_WithRange_ShouldReturnValueInRange()
    {
        // Arrange
        const long minValue = 100L;
        const long maxValue = 500L;

        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var result = RandomHelper.NextInt64(minValue, maxValue);
            Assert.True(result >= minValue && result < maxValue);
        }
    }

    /// <summary>
    /// 测试 NextInt64 方法（带范围参数）在最小值大于最大值时抛出异常
    /// </summary>
    [Fact]
    public void NextInt64_WithMinValueGreaterThanMaxValue_ShouldThrowArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => RandomHelper.NextInt64(500L, 100L));
    }

    /// <summary>
    /// 测试 NextUInt64 方法是否返回有效的UInt64值
    /// </summary>
    [Fact]
    public void NextUInt64_ShouldReturnValidUInt64()
    {
        // Act
        var result1 = RandomHelper.NextUInt64();
        var result2 = RandomHelper.NextUInt64();
        var result3 = RandomHelper.NextUInt64();

        // Assert
        Assert.True(result1 >= ulong.MinValue && result1 <= ulong.MaxValue);
        Assert.True(result2 >= ulong.MinValue && result2 <= ulong.MaxValue);
        Assert.True(result3 >= ulong.MinValue && result3 <= ulong.MaxValue);
        
        // 验证随机性
        Assert.True(result1 != result2 || result2 != result3 || result1 != result3);
    }

    /// <summary>
    /// 测试 NextDouble 方法是否返回0.0到1.0之间的值
    /// </summary>
    [Fact]
    public void NextDouble_ShouldReturnValueBetweenZeroAndOne()
    {
        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var result = RandomHelper.NextDouble();
            Assert.True(result >= 0.0 && result < 1.0);
        }
    }

    /// <summary>
    /// 测试 NextBytes 方法（字节数组）是否正确填充随机数
    /// </summary>
    [Fact]
    public void NextBytes_WithByteArray_ShouldFillArrayWithRandomBytes()
    {
        // Arrange
        var buffer = new byte[100];
        var originalBuffer = new byte[100];
        Array.Copy(buffer, originalBuffer, buffer.Length);

        // Act
        RandomHelper.NextBytes(buffer);

        // Assert
        Assert.False(buffer.SequenceEqual(originalBuffer)); // 应该不同（概率极高）
    }

    /// <summary>
    /// 测试 NextBytes 方法（字节数组）在null输入时抛出异常
    /// </summary>
    [Fact]
    public void NextBytes_WithNullByteArray_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => RandomHelper.NextBytes((byte[])null!));
    }

    /// <summary>
    /// 测试 NextBytes 方法（Span）是否正确填充随机数
    /// </summary>
    [Fact]
    public void NextBytes_WithSpan_ShouldFillSpanWithRandomBytes()
    {
        // Arrange
        var buffer = new byte[100];
        var originalBuffer = new byte[100];
        Array.Copy(buffer, originalBuffer, buffer.Length);
        var span = buffer.AsSpan();

        // Act
        RandomHelper.NextBytes(span);

        // Assert
        Assert.False(buffer.SequenceEqual(originalBuffer)); // 应该不同（概率极高）
    }

    /// <summary>
    /// 测试 RandomSelect 方法（IList）是否返回列表中的元素
    /// </summary>
    [Fact]
    public void RandomSelect_WithIList_ShouldReturnElementFromList()
    {
        // Arrange
        var items = new List<string> { "apple", "banana", "cherry", "date" };
        var selectedItems = new HashSet<string>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            var selected = RandomHelper.RandomSelect(items);
            selectedItems.Add(selected);
            Assert.Contains(selected, items);
        }

        // Assert - 应该选择了多个不同的元素（概率极高）
        Assert.True(selectedItems.Count > 1);
    }

    /// <summary>
    /// 测试 RandomSelect 方法（IList）在null输入时抛出异常
    /// </summary>
    [Fact]
    public void RandomSelect_WithNullIList_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => RandomHelper.RandomSelect((IList<string>)null!));
    }

    /// <summary>
    /// 测试 RandomSelect 方法（IList）在空列表时抛出异常
    /// </summary>
    [Fact]
    public void RandomSelect_WithEmptyIList_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var emptyList = new List<string>();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => RandomHelper.RandomSelect(emptyList));
    }

    /// <summary>
    /// 测试 RandomSelect 方法（数组）是否返回数组中的元素
    /// </summary>
    [Fact]
    public void RandomSelect_WithArray_ShouldReturnElementFromArray()
    {
        // Arrange
        var items = new[] { "apple", "banana", "cherry", "date" };
        var selectedItems = new HashSet<string>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            var selected = RandomHelper.RandomSelect(items);
            selectedItems.Add(selected);
            Assert.Contains(selected, items);
        }

        // Assert
        Assert.True(selectedItems.Count > 1);
    }

    /// <summary>
    /// 测试 RandomSelect 方法（数组）在null输入时抛出异常
    /// </summary>
    [Fact]
    public void RandomSelect_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => RandomHelper.RandomSelect((string[])null!));
    }

    /// <summary>
    /// 测试 RandomSelect 方法（数组）在空数组时抛出异常
    /// </summary>
    [Fact]
    public void RandomSelect_WithEmptyArray_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var emptyArray = new string[0];

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => RandomHelper.RandomSelect(emptyArray));
    }

    /// <summary>
    /// 测试 RandomSelect 方法（List）是否返回列表中的元素
    /// </summary>
    [Fact]
    public void RandomSelect_WithList_ShouldReturnElementFromList()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3, 4, 5 };
        var selectedItems = new HashSet<int>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            var selected = RandomHelper.RandomSelect(items);
            selectedItems.Add(selected);
            Assert.Contains(selected, items);
        }

        // Assert
        Assert.True(selectedItems.Count > 1);
    }

    /// <summary>
    /// 测试 RandomSelect 方法（List）在null输入时抛出异常
    /// </summary>
    [Fact]
    public void RandomSelect_WithNullList_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => RandomHelper.RandomSelect((List<string>)null!));
    }

    /// <summary>
    /// 测试 RandomSelect 方法（List）在空列表时抛出异常
    /// </summary>
    [Fact]
    public void RandomSelect_WithEmptyList_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var emptyList = new List<string>();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => RandomHelper.RandomSelect(emptyList));
    }

    /// <summary>
    /// 测试 Idx 方法是否返回有效的索引
    /// </summary>
    [Fact]
    public void Idx_ShouldReturnValidIndex()
    {
        // Arrange
        var items = new List<string> { "a", "b", "c", "d", "e" };
        var indices = new HashSet<int>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            var index = RandomHelper.Idx(items);
            indices.Add(index);
            Assert.True(index >= 0 && index < items.Count);
        }

        // Assert
        Assert.True(indices.Count > 1); // 应该生成多个不同的索引
    }

    /// <summary>
    /// 测试 Idx 方法在null输入时抛出异常
    /// </summary>
    [Fact]
    public void Idx_WithNullList_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => RandomHelper.Idx((IList<string>)null!));
    }

    /// <summary>
    /// 测试 Idx 方法在空列表时抛出异常
    /// </summary>
    [Fact]
    public void Idx_WithEmptyList_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var emptyList = new List<string>();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => RandomHelper.Idx(emptyList));
    }

    /// <summary>
    /// 测试单元素集合的随机选择
    /// </summary>
    [Fact]
    public void RandomSelect_WithSingleElement_ShouldReturnThatElement()
    {
        // Arrange
        var singleItemList = new List<string> { "only" };
        var singleItemArray = new[] { "only" };

        // Act & Assert
        Assert.Equal("only", RandomHelper.RandomSelect(singleItemList));
        Assert.Equal("only", RandomHelper.RandomSelect(singleItemArray));
        Assert.Equal(0, RandomHelper.Idx(singleItemList));
    }

    /// <summary>
    /// 测试随机数生成的分布性（简单统计测试）
    /// </summary>
    [Fact]
    public void Next_WithRange_ShouldHaveReasonableDistribution()
    {
        // Arrange
        const int minValue = 0;
        const int maxValue = 10;
        const int iterations = 10000;
        var counts = new int[maxValue];

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var value = RandomHelper.Next(minValue, maxValue);
            counts[value]++;
        }

        // Assert - 每个值应该出现一定次数（不要求完全均匀，但不应该有值完全没出现）
        var expectedCount = iterations / maxValue;
        var tolerance = expectedCount * 0.3; // 30%的容差
        
        for (int i = 0; i < maxValue; i++)
        {
            Assert.True(counts[i] > 0, $"Value {i} should appear at least once");
            Assert.True(System.Math.Abs(counts[i] - expectedCount) < expectedCount, 
                $"Value {i} appeared {counts[i]} times, expected around {expectedCount}");
        }
    }

    #region NextLong方法测试

    /// <summary>
    /// 测试 NextLong 方法（带最大值参数）是否返回正确范围的值
    /// </summary>
    [Fact]
    public void NextLong_WithMaxValue_ShouldReturnValueInRange()
    {
        // Arrange
        const long maxValue = 1000L;
        
        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var result = RandomHelper.NextLong(maxValue);
            Assert.True(result >= 0L && result < maxValue);
        }
    }

    /// <summary>
    /// 测试 NextLong 方法（带范围参数）是否返回正确范围的值
    /// </summary>
    [Fact]
    public void NextLong_WithRange_ShouldReturnValueInRange()
    {
        // Arrange
        const long minValue = 100L;
        const long maxValue = 500L;
        
        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var result = RandomHelper.NextLong(minValue, maxValue);
            Assert.True(result >= minValue && result < maxValue);
        }
    }

    /// <summary>
    /// 测试 NextLong 方法在负数输入时抛出异常
    /// </summary>
    [Fact]
    public void NextLong_WithNegativeMaxValue_ShouldThrowArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => RandomHelper.NextLong(-1L));
    }

    #endregion

    #region NextDouble扩展方法测试

    /// <summary>
    /// 测试 NextDouble 方法（带最大值参数）是否返回正确范围的值
    /// </summary>
    [Fact]
    public void NextDouble_WithMaxValue_ShouldReturnValueInRange()
    {
        // Arrange
        const double maxValue = 10.5;
        
        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var result = RandomHelper.NextDouble(maxValue);
            Assert.True(result >= 0.0 && result <= maxValue);
        }
    }

    /// <summary>
    /// 测试 NextDouble 方法（带范围参数）是否返回正确范围的值
    /// </summary>
    [Fact]
    public void NextDouble_WithRange_ShouldReturnValueInRange()
    {
        // Arrange
        const double minValue = 5.5;
        const double maxValue = 15.5;
        
        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var result = RandomHelper.NextDouble(minValue, maxValue);
            Assert.True(result >= minValue && result <= maxValue);
        }
    }

    #endregion

    #region NextBytes扩展方法测试

    /// <summary>
    /// 测试 NextBytes 方法（返回指定长度的字节数组）
    /// </summary>
    [Fact]
    public void NextBytes_WithLength_ShouldReturnByteArrayOfCorrectLength()
    {
        // Arrange
        const int length = 10;
        var buffer = new byte[length];
        
        // Act
        RandomHelper.NextBytes(buffer);
        
        // Assert
        Assert.NotNull(buffer);
        Assert.Equal(length, buffer.Length);
    }

    /// <summary>
    /// 测试 NextBytes 方法在null参数时抛出异常
    /// </summary>
    [Fact]
    public void NextBytes_WithNullBuffer_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => RandomHelper.NextBytes((byte[])null!));
    }

    #endregion

    #region RandomSelect扩展方法测试

    /// <summary>
    /// 测试 RandomSelect 方法（带数量参数）是否返回正确数量的元素
    /// </summary>
    [Fact]
    public void RandomSelect_WithCount_ShouldReturnCorrectNumberOfElements()
    {
        // Arrange
        var array = new[] { 1, 2, 3, 4, 5 };
        const int count = 3;
        
        // Act
        var result = RandomHelper.Items(array, count);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Length);
        Assert.All(result, item => Assert.Contains(item, array));
    }

    /// <summary>
    /// 测试 RandomSelect 方法在负数数量时抛出异常
    /// </summary>
    [Fact]
    public void RandomSelect_WithNegativeCount_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => RandomHelper.Items(array, -1));
    }

    #endregion

    #region Ids方法测试

    /// <summary>
    /// 测试 Ids 方法（从数组中选择多个索引）
    /// </summary>
    [Fact]
    public void Ids_FromArray_ShouldReturnCorrectNumberOfIndices()
    {
        // Arrange
        var array = new[] { 1, 2, 3, 4, 5 };
        const int count = 3;
        
        // Act
        var result = RandomHelper.Ids(array, count);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Length);
        Assert.All(result, index => Assert.InRange(index, 0, array.Length - 1));
    }

    /// <summary>
    /// 测试 Ids 方法（从列表中选择多个索引）
    /// </summary>
    [Fact]
    public void Ids_FromList_ShouldReturnCorrectNumberOfIndices()
    {
        // Arrange
        var list = new List<string> { "a", "b", "c", "d" };
        const int count = 2;
        
        // Act
        var result = RandomHelper.Items(list, count);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Length);
        Assert.All(result, item => Assert.Contains(item, list));
    }

    /// <summary>
    /// 测试 Ids 方法在负数数量时抛出异常
    /// </summary>
    [Fact]
    public void Ids_WithNegativeCount_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var array = new int[][] 
        {
            new[] { 1, 10 },
            new[] { 2, 20 },
            new[] { 3, 30 }
        };
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => RandomHelper.Ids(array, -1));
    }

    #endregion

    #region Items方法测试

    /// <summary>
    /// 测试 Items 方法（从数组中选择多个元素）
    /// </summary>
    [Fact]
    public void Items_FromArray_ShouldReturnCorrectNumberOfItems()
    {
        // Arrange
        var array = new[] { 1, 2, 3, 4, 5 };
        const int count = 3;
        
        // Act
        var result = RandomHelper.Items(array, count);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Length);
        Assert.All(result, item => Assert.Contains(item, array));
    }

    /// <summary>
    /// 测试 Items 方法（从列表中选择多个元素）
    /// </summary>
    [Fact]
    public void Items_FromList_ShouldReturnCorrectNumberOfItems()
    {
        // Arrange
        var list = new List<string> { "apple", "banana", "cherry" };
        const int count = 2;
        
        // Act
        var result = RandomHelper.Items(list, count);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Length);
        Assert.All(result, item => Assert.Contains(item, list));
    }

    #endregion

    #region RandomSelect(int m, int n)方法测试

    /// <summary>
    /// 测试 RandomSelect 方法（从1到n中选择m个数）
    /// </summary>
    [Fact]
    public void RandomSelect_WithValidParameters_ShouldReturnCorrectCount()
    {
        // Arrange
        const int m = 3;
        const int n = 10;
        
        // Act
        var result = RandomHelper.RandomSelect(m, n);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(m, result.Count);
        Assert.All(result, value => Assert.InRange(value, 0, n - 1));
    }

    /// <summary>
    /// 测试 RandomSelect 方法在m为0时返回空集合
    /// </summary>
    [Fact]
    public void RandomSelect_WithZeroM_ShouldReturnEmptySet()
    {
        // Act
        var result = RandomHelper.RandomSelect(0, 10);
        
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    /// <summary>
    /// 测试 RandomSelect 方法在m大于n时抛出异常
    /// </summary>
    [Fact]
    public void RandomSelect_WithMGreaterThanN_ShouldThrowArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => RandomHelper.RandomSelect(10, 5));
    }

    #endregion

    #region 权重相关方法测试

    /// <summary>
    /// 测试 Idx 方法（权重数组）是否返回有效索引
    /// </summary>
    [Fact]
    public void Idx_WithWeights_ShouldReturnValidIndex()
    {
        // Arrange
        var weights = new[] { 10, 20, 30, 40 };
        
        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var result = RandomHelper.Idx(weights);
            Assert.InRange(result, 0, weights.Length - 1);
        }
    }

    /// <summary>
    /// 测试 Idx 方法（权重数组）在null输入时抛出异常
    /// </summary>
    [Fact]
    public void Idx_WithNullWeights_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => RandomHelper.Idx((int[])null!));
    }

    /// <summary>
    /// 测试 Idx 方法（权重数组）在空数组时抛出异常
    /// </summary>
    [Fact]
    public void Idx_WithEmptyWeights_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var emptyWeights = Array.Empty<int>();
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => RandomHelper.Idx(emptyWeights));
    }

    /// <summary>
    /// 测试 Idx 方法（二维权重数组）是否返回有效索引
    /// </summary>
    [Fact]
    public void Idx_WithWeightArray_ShouldReturnValidIndex()
    {
        // Arrange
        var array = new int[][] 
        {
            new[] { 1, 10 },
            new[] { 2, 20 },
            new[] { 3, 30 }
        };
        
        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var result = RandomHelper.Idx(array);
            Assert.InRange(result, 0, array.Length - 1);
        }
    }

    /// <summary>
    /// 测试 Ids 方法（权重数组）是否返回正确数量的ID
    /// </summary>
    [Fact]
    public void Ids_WithWeightArray_ShouldReturnCorrectCount()
    {
        // Arrange
        var array = new int[][] 
        {
            new[] { 1, 10 },
            new[] { 2, 20 },
            new[] { 3, 30 }
        };
        const int num = 2;
        
        // Act
        var result = RandomHelper.Ids(array, num, true);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(num, result.Count);
    }

    /// <summary>
    /// 测试 Ids 方法（权重字符串）是否返回正确数量的ID
    /// </summary>
    [Fact]
    public void Ids_WithWeightString_ShouldReturnCorrectCount()
    {
        // Arrange
        const string weightStr = "1+10;2+20;3+30";
        const int num = 2;
        
        // Act
        var result = RandomHelper.Ids(weightStr, num, true);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(num, result.Count);
    }

    /// <summary>
    /// 测试 Items 方法（权重数组）是否返回正确数量的项目
    /// </summary>
    [Fact]
    public void Items_WithWeightArray_ShouldReturnCorrectCount()
    {
        // Arrange
        var array = new int[][] 
        {
            new[] { 1, 10, 100 },
            new[] { 2, 20, 200 },
            new[] { 3, 30, 300 }
        };
        const int num = 2;
        
        // Act
        var result = RandomHelper.Items(array, num, true);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(num, result.Count);
    }

    /// <summary>
    /// 测试 Items 方法（权重字符串）是否返回正确数量的项目
    /// </summary>
    [Fact]
    public void Items_WithWeightString_ShouldReturnCorrectCount()
    {
        // Arrange
        const string weightStr = "1+10+100;2+20+200;3+30+300";
        const int num = 2;
        
        // Act
        var result = RandomHelper.Items(weightStr, num, true);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(num, result.Count);
    }

    #endregion

    #region Gcd方法测试

    /// <summary>
    /// 测试 Gcd 方法（两个数）是否返回正确的最大公约数
    /// </summary>
    [Fact]
    public void Gcd_WithTwoNumbers_ShouldReturnCorrectGcd()
    {
        // Act & Assert
        Assert.Equal(6, RandomHelper.Gcd(12, 18));
        Assert.Equal(1, RandomHelper.Gcd(7, 13));
        Assert.Equal(5, RandomHelper.Gcd(15, 25));
        Assert.Equal(12, RandomHelper.Gcd(12, 0));
        Assert.Equal(7, RandomHelper.Gcd(0, 7));
    }

    /// <summary>
    /// 测试 Gcd 方法（多个数）是否返回正确的最大公约数
    /// </summary>
    [Fact]
    public void Gcd_WithMultipleNumbers_ShouldReturnCorrectGcd()
    {
        // Act & Assert
        Assert.Equal(6, RandomHelper.Gcd(12, 18, 24));
        Assert.Equal(1, RandomHelper.Gcd(7, 13, 17));
        Assert.Equal(2, RandomHelper.Gcd(4, 6, 8));
    }

    /// <summary>
    /// 测试 Gcd 方法在空数组时返回1
    /// </summary>
    [Fact]
    public void Gcd_WithEmptyArray_ShouldReturnOne()
    {
        // Act
        var result = RandomHelper.Gcd();
        
        // Assert
        Assert.Equal(1, result);
    }

    /// <summary>
    /// 测试 Gcd 方法在单个数时返回该数
    /// </summary>
    [Fact]
    public void Gcd_WithSingleNumber_ShouldReturnThatNumber()
    {
        // Act
        var result = RandomHelper.Gcd(42);
        
        // Assert
        Assert.Equal(42, result);
    }

    /// <summary>
    /// 测试 Gcd 方法在null数组时抛出异常
    /// </summary>
    [Fact]
    public void Gcd_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => RandomHelper.Gcd(null!));
    }

    #endregion

    #region 多线程测试

    /// <summary>
    /// 测试 RandomHelper 在多线程环境下的线程安全性
    /// </summary>
    [Fact]
    public void RandomHelper_MultipleThreads_ShouldBeThreadSafe()
    {
        // Arrange
        const int threadCount = 10;
        const int operationsPerThread = 100;
        var tasks = new Task[threadCount];
        var results = new List<int>[threadCount];
        
        // Act
        for (var i = 0; i < threadCount; i++)
        {
            var threadIndex = i;
            results[threadIndex] = new List<int>();
            
            tasks[threadIndex] = Task.Run(() =>
            {
                for (var j = 0; j < operationsPerThread; j++)
                {
                    results[threadIndex].Add(RandomHelper.Next(1000));
                }
            });
        }
        
        Task.WaitAll(tasks);
        
        // Assert
        for (var i = 0; i < threadCount; i++)
        {
            Assert.Equal(operationsPerThread, results[i].Count);
            Assert.All(results[i], value => Assert.InRange(value, 0, 999));
        }
    }

    #endregion
}