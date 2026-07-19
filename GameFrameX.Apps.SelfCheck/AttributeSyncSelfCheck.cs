using System;
using System.Collections.Generic;
using GameFrameX.Apps.Player.Attribute;
using GameFrameX.Apps.Player.Attribute.Entity;
using GameFrameX.Hotfix.Logic.Player.Attribute;
using GameFrameX.Proto.Proto;

namespace GameFrameX.Apps.SelfCheck;

internal static class AttributeSyncSelfCheck
{
    public static void Run()
    {
        // 增量构建
        var changed = PlayerAttributeSyncBuilder.BuildChanged(AttributeType.Life, 1480L);
        AssertEqual((int)AttributeType.Life, changed.Type, "BuildChanged.Type");
        AssertEqual(1480L, changed.Value, "BuildChanged.Value");

        // 快照构建
        var state = new PlayerAttributeState();
        state.Values[(int)AttributeType.LifeBase] = 1000L;
        state.Values[(int)AttributeType.LifeAdd] = 200L;
        state.Values[(int)AttributeType.LifePct] = 1500L;
        state.Values[(int)AttributeType.Life] = 1480L;
        state.Values[(int)AttributeType.PhysicalAttackBase] = 50L;
        state.Values[(int)AttributeType.PhysicalAttack] = 50L;

        var snapshot = PlayerAttributeSyncBuilder.BuildSnapshot(state);
        // 最终值槽固定 9 个：Life / PhysicalAttack / MagicAttack / PhysicalDefense / MagicDefense / Critical / CriticalDamage / Precision / Block
        AssertEqual(9, snapshot.Attributes.Count, "BuildSnapshot.Count");

        var lifeEntry = snapshot.Attributes.Find(e => e.Type == (int)AttributeType.Life);
        if (lifeEntry == null)
        {
            throw new InvalidOperationException("BuildSnapshot 缺少 Life 条目");
        }

        AssertEqual(1480L, lifeEntry.Value, "BuildSnapshot.Life.Value");
        AssertEqual(1000L, lifeEntry.Base, "BuildSnapshot.Life.Base");
        AssertEqual(200L, lifeEntry.Add, "BuildSnapshot.Life.Add");
        AssertEqual(1500L, lifeEntry.Pct, "BuildSnapshot.Life.Pct");

        // 缺失派生槽默认 0
        var physicalAttackEntry = snapshot.Attributes.Find(e => e.Type == (int)AttributeType.PhysicalAttack);
        if (physicalAttackEntry == null)
        {
            throw new InvalidOperationException("BuildSnapshot 缺少 PhysicalAttack 条目");
        }

        AssertEqual(50L, physicalAttackEntry.Value, "BuildSnapshot.PhysicalAttack.Value");
        AssertEqual(50L, physicalAttackEntry.Base, "BuildSnapshot.PhysicalAttack.Base");
        AssertEqual(0L, physicalAttackEntry.Add, "BuildSnapshot.PhysicalAttack.Add");

        // 空状态快照仍包含全部最终值槽（值为 0）
        var emptySnapshot = PlayerAttributeSyncBuilder.BuildSnapshot(new PlayerAttributeState());
        AssertEqual(9, emptySnapshot.Attributes.Count, "BuildSnapshot.Empty.Count");
    }

    private static void AssertEqual<T>(T expected, T actual, string name)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException(name + " 自检失败，expected=" + expected + ", actual=" + actual + "。");
        }
    }
}
