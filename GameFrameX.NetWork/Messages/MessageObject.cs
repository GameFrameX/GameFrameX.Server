using Newtonsoft.Json;
using ProtoBuf;

namespace GameFrameX.NetWork.Messages
{
    [ProtoContract]
    public abstract class MessageObject :BaseMessageObject, IMessage
    {
        /// <summary>
        /// 消息唯一id
        /// </summary>
        [JsonIgnore]
        public int UniId { get; set; }
    }
}