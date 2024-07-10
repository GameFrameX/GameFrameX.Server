using System.Linq.Expressions;

namespace GameFrameX.Extension
{
    /// <summary>
    /// 提供对 <see cref="Expression"/> 类型的扩展方法。
    /// </summary>
    public static class ExpressionExtension
    {
        /// <summary>
        /// 将两个表达式进行逻辑与运算。
        /// </summary>
        /// <typeparam name="T">表达式的参数类型。</typeparam>
        /// <param name="leftExpression">第一个表达式。</param>
        /// <param name="rightExpression">第二个表达式。</param>
        /// <returns>逻辑与运算后的表达式。</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> leftExpression, Expression<Func<T, bool>> rightExpression)
        {
            if (leftExpression == null)
            {
                throw new ArgumentNullException(nameof(leftExpression) + " null不能处理");
            }

            if (rightExpression == null)
            {
                throw new ArgumentNullException(nameof(rightExpression) + " null不能处理");
            }

            var newParameter = Expression.Parameter(typeof(T), "x");
            var visitor      = new ExpressionVisitorCustom(newParameter);
            var left         = visitor.Visit(leftExpression.Body);
            var right        = visitor.Visit(rightExpression.Body);
            var body         = Expression.And(left, right);
            return Expression.Lambda<Func<T, bool>>(body, newParameter);
        }

        /// <summary>
        /// 将两个表达式进行逻辑与运算。
        /// </summary>
        /// <typeparam name="T">表达式的参数类型。</typeparam>
        /// <param name="leftExpression">第一个表达式。</param>
        /// <param name="condition">条件。</param>
        /// <param name="rightExpression">第二个表达式。</param>
        /// <returns>逻辑与运算后的表达式。</returns>
        public static Expression<Func<T, bool>> AndIf<T>(this Expression<Func<T, bool>> leftExpression, Func<bool> condition, Expression<Func<T, bool>> rightExpression)
        {
            if (leftExpression == null)
            {
                throw new ArgumentNullException(nameof(leftExpression) + " null不能处理");
            }

            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition) + " null不能处理");
            }

            if (rightExpression == null)
            {
                throw new ArgumentNullException(nameof(rightExpression) + " null不能处理");
            }

            if (!condition())
            {
                return leftExpression;
            }

            var newParameter = Expression.Parameter(typeof(T), "x");
            var visitor      = new ExpressionVisitorCustom(newParameter);
            var left         = visitor.Visit(leftExpression.Body);
            var right        = visitor.Visit(rightExpression.Body);
            var body         = Expression.And(left, right);
            return Expression.Lambda<Func<T, bool>>(body, newParameter);
        }


        /// <summary>
        /// 将两个表达式进行逻辑或运算。
        /// </summary>
        /// <typeparam name="T">表达式的参数类型。</typeparam>
        /// <param name="leftExpression">第一个表达式。</param>
        /// <param name="rightExpression">第二个表达式。</param>
        /// <returns>逻辑或运算后的表达式。</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> leftExpression, Expression<Func<T, bool>> rightExpression)
        {
            if (leftExpression == null)
            {
                throw new ArgumentNullException(nameof(leftExpression) + " null不能处理");
            }

            if (rightExpression == null)
            {
                throw new ArgumentNullException(nameof(rightExpression) + " null不能处理");
            }

            var newParameter = Expression.Parameter(typeof(T), "x");
            var visitor      = new ExpressionVisitorCustom(newParameter);
            var left         = visitor.Visit(leftExpression.Body);
            var right        = visitor.Visit(rightExpression.Body);
            var body         = Expression.Or(left, right);
            return Expression.Lambda<Func<T, bool>>(body, newParameter);
        }

        /// <summary>
        /// 将两个表达式使用逻辑或操作符进行组合，如果指定的条件为真则使用右表达式，否则使用左表达式。
        /// </summary>
        /// <typeparam name="T">表达式的参数类型。</typeparam>
        /// <param name="leftExpression">左表达式。</param>
        /// <param name="condition">条件委托，用于确定使用哪个表达式。</param>
        /// <param name="rightExpression">右表达式。</param>
        /// <returns>组合后的表达式。</returns>
        public static Expression<Func<T, bool>> OrIf<T>(this Expression<Func<T, bool>> leftExpression, Func<bool> condition, Expression<Func<T, bool>> rightExpression)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition) + " null不能处理");
            }

            if (leftExpression == null)
            {
                throw new ArgumentNullException(nameof(leftExpression) + " null不能处理");
            }

            if (rightExpression == null)
            {
                throw new ArgumentNullException(nameof(rightExpression) + " null不能处理");
            }

            if (!condition())
            {
                return leftExpression;
            }

            var newParameter = Expression.Parameter(typeof(T), "x");
            var visitor      = new ExpressionVisitorCustom(newParameter);
            var left         = visitor.Visit(leftExpression.Body);
            var right        = visitor.Visit(rightExpression.Body);
            var body         = Expression.Or(left, right);
            return Expression.Lambda<Func<T, bool>>(body, newParameter);
        }

        /// <summary>
        /// 对表达式进行逻辑非运算。
        /// </summary>
        /// <typeparam name="T">表达式的参数类型。</typeparam>
        /// <param name="expr">要进行逻辑非运算的表达式。</param>
        /// <returns>逻辑非运算后的表达式。</returns>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
        {
            if (expr == null)
            {
                throw new ArgumentNullException(nameof(expr) + " null不能处理");
            }

            var newParameter = expr.Parameters[0];
            var body         = Expression.Not(expr.Body);
            return Expression.Lambda<Func<T, bool>>(body, newParameter);
        }
    }
}