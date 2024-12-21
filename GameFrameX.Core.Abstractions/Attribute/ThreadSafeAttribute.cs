namespace GameFrameX.Core.Abstractions.Attribute;

/// <summary>
/// 此方法线程安全
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ThreadSafeAttribute : System.Attribute
{
}