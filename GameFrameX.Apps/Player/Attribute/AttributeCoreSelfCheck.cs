using System;
using System.Collections.Generic;

namespace GameFrameX.Apps.Player.Attribute;

/// <summary>
/// 属性核心最小自检。
/// </summary>
public static class AttributeCoreSelfCheck
{
    /// <summary>
    /// 执行属性编号和重算公式自检，失败时抛出异常。
    /// </summary>
    public static void Run()
    {
        var attributes = new Dictionary<AttributeType, long>
        {
            [AttributeType.LifeBase] = 1000,
            [AttributeType.LifeAdd] = 200,
            [AttributeType.LifePct] = 1500,
            [AttributeType.LifeFinalAdd] = 30,
            [AttributeType.LifeFinalPct] = 500
        };

        AssertEqual(1480, AttributeCore.RecalculateFinal(attributes, AttributeType.Life), "Life");
        AssertEqual(AttributeType.LifeBase, AttributeCore.GetSlotAttribute(AttributeType.Life, AttributeSlotKind.Base), "LifeBase");
        AssertEqual(AttributeType.LifeAdd, AttributeCore.GetSlotAttribute(AttributeType.Life, AttributeSlotKind.Add), "LifeAdd");
        AssertEqual(AttributeType.LifePct, AttributeCore.GetSlotAttribute(AttributeType.Life, AttributeSlotKind.Pct), "LifePct");
        AssertEqual(AttributeType.LifeFinalAdd, AttributeCore.GetSlotAttribute(AttributeType.Life, AttributeSlotKind.FinalAdd), "LifeFinalAdd");
        AssertEqual(AttributeType.LifeFinalPct, AttributeCore.GetSlotAttribute(AttributeType.Life, AttributeSlotKind.FinalPct), "LifeFinalPct");

        AssertDerived(AttributeType.LifeBase);
        AssertDerived(AttributeType.LifeAdd);
        AssertDerived(AttributeType.LifePct);
        AssertDerived(AttributeType.LifeFinalAdd);
        AssertDerived(AttributeType.LifeFinalPct);

        if (AttributeCore.TryGetFinalAttribute(AttributeType.Life, out _))
        {
            throw new InvalidOperationException("Life 是最终值槽，不应触发反向重算。");
        }

        if (AttributeCore.TryGetFinalAttribute((AttributeType)10010, out _))
        {
            throw new InvalidOperationException("未知派生槽不应触发反向重算。");
        }
    }

    private static void AssertDerived(AttributeType attributeType)
    {
        if (!AttributeCore.TryGetFinalAttribute(attributeType, out var finalAttribute) || finalAttribute != AttributeType.Life)
        {
            throw new InvalidOperationException(attributeType + " 未能反推 Life。");
        }
    }

    private static void AssertEqual<T>(T expected, T actual, string name)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException(name + " 自检失败，expected=" + expected + ", actual=" + actual + "。");
        }
    }
}
