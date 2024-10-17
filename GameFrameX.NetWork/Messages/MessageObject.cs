using System.Text;
using GameFrameX.Extension;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.Utility;
using Newtonsoft.Json;
using ProtoBuf;

namespace GameFrameX.NetWork.Messages
{
    /// <summary>
    /// 消息对象
    /// </summary>
    [ProtoContract]
    public abstract class MessageObject : INetworkMessage
    {
        /*/// <summary>
        /// 单位id
        /// </summary>
        [JsonIgnore]
        public int UniId { get; set; }*/

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
        public int UniqueId { get; set; }

        /// <summary>
        /// 消息业务类型
        /// </summary>
        [JsonIgnore]
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
            UniqueId = IdGenerator.GetNextUniqueIntId();
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
        /// 获取格式化后的消息字符串
        /// </summary>
        /// <returns></returns>
        public string ToFormatMessageString()
        {
            StringBuilder stringBuilder = StringBuilderCache.Acquire();
            stringBuilder.Clear();
            stringBuilder.AppendLine();
            // 向下的箭头
            stringBuilder.AppendLine($"{'\u2193'.RepeatChar(120)}");
            // 消息的头部信息
            // 消息类型
            stringBuilder.Append($"---MessageType:[{GetType().Name.CenterAlignedText(20)}]");
            // 消息ID
            stringBuilder.Append($"--MsgId:[{MessageId.ToString().CenterAlignedText(10)}]({MessageIdUtility.GetMainId(MessageId).ToString().CenterAlignedText(5)},{MessageIdUtility.GetSubId(MessageId).ToString().CenterAlignedText(5)})");
            // 操作类型
            stringBuilder.Append($"--OpType:[{OperationType.ToString().CenterAlignedText(12)}]");
            // 唯一ID
            stringBuilder.Append($"--UniqueId:[{UniqueId.ToString().CenterAlignedText(12)}]---");
            stringBuilder.AppendLine();
            // 消息的内容 分割
            stringBuilder.AppendLine();
            // 消息内容
            stringBuilder.AppendLine($"{ToJsonString()}");
            // 向上的箭头
            stringBuilder.AppendLine($"{'\u2191'.RepeatChar(120)}");
            stringBuilder.AppendLine();
            return StringBuilderCache.GetStringAndRelease(stringBuilder);
        }

        /// <summary>
        /// 获取JSON格式化后的消息字符串
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            return JsonHelper.SerializeFormat(this);
        }

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