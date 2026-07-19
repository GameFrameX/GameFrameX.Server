using System;
using System.Collections.Generic;
using GameFrameX.Apps.Player.Attribute;
using GameFrameX.Hotfix.Logic.Player.Attribute;

namespace GameFrameX.Apps.SelfCheck;

internal static class AttributeAgentSelfCheck
{
    public static void Run()
    {
        VerifyMutation();
        VerifyDefaultInitialization();
    }

    private static void VerifyMutation()
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

    private static void VerifyDefaultInitialization()
    {
        var values = new Dictionary<int, long>();
        AssertEqual(true, PlayerInitialAttributeDefaults.ApplyMissing(values), "PlayerInitialAttributeDefaults.FirstChanged");
        AssertEqual(1000, values[(int)AttributeType.LifeBase], "PlayerInitialAttributeDefaults.LifeBase");
        AssertEqual(1000, values[(int)AttributeType.Life], "PlayerInitialAttributeDefaults.Life");
        AssertEqual(100, values[(int)AttributeType.PhysicalAttackBase], "PlayerInitialAttributeDefaults.PhysicalAttackBase");
        AssertEqual(100, values[(int)AttributeType.PhysicalAttack], "PlayerInitialAttributeDefaults.PhysicalAttack");

        var count = values.Count;
        values[(int)AttributeType.LifeBase] = 1200;
        values[(int)AttributeType.Life] = 1200;
        AssertEqual(false, PlayerInitialAttributeDefaults.ApplyMissing(values), "PlayerInitialAttributeDefaults.RepeatChanged");
        AssertEqual(count, values.Count, "PlayerInitialAttributeDefaults.RepeatCount");
        AssertEqual(1200, values[(int)AttributeType.LifeBase], "PlayerInitialAttributeDefaults.KeepLifeBase");
        AssertEqual(1200, values[(int)AttributeType.Life], "PlayerInitialAttributeDefaults.KeepLife");
    }

    private static void AssertEqual<T>(T expected, T actual, string name)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException(name + " 自检失败，expected=" + expected + ", actual=" + actual + "。");
        }
    }
}
