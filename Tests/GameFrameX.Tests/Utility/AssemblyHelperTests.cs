using System.Reflection;
using Xunit;
using GameFrameX.Utility;

namespace GameFrameX.Tests.Utility;

/// <summary>
/// AssemblyHelper 类的单元测试
/// </summary>
public class AssemblyHelperTests
{
    /// <summary>
    /// 测试用的接口
    /// </summary>
    public interface ITestInterface
    {
        void TestMethod();
    }

    /// <summary>
    /// 测试用的基类
    /// </summary>
    public abstract class TestBaseClass
    {
        public abstract void AbstractMethod();
    }

    /// <summary>
    /// 测试用的实现类
    /// </summary>
    public class TestImplementation : TestBaseClass, ITestInterface
    {
        public override void AbstractMethod() { }
        public void TestMethod() { }
    }

    /// <summary>
    /// 另一个测试用的实现类
    /// </summary>
    public class AnotherTestImplementation : ITestInterface
    {
        public void TestMethod() { }
    }

    /// <summary>
    /// 测试用的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TestAttribute : Attribute
    {
    }

    /// <summary>
    /// 带有特性的测试类
    /// </summary>
    [TestAttribute]
    public class AttributedTestClass : ITestInterface
    {
        public void TestMethod() { }
    }

    /// <summary>
    /// 测试 GetAssemblies 方法 - 应该返回已加载的程序集
    /// </summary>
    [Fact]
    public void GetAssemblies_ShouldReturnLoadedAssemblies()
    {
        // Act
        var assemblies = AssemblyHelper.GetAssemblies();

        // Assert
        Assert.NotNull(assemblies);
        Assert.NotEmpty(assemblies);
        
        // 验证包含当前测试程序集
        var currentAssembly = Assembly.GetExecutingAssembly();
        Assert.Contains(currentAssembly, assemblies);
    }

    /// <summary>
    /// 测试 GetTypes 方法 - 应该返回所有类型
    /// </summary>
    [Fact]
    public void GetTypes_ShouldReturnAllTypes()
    {
        // Act
        var types = AssemblyHelper.GetTypes();

        // Assert
        Assert.NotNull(types);
        Assert.NotEmpty(types);
        
        // 验证包含已知的系统类型
        Assert.Contains(typeof(string), types);
        Assert.Contains(typeof(int), types);
        
        // 验证包含测试类型
        Assert.Contains(typeof(TestImplementation), types);
    }

    /// <summary>
    /// 测试 GetTypes 方法 - 使用列表参数
    /// </summary>
    [Fact]
    public void GetTypes_WithList_ShouldPopulateList()
    {
        // Arrange
        var results = new List<Type>();

        // Act
        AssemblyHelper.GetTypes(results);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(typeof(string), results);
        Assert.Contains(typeof(TestImplementation), results);
    }

    /// <summary>
    /// 测试 GetTypes 方法 - null 参数应该抛出异常
    /// </summary>
    [Fact]
    public void GetTypes_WithNullList_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AssemblyHelper.GetTypes(null));
    }

    /// <summary>
    /// 测试 GetType 方法 - 获取已知类型
    /// </summary>
    [Fact]
    public void GetType_WithValidTypeName_ShouldReturnCorrectType()
    {
        // Act
        var stringType = AssemblyHelper.GetType("System.String");
        var intType = AssemblyHelper.GetType("System.Int32");

        // Assert
        Assert.Equal(typeof(string), stringType);
        Assert.Equal(typeof(int), intType);
    }

    /// <summary>
    /// 测试 GetType 方法 - 获取不存在的类型
    /// </summary>
    [Fact]
    public void GetType_WithInvalidTypeName_ShouldReturnNull()
    {
        // Act
        var result = AssemblyHelper.GetType("NonExistent.Type.Name");

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// 测试 GetType 方法 - null 或空字符串应该抛出异常
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void GetType_WithInvalidInput_ShouldThrowArgumentException(string typeName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => AssemblyHelper.GetType(typeName));
    }

    /// <summary>
    /// 测试 GetType 方法 - null 参数应该抛出 ArgumentNullException
    /// </summary>
    [Fact]
    public void GetType_WithNullInput_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AssemblyHelper.GetType(null));
    }

    /// <summary>
    /// 测试 GetRuntimeImplementTypeNames 泛型方法 - 获取接口实现
    /// </summary>
    [Fact]
    public void GetRuntimeImplementTypeNames_Generic_ShouldReturnImplementations()
    {
        // Act
        var implementations = AssemblyHelper.GetRuntimeImplementTypeNames<ITestInterface>();

        // Assert
        Assert.NotNull(implementations);
        Assert.Contains(typeof(TestImplementation), implementations);
        Assert.Contains(typeof(AnotherTestImplementation), implementations);
        Assert.Contains(typeof(AttributedTestClass), implementations);
    }

    /// <summary>
    /// 测试 GetRuntimeImplementTypeNames 方法 - 获取基类派生类
    /// </summary>
    [Fact]
    public void GetRuntimeImplementTypeNames_WithBaseClass_ShouldReturnDerivedClasses()
    {
        // Act
        var derivedClasses = AssemblyHelper.GetRuntimeImplementTypeNames(typeof(TestBaseClass));

        // Assert
        Assert.NotNull(derivedClasses);
        Assert.Contains(typeof(TestImplementation), derivedClasses);
        
        // 抽象基类本身不应该包含在结果中
        Assert.DoesNotContain(typeof(TestBaseClass), derivedClasses);
    }

    /// <summary>
    /// 测试 GetRuntimeImplementTypeNames 方法 - null 参数应该抛出异常
    /// </summary>
    [Fact]
    public void GetRuntimeImplementTypeNames_WithNullType_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AssemblyHelper.GetRuntimeImplementTypeNames(null));
    }

    /// <summary>
    /// 测试 GetRuntimeImplementTypeNames 带特性过滤的方法
    /// </summary>
    [Fact]
    public void GetRuntimeImplementTypeNames_WithAttribute_ShouldReturnFilteredTypes()
    {
        // Act
        var attributedTypes = AssemblyHelper.GetRuntimeImplementTypeNames<ITestInterface, TestAttribute>();

        // Assert
        Assert.NotNull(attributedTypes);
        Assert.Contains(typeof(AttributedTestClass), attributedTypes);
        
        // 没有特性的类型不应该包含在结果中
        Assert.DoesNotContain(typeof(TestImplementation), attributedTypes);
        Assert.DoesNotContain(typeof(AnotherTestImplementation), attributedTypes);
    }

    /// <summary>
    /// 测试 GetRuntimeTypeNames 方法 - 返回类型全名
    /// </summary>
    [Fact]
    public void GetRuntimeTypeNames_ShouldReturnFullNames()
    {
        // Act
        var typeNames = AssemblyHelper.GetRuntimeTypeNames(typeof(ITestInterface));

        // Assert
        Assert.NotNull(typeNames);
        Assert.Contains(typeof(TestImplementation).FullName, typeNames);
        Assert.Contains(typeof(AnotherTestImplementation).FullName, typeNames);
        Assert.Contains(typeof(AttributedTestClass).FullName, typeNames);
    }

    /// <summary>
    /// 测试 GetRuntimeImplementTypeNamesInstance 方法 - 创建实例
    /// </summary>
    [Fact]
    public void GetRuntimeImplementTypeNamesInstance_ShouldCreateInstances()
    {
        // Act
        var instances = AssemblyHelper.GetRuntimeImplementTypeNamesInstance<ITestInterface>();

        // Assert
        Assert.NotNull(instances);
        Assert.NotEmpty(instances);
        
        // 验证实例类型
        var types = instances.Select(i => i.GetType()).ToList();
        Assert.Contains(typeof(TestImplementation), types);
        Assert.Contains(typeof(AnotherTestImplementation), types);
        Assert.Contains(typeof(AttributedTestClass), types);
        
        // 验证所有实例都实现了接口
        Assert.All(instances, instance => Assert.IsAssignableFrom<ITestInterface>(instance));
    }

    /// <summary>
    /// 测试类型缓存功能 - 多次调用应该返回相同结果
    /// </summary>
    [Fact]
    public void GetType_MultipleCalls_ShouldReturnSameResult()
    {
        // Arrange
        const string typeName = "System.String";

        // Act
        var type1 = AssemblyHelper.GetType(typeName);
        var type2 = AssemblyHelper.GetType(typeName);
        var type3 = AssemblyHelper.GetType(typeName);

        // Assert
        Assert.Same(type1, type2);
        Assert.Same(type2, type3);
        Assert.Equal(typeof(string), type1);
    }

    /// <summary>
    /// 测试线程安全性 - 多线程访问应该正常工作
    /// </summary>
    [Fact]
    public void GetTypes_MultipleThreads_ShouldWorkCorrectly()
    {
        // Arrange
        const int threadCount = 10;
        var results = new List<Type[]>();
        var tasks = new List<Task>();
        var lockObject = new object();

        // Act
        for (int i = 0; i < threadCount; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var types = AssemblyHelper.GetTypes();
                lock (lockObject)
                {
                    results.Add(types);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        Assert.Equal(threadCount, results.Count);
        
        // 所有结果应该相同（引用相等）
        var firstResult = results[0];
        foreach (var result in results)
        {
            Assert.Same(firstResult, result);
        }
    }

    /// <summary>
    /// 测试性能 - 类型获取应该相对快速
    /// </summary>
    [Fact]
    public void GetTypes_Performance_ShouldBeReasonablyFast()
    {
        // Arrange
        const int iterations = 1000;
        
        // 预热
        AssemblyHelper.GetTypes();
        
        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            var types = AssemblyHelper.GetTypes();
            Assert.NotNull(types);
        }
        
        stopwatch.Stop();

        // Assert
        // 由于使用了缓存，后续调用应该很快
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, 
            $"Performance test failed: {stopwatch.ElapsedMilliseconds}ms for {iterations} iterations");
    }
}