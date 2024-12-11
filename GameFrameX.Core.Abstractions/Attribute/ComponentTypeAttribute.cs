namespace GameFrameX.Core.Abstractions.Attribute;

/// <summary>
/// 组件类型标记
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ComponentTypeAttribute : System.Attribute
{
    /// <summary>
    /// 组件类型
    /// </summary>
    public ushort Type { get; }

    /// <summary>
    /// 组件类型
    /// </summary>
    /// <param name="type">组件类型</param>
    public ComponentTypeAttribute(ActorType type) : this((ushort)type)
    {
    }

    /// <summary>
    /// 组件类型
    /// </summary>
    /// <param name="type">组件类型,值应大于0且小于ActorType.Max并且不为ActorType.Separator</param>
    public ComponentTypeAttribute(ushort type)
    {
        if (type is 0 or (ushort)ActorType.Separator || type >= (ushort)ActorType.Max)
        {
            throw new InvalidDataException($"无效的组件类型 {type},值应大于{ActorType.None}且小于{ActorType.Max}和不为{ActorType.Separator}");
        }

        Type = type;
    }
}