using GameFrameX.DataBase.State;
using GameFrameX.DataBase.Storage;

namespace GameFrameX.DataBase
{
    /// <summary>
    /// 缓存数据对象
    /// </summary>
    public abstract class BaseCacheState : ICacheState
    {
        /// <summary>
        /// 唯一ID。给DB用的
        /// </summary>
        public const string UniqueId = nameof(Id);

        /// <summary>
        /// 唯一ID
        /// </summary>
        public virtual long Id { get; set; }

        /// <summary>
        /// 是否修改
        /// </summary>
        public virtual bool IsModify
        {
            get { return IsChanged().isChanged; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{base.ToString()}[Id={Id}]";
        }

        #region hash

        private StateHash _stateHash;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isNew"></param>
        public virtual void AfterLoadFromDb(bool isNew)
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
        /// 
        /// </summary>
        public void AfterSaveToDb()
        {
            _stateHash.AfterSaveToDb();
        }

        /// <summary>
        /// 将对象序列化转换为字节数组
        /// </summary>
        /// <returns></returns>
        public abstract byte[] ToBytes();

        #endregion

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public long DeleteTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public long CreateId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreateTime { get; set; }

        /// <summary>
        /// 更新次数
        /// </summary>
        public int UpdateCount { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public long UpdateTime { get; set; }
    }
}