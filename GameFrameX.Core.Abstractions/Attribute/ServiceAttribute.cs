namespace GameFrameX.Core.Abstractions.Attribute
{
    /// <summary>
    /// 此方法会提供给其他Actor访问(对外提供服务)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ServiceAttribute : System.Attribute
    {
    }
}