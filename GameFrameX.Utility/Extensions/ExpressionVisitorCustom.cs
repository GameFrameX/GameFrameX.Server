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