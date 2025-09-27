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

// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Linq.Expressions;
using GameFrameX.Utility.Extensions;
using Xunit;

namespace GameFrameX.Tests.Utility;

public class ExpressionExtensionTests
{
    [Fact]
    public void And_CombinesTwoExpressions()
    {
        // Arrange
        Expression<Func<string, bool>> expr1 = s => s.Length > 5;
        Expression<Func<string, bool>> expr2 = s => s.StartsWith("test");

        // Act
        var combined = expr1.And(expr2);

        // Assert
        Assert.True(combined.Compile()("testing"));
        Assert.False(combined.Compile()("test"));
    }

    [Fact]
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
        Assert.True(result1.Compile()("testing"));
        Assert.False(result1.Compile()("test"));
        Assert.True(result2.Compile()("longstr")); // Only first expression applies
    }

    [Fact]
    public void Or_CombinesTwoExpressions()
    {
        // Arrange
        Expression<Func<string, bool>> expr1 = s => s.Length > 5;
        Expression<Func<string, bool>> expr2 = s => s.StartsWith("test");

        // Act
        var combined = expr1.Or(expr2);

        // Assert
        Assert.True(combined.Compile()("testing")); // Matches both
        Assert.True(combined.Compile()("test")); // Matches second
        Assert.True(combined.Compile()("longstr")); // Matches first
        Assert.False(combined.Compile()("no")); // Matches neither
    }

    [Fact]
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
        Assert.True(result1.Compile()("test")); // Second expression applies
        Assert.False(result2.Compile()("test")); // Only first expression applies
    }

    [Fact]
    public void Not_InvertsExpression()
    {
        // Arrange
        Expression<Func<string, bool>> expr = s => s.Length > 5;

        // Act
        var notExpr = expr.Not();

        // Assert
        Assert.True(notExpr.Compile()("test")); // Length <= 5
        Assert.False(notExpr.Compile()("testing")); // Length > 5
    }

    [Fact]
    public void All_ThrowArgumentNullException_WhenInputIsNull()
    {
        // Arrange
        Expression<Func<string, bool>> expr = s => s.Length > 5;
        Expression<Func<string, bool>> nullExpr = null;

        // Assert
        Assert.Throws<ArgumentNullException>(() => expr.And(nullExpr));
        Assert.Throws<ArgumentNullException>(() => nullExpr.And(expr));
        Assert.Throws<ArgumentNullException>(() => expr.AndIf(null, expr));
        Assert.Throws<ArgumentNullException>(() => expr.AndIf(() => true, null));
        Assert.Throws<ArgumentNullException>(() => expr.Or(nullExpr));
        Assert.Throws<ArgumentNullException>(() => nullExpr.Or(expr));
        Assert.Throws<ArgumentNullException>(() => expr.OrIf(null, expr));
        Assert.Throws<ArgumentNullException>(() => expr.OrIf(() => true, null));
        Assert.Throws<ArgumentNullException>(() => nullExpr.Not());
    }
}