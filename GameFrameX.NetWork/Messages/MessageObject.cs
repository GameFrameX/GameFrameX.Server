using GameFrameX.NetWork.Abstractions;
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
        public int MessageId { get; private set; }

        /// <summary>
        /// 设置消息ID
        /// </summary>
        /// <param name="messageId"></param>
        public void SetMessageId(int messageId)
        {
            MessageId = messageId;
        }

        /// <summary>
        /// 消息的唯一ID
        /// </summary>
        [JsonIgnore]
        [ProtoMember(999)]
        public int UniqueId { get; set; }

        /// <summary>
        /// 消息业务类型
        /// </summary>
        public MessageOperationType OperationType { get; private set; }

        /// <summary>
        /// 设置消息业务类型
        /// </summary>
        /// <param name="messageOperationType">消息业务类型 </param>
        public void SetOperationType(MessageOperationType messageOperationType)
        {
            OperationType = messageOperationType;
        }

        /// <summary>
        /// 
        /// </summary>
        protected MessageObject()
        {
            UpdateUniqueId();
        }


        /// <summary>
        /// 更新唯一消息ID
        /// </summary>
        public void UpdateUniqueId()
        {
            UniqueId = UtilityIdGenerator.GetNextUniqueIntId();
        }

        /// <summary>
        /// 设置唯一消息ID
        /// </summary>
        /// <param name="uniqueId"></param>
        public void SetUniqueId(int uniqueId)
        {
            UniqueId = uniqueId;
        }

        /// <summary>
        /// 获取发送消息字符串
        /// </summary>
        /// <param name="srcServerType">发送方</param>
        /// <param name="destServerType">接收方</param>
        /// <returns></returns>
        public string ToSendMessageString(ServerType srcServerType, ServerType destServerType)
        {
            return $"---发送[{srcServerType} To {destServerType}] {ToMessageString()}";
        }

        /// <summary>
        /// 获取接收消息字符串
        /// </summary>
        /// <param name="srcServerType">发送方</param>
        /// <param name="destServerType">接收方</param>
        /// <returns></returns>
        public string ToReceiveMessageString(ServerType srcServerType, ServerType destServerType)
        {
            return $"---收到[{srcServerType} To {destServerType}] {ToMessageString()}";
        }

        /// <summary>
        /// 获取消息字符串
        /// </summary>
        /// <returns></returns>
        public string ToMessageString()
        {
            return $"消息ID:[{MessageId}=MainId: {MessageManager.GetMainId(MessageId)} + SubId: {MessageManager.GetSubId(MessageId)},{GetType().Name}] 消息内容:{JsonHelper.Serialize(this)}";
        }

        public override string ToString()
        {
            return JsonHelper.Serialize(this);
        }
    }
}