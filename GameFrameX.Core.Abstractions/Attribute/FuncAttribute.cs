namespace GameFrameX.Core.Abstractions.Attribute;

/// <summary>
/// 有关组件的功能属性
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class FuncAttribute : System.Attribute
{
    /// <summary>
    /// 功能标识
    /// </summary>
    public readonly short Func;

    /// <summary>
    /// 初始化功能属性
    /// </summary>
    /// <param name="func">功能标识</param>
    public FuncAttribute(short func)
    {
        Func = func;
    }
}