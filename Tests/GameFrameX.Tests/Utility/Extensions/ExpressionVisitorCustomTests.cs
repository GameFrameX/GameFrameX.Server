using GameFrameX.Utility.Extensions;
using System.Linq.Expressions;
using Xunit;

namespace GameFrameX.Tests.Utility.Extensions;

/// <summary>
/// ExpressionVisitorCustom 类的单元测试
/// </summary>
public class ExpressionVisitorCustomTests
{
    /// <summary>
    /// 测试用的简单类
    /// </summary>
    private class TestClass
    {
        public int Value { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// 测试 ExpressionVisitorCustom 构造函数的基本功能
    /// </summary>
    [Fact]
    public void Constructor_WithValidParameter_ShouldCreateInstance()
    {
        // Arrange
        var parameter = Expression.Parameter(typeof(TestClass), "x");

        // Act
        var visitor = new ExpressionVisitorCustom(parameter);

        // Assert
        Assert.NotNull(visitor);
    }

    /// <summary>
    /// 测试 ExpressionVisitorCustom 构造函数在null参数时抛出异常
    /// </summary>
    [Fact]
    public void Constructor_WithNullParameter_ShouldThrowArgumentNullException()
    {
        // Arrange
        ParameterExpression? nullParameter = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ExpressionVisitorCustom(nullParameter!));
    }

    /// <summary>
    /// 测试 VisitParameter 方法返回构造函数中传入的参数
    /// </summary>
    [Fact]
    public void VisitParameter_ShouldReturnConstructorParameter()
    {
        // Arrange
        var expectedParameter = Expression.Parameter(typeof(TestClass), "expected");
        var visitor = new ExpressionVisitorCustom(expectedParameter);
        var someOtherParameter = Expression.Parameter(typeof(TestClass), "other");

        // Act
        var result = visitor.Visit(someOtherParameter);

        // Assert
        Assert.Equal(expectedParameter, result);
    }

    /// <summary>
    /// 测试 ExpressionVisitorCustom 在简单表达式中的参数替换
    /// </summary>
    [Fact]
    public void Visit_WithSimpleExpression_ShouldReplaceParameter()
    {
        // Arrange
        var originalParam = Expression.Parameter(typeof(TestClass), "original");
        var newParam = Expression.Parameter(typeof(TestClass), "new");
        var visitor = new ExpressionVisitorCustom(newParam);
        
        // 创建一个简单的表达式：x => x.Value
        var propertyAccess = Expression.Property(originalParam, nameof(TestClass.Value));
        var lambda = Expression.Lambda<Func<TestClass, int>>(propertyAccess, originalParam);

        // Act
        var result = visitor.Visit(lambda) as LambdaExpression;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newParam, result.Parameters[0]);
        
        // 验证表达式体中的参数也被替换了
        var bodyProperty = result.Body as MemberExpression;
        Assert.NotNull(bodyProperty);
        Assert.Equal(newParam, bodyProperty.Expression);
    }

    /// <summary>
    /// 测试 ExpressionVisitorCustom 在复杂表达式中的参数替换
    /// </summary>
    [Fact]
    public void Visit_WithComplexExpression_ShouldReplaceAllParameterReferences()
    {
        // Arrange
        var originalParam = Expression.Parameter(typeof(TestClass), "x");
        var newParam = Expression.Parameter(typeof(TestClass), "y");
        var visitor = new ExpressionVisitorCustom(newParam);
        
        // 创建复杂表达式：x => x.Value > 5 && x.Name.Length > 0
        var valueProperty = Expression.Property(originalParam, nameof(TestClass.Value));
        var nameProperty = Expression.Property(originalParam, nameof(TestClass.Name));
        var lengthProperty = Expression.Property(nameProperty, "Length");
        
        var valueCondition = Expression.GreaterThan(valueProperty, Expression.Constant(5));
        var lengthCondition = Expression.GreaterThan(lengthProperty, Expression.Constant(0));
        var andCondition = Expression.AndAlso(valueCondition, lengthCondition);
        
        var lambda = Expression.Lambda<Func<TestClass, bool>>(andCondition, originalParam);

        // Act
        var result = visitor.Visit(lambda) as LambdaExpression;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newParam, result.Parameters[0]);
        
        // 验证所有参数引用都被替换
        var compiledOriginal = lambda.Compile();
        var compiledResult = result.Compile() as Func<TestClass, bool>;
        Assert.NotNull(compiledResult);
        
        var testObj = new TestClass { Value = 10, Name = "Test" };
        Assert.Equal(compiledOriginal(testObj), compiledResult(testObj));
    }

    /// <summary>
    /// 测试 ExpressionVisitorCustom 在嵌套表达式中的行为
    /// </summary>
    [Fact]
    public void Visit_WithNestedExpression_ShouldReplaceParametersCorrectly()
    {
        // Arrange
        var originalParam = Expression.Parameter(typeof(TestClass), "item");
        var newParam = Expression.Parameter(typeof(TestClass), "element");
        var visitor = new ExpressionVisitorCustom(newParam);
        
        // 创建嵌套表达式：item => item.Name.Substring(0, item.Value)
        var nameProperty = Expression.Property(originalParam, nameof(TestClass.Name));
        var valueProperty = Expression.Property(originalParam, nameof(TestClass.Value));
        var substringMethod = typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) })!;
        var substringCall = Expression.Call(nameProperty, substringMethod, Expression.Constant(0), valueProperty);
        
        var lambda = Expression.Lambda<Func<TestClass, string>>(substringCall, originalParam);

        // Act
        var result = visitor.Visit(lambda) as LambdaExpression;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newParam, result.Parameters[0]);
        
        // 验证功能正确性
        var compiledResult = result.Compile() as Func<TestClass, string>;
        Assert.NotNull(compiledResult);
        
        var testObj = new TestClass { Value = 2, Name = "Testing" };
        Assert.Equal("Te", compiledResult(testObj));
    }

    /// <summary>
    /// 测试 ExpressionVisitorCustom 在条件表达式中的行为
    /// </summary>
    [Fact]
    public void Visit_WithConditionalExpression_ShouldReplaceParametersCorrectly()
    {
        // Arrange
        var originalParam = Expression.Parameter(typeof(TestClass), "x");
        var newParam = Expression.Parameter(typeof(TestClass), "y");
        var visitor = new ExpressionVisitorCustom(newParam);
        
        // 创建条件表达式：x => x.Value > 0 ? x.Name : "Empty"
        var valueProperty = Expression.Property(originalParam, nameof(TestClass.Value));
        var nameProperty = Expression.Property(originalParam, nameof(TestClass.Name));
        var condition = Expression.GreaterThan(valueProperty, Expression.Constant(0));
        var conditionalExpr = Expression.Condition(condition, nameProperty, Expression.Constant("Empty"));
        
        var lambda = Expression.Lambda<Func<TestClass, string>>(conditionalExpr, originalParam);

        // Act
        var result = visitor.Visit(lambda) as LambdaExpression;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newParam, result.Parameters[0]);
        
        // 验证功能正确性
        var compiledResult = result.Compile() as Func<TestClass, string>;
        Assert.NotNull(compiledResult);
        
        var testObj1 = new TestClass { Value = 5, Name = "Test" };
        var testObj2 = new TestClass { Value = -1, Name = "Test" };
        
        Assert.Equal("Test", compiledResult(testObj1));
        Assert.Equal("Empty", compiledResult(testObj2));
    }

    /// <summary>
    /// 测试 ExpressionVisitorCustom 在方法调用表达式中的行为
    /// </summary>
    [Fact]
    public void Visit_WithMethodCallExpression_ShouldReplaceParametersCorrectly()
    {
        // Arrange
        var originalParam = Expression.Parameter(typeof(TestClass), "obj");
        var newParam = Expression.Parameter(typeof(TestClass), "instance");
        var visitor = new ExpressionVisitorCustom(newParam);
        
        // 创建方法调用表达式：obj => obj.Name.ToUpper()
        var nameProperty = Expression.Property(originalParam, nameof(TestClass.Name));
        var toUpperMethod = typeof(string).GetMethod("ToUpper", Type.EmptyTypes)!;
        var methodCall = Expression.Call(nameProperty, toUpperMethod);
        
        var lambda = Expression.Lambda<Func<TestClass, string>>(methodCall, originalParam);

        // Act
        var result = visitor.Visit(lambda) as LambdaExpression;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newParam, result.Parameters[0]);
        
        // 验证功能正确性
        var compiledResult = result.Compile() as Func<TestClass, string>;
        Assert.NotNull(compiledResult);
        
        var testObj = new TestClass { Name = "test" };
        Assert.Equal("TEST", compiledResult(testObj));
    }

    /// <summary>
    /// 测试 ExpressionVisitorCustom 在数组访问表达式中的行为
    /// </summary>
    [Fact]
    public void Visit_WithArrayAccessExpression_ShouldReplaceParametersCorrectly()
    {
        // Arrange
        var originalParam = Expression.Parameter(typeof(TestClass), "x");
        var newParam = Expression.Parameter(typeof(TestClass), "y");
        var visitor = new ExpressionVisitorCustom(newParam);
        
        // 创建字符串索引访问表达式：x => x.Name[x.Value]
        var nameProperty = Expression.Property(originalParam, nameof(TestClass.Name));
        var valueProperty = Expression.Property(originalParam, nameof(TestClass.Value));
        // 使用 MakeIndex 来创建字符串索引访问
        var stringIndexer = typeof(string).GetProperty("Chars")!;
        var indexAccess = Expression.MakeIndex(nameProperty, stringIndexer, new[] { valueProperty });
        
        var lambda = Expression.Lambda<Func<TestClass, char>>(indexAccess, originalParam);

        // Act
        var result = visitor.Visit(lambda) as LambdaExpression;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newParam, result.Parameters[0]);
        
        // 验证功能正确性
        var compiledResult = result.Compile() as Func<TestClass, char>;
        Assert.NotNull(compiledResult);
        
        var testObj = new TestClass { Value = 1, Name = "Test" };
        Assert.Equal('e', compiledResult(testObj));
    }

    /// <summary>
    /// 测试 ExpressionVisitorCustom 在不同类型参数上的行为
    /// </summary>
    [Fact]
    public void Visit_WithDifferentParameterTypes_ShouldWorkCorrectly()
    {
        // Arrange
        var originalParam = Expression.Parameter(typeof(string), "str");
        var newParam = Expression.Parameter(typeof(string), "text");
        var visitor = new ExpressionVisitorCustom(newParam);
        
        // 创建字符串表达式：str => str.Length
        var lengthProperty = Expression.Property(originalParam, "Length");
        var lambda = Expression.Lambda<Func<string, int>>(lengthProperty, originalParam);

        // Act
        var result = visitor.Visit(lambda) as LambdaExpression;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newParam, result.Parameters[0]);
        
        // 验证功能正确性
        var compiledResult = result.Compile() as Func<string, int>;
        Assert.NotNull(compiledResult);
        
        Assert.Equal(5, compiledResult("Hello"));
    }

    /// <summary>
    /// 测试 ExpressionVisitorCustom 在多参数Lambda中的行为（只替换匹配的参数）
    /// </summary>
    [Fact]
    public void Visit_WithMultiParameterLambda_ShouldOnlyReplaceMatchingParameter()
    {
        // Arrange
        var param1 = Expression.Parameter(typeof(int), "x");
        var param2 = Expression.Parameter(typeof(int), "y");
        var newParam = Expression.Parameter(typeof(int), "newX");
        var visitor = new ExpressionVisitorCustom(newParam);
        
        // 创建多参数表达式：(x, y) => x + y
        var addExpression = Expression.Add(param1, param2);
        var lambda = Expression.Lambda<Func<int, int, int>>(addExpression, param1, param2);

        // Act
        var result = visitor.Visit(lambda) as LambdaExpression;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Parameters.Count);
        Assert.Equal(newParam, result.Parameters[0]); // 第一个参数被替换
        Assert.Equal(param2, result.Parameters[1]); // 第二个参数保持不变
    }
}