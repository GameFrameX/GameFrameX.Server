namespace GameFrameX.DBServer.State;

/// <summary>
/// 缓存数据对象接口
/// </summary>
public interface ICacheState : ISafeDelete, ISafeCreate, ISafeUpdate
{
    /// <summary>
    /// 唯一ID
    /// </summary>
    long Id { get; set; }

    /// <summary>
    /// 是否修改
    /// </summary>
    bool IsModify { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isNew"></param>
    void AfterLoadFromDB(bool isNew);

    /// <summary>
    /// 
    /// </summary>
    void AfterSaveToDB();
}