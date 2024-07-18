using GameFrameX.Extension;

namespace GameFrameX.DBServer.NoSql;

/// <summary>
/// 键Key重命名,用于重写NoSql中的Key
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class KeyValue : Attribute
{
    /// <summary>
    /// 重写Key
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// 重写Key
    /// </summary>
    /// <param name="value"></param>
    public KeyValue(string value)
    {
        value.CheckNotNullOrEmpty(nameof(value));
        Value = value;
    }
}