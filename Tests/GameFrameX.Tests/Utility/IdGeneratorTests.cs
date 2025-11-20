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

using GameFrameX.Utility;
using Xunit;

namespace GameFrameX.Tests.Utility;

/// <summary>
/// IdGenerator 类的单元测试
/// </summary>
public class IdGeneratorTests
{
    /// <summary>
    /// 测试 GetNextUniqueIntId 方法是否能生成唯一的整数ID
    /// </summary>
    [Fact]
    public void GetNextUniqueIntId_ShouldReturnUniqueIds()
    {
        // Arrange & Act
        var id1 = IdGenerator.GetNextUniqueIntId();
        var id2 = IdGenerator.GetNextUniqueIntId();
        var id3 = IdGenerator.GetNextUniqueIntId();

        // Assert
        Assert.NotEqual(id1, id2);
        Assert.NotEqual(id2, id3);
        Assert.NotEqual(id1, id3);
        Assert.True(id2 > id1);
        Assert.True(id3 > id2);
    }

    /// <summary>
    /// 测试 GetNextUniqueIntId 方法在并发环境下的线程安全性
    /// </summary>
    [Fact]
    public void GetNextUniqueIntId_ShouldBeThreadSafe()
    {
        // Arrange
        const int threadCount = 10;
        const int idsPerThread = 1000;
        var allIds = new List<int>();
        var tasks = new List<Task>();
        var lockObject = new object();

        // Act
        for (int i = 0; i < threadCount; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var localIds = new List<int>();
                for (int j = 0; j < idsPerThread; j++)
                {
                    localIds.Add(IdGenerator.GetNextUniqueIntId());
                }

                lock (lockObject)
                {
                    allIds.AddRange(localIds);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        Assert.Equal(threadCount * idsPerThread, allIds.Count);
        Assert.Equal(allIds.Count, allIds.Distinct().Count()); // 所有ID都应该是唯一的
    }

    /// <summary>
    /// 测试 GetNextUniqueId 方法是否能生成唯一的长整数ID
    /// </summary>
    [Fact]
    public void GetNextUniqueId_ShouldReturnUniqueIds()
    {
        // Arrange & Act
        var id1 = IdGenerator.GetNextUniqueId();
        var id2 = IdGenerator.GetNextUniqueId();
        var id3 = IdGenerator.GetNextUniqueId();

        // Assert
        Assert.NotEqual(id1, id2);
        Assert.NotEqual(id2, id3);
        Assert.NotEqual(id1, id3);
        Assert.True(id1 > 0);
        Assert.True(id2 > 0);
        Assert.True(id3 > 0);
    }

    /// <summary>
    /// 测试 GetNextUniqueId 方法在并发环境下的线程安全性
    /// </summary>
    [Fact]
    public void GetNextUniqueId_ShouldBeThreadSafe()
    {
        // Arrange
        const int threadCount = 10;
        const int idsPerThread = 1000;
        var allIds = new List<long>();
        var tasks = new List<Task>();
        var lockObject = new object();

        // Act
        for (int i = 0; i < threadCount; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var localIds = new List<long>();
                for (int j = 0; j < idsPerThread; j++)
                {
                    localIds.Add(IdGenerator.GetNextUniqueId());
                }

                lock (lockObject)
                {
                    allIds.AddRange(localIds);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        Assert.Equal(threadCount * idsPerThread, allIds.Count);
        Assert.Equal(allIds.Count, allIds.Distinct().Count()); // 所有ID都应该是唯一的
    }

    /// <summary>
    /// 测试 GetUniqueIdString 方法是否能生成唯一的字符串ID
    /// </summary>
    [Fact]
    public void GetUniqueIdString_ShouldReturnUniqueStrings()
    {
        // Arrange & Act
        var id1 = IdGenerator.GetUniqueIdString();
        var id2 = IdGenerator.GetUniqueIdString();
        var id3 = IdGenerator.GetUniqueIdString();

        // Assert
        Assert.NotEqual(id1, id2);
        Assert.NotEqual(id2, id3);
        Assert.NotEqual(id1, id3);
        Assert.NotNull(id1);
        Assert.NotNull(id2);
        Assert.NotNull(id3);
        Assert.NotEmpty(id1);
        Assert.NotEmpty(id2);
        Assert.NotEmpty(id3);
    }

    /// <summary>
    /// 测试 GetUniqueIdString 方法生成的字符串格式是否正确（无连字符的GUID）
    /// </summary>
    [Fact]
    public void GetUniqueIdString_ShouldReturnValidGuidFormat()
    {
        // Arrange & Act
        var id = IdGenerator.GetUniqueIdString();

        // Assert
        Assert.Equal(32, id.Length); // GUID without hyphens should be 32 characters
        Assert.DoesNotContain("-", id); // Should not contain hyphens
        Assert.True(Guid.TryParse(id, out _)); // Should be parseable as GUID
    }

    /// <summary>
    /// 测试 GetUniqueIdString 方法在并发环境下的线程安全性
    /// </summary>
    [Fact]
    public void GetUniqueIdString_ShouldBeThreadSafe()
    {
        // Arrange
        const int threadCount = 10;
        const int idsPerThread = 1000;
        var allIds = new List<string>();
        var tasks = new List<Task>();
        var lockObject = new object();

        // Act
        for (int i = 0; i < threadCount; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var localIds = new List<string>();
                for (int j = 0; j < idsPerThread; j++)
                {
                    localIds.Add(IdGenerator.GetUniqueIdString());
                }

                lock (lockObject)
                {
                    allIds.AddRange(localIds);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        Assert.Equal(threadCount * idsPerThread, allIds.Count);
        Assert.Equal(allIds.Count, allIds.Distinct().Count()); // 所有ID都应该是唯一的
    }

    /// <summary>
    /// 测试 GetNextUniqueId 方法生成的ID是否符合雪花算法的特征
    /// </summary>
    [Fact]
    public void GetNextUniqueId_ShouldFollowSnowflakePattern()
    {
        // Arrange & Act
        var ids = new List<long>();
        for (int i = 0; i < 100; i++)
        {
            ids.Add(IdGenerator.GetNextUniqueId());
            Thread.Sleep(1); // 确保时间戳不同
        }

        // Assert
        // 雪花算法生成的ID应该是递增的（在同一毫秒内可能相等，但总体趋势是递增的）
        for (int i = 1; i < ids.Count; i++)
        {
            Assert.True(ids[i] >= ids[i - 1], $"ID at index {i} ({ids[i]}) should be >= ID at index {i - 1} ({ids[i - 1]})");
        }
    }
}