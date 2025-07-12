using System.Linq.Expressions;

namespace GameFrameX.Utility.Extensions;

/// <summary>
/// 表达式访问器的自定义实现。
/// </summary>
public class ExpressionVisitorCustom : ExpressionVisitor
{
    private ParameterExpression _targetParameter;
    
    /// <summary>
    /// 初始化 <see cref="ExpressionVisitorCustom" /> 类的新实例。
    /// </summary>
    /// <param name="param">访问器中的参数表达式。不能为 null。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="param"/> 为 null 时抛出。</exception>
    public ExpressionVisitorCustom(ParameterExpression param)
    {
        ArgumentNullException.ThrowIfNull(param, nameof(param));
        Parameter = param;
    }

    /// <summary>
    /// 获取或设置访问器中的参数表达式。
    /// </summary>
    public ParameterExpression Parameter { get; }

    /// <summary>
    /// 访问Lambda表达式，正确处理参数替换。
    /// </summary>
    /// <typeparam name="T">Lambda表达式的委托类型。</typeparam>
    /// <param name="node">要访问的Lambda表达式。</param>
    /// <returns>返回访问后的Lambda表达式。</returns>
    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        ArgumentNullException.ThrowIfNull(node, nameof(node));
        
        // 记录要替换的目标参数（第一个参数）
        var originalTarget = _targetParameter;
        _targetParameter = node.Parameters.Count > 0 ? node.Parameters[0] : null;
        
        try
        {
            // 如果Lambda表达式只有一个参数，则替换它
            if (node.Parameters.Count == 1)
            {
                var newParameters = new[] { Parameter };
                var newBody = Visit(node.Body);
                return Expression.Lambda<T>(newBody, newParameters);
            }
            
            // 如果有多个参数，只替换第一个参数
            if (node.Parameters.Count > 1)
            {
                var newParameters = new ParameterExpression[node.Parameters.Count];
                newParameters[0] = Parameter;
                for (int i = 1; i < node.Parameters.Count; i++)
                {
                    newParameters[i] = node.Parameters[i];
                }
                
                var newBody = Visit(node.Body);
                return Expression.Lambda<T>(newBody, newParameters);
            }
            
            return base.VisitLambda(node);
        }
        finally
        {
            // 恢复原来的目标参数
            _targetParameter = originalTarget;
        }
    }

    /// <summary>
    /// 访问参数表达式。
    /// </summary>
    /// <param name="node">要访问的参数表达式。</param>
    /// <returns>返回访问后的表达式。</returns>
    protected override Expression VisitParameter(ParameterExpression node)
    {
        ArgumentNullException.ThrowIfNull(node, nameof(node));
        
        // 如果在Lambda上下文中，只替换目标参数
        if (_targetParameter != null)
        {
            return ReferenceEquals(node, _targetParameter) ? Parameter : node;
        }
        
        // 如果不在Lambda上下文中，总是返回构造函数中的参数（保持原有行为）
        return Parameter;
    }
}