using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reflection;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.Utility;

namespace GameFrameX.NetWork.Message
{
    /// <summary>
    /// 协议消息处理器
    /// </summary>
    public static class MessageProtoHelper
    {
        private static readonly BidirectionalDictionary<int, Type> RequestDictionary = new BidirectionalDictionary<int, Type>();
        private static readonly BidirectionalDictionary<int, Type> AllMessageDictionary = new BidirectionalDictionary<int, Type>();
        private static readonly BidirectionalDictionary<int, Type> ResponseDictionary = new BidirectionalDictionary<int, Type>();
        private static readonly ConcurrentDictionary<Type, MessageOperationType> OperationType = new ConcurrentDictionary<Type, MessageOperationType>();
        private static readonly List<Type> HeartBeatList = new List<Type>();

        /// <summary>
        /// 获取消息ID,如果没有则返回0
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
        /// 获取消息类型，如果没有则返回null
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <returns></returns>
        public static Type GetMessageTypeById(int messageId)
        {
            AllMessageDictionary.TryGetValue(messageId, out var value);
            return value;
        }

        /// <summary>
        /// 获取消息操作类型
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <returns></returns>
        public static MessageOperationType GetMessageOperationType(Type type)
        {
            if (IsHeartbeat(type))
            {
                return MessageOperationType.HeartBeat;
            }

            if (OperationType.TryGetValue(type, out var value))
            {
                return value;
            }

            return MessageOperationType.None;
        }

        /// <summary>
        /// 设置消息ID
        /// </summary>
        /// <param name="message">消息对象</param>
        public static void SetMessageId(INetworkMessage message)
        {
            message.CheckNotNull(nameof(message));
            message.SetMessageId(GetMessageIdByType(message.GetType()));
        }

        /// <summary>
        /// 设置消息操作类型
        /// </summary>
        /// <param name="message">消息对象</param>
        public static void SetMessageOperationType(INetworkMessage message)
        {
            message.CheckNotNull(nameof(message));
            message.SetOperationType(GetMessageOperationType(message.GetType()));
        }

        /// <summary>
        /// 获取消息类型是否是心跳类型
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <returns></returns>
        public static bool IsHeartbeat(Type type)
        {
            return HeartBeatList.Contains(type);
        }

        /// <summary>
        /// 初始化所有协议对象
        /// </summary>
        /// <param name="assembly">协议所在程序集.将在此程序集中查找所有的类型进行识别</param>
        /// <param name="isClear">是否清理之前存在的缓存,默认为true</param>
        /// <exception cref="Exception">如果ID重复将会触发异常</exception>
        public static void Init(Assembly assembly, bool isClear = true)
        {
            assembly.CheckNotNull(nameof(assembly));
            if (isClear)
            {
                AllMessageDictionary.Clear();
                RequestDictionary.Clear();
                ResponseDictionary.Clear();
                HeartBeatList.Clear();
            }

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var messageTypeHandlerAttribute = type.GetCustomAttribute(typeof(MessageTypeHandlerAttribute));

                if (messageTypeHandlerAttribute is MessageTypeHandlerAttribute messageIdHandler)
                {
                    if (!AllMessageDictionary.TryAdd(messageIdHandler.MessageId, type))
                    {
                        RequestDictionary.TryGetValue(messageIdHandler.MessageId, out var value);
                        throw new Exception($"消息Id重复==>当前ID:{messageIdHandler.MessageId},已有ID类型:{value.FullName}");
                    }

                    OperationType.TryAdd(type, messageIdHandler.OperationType);

                    if (type.IsImplWithInterface(typeof(IHeartBeatMessage)))
                    {
                        if (HeartBeatList.Contains(type))
                        {
                            LogHelper.Error($"心跳消息重复==>类型:{type.FullName}");
                        }
                        else
                        {
                            HeartBeatList.Add(type);
                        }
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
                    else if (type.IsImplWithInterface(typeof(IResponseMessage)) || type.IsImplWithInterface(typeof(INotifyMessage)))
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
        }
    }
}