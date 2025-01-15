using System.Buffers;
using System.Net;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.Proto.Proto;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.SuperSocket.ClientEngine;
using GameFrameX.SuperSocket.ProtoBase;
using GameFrameX.Utility;
using ErrorEventArgs = GameFrameX.SuperSocket.ClientEngine.ErrorEventArgs;

namespace GameFrameX.Bot;

public class BotTcpClient
{
    private const string ServerHost = "127.0.0.1";
    private const int ServerPort = 29100;
    private const int DelayTimes = 1000;
    private const int MaxRetryCount = 5;
    private readonly AsyncTcpSession m_TcpClient;
    private int m_RetryCount;
    private int m_RetryDelay = 5000;
    private readonly BotTcpClientEvent m_BotTcpClientEvent;

    private const ushort InnerPackageHeaderLength = 14;
    private static int _count;

    public BotTcpClient(BotTcpClientEvent clientEvent)
    {
        m_BotTcpClientEvent = clientEvent;
        m_TcpClient = new AsyncTcpSession();
        m_TcpClient.Connected += OnMTcpClientOnConnected;
        m_TcpClient.Closed += OnMTcpClientOnClosed;
        m_TcpClient.DataReceived += OnMTcpClientOnDataReceived;
        m_TcpClient.Error += OnMTcpClientOnError;
    }

    private const ushort PackageHeaderLength = sizeof(uint) + sizeof(byte) + sizeof(byte) + sizeof(int) + sizeof(int);

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

    private void SendHeartBeat()
    {
        ReqHeartBeat req = new ReqHeartBeat
        {
            Timestamp = TimeHelper.UnixTimeMilliseconds(),
        };
        SendToServer(req);
    }

    public void SendToServer(MessageObject messageObject)
    {
        var buffer = Handler(messageObject);
        if (buffer != null)
        {
            m_TcpClient.Send(buffer);
        }
    }

    private void OnMTcpClientOnError(object? client, ErrorEventArgs e)
    {
        LogHelper.Info("客户端发生错误:" + e.Exception.Message);
        m_BotTcpClientEvent.OnErrorCallback(e);
    }

    private void OnMTcpClientOnClosed(object? client, EventArgs e)
    {
        LogHelper.Info("客户端断开连接");
        m_BotTcpClientEvent.OnClosedCallback();
    }

    private void OnMTcpClientOnConnected(object? client, EventArgs e)
    {
        LogHelper.Info("客户端成功连接到服务器");
        m_BotTcpClientEvent.OnConnectedCallback();
    }

    private void OnMTcpClientOnDataReceived(object? client, DataEventArgs e)
    {
        DecodeMessage(e.Data.ReadBytes(e.Offset, e.Length));
    }

    private void DecodeMessage(byte[] data)
    {
        var offset = 0;

        // 消息总长度
        var totalLength = data.ReadInt(ref offset);
        // 消息头长度
        var operationType = data.ReadByte(ref offset);
        var zipFlag = data.ReadByte(ref offset);
        var UniqueId = data.ReadInt(ref offset);
        var MessageId = data.ReadInt(ref offset);
        // ushort headerLength = data.ReadByte(ref offset);
        // 消息头字节数组
        // var messageHeaderData = data.ReadBytes(ref offset, headerLength);
        // 消息对象头
        // var messageObjectHeader = DecodeHeaderNetworkMessage(messageHeaderData);
        // 消息内容
        var messageData = data.ReadBytes(ref offset, totalLength - 14);
        var messageType = MessageProtoHelper.GetMessageTypeById(MessageId);
        if (messageType != null)
        {
            var messageObject = (MessageObject)ProtoBufSerializerHelper.Deserialize(messageData, messageType);
            messageObject.SetMessageId(MessageId);
            messageObject.SetOperationType((MessageOperationType)operationType);
            messageObject.SetUniqueId(UniqueId);
            m_BotTcpClientEvent.OnReceiveMsgCallback(messageObject);
        }
    }

    private static byte[] Handler(MessageObject message)
    {
        _count++;

        MessageProtoHelper.SetMessageId(message);
        message.SetOperationType(MessageProtoHelper.GetMessageOperationType(message));

        var messageObjectHeader = new MessageObjectHeader
        {
            OperationType = message.OperationType,
            UniqueId = message.UniqueId,
            MessageId = message.MessageId,
        };
        var header = ProtoBufSerializerHelper.Serialize(messageObjectHeader);
        var messageData = ProtoBufSerializerHelper.Serialize(message);
        var totalLength = messageData.Length + InnerPackageHeaderLength;
        var buffer = new byte[totalLength];
        var offset = 0;
        buffer.WriteInt(totalLength, ref offset);
        buffer.WriteByte((byte)messageObjectHeader.OperationType, ref offset);
        buffer.WriteByte(messageObjectHeader.ZipFlag, ref offset);
        buffer.WriteInt(messageObjectHeader.UniqueId, ref offset);
        buffer.WriteInt(messageObjectHeader.MessageId, ref offset);
        // buffer.WriteBytesWithoutLength(header, ref offset);
        buffer.WriteBytesWithoutLength(messageData, ref offset);
        // Console.WriteLine($"客户端接发送信息：{message.ToFormatMessageString()}");
        return buffer;
    }
}