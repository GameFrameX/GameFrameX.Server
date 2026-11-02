using System.Collections.Generic;

namespace GameFrameX.Apps.Player.Attribute.Entity;

public sealed class PlayerAttributeState : BaseCacheState
{
    /// <summary>
    /// 玩家属性值。Key 使用 <see cref="AttributeType" /> 的整数编号，Value 使用 long 定点数。
    /// </summary>
    public Dictionary<int, long> Values { get; set; } = new Dictionary<int, long>();

    /// <summary>
    /// 读取指定玩家属性值，未保存的属性槽默认返回 0。
    /// </summary>
    /// <param name="attributeType">玩家属性编号。</param>
    /// <returns>已保存的属性值；字典缺失时返回 0。</returns>
    public long GetValue(AttributeType attributeType)
    {
        long value;
        return Values.TryGetValue((int)attributeType, out value) ? value : 0L;
    }
}
