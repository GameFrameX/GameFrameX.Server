using System.Net;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Message;
using GameFrameX.NetWork.Messages;
using GameFrameX.Proto.Proto;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.SuperSocket.ClientEngine;
using GameFrameX.Utility;
using GameFrameX.Utility.Extensions;
using GameFrameX.Foundation.Logger;
using ErrorEventArgs = GameFrameX.SuperSocket.ClientEngine.ErrorEventArgs;

namespace GameFrameX.Client.Bot;

/// <summary>
/// 机器人TCP客户端事件结构体,包含各种回调事件
/// </summary>
public struct BotTcpClientEvent
{
    /// <summary>
    /// 连接成功时的回调
    /// </summary>
    public Action OnConnectedCallback;
    
    /// <summary>
    /// 连接关闭时的回调
    /// </summary>
    public Action OnClosedCallback;
    
    /// <summary>
    /// 发生错误时的回调
    /// </summary>
    public Action<ErrorEventArgs> OnErrorCallback;
    
    /// <summary>
    /// 接收到消息时的回调
    /// </summary>
    public Action<MessageObject> OnReceiveMsgCallback;
}

/// <summary>
/// 机器人TCP客户端类,用于处理与服务器的TCP连接和消息收发
/// </summary>
public sealed class BotTcpClient
{
    private const string ServerHost = "127.0.0.1";
    private const int ServerPort = 29100;
    private const int DelayTimes = 1000;
    private const int MaxRetryCount = 5;
    private readonly AsyncTcpSession m_TcpClient;
    private int m_RetryCount;
    private int m_RetryDelay = 5000;
    private readonly BotTcpClientEvent m_BotTcpClientEvent;
    private readonly IMessageDecompressHandler messageDecompressHandler;
    private readonly IMessageCompressHandler messageCompressHandler;

    private const ushort InnerPackageHeaderLength = 14;

    /// <summary>
    /// 初始化机器人TCP客户端
    /// </summary>
    /// <param name="clientEvent">客户端事件回调结构体</param>
    public BotTcpClient(BotTcpClientEvent clientEvent)
    {
        m_BotTcpClientEvent = clientEvent;
        m_TcpClient = new AsyncTcpSession();
        m_TcpClient.Connected += OnMTcpClientOnConnected;
        m_TcpClient.Closed += OnMTcpClientOnClosed;
        m_TcpClient.DataReceived += OnMTcpClientOnDataReceived;
        m_TcpClient.Error += OnMTcpClientOnError;
        messageDecompressHandler = new DefaultMessageDecompressHandler();
        messageCompressHandler = new DefaultMessageCompressHandler();
    }

    /// <summary>
    /// 启动客户端并尝试连接服务器
    /// </summary>
    /// <returns>异步任务</returns>
    public async Task EntryAsync()
    {
        while (true)
        {
            if (!m_TcpClient.IsConnected && !m_TcpClient.IsInConnecting)
            {
                LogHelper.Info("尝试连接到服务器...");
                m_TcpClient.Connect(new IPEndPoint(IPAddress.Parse(ServerHost), ServerPort));
                await Task.Delay(DelayTimes);
                if (m_TcpClient.IsConnected || m_TcpClient.IsInConnecting)
                {
                    continue;
                }

                if (m_RetryCount < MaxRetryCount)
                {
                    LogHelper.Info($"未连接到服务器, 尝试重连 (尝试次数: {m_RetryCount + 1}/{MaxRetryCount})");
                    m_TcpClient.Connect(new IPEndPoint(IPAddress.Parse(ServerHost), ServerPort));
                    m_RetryCount++;
                    await Task.Delay(m_RetryDelay);
                    m_RetryDelay *= 2;
                }
                else
                {
                    LogHelper.Info("重连次数已达到上限，停止尝试。");
                    break;
                }
            }
            else
            {
                m_RetryCount = 0;
                SendHeartBeat();
                await Task.Delay(5000);
            }
        }
    }

    /// <summary>
    /// 发送心跳包到服务器
    /// </summary>
    private void SendHeartBeat()
    {
        ReqHeartBeat req = new ReqHeartBeat
        {
            Timestamp = TimeHelper.UnixTimeMilliseconds(),
        };
        SendToServer(req);
    }

    /// <summary>
    /// 发送消息到服务器
    /// </summary>
    /// <param name="messageObject">要发送的消息对象</param>
    public void SendToServer(MessageObject messageObject)
    {
        var buffer = Handler(messageObject);
        if (buffer != null)
        {
            m_TcpClient.Send(buffer);
        }
    }

    /// <summary>
    /// 处理客户端错误事件
    /// </summary>
    private void OnMTcpClientOnError(object client, ErrorEventArgs e)
    {
        LogHelper.Info("客户端发生错误:" + e.Exception.Message);
        m_BotTcpClientEvent.OnErrorCallback(e);
    }

    /// <summary>
    /// 处理客户端连接关闭事件
    /// </summary>
    private void OnMTcpClientOnClosed(object client, EventArgs e)
    {
        LogHelper.Info("客户端断开连接");
        m_BotTcpClientEvent.OnClosedCallback();
    }

    /// <summary>
    /// 处理客户端连接成功事件
    /// </summary>
    private void OnMTcpClientOnConnected(object client, EventArgs e)
    {
        LogHelper.Info("客户端成功连接到服务器");
        m_BotTcpClientEvent.OnConnectedCallback();
    }

    /// <summary>
    /// 处理接收到数据事件
    /// </summary>
    private void OnMTcpClientOnDataReceived(object client, DataEventArgs e)
    {
        DecodeMessage(e.Data.ReadBytes(e.Offset, e.Length));
    }

    /// <summary>
    /// 解码接收到的消息数据
    /// </summary>
    /// <param name="data">接收到的字节数据</param>
    private void DecodeMessage(byte[] data)
    {
        var offset = 0;

        // 消息总长度
        var totalLength = data.ReadInt(ref offset);
        // 消息头长度
        var operationType = data.ReadByte(ref offset);
        var zipFlag = data.ReadByte(ref offset);
        var uniqueId = data.ReadInt(ref offset);
        var messageId = data.ReadInt(ref offset);
        var messageData = data.ReadBytes(ref offset, totalLength - InnerPackageHeaderLength);
        var messageType = MessageProtoHelper.GetMessageTypeById(messageId);
        if (messageType != null)
        {
            if (zipFlag > 0)
            {
                // 消息解压缩
                messageData = messageDecompressHandler.Handler(messageData);
            }

            var messageObject = (MessageObject)ProtoBufSerializerHelper.Deserialize(messageData, messageType);
            messageObject.SetMessageId(messageId);
            messageObject.SetOperationType((MessageOperationType)operationType);
            messageObject.SetUniqueId(uniqueId);
            m_BotTcpClientEvent.OnReceiveMsgCallback(messageObject);
        }
    }

    /// <summary>
    /// 处理要发送的消息,将消息对象转换为字节数组
    /// </summary>
    /// <param name="message">要处理的消息对象</param>
    /// <returns>处理后的字节数组</returns>
    private byte[] Handler(MessageObject message)
    {
        MessageProtoHelper.SetMessageId(message);
        message.SetOperationType(MessageProtoHelper.GetMessageOperationType(message));

        var messageData = ProtoBufSerializerHelper.Serialize(message);
        byte zipFlag = 0;
        if (messageData.Length > 512)
        {
            messageData = messageCompressHandler.Handler(messageData);
            zipFlag = 1;
        }

        var totalLength = messageData.Length + InnerPackageHeaderLength;
        var buffer = new byte[totalLength];
        var offset = 0;
        buffer.WriteInt(totalLength, ref offset);
        buffer.WriteByte((byte)message.OperationType, ref offset);
        buffer.WriteByte(zipFlag, ref offset);
        buffer.WriteInt(message.UniqueId, ref offset);
        buffer.WriteInt(message.MessageId, ref offset);
        buffer.WriteBytesWithoutLength(messageData, ref offset);
        return buffer;
    }
}