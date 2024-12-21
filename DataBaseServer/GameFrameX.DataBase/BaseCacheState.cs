using GameFrameX.DataBase.Abstractions;
using GameFrameX.DataBase.Storage;
using GameFrameX.Utility;
using SqlSugar;

namespace GameFrameX.DataBase;

/// <summary>
/// 缓存数据对象
/// </summary>
[SugarTable]
public abstract class BaseCacheState : ICacheState
{
    /// <summary>
    /// 唯一ID。给DB用的
    /// </summary>
    public const string UniqueId = nameof(Id);

    /// <summary>
    /// 唯一ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "唯一ID")]
    public virtual long Id { get; set; }

    /// <summary>
    /// 获取数据对象是否修改
    /// </summary>
    public virtual bool IsModify()
    {
        return IsChanged().isChanged;
    }

    /// <summary>
    /// 是否删除
    /// </summary>
    [SugarColumn(ColumnDescription = "是否删除标记")]
    public virtual bool IsDeleted { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    [SugarColumn(ColumnDescription = "删除时间")]
    public virtual long DeleteTime { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    [SugarColumn(ColumnDescription = "创建人")]
    public virtual long CreateId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间", InsertServerTime = true)]
    public virtual long CreateTime { get; set; }

    /// <summary>
    /// 更新次数
    /// </summary>
    [SugarColumn(ColumnDescription = "修改次数", IsEnableUpdateVersionValidation = true)]
    public virtual int UpdateCount { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "修改时间", UpdateServerTime = true)]
    public virtual long UpdateTime { get; set; }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonHelper.Serialize(this);
    }

    #region hash

    private StateHash _stateHash;


    /// <summary>
    /// 用于在对象从数据库加载后进行一些特定的处理，如初始化数据或设置状态。
    /// </summary>
    /// <param name="isNew"></param>
    public virtual void LoadFromDbPostHandler(bool isNew)
    {
        _stateHash = new StateHash(this, isNew);
    }

    /// <summary>
    /// 是否修改
    /// </summary>
    /// <returns></returns>
    public virtual (bool isChanged, byte[] data) IsChanged()
    {
        return _stateHash.IsChanged();
    }

    /// <summary>
    /// 是否由ID引起的变化
    /// </summary>
    /// <returns></returns>
    public virtual (bool isChanged, long stateId, byte[] data) IsChangedWithId()
    {
        var res = _stateHash.IsChanged();
        return (res.Item1, Id, res.Item2);
    }

    /// <summary>
    /// 仅DBModel.Mongodb时调用
    /// </summary>
    public virtual void BeforeSaveToDb()
    {
        // var db = GameDb.As<RocksDBConnection>().CurDataBase;
        // var table = db.GetTable<SaveTimestamp>();
        // var saveState = new SaveTimestamp
        // {
        //     //此处使用UTC时间
        //     Timestamp = TimeUtils.CurrentTimeMillisUTC(),
        //     StateName = GetType().FullName,
        //     StateId = Id.ToString(),
        // };
        // table.Set(saveState.Key, saveState);
    }


    /// <summary>
    /// 在对象保存到数据库后调用的方法，可以进行一些后续处理。
    /// </summary>
    public void SaveToDbPostHandler()
    {
        _stateHash.SaveToDbPostHandler();
    }

    /// <summary>
    /// 将对象序列化转换为字节数组
    /// </summary>
    /// <returns></returns>
    public abstract byte[] ToBytes();

    #endregion
}