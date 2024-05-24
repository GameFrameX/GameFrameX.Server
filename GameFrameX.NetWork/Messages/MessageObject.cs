using GameFrameX.Setting;
using Newtonsoft.Json;
using ProtoBuf;

namespace GameFrameX.NetWork.Messages
{
    [ProtoContract]
    public abstract class MessageObject : BaseMessageObject, IMessage
    {
        /// <summary>
        /// 消息唯一id
        /// </summary>
        [JsonIgnore]
        public int UniId { get; set; }

        public string ToSendMessageString(ServerType srcServerType, ServerType destServerType)
        {
            return $"---发送[{srcServerType} To {destServerType}] {ToMessageString()}";
        }

        public string ToReceiveMessageString(ServerType srcServerType, ServerType destServerType)
        {
            return $"---收到[{srcServerType} To {destServerType}] {ToMessageString()}";
        }
    }
}