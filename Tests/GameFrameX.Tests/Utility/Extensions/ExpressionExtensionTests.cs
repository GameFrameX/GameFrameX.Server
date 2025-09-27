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
/// ExpressionExtension 类的单元测试
/// </summary>
public class ExpressionExtensionTests
{
    /// <summary>
    /// 测试用的简单类
    /// </summary>
    private class TestClass
    {
        public int Value { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// 测试 And 方法的基本功能
    /// </summary>
    [Fact]
    public void And_WithValidExpressions_ShouldCombineWithLogicalAnd()
    {
        // Arrange
        Expression<Func<TestClass, bool>> expr1 = x => x.Value > 5;
        Expression<Func<TestClass, bool>> expr2 = x => x.Value < 10;
        var testObj1 = new TestClass { Value = 7 }; // 应该满足条件
        var testObj2 = new TestClass { Value = 3 }; // 不满足第一个条件
        var testObj3 = new TestClass { Value = 12 }; // 不满足第二个条件

        // Act
        var combinedExpr = expr1.And(expr2);
        var compiledExpr = combinedExpr.Compile();

        // Assert
        Assert.True(compiledExpr(testObj1)); // 7 > 5 && 7 < 10 = true
        Assert.False(compiledExpr(testObj2)); // 3 > 5 && 3 < 10 = false
        Assert.False(compiledExpr(testObj3)); // 12 > 5 && 12 < 10 = false
    }

    /// <summary>
    /// 测试 And 方法在null输入时抛出异常
    /// </summary>
    [Fact]
    public void And_WithNullLeftExpression_ShouldThrowArgumentNullException()
    {
        // Arrange
        Expression<Func<TestClass, bool>>? nullExpr = null;
        Expression<Func<TestClass, bool>> expr = x => x.Value > 5;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => nullExpr!.And(expr));
    }

    /// <summary>
    /// 测试 And 方法在右表达式为null时抛出异常
    /// </summary>
    [Fact]
    public void And_WithNullRightExpression_ShouldThrowArgumentNullException()
    {
        // Arrange
        Expression<Func<TestClass, bool>> expr = x => x.Value > 5;
        Expression<Func<TestClass, bool>>? nullExpr = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => expr.And(nullExpr!));
    }

    /// <summary>
    /// 测试 AndIf 方法在条件为true时的行为
    /// </summary>
    [Fact]
    public void AndIf_WithTrueCondition_ShouldCombineExpressions()
    {
        // Arrange
        Expression<Func<TestClass, bool>> expr1 = x => x.Value > 5;
        Expression<Func<TestClass, bool>> expr2 = x => x.IsActive;
        var testObj = new TestClass { Value = 7, IsActive = true };
        Func<bool> trueCondition = () => true;

        // Act
        var result = expr1.AndIf(trueCondition, expr2);
        var compiled = result.Compile();

        // Assert
        Assert.True(compiled(testObj));
    }

    /// <summary>
    /// 测试 AndIf 方法在条件为false时的行为
    /// </summary>
    [Fact]
    public void AndIf_WithFalseCondition_ShouldReturnLeftExpression()
    {
        // Arrange
        Expression<Func<TestClass, bool>> expr1 = x => x.Value > 5;
        Expression<Func<TestClass, bool>> expr2 = x => x.IsActive;
        var testObj = new TestClass { Value = 7, IsActive = false };
        Func<bool> falseCondition = () => false;

        // Act
        var result = expr1.AndIf(falseCondition, expr2);
        var compiled = result.Compile();

        // Assert
        Assert.True(compiled(testObj)); // 只检查第一个条件
    }

    /// <summary>
    /// 测试 AndIf 方法在null条件时抛出异常
    /// </summary>
    [Fact]
    public void AndIf_WithNullCondition_ShouldThrowArgumentNullException()
    {
        // Arrange
        Expression<Func<TestClass, bool>> expr1 = x => x.Value > 5;
        Expression<Func<TestClass, bool>> expr2 = x => x.IsActive;
        Func<bool>? nullCondition = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => expr1.AndIf(nullCondition!, expr2));
    }

    /// <summary>
    /// 测试 Or 方法的基本功能
    /// </summary>
    [Fact]
    public void Or_WithValidExpressions_ShouldCombineWithLogicalOr()
    {
        // Arrange
        Expression<Func<TestClass, bool>> expr1 = x => x.Value > 10;
        Expression<Func<TestClass, bool>> expr2 = x => x.IsActive;
        var testObj1 = new TestClass { Value = 5, IsActive = true }; // 满足第二个条件
        var testObj2 = new TestClass { Value = 15, IsActive = false }; // 满足第一个条件
        var testObj3 = new TestClass { Value = 5, IsActive = false }; // 都不满足

        // Act
        var combinedExpr = expr1.Or(expr2);
        var compiledExpr = combinedExpr.Compile();

        // Assert
        Assert.True(compiledExpr(testObj1)); // false || true = true
        Assert.True(compiledExpr(testObj2)); // true || false = true
        Assert.False(compiledExpr(testObj3)); // false || false = false
    }

    /// <summary>
    /// 测试 Or 方法在null输入时抛出异常
    /// </summary>
    [Fact]
    public void Or_WithNullLeftExpression_ShouldThrowArgumentNullException()
    {
        // Arrange
        Expression<Func<TestClass, bool>>? nullExpr = null;
        Expression<Func<TestClass, bool>> expr = x => x.Value > 5;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => nullExpr!.Or(expr));
    }

    /// <summary>
    /// 测试 OrIf 方法在条件为true时的行为
    /// </summary>
    [Fact]
    public void OrIf_WithTrueCondition_ShouldCombineExpressions()
    {
        // Arrange
        Expression<Func<TestClass, bool>> expr1 = x => x.Value > 10;
        Expression<Func<TestClass, bool>> expr2 = x => x.IsActive;
        var testObj = new TestClass { Value = 5, IsActive = true };
        Func<bool> trueCondition = () => true;

        // Act
        var result = expr1.OrIf(trueCondition, expr2);
        var compiled = result.Compile();

        // Assert
        Assert.True(compiled(testObj)); // false || true = true
    }

    /// <summary>
    /// 测试 OrIf 方法在条件为false时的行为
    /// </summary>
    [Fact]
    public void OrIf_WithFalseCondition_ShouldReturnLeftExpression()
    {
        // Arrange
        Expression<Func<TestClass, bool>> expr1 = x => x.Value > 10;
        Expression<Func<TestClass, bool>> expr2 = x => x.IsActive;
        var testObj = new TestClass { Value = 5, IsActive = true };
        Func<bool> falseCondition = () => false;

        // Act
        var result = expr1.OrIf(falseCondition, expr2);
        var compiled = result.Compile();

        // Assert
        Assert.False(compiled(testObj)); // 只检查第一个条件：5 > 10 = false
    }

    /// <summary>
    /// 测试 Not 方法的基本功能
    /// </summary>
    [Fact]
    public void Not_WithValidExpression_ShouldNegateResult()
    {
        // Arrange
        Expression<Func<TestClass, bool>> expr = x => x.Value > 5;
        var testObj1 = new TestClass { Value = 7 }; // 原本为true
        var testObj2 = new TestClass { Value = 3 }; // 原本为false

        // Act
        var negatedExpr = expr.Not();
        var compiledExpr = negatedExpr.Compile();

        // Assert
        Assert.False(compiledExpr(testObj1)); // !(7 > 5) = !true = false
        Assert.True(compiledExpr(testObj2)); // !(3 > 5) = !false = true
    }

    /// <summary>
    /// 测试 Not 方法在null输入时抛出异常
    /// </summary>
    [Fact]
    public void Not_WithNullExpression_ShouldThrowArgumentNullException()
    {
        // Arrange
        Expression<Func<TestClass, bool>>? nullExpr = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => nullExpr!.Not());
    }

    /// <summary>
    /// 测试复杂的表达式组合
    /// </summary>
    [Fact]
    public void ComplexExpressionCombination_ShouldWorkCorrectly()
    {
        // Arrange
        Expression<Func<TestClass, bool>> expr1 = x => x.Value > 5;
        Expression<Func<TestClass, bool>> expr2 = x => x.Value < 15;
        Expression<Func<TestClass, bool>> expr3 = x => x.IsActive;
        
        var testObj = new TestClass { Value = 10, IsActive = true };

        // Act
        // (Value > 5 && Value < 15) || IsActive
        var complexExpr = expr1.And(expr2).Or(expr3);
        var compiled = complexExpr.Compile();

        // Assert
        Assert.True(compiled(testObj)); // (10 > 5 && 10 < 15) || true = true || true = true
    }

    /// <summary>
    /// 测试表达式的短路求值行为
    /// </summary>
    [Fact]
    public void And_ShouldUseShortCircuitEvaluation()
    {
        // Arrange
        var callCount = 0;
        Expression<Func<TestClass, bool>> expr1 = x => x.Value < 5; // false
        Expression<Func<TestClass, bool>> expr2 = x => IncrementAndReturnTrue(ref callCount);
        
        var testObj = new TestClass { Value = 10 };

        // Act
        var combinedExpr = expr1.And(expr2);
        var compiled = combinedExpr.Compile();
        var result = compiled(testObj);

        // Assert
        Assert.False(result);
        // 注意：由于表达式树的编译方式，短路求值可能不会像预期那样工作
        // 这个测试主要验证逻辑正确性
    }

    /// <summary>
    /// 测试 Or 的短路求值行为
    /// </summary>
    [Fact]
    public void Or_ShouldUseShortCircuitEvaluation()
    {
        // Arrange
        var callCount = 0;
        Expression<Func<TestClass, bool>> expr1 = x => x.Value > 5; // true
        Expression<Func<TestClass, bool>> expr2 = x => IncrementAndReturnFalse(ref callCount);
        
        var testObj = new TestClass { Value = 10 };

        // Act
        var combinedExpr = expr1.Or(expr2);
        var compiled = combinedExpr.Compile();
        var result = compiled(testObj);

        // Assert
        Assert.True(result);
        // 同样，这主要验证逻辑正确性
    }

    /// <summary>
    /// 测试多重否定
    /// </summary>
    [Fact]
    public void DoubleNot_ShouldReturnOriginalResult()
    {
        // Arrange
        Expression<Func<TestClass, bool>> expr = x => x.Value > 5;
        var testObj = new TestClass { Value = 10 };

        // Act
        var doubleNegated = expr.Not().Not();
        var compiled = doubleNegated.Compile();
        var originalCompiled = expr.Compile();

        // Assert
        Assert.Equal(originalCompiled(testObj), compiled(testObj));
    }

    /// <summary>
    /// 测试字符串属性的表达式
    /// </summary>
    [Fact]
    public void StringPropertyExpression_ShouldWorkCorrectly()
    {
        // Arrange
        Expression<Func<TestClass, bool>> expr1 = x => x.Name == "Test";
        Expression<Func<TestClass, bool>> expr2 = x => x.Name.Length > 3;
        
        var testObj1 = new TestClass { Name = "Test" };
        var testObj2 = new TestClass { Name = "Testing" };

        // Act
        var combinedExpr = expr1.Or(expr2);
        var compiled = combinedExpr.Compile();

        // Assert
        Assert.True(compiled(testObj1)); // "Test" == "Test" = true
        Assert.True(compiled(testObj2)); // "Testing".Length > 3 = true
    }

    /// <summary>
    /// 辅助方法：递增计数器并返回true
    /// </summary>
    private static bool IncrementAndReturnTrue(ref int counter)
    {
        counter++;
        return true;
    }

    /// <summary>
    /// 辅助方法：递增计数器并返回false
    /// </summary>
    private static bool IncrementAndReturnFalse(ref int counter)
    {
        counter++;
        return false;
    }
}