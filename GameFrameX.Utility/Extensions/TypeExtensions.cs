namespace GameFrameX.Utility.Extensions;

/// <summary>
/// 提供对 <see cref="Type" /> 类型的扩展方法。
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// 判断类型是否实现了指定的接口。
    /// 此方法用于检查一个具体类型是否实现了目标接口。
    /// </summary>
    /// <param name="self">要判断的类型。必须是非空的具体类型。</param>
    /// <param name="target">要判断的接口类型。必须是非空的接口类型。</param>
    /// <param name="directOnly">是否只检查直接实现的接口。
    /// 当设置为true时，只检查直接实现的接口；
    /// 当设置为false时，同时检查继承链上的所有接口。</param>
    /// <returns>
    /// 满足以下所有条件时返回true:
    /// 1. self和target参数都不为null
    /// 2. target是接口类型
    /// 3. self不是接口类型或抽象类型
    /// 4. self实现了target接口（根据directOnly参数决定检查范围）
    /// 否则返回false
    /// </returns>
    public static bool IsImplWithInterface(this Type self, Type target, bool directOnly = false)
    {
        // 参数有效性检查
        if (target == null || self == null)
        {
            return false;
        }

        // 确保target是接口类型
        if (!target.IsInterface)
        {
            return false;
        }

        // 检查是否是接口类型或抽象类型
        if (self.IsInterface || self.IsAbstract)
        {
            return false;
        }

        if (directOnly)
        {
            // 只检查直接实现的接口
            return self.GetInterfaces().Any(i => i == target);
        }

        // 检查所有实现的接口（包括继承的接口）
        return self.GetInterfaces().Any(i => i == target || i.GetInterfaces().Contains(target));
    }
}