// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using System.Collections.Concurrent;
using System.Reflection;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Localization.Core;

namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 协议消息处理器
/// </summary>
public static class MessageProtoHelper
{
    private static readonly BidirectionalDictionary<int, Type> RequestDictionary = new();
    private static readonly BidirectionalDictionary<int, Type> AllMessageDictionary = new();
    private static readonly BidirectionalDictionary<int, Type> ResponseDictionary = new();
    private static readonly ConcurrentDictionary<Type, byte> OperationType = new();
    private static readonly List<Type> HeartBeatList = new();

    /// <summary>
    /// 获取消息ID,如果没有找到则返回 -1
    /// </summary>
    /// <param name="message">消息对象</param>
    /// <returns></returns>
    public static int GetMessageIdByType(INetworkMessage message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        var messageType = message.GetType();
        return GetMessageIdByType(messageType);
    }

    /// <summary>
    /// 获取消息ID,如果没有找到则返回 -1
    /// </summary>
    /// <param name="type">消息类型</param>
    /// <returns></returns>
    public static int GetMessageIdByType(Type type)
    {
        if (AllMessageDictionary.TryGetKey(type, out var value))
        {
            return value;
        }

        return -1;
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
    /// <param name="message">消息对象</param>
    /// <returns></returns>
    public static byte GetMessageOperationType(INetworkMessage message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        var messageType = message.GetType();
        return GetMessageOperationType(messageType);
    }

    /// <summary>
    /// 获取消息操作类型
    /// </summary>
    /// <param name="type">消息类型</param>
    /// <returns></returns>
    public static byte GetMessageOperationType(Type type)
    {
        if (IsHeartbeat(type))
        {
            return (byte)MessageOperationType.HeartBeat;
        }

        if (OperationType.TryGetValue(type, out var value))
        {
            return value;
        }

        return default;
    }

    /// <summary>
    /// 设置消息ID和操作类型
    /// </summary>
    /// <param name="message">消息对象</param>
    public static void SetMessageId(INetworkMessage message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        var messageType = message.GetType();
        message.SetMessageId(GetMessageIdByType(messageType));
    }

    /// <summary>
    /// 获取消息类型是否是心跳类型
    /// </summary>
    /// <param name="message">消息对象</param>
    /// <returns></returns>
    public static bool IsHeartbeat(INetworkMessage message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        var messageType = message.GetType();
        return IsHeartbeat(messageType);
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
    /// <param name="assemblies">协议所在程序集集合.将在集合中查找所有的类型进行识别</param>
    /// <exception cref="Exception">如果ID重复将会触发异常</exception>
    public static void Init(params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies, nameof(assemblies));

        AllMessageDictionary.Clear();
        RequestDictionary.Clear();
        ResponseDictionary.Clear();
        HeartBeatList.Clear();
        OperationType.Clear();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var messageTypeHandlerAttribute = type.GetCustomAttribute(typeof(MessageTypeHandlerAttribute));

                if (messageTypeHandlerAttribute is MessageTypeHandlerAttribute messageTypeHandler)
                {
                    if (!AllMessageDictionary.TryAdd(messageTypeHandler.MessageId, type))
                    {
                        RequestDictionary.TryGetValue(messageTypeHandler.MessageId, out var value);
                        throw new ArgumentAlreadyException(LocalizationService.GetString(
                            GameFrameX.Localization.Keys.NetWorkAbstractions.MessageIdDuplicate,
                            messageTypeHandler.MessageId,
                            value?.FullName ?? "Unknown"));
                    }

                    OperationType.TryAdd(type, messageTypeHandler.OperationType);

                    if (type.IsImplWithInterface(typeof(IHeartBeatMessage)))
                    {
                        if (HeartBeatList.Contains(type))
                        {
                            LogHelper.Error(LocalizationService.GetString(
                                GameFrameX.Localization.Keys.NetWorkAbstractions.HeartbeatMessageDuplicate,
                                type?.FullName ?? "Unknown"));
                        }
                        else
                        {
                            HeartBeatList.Add(type);
                        }
                    }

                    if (type.IsImplWithInterface(typeof(IRequestMessage)))
                    {
                        // 请求
                        if (!RequestDictionary.TryAdd(messageTypeHandler.MessageId, type))
                        {
                            RequestDictionary.TryGetValue(messageTypeHandler.MessageId, out var value);
                            throw new ArgumentAlreadyException(LocalizationService.GetString(
                                GameFrameX.Localization.Keys.NetWorkAbstractions.RequestIdDuplicate,
                                messageTypeHandler.MessageId,
                                value?.FullName ?? "Unknown"));
                        }
                    }
                    else if (type.IsImplWithInterface(typeof(IResponseMessage)) || type.IsImplWithInterface(typeof(INotifyMessage)))
                    {
                        // 返回
                        if (!ResponseDictionary.TryAdd(messageTypeHandler.MessageId, type))
                        {
                            ResponseDictionary.TryGetValue(messageTypeHandler.MessageId, out var value);
                            throw new ArgumentAlreadyException(LocalizationService.GetString(
                                GameFrameX.Localization.Keys.NetWorkAbstractions.ResponseIdDuplicate,
                                messageTypeHandler.MessageId,
                                value?.FullName ?? "Unknown"));
                        }
                    }
                }
            }
        }
    }
}