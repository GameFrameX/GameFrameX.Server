// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Linq.Expressions;
using GameFrameX.Utility.Extensions;

namespace GameFrameX.Tests.Utility;

public class ExpressionExtensionTests
{
    [Test]
    public void And_CombinesTwoExpressions()
    {
        // Arrange
        Expression<Func<string, bool>> expr1 = s => s.Length > 5;
        Expression<Func<string, bool>> expr2 = s => s.StartsWith("test");

        // Act
        var combined = expr1.And(expr2);

        // Assert
        Assert.That(combined.Compile()("testing"), Is.True);
        Assert.That(combined.Compile()("test"), Is.False);
    }

    [Test]
    public void AndIf_OnlyAppliesWhenConditionIsTrue()
    {
        // Arrange
        Expression<Func<string, bool>> expr1 = s => s.Length > 5;
        Expression<Func<string, bool>> expr2 = s => s.StartsWith("test");
        bool condition = true;

        // Act
        var result1 = expr1.AndIf(() => condition, expr2);
        condition = false;
        var result2 = expr1.AndIf(() => condition, expr2);

        // Assert
        Assert.That(result1.Compile()("testing"), Is.True);
        Assert.That(result1.Compile()("test"), Is.False);
        Assert.That(result2.Compile()("longstr"), Is.True); // Only first expression applies
    }

    [Test]
    public void Or_CombinesTwoExpressions()
    {
        // Arrange
        Expression<Func<string, bool>> expr1 = s => s.Length > 5;
        Expression<Func<string, bool>> expr2 = s => s.StartsWith("test");

        // Act
        var combined = expr1.Or(expr2);

        // Assert
        Assert.That(combined.Compile()("testing"), Is.True); // Matches both
        Assert.That(combined.Compile()("test"), Is.True); // Matches second
        Assert.That(combined.Compile()("longstr"), Is.True); // Matches first
        Assert.That(combined.Compile()("no"), Is.False); // Matches neither
    }

    [Test]
    public void OrIf_OnlyAppliesWhenConditionIsTrue()
    {
        // Arrange
        Expression<Func<string, bool>> expr1 = s => s.Length > 5;
        Expression<Func<string, bool>> expr2 = s => s.StartsWith("test");
        bool condition = true;

        // Act
        var result1 = expr1.OrIf(() => condition, expr2);
        condition = false;
        var result2 = expr1.OrIf(() => condition, expr2);

        // Assert
        Assert.That(result1.Compile()("test"), Is.True); // Second expression applies
        Assert.That(result2.Compile()("test"), Is.False); // Only first expression applies
    }

    [Test]
    public void Not_InvertsExpression()
    {
        // Arrange
        Expression<Func<string, bool>> expr = s => s.Length > 5;

        // Act
        var notExpr = expr.Not();

        // Assert
        Assert.That(notExpr.Compile()("test"), Is.True); // Length <= 5
        Assert.That(notExpr.Compile()("testing"), Is.False); // Length > 5
    }

    [Test]
    public void All_ThrowArgumentNullException_WhenInputIsNull()
    {
        // Arrange
        Expression<Func<string, bool>> expr = s => s.Length > 5;
        Expression<Func<string, bool>> nullExpr = null;

        // Assert
        Assert.That(() => expr.And(nullExpr), Throws.ArgumentNullException);
        Assert.That(() => nullExpr.And(expr), Throws.ArgumentNullException);
        Assert.That(() => expr.AndIf(null, expr), Throws.ArgumentNullException);
        Assert.That(() => expr.AndIf(() => true, null), Throws.ArgumentNullException);
        Assert.That(() => expr.Or(nullExpr), Throws.ArgumentNullException);
        Assert.That(() => nullExpr.Or(expr), Throws.ArgumentNullException);
        Assert.That(() => expr.OrIf(null, expr), Throws.ArgumentNullException);
        Assert.That(() => expr.OrIf(() => true, null), Throws.ArgumentNullException);
        Assert.That(() => nullExpr.Not(), Throws.ArgumentNullException);
    }
}