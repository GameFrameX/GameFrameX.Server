namespace GameFrameX.Core.Abstractions.Attribute;

/// <summary>
/// 此方法使用了弃元运算符，不会等待执行(将强制追加到队列末端执行)
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class DiscardAttribute : System.Attribute
{
}