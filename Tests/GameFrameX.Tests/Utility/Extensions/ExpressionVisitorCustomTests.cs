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