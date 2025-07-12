using System.Collections.Concurrent;
using Xunit;
using GameFrameX.Utility;

namespace GameFrameX.Tests.Utility;

/// <summary>
/// Singleton 类的单元测试
/// </summary>
public class SingletonTests
{
    /// <summary>
    /// 测试用的单例类
    /// </summary>
    public class TestSingleton : Singleton<TestSingleton>
    {
        public int Value { get; set; }
        public DateTime CreatedTime { get; }

        public TestSingleton()
        {
            CreatedTime = DateTime.Now;
        }
    }

    /// <summary>
    /// 另一个测试用的单例类
    /// </summary>
    public class AnotherTestSingleton : Singleton<AnotherTestSingleton>
    {
        public string Name { get; set; } = "Test";
    }

    /// <summary>
    /// 测试 Instance 属性 - 返回相同的实例
    /// </summary>
    [Fact]
    public void Instance_ShouldReturnSameInstance()
    {
        // Act
        var instance1 = TestSingleton.Instance;
        var instance2 = TestSingleton.Instance;

        // Assert
        Assert.Same(instance1, instance2);
        Assert.NotNull(instance1);
        Assert.NotNull(instance2);
    }

    /// <summary>
    /// 测试 Instance 属性 - 不同类型的单例应该是不同的实例
    /// </summary>
    [Fact]
    public void Instance_DifferentSingletonTypes_ShouldReturnDifferentInstances()
    {
        // Act
        var testInstance = TestSingleton.Instance;
        var anotherInstance = AnotherTestSingleton.Instance;

        // Assert
        Assert.NotSame(testInstance, anotherInstance);
        Assert.IsType<TestSingleton>(testInstance);
        Assert.IsType<AnotherTestSingleton>(anotherInstance);
    }

    /// <summary>
    /// 测试单例的状态保持
    /// </summary>
    [Fact]
    public void Instance_ShouldMaintainState()
    {
        // Arrange
        const int expectedValue = 42;

        // Act
        var instance1 = TestSingleton.Instance;
        instance1.Value = expectedValue;
        
        var instance2 = TestSingleton.Instance;

        // Assert
        Assert.Equal(expectedValue, instance2.Value);
        Assert.Same(instance1, instance2);
    }

    /// <summary>
    /// 测试线程安全性 - 多线程访问应该返回相同的实例
    /// </summary>
    [Fact]
    public void Instance_MultipleThreads_ShouldReturnSameInstance()
    {
        // Arrange
        const int threadCount = 10;
        const int iterationsPerThread = 100;
        var instances = new ConcurrentBag<TestSingleton>();
        var tasks = new List<Task>();

        // Act
        for (int i = 0; i < threadCount; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < iterationsPerThread; j++)
                {
                    instances.Add(TestSingleton.Instance);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        var distinctInstances = instances.Distinct().ToList();
        Assert.Single(distinctInstances);
        Assert.Equal(threadCount * iterationsPerThread, instances.Count);
    }

    /// <summary>
    /// 测试线程安全性 - 验证只创建一个实例
    /// </summary>
    [Fact]
    public void Instance_ConcurrentAccess_ShouldCreateOnlyOneInstance()
    {
        // Arrange
        const int threadCount = 50;
        var instances = new ConcurrentBag<TestSingleton>();
        var barrier = new Barrier(threadCount);
        var tasks = new List<Task>();

        // Act
        for (int i = 0; i < threadCount; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                barrier.SignalAndWait(); // 确保所有线程同时开始
                instances.Add(TestSingleton.Instance);
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        var uniqueInstances = instances.Distinct().ToList();
        Assert.Single(uniqueInstances);
        
        // 验证所有实例的创建时间相同（因为是同一个实例）
        var creationTimes = instances.Select(i => i.CreatedTime).Distinct().ToList();
        Assert.Single(creationTimes);
    }

    /// <summary>
    /// 测试单例实例的初始化
    /// </summary>
    [Fact]
    public void Instance_ShouldBeProperlyInitialized()
    {
        // Act
        var instance = TestSingleton.Instance;

        // Assert
        Assert.NotNull(instance);
        Assert.True(instance.CreatedTime > DateTime.MinValue);
        Assert.True(instance.CreatedTime <= DateTime.Now);
    }

    /// <summary>
    /// 测试多个不同单例类型的独立性
    /// </summary>
    [Fact]
    public void Instance_MultipleSingletonTypes_ShouldBeIndependent()
    {
        // Act
        var testInstance1 = TestSingleton.Instance;
        var testInstance2 = TestSingleton.Instance;
        var anotherInstance1 = AnotherTestSingleton.Instance;
        var anotherInstance2 = AnotherTestSingleton.Instance;

        // Assert
        // 同类型的单例应该是相同的实例
        Assert.Same(testInstance1, testInstance2);
        Assert.Same(anotherInstance1, anotherInstance2);
        
        // 不同类型的单例应该是不同的实例
        Assert.NotSame(testInstance1, anotherInstance1);
        
        // 修改一个单例的状态不应该影响另一个
        testInstance1.Value = 100;
        anotherInstance1.Name = "Modified";
        
        Assert.Equal(100, testInstance2.Value);
        Assert.Equal("Modified", anotherInstance2.Name);
    }

    /// <summary>
    /// 测试单例的性能 - 第一次访问后的后续访问应该很快
    /// </summary>
    [Fact]
    public void Instance_SubsequentAccess_ShouldBeFast()
    {
        // Arrange
        const int iterations = 10000;
        
        // 第一次访问（可能较慢，因为需要创建实例）
        var firstInstance = TestSingleton.Instance;
        
        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            var instance = TestSingleton.Instance;
            Assert.Same(firstInstance, instance);
        }
        
        stopwatch.Stop();

        // Assert
        // 后续访问应该很快（这里只是确保没有异常，实际性能测试可能因环境而异）
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, 
            $"Subsequent access took too long: {stopwatch.ElapsedMilliseconds}ms for {iterations} iterations");
    }
}