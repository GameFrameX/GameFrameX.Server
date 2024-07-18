using GameFrameX.DBServer.State;
using GameFrameX.DBServer.Storage;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameFrameX.DBServer.Mongo
{
    /// <summary>
    /// 缓存数据对象
    /// </summary>
    [BsonIgnoreExtraElements(true, Inherited = true)]
    public class CacheState : BaseCacheState
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [BsonId]
        public override long Id { get; set; }

        /// <summary>
        /// 将对象序列化转换为字节数组
        /// </summary>
        /// <returns></returns>
        public override byte[] ToBytes()
        {
            return this.ToBson();
        }
    }
}