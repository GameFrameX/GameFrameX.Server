namespace GameFrameX.DataBase.Mongo;

/// <summary>
/// MongoDB索引
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class MongoIndexAttribute : Attribute
{
    /// <summary>
    /// 是否唯一，默认为false
    /// </summary>
    public bool Unique { get; set; } = false;

    /// <summary>
    /// 索引名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 是否升序
    /// </summary>
    public bool IsAscending { get; set; } = true;
}