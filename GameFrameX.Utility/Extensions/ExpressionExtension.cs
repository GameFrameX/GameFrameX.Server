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

using System.Linq.Expressions;

namespace GameFrameX.Utility.Extensions;

/// <summary>
/// 提供对 <see cref="Expression" /> 类型的扩展方法，用于组合和操作表达式树。
/// </summary>
public static class ExpressionExtension
{
    /// <summary>
    /// 将两个表达式进行逻辑与运算，使用短路求值。
    /// </summary>
    /// <typeparam name="T">表达式的参数类型。</typeparam>
    /// <param name="leftExpression">第一个表达式，作为逻辑与运算的左操作数。</param>
    /// <param name="rightExpression">第二个表达式，作为逻辑与运算的右操作数。</param>
    /// <returns>一个新的表达式，表示两个输入表达式的逻辑与运算结果。</returns>
    /// <exception cref="ArgumentNullException">当 leftExpression 或 rightExpression 为 null 时抛出。</exception>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> leftExpression, Expression<Func<T, bool>> rightExpression)
    {
        ArgumentNullException.ThrowIfNull(leftExpression, nameof(leftExpression));
        ArgumentNullException.ThrowIfNull(rightExpression, nameof(rightExpression));
        var newParameter = Expression.Parameter(typeof(T), nameof(And));
        var visitor = new ExpressionVisitorCustom(newParameter);
        var left = visitor.Visit(leftExpression.Body);
        var right = visitor.Visit(rightExpression.Body);
        var body = Expression.AndAlso(left, right); // 使用AndAlso替代And以支持短路求值
        return Expression.Lambda<Func<T, bool>>(body, newParameter);
    }

    /// <summary>
    /// 根据条件将两个表达式进行逻辑与运算，使用短路求值。
    /// 当条件为false时，仅返回左表达式；当条件为true时，返回两个表达式的逻辑与运算结果。
    /// </summary>
    /// <typeparam name="T">表达式的参数类型。</typeparam>
    /// <param name="leftExpression">第一个表达式，作为逻辑与运算的左操作数。</param>
    /// <param name="condition">决定是否执行逻辑与运算的条件委托。</param>
    /// <param name="rightExpression">第二个表达式，作为逻辑与运算的右操作数。</param>
    /// <returns>当条件为true时返回两个表达式的逻辑与运算结果，否则返回左表达式。</returns>
    /// <exception cref="ArgumentNullException">当任何参数为null时抛出。</exception>
    public static Expression<Func<T, bool>> AndIf<T>(this Expression<Func<T, bool>> leftExpression, Func<bool> condition, Expression<Func<T, bool>> rightExpression)
    {
        ArgumentNullException.ThrowIfNull(leftExpression, nameof(leftExpression));
        ArgumentNullException.ThrowIfNull(condition, nameof(condition));
        ArgumentNullException.ThrowIfNull(rightExpression, nameof(rightExpression));

        if (!condition())
        {
            return leftExpression;
        }

        var newParameter = Expression.Parameter(typeof(T), nameof(AndIf));
        var visitor = new ExpressionVisitorCustom(newParameter);
        var left = visitor.Visit(leftExpression.Body);
        var right = visitor.Visit(rightExpression.Body);
        var body = Expression.AndAlso(left, right); // 使用AndAlso替代And以支持短路求值
        return Expression.Lambda<Func<T, bool>>(body, newParameter);
    }


    /// <summary>
    /// 将两个表达式进行逻辑或运算，使用短路求值。
    /// </summary>
    /// <typeparam name="T">表达式的参数类型。</typeparam>
    /// <param name="leftExpression">第一个表达式，作为逻辑或运算的左操作数。</param>
    /// <param name="rightExpression">第二个表达式，作为逻辑或运算的右操作数。</param>
    /// <returns>一个新的表达式，表示两个输入表达式的逻辑或运算结果。</returns>
    /// <exception cref="ArgumentNullException">当 leftExpression 或 rightExpression 为 null 时抛出。</exception>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> leftExpression, Expression<Func<T, bool>> rightExpression)
    {
        ArgumentNullException.ThrowIfNull(leftExpression, nameof(leftExpression));
        ArgumentNullException.ThrowIfNull(rightExpression, nameof(rightExpression));

        var newParameter = Expression.Parameter(typeof(T), nameof(Or));
        var visitor = new ExpressionVisitorCustom(newParameter);
        var left = visitor.Visit(leftExpression.Body);
        var right = visitor.Visit(rightExpression.Body);
        var body = Expression.OrElse(left, right); // 使用OrElse替代Or以支持短路求值
        return Expression.Lambda<Func<T, bool>>(body, newParameter);
    }

    /// <summary>
    /// 根据条件将两个表达式进行逻辑或运算，使用短路求值。
    /// 当条件为false时，仅返回左表达式；当条件为true时，返回两个表达式的逻辑或运算结果。
    /// </summary>
    /// <typeparam name="T">表达式的参数类型。</typeparam>
    /// <param name="leftExpression">第一个表达式，作为逻辑或运算的左操作数。</param>
    /// <param name="condition">决定是否执行逻辑或运算的条件委托。</param>
    /// <param name="rightExpression">第二个表达式，作为逻辑或运算的右操作数。</param>
    /// <returns>当条件为true时返回两个表达式的逻辑或运算结果，否则返回左表达式。</returns>
    /// <exception cref="ArgumentNullException">当任何参数为null时抛出。</exception>
    public static Expression<Func<T, bool>> OrIf<T>(this Expression<Func<T, bool>> leftExpression, Func<bool> condition, Expression<Func<T, bool>> rightExpression)
    {
        ArgumentNullException.ThrowIfNull(leftExpression, nameof(leftExpression));
        ArgumentNullException.ThrowIfNull(condition, nameof(condition));
        ArgumentNullException.ThrowIfNull(rightExpression, nameof(rightExpression));

        if (!condition())
        {
            return leftExpression;
        }

        var newParameter = Expression.Parameter(typeof(T), nameof(OrIf));
        var visitor = new ExpressionVisitorCustom(newParameter);
        var left = visitor.Visit(leftExpression.Body);
        var right = visitor.Visit(rightExpression.Body);
        var body = Expression.OrElse(left, right); // 使用OrElse替代Or以支持短路求值
        return Expression.Lambda<Func<T, bool>>(body, newParameter);
    }

    /// <summary>
    /// 对表达式进行逻辑非运算，对表达式的结果取反。
    /// </summary>
    /// <typeparam name="T">表达式的参数类型。</typeparam>
    /// <param name="expr">要进行逻辑非运算的表达式。</param>
    /// <returns>一个新的表达式，表示输入表达式的逻辑非运算结果。</returns>
    /// <exception cref="ArgumentNullException">当 expr 为 null 时抛出。</exception>
    /// <remarks>
    /// 如果输入表达式为 x => x > 5，则输出表达式为 x => !(x > 5)，等价于 x => x &lt;= 5。
    /// </remarks>
    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
    {
        ArgumentNullException.ThrowIfNull(expr, nameof(expr));

        var newParameter = expr.Parameters[0];
        var body = Expression.Not(expr.Body);
        return Expression.Lambda<Func<T, bool>>(body, newParameter);
    }
}