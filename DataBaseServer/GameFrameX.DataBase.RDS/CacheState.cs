using System.ComponentModel;
using FreeSql.DataAnnotations;
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
    [Column(IsPrimary = true)]
    [Description("唯一ID")]
    public override long Id { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column(Name = nameof(CreateTime))]
    [Description("创建时间")]
    public override long CreateTime { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    [Column(Name = nameof(CreateId))]
    [Description("创建人")]
    public override long CreateId { get; set; }

    /// <summary>
    /// 修改次数
    /// </summary>
    [Column(Name = nameof(UpdateCount))]
    [Description("修改次数")]
    public override int UpdateCount { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    [Column(Name = nameof(UpdateTime))]
    [Description("修改时间")]
    public override long UpdateTime { get; set; }


    /// <summary>
    /// 删除时间
    /// </summary>
    [Column(Name = nameof(DeleteTime))]
    [Description("删除时间")]
    public override long DeleteTime { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    [Column(Name = nameof(IsDeleted))]
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