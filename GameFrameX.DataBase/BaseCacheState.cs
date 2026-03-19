// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using GameFrameX.DataBase.Abstractions;
using GameFrameX.DataBase.Storage;
using GameFrameX.Foundation.Json;
using GameFrameX.Foundation.Extensions;
using MongoDB.Entities;

namespace GameFrameX.DataBase;

/// <summary>
/// 缓存数据对象。
/// </summary>
/// <remarks>
/// Cache data object base class.
/// </remarks>
public abstract class BaseCacheState : ICacheState, IEntity
{
    /// <summary>
    /// 唯一ID。
    /// </summary>
    /// <remarks>
    /// Unique identifier.
    /// </remarks>
    /// <value>唯一ID / Unique ID</value>
    public virtual long Id { get; set; }

    /// <summary>
    /// 获取数据对象是否修改。
    /// </summary>
    /// <remarks>
    /// Get whether the data object has been modified.
    /// </remarks>
    /// <returns>如果已修改则返回 <c>true</c>；否则返回 <c>false</c> / <c>true</c> if modified; otherwise <c>false</c></returns>
    public virtual bool IsModify()
    {
        return IsChanged().isChanged;
    }

    /// <summary>
    /// 是否删除。
    /// </summary>
    /// <remarks>
    /// Whether the record is deleted.
    /// </remarks>
    /// <value>是否删除 / Whether deleted</value>
    public virtual bool? IsDeleted { get; set; }

    /// <summary>
    /// 删除时间。
    /// </summary>
    /// <remarks>
    /// Deletion time.
    /// </remarks>
    /// <value>删除时间, Unix时间戳毫秒 / Deletion time, Unix timestamp in milliseconds</value>
    public virtual long? DeleteTime { get; set; }

    /// <summary>
    /// 删除人ID。
    /// </summary>
    /// <remarks>
    /// ID of the user who deleted the record.
    /// </remarks>
    /// <value>删除人ID / Deleter ID</value>
    public long? DeletedId { get; set; }

    /// <summary>
    /// 删除人的用户名。
    /// </summary>
    /// <remarks>
    /// Username of the user who deleted the record.
    /// </remarks>
    /// <value>删除人的用户名 / Deleter username</value>
    public string DeletedName { get; set; } = "";

    /// <summary>
    /// 创建人ID, 通常是用户ID，用于记录创建该数据对象的用户，默认值为0。
    /// </summary>
    /// <remarks>
    /// Creator ID, typically the user ID, used to record the user who created this data object, default value is 0.
    /// </remarks>
    /// <value>创建人ID / Creator ID</value>
    public virtual long? CreatedId { get; set; }

    /// <summary>
    /// 创建时间, Unix时间戳毫秒。
    /// </summary>
    /// <remarks>
    /// Creation time, Unix timestamp in milliseconds.
    /// </remarks>
    /// <value>创建时间 / Creation time</value>
    public virtual long CreatedTime { get; set; }

    /// <summary>
    /// 创建人的名称。
    /// </summary>
    /// <remarks>
    /// Name of the creator.
    /// </remarks>
    /// <value>创建人的名称 / Creator name</value>
    public string CreatedName { get; set; } = "";

    /// <summary>
    /// 更新次数。
    /// </summary>
    /// <remarks>
    /// Number of updates.
    /// </remarks>
    /// <value>更新次数 / Update count</value>
    public virtual int? UpdateCount { get; set; }

    /// <summary>
    /// 更新时间, Unix时间戳毫秒。
    /// </summary>
    /// <remarks>
    /// Update time, Unix timestamp in milliseconds.
    /// </remarks>
    /// <value>更新时间 / Update time</value>
    public virtual long? UpdateTime { get; set; }

    /// <summary>
    /// 更新人ID。
    /// </summary>
    /// <remarks>
    /// ID of the user who last updated the record.
    /// </remarks>
    /// <value>更新人ID / Updater ID</value>
    public long? UpdatedId { get; set; }

    /// <summary>
    /// 更新人名称。
    /// </summary>
    /// <remarks>
    /// Name of the user who last updated the record.
    /// </remarks>
    /// <value>更新人名称 / Updater name</value>
    public string UpdatedName { get; set; } = "";

    /// <summary>
    /// 转换为字符串, 包含所有属性。
    /// </summary>
    /// <remarks>
    /// Convert to string, including all properties.
    /// </remarks>
    /// <returns>对象的JSON字符串表示 / JSON string representation of the object</returns>
    public override string ToString()
    {
        return JsonHelper.Serialize(this);
    }

    #region hash

    private StateHash _stateHash;


    /// <summary>
    /// 用于在对象从数据库加载后进行一些特定的处理，如初始化数据或设置状态。
    /// </summary>
    /// <remarks>
    /// Used to perform specific processing after the object is loaded from the database, such as initializing data or setting state.
    /// </remarks>
    /// <param name="isNew">是否是新创建的实例，true表示是新创建的实例，false表示不是 / Whether it is a newly created instance, true means it is new, false means it is not</param>
    public virtual void LoadFromDbPostHandler(bool isNew = false)
    {
        _stateHash = new StateHash(this, isNew);
    }

    /// <summary>
    /// 是否修改。
    /// </summary>
    /// <remarks>
    /// Check if the object has been modified.
    /// </remarks>
    /// <returns>包含是否修改和字节数据的元组 / Tuple containing whether modified and byte data</returns>
    public virtual (bool isChanged, byte[] data) IsChanged()
    {
        CheckStateHash();
        return _stateHash.IsChanged();
    }

    /// <summary>
    /// 是否由ID引起的变化。
    /// </summary>
    /// <remarks>
    /// Check if the change was caused by ID.
    /// </remarks>
    /// <returns>包含是否修改、状态ID和字节数据的元组 / Tuple containing whether modified, state ID, and byte data</returns>
    public virtual (bool isChanged, long stateId, byte[] data) IsChangedWithId()
    {
        CheckStateHash();
        var res = _stateHash.IsChanged();
        return (res.Item1, Id, res.Item2);
    }

    /// <summary>
    /// 仅DBModel.Mongodb时调用。
    /// </summary>
    /// <remarks>
    /// Called only when using DBModel.Mongodb.
    /// </remarks>
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
    /// <remarks>
    /// Method called after the object is saved to the database, can perform some post-processing.
    /// </remarks>
    public void SaveToDbPostHandler()
    {
        CheckStateHash();
        _stateHash.SaveToDbPostHandler();
    }

    /// <summary>
    /// 检查StateHash对象是否存在。
    /// </summary>
    /// <remarks>
    /// Check if the StateHash object exists.
    /// </remarks>
    private void CheckStateHash()
    {
        if (_stateHash.IsNotNull())
        {
            return;
        }

        LoadFromDbPostHandler();
    }

    /// <summary>
    /// 将对象序列化转换为字节数组。
    /// </summary>
    /// <remarks>
    /// Serialize the object to a byte array.
    /// </remarks>
    /// <returns>序列化后的字节数组 / Serialized byte array</returns>
    public abstract byte[] ToBytes();

    #endregion

    /// <summary>
    /// Generate and return a new ID from this method. It will be used when saving new entities that don't have their ID set.
    /// I.e. if an entity has a default ID value (determined by calling <see cref="M:MongoDB.Entities.IEntity.HasDefaultID" /> method),
    /// this method will be called for obtaining a new ID value. If you're not doing custom ID generation, simply do
    /// <c>return ObjectId.GenerateNewId().ToString()</c>
    /// </summary>
    public object GenerateNewID()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// When saving entities, this method will be called in order to determine if <see cref="M:MongoDB.Entities.IEntity.GenerateNewID" /> needs to be called.
    /// If this method returns <c>'true'</c>, <see cref="M:MongoDB.Entities.IEntity.GenerateNewID" /> method is called and the ID (primary key) of the entity is populated.
    /// If <c>'false'</c> is returned, it is assumed that ID generation is not required and the entity already has a non-default ID value.
    /// </summary>
    public bool HasDefaultID()
    {
        return Id == 0;
    }
}