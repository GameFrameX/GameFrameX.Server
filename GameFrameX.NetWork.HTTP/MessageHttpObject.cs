using System.Text.Json.Serialization;
using GameFrameX.Foundation.Json;
using GameFrameX.Utility;
using ProtoBuf;

namespace GameFrameX.NetWork.HTTP
{
    /// <summary>
    /// HTTP消息包装基类
    /// </summary>
    [ProtoContract]
    public sealed class MessageHttpObject
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 消息序列号
        /// </summary>
        [ProtoMember(2)]
        public int UniqueId { get; set; }

        /// <summary>
        /// 消息体
        /// </summary>
        [JsonIgnore]
        [ProtoMember(3)]
        public byte[] Body { get; set; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonHelper.Serialize(this);
        }
    }
}