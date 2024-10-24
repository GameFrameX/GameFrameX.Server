using System.ComponentModel;
using GameFrameX.Utility;

namespace GameFrameX.DataBase.RDS;

/// <summary>
/// 缓存数据对象
/// </summary>
public class CacheState : BaseCacheState
{
    /// <summary>
    /// 唯一ID
    /// </summary>
    [Description("唯一ID")]
    public override long Id { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Description("创建时间")]
    public override long CreateTime { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    [Description("创建人")]
    public override long CreateId { get; set; }

    /// <summary>
    /// 修改次数
    /// </summary>
    [Description("修改次数")]
    public override int UpdateCount { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    [Description("修改时间")]
    public override long UpdateTime { get; set; }


    /// <summary>
    /// 删除时间
    /// </summary>
    [Description("删除时间")]
    public override long DeleteTime { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    [Description("是否删除标记")]
    public override bool IsDeleted { get; set; }

    /// <summary>
    /// 将对象序列化转换为字节数组
    /// </summary>
    /// <returns></returns>
    public override byte[] ToBytes()
    {
        var json  = JsonHelper.Serialize(this);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        return bytes;
    }
}