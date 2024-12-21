using GameFrameX.Setting;

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
    /// <param name="type">组件类型,值应大于0且小于ActorType.Max并且不为ActorType.Separator</param>
    public ComponentTypeAttribute(ushort type)
    {
        if (type is 0 or GlobalConst.ActorTypeSeparator || type >= GlobalConst.ActorTypeMax)
        {
            throw new InvalidDataException($"无效的组件类型 {type},值应大于{GlobalConst.ActorTypeNone}且小于{GlobalConst.ActorTypeMax}和不为{GlobalConst.ActorTypeSeparator}");
        }

        Type = type;
    }

    /// <summary>
    /// 组件类型
    /// </summary>
    public ushort Type { get; }
}