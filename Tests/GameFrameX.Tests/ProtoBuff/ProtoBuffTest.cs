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

using GameFrameX.ProtoBuf.Net;
using ProtoBuf;
using Xunit;

namespace GameFrameX.Tests.ProtoBuff;

public class ProtoBuffTest
{
    [ProtoContract]
    public class PbTest
    {
        [ProtoMember(1, IsRequired = false)] public List<string> Test { get; set; } = new List<string>();
        [ProtoMember(2, IsRequired = false)] public int Id { get; set; }
        [ProtoMember(3, IsRequired = false)] public string Name { get; set; } = string.Empty;
    }

    public ProtoBuffTest()
    {
        // xUnit uses constructor for setup
    }

    [Fact]
    public void TestSerialize()
    {
        PbTest pbTest = new PbTest();
        pbTest.Test.Add("1");
        pbTest.Test.Add("2"); // 移除null值，因为ProtoBuf不支持序列化null字符串
        pbTest.Test.Add("3");
        pbTest.Test.Add("4");
        pbTest.Test.Add("5");
        pbTest.Test.Add("6");
        pbTest.Test.Add("7");
        pbTest.Test.Add("8");
        pbTest.Test.Add("9");

        try
        {
            var xx = ProtoBufSerializerHelper.Serialize(pbTest);
            Console.WriteLine(xx);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// 测试 Serializer.Merge 方法的合并功能 / Test the merge functionality of Serializer.Merge method
    /// </summary>
    [Fact]
    public void TestSerializerMerge_ShouldMergeDataCorrectly()
    {
        // Arrange - 准备测试数据
        var originalData = new PbTest
        {
            Id = 100,
            Name = "Original",
            Test = new List<string> { "item1", "item2" }
        };

        var updateData = new PbTest
        {
            Id = 200,
            Name = "Updated",
            Test = new List<string> { "item3", "item4", "item5" }
        };

        // 序列化更新数据
        var serializedUpdateData = ProtoBufSerializerHelper.Serialize(updateData);

        // Act - 执行合并操作
        var mergedResult = ProtoBufSerializerHelper.Deserialize(serializedUpdateData, originalData);

        // Assert - 验证合并结果
        Assert.NotNull(mergedResult);
        Assert.Same(originalData, mergedResult); // 应该返回同一个实例

        // 验证字段是否正确合并
        Assert.Equal(200, mergedResult.Id); // Id 应该被更新
        Assert.Equal("Updated", mergedResult.Name); // Name 应该被更新

        // 验证列表合并行为
        Assert.NotNull(mergedResult.Test);
        // ProtoBuf 的合并行为：列表会被追加而不是替换
        Assert.True(mergedResult.Test.Count >= updateData.Test.Count);
    }

    /// <summary>
    /// 测试空数据合并 / Test merging with empty data
    /// </summary>
    [Fact]
    public void TestSerializerMerge_WithEmptyData_ShouldReturnOriginalInstance()
    {
        // Arrange
        var originalData = new PbTest
        {
            Id = 100,
            Name = "Original",
            Test = new List<string> { "item1", "item2" }
        };

        var emptyData = new byte[0];

        // Act
        var result = ProtoBufSerializerHelper.Deserialize(emptyData, originalData);

        // Assert
        Assert.Same(originalData, result);
        Assert.Equal(100, result.Id);
        Assert.Equal("Original", result.Name);
        Assert.Equal(2, result.Test.Count);
    }

    /// <summary>
    /// 测试 null 实例参数 / Test with null instance parameter
    /// </summary>
    [Fact]
    public void TestSerializerMerge_WithNullInstance_ShouldThrowArgumentNullException()
    {
        // Arrange
        var testData = new PbTest { Id = 100, Name = "Test" };
        var serializedData = ProtoBufSerializerHelper.Serialize(testData);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
                                                 ProtoBufSerializerHelper.Deserialize<PbTest>(serializedData, null));
    }

    /// <summary>
    /// 测试 null 数据参数 / Test with null data parameter
    /// </summary>
    [Fact]
    public void TestSerializerMerge_WithNullData_ShouldThrowArgumentNullException()
    {
        // Arrange
        var instance = new PbTest { Id = 100, Name = "Test" };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
                                                 ProtoBufSerializerHelper.Deserialize<PbTest>(null, instance));
    }

    /// <summary>
    /// 测试部分字段更新 / Test partial field updates
    /// </summary>
    [Fact]
    public void TestSerializerMerge_PartialUpdate_ShouldMergeOnlyProvidedFields()
    {
        // Arrange
        var originalData = new PbTest
        {
            Id = 100,
            Name = "Original",
            Test = new List<string> { "item1", "item2" }
        };

        // 创建只包含部分字段的更新数据
        var partialUpdateData = new PbTest
        {
            Name = "PartiallyUpdated"
            // Id 和 Test 保持默认值
        };

        var serializedPartialData = ProtoBufSerializerHelper.Serialize(partialUpdateData);

        // Act
        var result = ProtoBufSerializerHelper.Deserialize(serializedPartialData, originalData);

        // Assert
        Assert.Same(originalData, result);
        // 根据 ProtoBuf 的合并规则，只有非默认值的字段会被合并
        Assert.Equal("PartiallyUpdated", result.Name); // Name 应该被更新
        // Id 和 Test 的行为取决于 ProtoBuf 的具体实现
    }
}