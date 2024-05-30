using System;
using System.Reflection;
using System.Text;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Proto
{
    /// <summary>
    /// 协议消息处理器
    /// </summary>
    public static class ProtoMessageIdHandler
    {
        private static readonly BidirectionalDictionary<int, Type> RequestDictionary = new BidirectionalDictionary<int, Type>();
        private static readonly BidirectionalDictionary<int, Type> AllMessageDictionary = new BidirectionalDictionary<int, Type>();
        private static readonly BidirectionalDictionary<int, Type> ResponseDictionary = new BidirectionalDictionary<int, Type>();

        /// <summary>
        /// 获取消息ID
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <returns></returns>
        public static int GetMessageIdByType(Type type)
        {
            if (AllMessageDictionary.TryGetKey(type, out var value))
            {
                return value;
            }

            return 0;
        }

        /// <summary>
        /// 获取消息类型
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <returns></returns>
        public static Type GetMessageTypeById(int messageId)
        {
            AllMessageDictionary.TryGetValue(messageId, out var value);
            return value;
        }


        /// <summary>
        /// 初始化所有协议对象
        /// </summary>
        public static void Init()
        {
            AllMessageDictionary.Clear();
            RequestDictionary.Clear();
            ResponseDictionary.Clear();
            var assembly = typeof(ProtoMessageIdHandler).Assembly;
            var types = assembly.GetTypes();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute(typeof(MessageTypeHandlerAttribute));
                if (attribute == null)
                {
                    continue;
                }


                // SerializerHelper.Register(type);
                if (attribute is MessageTypeHandlerAttribute messageIdHandler)
                {
                    stringBuilder.AppendLine($"ID:{messageIdHandler.MessageId},类型: {type}");

                    if (!AllMessageDictionary.TryAdd(messageIdHandler.MessageId, type))
                    {
                        RequestDictionary.TryGetValue(messageIdHandler.MessageId, out var value);
                        throw new Exception($"消息Id重复==>当前ID:{messageIdHandler.MessageId},已有ID类型:{value.FullName}");
                    }

                    if (type.IsImplWithInterface(typeof(IRequestMessage)))
                    {
                        // 请求
                        if (!RequestDictionary.TryAdd(messageIdHandler.MessageId, type))
                        {
                            RequestDictionary.TryGetValue(messageIdHandler.MessageId, out var value);
                            LogHelper.Error($"请求Id重复==>当前ID:{messageIdHandler.MessageId},已有ID类型:{value.FullName}");
                        }
                    }
                    else if (type.IsImplWithInterface(typeof(IResponseMessage)))
                    {
                        // 返回
                        if (!ResponseDictionary.TryAdd(messageIdHandler.MessageId, type))
                        {
                            ResponseDictionary.TryGetValue(messageIdHandler.MessageId, out var value);
                            LogHelper.Error($"返回Id重复==>当前ID:{messageIdHandler.MessageId},已有ID类型:{value.FullName}");
                        }
                    }
                }
            }

            // LogHelper.Debug(stringBuilder.ToString());
            // LogHelper.Info(" 注册消息ID类型: 结束");
        }
    }
}