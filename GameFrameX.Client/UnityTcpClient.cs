using System.Net;
using GameFrameX.Extension;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.NetWork.Message;
using GameFrameX.Proto;
using GameFrameX.Proto.Proto;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.SuperSocket.ClientEngine;
using GameFrameX.Utility;
using ErrorEventArgs = GameFrameX.SuperSocket.ClientEngine.ErrorEventArgs;

namespace GameFrameX.Client;

public class UnityTcpClient
{
    private AsyncTcpSession _tcpClient;

    public void Entry(string[] args)
    {
        _tcpClient = new AsyncTcpSession();
        _tcpClient.Connected += OnTcpClientOnConnected; //成功连接到服务器
        _tcpClient.Closed += OnTcpClientOnClosed; //从服务器断开连接，当连接不成功时不会触发。
        _tcpClient.DataReceived += OnTcpClientOnDataReceived;
        _tcpClient.Error += OnTcpClientOnError;

        while (true)
        {
            Thread.Sleep(5000);

            if (!_tcpClient.IsConnected)
            {
                Console.WriteLine("未链接到服务器,开启重连");
                _tcpClient.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 29100));
                continue;
                // Console.WriteLine("链接到服务器结果：" + result);
                // if (result.ResultCode != ResultCode.Success)
                // {
                //     continue;
                // }
            }

            Console.WriteLine("--------------------------------");
            // for (int i = 0; i < 10; i++)
            {
                ReqHeartBeat req = new ReqHeartBeat
                {
                    Timestamp = TimeHelper.UnixTimeMilliseconds(),
                };
                SendToServer(req);

                // if (_count % 2 == 0)
                // {
                //     ReqLogin reqLogin = new ReqLogin
                //     {
                //         UserName = "admin",
                //         Password = "123456",
                //         UniqueId = _count
                //     };
                //     SendToServer(reqLogin);
                // }
            }
        }
    }

    private void SendToServer(MessageObject messageObject)
    {
        var buffer = Handler(messageObject);
        _tcpClient.Send(buffer);
    }

    private static void OnTcpClientOnError(object client, ErrorEventArgs e)
    {
        Console.WriteLine("客户端发生错误:" + e.Exception.Message);
    }

    private static void OnTcpClientOnClosed(object client, EventArgs e)
    {
        Console.WriteLine("客户端断开连接");
    }

    private static void OnTcpClientOnConnected(object client, EventArgs e)
    {
        Console.WriteLine("客户端成功连接到服务器");
    }

    private static void OnTcpClientOnDataReceived(object client, DataEventArgs e)
    {
        DecodeMessage(e.Data.ReadBytes(e.Offset, e.Length));
    }

    private static INetworkMessageHeader DecodeHeaderNetworkMessage(byte[] messageData)
    {
        var messageObjectHeader = (INetworkMessageHeader)ProtoBufSerializerHelper.Deserialize(messageData, typeof(MessageObjectHeader));
        return messageObjectHeader;
    }

    private static void DecodeMessage(byte[] data)
    {
        int offset = 0;

        // 消息总长度
        var totalLength = data.ReadInt(ref offset);
        // 消息头长度
        byte operationType = data.ReadByte(ref offset);
        byte zipFlag = data.ReadByte(ref offset);
        var UniqueId = data.ReadInt(ref offset);
        var MessageId = data.ReadInt(ref offset);
        // ushort headerLength = data.ReadByte(ref offset);
        // 消息头字节数组
        // var messageHeaderData = data.ReadBytes(ref offset, headerLength);
        // 消息对象头
        // var messageObjectHeader = DecodeHeaderNetworkMessage(messageHeaderData);
        // 消息内容
        var messageData = data.ReadBytes(ref offset, (int)(totalLength - 14));
        var messageType = MessageProtoHelper.GetMessageTypeById(MessageId);
        if (messageType != null)
        {
            var messageObject = (MessageObject)ProtoBufSerializerHelper.Deserialize(messageData, messageType);
            messageObject.SetMessageId(MessageId);
            messageObject.SetOperationType((MessageOperationType)operationType);
            messageObject.SetUniqueId(UniqueId);
            // Console.WriteLine($"客户端接收到信息：{messageObject.ToFormatMessageString()}");
        }
    }

    private static int _count;

    private static byte[] Handler(MessageObject message)
    {
        _count++;

        MessageProtoHelper.SetMessageId(message);
        message.SetOperationType(MessageProtoHelper.GetMessageOperationType(message));

        MessageObjectHeader messageObjectHeader = new MessageObjectHeader
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
        buffer.WriteByte((byte)messageObjectHeader.ZipFlag, ref offset);
        buffer.WriteInt(messageObjectHeader.UniqueId, ref offset);
        buffer.WriteInt(messageObjectHeader.MessageId, ref offset);
        // buffer.WriteBytesWithoutLength(header, ref offset);
        buffer.WriteBytesWithoutLength(messageData, ref offset);
        // Console.WriteLine($"客户端接发送信息：{message.ToFormatMessageString()}");
        return buffer;
    }

    const ushort InnerPackageHeaderLength = 14;
}