using GameFrameX.Setting;
using GameFrameX.Utility;
using Newtonsoft.Json;
using ProtoBuf;

namespace GameFrameX.NetWork.Messages
{
    [ProtoContract]
    public abstract class MessageObject : IMessage
    {
        /// <summary>
        /// 消息唯一id
        /// </summary>
        [JsonIgnore]
        public int UniId { get; set; }

        /// <summary>
        /// 消息ID
        /// </summary>
        [JsonIgnore]
        [ProtoMember(998)]
        public int MessageId { get; set; }

        /// <summary>
        /// 消息的唯一ID
        /// </summary>
        [ProtoMember(999)]
        public int UniqueId { get; set; }

        public MessageObject()
        {
            UpdateUniqueId();
        }

        public void UpdateUniqueId()
        {
            UniqueId = UtilityIdGenerator.GetNextUniqueIntId();
        }

        public string ToSendMessageString(ServerType srcServerType, ServerType destServerType)
        {
            return $"---发送[{srcServerType} To {destServerType}] {ToMessageString()}";
        }

        public string ToReceiveMessageString(ServerType srcServerType, ServerType destServerType)
        {
            return $"---收到[{srcServerType} To {destServerType}] {ToMessageString()}";
        }

        public string ToMessageString()
        {
            return $"消息ID:[{MessageId},{GetType().Name}] 消息内容:{JsonHelper.Serialize(this)}";
        }

        public override string ToString()
        {
            return JsonHelper.Serialize(this);
        }
    }
}