namespace GameFrameX.Core.Config;

/// <summary>
/// </summary>
public abstract class BeanBase : ITypeId
{
    /// <summary>
    /// 获取类型ID
    /// </summary>
    /// <returns></returns>
    public abstract int GetTypeId();
}