namespace GameFrameX.DataBase.State;

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
    bool IsModify();

    /// <summary>
    /// 用于在对象从数据库加载后进行一些特定的处理，如初始化数据或设置状态。
    /// </summary>
    /// <param name="isNew"></param>
    void AfterLoadFromDb(bool isNew);

    /// <summary>
    /// 在对象保存到数据库后调用的方法，可以进行一些后续处理。
    /// </summary>
    void AfterSaveToDb();

    /// <summary>
    /// 将对象序列化转换为字节数组
    /// </summary>
    /// <returns></returns>
    byte[] ToBytes();
}