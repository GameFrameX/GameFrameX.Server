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
/// 缓存数据对象
/// </summary>
public abstract class BaseCacheState : ICacheState, IEntity
{
    /// <summary>
    /// 唯一ID
    /// </summary>
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
    public virtual bool? IsDeleted { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    public virtual long DeleteTime { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public virtual long CreateId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public virtual long CreateTime { get; set; }

    /// <summary>
    /// 更新次数
    /// </summary>
    public virtual int UpdateCount { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
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
    /// <param name="isNew">是否是新创建的实例，true表示是新创建的实例，false表示不是</param>
    public virtual void LoadFromDbPostHandler(bool isNew = false)
    {
        _stateHash = new StateHash(this, isNew);
    }

    /// <summary>
    /// 是否修改
    /// </summary>
    /// <returns></returns>
    public virtual (bool isChanged, byte[] data) IsChanged()
    {
        CheckStateHash();
        return _stateHash.IsChanged();
    }

    /// <summary>
    /// 是否由ID引起的变化
    /// </summary>
    /// <returns></returns>
    public virtual (bool isChanged, long stateId, byte[] data) IsChangedWithId()
    {
        CheckStateHash();
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
        CheckStateHash();
        _stateHash.SaveToDbPostHandler();
    }

    /// <summary>
    /// 检查StateHash对象是否存在
    /// </summary>
    private void CheckStateHash()
    {
        if (_stateHash.IsNotNull())
        {
            return;
        }

        LoadFromDbPostHandler();
    }

    /// <summary>
    /// 将对象序列化转换为字节数组
    /// </summary>
    /// <returns></returns>
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