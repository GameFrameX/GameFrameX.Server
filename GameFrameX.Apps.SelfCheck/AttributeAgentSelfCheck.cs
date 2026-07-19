using System;
using System.Collections.Generic;
using GameFrameX.Apps.Player.Attribute;
using GameFrameX.Hotfix.Logic.Player.Attribute;

namespace GameFrameX.Apps.SelfCheck;

internal static class AttributeAgentSelfCheck
{
    public static void Run()
    {
        var values = new Dictionary<int, long>
        {
            [(int)AttributeType.LifeBase] = 1000,
            [(int)AttributeType.Life] = 1000
        };

        var result = PlayerAttributeMutation.ApplyValue(values, AttributeType.LifeAdd, 200, false);
        AssertEqual(200, values[(int)AttributeType.LifeAdd], "PlayerAttributeComponentAgent.LifeAdd");
        AssertEqual(1200, values[(int)AttributeType.Life], "PlayerAttributeComponentAgent.Life");
        AssertEqual(AttributeType.Life, result.FinalAttributeType, "PlayerAttributeComponentAgent.FinalAttribute");
        AssertEqual(true, result.ShouldDispatch, "PlayerAttributeComponentAgent.DispatchChanged");

        result = PlayerAttributeMutation.ApplyValue(values, AttributeType.LifeAdd, 200, false);
        AssertEqual(false, result.StateChanged, "PlayerAttributeComponentAgent.UnchangedState");
        AssertEqual(false, result.ShouldDispatch, "PlayerAttributeComponentAgent.UnchangedDispatch");

        result = PlayerAttributeMutation.ApplyValue(values, AttributeType.LifeAdd, 300, true);
        AssertEqual(1300, values[(int)AttributeType.Life], "PlayerAttributeComponentAgent.SilentLife");
        AssertEqual(false, result.ShouldDispatch, "PlayerAttributeComponentAgent.SilentDispatch");
    }

    private static void AssertEqual<T>(T expected, T actual, string name)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException(name + " 自检失败，expected=" + expected + ", actual=" + actual + "。");
        }
    }
}
